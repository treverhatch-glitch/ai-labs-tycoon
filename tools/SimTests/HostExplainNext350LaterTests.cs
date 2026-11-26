using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 5: reserved compute and Envydia queue host slices (batch 350).</summary>
public class HostExplainNext350LaterTests
{
    [Fact]
    public void Vendor_queue_parts_track_standing_and_allocation()
    {
        var g = Game.Solo();
        VendorStanding st = g.State.Vendor.Standing(g.Me.Id);
        st.QueuedGpus = 2;
        st.QueueHoldingId = HardwareBatch.Hq;
        DisplayQueries.VendorQueueBreakdown b = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        Assert.Equal(st.Standing, b.Standing);
        Assert.Equal(2, b.QueuedGpus);
        Assert.Equal(VendorSystem.AllocationLeft(g.State, g.Me), b.AllocationLeft);
    }

    [Fact]
    public void Reserved_compute_parts_match_active_contract()
    {
        var g = Game.Solo(tweak: c => c.Compute.ReservedRentPermille = 800);
        Assert.True(DisplayQueries.TrySignReservedQuote(g.C, g.State, g.Me, 4, out DisplayQueries.SignReservedQuote sign));
        g.Do(Command.SignReservedCompute(g.Id, 4));
        DisplayQueries.ReservedComputeBreakdown reserved = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        Assert.Equal(4, reserved.ReservedGpus);
        Assert.Equal(1, reserved.ActiveContracts);
        Assert.Equal(sign.WeeklyRentAfterSign, reserved.WeeklyDiscounted);
    }

    [Fact]
    public void Vendor_log_lines_cap_matches_envydia_log()
    {
        var g = Game.Solo();
        int total = g.State.Vendor.Log.Count;
        const int want = 4;
        List<string> lines = DisplayQueries.VendorLogLines(g.C, g.State, want);
        Assert.Equal(total < want ? total : want, lines.Count);
    }
}