using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT 338+: rival decision slices vs log lines, funding quote dilution, soak preset ids.</summary>
public class HostExplainNext338LaterTests
{
  static void OpenFunding(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
    }

    [Fact]
    public void Rival_slices_and_log_lines_share_tail_length()
    {
        var options = new NewGameOptions { Seed = 55, RivalIds = new List<string> { "openaye" } };
        Game g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.Weeks(36);
        LabState rival = g.Rival("openaye");
        const int max = 5;
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, max);
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, max);
        Assert.Equal(lines.Count, slices.Count);
    }

    [Fact]
    public void Funding_quote_exposes_dilution_for_host_panel()
    {
        var g = Game.Solo(tweak: OpenFunding);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote q));
        Assert.True(q.DilutionPermille > 0);
        Assert.True(q.FounderShareAfterPermille < g.Me.FounderSharePermille);
    }

    [Fact]
    public void Balance_soak_presets_are_unique_ids()
    {
        var presets = BalanceSoak.DefaultPresets(TestData.Config());
        var seen = new HashSet<string>();
        foreach (BalanceSoak.Preset p in presets)
        {
            Assert.True(seen.Add(p.Id));
        }
    }
}