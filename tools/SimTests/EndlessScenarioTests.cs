using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EndlessScenarioTests
{
    [Fact]
    public void Scenario_win_lists_keep_going()
    {
        EndingDefConfig def = TestData.Config().Endings.Def("scenario-win");
        Assert.Contains("keep-going", def.ContinueModes);
    }

    [Fact]
    public void Keep_going_keeps_player_lab()
    {
        Game g = Game.Solo(73, c => c.EndlessScenario.Enabled = 1);
        g.State.Endings.Add(new EndingRecord { EndingId = "scenario-win", LabId = g.Id, Tick = g.Tick, Choice = "" });
        int idx = g.State.Endings.Count - 1;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "keep-going"));
        Assert.Equal(g.Id, g.State.PlayerLabId);
        Assert.Equal(1, g.Me.ScenarioEndlessAck);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioEndlessAck);
    }
}