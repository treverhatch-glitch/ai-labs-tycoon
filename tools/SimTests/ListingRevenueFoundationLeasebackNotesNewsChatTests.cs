using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the listing, revenue financing, foundation, sale-leaseback and convertible notes.</summary>
public class ListingRevenueFoundationLeasebackNotesNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.CompanyListed, EventKind.EarningsReported, EventKind.FollowOnSold, EventKind.SharesBoughtBack, EventKind.ActivistArrived, EventKind.CompanyDelisted,
        EventKind.RevenueAdvanceTaken, EventKind.RevenueAdvanceCleared, EventKind.RevenueAdvanceStretched,
        EventKind.FoundationEndowed, EventKind.FoundationReported, EventKind.FoundationRefocused, EventKind.FoundationWoundUp, EventKind.FoundationRanDry,
        EventKind.GpusLeasedBack, EventKind.LeaseBoughtBack, EventKind.LeaseEnded, EventKind.LeaseRentMissed, EventKind.LeaseExtended,
        EventKind.NoteIssued, EventKind.NoteConverted, EventKind.NoteRepaid, EventKind.NoteExtended,
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
        Assert.Equal(443, c.News.Headlines.Count);
        Assert.Equal(396, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " LISTS ON THE MARKET AND RAISES 7000", Headline(g, EventKind.CompanyListed, -1));
        Assert.StartsWith(lab + " MISSES ITS OWN GUIDANCE", Headline(g, EventKind.EarningsReported, 0));
        Assert.StartsWith(lab + " MEETS ITS GUIDANCE EXACTLY", Headline(g, EventKind.EarningsReported, 1));
        Assert.StartsWith(lab + " BEATS ITS GUIDANCE", Headline(g, EventKind.EarningsReported, 2));
        Assert.StartsWith(lab + " SELLS MORE SHARES AND RAISES 7000", Headline(g, EventKind.FollowOnSold, -1));
        Assert.StartsWith(lab + " SPENDS 7000 BUYING ITS OWN SHARES", Headline(g, EventKind.SharesBoughtBack, -1));
        Assert.StartsWith("AN ACTIVIST INVESTOR ARRIVES AT " + lab, Headline(g, EventKind.ActivistArrived, -1));
        Assert.StartsWith(lab + " GOES PRIVATE AGAIN FOR 7000", Headline(g, EventKind.CompanyDelisted, -1));
        Assert.StartsWith(lab + " TAKES A REVENUE ADVANCE OF 7000", Headline(g, EventKind.RevenueAdvanceTaken, -1));
        Assert.StartsWith(lab + " FINISHES PAYING OFF ITS REVENUE ADVANCE", Headline(g, EventKind.RevenueAdvanceCleared, 0));
        Assert.StartsWith(lab + " BUYS OUT ITS REVENUE ADVANCE FOR 7000", Headline(g, EventKind.RevenueAdvanceCleared, 1));
        Assert.StartsWith("A WEAK WEEK STRETCHES " + lab + "'S REVENUE ADVANCE BY 7000", Headline(g, EventKind.RevenueAdvanceStretched, -1));
        Assert.StartsWith(lab + " PUTS 7000 INTO A NEW FOUNDATION", Headline(g, EventKind.FoundationEndowed, -1));
        Assert.StartsWith(lab + "'S FOUNDATION REPORTS 7000 GIVEN AWAY", Headline(g, EventKind.FoundationReported, -1));
        Assert.StartsWith(lab + "'S FOUNDATION TURNS TO A NEW CAUSE", Headline(g, EventKind.FoundationRefocused, -1));
        Assert.StartsWith(lab + " WINDS UP ITS FOUNDATION AND TAKES BACK 7000", Headline(g, EventKind.FoundationWoundUp, -1));
        Assert.StartsWith(lab + "'S FOUNDATION RUNS DRY AFTER GIVING 7000", Headline(g, EventKind.FoundationRanDry, -1));
        Assert.StartsWith(lab + " SELLS ITS GPUS FOR 7000 AND LEASES THEM STRAIGHT BACK", Headline(g, EventKind.GpusLeasedBack, -1));
        Assert.StartsWith(lab + " BUYS ITS LEASED GPUS BACK FOR 7000", Headline(g, EventKind.LeaseBoughtBack, -1));
        Assert.StartsWith(lab + "'S GPU LEASE RUNS OUT", Headline(g, EventKind.LeaseEnded, 0));
        Assert.StartsWith(lab + " HANDS ITS LEASED GPUS BACK EARLY", Headline(g, EventKind.LeaseEnded, 1));
        Assert.StartsWith("THE LESSOR REPOSSESSES THE GPUS IT LEASED TO " + lab, Headline(g, EventKind.LeaseEnded, 2));
        Assert.StartsWith(lab + " MISSES THE RENT ON ITS LEASED GPUS", Headline(g, EventKind.LeaseRentMissed, -1));
        Assert.StartsWith(lab + " EXTENDS ITS GPU LEASE", Headline(g, EventKind.LeaseExtended, -1));
        Assert.StartsWith(lab + " ISSUES A CONVERTIBLE NOTE FOR 7000", Headline(g, EventKind.NoteIssued, -1));
        Assert.Contains("TURNS INTO SHARES IN THE NEW ROUND", Headline(g, EventKind.NoteConverted, 0));
        Assert.Contains("TURNS INTO SHARES AT THE LISTING", Headline(g, EventKind.NoteConverted, 1));
        Assert.Contains("MATURES AND TURNS INTO SHARES ON HARSH TERMS", Headline(g, EventKind.NoteConverted, 2));
        Assert.StartsWith(lab + " REPAYS ITS CONVERTIBLE NOTE WITH 7000", Headline(g, EventKind.NoteRepaid, -1));
        Assert.StartsWith(lab + " GETS ITS CONVERTIBLE NOTE EXTENDED", Headline(g, EventKind.NoteExtended, -1));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("listing-listed-chat", Say(g, EventKind.CompanyListed, -1));
        Assert.Equal("earnings-chat", Say(g, EventKind.EarningsReported, 2));
        Assert.Equal("listing-follow-on-chat", Say(g, EventKind.FollowOnSold, -1));
        Assert.Equal("listing-buyback-chat", Say(g, EventKind.SharesBoughtBack, -1));
        Assert.Equal("listing-activist-chat", Say(g, EventKind.ActivistArrived, -1));
        Assert.Equal("listing-delisted-chat", Say(g, EventKind.CompanyDelisted, -1));
        Assert.Equal("rbf-taken-chat", Say(g, EventKind.RevenueAdvanceTaken, -1));
        Assert.Equal("rbf-cleared-chat", Say(g, EventKind.RevenueAdvanceCleared, 1));
        Assert.Equal("rbf-stretched-chat", Say(g, EventKind.RevenueAdvanceStretched, -1));
        Assert.Equal("foundation-endowed-chat", Say(g, EventKind.FoundationEndowed, -1));
        Assert.Equal("foundation-reported-chat", Say(g, EventKind.FoundationReported, -1));
        Assert.Equal("foundation-refocused-chat", Say(g, EventKind.FoundationRefocused, -1));
        Assert.Equal("foundation-wound-up-chat", Say(g, EventKind.FoundationWoundUp, -1));
        Assert.Equal("foundation-dry-chat", Say(g, EventKind.FoundationRanDry, -1));
        Assert.Equal("lease-sold-chat", Say(g, EventKind.GpusLeasedBack, -1));
        Assert.Equal("lease-bought-back-chat", Say(g, EventKind.LeaseBoughtBack, -1));
        Assert.Equal("lease-ended-chat", Say(g, EventKind.LeaseEnded, 2));
        Assert.Equal("lease-missed-chat", Say(g, EventKind.LeaseRentMissed, -1));
        Assert.Equal("lease-extended-chat", Say(g, EventKind.LeaseExtended, -1));
        Assert.Equal("note-issued-chat", Say(g, EventKind.NoteIssued, -1));
        Assert.Equal("note-converted-chat", Say(g, EventKind.NoteConverted, 1));
        Assert.Equal("note-repaid-chat", Say(g, EventKind.NoteRepaid, -1));
        Assert.Equal("note-extended-chat", Say(g, EventKind.NoteExtended, -1));
    }
}
