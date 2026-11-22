using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for internships, the support desk, sabbaticals, open requisitions and visiting scholars.</summary>
public class InternsSupportSabbaticalRequisitionsScholarsNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 3, string text = "engineer", int subject = -1)
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
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "engineer");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(298, c.News.Headlines.Count);
        Assert.Equal(300, c.Chat.Lines.Count);
        foreach (string kind in new[] { "InternshipStarted", "InternshipsEnded", "SupportDeskSet", "SupportBacklogChanged", "SabbaticalStarted", "SabbaticalEnded", "RequisitionOpened", "RequisitionClosed", "ScholarsArrived", "ScholarsLeft" })
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
        Assert.StartsWith(lab + " TAKES ON 3 INTERNS", Headline(g, EventKind.InternshipStarted, 26));
        Assert.StartsWith(lab + "'S INTERNS FINISH", Headline(g, EventKind.InternshipsEnded, 1, 2));
        Assert.StartsWith(lab + " SENDS ITS INTERNS HOME EARLY", Headline(g, EventKind.InternshipsEnded, 0, 0));
        Assert.StartsWith(lab + " PUTS 2 ENGINEERS ON A SUPPORT DESK", Headline(g, EventKind.SupportDeskSet, -1, 2, ""));
        Assert.StartsWith(lab + "'S SUPPORT BACKLOG HITS 30 TICKETS", Headline(g, EventKind.SupportBacklogChanged, 1, 30, ""));
        Assert.StartsWith(lab + " CLEARS ITS SUPPORT BACKLOG", Headline(g, EventKind.SupportBacklogChanged, 0, 4, ""));
        Assert.StartsWith(lab + " SENDS ONE OF ITS ENGINEERS ON SABBATICAL", Headline(g, EventKind.SabbaticalStarted, -1, 8));
        Assert.StartsWith("ONE OF " + lab + "'S ENGINEERS RETURNS FROM SABBATICAL, RESTED", Headline(g, EventKind.SabbaticalEnded, 1, 600));
        Assert.Contains("RESTLESS", Headline(g, EventKind.SabbaticalEnded, 0, 400));
        Assert.Contains("BACK FROM SABBATICAL", Headline(g, EventKind.SabbaticalEnded, 2, 850));
        Assert.StartsWith(lab + " ORDERS 3 ENGINEERS FOR LATER", Headline(g, EventKind.RequisitionOpened, 8));
        Assert.StartsWith(lab + "'S ORDER OF ENGINEERS ARRIVES ON TIME", Headline(g, EventKind.RequisitionClosed, 1, 3));
        Assert.Contains("ARRIVES SHORT", Headline(g, EventKind.RequisitionClosed, 0, 1));
        Assert.Contains("CANCELS AN ORDER", Headline(g, EventKind.RequisitionClosed, -1, 0));
        Assert.StartsWith(lab + " HOSTS 3 VISITING SCHOLARS", Headline(g, EventKind.ScholarsArrived, 12, 3, ""));
        Assert.Contains("LEAVE ON SCHEDULE", Headline(g, EventKind.ScholarsLeft, 1, 40, ""));
        Assert.Contains("HOME EARLY", Headline(g, EventKind.ScholarsLeft, 0, 0, ""));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("internship-started-chat", Say(Lab("sales"), EventKind.InternshipStarted, 26).LineId);
        Assert.Equal("internship-done-chat", Say(Lab("sales"), EventKind.InternshipsEnded, 1).LineId);
        Assert.Equal("internship-early-chat", Say(Lab("sales"), EventKind.InternshipsEnded, 0).LineId);
        Assert.Equal("support-desk-chat", Say(Lab("engineer"), EventKind.SupportDeskSet, -1).LineId);
        Assert.Equal("support-desk-chat-any", Say(Lab("sales"), EventKind.SupportDeskSet, -1).LineId);
        Assert.Equal("support-backlog-up-chat", Say(Lab("sales"), EventKind.SupportBacklogChanged, 1).LineId);
        Assert.Equal("support-backlog-clear-chat", Say(Lab("sales"), EventKind.SupportBacklogChanged, 0).LineId);
        Assert.Equal("sabbatical-started-chat", Say(Lab("sales"), EventKind.SabbaticalStarted, -1).LineId);
        Assert.Equal("sabbatical-rested-chat", Say(Lab("sales"), EventKind.SabbaticalEnded, 1).LineId);
        Assert.Equal("sabbatical-restless-chat", Say(Lab("sales"), EventKind.SabbaticalEnded, 0).LineId);
        Assert.Equal("requisition-opened-chat", Say(Lab("sales"), EventKind.RequisitionOpened, 8).LineId);
        Assert.Equal("requisition-arrived-chat", Say(Lab("sales"), EventKind.RequisitionClosed, 1).LineId);
        Assert.Equal("requisition-short-chat", Say(Lab("sales"), EventKind.RequisitionClosed, 0).LineId);
        Assert.Equal("scholars-arrived-chat", Say(Lab("researcher"), EventKind.ScholarsArrived, 12).LineId);
        Assert.Equal("scholars-arrived-chat-any", Say(Lab("sales"), EventKind.ScholarsArrived, 12).LineId);
        Assert.Equal("scholars-left-chat", Say(Lab("sales"), EventKind.ScholarsLeft, 1).LineId);
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo(2, c =>
        {
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
        }).WithStarterLab(researchers: 4, engineers: 3, safety: 2, sales: 3, gpus: 8);
        g.Do(Command.Fire(g.Id, "researcher", 3));
        g.State.Events.Clear();
        g.Do(Command.StartInternships(g.Id, "researcher", 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("TAKES ON 1 INTERNS"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "internship-started-chat");
        g.Do(Command.EndInternships(g.Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("INTERNS HOME EARLY"));
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SUPPORT DESK"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("support-desk-chat"));
        g.Do(Command.SendOnSabbatical(g.Id, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("ON SABBATICAL"));
        g.Do(Command.OpenRequisition(g.Id, "safety", 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("ORDERS 1"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "requisition-opened-chat");
        g.Do(Command.HostScholars(g.Id, 2));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("VISITING SCHOLARS"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("scholars-arrived-chat"));
        g.Weeks(g.C.Scholars.Weeks);
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 30), h => !h.IsFiller && h.Text.Contains("LEAVE ON SCHEDULE"));
    }
}
