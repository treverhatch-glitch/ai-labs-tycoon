using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Loans against chips: borrow up to a share of what the owned GPUs fetch, pay weekly, lose the chips if the payments stop.</summary>
public class LoanTests
{
    static Game Lab(int gpus = 6, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            if (tweak != null) tweak(c);
        });
        g.Build("server-closet", 4, 4);
        g.Me.Cash = 1_000_000_000_000;
        if (gpus > 0) g.Do(Command.BuyGpus(g.Id, gpus, HardwareBatch.Hq));
        return g;
    }

    static int Take(Game g, int permille = 1000)
    {
        CommandResult r = g.Do(Command.TakeLoan(g.Id, permille));
        return r.CreatedId;
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the data

    [Fact]
    public void The_shipped_numbers_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.InRange(c.Loans.LoanToValuePermille, 1, 999);
        Assert.True(c.Loans.InterestPermille > 0 && c.Loans.TermWeeks > 1 && c.Loans.DefaultAfterMisses > 1);
    }

    [Fact]
    public void The_validator_catches_bad_loan_data()
    {
        AssertProblem(Problems(c => c.Loans.LoanToValuePermille = 0), "loanToValuePermille");
        AssertProblem(Problems(c => c.Loans.LoanToValuePermille = 1001), "loanToValuePermille");
        AssertProblem(Problems(c => c.Loans.TermWeeks = 0), "termWeeks");
        AssertProblem(Problems(c => c.Loans.InterestPermille = -1), "interestPermille");
        AssertProblem(Problems(c => c.Loans.MinPrincipal = 0), "minPrincipal");
        AssertProblem(Problems(c => c.Loans.MaxOpen = 0), "maxOpen");
        AssertProblem(Problems(c => c.Loans.MissPenaltyPermille = 1001), "missPenaltyPermille");
        AssertProblem(Problems(c => c.Loans.DefaultAfterMisses = 0), "defaultAfterMisses");
    }

    // ---- borrowing

    [Fact]
    public void The_lab_can_borrow_a_share_of_what_its_gpus_would_fetch()
    {
        Game g = Lab();
        long value = LabQueries.FleetResaleValue(g.C, g.State, g.Me);
        Assert.True(value > 0);
        Assert.Equal(value, Loans.Collateral(g.C, g.State, g.Me));
        long room = MathI.MulPermille(value, g.C.Loans.LoanToValuePermille);
        Assert.Equal(room, Loans.Capacity(g.C, g.State, g.Me));
    }

    [Fact]
    public void Taking_a_loan_pays_out_the_principal_and_owes_it_with_interest()
    {
        Game g = Lab();
        long start = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        long cash = g.Me.Cash;
        long room = Loans.Capacity(g.C, g.State, g.Me);
        int id = Take(g, 500);
        long principal = MathI.MulPermille(room, 500);
        Loan loan = Loans.Find(g.Me, id);
        Assert.NotNull(loan);
        Assert.Equal(principal, loan.Principal);
        Assert.Equal(cash + principal, g.Me.Cash);
        Assert.Equal(principal + MathI.MulPermille(principal, g.C.Loans.InterestPermille), loan.Balance);
        Assert.Equal((loan.Balance + g.C.Loans.TermWeeks - 1) / g.C.Loans.TermWeeks, loan.Payment);
        Assert.Equal(0, loan.Misses);
        Assert.Equal(start, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoanTaken);
        Assert.Equal(id, e.SubjectId);
        Assert.Equal(principal, e.Amount);
        Assert.Contains("borrowed", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void What_is_already_owed_comes_off_what_more_can_be_borrowed()
    {
        Game g = Lab();
        long before = Loans.Capacity(g.C, g.State, g.Me);
        int id = Take(g, 500);
        Loan loan = Loans.Find(g.Me, id);
        Assert.Equal(before - loan.Balance, Loans.Capacity(g.C, g.State, g.Me));
        Assert.Equal(loan.Balance, Loans.Outstanding(g.Me));
    }

    [Fact]
    public void Bad_orders_are_refused_and_change_nothing()
    {
        Game none = Lab(0);
        none.Refused(Command.TakeLoan(none.Id, 1000), CommandError.LoanLimit);

        Game g = Lab();
        g.Refused(Command.TakeLoan(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.TakeLoan(g.Id, 1001), CommandError.InvalidArgument);
        g.Refused(Command.RepayLoan(g.Id, 12345), CommandError.UnknownId);

        Game tiny = Lab(6, c => c.Loans.MinPrincipal = 1_000_000_000_000_000);
        tiny.Refused(Command.TakeLoan(tiny.Id, 1000), CommandError.LoanLimit);

        Game many = Lab(6, c =>
        {
            c.Loans.MaxOpen = 2;
            c.Loans.MinPrincipal = 1;
        });
        Take(many, 100);
        Take(many, 100);
        many.Refused(Command.TakeLoan(many.Id, 100), CommandError.LoanLimit);
    }

    [Fact]
    public void Only_the_players_lab_borrows()
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = TestData.Config().Rivals.Take(1).Select(r => r.Id).ToList() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        g.Refused(Command.TakeLoan(rival.Id, 500), CommandError.InvalidArgument);
        g.Refused(Command.RepayLoan(rival.Id, 1), CommandError.InvalidArgument);
    }

    // ---- paying

    [Fact]
    public void The_payments_come_out_each_week_until_it_is_paid_off()
    {
        Game g = Lab(6, c => c.Loans.TermWeeks = 4);
        int id = Take(g, 1000);
        Loan loan = Loans.Find(g.Me, id);
        long owed = loan.Balance;
        long payment = loan.Payment;
        long start = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Weeks(1);
        Assert.Equal(owed - payment, loan.Balance);
        g.Weeks(3);
        Assert.Null(Loans.Find(g.Me, id));
        Assert.Equal(start, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoanRepaid);
        Assert.Equal("term", e.Ref);
        Assert.Equal(id, e.SubjectId);
        Assert.Contains("paid off", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_lab_that_cannot_pay_misses_and_owes_a_penalty_and_logs_the_first_miss_once()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        Loan loan = Loans.Find(g.Me, id);
        long payment = loan.Payment;
        long balance = loan.Balance;
        g.Me.Cash = 0;
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, loan.Misses);
        Assert.Equal(balance + MathI.MulPermille(payment, g.C.Loans.MissPenaltyPermille), loan.Balance);
        Assert.Equal(0, g.Me.Cash);
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, loan.Misses);
        Assert.Equal(1, g.State.Events.Count(x => x.Kind == EventKind.LoanMissed));
        Assert.Contains("missed", EventText.Describe(g.C, g.State, g.State.Events.Last(x => x.Kind == EventKind.LoanMissed)));
    }

    [Fact]
    public void A_payment_made_clears_the_run_of_misses()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        Loan loan = Loans.Find(g.Me, id);
        long cash = g.Me.Cash;
        g.Me.Cash = 0;
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, loan.Misses);
        g.Me.Cash = cash;
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, loan.Misses);
    }

    // ---- default

    [Fact]
    public void Enough_misses_in_a_row_and_the_lender_takes_the_chips()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        int owned = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        g.Me.Cash = 0;
        for (int i = 0; i < g.C.Loans.DefaultAfterMisses; i++) Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Null(Loans.Find(g.Me, id));
        Assert.True(LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq) < owned);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoanDefaulted);
        Assert.Equal(owned - LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq), e.SubjectId);
        Assert.Equal(id.ToString(), e.Ref);
        Assert.Contains("lender took", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_lender_takes_only_what_covers_the_debt_and_writes_off_the_rest()
    {
        Game g = Lab(6, c => { c.Loans.LoanToValuePermille = 1000; c.Loans.MinPrincipal = 1; });
        int id = Take(g, 100);
        Loan loan = Loans.Find(g.Me, id);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long each = LabQueries.ResalePerUnit(g.C, gen, g.Tick);
        int owned = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        Assert.True(loan.Balance < each * owned / 2, "a small loan");
        g.Me.Cash = 0;
        for (int i = 0; i < g.C.Loans.DefaultAfterMisses; i++) Loans.RunWeek(g.C, g.State, g.Me);
        int left = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        Assert.True(left > 0, "chips were left");
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoanDefaulted);
        Assert.Equal(0, e.Amount);
        Assert.Equal(owned - left, e.SubjectId);
        Assert.True(e.SubjectId * each >= loan.Balance);
    }

    [Fact]
    public void A_debt_bigger_than_the_chips_is_written_off_after_they_all_go()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        Loan loan = Loans.Find(g.Me, id);
        loan.Balance *= 5;
        g.Me.Cash = 0;
        for (int i = 0; i < g.C.Loans.DefaultAfterMisses; i++) Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq));
        Assert.True(g.State.Events.Last(x => x.Kind == EventKind.LoanDefaulted).Amount > 0);
        Assert.Empty(g.Me.Loans);
    }

    [Fact]
    public void The_most_valuable_chips_go_first()
    {
        Game g = Lab(0);
        GpuGenerationConfig oldest = g.C.Compute.GpuGenerations.First();
        GpuGenerationConfig newest = g.C.Compute.GpuGenerations.Last();
        Assert.NotEqual(oldest.IdValue, newest.IdValue);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, oldest.IdValue, 4);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, newest.IdValue, 4);
        g.Me.Loans.Add(new Loan { Id = 900, Principal = 1, Balance = 1, Payment = 1, Misses = 2 });
        g.Me.Cash = 0;
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(4, g.Me.Fleet.Where(b => b.GpuGenId == oldest.IdValue).Sum(b => b.Count));
        Assert.Equal(3, g.Me.Fleet.Where(b => b.GpuGenId == newest.IdValue).Sum(b => b.Count));
    }

    // ---- repaying early, and chips that lose value

    [Fact]
    public void Paying_off_early_costs_what_is_owed_and_frees_the_room()
    {
        Game g = Lab();
        long before = Loans.Capacity(g.C, g.State, g.Me);
        int id = Take(g, 1000);
        Loan loan = Loans.Find(g.Me, id);
        long owed = loan.Balance;
        long cash = g.Me.Cash;
        g.Do(Command.RepayLoan(g.Id, id));
        Assert.Equal(cash - owed, g.Me.Cash);
        Assert.Null(Loans.Find(g.Me, id));
        Assert.Equal(before, Loans.Capacity(g.C, g.State, g.Me));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoanRepaid);
        Assert.Equal("early", e.Ref);
        Assert.Contains("early", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Paying_off_needs_the_cash()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        g.Me.Cash = 1;
        g.Refused(Command.RepayLoan(g.Id, id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void When_the_chips_lose_value_the_loan_stays_and_nothing_more_can_be_borrowed()
    {
        Game g = Lab();
        int id = Take(g, 1000);
        Assert.Equal(0, Loans.Capacity(g.C, g.State, g.Me));
        int owned = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        Fleet.Take(g.Me, g.Me.Fleet.First(), owned - 1);
        Assert.Equal(0, Loans.Capacity(g.C, g.State, g.Me));
        Assert.NotNull(Loans.Find(g.Me, id));
        g.Refused(Command.TakeLoan(g.Id, 1000), CommandError.LoanLimit);
    }

    // ---- the rest

    [Fact]
    public void Going_bankrupt_wipes_the_loans()
    {
        Game g = Lab();
        Take(g, 500);
        Finance.GoBankrupt(g.State, g.Me);
        Assert.Empty(g.Me.Loans);
    }

    [Fact]
    public void Nobody_borrows_on_their_own()
    {
        Game g = Lab();
        g.Autopilot(52);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.LoanTaken);
        Assert.Empty(g.Me.Loans);
    }

    [Fact]
    public void A_save_keeps_the_loans_and_an_older_save_has_none()
    {
        Game g = Lab(6, c => c.Loans.MinPrincipal = 1);
        Take(g, 500);
        Take(g, 100);
        g.Weeks(2);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        string json = g.Sim.Save();
        Simulation again = Simulation.Load(g.C, json);
        Assert.Equal(2, again.State.Labs.First(l => l.Id == g.Id).Loans.Count);

        string old = SaveSchemaPretend.At(g.C, 40, json);
        Assert.DoesNotContain("\"loans\"", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => Assert.Empty(l.Loans));
    }

    [Fact]
    public void The_save_checker_notices_a_loan_that_owes_nothing()
    {
        Game g = Lab();
        Take(g, 500);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.Loans[0].Balance = 0;
        Assert.NotEmpty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_chat_has_a_line_for_a_default()
    {
        Assert.Contains(TestData.Config().Chat.Lines, l => l.Kind == "LoanDefaulted");
    }
}
