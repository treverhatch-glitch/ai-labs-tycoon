using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 7: funding ladder host slices (batch 358).</summary>
public class HostExplainNext358LaterTests
{
    static void OpenFunding(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
    }

    [Fact]
    public void Funding_ladder_lists_every_round_id()
    {
        var g = Game.Solo(tweak: OpenFunding);
        List<DisplayQueries.FundingLadderStep> ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.Equal(g.C.Funding.Rounds.Count, ladder.Count);
        Assert.All(ladder, s => Assert.False(string.IsNullOrEmpty(s.RoundId)));
    }

    [Fact]
    public void Funding_quote_exposes_dilution_on_first_open_round()
    {
        var g = Game.Solo(tweak: OpenFunding);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote q));
        Assert.True(q.DilutionPermille > 0);
        Assert.True(q.FounderShareAfterPermille <= g.Me.FounderSharePermille);
    }

    [Fact]
    public void Funding_ladder_marks_completed_round()
    {
        var g = Game.Solo(tweak: OpenFunding);
        var before = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.False(before[0].Completed);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        var after = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.True(after[0].Completed);
    }
}