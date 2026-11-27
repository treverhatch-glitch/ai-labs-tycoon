using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Zero/max, bankrupt, save-load and cross edges for batch twenty-four (Q502–Q505).</summary>
public class EraQBatchTwentyFourHardeningTests
{
    static Game Lab(long seed = 200, System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginSatisfyEraRelief.Enabled = 1;
            c.MarginSatisfyEraRelief.MinEra = 0;
            c.ScenarioGoals.Enabled = 1;
            c.CampaignDeadline.MinEra = 0;
            c.ScenarioGoalsEraSlack.Enabled = 1;
            c.ScenarioGoalsEraSlack.MinEra = 0;
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisisEraGrace.Enabled = 1;
            c.TakeoffCrisisEraGrace.MinEra = 0;
            c.Office.ChancePermille = 0;
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Max_relief_permille_still_requires_positive_pay_when_breached()
    {
        Game g = Lab(201, c =>
        {
            c.MarginSatisfyEraRelief.ReliefPermille = 1000;
        });
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 950));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        long due = MarginCall.CashDue(g.C, g.State, g.Me);
        Assert.Equal(0, due);
        Assert.True(g.Me.MarginCallLeft > 0);
    }

    [Fact]
    public void Zero_slack_weeks_matches_pre_batch_deadline_mark()
    {
        Game g = Lab(202, c => c.ScenarioGoalsEraSlack.SlackWeeks = 0);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.C.Scenarios.Scenarios[0].GoalDeadlinePermille = 500;
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 10));
        long span = g.Me.CampaignDeadlineTick - g.Me.PlanStartTick;
        long mark = g.Me.PlanStartTick + span * 500 / 1000;
        Assert.Equal(mark, ScenarioGoalsEraSlack.AdjustEarlyMark(g.C, g.State, mark));
    }

    [Fact]
    public void Bankrupt_lab_skips_scenario_slack_checks()
    {
        Game g = Lab(203);
        g.Me.Bankrupt = true;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ScenarioGoalsEraSlacked);
    }

    [Fact]
    public void Mid_save_margin_relief_still_applies_after_load()
    {
        Game g = Lab(204, c => c.MarginSatisfyEraRelief.ReliefPermille = 250);
        g.C.MarginSatisfyEraRelief.MinEra = 99;
        string mid = g.Sim.Save();
        g.C.MarginSatisfyEraRelief.MinEra = 0;
        Game after = new Game(Simulation.Load(g.C, mid));
        Assert.Equal(250, after.C.MarginSatisfyEraRelief.ReliefPermille);
        long adjusted = MarginSatisfyEraRelief.AdjustDue(after.C, after.State, 1_000);
        Assert.True(adjusted < 1_000);
        SaveSchemaPretend.AssertRoundTrip(after.Sim);
    }

    [Fact]
    public void Rival_refused_margin_satisfy_and_takeoff_grace_is_player_only()
    {
        Game g = Game.New(206, c =>
        {
            c.MarginCall.Enabled = 1;
            c.TakeoffCrisisEraGrace.Enabled = 1;
            c.TakeoffCrisisEraGrace.MinEra = 0;
            c.Office.ChancePermille = 0;
        });
        int rivalId = g.State.Labs.First(l => l.Id != g.Me.Id).Id;
        g.Refused(Command.SatisfyMarginCall(rivalId, 0, 1), CommandError.InvalidArgument);
        LabState rival = g.State.Lab(rivalId);
        rival.OfficeEventId = g.C.Office.Events.First(e => e.Trigger == OfficeTrigger.Takeoff).IdValue;
        TakeoffCrisisEraGrace.OnPosed(g.C, g.State, rival);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.TakeoffCrisisEraGraced && e.LabId == rivalId);
    }
}