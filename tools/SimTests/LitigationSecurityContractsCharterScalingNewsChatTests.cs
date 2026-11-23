using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the litigation docket, security, government contracts, the charter and the scaling policy.</summary>
public class LitigationSecurityContractsCharterScalingNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.CaseFiled, EventKind.CaseJudged, EventKind.CaseClosed,
        EventKind.SecurityBuilt, EventKind.BreachOpened, EventKind.BreachAnswered, EventKind.BreachLeaked,
        EventKind.ContractOffered, EventKind.ContractAccepted, EventKind.ContractDeclined, EventKind.ContractInstalment, EventKind.ContractEnded,
        EventKind.CharterReviewStarted, EventKind.CharterReviewDropped, EventKind.CharterChanged,
        EventKind.TripwireHit, EventKind.TripwireCleared, EventKind.PolicyChanged,
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
        Assert.Equal(409, c.News.Headlines.Count);
        Assert.Equal(362, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " IS SUED OVER AN INCIDENT", Headline(g, EventKind.CaseFiled, 0));
        Assert.StartsWith(lab + "'S LAID-OFF STAFF FILE A CLAIM FOR 7000", Headline(g, EventKind.CaseFiled, 1));
        Assert.StartsWith(lab + " IS SUED FOR 7000", Headline(g, EventKind.CaseFiled, 2));
        Assert.StartsWith(lab + " IS SUED FOR 7000", Headline(g, EventKind.CaseFiled, 3));
        Assert.StartsWith(lab + " LOSES A HEARING AND OWES 7000", Headline(g, EventKind.CaseJudged, 0));
        Assert.StartsWith(lab + " LOSES ITS APPEAL", Headline(g, EventKind.CaseJudged, 1));
        Assert.StartsWith(lab + " SETTLES A CLAIM FOR 7000", Headline(g, EventKind.CaseClosed, 0));
        Assert.StartsWith(lab + " WINS IN COURT", Headline(g, EventKind.CaseClosed, 1));
        Assert.StartsWith(lab + "'S LONG CASE ENDS", Headline(g, EventKind.CaseClosed, 2));
        Assert.StartsWith(lab + " REACHES SECURITY TIER 3", Headline(g, EventKind.SecurityBuilt, -1, 3, ""));
        Assert.StartsWith(lab + " SAYS IT IS 'LOOKING INTO'", Headline(g, EventKind.BreachOpened, -1));
        Assert.StartsWith(lab + " PAYS 7000 TO WHOEVER", Headline(g, EventKind.BreachAnswered, 0));
        Assert.StartsWith(lab + " TELLS EVERYONE ABOUT A BREACH", Headline(g, EventKind.BreachAnswered, 1));
        Assert.StartsWith(lab + " LOCKS ITS SYSTEMS DOWN FOR 7000", Headline(g, EventKind.BreachAnswered, 2));
        Assert.StartsWith("A BREACH AT " + lab + " LEAKS", Headline(g, EventKind.BreachLeaked, -1));
        Assert.StartsWith(lab + " IS OFFERED A GOVERNMENT CONTRACT WORTH 7000", Headline(g, EventKind.ContractOffered, 24));
        Assert.StartsWith(lab + " SIGNS A GOVERNMENT CONTRACT WORTH 7000", Headline(g, EventKind.ContractAccepted, 24));
        Assert.StartsWith(lab + " TURNS DOWN A GOVERNMENT CONTRACT", Headline(g, EventKind.ContractDeclined, 1));
        Assert.StartsWith(lab + " LETS A GOVERNMENT OFFER LAPSE", Headline(g, EventKind.ContractDeclined, 0));
        Assert.StartsWith(lab + " IS PAID 7000 ON ITS GOVERNMENT CONTRACT", Headline(g, EventKind.ContractInstalment, 1));
        Assert.StartsWith(lab + " MISSES A MILESTONE", Headline(g, EventKind.ContractInstalment, 0));
        Assert.StartsWith(lab + " COMPLETES ITS GOVERNMENT CONTRACT", Headline(g, EventKind.ContractEnded, 1));
        Assert.Contains("MISSED MILESTONES. 7000 IS OWED BACK", Headline(g, EventKind.ContractEnded, 0));
        Assert.Contains("ENDS EARLY", Headline(g, EventKind.ContractEnded, -1));
        Assert.Contains("ENDS EARLY", Headline(g, EventKind.ContractEnded, 2));
        Assert.StartsWith(lab + " ASKS REGULATORS TO APPROVE A NEW LEGAL STRUCTURE. THE FEE IS 7000", Headline(g, EventKind.CharterReviewStarted, 1));
        Assert.StartsWith(lab + " DROPS ITS RESTRUCTURING PLAN", Headline(g, EventKind.CharterReviewDropped, 1));
        Assert.StartsWith(lab + " CONVERTS TO A FOR-PROFIT", Headline(g, EventKind.CharterChanged, 0));
        Assert.StartsWith(lab + " BECOMES A CAPPED-PROFIT COMPANY", Headline(g, EventKind.CharterChanged, 1));
        Assert.StartsWith(lab + " BECOMES A NONPROFIT", Headline(g, EventKind.CharterChanged, 2));
        Assert.StartsWith(lab + "'S MINI TRIPS THE LAB'S OWN SCALING POLICY", Headline(g, EventKind.TripwireHit, -1, 2));
        Assert.StartsWith(lab + " MEETS ITS SAFEGUARDS", Headline(g, EventKind.TripwireCleared, 1));
        Assert.StartsWith(lab + "'S SCALING POLICY NO LONGER HOLDS", Headline(g, EventKind.TripwireCleared, 0));
        Assert.StartsWith(lab + " CHANGES ITS SCALING POLICY TO LEVEL 2", Headline(g, EventKind.PolicyChanged, 0, 2));
    }

    [Fact]
    public void Chat_lines_follow_the_pick()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("case-filed-chat", Say(g, EventKind.CaseFiled, 0));
        Assert.Equal("case-judged-chat", Say(g, EventKind.CaseJudged, 1));
        Assert.Equal("case-closed-won-chat", Say(g, EventKind.CaseClosed, 1));
        Assert.Equal("case-closed-settled-chat", Say(g, EventKind.CaseClosed, 0));
        Assert.Equal("case-closed-chat", Say(g, EventKind.CaseClosed, 3));
        Assert.Equal("security-built-chat", Say(g, EventKind.SecurityBuilt, -1));
        Assert.Equal("breach-opened-chat", Say(g, EventKind.BreachOpened, -1));
        Assert.Equal("breach-paid-chat", Say(g, EventKind.BreachAnswered, 0));
        Assert.Equal("breach-disclosed-chat", Say(g, EventKind.BreachAnswered, 1));
        Assert.Equal("breach-locked-chat", Say(g, EventKind.BreachAnswered, 2));
        Assert.Equal("breach-leaked-chat", Say(g, EventKind.BreachLeaked, -1));
        Assert.Equal("contract-offered-chat", Say(g, EventKind.ContractOffered, 24));
        Assert.Equal("contract-accepted-chat", Say(g, EventKind.ContractAccepted, 24));
        Assert.Equal("contract-declined-chat", Say(g, EventKind.ContractDeclined, 1));
        Assert.Equal("contract-paid-chat", Say(g, EventKind.ContractInstalment, 1));
        Assert.Equal("contract-missed-chat", Say(g, EventKind.ContractInstalment, 0));
        Assert.Equal("contract-done-chat", Say(g, EventKind.ContractEnded, 1));
        Assert.Equal("contract-ended-chat", Say(g, EventKind.ContractEnded, 2));
        Assert.Equal("charter-review-chat", Say(g, EventKind.CharterReviewStarted, 1));
        Assert.Equal("charter-dropped-chat", Say(g, EventKind.CharterReviewDropped, 1));
        Assert.Equal("charter-changed-chat", Say(g, EventKind.CharterChanged, 2));
        Assert.Equal("tripwire-hit-chat", Say(g, EventKind.TripwireHit, -1));
        Assert.Equal("tripwire-cleared-chat", Say(g, EventKind.TripwireCleared, 1));
        Assert.Equal("policy-changed-chat", Say(g, EventKind.PolicyChanged, 0));
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Security.Enabled = 1;
            c.Support.Enabled = 0;
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.State.Events.Clear();
        g.Do(Command.BuildSecurity(g.Id));
        g.Do(Command.AdoptPolicy(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SCALING POLICY TO LEVEL 1"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "policy-changed-chat");
        g.Do(Command.ConvertCharter(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("NEW LEGAL STRUCTURE"));
        g.Do(Command.CancelCharterChange(g.Id));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "charter-dropped-chat");
        g.Weeks(g.C.Security.BuildWeeks);
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 30), h => !h.IsFiller && h.Text.Contains("SECURITY TIER 1"));
    }
}
