using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ScenarioGoalsTests
{
    [Fact]
    public void Goal_met_before_deadline()
    {
        GameConfig c = TestData.Config();
        ScenarioConfig sc = c.Scenarios.Scenarios[0];
        var options = new NewGameOptions { Seed = 55, ScenarioId = sc.IdValue, RivalIds = new List<string>() };
        Game g = new Game(Simulation.NewGame(c, options));
        g.Me.ScenarioId = sc.IdValue;
        for (int i = 0; i < 12; i++) g.Do(Command.Hire(g.Id, "researcher", 1));
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.Equal(1, g.Me.ScenarioGoalResult);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalMet);
    }

    [Fact]
    public void Goal_missed_at_campaign_deadline()
    {
        Game g = Game.Solo(56, c => { c.CampaignDeadline.MinEra = 0; c.ScenarioGoals.Enabled = 1; });
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 2;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        g.Weeks(2);
        Assert.Equal(-1, g.Me.ScenarioGoalResult);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalMissed);
    }
}