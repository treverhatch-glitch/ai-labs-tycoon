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
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(112, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Margin_satisfy_fires_era_relief()
    {
        Game g = Lab(c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.MarginSatisfyEraRelief.Enabled = 1;
            c.MarginSatisfyEraRelief.MinEra = 0;
            c.MarginSatisfyEraRelief.ReliefPermille = 250;
        });
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        long listPay = MarginCall.CashDue(g.C, g.State, g.Me);
        long pay = MarginSatisfyEraRelief.CashDue(g.C, g.State, listPay);
        Assert.True(pay < listPay);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, (int)pay));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MarginSatisfyEraRelieved);
    }

    [Fact]
    public void Scenario_goal_slack_event_at_mark()
    {
        Game g = Lab(c =>
        {
            c.ScenarioGoals.Enabled = 1;
            c.ScenarioGoalsEraSlack.Enabled = 1;
            c.ScenarioGoalsEraSlack.MinEra = 0;
            c.ScenarioGoalsEraSlack.SlackWeeks = 5;
            c.CampaignDeadline.MinEra = 0;
            c.Scenarios.Scenarios[0].GoalDeadlinePermille = 500;
            ScenarioGoalConfig goal = c.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
            if (goal != null) goal.Target = 99;
        });
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 100;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        long span = stop - g.Me.PlanStartTick;
        int baseMark = g.Me.PlanStartTick + (int)(span * 500 / 1000);
        g.Weeks(baseMark - g.Tick - 1);
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalsEraSlacked);
    }

    [Fact]
    public void Takeoff_grace_extends_answer_window()
    {
        Game g = Lab(c =>
        {
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.TakeoffCrisisEraGrace.Enabled = 1;
            c.TakeoffCrisisEraGrace.MinEra = 0;
            c.TakeoffCrisisEraGrace.GraceWeeks = 3;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        g.Me.OfficeEventId = "";
        int baseWeeks = g.C.Office.ExpireWeeks;
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        Assert.Equal(g.Tick + baseWeeks + 3, g.Me.OfficeEventExpires);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TakeoffCrisisEraGraced);
    }
}