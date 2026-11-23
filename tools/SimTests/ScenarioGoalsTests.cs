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
        Game g = Game.Solo(55, cfg =>
            {
                cfg.Scenarios.Scenarios[0].GoalDeadlinePermille = 0;
                cfg.ScenarioGoals.Enabled = 1;
                ScenarioGoalConfig goal = cfg.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
                if (goal != null) goal.Target = 4;
            })
            .WithStarterLab(researchers: 2, engineers: 1, safety: 0, sales: 1, gpus: 2);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.Equal(1, g.Me.ScenarioGoalResult);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalMet);
    }

    [Fact]
    public void Goal_missed_at_campaign_deadline()
    {
        Game g = Game.Solo(56, c => { c.CampaignDeadline.MinEra = 0; c.ScenarioGoals.Enabled = 1; }).WithStarterLab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 2;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        g.Weeks(2);
        Assert.Equal(-1, g.Me.ScenarioGoalResult);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalMissed);
    }
}