using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT 330+: host slices for rival/vendor logs, funding ladder tail, soak presets.</summary>
public class HostExplainNext330LaterTests
{
    [Fact]
    public void Vendor_log_lines_render_after_queue_activity()
    {
        var g = Game.Solo();
        int total = g.State.Vendor.Log.Count;
        List<string> lines = DisplayQueries.VendorLogLines(g.C, g.State, 6);
        Assert.True(lines.Count <= 6);
        Assert.Equal(total < 6 ? total : 6, lines.Count);
    }

    [Fact]
    public void Funding_ladder_lists_all_config_rounds_for_host()
    {
        var g = Game.Solo();
        List<DisplayQueries.FundingLadderStep> ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.Equal(g.C.Funding.Rounds.Count, ladder.Count);
        Assert.All(ladder, s => Assert.False(string.IsNullOrEmpty(s.RoundId)));
    }

    [Fact]
    public void Balance_soak_default_presets_match_preview_count()
    {
        GameConfig c = TestData.Config();
        var preview = DisplayQueries.BalanceSoakPreview(c);
        var presets = BalanceSoak.DefaultPresets(c);
        Assert.True(preview.PresetCount >= 3);
        Assert.Equal(presets.Length, preview.PresetCount);
    }
}