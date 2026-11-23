using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for audits, the research focus, hack week, staff equity and retraining.</summary>
public class ResearchAndStaffToolsNewsChatTests
{
    static readonly string[] Kinds = { "AuditDone", "AuditRevoked", "FocusChanged", "HackWeek", "EquityGranted", "StaffRetrained" };

    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, int subject, long amount, string reference)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, subject, amount, reference);
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
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void Ten_headlines_and_nine_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(407, c.News.Headlines.Count);
        Assert.Equal(360, c.Chat.Lines.Count);
        foreach (string kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind);
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void An_audit_has_a_headline_for_each_outcome_that_names_the_lab_and_the_model()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + " GETS AN OUTSIDE SAFETY CERTIFICATE FOR BIG-ONE. THE AUDITORS 'COULD NOT FIND THE REST OF IT'", Headline(g, EventKind.AuditDone, 1, 5, 25_000, "Big-One"));
        Assert.Equal("OUTSIDE AUDITORS PUBLISH THE REAL SAFETY SCORE OF BIG-ONE. IT IS NOT THE ONE ON " + lab + "'S WEBSITE", Headline(g, EventKind.AuditDone, 0, 5, 25_000, "Big-One"));
        Assert.Equal("AUDITORS PUBLISH THE REAL SCORE OF BIG-ONE. " + lab + " CALLS IT 'A DATA POINT'", Headline(g, EventKind.AuditDone, -1, 5, 25_000, "Big-One"));
        Assert.Equal(lab + " LOSES THE SAFETY CERTIFICATE OF BIG-ONE AFTER AN INCIDENT. THE STICKER IS BEING 'REPURPOSED'", Headline(g, EventKind.AuditRevoked, -1, 5, 0, "Big-One"));
    }

    [Fact]
    public void The_focus_the_hack_week_and_the_two_staff_tools_each_have_a_headline()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + " ANNOUNCES A NEW RESEARCH FOCUS. THE OLD ONE IS 'STILL IN THE BUILDING'", Headline(g, EventKind.FocusChanged, 1, -1, 50_000, "frontier"));
        Assert.Equal(lab + " DROPS ITS RESEARCH FOCUS AND WILL 'FOLLOW THE SCIENCE, WHEREVER IT IS'", Headline(g, EventKind.FocusChanged, 0, -1, 50_000, ""));
        Assert.Contains("HACK WEEK. SOMEONE HAS BUILT A TOASTER", Headline(g, EventKind.HackWeek, 1, 2, 5_000, ""));
        Assert.Contains("HACK WEEK ENDS. THE TOASTER", Headline(g, EventKind.HackWeek, 0, 2, 0, ""));
        Assert.Contains("GIVES ITS STAFF EQUITY", Headline(g, EventKind.EquityGranted, -1, 1, 40, ""));
        Assert.Contains("RETRAINS A PERSON FOR A NEW ROLE", Headline(g, EventKind.StaffRetrained, 1, 7, 900, "sales>policy"));
    }

    [Fact]
    public void An_audit_chat_line_follows_the_outcome()
    {
        Assert.Equal("audit-done-chat-clean", Say(Lab("sales"), EventKind.AuditDone, 1).LineId);
        Assert.Equal("audit-done-chat-shamed", Say(Lab("sales"), EventKind.AuditDone, 0).LineId);
        Assert.Equal("audit-revoked-chat", Say(Lab("sales"), EventKind.AuditRevoked, -1).LineId);
    }

    [Fact]
    public void A_researcher_has_their_own_hack_week_line_and_everyone_else_a_general_one()
    {
        Assert.Equal("hack-week-chat-research", Say(Lab("researcher"), EventKind.HackWeek, 1).LineId);
        Assert.Equal("hack-week-chat", Say(Lab("sales"), EventKind.HackWeek, 1).LineId);
        Assert.Equal("hack-week-over-chat", Say(Lab("researcher"), EventKind.HackWeek, 0).LineId);
    }

    [Fact]
    public void The_focus_equity_and_retraining_have_their_own_lines()
    {
        Assert.Equal("focus-changed-chat", Say(Lab("engineer"), EventKind.FocusChanged, 1).LineId);
        Assert.Equal("equity-granted-chat", Say(Lab("engineer"), EventKind.EquityGranted, -1).LineId);
        Assert.Equal("staff-retrained-chat", Say(Lab("engineer"), EventKind.StaffRetrained, 0).LineId);
    }

    [Fact]
    public void The_real_events_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 4, engineers: 2, safety: 2, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 600;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.DisplayedAlignment = m.Alignment;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        g.Do(Command.HoldHackWeek(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.RetrainStaff(g.Id, g.Me.Staff.First(p => p.RoleId == "sales").Id, "engineer"));
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 12);
        foreach (string text in new[] { "SAFETY CERTIFICATE", "NEW RESEARCH FOCUS", "HACK WEEK", "GIVES ITS STAFF EQUITY", "RETRAINS A PERSON" })
            Assert.Contains(ticker, h => !h.IsFiller && h.Text.Contains(text));
        List<ChatMessage> chat = CompanyChat.Feed(g.C, g.State, g.Me, 20);
        foreach (string id in new[] { "audit-done-chat-clean", "focus-changed-chat", "equity-granted-chat", "staff-retrained-chat" })
            Assert.Contains(chat, x => x.LineId == id);
        Assert.Contains(chat, x => x.LineId.StartsWith("hack-week-chat"));
    }
}
