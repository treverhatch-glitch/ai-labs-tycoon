using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FrontierCostBreakdownHardeningTests
{
    static void ResearchAllOpen(Game g)
    {
        g.Me.ResearchPoints = 9_999_999;
        foreach (TechConfig tech in g.C.Research.Techs)
            if (EraSystem.TechOpen(g.C, tech, g.State.Tick) && !g.Me.Techs.Contains(tech.Id))
                g.Research(tech.Id);
    }

    [Fact]
    public void Level_zero_parts_sum_to_next_frontier_cost()
    {
        var g = Game.Solo();
        var parts = DisplayQueries.FrontierCostParts(g.C, g.Me);
        Assert.Equal(0, parts.FromGrowth);
        Assert.Equal(parts.Base + parts.FromGrowth, parts.Total);
        Assert.Equal(parts.Total, EndingSystem.FrontierCost(g.C, g.Me));
    }

    [Fact]
    public void After_one_level_growth_part_matches_config_permille()
    {
        var g = Game.Solo();
        long c0 = EndingSystem.FrontierCost(g.C, g.Me);
        ResearchAllOpen(g);
        g.Do(Command.ResearchFrontier(g.Id));
        var parts = DisplayQueries.FrontierCostParts(g.C, g.Me);
        Assert.Equal(1, parts.Level);
        Assert.Equal(c0, parts.Base);
        Assert.Equal(MathI.MulPermille(c0, g.C.Endings.Frontier.GrowthPermille) - c0, parts.FromGrowth);
        Assert.Equal(parts.Total, EndingSystem.FrontierCost(g.C, g.Me));
    }

    [Fact]
    public void Max_level_marks_capped_and_negative_total()
    {
        var g = Game.Solo(tweak: c => c.Endings.Frontier.MaxLevel = 2);
        ResearchAllOpen(g);
        g.Do(Command.ResearchFrontier(g.Id));
        g.Do(Command.ResearchFrontier(g.Id));
        var parts = DisplayQueries.FrontierCostParts(g.C, g.Me);
        Assert.True(parts.Capped);
        Assert.Equal(-1, parts.Total);
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.FrontierUnavailable);
    }

    [Fact]
    public void Frontier_command_spends_total_research_points()
    {
        var g = Game.Solo();
        ResearchAllOpen(g);
        long cost = EndingSystem.FrontierCost(g.C, g.Me);
        g.Me.ResearchPoints = cost;
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(0, g.Me.ResearchPoints);
        Assert.Equal(1, g.Me.FrontierLevel);
    }

    [Fact]
    public void Not_enough_points_refused_at_exact_cost_minus_one()
    {
        var g = Game.Solo();
        ResearchAllOpen(g);
        long cost = EndingSystem.FrontierCost(g.C, g.Me);
        g.Me.ResearchPoints = cost - 1;
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.NotEnoughResearch);
    }

    [Fact]
    public void Frontier_level_raises_capability_tech_bonus()
    {
        var g = Game.Solo();
        ResearchAllOpen(g);
        int before = LabQueries.TechBonus(g.C, g.Me, TechEffectKind.CapabilityPermille);
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(before + g.C.Endings.Frontier.CapabilityPermillePerLevel, LabQueries.TechBonus(g.C, g.Me, TechEffectKind.CapabilityPermille));
    }

    [Fact]
    public void Bankrupt_lab_cannot_buy_frontier_but_parts_still_query()
    {
        var g = Game.Solo();
        ResearchAllOpen(g);
        g.Me.Bankrupt = true;
        Assert.True(DisplayQueries.FrontierCostParts(g.C, g.Me).Total > 0);
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void All_open_techs_required_before_first_purchase()
    {
        var g = Game.Solo();
        g.Me.ResearchPoints = 9_999_999;
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.FrontierUnavailable);
    }

    [Fact]
    public void Cost_parts_survive_save_load_and_continue()
    {
        var g = Game.Solo();
        ResearchAllOpen(g);
        g.Do(Command.ResearchFrontier(g.Id));
        var before = DisplayQueries.FrontierCostParts(g.C, g.Me);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = DisplayQueries.FrontierCostParts(loaded.C, loaded.Me);
        Assert.Equal(before.Total, after.Total);
        Assert.Equal(before.Level, after.Level);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 4);
    }
}