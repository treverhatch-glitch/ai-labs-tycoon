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

/// <summary>Restructuring a loan: a longer term and a lower payment for a fee, a dent in the lab's name and a clean sheet of missed payments.</summary>
public class RestructureTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        });
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        return g;
    }

    static Loan Borrow(Game g, int permille = 1000)
    {
        int id = g.Do(Command.TakeLoan(g.Id, permille)).CreatedId;
        return Loans.Find(g.Me, id);
    }

    static List<GameEvent> Of(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.LoanRestructured).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Restructure.Enabled);
        Assert.Equal(2000, c.Restructure.StretchPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Restructure.Enabled = 2;
        c.Restructure.StretchPermille = 999;
        c.Restructure.FeePermille = 501;
        c.Restructure.ReputationLoss = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "stretchPermille", "feePermille", "reputationLoss" })
            Assert.Contains(problems, p => p.Contains("restructure." + key));
    }

    [Fact]
    public void A_loan_nobody_restructures_is_exactly_as_it_was()
    {
        Game g = Lab();
        Loan loan = Borrow(g);
        long payment = loan.Payment;
        g.Weeks(10);
        Assert.Equal(payment, loan.Payment);
        Assert.Empty(Of(g));
    }

    [Fact]
    public void Restructuring_halves_the_payment_for_a_fee_added_to_the_balance_and_costs_reputation()
    {
        Game g = Lab();
        Loan loan = Borrow(g);
        long balance = loan.Balance;
        long payment = loan.Payment;
        long fee = balance / 10;
        int rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        Assert.Equal(balance + fee, loan.Balance);
        Assert.Equal((balance + fee + 103) / 104, loan.Payment);
        Assert.True(loan.Payment < payment * 6 / 10);
        Assert.Equal(rep - PressSystem.Damp(g.C, g.Me, 15), g.Me.Reputation);
        Assert.Equal(cash, g.Me.Cash);
        GameEvent e = Of(g).Single();
        Assert.Equal(loan.Id, e.SubjectId);
        Assert.Equal(fee, e.Amount);
        Assert.Contains("restructured loan", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void It_clears_missed_payments_and_the_lender_does_not_take_the_chips_after_it()
    {
        Game g = Lab();
        Loan loan = Borrow(g);
        long gpus = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        g.Me.Cash = 0;
        Loans.RunWeek(g.C, g.State, g.Me);
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, loan.Misses);
        g.Me.Cash = 10_000_000_000;
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        Assert.Equal(0, loan.Misses);
        Assert.Equal(1, Of(g).Single().Pick);
        Assert.Contains("forgiven", EventText.Describe(g.C, g.State, Of(g).Single()));
        g.Me.Cash = 0;
        Loans.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, loan.Misses);
        Assert.Equal(gpus, LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.LoanDefaulted);
    }

    [Fact]
    public void It_costs_more_in_all_and_cannot_be_repeated_for_nothing()
    {
        Game g = Lab();
        Loan loan = Borrow(g);
        long balance = loan.Balance;
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        long total = loan.Balance;
        Assert.True(total > balance);
        string d = g.Sim.Digest();
        g.Refused(Command.RestructureLoan(g.Id, loan.Id), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Weeks(60);
        Assert.True(Restructure.Worthwhile(g.C, loan));
        long before = loan.Payment;
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        Assert.True(loan.Payment < before);
        Assert.Equal(2, Of(g).Count);
    }

    [Fact]
    public void A_restructured_loan_is_repaid_over_the_longer_term_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        Loan loan = Borrow(g);
        int id = loan.Id;
        long owed = loan.Balance + Restructure.Fee(g.C, loan);
        g.Do(Command.RestructureLoan(g.Id, id));
        g.Weeks(60);
        Assert.NotNull(Loans.Find(g.Me, id));
        g.Weeks(60);
        Assert.Null(Loans.Find(g.Me, id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LoanRepaid && e.SubjectId == id);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.True(owed > 0);
    }

    [Fact]
    public void Refused_requests_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.RestructureLoan(g.Id, 9999), CommandError.UnknownId);
        Assert.Equal(d, g.Sim.Digest());
        Game off = Lab(c => c.Restructure.Enabled = 0);
        Loan loan = Borrow(off);
        off.Refused(Command.RestructureLoan(off.Id, loan.Id), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.RestructureLoan(rival.Id, 1)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_new_terms_and_play_on_the_same()
    {
        Game g = Lab();
        Loan loan = Borrow(g);
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Loan again = Loans.Find(back.State.Lab(g.Id), loan.Id);
        Assert.Equal(loan.Payment, again.Payment);
        Assert.Equal(loan.Balance, again.Balance);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        Loan loan = Borrow(g, 300);
        g.Weeks(3);
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        g.Weeks(30);
        Loan second = Borrow(g, 1000);
        g.Do(Command.RestructureLoan(g.Id, second.Id));
        g.Weeks(30);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
