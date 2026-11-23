using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SandboxPlaybookOrchRouteMetricsNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.SandboxStarted, EventKind.SandboxFinished,
        EventKind.PlaybookSet, EventKind.PlaybookArmed,
        EventKind.OrchBudgetSet, EventKind.RouteShareSet,
        EventKind.MetricsPublished, EventKind.MetricsStopped,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.SandboxStarted, EventKind.PlaybookSet, EventKind.OrchBudgetSet, EventKind.RouteShareSet, EventKind.MetricsStopped, EventKind.SandboxFinished,
    };

    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, string text = "mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 7000, text);
        e.Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind + " " + pick);
        return ticker[0].Text;
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(405, c.News.Headlines.Count);
        Assert.Equal(358, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            if (!NoChat.Contains(kind)) Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab, Headline(g, EventKind.SandboxStarted, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.SandboxFinished, 1));
        Assert.StartsWith(lab, Headline(g, EventKind.SandboxFinished, 0));
        Assert.StartsWith(lab, Headline(g, EventKind.PlaybookArmed, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.MetricsPublished, -1));
    }
}