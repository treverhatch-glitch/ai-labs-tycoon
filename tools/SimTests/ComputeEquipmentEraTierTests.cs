using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ComputeEquipmentEraTierTests
{
    [Fact]
    public void Upgrade_path_save_and_visuals()
    {
        Game g = Game.Solo(201, c =>
        {
            c.ComputeEquipment.Enabled = 1;
            c.ComputeEquipment.Tiers[1].MinEra = 0;
            c.ComputeEquipment.Tiers[2].MinEra = 0;
        }).WithStarterLab();
        var v0 = DisplayQueries.ComputeEquipmentVisualsFor(g.C, g.Me);
        Assert.Equal("compute-pc-tower-era1", v0.PcTower);
        long cash = g.Me.Cash;
        g.Do(Command.UpgradeComputeEquipment(g.Id));
        Assert.Equal(1, g.Me.ComputeEquipTier);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ComputeEquipmentUpgraded);
        Assert.True(g.Me.Cash < cash);
        string save = g.Sim.Save();
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(1, loaded.Player.ComputeEquipTier);
    }

    [Fact]
    public void Era_locked_refuses_upgrade()
    {
        Game g = Game.Solo(202, c =>
        {
            c.ComputeEquipment.Enabled = 1;
            c.ComputeEquipment.Tiers[2].MinEra = 5;
        }).WithStarterLab();
        g.Do(Command.UpgradeComputeEquipment(g.Id));
        g.Refused(Command.UpgradeComputeEquipment(g.Id), CommandError.EraLocked);
    }

    [Fact]
    public void Research_bonus_after_upgrade()
    {
        Game g = Game.Solo(203, c =>
        {
            c.ComputeEquipment.Enabled = 1;
            c.ComputeEquipment.Tiers[1].MinEra = 0;
            c.ComputeEquipment.Tiers[1].ResearchPermille = 100;
        }).WithStarterLab(researchers: 2);
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        g.Do(Command.UpgradeComputeEquipment(g.Id));
        long after = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(after > before);
    }

    [Fact]
    public void Save_at_172_has_no_equipment_field()
    {
        Game g = Game.Solo(204, c => c.ComputeEquipment.Enabled = 1).WithStarterLab();
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 172, g.Sim.Save()));
        Assert.Equal(0, older.Player.ComputeEquipTier);
    }
}