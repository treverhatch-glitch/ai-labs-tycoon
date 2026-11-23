using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Edge coverage for batch-fifteen governance systems (schemas 139–143).</summary>
public class EraOSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 120)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.Lobbying.CooldownWeeks = 0;
            c.TakeoffCrisis.MinEra = 0;
            c.ScenarioBaggage.Enabled = 1;
            c.PlayerStart.Cash = 50_000_000;
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    [Fact]
    public void Campaign_deadline_mid_save_load_matches_tick()
    {
        Game g = Lab();
        int stop = g.Tick + 12;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
    }

    [Fact]
    public void Law_shape_mid_save_load_matches_pending()
    {
        Game g = Lab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal(g.Me.LawShapePending, back.Player.LawShapePending);
    }

    [Fact]
    public void Scenario_baggage_mid_save_load_matches_id()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        string mid = g.Sim.Save();
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(5);
        Assert.Equal("early-lawsuit", back.Player.BaggageId);
    }

    [Fact]
    public void Takeoff_cooldown_mid_save_load_matches_until()
    {
        Game g = Lab(121);
        g.C.TakeoffCrisis.CapabilityMin = 100;
        g.C.TakeoffCrisis.CorruptionMin = 100;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        g.Me.OfficeEventId = "";
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondTakeoff(g.Id, e.Choices[0].IdValue));
        int until = g.Me.TakeoffCooldownUntil;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(until, back.Player.TakeoffCooldownUntil);
    }

    [Fact]
    public void Schema_139_through_143_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(122);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 40));
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        string current = g.Sim.Save();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 130) HardeningInvariants.AssertEraMStateBounds(g.C, sim.Player);
            if (schema >= 135) HardeningInvariants.AssertEraNStateBounds(g.C, sim.Player);
            if (schema >= 139) HardeningInvariants.AssertEraOStateBounds(g.C, sim.Player);
        }
    }
}