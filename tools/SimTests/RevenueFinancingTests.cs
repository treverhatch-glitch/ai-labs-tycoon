using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class RevenueFinancingTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(5, c =>
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
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        return g;
    }

    static void Week(Game g, long revenue)
    {
        g.Me.Ledger.LastWeek.Revenue = revenue;
        RevenueFinancing.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void An_advance_pays_a_share_of_the_weekly_revenue_and_owes_the_cap()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.TakeRevenueAdvance(g.Id, 2, 500));
        long advance = 100_000L * g.C.Rbf.AdvanceWeeks / 2;
        Assert.Equal(cash + advance, g.Me.Cash);
        Assert.Equal(advance, g.Me.RbfAdvance);
        Assert.Equal(MathI.MulPermille(advance, g.C.Rbf.CapB), g.Me.RbfOwed);
        Assert.Equal(g.C.Rbf.ShareB, g.Me.RbfShare);
        Assert.Equal(100_000, g.Me.RbfBase);
        Assert.Equal(2, g.State.Events.Last(e => e.Kind == EventKind.RevenueAdvanceTaken).SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_one_advance_at_a_time_and_only_with_the_revenue_and_a_sane_ask()
    {
        Game g = Lab();
        g.Refused(Command.TakeRevenueAdvance(g.Id, 0, 500), CommandError.InvalidArgument);
        g.Refused(Command.TakeRevenueAdvance(g.Id, 4, 500), CommandError.InvalidArgument);
        g.Refused(Command.TakeRevenueAdvance(g.Id, 1, 0), CommandError.InvalidArgument);
        g.Refused(Command.TakeRevenueAdvance(g.Id, 1, 1001), CommandError.InvalidArgument);
        g.Me.Ledger.LastWeek.Revenue = g.C.Rbf.MinWeeklyRevenue - 1;
        g.Refused(Command.TakeRevenueAdvance(g.Id, 1, 500), CommandError.MissingPrerequisite);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Do(Command.TakeRevenueAdvance(g.Id, 1, 500));
        g.Refused(Command.TakeRevenueAdvance(g.Id, 1, 500), CommandError.InvalidArgument);
        Game off = Lab(c => c.Rbf.Enabled = 0);
        off.Refused(Command.TakeRevenueAdvance(off.Id, 1, 500), CommandError.InvalidArgument);
    }

    [Fact]
    public void Each_week_sweeps_the_share_of_revenue_and_a_bigger_tier_clears_sooner_for_less()
    {
        int weeks1 = 0;
        long paid1 = 0;
        Game a = Lab();
        a.Do(Command.TakeRevenueAdvance(a.Id, 1, 1000));
        long owed1 = a.Me.RbfOwed;
        while (RevenueFinancing.Owing(a.Me) && weeks1 < 400)
        {
            long before = a.Me.RbfOwed;
            Week(a, 100_000);
            paid1 += before - a.Me.RbfOwed;
            weeks1++;
        }
        Game b = Lab();
        b.Do(Command.TakeRevenueAdvance(b.Id, 3, 1000));
        long owed3 = b.Me.RbfOwed;
        int weeks3 = 0;
        while (RevenueFinancing.Owing(b.Me) && weeks3 < 400)
        {
            Week(b, 100_000);
            weeks3++;
        }
        Assert.True(weeks3 < weeks1);
        Assert.True(owed3 < owed1);
        Assert.Equal(owed1, paid1);
        Assert.Equal(0, a.Me.RbfAdvance);
        Assert.Equal(0, a.Me.RbfShare);
    }

    [Fact]
    public void A_sweep_is_never_more_than_what_is_owed_and_clearing_it_resets_the_lab()
    {
        Game g = Lab();
        g.Do(Command.TakeRevenueAdvance(g.Id, 3, 10));
        long owed = g.Me.RbfOwed;
        long cashIn = g.Me.Ledger.Lifetime.CapitalIn;
        Week(g, 100_000_000);
        Assert.False(RevenueFinancing.Owing(g.Me));
        Assert.Equal(-owed, g.Me.Ledger.Lifetime.CapitalIn - cashIn);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.RevenueAdvanceCleared).Pick);
        Assert.Equal(owed, g.State.Events.Last(e => e.Kind == EventKind.RevenueAdvanceCleared).Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_slump_week_adds_to_what_is_owed_and_a_steady_week_does_not()
    {
        Game g = Lab();
        g.Do(Command.TakeRevenueAdvance(g.Id, 1, 1000));
        long owed = g.Me.RbfOwed;
        Week(g, 100_000);
        long steady = g.Me.RbfOwed;
        Assert.True(steady < owed);
        Assert.Equal(0, g.Me.RbfSlump);
        Week(g, 10_000);
        long added = MathI.MulPermille(g.Me.RbfAdvance, g.C.Rbf.SlumpCap);
        Assert.Equal(steady + added - MathI.MulPermille(10_000, g.C.Rbf.ShareA), g.Me.RbfOwed);
        Assert.Equal(1, g.Me.RbfSlump);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.RevenueAdvanceStretched));
    }

    [Fact]
    public void A_buyout_waits_and_then_pays_most_of_what_is_left()
    {
        Game g = Lab();
        g.Do(Command.TakeRevenueAdvance(g.Id, 1, 1000));
        g.Me.Cash = 1_000_000_000;
        g.Refused(Command.BuyOutAdvance(g.Id), CommandError.InvalidArgument);
        for (int i = 0; i < g.C.Rbf.BuyoutAfter; i++) Week(g, 100_000);
        long cost = RevenueFinancing.Buyout(g.C, g.Me);
        Assert.Equal(MathI.MulPermille(g.Me.RbfOwed, g.C.Rbf.BuyoutPermille), cost);
        long cash = g.Me.Cash;
        g.Me.Cash = cost - 1;
        Assert.False(g.Try(Command.BuyOutAdvance(g.Id)).Ok);
        g.Me.Cash = cash;
        g.Do(Command.BuyOutAdvance(g.Id));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.False(RevenueFinancing.Owing(g.Me));
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.RevenueAdvanceCleared).Pick);
        g.Refused(Command.BuyOutAdvance(g.Id), CommandError.InvalidArgument);
        g.Do(Command.TakeRevenueAdvance(g.Id, 2, 300));
    }

    [Fact]
    public void An_advance_and_its_repayment_do_not_change_the_tax_base()
    {
        Game g = Lab(c => c.Tax.Enabled = 1);
        Game control = Lab(c => c.Tax.Enabled = 1);
        g.Me.Ledger.LastWeek.CapitalIn = 400_000;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        control.Me.Ledger.LastWeek.Revenue = 100_000;
        Tax.RunWeek(g.C, g.State, g.Me);
        Tax.RunWeek(control.C, control.State, control.Me);
        Assert.True(control.Me.TaxAccrued > 0);
        Assert.Equal(control.Me.TaxAccrued, g.Me.TaxAccrued);
        g.Me.Ledger.LastWeek.CapitalIn = -50_000;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        Tax.RunWeek(g.C, g.State, g.Me);
        Tax.RunWeek(control.C, control.State, control.Me);
        Assert.Equal(control.Me.TaxAccrued, g.Me.TaxAccrued);
    }

    [Fact]
    public void A_live_game_with_an_advance_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.TakeRevenueAdvance(g.Id, 2, 800));
        g.Weeks(40);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(25);
        g.Weeks(25);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 90, g.Sim.Save());
        Assert.DoesNotContain("rbfOwed", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).RbfOwed);
    }

    [Fact]
    public void A_rival_with_an_advance_and_bad_data_are_caught_and_the_data_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.RbfOwed = 5;
        rival.RbfAdvance = 5;
        rival.RbfTier = 1;
        rival.RbfShare = 50;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue advance"));
        rival.RbfOwed = 0;
        rival.RbfAdvance = 0;
        rival.RbfTier = 0;
        rival.RbfShare = 0;
        g.Me.RbfAdvance = 10;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue advance"));
        g.Me.RbfAdvance = 0;
        g.Me.RbfOwed = 10;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue advance"));
        g.Me.RbfOwed = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Rbf.Enabled = 3;
        c.Rbf.AdvanceWeeks = 0;
        c.Rbf.ShareA = 0;
        c.Rbf.CapA = 900;
        c.Rbf.SlumpPermille = 2000;
        c.Rbf.BuyoutPermille = 50;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minWeeklyRevenue", "shareC", "capA", "slumpPermille", "buyoutAfter" })
            Assert.Contains(problems, p => p.Contains("rbf." + key));
    }
}
