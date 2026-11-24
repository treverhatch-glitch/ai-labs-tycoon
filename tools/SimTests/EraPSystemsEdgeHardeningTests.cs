using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Edge coverage for batch-sixteen systems (schemas 144–148).</summary>
public class EraPSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 170)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.MarginCall.Enabled = 1;
            c.HearingSharePrice.MinFloatPpm = 1;
            c.RungSixCrisis.MinEra = 0;
            c.ScenarioGoals.Enabled = 1;
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.PlayerStart.Cash = 50_000_000;
        }).WithStarterLab(researchers: 3, engineers: 4, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    [Fact]
    public void Law_target_mid_save_load_matches()
    {
        Game g = Lab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(LawShaping.Strict, back.Player.LawTarget);
    }

    [Fact]
    public void Hearing_bubble_mid_save_load_matches_permille()
    {
        Game g = Lab(171);
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        int bubble = g.State.BubblePermille;
        string mid = g.Sim.Save();
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        string choice = q.Choices[0].IdValue;
        g.Do(Command.AnswerOfficeEvent(g.Id, choice));
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
    }

    [Fact]
    public void Hearing_float_with_scenario_goal_result_mid_save_matches()
    {
        Game g = Lab(184);
        g.Me.PublicFloatPpm = 90_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int ppm = g.Me.PublicFloatPpm;
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_arc_with_law_target_mid_save_matches()
    {
        Game g = Lab(185);
        g.Me.RungSixArcUntil = g.Tick + 16;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        int arc = g.Me.RungSixArcUntil;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(arc, back.Player.RungSixArcUntil);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_cooldown_mid_save_load_matches_until()
    {
        Game g = Lab(172);
        g.C.RungSixCrisis.CorruptionMin = 100;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        if (!OfficeEvents.IsPending(g.Me)) return;
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondRungSix(g.Id, e.Choices[0].IdValue));
        int until = g.Me.RungSixCooldownUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(until, back.Player.RungSixCooldownUntil);
    }

    [Fact]
    public void Scenario_goal_result_mid_save_load_matches()
    {
        Game g = Lab(173);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
    }

    [Fact]
    public void Public_float_ppm_mid_save_load_matches()
    {
        Game g = Lab(175);
        g.Me.PublicFloatPpm = 75_000;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(75_000, back.Player.PublicFloatPpm);
    }

    [Fact]
    public void Float_scenario_goal_result_mid_save_load_matches()
    {
        Game g = Lab(180);
        g.Me.PublicFloatPpm = 62_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int ppm = g.Me.PublicFloatPpm;
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Hearing_float_and_law_target_mid_save_load_match()
    {
        Game g = Lab(179);
        g.Me.PublicFloatPpm = 85_000;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        int ppm = g.Me.PublicFloatPpm;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_cooldown_with_law_target_mid_save_matches()
    {
        Game g = Lab(181);
        g.Me.RungSixCooldownUntil = g.Tick + 20;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        int until = g.Me.RungSixCooldownUntil;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(until, back.Player.RungSixCooldownUntil);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Bubble_permille_with_scenario_id_mid_save_matches()
    {
        Game g = Lab(182);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.State.BubblePermille = 420;
        string scenario = g.Me.ScenarioId;
        int bubble = g.State.BubblePermille;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(bubble, back.State.BubblePermille);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Bubble_permille_with_law_shape_pending_mid_save_matches()
    {
        Game g = Lab(186);
        g.State.BubblePermille = 510;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        int bubble = g.State.BubblePermille;
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(pending, back.Player.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_id_mid_save_load_matches_after_pick()
    {
        Game g = Lab(187);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(1, back.Player.BaggagePicked);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Hearing_bubble_with_law_target_mid_save_matches()
    {
        Game g = Lab(188);
        g.Me.PublicFloatPpm = 95_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        int bubble = g.State.BubblePermille;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_arc_with_scenario_id_mid_save_matches()
    {
        Game g = Lab(189);
        g.Me.RungSixArcUntil = g.Tick + 14;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int arc = g.Me.RungSixArcUntil;
        string scenario = g.Me.ScenarioId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(arc, back.Player.RungSixArcUntil);
        Assert.Equal(scenario, back.Player.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Public_float_with_baggage_mid_save_matches()
    {
        Game g = Lab(190);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.PublicFloatPpm = 68_000;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        int ppm = g.Me.PublicFloatPpm;
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(baggage, back.Player.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_cooldown_with_scenario_id_mid_save_matches()
    {
        Game g = Lab(191);
        g.Me.RungSixCooldownUntil = g.Tick + 24;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int until = g.Me.RungSixCooldownUntil;
        string scenario = g.Me.ScenarioId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(until, back.Player.RungSixCooldownUntil);
        Assert.Equal(scenario, back.Player.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Baggage_with_rung_six_arc_mid_save_matches()
    {
        Game g = Lab(192);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.RungSixArcUntil = g.Tick + 18;
        string baggage = g.Me.BaggageId;
        int arc = g.Me.RungSixArcUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(arc, back.Player.RungSixArcUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Bubble_scenario_goal_and_law_target_mid_save_matches()
    {
        Game g = Lab(193);
        g.State.BubblePermille = 380;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Loose);
        int bubble = g.State.BubblePermille;
        int result = g.Me.ScenarioGoalResult;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Law_shape_pending_with_deadline_tick_mid_save_matches()
    {
        Game g = Lab(194);
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 44));
        int pending = g.Me.LawShapePending;
        int stop = g.Me.CampaignDeadlineTick;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(pending, back.Player.LawShapePending);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Float_baggage_and_rung_cooldown_mid_save_matches()
    {
        Game g = Lab(195);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.PublicFloatPpm = 71_000;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.RungSixCooldownUntil = g.Tick + 22;
        int ppm = g.Me.PublicFloatPpm;
        string baggage = g.Me.BaggageId;
        int until = g.Me.RungSixCooldownUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(until, back.Player.RungSixCooldownUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Hearing_float_with_baggage_mid_save_matches()
    {
        Game g = Lab(196);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.PublicFloatPpm = 88_000;
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        int ppm = g.Me.PublicFloatPpm;
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(baggage, back.Player.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Hearing_bubble_law_target_and_scenario_id_mid_save_matches()
    {
        Game g = Lab(201);
        g.Me.PublicFloatPpm = 96_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        int bubble = g.State.BubblePermille;
        string law = g.Me.LawTarget;
        string scenario = g.Me.ScenarioId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(law, back.Player.LawTarget);
        Assert.Equal(scenario, back.Player.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Public_float_scenario_goal_and_baggage_mid_save_matches()
    {
        Game g = Lab(202);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.PublicFloatPpm = 77_000;
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int ppm = g.Me.PublicFloatPpm;
        string baggage = g.Me.BaggageId;
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_cooldown_baggage_and_scenario_id_mid_save_matches()
    {
        Game g = Lab(203);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Me.RungSixCooldownUntil = g.Tick + 28;
        string baggage = g.Me.BaggageId;
        string scenario = g.Me.ScenarioId;
        int until = g.Me.RungSixCooldownUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(until, back.Player.RungSixCooldownUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Bubble_permille_baggage_and_scenario_id_mid_save_matches()
    {
        Game g = Lab(200);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.State.BubblePermille = 445;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        int bubble = g.State.BubblePermille;
        string baggage = g.Me.BaggageId;
        string scenario = g.Me.ScenarioId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(scenario, back.Player.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Hearing_bubble_with_baggage_mid_save_matches()
    {
        Game g = Lab(197);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        int bubble = g.State.BubblePermille;
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        Assert.Equal(baggage, back.Player.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Public_float_law_target_and_scenario_id_mid_save_matches()
    {
        Game g = Lab(198);
        g.Me.PublicFloatPpm = 72_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        int ppm = g.Me.PublicFloatPpm;
        string scenario = g.Me.ScenarioId;
        string law = g.Me.LawTarget;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(ppm, back.Player.PublicFloatPpm);
        Assert.Equal(scenario, back.Player.ScenarioId);
        Assert.Equal(law, back.Player.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_arc_law_shape_pending_and_baggage_mid_save_matches()
    {
        Game g = Lab(199);
        g.C.ScenarioBaggage.Enabled = 1;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.RungSixArcUntil = g.Tick + 20;
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        string baggage = g.Me.BaggageId;
        int arc = g.Me.RungSixArcUntil;
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(arc, back.Player.RungSixArcUntil);
        Assert.Equal(pending, back.Player.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Campaign_deadline_tick_with_scenario_goal_result_mid_save_matches()
    {
        Game g = Lab(183);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 36));
        int stop = g.Me.CampaignDeadlineTick;
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Rung_six_arc_until_mid_save_load_matches()
    {
        Game g = Lab(176);
        g.Me.RungSixArcUntil = g.Tick + 12;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(g.Tick + 12, back.Player.RungSixArcUntil);
    }

    [Fact]
    public void Margin_satisfy_while_rung_six_cooldown_mid_save_clears_call_only()
    {
        Game g = Lab(178);
        g.C.RungSixCrisis.CorruptionMin = 100;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        if (!OfficeEvents.IsPending(g.Me)) return;
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondRungSix(g.Id, e.Choices[0].IdValue));
        int cooldown = g.Me.RungSixCooldownUntil;
        g.C.Loans.LoanToValuePermille = 800;
        g.C.MarginCall.CovenantPermille = 1000;
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
        Assert.Equal(cooldown, after.Me.RungSixCooldownUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, after.Me);
    }

    [Fact]
    public void Rung_six_respond_mid_save_keeps_cooldown_after_weeks()
    {
        Game g = Lab(177);
        g.C.RungSixCrisis.CorruptionMin = 100;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        if (!OfficeEvents.IsPending(g.Me)) return;
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondRungSix(g.Id, e.Choices[0].IdValue));
        int arcUntil = g.Me.RungSixArcUntil;
        int cooldown = g.Me.RungSixCooldownUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(arcUntil, back.Player.RungSixArcUntil);
        Assert.Equal(cooldown, back.Player.RungSixCooldownUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Schema_144_through_148_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(174);
        g.Me.PublicFloatPpm = 50_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 40));
        string current = g.Sim.Save();
        for (int schema = 144; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 139) HardeningInvariants.AssertEraOStateBounds(g.C, sim.Player);
            if (schema >= 144) HardeningInvariants.AssertEraPStateBounds(g.C, sim.Player);
        }
    }
}