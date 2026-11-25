using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Host explain prep: rival decision log tail, funding ladder quotes, balance soak preview (NEXT queue).</summary>
public class HostExplainQueuePrepLaterTests
{
    static void OpenFunding(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
        c.Funding.Round("series-a").OpensYear = 1995;
        c.Funding.Round("series-a").MinReputation = 0;
    }

    [Fact]
    public void Rival_log_tail_respects_max_and_matches_slice_reasons()
    {
        var options = new NewGameOptions { Seed = 41, RivalIds = new List<string> { "openaye" } };
        GameConfig c = TestData.Config(cfg => cfg.Rivalry.LogMax = 80);
        Game g = new Game(Simulation.NewGame(c, options));
        LabState rival = g.Rival("openaye");
        g.Weeks(40);
        const int max = 4;
        List<string> lines = DisplayQueries.RivalLogLines(c, g.State, rival, max);
        List<DisplayQueries.RivalDecisionSlice> slices = DisplayQueries.RivalDecisionSlices(c, g.State, rival, max);
        Assert.True(lines.Count <= max);
        Assert.Equal(lines.Count, slices.Count);
        if (rival.Log.Count > 0)
        {
            Assert.False(string.IsNullOrWhiteSpace(lines[^1]));
            Assert.True(slices[^1].HasReason);
        }
    }

    [Fact]
    public void Funding_ladder_unlocked_step_matches_next_quote()
    {
        var g = Game.Solo(tweak: c => { OpenFunding(c); c.Funding.FollowOn.DownRoundBubblePermille = 500; });
        List<DisplayQueries.FundingLadderStep> ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.True(ladder.Count >= 2);
        int openIdx = ladder.FindIndex(s => s.Unlocked && !s.Completed);
        Assert.True(openIdx >= 0);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote quote));
        Assert.Equal(ladder[openIdx].RoundId, quote.RoundId);
        Assert.Equal(FundingSystem.EffectiveCash(g.C, g.State, g.C.Funding.Round(quote.RoundId)), quote.Cash);
    }

    [Fact]
    public void Balance_soak_host_preview_ready_for_simcli_scale()
    {
        GameConfig c = TestData.Config();
        DisplayQueries.BalanceSoakHostPreview preview = DisplayQueries.BalanceSoakPreview(c);
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(c);
        Assert.Equal(presets.Length, preview.PresetCount);
        Assert.Equal(24, preview.DefaultGamesPerPreset);
        Assert.All(presets, p => Assert.False(string.IsNullOrEmpty(p.Id)));
    }

    [Fact]
    public void Rival_relation_slices_expose_log_depth_for_host_badges()
    {
        var options = new NewGameOptions { Seed = 9, RivalIds = new List<string> { "openaye" } };
        GameConfig c = TestData.Config();
        Game g = new Game(Simulation.NewGame(c, options));
        LabState rival = g.Rival("openaye");
        g.Weeks(52);
        var relations = DisplayQueries.RivalRelationSlices(g.C, g.State, 4);
        Assert.Single(relations);
        Assert.Equal(rival.Log.Count, relations[0].LogLineCount);
        Assert.False(string.IsNullOrEmpty(relations[0].RivalId));
    }

    [Fact]
    public void Funding_ladder_marks_completed_rung_after_raise()
    {
        var g = Game.Solo(tweak: OpenFunding);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote q));
        g.Do(Command.RaiseFunding(g.Id, q.RoundId));
        List<DisplayQueries.FundingLadderStep> ladder = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.Contains(ladder, s => s.RoundId == q.RoundId && s.Completed);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out _));
    }

    [Fact]
    public void Balance_soak_preview_preset_count_matches_default_ids()
    {
        GameConfig c = TestData.Config();
        DisplayQueries.BalanceSoakHostPreview preview = DisplayQueries.BalanceSoakPreview(c);
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(c);
        HashSet<string> ids = new();
        foreach (BalanceSoak.Preset p in presets)
            ids.Add(p.Id);
        Assert.Equal(ids.Count, preview.PresetCount);
        Assert.True(preview.DefaultGamesPerPreset >= 1);
    }
}