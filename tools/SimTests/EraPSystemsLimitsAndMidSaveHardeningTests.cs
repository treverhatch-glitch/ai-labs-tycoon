using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraPSystemsLimitsAndMidSaveHardeningTests
{
    static Game MarginLab(long seed = 140)
    {
        Game g = Game.Solo(seed, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    static void TriggerMarginCall(Game g)
    {
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.MarginCallLeft > 0);
    }

    [Fact]
    public void Satisfy_margin_call_refuses_when_none_outstanding()
    {
        Game g = MarginLab();
        g.Refused(Command.SatisfyMarginCall(g.Id, 0, 1000), CommandError.InvalidArgument);
    }

    [Fact]
    public void Law_shape_refuses_without_target_when_enabled()
    {
        Game g = Game.Solo(141, c =>
        {
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Refused(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Law_target_refuses_unknown_goal()
    {
        Game g = Game.Solo(142).WithStarterLab();
        g.Refused(Command.SetLawTarget(g.Id, "vague"), CommandError.UnknownId);
    }

    [Fact]
    public void Margin_call_left_mid_save_load_matches()
    {
        Game g = MarginLab(143);
        TriggerMarginCall(g);
        int left = g.Me.MarginCallLeft;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(left, back.Player.MarginCallLeft);
    }

    [Fact]
    public void Margin_satisfy_mid_save_load_clears_call()
    {
        Game g = MarginLab(144);
        TriggerMarginCall(g);
        int pay = (int)MarginCall.CashDue(g.C, g.State, g.Me);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, pay));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(0, back.Player.MarginCallLeft);
    }

    [Fact]
    public void Respond_rung_six_refuses_without_pending_event()
    {
        Game g = Game.Solo(145, c => { c.RungSixCrisis.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        g.Me.OfficeEventId = "";
        g.Refused(Command.RespondRungSix(g.Id, "hold"), CommandError.NoOfficeEvent);
    }

    [Fact]
    public void Satisfy_margin_call_refuses_when_margin_calls_disabled()
    {
        Game g = Game.Solo(146, c => c.MarginCall.Enabled = 0).WithStarterLab();
        g.Refused(Command.SatisfyMarginCall(g.Id, 0, 1000), CommandError.InvalidArgument);
    }

    [Fact]
    public void Law_target_refuses_when_shaping_disabled()
    {
        Game g = Game.Solo(147, c => c.LawTarget.Enabled = 0).WithStarterLab();
        g.Refused(Command.SetLawTarget(g.Id, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Margin_call_left_mid_save_validates_bounds()
    {
        Game g = MarginLab(148);
        TriggerMarginCall(g);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.True(back.Player.MarginCallLeft > 0);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Set_law_target_mid_save_load_matches()
    {
        Game g = Game.Solo(149).WithStarterLab();
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Loose));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(LawShaping.Loose, back.Player.LawTarget);
    }

    [Fact]
    public void Margin_call_left_with_scenario_id_mid_save_matches()
    {
        Game g = MarginLab(150);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        TriggerMarginCall(g);
        int margin = g.Me.MarginCallLeft;
        string scenario = g.Me.ScenarioId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        Assert.Equal(scenario, back.Player.ScenarioId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_call_left_with_baggage_mid_save_matches()
    {
        Game g = Game.Solo(151, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.Loans.LoanToValuePermille = 800;
            c.ScenarioBaggage.Enabled = 1;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 500_000_000_000;
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        TriggerMarginCall(g);
        int margin = g.Me.MarginCallLeft;
        string baggage = g.Me.BaggageId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        Assert.Equal(baggage, back.Player.BaggageId);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }

    [Fact]
    public void Margin_call_left_with_scenario_goal_and_baggage_mid_save_matches()
    {
        Game g = Game.Solo(152, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.Loans.LoanToValuePermille = 800;
            c.ScenarioBaggage.Enabled = 1;
            c.ScenarioGoals.Enabled = 1;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 4);
        g.Me.Cash = 500_000_000_000;
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = 4;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        TriggerMarginCall(g);
        int margin = g.Me.MarginCallLeft;
        string baggage = g.Me.BaggageId;
        int result = g.Me.ScenarioGoalResult;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(margin, back.Player.MarginCallLeft);
        Assert.Equal(baggage, back.Player.BaggageId);
        Assert.Equal(result, back.Player.ScenarioGoalResult);
        HardeningInvariants.AssertEraPStateBounds(g.C, back.Player);
    }
}