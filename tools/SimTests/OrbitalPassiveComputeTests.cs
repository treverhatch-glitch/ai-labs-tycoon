using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class OrbitalPassiveComputeTests
{
    const string Uplink = "orbital-uplink";

    [Fact]
    public void Uplink_tiles_add_general_compute_after_upward_era()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2034);
        long before = LabQueries.Split(g.C, g.State, g.Me, false).General;
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink, 6, 6);
        long passive = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        Assert.True(passive > 0);
        Assert.Equal(before + passive, LabQueries.Split(g.C, g.State, g.Me, false).General);
    }

    [Fact]
    public void Passive_part_scales_with_current_gpu_generation()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2034, tweak: c => c.Expansions.Upward.PassiveComputePermillePerTile = 100);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink, 6, 6);
        long passive = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        long expected = MathI.MulPermille(36 * gen.ComputePerGpuWeek, 100);
        Assert.Equal(expected, passive);
    }

    [Fact]
    public void Before_upward_era_passive_compute_is_zero()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2032);
        g.Me.Rooms.Add(new RoomState { TypeId = Uplink, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        Assert.Equal(0, ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void No_room_means_no_passive_compute()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        Assert.Equal(0, ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Passive_compute_feeds_training_share_when_runs_exist()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        g.WithStarterLab(gpus: 8);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        ModelState model = g.Train("mini", 200);
        long splitGeneral = LabQueries.Split(g.C, g.State, g.Me, false).General;
        Assert.True(splitGeneral > 0);
        Assert.True(TrainingSystem.TrainingCompute(g.C, g.State, g.Me) > 0);
        g.UntilTrained(model);
    }

    [Fact]
    public void Demolish_uplink_removes_passive_compute()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        int room = ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long with = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        g.Do(Command.DemolishRoom(g.Id, room));
        Assert.True(with > 0);
        Assert.Equal(0, ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Zero_config_permille_disables_passive_compute()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033, tweak: c => c.Expansions.Upward.PassiveComputePermillePerTile = 0);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.Equal(0, ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Weekly_compute_total_includes_orbital_passive()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long weekly = LabQueries.WeeklyCompute(g.C, g.State, g.Me);
        long passive = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        Assert.True(weekly >= passive);
    }

    [Fact]
    public void Passive_orbital_survives_save_load()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long passive = ExpansionSystem.PassiveOrbitalGeneralCompute(g.C, g.State, g.Me);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(passive, ExpansionSystem.PassiveOrbitalGeneralCompute(loaded.C, loaded.State, loaded.Me));
    }
}