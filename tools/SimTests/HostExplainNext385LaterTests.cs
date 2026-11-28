using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 5: reserved compute and Envydia sketch (batch 385).</summary>
public class HostExplainNext385LaterTests
{
    [Fact]
    public void Sign_quote_deposit_and_weekly_rent_align_after_sign()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.ReservedComputeGate.Enabled = 0;
            c.Compute.ReservedRentPermille = 700;
        });
        Assert.True(DisplayQueries.TrySignReservedQuote(g.C, g.State, g.Me, 3, out DisplayQueries.SignReservedQuote sign));
        long cash = g.Me.Cash;
        g.Do(Command.SignReservedCompute(g.Id, 3));
        Assert.Equal(cash - sign.Deposit, g.Me.Cash);
        DisplayQueries.ReservedComputeBreakdown r = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        Assert.Equal(sign.WeeklyRentAfterSign, r.WeeklyDiscounted);
    }

    [Fact]
    public void Queue_purchase_shows_on_vendor_queue_parts()
    {
        var g = Game.Solo(tweak: c => c.GpuTierUnlock.Enabled = 0);
        g.State.Vendor.Standing(g.Me.Id).QueuedGpus = 5;
        g.State.Vendor.Standing(g.Me.Id).QueueHoldingId = HardwareBatch.Hq;
        DisplayQueries.VendorQueueBreakdown q = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        Assert.Equal(5, q.QueuedGpus);
        Assert.Equal(HardwareBatch.Hq, q.QueueHoldingId);
    }

    [Fact]
    public void Reserved_gate_blocks_sign_when_reputation_low()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.ReservedComputeGate.Enabled = 1;
            c.ReservedComputeGate.MinReputationPermille = 900;
        });
        g.Me.Reputation = 100;
        g.Refused(Command.SignReservedCompute(g.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Vendor_log_line_matches_newest_allocation_entry()
    {
        var g = Game.Solo();
        int n = g.State.Vendor.Log.Count;
        var lines = DisplayQueries.VendorLogLines(g.C, g.State, 1);
        if (n > 0)
            Assert.Equal(DecisionText.Line(g.C, g.State, null, g.State.Vendor.Log[n - 1]), lines[0]);
        else
            Assert.Empty(lines);
    }
}