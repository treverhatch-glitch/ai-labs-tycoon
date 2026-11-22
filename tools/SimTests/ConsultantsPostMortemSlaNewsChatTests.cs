using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for policy consultants, public post-mortems, the service-level pledge and research grants.</summary>
public class ConsultantsPostMortemSlaNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 2, 0, "Mini");
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
        Assert.Equal(298, c.News.Headlines.Count);
        Assert.Equal(300, c.Chat.Lines.Count);
        foreach (string kind in new[] { "ConsultantsHired", "PostMortemPublished", "SlaPledged", "SlaBroken", "GrantDecided", "GrantSettled" })
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
        Assert.StartsWith(lab + " HIRES POLICY CONSULTANTS", Headline(g, EventKind.ConsultantsHired, 1));
        Assert.StartsWith(lab + "'S POLICY CONSULTANTS HEAD HOME", Headline(g, EventKind.ConsultantsHired, 0));
        Assert.Contains("SHORT NOTE ON WHAT WENT WRONG WITH MINI", Headline(g, EventKind.PostMortemPublished, 0));
        Assert.Contains("FULL POST-MORTEM OF MINI", Headline(g, EventKind.PostMortemPublished, 1));
        Assert.StartsWith(lab + " PLEDGES A SERVICE LEVEL", Headline(g, EventKind.SlaPledged, 1));
        Assert.StartsWith(lab + " LOWERS ITS SERVICE-LEVEL PLEDGE", Headline(g, EventKind.SlaPledged, 0));
        Assert.StartsWith(lab + " MISSES ITS SERVICE LEVEL", Headline(g, EventKind.SlaBroken, 0));
        Assert.StartsWith(lab + " BREAKS ITS SERVICE-LEVEL PLEDGE", Headline(g, EventKind.SlaBroken, 1));
        Assert.StartsWith(lab + " WINS A RESEARCH GRANT", Headline(g, EventKind.GrantDecided, 1));
        Assert.StartsWith(lab + "'S GRANT APPLICATION IS DECLINED", Headline(g, EventKind.GrantDecided, 0));
        Assert.StartsWith(lab + " DELIVERS THE PAPERS ITS GRANT ASKED FOR", Headline(g, EventKind.GrantSettled, 1));
        Assert.StartsWith(lab + " MISSES ITS GRANT DEADLINE", Headline(g, EventKind.GrantSettled, 0));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("consultants-chat-policy", Say(Lab("policy"), EventKind.ConsultantsHired, 1).LineId);
        Assert.Equal("consultants-chat", Say(Lab("sales"), EventKind.ConsultantsHired, 1).LineId);
        Assert.Equal("post-mortem-chat-safety", Say(Lab("safety"), EventKind.PostMortemPublished, 1).LineId);
        Assert.Equal("post-mortem-chat", Say(Lab("sales"), EventKind.PostMortemPublished, 1).LineId);
        Assert.Equal("post-mortem-summary-chat", Say(Lab("sales"), EventKind.PostMortemPublished, 0).LineId);
        Assert.Equal("sla-chat-engineer", Say(Lab("engineer"), EventKind.SlaPledged, 1).LineId);
        Assert.Equal("sla-chat", Say(Lab("sales"), EventKind.SlaPledged, 1).LineId);
        Assert.Equal("sla-broken-chat", Say(Lab("sales"), EventKind.SlaBroken, 0).LineId);
        Assert.Equal("grant-awarded-chat-researcher", Say(Lab("researcher"), EventKind.GrantDecided, 1).LineId);
        Assert.Equal("grant-awarded-chat", Say(Lab("sales"), EventKind.GrantDecided, 1).LineId);
        Assert.Equal("grant-declined-chat", Say(Lab("sales"), EventKind.GrantDecided, 0).LineId);
        Assert.Equal("grant-late-chat", Say(Lab("sales"), EventKind.GrantSettled, 0).LineId);
    }

    [Fact]
    public void A_dropped_pledge_and_an_incident_break_have_no_chat_line_unless_the_data_has_one()
    {
        Game g = Lab("engineer");
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.SlaPledged, g.Id, 0, 0, "").Pick = 0;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.SlaBroken, g.Id, 1, 0, "").Pick = 1;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.GrantSettled, g.Id, 1, 0, "").Pick = 1;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.GrantApplied, g.Id, 1, 5000, "").Pick = 1;
        Assert.True(NewsTicker.Headlines(g.C, g.State, 1)[0].IsFiller);
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3, engineers: 4, safety: 2, gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 600;
        g.Do(Command.SetSla(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PLEDGES A SERVICE LEVEL"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("sla-chat"));
        g.Me.ResearchPoints = 1_000_000;
        g.Do(Command.ApplyForGrant(g.Id, 1));
        g.Do(Command.PublishPaper(g.Id));
        g.Weeks(6);
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 20), h => !h.IsFiller && h.Text.Contains("WINS A RESEARCH GRANT"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 40), m => m.LineId.StartsWith("grant-awarded-chat"));
    }
}
