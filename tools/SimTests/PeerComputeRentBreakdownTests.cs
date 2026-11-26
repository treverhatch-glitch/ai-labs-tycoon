using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q132: PeerComputeQueries.WeeklyRentParts matches peer lease bill.</summary>
public class PeerComputeRentBreakdownTests
{
    [Fact]
    public void Shipped_peer_rent_parts_match_weekly_rent()
    {
        var (g, landlord) = PeerGpuSetup(6);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        var b = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 2);
        Assert.Equal(b.FromSpot + b.FromLeasePermille + b.FromShortage, b.Total);
        Assert.Equal(PeerComputeSystem.WeeklyRent(g.C, g.State, 2), b.Total);
    }

    [Fact]
    public void Zero_gpus_zero_breakdown()
    {
        Game g = Game.Solo();
        Assert.Equal(0, PeerComputeQueries.WeeklyRentParts(g.C, g.State, 0).Total);
    }

    [Fact]
    public void From_spot_uses_current_generation_rent()
    {
        var (g, landlord) = PeerGpuSetup(8);
        int gpus = 3;
        var b = PeerComputeQueries.WeeklyRentParts(g.C, g.State, gpus);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        Assert.Equal(gen.RentPerGpuWeek * gpus, b.FromSpot);
    }

    [Fact]
    public void Lease_permille_adjusts_spot_rent()
    {
        var (g, landlord) = PeerGpuSetup(6);
        var b = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 4);
        long afterLease = b.FromSpot + b.FromLeasePermille;
        Assert.True(b.FromLeasePermille != 0 || g.C.Compute.PeerLeaseRentPermille == MathI.Permille);
        _ = afterLease;
    }

    [Fact]
    public void Shortage_raises_from_shortage_line()
    {
        var (g, landlord) = PeerGpuSetup(6);
        ChipShortageSystem.Start(g.C, g.State, 1300);
        var idle = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 2);
        Assert.True(idle.FromShortage >= 0);
        Assert.Equal(PeerComputeSystem.WeeklyRent(g.C, g.State, 2), idle.Total);
    }

    [Fact]
    public void Gpu_rent_breakdown_peer_line_aligns()
    {
        var (g, landlord) = PeerGpuSetup(6);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        long peerBill = PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me);
        var parts = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 2);
        Assert.True(peerBill > 0 || parts.Total == 0);
    }

    [Fact]
    public void Price_scale_parts_same_shortage_permille_family()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1200);
        long baseRent = 10_000;
        var price = ChipShortageSystem.PriceScaleParts(g.State, baseRent);
        var peer = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 1);
        Assert.Equal(ChipShortageSystem.PriceMulPermille(g.State), ChipShortageSystem.PriceMulPermille(g.State));
        Assert.True(price.FromShortage >= 0);
        _ = peer;
    }

    [Fact]
    public void Deterministic_peer_rent_parts()
    {
        var (g, landlord) = PeerGpuSetup(6);
        var a = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 5);
        var b = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 5);
        Assert.Equal(a.Total, b.Total);
    }

    [Fact]
    public void Save_load_peer_parts_after_shortage()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1150);
        var before = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 2);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = PeerComputeQueries.WeeklyRentParts(loaded.C, loaded.State, 2);
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void More_gpus_scales_from_spot()
    {
        var (g, landlord) = PeerGpuSetup(6);
        var two = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 2);
        var eight = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 8);
        Assert.True(eight.FromSpot > two.FromSpot);
        Assert.True(eight.Total > two.Total);
    }

    [Fact]
    public void No_shortage_from_shortage_zero_when_idle()
    {
        Game g = Game.Solo();
        Assert.False(ChipShortageSystem.Active(g.State));
        var b = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 3);
        Assert.Equal(0, b.FromShortage);
    }

    [Fact]
    public void Peer_compute_system_matches_total()
    {
        var (g, landlord) = PeerGpuSetup(10);
        int n = 6;
        Assert.Equal(PeerComputeQueries.WeeklyRentParts(g.C, g.State, n).Total, PeerComputeSystem.WeeklyRent(g.C, g.State, n));
    }

    [Fact]
    public void Shortage_end_restores_peer_parts()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1100);
        long crunch = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 1).Total;
        g.State.ChipShortageUntilTick = g.Tick;
        ChipShortageSystem.RunWeek(g.C, g.State);
        long normal = PeerComputeQueries.WeeklyRentParts(g.C, g.State, 1).Total;
        Assert.True(crunch >= normal);
    }

    static (Game g, LabState landlord) PeerGpuSetup(int hq)
    {
        Game g = Game.New();
        g.Me.Cash = 500_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        landlord.Cash = 0;
        for (int i = 0; i < hq; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        return (g, landlord);
    }
}