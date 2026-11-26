using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 4: rival decision log host slices (batch 346).</summary>
public class HostExplainNext346LaterTests
{
    static Game WithRival(string id)
    {
        return new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 41, RivalIds = new List<string> { id } }));
    }

    [Fact]
    public void Rival_log_lines_cap_at_requested_tail()
    {
        Game g = WithRival("openaye");
        LabState rival = g.Rival("openaye");
        g.Weeks(20);
        const int want = 6;
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, want);
        Assert.True(lines.Count <= want);
        Assert.Equal(rival.Log.Count < want ? rival.Log.Count : want, lines.Count);
    }

    [Fact]
    public void Rival_decision_slices_align_with_log_tail()
    {
        Game g = WithRival("anthoptics");
        LabState rival = g.Rival("anthoptics");
        g.Weeks(24);
        const int want = 5;
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, want);
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, want);
        Assert.Equal(lines.Count, slices.Count);
    }

    [Fact]
    public void Rival_relation_slices_list_non_player_labs()
    {
        Game g = WithRival("openaye");
        var rel = DisplayQueries.RivalRelationSlices(g.C, g.State, 8);
        Assert.True(rel.Count >= 1);
        Assert.All(rel, r => Assert.False(string.IsNullOrEmpty(r.RivalId)));
    }
}