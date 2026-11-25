using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT 314+: host explain cross-checks (rival logs, funding ladder, balance soak prep).</summary>
public class HostExplainNext314LaterTests
{
    static void OpenFunding(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
        c.Funding.Round("series-a").OpensYear = 1995;
        c.Funding.Round("series-a").MinReputation = 0;
    }

    [Fact]
    public void Multi_rival_log_lines_stay_bounded_per_lab()
    {
        var options = new NewGameOptions
        {
            Seed = 22,
            RivalIds = new List<string> { "openaye", "xay" },
        };
        GameConfig c = TestData.Config(cfg => cfg.Rivalry.LogMax = 50);
        Game g = new Game(Simulation.NewGame(c, options));
        g.Weeks(30);
        foreach (string id in options.RivalIds)
        {
            LabState rival = g.Rival(id);
            List<string> lines = DisplayQueries.RivalLogLines(c, g.State, rival, 12);
            Assert.True(lines.Count <= 12);
            Assert.True(rival.Log.Count <= c.Rivalry.LogMax);
        }
    }

    [Fact]
    public void Funding_ladder_and_quotes_track_two_raises()
    {
        var g = Game.Solo(tweak: OpenFunding);
        g.State.BubblePermille = 300;
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote first));
        g.Do(Command.RaiseFunding(g.Id, first.RoundId));
        List<DisplayQueries.FundingLadderStep> mid = DisplayQueries.FundingLadderParts(g.C, g.State, g.Me);
        Assert.Contains(mid, s => s.RoundId == first.RoundId && s.Completed);
        Assert.True(DisplayQueries.TryNextFundingQuote(g.C, g.State, g.Me, out DisplayQueries.FundingRoundQuote second));
        Assert.NotEqual(first.RoundId, second.RoundId);
    }

    [Fact]
    public void Balance_soak_preview_pairs_with_preset_game_counts()
    {
        GameConfig c = TestData.Config();
        DisplayQueries.BalanceSoakHostPreview preview = DisplayQueries.BalanceSoakPreview(c);
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(c);
        int impliedGames = preview.PresetCount * preview.DefaultGamesPerPreset;
        Assert.True(impliedGames >= presets.Length * 24);
    }
}