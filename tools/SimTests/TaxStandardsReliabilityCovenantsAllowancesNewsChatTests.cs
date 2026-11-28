using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the tax, industry standards, hardware reliability, loan covenants and capital allowances.</summary>
public class TaxStandardsReliabilityCovenantsAllowancesNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.TaxStructureBuilt, EventKind.TaxPaid, EventKind.TaxCreditClaimed, EventKind.TaxAuditOpened, EventKind.TaxAuditClosed,
        EventKind.StandardDrafted, EventKind.StandardRatified, EventKind.StandardAdopted, EventKind.StandardLapsed,
        EventKind.GpusFailed, EventKind.ReplacementsArrived, EventKind.WarrantyChanged, EventKind.SparesStocked,
        EventKind.CovenantBreached, EventKind.CovenantCured, EventKind.CovenantWaived, EventKind.LoansCalled,
        EventKind.CapitalPolicySet,
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
        Assert.Equal(446, c.News.Headlines.Count);
        Assert.Equal(399, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " OPENS A NEW CORPORATE STRUCTURE", Headline(g, EventKind.TaxStructureBuilt, -1, 2, ""));
        Assert.StartsWith(lab + " PAYS 7000 IN TAX 'AFTER CAREFUL STRUCTURING'", Headline(g, EventKind.TaxPaid, 1));
        Assert.StartsWith(lab + " PAYS 7000 IN TAX. THE ACCOUNTANTS", Headline(g, EventKind.TaxPaid, 0));
        Assert.StartsWith(lab + " CLAIMS A RESEARCH CREDIT OF 7000", Headline(g, EventKind.TaxCreditClaimed, -1));
        Assert.Contains("THE OFFSHORE STRUCTURE DRAWS QUESTIONS", Headline(g, EventKind.TaxAuditOpened, 1));
        Assert.Contains("A POLITE LETTER IS ATTACHED", Headline(g, EventKind.TaxAuditOpened, 0));
        Assert.StartsWith(lab + " PAYS BACK 7000 TO THE TAX OFFICE", Headline(g, EventKind.TaxAuditClosed, 0));
        Assert.StartsWith(lab + " SETTLES A TAX AUDIT FOR 7000", Headline(g, EventKind.TaxAuditClosed, 1));
        Assert.StartsWith(lab + "'S TAX AUDIT ENDS", Headline(g, EventKind.TaxAuditClosed, 2));
        Assert.StartsWith(lab + "'S TAX AUDIT ENDS", Headline(g, EventKind.TaxAuditClosed, 3));
        Assert.StartsWith("THE INDUSTRY STARTS DRAFTING A LEVEL 7000 STANDARD", Headline(g, EventKind.StandardDrafted, -1));
        Assert.StartsWith("A LEVEL 7000 INDUSTRY STANDARD IS RATIFIED", Headline(g, EventKind.StandardRatified, 2));
        Assert.StartsWith(lab + " ADOPTS THE INDUSTRY STANDARDS UP TO LEVEL 7000", Headline(g, EventKind.StandardAdopted, -1));
        Assert.StartsWith(lab + " RELEASES A MODEL WITHOUT MEETING THE STANDARDS AND IS FINED 7000", Headline(g, EventKind.StandardLapsed, 1));
        Assert.StartsWith(lab + " LOSES 7000 GPUS TO FAILURE", Headline(g, EventKind.GpusFailed, 0));
        Assert.StartsWith(lab + " SWAPS 7000 FAILED GPUS FOR SPARES", Headline(g, EventKind.GpusFailed, 1));
        Assert.StartsWith(lab + " SHIPS 7000 FAILED GPUS BACK UNDER ITS WARRANTY", Headline(g, EventKind.GpusFailed, 2));
        Assert.StartsWith(lab + " HAS 7000 REPLACEMENT GPUS RUSHED IN", Headline(g, EventKind.ReplacementsArrived, 1));
        Assert.StartsWith(lab + "'S 7000 REPLACEMENT GPUS ARRIVE", Headline(g, EventKind.ReplacementsArrived, 0));
        Assert.StartsWith(lab + " CHANGES ITS GPU WARRANTY TO TIER 7000", Headline(g, EventKind.WarrantyChanged, -1));
        Assert.StartsWith(lab + " BUYS 7000 SPARE GPUS", Headline(g, EventKind.SparesStocked, -1));
        Assert.StartsWith(lab + "'S CHIPS NO LONGER COVER ITS LOANS", Headline(g, EventKind.CovenantBreached, -1));
        Assert.StartsWith(lab + " PAYS ITS LOANS DOWN AND IS BACK WITHIN ITS COVENANT", Headline(g, EventKind.CovenantCured, 1));
        Assert.StartsWith(lab + " DRIFTS BACK WITHIN ITS LOAN COVENANT", Headline(g, EventKind.CovenantCured, 0));
        Assert.StartsWith(lab + " PAYS 7000 TO HAVE ITS LOAN COVENANT WAIVED", Headline(g, EventKind.CovenantWaived, -1));
        Assert.Contains("PAYS 7000 OUT OF CASH", Headline(g, EventKind.LoansCalled, 1));
        Assert.Contains("TAKES CHIPS FOR THE 7000 OWED", Headline(g, EventKind.LoansCalled, 0));
        Assert.StartsWith(lab + " GOES BACK TO WRITING OFF ITS HARDWARE AT ONCE", Headline(g, EventKind.CapitalPolicySet, 0, 0, ""));
        Assert.StartsWith(lab + " SPREADS ITS HARDWARE SPEND OVER THE YEARS", Headline(g, EventKind.CapitalPolicySet, 1, 1, ""));
        Assert.StartsWith(lab + " CLAIMS AN ENHANCED CAPITAL ALLOWANCE", Headline(g, EventKind.CapitalPolicySet, 2, 2, ""));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("tax-structure-chat", Say(g, EventKind.TaxStructureBuilt, -1));
        Assert.Equal("tax-paid-chat", Say(g, EventKind.TaxPaid, 1));
        Assert.Equal("tax-credit-chat", Say(g, EventKind.TaxCreditClaimed, -1));
        Assert.Equal("tax-audit-chat", Say(g, EventKind.TaxAuditOpened, 1));
        Assert.Equal("tax-audit-closed-chat", Say(g, EventKind.TaxAuditClosed, 2));
        Assert.Equal("standard-drafted-chat", Say(g, EventKind.StandardDrafted, -1));
        Assert.Equal("standard-ratified-chat", Say(g, EventKind.StandardRatified, 3));
        Assert.Equal("standard-adopted-chat", Say(g, EventKind.StandardAdopted, -1));
        Assert.Equal("standard-lapsed-chat", Say(g, EventKind.StandardLapsed, 2));
        Assert.Equal("gpus-failed-chat", Say(g, EventKind.GpusFailed, 2));
        Assert.Equal("gpus-returned-chat", Say(g, EventKind.ReplacementsArrived, 1));
        Assert.Equal("warranty-changed-chat", Say(g, EventKind.WarrantyChanged, -1));
        Assert.Equal("spares-stocked-chat", Say(g, EventKind.SparesStocked, -1));
        Assert.Equal("covenant-breached-chat", Say(g, EventKind.CovenantBreached, -1));
        Assert.Equal("covenant-cured-chat", Say(g, EventKind.CovenantCured, 1));
        Assert.Equal("covenant-waived-chat", Say(g, EventKind.CovenantWaived, -1));
        Assert.Equal("loans-called-chat", Say(g, EventKind.LoansCalled, 0));
        Assert.Equal("capital-policy-chat", Say(g, EventKind.CapitalPolicySet, 1));
    }
}
