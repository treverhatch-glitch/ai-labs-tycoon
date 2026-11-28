using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Governance commands (batch 15–16) refuse rival labs and spectate mode.</summary>
public class EraGovernancePlayerOnlyHardeningTests
{
    static Game WithRival(long seed = 180)
    {
        return Game.New(seed, c =>
        {
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.MarginCall.Enabled = 1;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.ScenarioBaggage.Enabled = 1;
            c.TakeoffCrisis.MinEra = 0;
            c.RungSixCrisis.MinEra = 0;
            c.Office.ChancePermille = 0;
        });
    }

    [Fact]
    public void Campaign_deadline_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SetCampaignDeadline(rival.Id, g.Tick + 50), CommandError.InvalidArgument);
    }

    [Fact]
    public void Shape_law_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.ShapeLaw(rival.Id, rival.HomeBlocId, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pick_scenario_baggage_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.PickScenarioBaggage(rival.Id, "early-lawsuit"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Respond_takeoff_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.RespondTakeoff(rival.Id, "contain"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Set_law_target_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SetLawTarget(rival.Id, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Satisfy_margin_call_refuses_rival_lab_id()
    {
        Game g = WithRival(181);
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SatisfyMarginCall(rival.Id, 0, 1000), CommandError.InvalidArgument);
    }

    [Fact]
    public void Respond_rung_six_refuses_rival_lab_id()
    {
        Game g = WithRival(182);
        LabState rival = g.Rival("openaye");
        g.Refused(Command.RespondRungSix(rival.Id, "hold"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Spectate_refuses_set_campaign_deadline()
    {
        Game g = WithRival(183);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SetCampaignDeadline(playerId, g.Tick + 40), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_shape_law()
    {
        Game g = WithRival(184);
        int playerId = g.Id;
        string home = g.Me.HomeBlocId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.ShapeLaw(playerId, home, LawShaping.Loose), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_set_law_target()
    {
        Game g = WithRival(185);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SetLawTarget(playerId, LawShaping.Strict), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_satisfy_margin_call()
    {
        Game g = WithRival(186);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SatisfyMarginCall(playerId, 0, 1), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_respond_rung_six()
    {
        Game g = WithRival(187);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.RespondRungSix(playerId, "hold"), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_pick_scenario_baggage()
    {
        Game g = WithRival(188);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.PickScenarioBaggage(playerId, "early-lawsuit"), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_respond_takeoff()
    {
        Game g = WithRival(189);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.RespondTakeoff(playerId, "contain"), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_keeps_picked_baggage_mid_save_matches()
    {
        Game g = WithRival(191);
        int playerId = g.Id;
        g.Do(Command.PickScenarioBaggage(playerId, "early-lawsuit"));
        string baggage = g.Me.BaggageId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(baggage, spectating.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_scenario_id_mid_save_matches()
    {
        Game g = WithRival(192);
        int playerId = g.Id;
        g.C.ScenarioGoals.Enabled = 1;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        string scenario = g.Me.ScenarioId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(scenario, spectating.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_law_target_mid_save_matches()
    {
        Game g = WithRival(193);
        int playerId = g.Id;
        g.Try(Command.SetLawTarget(playerId, LawShaping.Loose));
        string law = g.Me.LawTarget;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(law, spectating.LawTarget);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_scenario_goal_result_mid_save_matches()
    {
        Game g = WithRival(194);
        int playerId = g.Id;
        g.C.ScenarioGoals.Enabled = 1;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        int result = g.Me.ScenarioGoalResult;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(result, spectating.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_public_float_mid_save_matches()
    {
        Game g = WithRival(195);
        int playerId = g.Id;
        g.Me.PublicFloatPpm = 54_000;
        int ppm = g.Me.PublicFloatPpm;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(ppm, spectating.PublicFloatPpm);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_campaign_deadline_tick_mid_save_matches()
    {
        Game g = WithRival(196);
        int playerId = g.Id;
        g.C.CampaignDeadline.MinEra = 0;
        g.Do(Command.SetCampaignDeadline(playerId, g.Tick + 52));
        int stop = g.Me.CampaignDeadlineTick;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(stop, spectating.CampaignDeadlineTick);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_law_shape_pending_mid_save_matches()
    {
        Game g = WithRival(197);
        int playerId = g.Id;
        g.C.LawShaping.MinEra = 0;
        g.C.Lobbying.BackfirePermille = 0;
        g.C.Lobbying.BackfireFloorPermille = 0;
        g.Me.Cash = 1_000_000_000_000;
        EraLawHardeningSupport.SetLawTarget(g, LawShaping.Strict);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        int pending = g.Me.LawShapePending;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(pending, spectating.LawShapePending);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_bubble_permille_mid_save_matches()
    {
        Game g = WithRival(202);
        int playerId = g.Id;
        g.State.BubblePermille = 388;
        int bubble = g.State.BubblePermille;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(bubble, back.State.BubblePermille);
        LabState spectating = back.State.Lab(playerId);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_baggage_picked_mid_save_matches()
    {
        Game g = WithRival(201);
        int playerId = g.Id;
        g.Do(Command.PickScenarioBaggage(playerId, "early-lawsuit"));
        int picked = g.Me.BaggagePicked;
        string baggage = g.Me.BaggageId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(picked, spectating.BaggagePicked);
        Assert.Equal(baggage, spectating.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_rung_six_cooldown_mid_save_matches()
    {
        Game g = WithRival(199);
        int playerId = g.Id;
        g.Me.RungSixCooldownUntil = g.Tick + 30;
        int until = g.Me.RungSixCooldownUntil;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(until, spectating.RungSixCooldownUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_rung_six_arc_until_mid_save_matches()
    {
        Game g = WithRival(200);
        int playerId = g.Id;
        g.Me.RungSixArcUntil = g.Tick + 18;
        int arc = g.Me.RungSixArcUntil;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(arc, spectating.RungSixArcUntil);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_hearing_float_and_scenario_id_mid_save_matches()
    {
        Game g = WithRival(203);
        int playerId = g.Id;
        g.C.HearingSharePrice.MinFloatPpm = 1;
        g.Me.PublicFloatPpm = 88_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int ppm = g.Me.PublicFloatPpm;
        string scenario = g.Me.ScenarioId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(ppm, spectating.PublicFloatPpm);
        Assert.Equal(scenario, spectating.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }

    [Fact]
    public void Spectate_keeps_margin_call_left_mid_save_matches()
    {
        Game g = WithRival(198);
        int playerId = g.Id;
        g.C.MarginCall.Enabled = 1;
        g.C.MarginCall.CovenantPermille = 1000;
        g.C.Loans.LoanToValuePermille = 800;
        g.Me.Cash = 500_000_000_000;
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(playerId, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        if (g.Me.MarginCallLeft < 1) return;
        int margin = g.Me.MarginCallLeft;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        LabState spectating = back.State.Lab(playerId);
        Assert.Equal(margin, spectating.MarginCallLeft);
        HardeningInvariants.AssertEraPStateBounds(g.C, spectating);
    }
}