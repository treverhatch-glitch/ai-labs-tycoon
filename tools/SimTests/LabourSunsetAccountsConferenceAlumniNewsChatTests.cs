using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for labour relations, model end-of-life, account teams, conference papers and the alumni network.</summary>
public class LabourSunsetAccountsConferenceAlumniNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 3, string text = "", int subject = -1)
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
        Assert.Equal(405, c.News.Headlines.Count);
        Assert.Equal(358, c.Chat.Lines.Count);
        foreach (string kind in new[] { "LabourDrive", "LabourRecognised", "LabourResisted", "LabourHeld", "LabourStrike", "SunsetAnnounced", "SunsetDone", "SunsetCalledOff", "AccountTeamChanged", "PaperSubmitted", "PaperDecided", "AlumniRehired" })
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
        Assert.StartsWith("STAFF AT " + lab + " ARE ORGANISING", Headline(g, EventKind.LabourDrive, -1, 300));
        Assert.StartsWith(lab + " RECOGNISES ITS STAFF UNION", Headline(g, EventKind.LabourRecognised, 1, 80));
        Assert.StartsWith(lab + " IS FORCED TO RECOGNISE ITS STAFF UNION", Headline(g, EventKind.LabourRecognised, 0, 120));
        Assert.StartsWith(lab + " HIRES LAWYERS", Headline(g, EventKind.LabourResisted, 1, 500_000));
        Assert.StartsWith(lab + " LETS ITS STAFF DRIVE GO UNANSWERED", Headline(g, EventKind.LabourResisted, 0, 0));
        Assert.StartsWith("THE STAFF DRIVE AT " + lab + " FIZZLES", Headline(g, EventKind.LabourHeld, -1, 200));
        Assert.Contains("WALK OUT FOR 3 WEEKS", Headline(g, EventKind.LabourStrike, -1, 3));
        Assert.StartsWith(lab + " ANNOUNCES THAT MINI WILL BE RETIRED IN 8 WEEKS", Headline(g, EventKind.SunsetAnnounced, -1, 8, "Mini"));
        Assert.StartsWith(lab + " RETIRES MINI", Headline(g, EventKind.SunsetDone, -1, 2, "Mini"));
        Assert.StartsWith(lab + " CALLS OFF THE RETIREMENT OF MINI", Headline(g, EventKind.SunsetCalledOff, 1, 20, "Mini"));
        Assert.StartsWith(lab + " WITHDRAWS THE RETIREMENT OF MINI", Headline(g, EventKind.SunsetCalledOff, 0, 40, "Mini"));
        Assert.StartsWith(lab + " PUTS 2 SALESPEOPLE ON ITS BOTS ACCOUNT TEAM", Headline(g, EventKind.AccountTeamChanged, 1, 2, "Bots"));
        Assert.StartsWith(lab + " RESHUFFLES ITS BOTS ACCOUNT TEAM", Headline(g, EventKind.AccountTeamChanged, 2, 2, "Bots"));
        Assert.StartsWith(lab + " SENDS A PAPER TO A WORKSHOP", Headline(g, EventKind.PaperSubmitted, -1, 100, "workshop"));
        Assert.StartsWith("A FLAGSHIP ACCEPTS A PAPER FROM " + lab, Headline(g, EventKind.PaperDecided, 1, 450, "flagship"));
        Assert.StartsWith("A FLAGSHIP REJECTS A PAPER FROM " + lab, Headline(g, EventKind.PaperDecided, 0, 450, "flagship"));
        Assert.StartsWith(lab + " WITHDRAWS A PAPER FROM A FLAGSHIP", Headline(g, EventKind.PaperDecided, 2, 450, "flagship"));
        Assert.Contains("LEFT ON GOOD TERMS", Headline(g, EventKind.AlumniRehired, 1, 1000, "researcher"));
        Assert.Contains("LAID OFF", Headline(g, EventKind.AlumniRehired, 0, 1000, "researcher"));
        Assert.Contains("QUIT", Headline(g, EventKind.AlumniRehired, -1, 1000, "researcher"));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("labour-drive-chat", Say(Lab("sales"), EventKind.LabourDrive, -1).LineId);
        Assert.Equal("labour-recognised-chat", Say(Lab("sales"), EventKind.LabourRecognised, 1).LineId);
        Assert.Equal("labour-forced-chat", Say(Lab("sales"), EventKind.LabourRecognised, 0).LineId);
        Assert.Equal("labour-resisted-chat", Say(Lab("sales"), EventKind.LabourResisted, 1).LineId);
        Assert.Equal("labour-held-chat", Say(Lab("sales"), EventKind.LabourHeld, -1).LineId);
        Assert.Equal("labour-strike-chat", Say(Lab("researcher"), EventKind.LabourStrike, -1).LineId);
        Assert.Equal("labour-strike-chat-any", Say(Lab("sales"), EventKind.LabourStrike, -1).LineId);
        Assert.Equal("sunset-announced-chat", Say(Lab("engineer"), EventKind.SunsetAnnounced, -1).LineId);
        Assert.Equal("sunset-announced-chat-any", Say(Lab("sales"), EventKind.SunsetAnnounced, -1).LineId);
        Assert.Equal("sunset-done-chat", Say(Lab("sales"), EventKind.SunsetDone, -1).LineId);
        Assert.Equal("sunset-off-chat", Say(Lab("sales"), EventKind.SunsetCalledOff, 1).LineId);
        Assert.Equal("account-team-chat", Say(Lab("sales"), EventKind.AccountTeamChanged, 1).LineId);
        Assert.Equal("account-team-chat-any", Say(Lab("safety"), EventKind.AccountTeamChanged, 1).LineId);
        Assert.Equal("paper-submitted-chat", Say(Lab("researcher"), EventKind.PaperSubmitted, -1).LineId);
        Assert.Equal("paper-accepted-chat", Say(Lab("sales"), EventKind.PaperDecided, 1).LineId);
        Assert.Equal("paper-rejected-chat", Say(Lab("sales"), EventKind.PaperDecided, 0).LineId);
        Assert.Equal("paper-withdrawn-chat", Say(Lab("sales"), EventKind.PaperDecided, 2).LineId);
        Assert.Equal("alumni-rehired-chat", Say(Lab("sales"), EventKind.AlumniRehired, 1).LineId);
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
        ModelState old = g.Train("mini", 300);
        g.UntilTrained(old);
        g.Release(old, "game-ai", "game-bots");
        ModelState next = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, next.Id, old.Id));
        g.UntilTrained(next);
        g.Release(next, "game-ai");
        g.Weeks(2);
        g.Do(Command.AssignAccountTeam(g.Id, "game-bots", 2));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("ACCOUNT TEAM"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("account-team-chat"));
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 6));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("WILL BE RETIRED"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("sunset-announced-chat"));
        g.Do(Command.CancelSunset(g.Id, old.Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("CALLS OFF THE RETIREMENT"));
        g.Weeks(40);
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SENDS A PAPER"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("paper-submitted-chat"));
        g.Weeks(20);
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 20), h => !h.IsFiller && h.Text.Contains("A PAPER FROM"));
    }
}
