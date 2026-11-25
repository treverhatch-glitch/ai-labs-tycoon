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
}