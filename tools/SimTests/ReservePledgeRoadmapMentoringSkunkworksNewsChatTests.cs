using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the reserve, the pledge, the roadmap, mentoring and skunkworks.</summary>
public class ReservePledgeRoadmapMentoringSkunkworksNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 2_000_000, string text = "", int subject = -1)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, subject, amount, text);
        e.Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind + " " + pick);
        return ticker[0].Text;
    }

    static Game Lab(params string[] roles)
    {
        Game g = Game.Solo();
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        foreach (string role in roles) g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 });
        return g;
    }

    static ChatMessage Say(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "game-ai");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(233, c.News.Headlines.Count);
        Assert.Equal(260, c.Chat.Lines.Count);
        foreach (string kind in new[] { "ReserveMoved", "ReserveCovered", "PledgeSet", "PledgeBroken", "RoadmapAnnounced", "RoadmapSettled", "MentoringStarted", "MentoringEnded", "SkunkworksStarted", "SkunkworksDone" })
        {
            if (kind != "MentoringStarted") Assert.Contains(c.News.Headlines, h => h.Kind == kind);
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " SETS $2M ASIDE", Headline(g, EventKind.ReserveMoved, 1));
        Assert.StartsWith(lab + " TAKES $2M BACK OUT OF ITS RESERVE", Headline(g, EventKind.ReserveMoved, 0));
        Assert.StartsWith(lab + "'S RESERVE COVERS A DEBT OF $2M", Headline(g, EventKind.ReserveCovered, -1));
        Assert.StartsWith(lab + " PLEDGES PUBLICLY", Headline(g, EventKind.PledgeSet, 1, 0, "", 2));
        Assert.StartsWith(lab + " WALKS BACK ITS SAFETY PLEDGE", Headline(g, EventKind.PledgeSet, 0, 15, "", 1));
        Assert.StartsWith(lab + " BREAKS ITS SAFETY PLEDGE", Headline(g, EventKind.PledgeBroken, 2, 600, "Mini"));
        Assert.StartsWith(lab + " PUBLISHES A ROADMAP: CAPABILITY 150", Headline(g, EventKind.RoadmapAnnounced, 26, 150, "", 2));
        Assert.StartsWith(lab + " DELIVERS ITS ROADMAP", Headline(g, EventKind.RoadmapSettled, 1, 150, "", 2));
        Assert.StartsWith(lab + " MISSES ITS ROADMAP", Headline(g, EventKind.RoadmapSettled, 0, 150, "", 2));
        Assert.StartsWith(lab + " GRADUATES A JUNIOR TO SENIOR", Headline(g, EventKind.MentoringEnded, 0, 26, "researcher", 7));
        Assert.StartsWith(lab + "'S MENTORING PAIR BREAKS UP", Headline(g, EventKind.MentoringEnded, 1, 9, "researcher", 7));
        Assert.StartsWith(lab + " PUTS 3 PEOPLE ON A SKUNKWORKS PROJECT", Headline(g, EventKind.SkunkworksStarted, 13, 3, "research", 1));
        Assert.StartsWith(lab + "'S SKUNKWORKS TEAM RESURFACES", Headline(g, EventKind.SkunkworksDone, 1, 300, "research", 1));
        Assert.StartsWith(lab + " WRAPS A SKUNKWORKS PROJECT UP EARLY", Headline(g, EventKind.SkunkworksDone, 0, 100, "research", 1));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("reserve-set-chat", Say(Lab("sales"), EventKind.ReserveMoved, 1).LineId);
        Assert.Equal("reserve-cover-chat", Say(Lab("sales"), EventKind.ReserveCovered, -1).LineId);
        Assert.Equal("pledge-made-chat-safety", Say(Lab("safety"), EventKind.PledgeSet, 1).LineId);
        Assert.Equal("pledge-made-chat", Say(Lab("sales"), EventKind.PledgeSet, 1).LineId);
        Assert.Equal("pledge-broken-chat", Say(Lab("sales"), EventKind.PledgeBroken, 1).LineId);
        Assert.Equal("roadmap-announced-chat-researcher", Say(Lab("researcher"), EventKind.RoadmapAnnounced, 26).LineId);
        Assert.Equal("roadmap-announced-chat", Say(Lab("sales"), EventKind.RoadmapAnnounced, 26).LineId);
        Assert.Equal("roadmap-delivered-chat", Say(Lab("sales"), EventKind.RoadmapSettled, 1).LineId);
        Assert.Equal("roadmap-missed-chat", Say(Lab("sales"), EventKind.RoadmapSettled, 0).LineId);
        Assert.Equal("mentoring-started-chat", Say(Lab("sales"), EventKind.MentoringStarted, -1).LineId);
        Assert.Equal("mentoring-graduated-chat", Say(Lab("sales"), EventKind.MentoringEnded, 0).LineId);
        Assert.Equal("skunkworks-started-chat-researcher", Say(Lab("researcher"), EventKind.SkunkworksStarted, 13).LineId);
        Assert.Equal("skunkworks-started-chat", Say(Lab("sales"), EventKind.SkunkworksStarted, 13).LineId);
        Assert.Equal("skunkworks-done-chat", Say(Lab("sales"), EventKind.SkunkworksDone, 1).LineId);
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo(2, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 3, safety: 2, gpus: 8);
        g.Do(Command.SetAsideReserve(g.Id, 500_000));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("ASIDE"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "reserve-set-chat");
        g.Do(Command.SetPledge(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PLEDGES PUBLICLY"));
        ModelState m1 = g.Train("mini", 300);
        g.UntilTrained(m1);
        g.Do(Command.ReleaseModel(g.Id, m1.Id, "game-ai"));
        g.Weeks(5);
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 20));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PUBLISHES A ROADMAP"));
        StaffMember mentor = g.Me.Staff.First(p => p.RoleId == "researcher");
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, g.Me.Staff.Where(p => p.RoleId == "researcher").Skip(1).First().Id));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "mentoring-started-chat");
        g.Do(Command.EndMentoring(g.Id, g.Me.Staff.First(p => p.MentorId > 0).Id));
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 8));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SKUNKWORKS PROJECT"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("skunkworks-started-chat"));
        g.Weeks(8);
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SKUNKWORKS TEAM RESURFACES"));
    }
}
