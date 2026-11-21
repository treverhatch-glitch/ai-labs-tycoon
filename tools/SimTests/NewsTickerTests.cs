using System;
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

/// <summary>The news ticker: headlines in capitals read off the event log for every lab, padded with filler. Never saved.</summary>
public class NewsTickerTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static GameEvent Add(Game g, EventKind kind, int labId = 0, int subject = -1, long amount = 0, string reference = "")
    {
        return Events.Add(g.State, kind, labId, subject, amount, reference);
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the data

    [Fact]
    public void The_shipped_news_validates_and_has_the_bibles_headlines()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.True(c.News.Headlines.Count >= 12);
        Assert.True(c.News.Filler.Count >= c.News.DefaultHeadlines);
        Assert.Contains(c.News.Headlines, h => h.Text.Contains("Minesweeper"));
        Assert.Contains(c.News.Headlines, h => h.Text.Contains("sold out before the presenter walked on stage"));
        Assert.Contains(c.News.Filler, f => f.Text.Contains("more power than local country"));
    }

    [Fact]
    public void The_validator_catches_bad_news_data()
    {
        AssertProblem(Problems(c => c.News.DefaultHeadlines = 0), "news.defaultHeadlines");
        AssertProblem(Problems(c => c.News.MinHeadlines = c.News.DefaultHeadlines + 1), "news.minHeadlines");
        AssertProblem(Problems(c => { c.News.Filler.Clear(); c.News.MinHeadlines = 3; }), "news.filler");
        AssertProblem(Problems(c => c.News.Headlines[1].IdValue = c.News.Headlines[0].IdValue), "news.headlines");
        AssertProblem(Problems(c => c.News.Headlines[0].Kind = "Nope"), "unknown event kind");
        AssertProblem(Problems(c => c.News.Headlines[0].Text = ""), "has no text");
        AssertProblem(Problems(c => c.News.Headlines[0].Pick = 2), "pick");
        AssertProblem(Problems(c => c.News.Headlines[0].Text = "{lab} {wat}"), "unknown placeholder {wat}");
        AssertProblem(Problems(c => c.News.Headlines[0].Text = "{lab"), "unknown placeholder");
        AssertProblem(Problems(c => c.News.Filler[1].IdValue = c.News.Filler[0].IdValue), "news.filler");
        AssertProblem(Problems(c => c.News.Filler[0].Text = ""), "has no text");
        AssertProblem(Problems(c => c.News.Filler[0].Text = "{lab} wins"), "placeholder");
        AssertProblem(Problems(c => c.News.Filler[0].FromYear = 5), "fromYear");
    }

    // ---- money

    [Theory]
    [InlineData(0, "$0")]
    [InlineData(12, "$12")]
    [InlineData(999, "$999")]
    [InlineData(1000, "$1K")]
    [InlineData(340_000, "$340K")]
    [InlineData(1_000_000, "$1M")]
    [InlineData(1_500_000, "$1.5M")]
    [InlineData(1_999_999, "$1.9M")]
    [InlineData(40_000_000_000, "$40B")]
    [InlineData(2_200_000_000_000, "$2.2T")]
    [InlineData(-1_500_000, "-$1.5M")]
    public void Money_reads_the_way_a_headline_says_it(long dollars, string text)
    {
        Assert.Equal(text, NewsTicker.Money(dollars));
    }

    // ---- headlines for events

    [Fact]
    public void An_incident_names_the_lab_and_the_model_in_capitals()
    {
        Game g = World();
        g.Me.Models.Add(new ModelState { Id = 77, Name = "Glimmer 3", LineId = "mini", Status = ModelStatus.Released });
        GameEvent e = Add(g, EventKind.Incident, g.Id, 77);
        Assert.True(NewsTicker.About(g.C, g.State, e, out Headline h));
        Assert.Equal(h.Text.ToUpperInvariant(), h.Text);
        Assert.Contains("GLIMMER 3", h.Text);
        Assert.Contains(g.Me.Name.ToUpperInvariant(), h.Text);
        Assert.False(h.IsFiller);
        Assert.Equal(EventKind.Incident, h.Kind);
        Assert.Equal(e.Tick, h.Tick);
    }

    [Fact]
    public void An_acquisition_names_both_labs_and_the_price()
    {
        Game g = Game.New(1);
        g.State.Events.Clear();
        LabState other = g.State.Labs.First(l => l.Id != g.Id);
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.LabAcquired, g.Id, other.Id, 1_500_000_000), out Headline h));
        Assert.Contains(other.Name.ToUpperInvariant(), h.Text);
        Assert.Contains(g.Me.Name.ToUpperInvariant(), h.Text);
        Assert.Contains("$1.5B", h.Text);
    }

    [Fact]
    public void Eras_world_news_keynotes_rivals_and_endings_read_their_names_from_the_data()
    {
        Game g = World();
        EraConfig era = g.C.Eras[1];
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.EraBegan, -1, -1, 0, era.Id), out Headline eraNews));
        Assert.Contains(era.Name.ToUpperInvariant(), eraNews.Text);

        WorldEventConfig world = g.C.WorldEvents[0];
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.WorldNews, -1, -1, 0, world.Id), out Headline worldNews));
        Assert.Equal(world.Headline.ToUpperInvariant(), worldNews.Text);

        GpuGenerationConfig gen = g.C.Compute.GpuGenerations[0];
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.Keynote, -1, -1, 100, gen.Id), out Headline keynote));
        Assert.Contains(gen.Name.ToUpperInvariant(), keynote.Text);
        Assert.Contains("SOLD OUT BEFORE THE PRESENTER WALKED ON STAGE", keynote.Text);

        EndingDefConfig ending = g.C.Endings.Endings[0];
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.EndingReached, g.Id, -1, 0, ending.Id), out Headline endingNews));
        Assert.Contains(ending.Name.ToUpperInvariant(), endingNews.Text);

        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.RivalJoined, g.Id, -1, 2_000_000_000, "x"), out Headline joined));
        Assert.Contains("$2B", joined.Text);
    }

    [Fact]
    public void A_firsts_chip_gets_its_own_headline_and_a_later_one_another()
    {
        Game g = World();
        GameEvent first = Add(g, EventKind.ChipReady, g.Id, 1, 0, "");
        first.Pick = 1;
        GameEvent later = Add(g, EventKind.ChipReady, g.Id, 2, 0, "");
        later.Pick = 0;
        Assert.True(NewsTicker.About(g.C, g.State, first, out Headline a));
        Assert.True(NewsTicker.About(g.C, g.State, later, out Headline b));
        Assert.Contains("MINESWEEPER", a.Text);
        Assert.DoesNotContain("MINESWEEPER", b.Text);
        Assert.NotEqual(a.LineId, b.LineId);
    }

    [Fact]
    public void An_event_the_news_has_no_line_for_makes_no_headline()
    {
        Game g = World();
        Assert.False(NewsTicker.About(g.C, g.State, Add(g, EventKind.RoomBuilt, g.Id, -1, 0, "research-lab"), out _));
        Assert.False(NewsTicker.About(g.C, g.State, Add(g, EventKind.Hired, g.Id, -1, 0, "researcher"), out _));
    }

    [Fact]
    public void Missing_names_fall_back_to_something_readable()
    {
        Game g = World();
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.EraBegan, -1, -1, 0, "lost-era"), out Headline h));
        Assert.Contains("LOST-ERA", h.Text);
        Assert.True(NewsTicker.About(g.C, g.State, Add(g, EventKind.Bankrupt, 4242), out Headline gone));
        Assert.Contains("A LAB", gone.Text);
        Assert.DoesNotContain("{", gone.Text);
    }

    // ---- the ticker

    [Fact]
    public void The_newest_headlines_come_first_and_cover_every_lab()
    {
        Game g = Game.New(1);
        g.State.Events.Clear();
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Add(g, EventKind.Bankrupt, rival.Id);
        Add(g, EventKind.Bankrupt, g.Id);
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 2);
        Assert.Equal(2, ticker.Count);
        Assert.Contains(g.Me.Name.ToUpperInvariant(), ticker[0].Text);
        Assert.Contains(rival.Name.ToUpperInvariant(), ticker[1].Text);
        Assert.True(ticker[0].Tick >= ticker[1].Tick);
    }

    [Fact]
    public void The_screen_can_ask_for_fewer_and_the_default_comes_from_the_data()
    {
        Game g = World();
        for (int i = 0; i < 20; i++) Add(g, EventKind.Bankrupt, g.Id);
        Assert.Equal(g.C.News.DefaultHeadlines, NewsTicker.Headlines(g.C, g.State).Count);
        Assert.Equal(3, NewsTicker.Headlines(g.C, g.State, 3).Count);
        Assert.All(NewsTicker.Headlines(g.C, g.State), h => Assert.False(h.IsFiller));
    }

    [Fact]
    public void A_quiet_world_is_padded_with_filler_that_is_not_repeated_and_not_from_the_future()
    {
        Game g = World();
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State);
        Assert.Equal(g.C.News.MinHeadlines, ticker.Count);
        Assert.All(ticker, h => Assert.True(h.IsFiller));
        Assert.Equal(ticker.Count, ticker.Select(h => h.LineId).Distinct().Count());
        int year = GameDate.Year(g.C.Calendar, g.Tick);
        Assert.All(ticker, h => Assert.True(g.C.News.Filler.First(f => f.IdValue == h.LineId).FromYear <= year));
        Assert.All(ticker, h => Assert.Equal(h.Text.ToUpperInvariant(), h.Text));
    }

    [Fact]
    public void Filler_waits_for_its_year()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = TestData.Config().Eras[0].Id, RivalIds = new List<string>() };
        var early = new Game(Simulation.NewGame(TestData.Config(), options));
        early.State.Events.Clear();
        Assert.True(GameDate.Year(early.C.Calendar, early.Tick) < 2018);
        Assert.DoesNotContain(NewsTicker.Headlines(early.C, early.State), h => h.LineId == "power");
        Assert.NotEmpty(NewsTicker.Headlines(early.C, early.State));
    }

    [Fact]
    public void Real_news_pushes_filler_out()
    {
        Game g = World();
        for (int i = 0; i < g.C.News.MinHeadlines; i++) Add(g, EventKind.Bankrupt, g.Id);
        Assert.All(NewsTicker.Headlines(g.C, g.State), h => Assert.False(h.IsFiller));
        Game one = World();
        Add(one, EventKind.Bankrupt, one.Id);
        List<Headline> mixed = NewsTicker.Headlines(one.C, one.State);
        Assert.Equal(one.C.News.MinHeadlines, mixed.Count);
        Assert.False(mixed[0].IsFiller);
        Assert.All(mixed.Skip(1), h => Assert.True(h.IsFiller));
    }

    [Fact]
    public void The_ticker_is_the_same_when_asked_twice_and_changes_nothing()
    {
        Game g = Game.New(1);
        g.Weeks(60);
        string before = g.Sim.Digest();
        List<Headline> a = NewsTicker.Headlines(g.C, g.State);
        List<Headline> b = NewsTicker.Headlines(g.C, g.State);
        Assert.Equal(a.Select(h => h.Text).ToList(), b.Select(h => h.Text).ToList());
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void A_real_run_gives_a_full_ticker_of_clean_capitals()
    {
        Game g = Game.New(1);
        g.Autopilot(3 * g.C.Calendar.WeeksPerYear);
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State);
        Assert.True(ticker.Count >= g.C.News.MinHeadlines);
        Assert.Contains(ticker, h => !h.IsFiller);
        Assert.All(ticker, h =>
        {
            Assert.False(string.IsNullOrWhiteSpace(h.Text));
            Assert.DoesNotContain("{", h.Text);
            Assert.Equal(h.Text.ToUpperInvariant(), h.Text);
        });
    }
}
