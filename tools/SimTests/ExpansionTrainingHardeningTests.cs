using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ExpansionTrainingHardeningTests
{
    const string Arena = "robotics-arena";
    const string Uplink = "orbital-uplink";

    static Game BodiesLab(int gpus = 32)
    {
        var g = ExpansionTestSupport.SoloAtYear(2029);
        g.WithStarterLab(gpus: gpus, engineers: 4);
        return g;
    }

    static Game UpwardLab(int gpus = 48)
    {
        var g = ExpansionTestSupport.SoloAtYear(2033);
        g.WithStarterLab(gpus: gpus, engineers: 1);
        return g;
    }

    [Fact]
    public void Robotics_arena_raises_finished_capability_and_breakdown_matches()
    {
        Game g = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.True(model.CapExpansion > 0);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.Equal(parts.FromExpansion, model.CapExpansion);
        int before = model.Capability - model.CapExpansion;
        Assert.Equal((int)MathI.MulPermille(before, MathI.Permille + g.C.Expansions.Bodies.CapabilityBonusPermille), model.Capability);
    }

    [Fact]
    public void Without_the_arena_capability_matches_a_baseline_run()
    {
        Game baseline = BodiesLab();
        ModelState baseModel = baseline.Train("mini", 200);
        baseline.UntilTrained(baseModel);

        Game boosted = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(boosted, Arena);
        ModelState boostedModel = boosted.Train("mini", 200);
        boosted.UntilTrained(boostedModel);

        Assert.Equal(0, baseModel.CapExpansion);
        Assert.True(boostedModel.Capability > baseModel.Capability);
        Assert.Equal(MathI.MulPermille(baseModel.Capability, MathI.Permille + boosted.C.Expansions.Bodies.CapabilityBonusPermille), boostedModel.Capability);
    }

    [Fact]
    public void Before_the_bodies_era_the_capability_bonus_stays_off()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028);
        g.WithStarterLab(gpus: 24);
        g.Me.Rooms.Add(new RoomState { TypeId = Arena, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.Equal(0, model.CapExpansion);
        Assert.Equal(0, ExpansionSystem.CapabilityBonusPermille(g.C, g.State, g.Me, "mini"));
    }

    [Fact]
    public void Orbital_uplink_adds_training_speed_while_a_run_is_active()
    {
        Game slow = UpwardLab();
        int slowSpeed = TrainingSystem.SpeedPermille(slow.C, slow.State, slow.Me);

        Game fast = UpwardLab();
        ExpansionTestSupport.BuildExpansionRoom(fast, Uplink);
        int fastSpeed = TrainingSystem.SpeedPermille(fast.C, fast.State, fast.Me);

        Assert.Equal(slow.C.Expansions.Upward.TrainingSpeedBonusPermille, fastSpeed - slowSpeed);
        Assert.True(fastSpeed > slowSpeed);
    }

    [Fact]
    public void Orbital_speed_shortens_estimate_weeks_for_the_same_line()
    {
        Game slow = UpwardLab();
        int slowWeeks = TrainingSystem.EstimateWeeks(slow.C, slow.State, slow.Me, slow.C.Line("mini"), 0);

        Game fast = UpwardLab();
        ExpansionTestSupport.BuildExpansionRoom(fast, Uplink);
        int fastWeeks = TrainingSystem.EstimateWeeks(fast.C, fast.State, fast.Me, fast.C.Line("mini"), 0);

        Assert.True(fastWeeks > 0);
        Assert.True(slowWeeks > fastWeeks);
    }

    [Fact]
    public void Demolishing_the_arena_strips_capability_bonus_on_the_next_finish()
    {
        Game g = BodiesLab();
        int roomId = ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState first = g.Train("mini", 200);
        g.UntilTrained(first);
        Assert.True(first.CapExpansion > 0);

        g.Do(Command.DemolishRoom(g.Id, roomId));
        ModelState second = g.Train("mini", 200);
        g.UntilTrained(second);
        Assert.Equal(0, second.CapExpansion);
    }

    [Fact]
    public void Capability_bonus_stacks_with_licensed_data_source_in_breakdown()
    {
        Game g = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.True(parts.FromDataSource > 0);
        Assert.True(parts.FromExpansion > 0);
        Assert.Equal(model.Capability, parts.Total);
    }

    [Fact]
    public void Expansion_capability_survives_save_load_round_trip()
    {
        Game g = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        int cap = model.Capability;
        int expansion = model.CapExpansion;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState after = loaded.Me.Models.Find(m => m.Id == model.Id);
        Assert.NotNull(after);
        Assert.Equal(cap, after.Capability);
        Assert.Equal(expansion, after.CapExpansion);
        Assert.Equal(expansion, DisplayQueries.TrainedCapabilityParts(after).FromExpansion);
    }

    [Fact]
    public void Schema_15_migration_adds_cap_expansion_default()
    {
        Game g = BodiesLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        JsonValue root = Json.Parse(g.Sim.Save());
        root.Set("schema", JsonValue.Of(14));
        JsonPatch.ForEach(root, "labs.models", m => JsonPatch.Remove(m, "capExpansion"));
        var loaded = new Game(Simulation.Load(g.C, Json.Write(root, false)));
        ModelState after = loaded.Me.Models.Find(m => m.Id == model.Id);
        Assert.Equal(0, after.CapExpansion);
        Assert.Equal(model.Capability, after.Capability);
    }
}