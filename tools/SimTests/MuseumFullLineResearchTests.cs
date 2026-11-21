using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q89: full museum lines grant flat weekly research points.</summary>
public class MuseumFullLineResearchTests
{
    static void RetireMany(Game g, string line, int count, int capability = 80)
    {
        for (int i = 0; i < count; i++)
        {
            var m = new ModelState
            {
                Id = g.State.TakeId(),
                LineId = line,
                Name = line + "-v" + i,
                Status = ModelStatus.Retired,
                Capability = capability + i,
                TrainedTick = g.Tick - i,
                ReleasedTick = g.Tick - i
            };
            g.Me.Models.Add(m);
        }
    }

    [Fact]
    public void Shipped_museum_research_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(25, c.Endings.Museum.ResearchPointsPerFullLine);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Partial_line_grants_no_weekly_research()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap - 1);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Full_line_grants_weekly_research()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Endings.Museum.ResearchPointsPerFullLine, g.Me.ResearchPoints);
    }

    [Fact]
    public void Two_full_lines_double_research()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap);
        RetireMany(g, "max", cap);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Endings.Museum.ResearchPointsPerFullLine, g.Me.ResearchPoints);
    }

    [Fact]
    public void Zero_config_disables_weekly_research()
    {
        Game g = Game.Solo();
        g.C.Endings.Museum.ResearchPointsPerFullLine = 0;
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Weekly_research_stacks_with_reputation_hook()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        int repBefore = g.Me.Reputation;
        long rpBefore = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation > repBefore);
        Assert.True(g.Me.ResearchPoints > rpBefore);
    }

    [Fact]
    public void Prune_salvage_still_separate_from_full_line_grant()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap + 2, 100);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long gained = g.Me.ResearchPoints - before;
        Assert.True(gained >= g.C.Endings.Museum.ResearchPointsPerFullLine);
    }

    [Fact]
    public void Sim_week_runs_museum_research()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long before = g.Me.ResearchPoints;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints >= before + g.C.Endings.Museum.ResearchPointsPerFullLine);
    }

    [Fact]
    public void Save_load_preserves_research_grant()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        g.Weeks(1);
        long points = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(1);
        Assert.True(loaded.Me.ResearchPoints > points);
    }

    [Fact]
    public void Bankrupt_lab_museum_still_runs()
    {
        Game g = Game.Solo();
        g.Me.Bankrupt = true;
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Endings.Museum.ResearchPointsPerFullLine, g.Me.ResearchPoints);
    }

    [Fact]
    public void Extra_retired_beyond_cap_still_counts_full()
    {
        Game g = Game.Solo();
        g.C.Endings.Museum.ResearchPermillePerCapability = 0;
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine + 3);
        long before = g.Me.ResearchPoints;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchPoints >= before + g.C.Endings.Museum.ResearchPointsPerFullLine);
    }

    [Fact]
    public void Retired_on_line_query_matches_grant()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        Assert.Equal(g.C.Endings.Museum.PerLine, MuseumSystem.RetiredOnLine(g.Me, "mini"));
    }

    [Fact]
    public void Negative_research_config_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Endings.Museum.ResearchPointsPerFullLine = -1));
    }
}