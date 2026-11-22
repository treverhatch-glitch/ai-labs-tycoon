using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for devices, platform deals, the lab's values, world shocks and replication.</summary>
public class DevicesPlatformsCreedShocksReplicationNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.DeviceDesigned, EventKind.DeviceLaunched, EventKind.DeviceRecalled, EventKind.DeviceWithdrawn,
        EventKind.PlatformSigned, EventKind.PlatformHoldup, EventKind.PlatformRenewed, EventKind.PlatformEnded,
        EventKind.CreedSet, EventKind.TownHallHeld, EventKind.CreedOwnedUp, EventKind.CreedHollow, EventKind.CreedProud,
        EventKind.ShockStruck, EventKind.ShockAnswered,
        EventKind.CodeReleased, EventKind.CodeWithheld, EventKind.PaperReplicated,
    };

    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 7000, string text = "mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, amount, text);
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
        Assert.Equal(357, c.News.Headlines.Count);
        Assert.Equal(327, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        foreach (EventKind kind in Kinds.Where(k => k != EventKind.ShockStruck))
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " STARTS DESIGNING A GADGET", Headline(g, EventKind.DeviceDesigned, -1));
        Assert.StartsWith(lab + " LAUNCHES A GADGET AND THE CROWD APPLAUDS", Headline(g, EventKind.DeviceLaunched, 1));
        Assert.StartsWith(lab + " LAUNCHES A GADGET ON STAGE AND IT DOES NOT WORK", Headline(g, EventKind.DeviceLaunched, 0));
        Assert.StartsWith(lab + " RECALLS ITS GADGET AT A COST OF 7000", Headline(g, EventKind.DeviceRecalled, -1));
        Assert.StartsWith(lab + " ENDS ITS GADGET LINE AFTER 7000 UNITS", Headline(g, EventKind.DeviceWithdrawn, -1));
        Assert.StartsWith(lab + " SIGNS A DISTRIBUTION DEAL", Headline(g, EventKind.PlatformSigned, -1));
        Assert.StartsWith("THE PLATFORM RAISES ITS CUT ON " + lab + " TO 7000 PERMILLE", Headline(g, EventKind.PlatformHoldup, -1));
        Assert.StartsWith(lab + " RENEWS ITS DISTRIBUTION DEAL AT A 7000 PERMILLE CUT", Headline(g, EventKind.PlatformRenewed, -1));
        Assert.StartsWith(lab + " WALKS AWAY FROM ITS DISTRIBUTION DEAL", Headline(g, EventKind.PlatformEnded, 0));
        Assert.StartsWith(lab + "'S DISTRIBUTION DEAL RUNS OUT", Headline(g, EventKind.PlatformEnded, 1));
        Assert.StartsWith(lab + " PUTS ITS VALUES IN WRITING", Headline(g, EventKind.CreedSet, -1));
        Assert.StartsWith(lab + " HOLDS A TOWN HALL ON ITS VALUES", Headline(g, EventKind.TownHallHeld, -1));
        Assert.StartsWith(lab + " TELLS ITS STAFF IT HAS NOT LIVED UP TO ITS VALUES", Headline(g, EventKind.CreedOwnedUp, -1));
        Assert.StartsWith(lab + "'S STAFF NOW LAUGH AT ITS VALUES", Headline(g, EventKind.CreedHollow, -1));
        Assert.StartsWith(lab + " IS HELD UP AS A LAB THAT LIVES ITS VALUES", Headline(g, EventKind.CreedProud, -1));
        Assert.StartsWith(lab + " BRACES FOR THE NEWS", Headline(g, EventKind.ShockAnswered, 1));
        Assert.StartsWith(lab + " MEETS THE NEWS IN ITS OWN WAY", Headline(g, EventKind.ShockAnswered, 2));
        Assert.StartsWith(lab + " PUTS A PAPER'S CODE AND DATA OUT", Headline(g, EventKind.CodeReleased, -1));
        Assert.StartsWith(lab + " KEEPS A PAPER'S CODE AND DATA BACK", Headline(g, EventKind.CodeWithheld, 0));
        Assert.StartsWith(lab + " DITHERS UNTIL THE CODE", Headline(g, EventKind.CodeWithheld, 1));
        Assert.StartsWith("ANOTHER LAB REPRODUCES " + lab + "'S PAPER", Headline(g, EventKind.PaperReplicated, 1));
        Assert.StartsWith("ANOTHER LAB CANNOT REPRODUCE " + lab + "'S PAPER", Headline(g, EventKind.PaperReplicated, 0));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("device-designed-chat", Say(g, EventKind.DeviceDesigned, -1));
        Assert.Equal("device-launched-chat", Say(g, EventKind.DeviceLaunched, 1));
        Assert.Equal("device-recalled-chat", Say(g, EventKind.DeviceRecalled, -1));
        Assert.Equal("device-withdrawn-chat", Say(g, EventKind.DeviceWithdrawn, -1));
        Assert.Equal("platform-signed-chat", Say(g, EventKind.PlatformSigned, -1));
        Assert.Equal("platform-holdup-chat", Say(g, EventKind.PlatformHoldup, -1));
        Assert.Equal("platform-renewed-chat", Say(g, EventKind.PlatformRenewed, -1));
        Assert.Equal("platform-ended-chat", Say(g, EventKind.PlatformEnded, 1));
        Assert.Equal("creed-set-chat", Say(g, EventKind.CreedSet, -1));
        Assert.Equal("town-hall-chat", Say(g, EventKind.TownHallHeld, -1));
        Assert.Equal("creed-owned-up-chat", Say(g, EventKind.CreedOwnedUp, -1));
        Assert.Equal("creed-hollow-chat", Say(g, EventKind.CreedHollow, -1));
        Assert.Equal("creed-proud-chat", Say(g, EventKind.CreedProud, -1));
        Assert.Equal("shock-struck-chat", Say(g, EventKind.ShockStruck, -1));
        Assert.Equal("shock-answered-chat", Say(g, EventKind.ShockAnswered, 4));
        Assert.Equal("code-released-chat", Say(g, EventKind.CodeReleased, -1));
        Assert.Equal("code-withheld-chat", Say(g, EventKind.CodeWithheld, 1));
        Assert.Equal("paper-replicated-chat", Say(g, EventKind.PaperReplicated, 0));
    }
}
