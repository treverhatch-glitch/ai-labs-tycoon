using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for chairs, whistleblowers, the bug bounty, sector pullouts and loan restructures.</summary>
public class ChairsWhistleBountyPulloutRestructureNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 2_000_000, string text = "Mini", int subject = 2)
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
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "Mini");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(320, c.News.Headlines.Count);
        Assert.Equal(308, c.Chat.Lines.Count);
        foreach (string kind in new[] { "ChairEndowed", "WhistleRaised", "WhistleAnswered", "WhistleLeaked", "BountySet", "SectorPulledOut", "LoanRestructured" })
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind);
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " ENDOWS A UNIVERSITY CHAIR FOR $2M", Headline(g, EventKind.ChairEndowed, -1));
        Assert.StartsWith("A SAFETY PERSON AT " + lab, Headline(g, EventKind.WhistleRaised, -1, 200, "Mini", -1));
        Assert.StartsWith(lab + " COMES CLEAN ABOUT", Headline(g, EventKind.WhistleAnswered, 0, 0, "Mini", -1));
        Assert.StartsWith(lab + " 'RESOLVES' AN INTERNAL CONCERN ABOUT", Headline(g, EventKind.WhistleAnswered, 1, 250000, "Mini", -1));
        Assert.StartsWith("INSIDER LEAKS " + lab, Headline(g, EventKind.WhistleLeaked, 1, 60, "Mini", -1));
        Assert.Contains("A SAFETY PERSON RESIGNS", Headline(g, EventKind.WhistleLeaked, 1, 60, "Mini", -1));
        Assert.DoesNotContain("RESIGNS", Headline(g, EventKind.WhistleLeaked, 0, 60, "Mini", -1));
        Assert.StartsWith(lab + " RAISES ITS BUG BOUNTY", Headline(g, EventKind.BountySet, 1, 40000, "", 2));
        Assert.StartsWith(lab + " SCALES BACK ITS BUG BOUNTY", Headline(g, EventKind.BountySet, 0, 0, "", 1));
        Assert.StartsWith(lab + " PULLS", Headline(g, EventKind.SectorPulledOut, 0, 80000, "Game AI", -1));
        Assert.Contains("OUT OF GAME AI", Headline(g, EventKind.SectorPulledOut, 0, 80000, "Game AI", -1));
        Assert.StartsWith(lab + " LEAVES GAME AI AND RETIRES", Headline(g, EventKind.SectorPulledOut, 1, 0, "Game AI", -1));
        Assert.StartsWith(lab + " 'EXTENDS ITS RUNWAY'", Headline(g, EventKind.LoanRestructured, 0, 1000, "", 4));
        Assert.StartsWith(lab + " RESTRUCTURES A LOAN IT HAD STOPPED PAYING", Headline(g, EventKind.LoanRestructured, 1, 1000, "", 4));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("chair-endowed-chat-researcher", Say(Lab("researcher"), EventKind.ChairEndowed, -1).LineId);
        Assert.Equal("chair-endowed-chat", Say(Lab("sales"), EventKind.ChairEndowed, -1).LineId);
        Assert.Equal("whistle-raised-chat-safety", Say(Lab("safety"), EventKind.WhistleRaised, -1).LineId);
        Assert.Equal("whistle-raised-chat", Say(Lab("sales"), EventKind.WhistleRaised, -1).LineId);
        Assert.Equal("whistle-clean-chat", Say(Lab("sales"), EventKind.WhistleAnswered, 0).LineId);
        Assert.Equal("whistle-silence-chat", Say(Lab("sales"), EventKind.WhistleAnswered, 1).LineId);
        Assert.Equal("whistle-leaked-chat-resign", Say(Lab("sales"), EventKind.WhistleLeaked, 1).LineId);
        Assert.Equal("whistle-leaked-chat", Say(Lab("sales"), EventKind.WhistleLeaked, 0).LineId);
        Assert.Equal("bounty-raised-chat", Say(Lab("sales"), EventKind.BountySet, 1).LineId);
        Assert.Equal("bounty-lowered-chat", Say(Lab("sales"), EventKind.BountySet, 0).LineId);
        Assert.Equal("pullout-chat", Say(Lab("sales"), EventKind.SectorPulledOut, 0).LineId);
        Assert.Equal("pullout-last-chat", Say(Lab("sales"), EventKind.SectorPulledOut, 1).LineId);
        Assert.Equal("loan-restructured-chat-sales", Say(Lab("sales"), EventKind.LoanRestructured, 0).LineId);
        Assert.Equal("loan-restructured-chat", Say(Lab("engineer"), EventKind.LoanRestructured, 1).LineId);
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo(2, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 3, safety: 2, gpus: 8);
        g.Do(Command.EndowChair(g.Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("ENDOWS A UNIVERSITY CHAIR"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("chair-endowed-chat"));
        ModelState m1 = g.Train("mini", 300);
        g.UntilTrained(m1);
        g.Do(Command.ReleaseModel(g.Id, m1.Id, "game-ai", "game-bots"));
        g.Weeks(5);
        g.Do(Command.SetBugBounty(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("RAISES ITS BUG BOUNTY"));
        g.Do(Command.PullOutOfSector(g.Id, m1.Id, "game-bots"));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PULLS") && h.Text.Contains("OUT OF"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("pullout"));
    }
}
