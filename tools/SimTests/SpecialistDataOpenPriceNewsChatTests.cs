using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SpecialistDataOpenPriceNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.SpecialistBranched,
        EventKind.DataSourceAcquired, EventKind.DataSourceRenewed, EventKind.DataSourceLapsed,
        EventKind.WeightsOpened, EventKind.WeightsClosed, EventKind.WeightsCopied,
        EventKind.SectorPriceSet,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.DataSourceAcquired, EventKind.DataSourceRenewed, EventKind.WeightsClosed,
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
        Assert.StartsWith(lab, Headline(g, EventKind.SpecialistBranched, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.DataSourceLapsed, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.WeightsOpened, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.SectorPriceSet, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.SectorPriceSet, 1));
    }
}