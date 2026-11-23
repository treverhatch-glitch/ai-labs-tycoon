using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for API integrations, conformity assessment, the lab structure, the cluster fabric and the incident register.</summary>
public class IntegrationsConformityOrgFabricRegisterNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.ApiOpened, EventKind.ApiDeprecated, EventKind.ApiEnded,
        EventKind.ConformityStarted, EventKind.ConformityDecided, EventKind.ConformityEnded,
        EventKind.OrgChanged, EventKind.OrgStrained,
        EventKind.FabricOrdered, EventKind.FabricDelivered, EventKind.FabricCancelled,
        EventKind.RegisterChanged, EventKind.ReportFiled, EventKind.ReportLate, EventKind.InspectionHeld,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.ApiOpened, EventKind.ApiDeprecated, EventKind.ConformityStarted, EventKind.FabricOrdered, EventKind.FabricCancelled,
        EventKind.RegisterChanged, EventKind.ReportFiled,
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
        Assert.Equal(391, c.News.Headlines.Count);
        Assert.Equal(346, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " OPENS A PUBLIC API FOR MINI", Headline(g, EventKind.ApiOpened, -1));
        Assert.StartsWith(lab + " SETS AN END DATE FOR THE API ON MINI", Headline(g, EventKind.ApiDeprecated, -1));
        Assert.StartsWith(lab + " CLOSES THE API ON MINI WITH NOBODY LEFT", Headline(g, EventKind.ApiEnded, 1));
        Assert.StartsWith(lab + " TURNS OFF THE API ON MINI WITH CUSTOMERS STILL", Headline(g, EventKind.ApiEnded, 0));
        Assert.StartsWith(lab + " SENDS A CONFORMITY DOSSIER ON MINI", Headline(g, EventKind.ConformityStarted, 2));
        Assert.StartsWith(lab + "'S MINI PASSES ITS CONFORMITY ASSESSMENT", Headline(g, EventKind.ConformityDecided, 1));
        Assert.StartsWith(lab + "'S MINI FAILS ITS CONFORMITY REVIEW", Headline(g, EventKind.ConformityDecided, 0));
        Assert.StartsWith(lab + "'S CONFORMITY MARK ON MINI RUNS OUT", Headline(g, EventKind.ConformityEnded, 1));
        Assert.StartsWith(lab + "'S CONFORMITY MARK ON MINI IS SUSPENDED", Headline(g, EventKind.ConformityEnded, 2));
        Assert.StartsWith(lab + " WITHDRAWS MINI FROM CONFORMITY ASSESSMENT", Headline(g, EventKind.ConformityEnded, 0));
        Assert.StartsWith(lab + " ANNOUNCES A NEW WAY OF ORGANISING ITSELF", Headline(g, EventKind.OrgChanged, 2));
        Assert.StartsWith("PEOPLE AT " + lab + " SAY THE STRUCTURE", Headline(g, EventKind.OrgStrained, -1));
        Assert.StartsWith(lab + " ORDERS A FASTER FABRIC", Headline(g, EventKind.FabricOrdered, -1));
        Assert.StartsWith(lab + " FINISHES THE NEW FABRIC", Headline(g, EventKind.FabricDelivered, -1));
        Assert.StartsWith(lab + " CANCELS ITS CLUSTER FABRIC ORDER", Headline(g, EventKind.FabricCancelled, -1));
        Assert.StartsWith(lab + " JOINS THE VOLUNTARY INCIDENT REGISTER", Headline(g, EventKind.RegisterChanged, 1));
        Assert.StartsWith(lab + " LEAVES THE INCIDENT REGISTER", Headline(g, EventKind.RegisterChanged, 0));
        Assert.StartsWith(lab + " FILES A FULL INCIDENT REPORT", Headline(g, EventKind.ReportFiled, 2, ""));
        Assert.StartsWith(lab + " FILES A SHORT INCIDENT REPORT", Headline(g, EventKind.ReportFiled, 1, ""));
        Assert.StartsWith(lab + " IS LATE WITH AN INCIDENT REPORT", Headline(g, EventKind.ReportLate, -1, ""));
        Assert.StartsWith("THE INCIDENT REGISTER INSPECTS " + lab + " AND FINDS NOTHING", Headline(g, EventKind.InspectionHeld, 1, ""));
        Assert.StartsWith("THE INCIDENT REGISTER INSPECTS " + lab + " AND FINES IT", Headline(g, EventKind.InspectionHeld, 0, ""));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("api-ended-chat", Say(g, EventKind.ApiEnded, 1));
        Assert.Equal("conformity-decided-chat", Say(g, EventKind.ConformityDecided, 0));
        Assert.Equal("conformity-ended-chat", Say(g, EventKind.ConformityEnded, 2));
        Assert.Equal("org-changed-chat", Say(g, EventKind.OrgChanged, 1));
        Assert.Equal("org-strained-chat", Say(g, EventKind.OrgStrained, -1));
        Assert.Equal("fabric-delivered-chat", Say(g, EventKind.FabricDelivered, -1));
        Assert.Equal("report-late-chat", Say(g, EventKind.ReportLate, -1));
        Assert.Equal("inspection-chat", Say(g, EventKind.InspectionHeld, 0));
    }
}
