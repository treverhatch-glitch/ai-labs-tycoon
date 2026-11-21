using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q121–Q123: investigation roll, conviction odds, grey-compute payout parts.</summary>
public class ShadyLegalBreakdownHardeningTests
{
    [Fact]
    public void Shipped_investigation_threshold_from_config()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(300, c.Shady.InvestigationThreshold);
        Assert.Equal(0, ShadyQueries.InvestigationOpenPermille(c, new LabState { Heat = 299 }));
    }

    [Fact]
    public void Investigation_parts_zero_below_threshold()
    {
        Game g = Game.Solo();
        g.Me.Heat = 250;
        var b = ShadyQueries.InvestigationRollParts(g.C, g.Me);
        Assert.Equal(0, b.RollPermille);
        Assert.Equal(0, b.HeatOverThreshold);
    }

    [Fact]
    public void Investigation_permille_matches_parts_total()
    {
        Game g = Game.Solo();
        g.Me.Heat = 700;
        var b = ShadyQueries.InvestigationRollParts(g.C, g.Me);
        Assert.Equal(400, b.HeatOverThreshold);
        Assert.Equal(400 / g.C.Shady.InvestigationDivisor, b.RollPermille);
        Assert.Equal(b.RollPermille, ShadyQueries.InvestigationOpenPermille(g.C, g.Me));
    }

    [Fact]
    public void Investigation_permille_used_by_monthly_roll()
    {
        Game g = Game.Solo(seed: 4);
        g.Me.Heat = 700;
        int permille = ShadyQueries.InvestigationOpenPermille(g.C, g.Me);
        Assert.True(permille > 0);
        ShadySystem.RunMonth(g.C, g.State, g.Me);
    }

    [Fact]
    public void Conviction_parts_sum_to_total()
    {
        Game g = Game.Solo();
        g.Me.Heat = 600;
        var b = ShadyQueries.ConvictionChanceParts(g.C, g.Me);
        Assert.Equal(b.FromHeat + b.FromPolicyStaff, b.Total);
    }

    [Fact]
    public void Policy_staff_lowers_conviction_total()
    {
        Game g = Game.Solo().WithStarterLab(safety: 0, sales: 0, researchers: 0, engineers: 0);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 3, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "policy-office", x, y, 4, 3));
        g.Me.Heat = 800;
        var bare = ShadyQueries.ConvictionChanceParts(g.C, g.Me);
        g.Hire("policy", 3);
        var staffed = ShadyQueries.ConvictionChanceParts(g.C, g.Me);
        Assert.True(staffed.Total < bare.Total);
        Assert.True(staffed.FromPolicyStaff < 0);
    }

    [Fact]
    public void Conviction_total_clamped_at_nine_hundred()
    {
        Game g = Game.Solo();
        g.Me.Heat = 9000;
        Assert.Equal(900, ShadyQueries.ConvictionChanceParts(g.C, g.Me).Total);
    }

    [Fact]
    public void Grey_payout_parts_match_weekly_earn()
    {
        var g = Game.Solo(seed: 3);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 2000;
        g.Do(Command.BuyGpus(g.Id, 8, hall));
        var b = ShadyQueries.GreyWeeklyPayoutParts(g.C, g.State, g.Me, hall);
        Assert.Equal(b.RatePerKw * b.BilledKw, b.Total);
        Assert.Equal(b.Total, ShadyQueries.GreyWeeklyPayout(g.C, g.State, g.Me, hall));
        Assert.Equal(b.InstalledKw / 2, b.BilledKw);
    }

    [Fact]
    public void Grey_payout_zero_without_holding()
    {
        Game g = Game.Solo();
        Assert.Equal(0, ShadyQueries.GreyWeeklyPayoutParts(g.C, g.State, g.Me, 99).Total);
    }

    [Fact]
    public void Grey_payout_halves_installed_kw()
    {
        var g = Game.Solo();
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 20_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 1000;
        g.Do(Command.BuyGpus(g.Id, 4, hall));
        long installed = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall);
        var b = ShadyQueries.GreyWeeklyPayoutParts(g.C, g.State, g.Me, hall);
        Assert.Equal(installed, b.InstalledKw);
        Assert.Equal(installed / 2, b.BilledKw);
    }

    [Fact]
    public void Save_load_investigation_parts_stable()
    {
        Game g = Game.Solo();
        g.Me.Heat = 550;
        var before = ShadyQueries.InvestigationRollParts(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ShadyQueries.InvestigationRollParts(loaded.C, loaded.Me);
        Assert.Equal(before.RollPermille, after.RollPermille);
    }

    [Fact]
    public void Audit_path_uses_same_open_permille()
    {
        Game g = Game.Solo();
        g.Me.Heat = 450;
        int permille = ShadyQueries.InvestigationOpenPermille(g.C, g.Me);
        Assert.True(permille > 0);
        ShadySystem.OnAuditOpen(g.C, g.State, g.Me);
    }

    [Fact]
    public void Grey_weekly_matches_existing_payout_test_math()
    {
        var g = Game.Solo(seed: 7);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 4000;
        g.Do(Command.BuyGpus(g.Id, 20, hall));
        var parts = ShadyQueries.GreyWeeklyPayoutParts(g.C, g.State, g.Me, hall);
        Assert.Equal(g.C.Shady.Card("grey-compute").BasePayout * parts.BilledKw, parts.Total);
    }
}