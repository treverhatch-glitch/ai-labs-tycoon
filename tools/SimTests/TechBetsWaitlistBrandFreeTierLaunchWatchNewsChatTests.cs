using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for tech bets, the waitlist, employer brand, the free tier and the launch watch.</summary>
public class TechBetsWaitlistBrandFreeTierLaunchWatchNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.TechBetMade, EventKind.TechBetEnded, EventKind.WaitlistOpened, EventKind.WaitlistEnded,
        EventKind.CareersOpened, EventKind.BadReview, EventKind.ReviewAnswered,
        EventKind.FreeTierOpened, EventKind.FreeTierClosed,
        EventKind.WatchDeskOpened, EventKind.RivalLaunchSeen, EventKind.LaunchAnswered,
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
        Assert.Equal(431, c.News.Headlines.Count);
        Assert.Equal(384, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " TELLS THE WORLD IT WILL HAVE MINI WORKING", Headline(g, EventKind.TechBetMade, -1));
        Assert.StartsWith(lab + " DELIVERS MINI AS PROMISED", Headline(g, EventKind.TechBetEnded, 1));
        Assert.StartsWith(lab + " RUNS OUT OF TIME ON MINI", Headline(g, EventKind.TechBetEnded, 0));
        Assert.StartsWith(lab + " QUIETLY TAKES BACK ITS PROMISE ON MINI", Headline(g, EventKind.TechBetEnded, 2));
        Assert.StartsWith(lab + " OPENS A WAITLIST FOR A MODEL THAT IS NOT FINISHED", Headline(g, EventKind.WaitlistOpened, -1));
        Assert.StartsWith(lab + " SHIPS TO A WAITING LIST IN TIME", Headline(g, EventKind.WaitlistEnded, 1));
        Assert.StartsWith(lab + " LETS ITS WAITLIST PROMISE LAPSE", Headline(g, EventKind.WaitlistEnded, 0));
        Assert.StartsWith(lab + " CLOSES ITS WAITLIST WITHOUT A LAUNCH", Headline(g, EventKind.WaitlistEnded, 2));
        Assert.StartsWith(lab + " PUTS UP A CAREERS PAGE", Headline(g, EventKind.CareersOpened, -1));
        Assert.StartsWith("AN ANONYMOUS REVIEW OF " + lab + " AS A PLACE TO WORK", Headline(g, EventKind.BadReview, -1));
        Assert.StartsWith(lab + " OWNS UP TO A BAD REVIEW", Headline(g, EventKind.ReviewAnswered, 1));
        Assert.StartsWith("NOBODY AT " + lab + " ANSWERS A BAD REVIEW", Headline(g, EventKind.ReviewAnswered, 0));
        Assert.StartsWith(lab + " ANSWERS A BAD REVIEW IN ITS OWN WAY", Headline(g, EventKind.ReviewAnswered, 2));
        Assert.StartsWith(lab + " OPENS A FREE TIER", Headline(g, EventKind.FreeTierOpened, -1));
        Assert.StartsWith(lab + " SHUTS ITS FREE TIER", Headline(g, EventKind.FreeTierClosed, 1));
        Assert.StartsWith(lab + "'S FREE TIER ENDS WITH THE MODEL", Headline(g, EventKind.FreeTierClosed, 0));
        Assert.StartsWith(lab + " SETS UP A DESK TO WATCH RIVAL LAUNCHES", Headline(g, EventKind.WatchDeskOpened, -1));
        Assert.StartsWith("A RIVAL LAUNCHES A MODEL THAT BEATS " + lab + "'S BEST", Headline(g, EventKind.RivalLaunchSeen, 1));
        Assert.StartsWith("A RIVAL LAUNCHES A MODEL " + lab + " ALREADY BEATS", Headline(g, EventKind.RivalLaunchSeen, 0));
        Assert.StartsWith(lab + " COUNTER-PROGRAMMES A RIVAL'S LAUNCH", Headline(g, EventKind.LaunchAnswered, 1));
        Assert.StartsWith("NOBODY AT " + lab + " ANSWERS A RIVAL'S LAUNCH IN TIME", Headline(g, EventKind.LaunchAnswered, 0));
        Assert.StartsWith(lab + " ANSWERS A RIVAL'S LAUNCH IN ITS OWN WAY", Headline(g, EventKind.LaunchAnswered, 3));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("techbet-made-chat", Say(g, EventKind.TechBetMade, -1));
        Assert.Equal("techbet-ended-chat", Say(g, EventKind.TechBetEnded, 1));
        Assert.Equal("waitlist-opened-chat", Say(g, EventKind.WaitlistOpened, -1));
        Assert.Equal("waitlist-ended-chat", Say(g, EventKind.WaitlistEnded, 2));
        Assert.Equal("careers-opened-chat", Say(g, EventKind.CareersOpened, -1));
        Assert.Equal("review-bad-chat", Say(g, EventKind.BadReview, -1));
        Assert.Equal("review-answered-chat", Say(g, EventKind.ReviewAnswered, 3));
        Assert.Equal("freetier-opened-chat", Say(g, EventKind.FreeTierOpened, -1));
        Assert.Equal("freetier-closed-chat", Say(g, EventKind.FreeTierClosed, 1));
        Assert.Equal("watch-opened-chat", Say(g, EventKind.WatchDeskOpened, -1));
        Assert.Equal("watch-seen-chat", Say(g, EventKind.RivalLaunchSeen, 1));
        Assert.Equal("watch-answered-chat", Say(g, EventKind.LaunchAnswered, 2));
    }
}
