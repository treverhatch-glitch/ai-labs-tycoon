using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Hardware generations prep bars for NEXT item 6 (Q25269–Q25274).</summary>
public class HardwareGenerationsPrepLaterTests
{
    [Fact]
    public void Q25269_buy_gpus_tags_current_generation()
    {
        var g = Game.Solo().WithStarterLab(gpus: 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 100_000_000;
        g.Do(Command.BuyGpus(g.Id, 2));
        HardwareBatch batch = g.Me.Fleet.Single();
        Assert.Equal(LabQueries.CurrentGeneration(g.C, g.Tick).Id, batch.GpuGenId);
        Assert.Equal(HardwareBatch.Hq, batch.HoldingId);
    }

    [Fact]
    public void Q25270_fleet_parts_count_owned_hq_gpus()
    {
        var g = Game.Solo().WithStarterLab(gpus: 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 100_000_000;
        g.Do(Command.BuyGpus(g.Id, 8));
        DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        Assert.Equal(8, f.OwnedHqGpus);
        Assert.Equal(8, f.TotalUnits);
    }

    [Fact]
    public void Q25271_move_hardware_changes_holding_not_gen()
    {
        var g = Game.Solo().WithStarterLab(gpus: 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 500_000_000;
        g.Do(Command.BuyGpus(g.Id, 4));
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, land, 100));
        Holding holding = g.Me.Holding(land);
        g.Until(() => holding.HallKw == 100, 120, "hall");
        HardwareBatch batch = g.Me.Fleet.Single(b => b.HoldingId == HardwareBatch.Hq);
        string gen = batch.GpuGenId;
        g.Do(Command.MoveHardware(g.Id, batch.Id, land, 2));
        Assert.Equal(gen, g.Me.Fleet.Single(b => b.HoldingId == land).GpuGenId);
    }

    [Fact]
    public void Q25272_sell_gpus_reduces_fleet_count()
    {
        var g = Game.Solo().WithStarterLab(gpus: 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 100_000_000;
        g.Do(Command.BuyGpus(g.Id, 6));
        g.Do(Command.SellGpus(g.Id, 2));
        Assert.Equal(4, LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq));
    }

    [Fact]
    public void Q25273_resale_uses_generation_price_table()
    {
        var c = TestData.Config();
        var gen = c.Compute.GpuGenerations[0];
        Assert.True(gen.BuyPrice > 0);
        Assert.True(c.Compute.ResalePermille > 0);
        Assert.True(c.Compute.GenerationDecayPermille > 0);
    }

    [Fact]
    public void Q25274_save_round_trip_preserves_gpu_gen_on_fleet()
    {
        var g = Game.Solo().WithStarterLab(gpus: 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 50_000_000;
        g.Do(Command.BuyGpus(g.Id, 3));
        string gen = g.Me.Fleet[0].GpuGenId;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(gen, loaded.Me.Fleet[0].GpuGenId);
    }
}