using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Filler headlines with a last year as well as a first, and eighteen new lines for the eras.</summary>
public class EraFillerTests
{
    static GameState Quiet(string eraId, int seed = 1)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId, RivalIds = new List<string>() };
        GameState state = Simulation.NewGame(TestData.Config(), options).State;
        state.Events.Clear();
        return state;
    }

    static HashSet<string> Seen(GameConfig c, GameState state, int weeks)
    {
        var seen = new HashSet<string>();
        for (int i = 0; i < weeks; i++)
        {
            state.Tick++;
            foreach (Headline h in NewsTicker.Headlines(c, state)) if (h.IsFiller) seen.Add(h.LineId);
        }
        return seen;
    }

    [Fact]
    public void Eighteen_new_lines_ship_and_the_older_eight_have_no_end()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(26, c.News.Filler.Count);
        foreach (FillerConfig f in c.News.Filler.Take(8)) Assert.Equal(0, f.ToYear);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_line_stops_after_its_last_year()
    {
        GameConfig c = TestData.Config();
        GameState state = Quiet("dial-up-days");
        FillerConfig modem = c.News.Filler.First(f => f.IdValue == "modem-queue");
        Assert.Equal(2003, modem.ToYear);
        HashSet<string> early = Seen(c, state, 200);
        Assert.Contains("modem-queue", early);

        GameState late = Quiet("agents");
        Assert.True(GameDate.Year(c.Calendar, late.Tick) > 2003);
        Assert.DoesNotContain("modem-queue", Seen(c, late, 400));
    }

    [Fact]
    public void Every_year_has_filler_to_pad_with()
    {
        GameConfig c = TestData.Config();
        for (int year = 1995; year <= 2045; year++)
        {
            int open = c.News.Filler.Count(f => f.FromYear <= year && (f.ToYear == 0 || f.ToYear >= year));
            Assert.True(open >= c.News.MinHeadlines, year + " has only " + open);
        }
    }

    [Fact]
    public void The_new_lines_use_no_placeholders_and_read_in_capitals()
    {
        GameConfig c = TestData.Config();
        GameState state = Quiet("it-writes");
        foreach (Headline h in NewsTicker.Headlines(c, state)) Assert.Equal(h.Text.ToUpperInvariant(), h.Text);
        foreach (FillerConfig f in c.News.Filler) Assert.DoesNotContain("{", f.Text);
    }

    [Fact]
    public void A_line_that_ends_before_it_starts_is_caught_by_the_validator()
    {
        GameConfig c = TestData.Config();
        c.News.Filler[0].ToYear = c.News.Filler[0].FromYear - 1;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("ends (") && p.Contains("before it starts"));
    }

    [Fact]
    public void The_ticker_still_changes_nothing()
    {
        GameConfig c = TestData.Config();
        GameState state = Quiet("agents");
        string before = SaveGame.Digest(state);
        NewsTicker.Headlines(c, state);
        Assert.Equal(before, SaveGame.Digest(state));
    }
}
