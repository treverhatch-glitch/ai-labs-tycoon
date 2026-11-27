using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT 4b + item 5 queue: own chips and compute queue host slices (batch 387–388).</summary>
public class HostExplainNext387LaterTests
{
    [Fact]
    public void Chip_targets_and_listing_fee_expose_in_config()
    {
        var c = TestData.Config();
        Assert.True(c.Chips.Targets.Count >= 1);
        Assert.True(c.Listing.ListingFeePermille >= 0);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Compute_split_reports_cloud_chip_inference_share()
    {
        var g = Game.Solo().WithStarterLab(gpus: 4);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.True(split.Inference >= 0);
        Assert.True(split.General >= 0);
    }

    [Fact]
    public void Queue_parts_reflect_gpu_tier_gate_when_enabled()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.GpuTierUnlock.Enabled = 1;
            c.GpuTierUnlock.MinEra = 0;
            c.GpuTierUnlock.UnlockPermille = 500;
        });
        g.Me.Reputation = 600;
        DisplayQueries.VendorQueueBreakdown q = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        Assert.True(q.Standing >= 0);
        Assert.Equal(VendorSystem.PricePermille(g.C, g.State, g.Me), q.PricePermille);
    }

    [Fact]
    public void Reserved_and_vendor_queue_share_queued_count()
    {
        var g = Game.Solo(tweak: c => c.GpuTierUnlock.Enabled = 0);
        g.State.Vendor.Standing(g.Me.Id).QueuedGpus = 3;
        DisplayQueries.VendorQueueBreakdown vendor = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        DisplayQueries.ReservedComputeBreakdown reserved = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        Assert.Equal(3, vendor.QueuedGpus);
        Assert.Equal(3, reserved.VendorQueuedGpus);
    }
}