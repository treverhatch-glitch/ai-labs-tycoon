using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q109: frontier.cashPerLevel weekly museum stipend.</summary>
public class FrontierCashStipendTests
{
    [Fact]
    public void Shipped_frontier_cash_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(200, c.Endings.Frontier.CashPerLevel);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Zero_frontier_level_no_cash()
    {
        Game g = Game.Solo().WithStarterLab();
        long before = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Frontier_level_grants_weekly_cash()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 2;
        long before = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Endings.Frontier.CashPerLevel, g.Me.Cash);
    }

    [Fact]
    public void Zero_config_disables_frontier_cash()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.Endings.Frontier.CashPerLevel = 0;
        g.Me.FrontierLevel = 3;
        long before = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Cash_hits_grants_ledger()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 1;
        long grantsBefore = g.Me.Ledger.Lifetime.Grants;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(grantsBefore + g.C.Endings.Frontier.CashPerLevel, g.Me.Ledger.Lifetime.Grants);
    }

    [Fact]
    public void Stacks_with_frontier_research_same_week()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 4;
        long cash = g.Me.Cash;
        long rp = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash + 4 * g.C.Endings.Frontier.CashPerLevel, g.Me.Cash);
        Assert.Equal(rp + 4 * g.C.Endings.Frontier.ResearchPointsPerLevel, g.Me.ResearchPoints);
    }

    [Fact]
    public void Bankrupt_lab_skips_museum_week()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 2;
        g.Me.Bankrupt = true;
        long before = g.Me.Cash;
        g.Sim.Step();
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Save_load_preserves_frontier_cash_stipend()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 2;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long cash = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Me.FrontierLevel = 2;
        long before = loaded.Me.Cash;
        MuseumSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(cash + 2 * loaded.C.Endings.Frontier.CashPerLevel, loaded.Me.Cash);
        Assert.Equal(before + 2 * loaded.C.Endings.Frontier.CashPerLevel, loaded.Me.Cash);
    }

    [Fact]
    public void Plan_lab_gets_same_cash_formula()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.FrontierLevel = 1;
        long before = plan.Cash;
        MuseumSystem.RunWeek(g.C, g.State, plan);
        Assert.Equal(before + g.C.Endings.Frontier.CashPerLevel, plan.Cash);
    }

    [Fact]
    public void Negative_cash_per_level_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Endings.Frontier.CashPerLevel = -5));
    }

    [Fact]
    public void Frontier_cash_independent_of_museum_full_line_stipend()
    {
        Game g = Game.Solo(1, c => c.Endings.Museum.CashPerFullLine = 5000).WithStarterLab();
        g.Me.FrontierLevel = 1;
        long cash = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Cash >= cash + g.C.Endings.Frontier.CashPerLevel);
    }

    [Fact]
    public void High_level_scales_linearly()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = 10;
        long before = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 10 * g.C.Endings.Frontier.CashPerLevel, g.Me.Cash);
    }

    [Fact]
    public void Museum_salvage_unchanged_when_frontier_cash_runs()
    {
        Game g = Game.Solo(1, c => c.Endings.Museum.ResearchPointsPerFullLine = 0);
        g.Me.FrontierLevel = 1;
        long before = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Endings.Frontier.CashPerLevel, g.Me.Cash);
    }
}