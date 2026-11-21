using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class RoboticsPassiveComputeTests
{
    const string Arena = "robotics-arena";

    [Fact]
    public void Arena_tiles_add_general_compute_after_bodies_era()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        long before = LabQueries.Split(g.C, g.State, g.Me, false).General;
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        Assert.True(passive > 0);
        Assert.Equal(before + passive, LabQueries.Split(g.C, g.State, g.Me, false).General);
    }

    [Fact]
    public void Bodies_and_orbital_passive_compute_stack_in_split()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2034);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        ExpansionTestSupport.BuildExpansionRoom(g, "orbital-uplink");
        long both = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        long bodies = MathI.MulPermille(
            36L * LabQueries.CurrentGeneration(g.C, g.State.Tick).ComputePerGpuWeek,
            g.C.Expansions.Bodies.PassiveComputePermillePerTile);
        long orbital = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        Assert.Equal(bodies + orbital, both);
    }

    [Fact]
    public void Before_bodies_era_robotics_passive_is_zero()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028);
        g.Me.Rooms.Add(new RoomState { TypeId = Arena, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        Assert.Equal(0, ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Passive_scales_with_tile_count()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029, tweak: c => c.Expansions.Bodies.PassiveComputePermillePerTile = 100);
        int smallId = ExpansionTestSupport.BuildExpansionRoom(g, Arena, 6, 6);
        long small = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        g.Do(Command.DemolishRoom(g.Id, smallId));
        ExpansionTestSupport.BuildExpansionRoom(g, Arena, 8, 8);
        long large = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        Assert.True(large > small);
    }

    [Fact]
    public void Demolishing_arena_removes_passive_from_split()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        int roomId = ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long with = LabQueries.Split(g.C, g.State, g.Me, false).General;
        g.Do(Command.DemolishRoom(g.Id, roomId));
        long after = LabQueries.Split(g.C, g.State, g.Me, false).General;
        Assert.True(with > after);
        Assert.Equal(0, ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Passive_compute_survives_save_load()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(passive, ExpansionSystem.PassiveExpansionGeneralCompute(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Zero_permille_config_yields_no_passive()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029, tweak: c => c.Expansions.Bodies.PassiveComputePermillePerTile = 0);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Assert.Equal(0, ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Passive_uses_current_gpu_generation_weekly_compute()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena, 6, 6);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        long expected = MathI.MulPermille(36 * gen.ComputePerGpuWeek, g.C.Expansions.Bodies.PassiveComputePermillePerTile);
        Assert.Equal(expected, ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Training_split_includes_arena_passive_with_active_run()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        g.WithStarterLab(gpus: 24);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long passive = ExpansionSystem.PassiveExpansionGeneralCompute(g.C, g.State, g.Me);
        ModelState model = g.Train("mini", 200);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, true);
        Assert.True(split.General >= passive);
        Assert.NotNull(model);
    }
}