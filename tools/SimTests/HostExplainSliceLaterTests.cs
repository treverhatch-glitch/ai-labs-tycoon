using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Host-facing explain slices: GPU rent, funding ladder quote, rival decision log tail (Q17-style).</summary>
public class HostExplainSliceLaterTests
{
    static Game WithRival(string rivalId, long seed = 2)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string> { rivalId } };
        GameConfig c = TestData.Config(cfg => cfg.Rivalry.LogMax = 100000);
        return new Game(Simulation.NewGame(c, options));
    }

    static void OpenFunding(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
        c.Funding.Round("series-a").OpensYear = 1995;
        c.Funding.Round("series-a").MinReputation = 0;
    }

    [Fact]
    public void Gpu_rent_parts_sum_to_weekly_gpu_rent()
    {
        var g = Game.Solo(tweak: c => c.Compute.ReservedRentPermille = 800);
        int count = 6;
        g.Do(Command.SignReservedCompute(g.Id, count));
        DisplayQueries.GpuRentBreakdown b = DisplayQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me), b.Total);
        Assert.Equal(b.SpotRentedGpus + b.ReservedDiscounted + b.CloudChips, b.Total);
        Assert.True(b.ReservedDiscounted > 0);
    }

    [Fact]
    public void Next_funding_quote_matches_raise_terms_in_a_hot_bubble()
    {
        var g = Game.Solo(tweak: c => { OpenFunding(c); c.Funding.FollowOn.DownRoundBubblePermille = 500; });
        g.State.BubblePermille = 700;
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote q));
        Assert.Equal("angel", q.RoundId);
        Assert.True(q.DownRound);
        Assert.Equal(FundingSystem.EffectiveCash(g.C, g.State, g.C.Funding.Round("angel")), q.Cash);
        int before = g.Me.FounderSharePermille;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(q.Cash, g.Me.Ledger.Lifetime.CapitalIn);
        Assert.Equal(q.FounderShareAfterPermille, g.Me.FounderSharePermille);
    }

    [Fact]
    public void Player_lab_has_no_rival_log_lines()
    {
        var g = Game.Solo();
        Assert.Empty(DisplayQueries.RivalLogLines(g.C, g.State, g.Me, 5));
    }

    [Fact]
    public void Rival_log_lines_match_tail_of_decision_entries()
    {
        Game g = WithRival("openaye");
        LabState rival = g.Rival("openaye");
        Assert.Equal(LabController.Plan, rival.Controller);
        g.Weeks(12);
        int want = 3;
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, want);
        int expected = rival.Log.Count < want ? rival.Log.Count : want;
        Assert.Equal(expected, lines.Count);
        if (expected > 0)
        {
            int start = rival.Log.Count - expected;
            Assert.Equal(DecisionText.Line(g.C, g.State, rival, rival.Log[start]), lines[0]);
        }
    }

    [Fact]
    public void Funding_quote_skips_closed_rounds()
    {
        var g = Game.Solo(tweak: OpenFunding);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote q));
        Assert.Equal("series-a", q.RoundId);
    }

    [Fact]
    public void Reserved_compute_parts_align_with_gpu_rent_and_sign_quote()
    {
        var g = Game.Solo(tweak: c => c.Compute.ReservedRentPermille = 750);
        int count = 5;
        g.State.Vendor.Standing(g.Me.Id).QueuedGpus = 3;
        Assert.True(DisplayQueries.TrySignReservedQuote(g.C, g.State, g.Me, count, out DisplayQueries.SignReservedQuote sign));
        long cash = g.Me.Cash;
        g.Do(Command.SignReservedCompute(g.Id, count));
        Assert.Equal(cash - sign.Deposit, g.Me.Cash);
        DisplayQueries.ReservedComputeBreakdown reserved = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        DisplayQueries.GpuRentBreakdown rent = DisplayQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(count, reserved.ReservedGpus);
        Assert.Equal(1, reserved.ActiveContracts);
        Assert.Equal(rent.ReservedDiscounted, reserved.WeeklyDiscounted);
        Assert.Equal(sign.WeeklyRentAfterSign, reserved.WeeklyDiscounted);
        Assert.True(reserved.WeeklySavings > 0);
        Assert.Equal(3, reserved.VendorQueuedGpus);
    }

    [Fact]
    public void Rival_decision_slices_match_log_tail()
    {
        Game g = WithRival("openaye");
        LabState rival = g.Rival("openaye");
        g.Weeks(16);
        int want = 4;
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, want);
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, want);
        Assert.Equal(lines.Count, slices.Count);
        if (slices.Count > 0)
        {
            int i = rival.Log.Count - slices.Count;
            Assert.Equal(rival.Log[i].Action, slices[0].Action);
            Assert.Equal(rival.Log[i].Ref, slices[0].Ref);
            Assert.Equal(DecisionText.Line(g.C, g.State, rival, rival.Log[i]), lines[0]);
        }
    }

    [Fact]
    public void Vendor_queue_parts_match_vendor_system()
    {
        var g = Game.Solo();
        VendorStanding st = g.State.Vendor.Standing(g.Me.Id);
        st.QueuedGpus = 4;
        st.QueueHoldingId = HardwareBatch.Hq;
        DisplayQueries.VendorQueueBreakdown b = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        Assert.Equal(st.Standing, b.Standing);
        Assert.Equal(st.AllocationThisMonth, b.AllocationThisMonth);
        Assert.Equal(VendorSystem.AllocationLeft(g.State, g.Me), b.AllocationLeft);
        Assert.Equal(4, b.QueuedGpus);
        Assert.Equal(HardwareBatch.Hq, b.QueueHoldingId);
        Assert.Equal(VendorSystem.PricePermille(g.C, g.State, g.Me), b.PricePermille);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.Equal(VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen), b.UnitPrice);
    }

    [Fact]
    public void Vendor_log_lines_match_envydia_tail()
    {
        var g = Game.Solo();
        int total = g.State.Vendor.Log.Count;
        int want = 3;
        var lines = DisplayQueries.VendorLogLines(g.C, g.State, want);
        int expected = total < want ? total : want;
        Assert.Equal(expected, lines.Count);
        if (expected > 0)
        {
            int i = total - expected;
            Assert.Equal(DecisionText.Line(g.C, g.State, null, g.State.Vendor.Log[i]), lines[0]);
        }
    }
}