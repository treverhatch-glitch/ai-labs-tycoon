using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-sixteen combination rules keep effects through saves (Q473).</summary>
public class EraPBatchSixteenTogetherHardeningTests
{
    static Game Lab()
    {
        Game g = Game.Solo(160, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 950;
            c.CampaignDeadline.MinEra = 0;
            c.ScenarioGoals.Enabled = 1;
            c.ScenarioBaggage.Enabled = 1;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Scenario_deadline_goal_mid_save_still_resolves()
    {
        Game g = Game.Solo(161, c => { c.CampaignDeadline.MinEra = 0; c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab(researchers: 2, gpus: 4);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 3;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        GameConfig c = g.C;
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(c, mid));
        after.Weeks(3);
        Assert.Contains(after.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit || e.Kind == EventKind.ScenarioGoalMet || e.Kind == EventKind.ScenarioGoalMissed);
    }

    [Fact]
    public void Law_target_shape_mid_save_keeps_pending()
    {
        Game g = Lab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        Assert.Equal(pending, back.Player.LawShapePending);
        Assert.Equal(LawShaping.Loose, back.Player.LawTarget);
    }

    [Fact]
    public void Law_target_and_loan_mid_save_still_allow_shape()
    {
        Game g = Lab();
        g.Try(Command.TakeLoan(g.Id, 300));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(g.C, mid));
        EraLawHardeningSupport.TryShapeLaw(after, LawShaping.Strict);
        Assert.Equal(LawShaping.Strict, after.Me.LawTarget);
    }

    [Fact]
    public void Scenario_id_mid_save_load_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(g.Me.ScenarioId, back.Player.ScenarioId);
    }

    [Fact]
    public void Margin_call_mid_save_still_allows_satisfy()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        if (g.Me.MarginCallLeft < 1) return;
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(g.C, mid));
        int pay = (int)MarginCall.CashDue(after.C, after.State, after.Me);
        after.Do(Command.SatisfyMarginCall(after.Id, 0, pay));
        Assert.Equal(0, after.Me.MarginCallLeft);
    }

    static void TriggerMarginCall(Game g)
    {
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
    }

    static bool TryRespondRungSix(Game g)
    {
        g.C.RungSixCrisis.CorruptionMin = 100;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        if (!OfficeEvents.IsPending(g.Me)) return false;
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondRungSix(g.Id, e.Choices[0].IdValue));
        return true;
    }

    [Fact]
    public void Rung_six_respond_with_margin_call_mid_save_keeps_covenants()
    {
        Game g = Game.Solo(162, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 950;
            c.Loans.LoanToValuePermille = 800;
            c.RungSixCrisis.MinEra = 0;
            c.Office.ChancePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        }).WithStarterLab(researchers: 3, engineers: 4, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        if (!TryRespondRungSix(g)) return;
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        int marginLeft = g.Me.MarginCallLeft;
        int cooldown = g.Me.RungSixCooldownUntil;
        int arcUntil = g.Me.RungSixArcUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(marginLeft, back.Player.MarginCallLeft);
        Assert.Equal(cooldown, back.Player.RungSixCooldownUntil);
        Assert.Equal(arcUntil, back.Player.RungSixArcUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
        back.StepMany(1);
        Assert.Equal(cooldown, back.Player.RungSixCooldownUntil);
        Assert.InRange(back.Player.MarginCallLeft, marginLeft - 1, marginLeft);
    }

    [Fact]
    public void Campaign_deadline_with_margin_call_mid_save_matches()
    {
        Game g = Lab();
        int stop = g.Tick + 48;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        int deadline = g.Me.CampaignDeadlineTick;
        int margin = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(deadline, back.Player.CampaignDeadlineTick);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Law_target_with_scenario_goal_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        int result = g.Me.ScenarioGoalResult;
        string target = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(target, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Campaign_deadline_with_law_target_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 40));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        int stop = g.Me.CampaignDeadlineTick;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Law_shape_pending_with_margin_call_mid_save_matches()
    {
        Game g = Lab();
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        int pending = g.Me.LawShapePending;
        int margin = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(pending, back.Player.LawShapePending);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Scenario_id_with_law_shape_pending_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        string scenario = g.Me.ScenarioId;
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(pending, back.Player.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Public_float_with_scenario_goal_and_deadline_mid_save_matches()
    {
        Game g = Lab();
        g.Me.PublicFloatPpm = 72_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 32));
        int ppm = g.Me.PublicFloatPpm;
        int result = g.Me.ScenarioGoalResult;
        int stop = g.Me.CampaignDeadlineTick;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_with_law_target_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        string baggage = g.Me.BaggageId;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_call_left_with_law_target_mid_save_matches()
    {
        Game g = Lab();
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        string law = g.Me.LawTarget;
        int margin = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(law, back.Player.LawTarget);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_with_scenario_id_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        string scenario = g.Me.ScenarioId;
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(baggage, back.Player.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Law_shape_pending_with_scenario_goal_result_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        int result = g.Me.ScenarioGoalResult;
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(pending, back.Player.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_with_campaign_deadline_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 28));
        string baggage = g.Me.BaggageId;
        int stop = g.Me.CampaignDeadlineTick;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Loan_outstanding_with_scenario_id_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 800));
        string scenario = g.Me.ScenarioId;
        long debt = Loans.Outstanding(g.Me);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(debt, Loans.Outstanding(back.Player));
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_call_with_scenario_id_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        string scenario = g.Me.ScenarioId;
        int margin = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_with_law_shape_pending_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        string baggage = g.Me.BaggageId;
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(pending, back.Player.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_satisfy_with_scenario_goal_mid_save_clears_call()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int result = g.Me.ScenarioGoalResult;
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        int pay = (int)MarginCall.CashDue(g.C, g.State, g.Me);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, pay));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(0, back.Player.MarginCallLeft);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Loan_outstanding_with_law_target_mid_save_matches()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 800));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        long debt = Loans.Outstanding(g.Me);
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(debt, Loans.Outstanding(back.Player));
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_scenario_goal_and_deadline_mid_save_matches()
    {
        Game g = Lab();
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 30));
        string baggage = g.Me.BaggageId;
        int result = g.Me.ScenarioGoalResult;
        int stop = g.Me.CampaignDeadlineTick;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_call_with_baggage_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        TriggerMarginCall(g);
        if (g.Me.MarginCallLeft < 1) return;
        string baggage = g.Me.BaggageId;
        int margin = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Loan_baggage_and_law_target_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 800));
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        string baggage = g.Me.BaggageId;
        long debt = Loans.Outstanding(g.Me);
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(debt, Loans.Outstanding(back.Player));
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Loan_baggage_and_scenario_id_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 800));
        string baggage = g.Me.BaggageId;
        string scenario = g.Me.ScenarioId;
        long debt = Loans.Outstanding(g.Me);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(debt, Loans.Outstanding(back.Player));
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Loan_scenario_goal_and_baggage_mid_save_matches()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 800));
        string baggage = g.Me.BaggageId;
        int result = g.Me.ScenarioGoalResult;
        long debt = Loans.Outstanding(g.Me);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(debt, Loans.Outstanding(back.Player));
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }
}