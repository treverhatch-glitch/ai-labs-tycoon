using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ScenarioGoalsEraSlackTests
{
    [Fact]
    public void Slack_pushes_mid_check_and_fires_event()
    {
        Game g = Game.Solo(96, c =>
        {
            c.ScenarioGoals.Enabled = 1;
            c.CampaignDeadline.MinEra = 0;
            c.Scenarios.Scenarios[0].GoalDeadlinePermille = 500;
            c.ScenarioGoalsEraSlack.Enabled = 1;
            c.ScenarioGoalsEraSlack.MinEra = 0;
            c.ScenarioGoalsEraSlack.SlackWeeks = 4;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(researchers: 1, engineers: 0, safety: 0, sales: 0, gpus: 2);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 20;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        int markNoSlack = g.Tick + 10;
        while (g.Tick + 1 < markNoSlack + 4) g.Weeks(1);
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalsEraSlacked);
    }
}