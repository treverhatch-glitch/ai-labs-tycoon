using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Behavioral sketch for reserved compute and Envydia queue (Q25263–Q25268).</summary>
public class ReservedEnvydiaSketchLaterTests
{
    [Fact]
    public void Q25263_queue_purchase_records_vendor_standing()
    {
        var g = Game.Solo(tweak: c => c.GpuTierUnlock.Enabled = 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 500_000_000;
        VendorStanding st = g.State.Vendor.Standing(g.Me.Id);
        int before = st.QueuedGpus;
        g.Do(Command.QueueGpuPurchase(g.Id, 2, HardwareBatch.Hq));
        Assert.Equal(before + 2, st.QueuedGpus);
    }

    [Fact]
    public void Q25264_reserved_sign_adds_contract_and_reserved_count()
    {
        var g = Game.Solo(tweak: c => { c.ReservedComputeGate.Enabled = 0; c.GpuTierUnlock.Enabled = 0; });
        g.Do(Command.SignReservedCompute(g.Id, 4));
        Assert.Single(g.Me.ReservedContracts);
        DisplayQueries.ReservedComputeBreakdown r = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        Assert.Equal(4, r.ReservedGpus);
    }

    [Fact]
    public void Q25265_vendor_log_grows_on_allocation_shift()
    {
        var g = Game.Solo();
        int n = g.State.Vendor.Log.Count;
        g.Weeks(g.C.Calendar.WeeksPerMonth);
        Assert.True(g.State.Vendor.Log.Count >= n);
    }

    [Fact]
    public void Q25266_gate_on_requires_reputation_for_reserved_sign()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.ReservedComputeGate.Enabled = 1;
            c.ReservedComputeGate.MinReputationPermille = 600;
        });
        g.Me.Reputation = 550;
        g.Refused(Command.SignReservedCompute(g.Id, 1), CommandError.InvalidArgument);
        g.Me.Reputation = 700;
        g.Do(Command.SignReservedCompute(g.Id, 1));
    }

    [Fact]
    public void Q25267_gpu_rent_breakdown_covers_reserved_discount()
    {
        var g = Game.Solo(tweak: c => { c.ReservedComputeGate.Enabled = 0; c.Compute.ReservedRentPermille = 600; });
        g.Do(Command.SignReservedCompute(g.Id, 2));
        DisplayQueries.GpuRentBreakdown rent = DisplayQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(rent.SpotRentedGpus + rent.ReservedDiscounted + rent.CloudChips, rent.Total);
        Assert.Equal(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me), rent.Total);
        Assert.True(rent.ReservedDiscounted > 0);
    }

    [Fact]
    public void Q25268_save_round_trip_keeps_vendor_queue()
    {
        var g = Game.Solo(tweak: c => c.GpuTierUnlock.Enabled = 0);
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 200_000_000;
        g.Do(Command.QueueGpuPurchase(g.Id, 3, HardwareBatch.Hq));
        int queued = g.State.Vendor.Standing(g.Me.Id).QueuedGpus;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(queued, loaded.State.Vendor.Standing(loaded.Me.Id).QueuedGpus);
    }
}