using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the data pipeline, users' data, governance, round buybacks by board vote and spot capacity.</summary>
public class DatasetsUserDataGovernanceBuybackSpotNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.DataAcquired, EventKind.DataCleaned, EventKind.DataLicensed, EventKind.DataSuit,
        EventKind.UserDataChanged, EventKind.UserDataComplaint, EventKind.UserDataDeleted,
        EventKind.GovernanceChanged, EventKind.DirectorSeated, EventKind.DirectorResigned,
        EventKind.BoardVoted, EventKind.RoundBoughtBack, EventKind.SpotPreempted,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.DataAcquired, EventKind.DataLicensed, EventKind.UserDataChanged, EventKind.UserDataDeleted, EventKind.GovernanceChanged,
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

    static string Say(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 7000, "mini");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single().LineId;
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(421, c.News.Headlines.Count);
        Assert.Equal(374, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " TAKES DELIVERY OF A NEW PILE", Headline(g, EventKind.DataAcquired, -1, ""));
        Assert.StartsWith(lab + " FINISHES CLEANING A BATCH", Headline(g, EventKind.DataCleaned, -1, ""));
        Assert.StartsWith(lab + " SIGNS A DATA LICENCE", Headline(g, EventKind.DataLicensed, -1, ""));
        Assert.StartsWith(lab + " IS SUED OVER SCRAPED TRAINING DATA", Headline(g, EventKind.DataSuit, -1, ""));
        Assert.StartsWith(lab + " CHANGES WHAT IT DOES WITH ITS USERS", Headline(g, EventKind.UserDataChanged, -1, ""));
        Assert.StartsWith("A PRIVACY REGULATOR FINES " + lab, Headline(g, EventKind.UserDataComplaint, -1, ""));
        Assert.StartsWith(lab + " DELETES MOST OF THE USER DATA", Headline(g, EventKind.UserDataDeleted, -1, ""));
        Assert.StartsWith(lab + " CHANGES WHO CONTROLS THE COMPANY", Headline(g, EventKind.GovernanceChanged, -1, ""));
        Assert.StartsWith(lab + " SEATS AN INDEPENDENT DIRECTOR", Headline(g, EventKind.DirectorSeated, -1, ""));
        Assert.StartsWith("AN INDEPENDENT DIRECTOR RESIGNS FROM " + lab, Headline(g, EventKind.DirectorResigned, -1, ""));
        Assert.StartsWith("THE BOARD OF " + lab + " VOTES TO BUY ITS INVESTORS OUT", Headline(g, EventKind.BoardVoted, 1, "angel"));
        Assert.StartsWith("THE BOARD OF " + lab + " VOTES DOWN A BUYBACK", Headline(g, EventKind.BoardVoted, 0, "angel"));
        Assert.StartsWith(lab + " BUYS A FUNDING ROUND BACK", Headline(g, EventKind.RoundBoughtBack, -1, "angel"));
        Assert.StartsWith("THE CLOUD TAKES BACK " + lab + "'S SPOT CAPACITY AND MINI LOSES", Headline(g, EventKind.SpotPreempted, -1));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("data-cleaned-chat", Say(g, EventKind.DataCleaned, -1));
        Assert.Equal("data-suit-chat", Say(g, EventKind.DataSuit, -1));
        Assert.Equal("user-data-complaint-chat", Say(g, EventKind.UserDataComplaint, -1));
        Assert.Equal("director-seated-chat", Say(g, EventKind.DirectorSeated, -1));
        Assert.Equal("director-resigned-chat", Say(g, EventKind.DirectorResigned, -1));
        Assert.Equal("board-voted-chat", Say(g, EventKind.BoardVoted, 0));
        Assert.Equal("round-bought-chat", Say(g, EventKind.RoundBoughtBack, -1));
        Assert.Equal("spot-preempted-chat", Say(g, EventKind.SpotPreempted, -1));
    }
}
