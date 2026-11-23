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