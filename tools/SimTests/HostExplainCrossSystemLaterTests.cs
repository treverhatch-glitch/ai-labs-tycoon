using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Cross-system host explain: rival logs, Envydia queue, hardware fleet, funding ladder (NEXT #4–#7).</summary>
public class HostExplainCrossSystemLaterTests
{
    static Game WithRival(string rivalId)
    {
        var options = new NewGameOptions { Seed = 3, RivalIds = new List<string> { rivalId } };
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
    public void Reserved_sign_and_vendor_queue_explain_same_lab_state()
    {
        var g = Game.Solo(tweak: c => { c.Compute.ReservedRentPermille = 700; OpenFunding(c); });
        g.State.Vendor.Standing(g.Me.Id).QueuedGpus = 5;
        int signCount = 4;
        g.Do(Command.SignReservedCompute(g.Id, signCount));
        DisplayQueries.HardwareFleetBreakdown fleet = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        DisplayQueries.ReservedComputeBreakdown reserved = DisplayQueries.ReservedComputeParts(g.C, g.State, g.Me);
        DisplayQueries.VendorQueueBreakdown vendor = DisplayQueries.VendorQueueParts(g.C, g.State, g.Me);
        DisplayQueries.GpuRentBreakdown rent = DisplayQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(signCount, fleet.ReservedGpus);
        Assert.Equal(reserved.WeeklyDiscounted, rent.ReservedDiscounted);
        Assert.Equal(vendor.QueuedGpus, reserved.VendorQueuedGpus);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out _));
    }

    [Fact]
    public void Rival_log_and_relation_slices_stay_consistent_after_weeks()
    {
        Game g = WithRival("openaye");
        LabState rival = g.Rival("openaye");
        g.Weeks(24);
        var relations = DisplayQueries.RivalRelationSlices(g.C, g.State, 3);
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, 5);
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, 5);
        Assert.Single(relations);
        Assert.Equal(rival.Log.Count, relations[0].LogLineCount);
        Assert.Equal(lines.Count, slices.Count);
    }

    [Fact]
    public void Funding_ladder_and_bubble_quote_track_after_first_round()
    {
        var g = Game.Solo(tweak: c => { OpenFunding(c); c.Funding.FollowOn.DownRoundBubblePermille = 400; });
        g.State.BubblePermille = 600;
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote before));
        var ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.True(ladder[0].Unlocked);
        g.Do(Command.RaiseFunding(g.Id, before.RoundId));
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote after));
        ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.True(ladder[0].Completed);
        Assert.Equal("series-a", after.RoundId);
    }
}