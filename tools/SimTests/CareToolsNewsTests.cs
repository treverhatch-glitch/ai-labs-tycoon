using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines for the events of crunch time, insurance, the offsite, red teams, interpretability and pay.</summary>
public class CareToolsNewsTests
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
    public void Crunch_ending_says_how_many_weeks_it_lasted()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + "'S TEAM ENDS CRUNCH TIME AFTER 9 WEEKS. THE SOFAS ARE 'STILL WARM'", Headline(g, EventKind.CrunchEnded, -1, 9, "tired"));
        Assert.Contains("AFTER 0 WEEKS", Headline(g, EventKind.CrunchEnded, -1, 0, "empty"));
    }

    [Fact]
    public void An_insurance_claim_and_a_dropped_policy_have_their_own_lines()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + "'S INSURER PAYS OUT $750K ON ORION 2. THE PREMIUM 'WILL BE REVIEWED'", Headline(g, EventKind.InsuranceClaimPaid, 1, 750_000, "Orion 2"));
        Assert.Equal(lab + "'S INSURER DROPS IT AFTER 3 CLAIMS. 'IT IS NOT YOU, IT IS THE INCIDENTS'", Headline(g, EventKind.InsuranceDropped, -1, 3, ""));
    }

    [Fact]
    public void An_offsite_a_red_team_and_a_finding_name_the_lab_and_the_price_or_model()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + " TAKES THE TEAM ON AN OFFSITE FOR $14K. NOBODY WILL SAY WHAT HAPPENED AT THE ESCAPE ROOM", Headline(g, EventKind.OffsiteHeld, 6, 14_000, ""));
        Assert.Equal(lab + " PAYS $15K FOR A RED TEAM TO ATTACK ORION 2. THE RED TEAM 'HAD A LOVELY TIME'", Headline(g, EventKind.RedTeamRan, 1, 15_000, "Orion 2"));
        Assert.Equal(lab + "'S INTERPRETABILITY TEAM FINISHES READING ORION 2. IT IS 'MOSTLY ABOUT LUNCH'", Headline(g, EventKind.GapFound, 1, 12, "Orion 2"));
    }

    [Fact]
    public void A_pay_change_gives_the_new_weekly_bill()
    {
        Game g = World();
        Assert.Equal(g.Me.Name.ToUpperInvariant() + " CHANGES ITS PAY POLICY. THE WEEKLY SALARY BILL IS NOW $23K", Headline(g, EventKind.PayChanged, 1, 23_000, ""));
    }

    [Fact]
    public void A_real_game_that_uses_the_tools_puts_them_on_the_ticker()
    {
        Game g = Game.New(2, c => c.Office.ChancePermille = 0);
        g.Autopilot(60);
        g.Do(Command.SetPayLevel(g.Id, 1));
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Do(Command.HoldOffsite(g.Id));
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 8);
        string all = string.Join("|", ticker.Select(h => h.Text));
        Assert.Contains("CHANGES ITS PAY POLICY", all);
        Assert.Contains("ON AN OFFSITE FOR", all);
        Assert.DoesNotContain("INSURER", all);
    }

    [Fact]
    public void The_shipped_news_validates_with_its_new_lines()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        foreach (string id in new[] { "crunch-ended", "insurance-claim", "insurance-dropped", "offsite-held", "red-team-ran", "gap-found", "pay-changed" })
            Assert.NotNull(c.News.Headlines.FirstOrDefault(h => h.IdValue == id));
    }
}
