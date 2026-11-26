using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q505 combination wiring for batch twenty-four systems.</summary>
public class BatchTwentyFourTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(98, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Margin_satisfy_takeoff_and_scenario_slack_cross_save()
    {
        Game g = Lab(c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.MarginSatisfyEraRelief.Enabled = 1;
            c.MarginSatisfyEraRelief.MinEra = 0;
            c.MarginSatisfyEraRelief.ReliefPermille = 100;
            c.ScenarioGoals.Enabled = 1;
            c.CampaignDeadline.MinEra = 0;
            c.Scenarios.Scenarios[0].GoalDeadlinePermille = 500;
            c.ScenarioGoalsEraSlack.Enabled = 1;
            c.ScenarioGoalsEraSlack.MinEra = 0;
            c.ScenarioGoalsEraSlack.SlackWeeks = 2;
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.TakeoffCrisisEraGrace.Enabled = 1;
            c.TakeoffCrisisEraGrace.MinEra = 0;
            c.TakeoffCrisisEraGrace.GraceWeeks = 2;
        });
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 16));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TakeoffCrisisEraGraced);
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(g.C, mid));
        Assert.Equal(GameState.CurrentSchema, after.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(after.Sim);
    }
}