using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines for the chip and loan events the ticker had no line for.</summary>
public class ChipLoanNewsTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int subject, long amount, string reference)
    {
        g.State.Events.Clear();
        Events.Add(g.State, kind, g.Id, subject, amount, reference);
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind.ToString());
        return ticker[0].Text;
    }

    [Fact]
    public void A_tapeout_a_respin_and_a_yield_crash_name_the_lab_and_the_design()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + " TAPS OUT PIXIE 1 FOR $12M. THE FOUNDRY'S CHEQUE CLEARED FASTER THAN THE DESIGN", Headline(g, EventKind.ChipTapedOut, 1, 12_000_000, "Pixie 1"));
        Assert.Equal(lab + " RESPINS PIXIE 1 FOR $3.5M. THE FIRST ONE IS NOW 'A LEARNING CHIP'", Headline(g, EventKind.ChipRespin, 1, 3_500_000, "Pixie 1"));
        Assert.Equal(lab + "'S PIXIE 1 YIELD CRASHES: 800 CHIPS ARRIVE. THE FOUNDRY BLAMES THE WEATHER", Headline(g, EventKind.ChipYieldCrash, 1, 800, "Pixie 1"));
    }

    [Fact]
    public void A_snub_says_how_many_weeks()
    {
        Game g = World();
        Assert.Contains("DOWN THE QUEUE BY 4 WEEKS", Headline(g, EventKind.ChipSlotSnubbed, 1, 4, "9"));
    }

    [Fact]
    public void A_chip_sale_names_the_buyer_the_seller_and_the_count()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string> { g0Rival() } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        LabState other = g.State.Labs.First(l => l.Id != g.Id);
        string text = Headline(g, EventKind.ChipsBought, other.Id, 500, "Pixie 1");
        Assert.Contains(" BUYS 500 PIXIE 1 CHIPS FROM " + other.Name.ToUpperInvariant() + ".", text);
    }

    static string g0Rival() { return TestData.Config().Rivals[0].Id; }

    [Fact]
    public void A_loan_is_taken_missed_and_defaulted_in_the_banks_words()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + " BORROWS $50M. THE BANK CALLS THE LAB 'CONFIDENT'", Headline(g, EventKind.LoanTaken, 1, 50_000_000, ""));
        Assert.Equal(lab + " MISSES A LOAN PAYMENT. A SPOKESPERSON SAYS THE CHEQUE IS IN THE POST", Headline(g, EventKind.LoanMissed, 1, 2_000_000, ""));
        Assert.Equal(lab + " DEFAULTS ON A LOAN, $48M OWED. THE BANK 'REMAINS OPTIMISTIC'", Headline(g, EventKind.LoanDefaulted, 3, 48_000_000, "1"));
    }

    [Fact]
    public void A_repaid_loan_still_makes_no_news()
    {
        Game g = World();
        Events.Add(g.State, EventKind.LoanRepaid, g.Id, 1, 1000, "term");
        Assert.All(NewsTicker.Headlines(g.C, g.State, 1), h => Assert.True(h.IsFiller));
    }

    [Fact]
    public void The_shipped_news_validates_with_its_new_lines()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        foreach (string id in new[] { "chip-taped-out", "chip-respin", "chip-yield-crash", "chip-snub", "chips-bought", "loan-taken", "loan-missed", "loan-defaulted" })
            Assert.NotNull(c.News.Headlines.FirstOrDefault(h => h.IdValue == id));
    }
}
