using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class CovenantTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, 200);
        g.Do(Command.TakeLoan(g.Id, 1000));
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_loan_within_the_cover_is_no_breach_and_one_below_it_is()
    {
        Game g = Lab();
        Assert.True(Covenants.Cover(g.C, g.State, g.Me) >= g.C.Covenants.MinCoverPermille);
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.CovBreach);
        g.C.Covenants.MinCoverPermille = 5000;
        long before = Loans.Outstanding(g.Me);
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.CovBreach);
        Assert.Equal(1, Count(g, EventKind.CovenantBreached));
        Assert.Equal(before + MathI.MulPermille(before, g.C.Covenants.SurchargePermille), Loans.Outstanding(g.Me));
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, g.Me.CovBreach);
        Assert.Equal(1, Count(g, EventKind.CovenantBreached));
    }

    [Fact]
    public void A_breach_that_comes_right_by_itself_is_logged_as_cured()
    {
        Game g = Lab(c => c.Covenants.CureWeeks = 50);
        g.C.Covenants.MinCoverPermille = 5000;
        Covenants.RunWeek(g.C, g.State, g.Me);
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, g.Me.CovBreach);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, 2000);
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.CovBreach);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.CovenantCured).Pick);
    }

    [Fact]
    public void After_the_cure_weeks_the_lender_calls_the_loans_from_cash_when_there_is_enough()
    {
        Game g = Lab();
        g.C.Covenants.MinCoverPermille = 5000;
        for (int i = 0; i < g.C.Covenants.CureWeeks - 1; i++) Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Covenants.CureWeeks - 1, g.Me.CovBreach);
        long owed = Loans.Outstanding(g.Me);
        long cash = g.Me.Cash;
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.Loans);
        Assert.Equal(1, g.Me.CovCalls);
        Assert.Equal(0, g.Me.CovBreach);
        Assert.True(cash - g.Me.Cash >= owed);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LoansCalled);
        Assert.Equal(1, e.Pick);
        Assert.True(e.Amount >= owed);
    }

    [Fact]
    public void A_lab_without_the_cash_loses_chips_when_the_loans_are_called()
    {
        Game g = Lab();
        g.C.Covenants.MinCoverPermille = 5000;
        int units = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        for (int i = 0; i < g.C.Covenants.CureWeeks - 1; i++) Covenants.RunWeek(g.C, g.State, g.Me);
        g.Me.Cash = 0;
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.Loans);
        Assert.True(LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq) < units);
        Assert.Equal(0, g.State.Events.Last(x => x.Kind == EventKind.LoansCalled).Pick);
        Assert.Equal(1, Count(g, EventKind.LoanDefaulted));
    }

    [Fact]
    public void Paying_down_cures_a_breach_for_just_what_is_needed()
    {
        Game g = Lab();
        g.C.Covenants.MinCoverPermille = 3000;
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.CovBreach);
        long need = Covenants.CureAmount(g.C, g.State, g.Me);
        long owed = Loans.Outstanding(g.Me);
        Assert.True(need > 0 && need < owed);
        long cash = g.Me.Cash;
        g.Do(Command.CureCovenant(g.Id));
        Assert.Equal(cash - need, g.Me.Cash);
        Assert.Equal(owed - need, Loans.Outstanding(g.Me));
        Assert.True(Covenants.Cover(g.C, g.State, g.Me) >= 3000);
        Assert.False(Covenants.InBreach(g.C, g.State, g.Me));
        Assert.Equal(0, g.Me.CovBreach);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.CovenantCured).Pick);
        g.Refused(Command.CureCovenant(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_waiver_costs_a_fee_buys_a_spell_and_the_lender_grants_only_a_few()
    {
        Game g = Lab(c => c.Covenants.MaxWaivers = 2);
        g.C.Covenants.MinCoverPermille = 5000;
        Covenants.RunWeek(g.C, g.State, g.Me);
        long fee = Covenants.WaiverFee(g.C, g.Me);
        Assert.True(fee > 0);
        long cash = g.Me.Cash;
        g.Do(Command.WaiveCovenant(g.Id));
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(1, g.Me.CovWaivers);
        Assert.Equal(0, g.Me.CovBreach);
        Assert.Equal(1, Count(g, EventKind.CovenantWaived));
        g.Refused(Command.WaiveCovenant(g.Id), CommandError.InvalidArgument);
        for (int i = 0; i < g.C.Covenants.WaiverWeeks; i++)
        {
            g.Sim.Step();
            Covenants.RunWeek(g.C, g.State, g.Me);
        }
        Assert.False(Covenants.Waived(g.State, g.Me));
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.CovBreach >= 1);
        g.Do(Command.WaiveCovenant(g.Id));
        for (int i = 0; i < g.C.Covenants.WaiverWeeks; i++) g.Sim.Step();
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, g.Me.CovWaivers);
        g.Refused(Command.WaiveCovenant(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Only_a_lab_in_breach_with_cash_can_use_them()
    {
        Game g = Lab();
        g.Refused(Command.WaiveCovenant(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.CureCovenant(g.Id), CommandError.InvalidArgument);
        g.C.Covenants.MinCoverPermille = 5000;
        g.Me.Cash = 0;
        Assert.False(g.Try(Command.WaiveCovenant(g.Id)).Ok);
        Assert.False(g.Try(Command.CureCovenant(g.Id)).Ok);
        Assert.Equal(0, g.Me.CovWaivers);
    }

    [Fact]
    public void Nothing_happens_when_it_is_switched_off_or_nothing_is_owed()
    {
        Game g = Lab(c => c.Covenants.Enabled = 0);
        g.C.Covenants.MinCoverPermille = 5000;
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.CovBreach);
        g.Refused(Command.WaiveCovenant(g.Id), CommandError.InvalidArgument);
        Game h = Lab();
        h.Me.Loans.Clear();
        h.C.Covenants.MinCoverPermille = 5000;
        Covenants.RunWeek(h.C, h.State, h.Me);
        Assert.Equal(0, h.Me.CovBreach);
        Assert.False(Covenants.InBreach(h.C, h.State, h.Me));
    }

    [Fact]
    public void A_breach_saves_loads_and_plays_on_the_same_and_an_older_save_has_none()
    {
        Game g = Lab(c => c.Covenants.MinCoverPermille = 5000);
        g.Weeks(3);
        Assert.True(g.Me.CovBreach > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 87, g.Sim.Save());
        Assert.DoesNotContain("covBreach", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.CovBreach);
        Assert.Equal(0, lab.CovCalls);
    }

    [Fact]
    public void A_rival_in_breach_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.CovCalls = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("loan covenant"));
        rival.CovCalls = 0;
        g.Me.CovBreach = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("loan covenant"));
        g.Me.CovBreach = 0;
        g.Me.CovWaivers = g.C.Covenants.MaxWaivers + 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("loan covenant"));
        g.Me.CovWaivers = 0;
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Covenants.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Covenants.Enabled = 2;
        c.Covenants.MinCoverPermille = 999;
        c.Covenants.CureWeeks = 0;
        c.Covenants.SurchargePermille = 101;
        c.Covenants.WaiverFeePermille = -1;
        c.Covenants.WaiverWeeks = 0;
        c.Covenants.MaxWaivers = -1;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minCoverPermille", "cureWeeks", "surchargePermille", "waiverFeePermille", "waiverWeeks", "maxWaivers" })
            Assert.Contains(problems, p => p.Contains("covenants." + key));
    }
}
