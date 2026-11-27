using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Host explain for NEXT item 4 rival decision logs (batch 384).</summary>
public class HostExplainNext384LaterTests
{
    static Game WithRival(string id = "openaye", int logMax = 80)
    {
        return new Game(Simulation.NewGame(
            TestData.Config(c => c.Rivalry.LogMax = logMax),
            new NewGameOptions { Seed = 384, RivalIds = new List<string> { id } }));
    }

    [Fact]
    public void Decision_log_max_and_action_templates_ship()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Rivalry.LogMax >= 60);
        Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused")));
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Rival_slices_expose_latest_action_and_tick()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Weeks(18);
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, 5);
        Assert.True(slices.Count > 0);
        Assert.Equal(slices[0].Action, rival.Log[rival.Log.Count - slices.Count].Action);
    }

    [Fact]
    public void Decision_text_line_includes_calendar_label()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        DecisionLog.Add(g.C, g.State, rival, g.Tick + 40, DecisionAction.Paused, "q", 0);
        string line = DecisionText.Line(g.C, g.State, rival, rival.Log[^1]);
        Assert.Contains(":", line);
        Assert.Contains(rival.Name, line);
    }

    [Fact]
    public void Log_trim_drops_oldest_past_rivalry_log_max()
    {
        Game g = WithRival(logMax: 8);
        LabState rival = g.Rival("openaye");
        for (int i = 0; i < 12; i++)
            DecisionLog.Add(g.C, g.State, rival, i, DecisionAction.Paused, i.ToString(), 0);
        Assert.Equal(8, rival.Log.Count);
        Assert.Equal("4", rival.Log[0].Ref);
    }
}