using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q98: researchPointsPerLevel via MuseumSystem.RunWeek and FrontierLevel.</summary>
public class FrontierResearchStipendTests
{
    static Game WithFrontier(int level)
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.FrontierLevel = level;
        return g;
    }

    [Fact]
    public void Shipped_frontier_research_stipend_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(12, c.Endings.Frontier.ResearchPointsPerLevel);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Zero_frontier_level_no_research_stipend()
    {
        Game g = WithFrontier(0);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Each_level_grants_configured_research()
    {
        Game g = WithFrontier(4);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 4 * g.C.Endings.Frontier.ResearchPointsPerLevel, g.Me.ResearchPoints);
    }

    [Fact]
    public void Higher_level_grants_more_points()
    {
        Game low = WithFrontier(2);
        Game high = WithFrontier(8);
        long a = low.Me.ResearchPoints;
        long b = high.Me.ResearchPoints;
        MuseumSystem.RunWeek(low.C, low.State, low.Me);
        MuseumSystem.RunWeek(high.C, high.State, high.Me);
        Assert.True(high.Me.ResearchPoints - b > low.Me.ResearchPoints - a);
    }

    [Fact]
    public void Zero_config_disables_frontier_research_hook()
    {
        Game g = WithFrontier(5);
        g.C.Endings.Frontier.ResearchPointsPerLevel = 0;
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Stipend_independent_of_museum_retirees()
    {
        Game g = WithFrontier(3);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long gained = g.Me.ResearchPoints - before;
        Assert.Equal(3 * g.C.Endings.Frontier.ResearchPointsPerLevel, gained);
    }

    [Fact]
    public void Stacks_with_museum_full_line_research()
    {
        Game g = WithFrontier(2);
        int cap = g.C.Endings.Museum.PerLine;
        for (int i = 0; i < cap; i++)
        {
            g.Me.Models.Add(new ModelState
            {
                Id = g.State.TakeId(), LineId = "mini", Status = ModelStatus.Retired,
                Capability = 80, TrainedTick = g.Tick, ReleasedTick = g.Tick
            });
        }
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long expected = g.C.Endings.Museum.ResearchPointsPerFullLine
            + 2 * g.C.Endings.Frontier.ResearchPointsPerLevel;
        Assert.Equal(before + expected, g.Me.ResearchPoints);
    }

    [Fact]
    public void Sim_week_runs_frontier_stipend()
    {
        Game g = WithFrontier(3);
        long before = g.Me.ResearchPoints;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints >= before + g.C.Endings.Frontier.ResearchPointsPerLevel * 3);
    }

    [Fact]
    public void Save_load_preserves_frontier_stipend()
    {
        Game g = WithFrontier(5);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long points = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long before = loaded.Me.ResearchPoints;
        MuseumSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(points + 5 * loaded.C.Endings.Frontier.ResearchPointsPerLevel, loaded.Me.ResearchPoints);
        Assert.True(loaded.Me.ResearchPoints > before);
    }

    [Fact]
    public void Bankrupt_lab_still_gets_frontier_stipend()
    {
        Game g = WithFrontier(2);
        g.Me.Bankrupt = true;
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Endings.Frontier.ResearchPointsPerLevel, g.Me.ResearchPoints);
    }

    [Fact]
    public void Plan_lab_gets_same_frontier_hook()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.FrontierLevel = 6;
        long before = plan.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, plan);
        Assert.Equal(before + 6 * g.C.Endings.Frontier.ResearchPointsPerLevel, plan.ResearchPoints);
    }

    [Fact]
    public void Negative_research_per_level_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Endings.Frontier.ResearchPointsPerLevel = -1));
    }

    [Fact]
    public void Second_run_week_stacks_again()
    {
        Game g = WithFrontier(1);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long afterFirst = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(afterFirst + g.C.Endings.Frontier.ResearchPointsPerLevel, g.Me.ResearchPoints);
    }
}