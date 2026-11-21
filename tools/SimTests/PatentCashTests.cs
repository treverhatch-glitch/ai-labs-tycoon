using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q114: active patents grant weekly cash (Grants ledger).</summary>
public class PatentCashTests
{
    static Game WithPatent(string tech = "minimax")
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 2_000_000;
        g.GrantResearch(5_000);
        g.Research("state-machines");
        if (tech != "state-machines") g.Research(tech);
        g.Do(Command.FilePatent(g.Id, tech));
        return g;
    }

    [Fact]
    public void Shipped_patent_cash_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(40, c.Research.Patents.CashPerActivePatent);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_patents_no_cash_gain()
    {
        Game g = Game.Solo().WithStarterLab();
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void One_active_patent_grants_cash()
    {
        Game g = WithPatent();
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.CashPerActivePatent, g.Me.Cash);
    }

    [Fact]
    public void Two_patents_double_weekly_cash()
    {
        Game g = WithPatent("state-machines");
        g.Research("minimax");
        g.Do(Command.FilePatent(g.Id, "minimax"));
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Research.Patents.CashPerActivePatent, g.Me.Cash);
    }

    [Fact]
    public void Zero_cash_config_disables_hook()
    {
        Game g = WithPatent();
        g.C.Research.Patents.CashPerActivePatent = 0;
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Cash_posts_to_grants_ledger_line()
    {
        Game g = WithPatent();
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Research.Patents.CashPerActivePatent, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Cash_stacks_with_research_and_reputation()
    {
        Game g = WithPatent();
        long cash = g.Me.Cash;
        long rp = g.Me.ResearchPoints;
        int rep = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Cash > cash);
        Assert.True(g.Me.ResearchPoints > rp);
        Assert.True(g.Me.Reputation > rep);
    }

    [Fact]
    public void Expired_patent_stops_cash_stipend()
    {
        Game g = WithPatent();
        g.Me.Patents[0].ExpiresTick = g.Tick;
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Sim_week_runs_patent_cash()
    {
        Game g = WithPatent();
        g.Weeks(1);
        Assert.True(g.Me.Ledger.LastWeek.Grants >= g.C.Research.Patents.CashPerActivePatent);
        Game bare = Game.Solo().WithStarterLab();
        bare.Me.Cash = g.Me.Cash;
        bare.Weeks(1);
        Assert.Equal(g.Me.Ledger.LastWeek.Grants - bare.Me.Ledger.LastWeek.Grants,
            g.C.Research.Patents.CashPerActivePatent);
    }

    [Fact]
    public void Save_load_preserves_patent_cash_path()
    {
        Game g = WithPatent();
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long before = loaded.Me.Cash;
        PatentSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(before + loaded.C.Research.Patents.CashPerActivePatent, loaded.Me.Cash);
    }

    [Fact]
    public void Bankrupt_lab_still_accrues_patent_cash()
    {
        Game g = WithPatent();
        g.Me.Bankrupt = true;
        long before = g.Me.Cash;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.CashPerActivePatent, g.Me.Cash);
    }

    [Fact]
    public void Plan_lab_patents_no_player_cash_when_hooks_target_player_only()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        long before = plan.Cash;
        PatentSystem.RunWeek(g.C, g.State, plan);
        Assert.Equal(before, plan.Cash);
    }

    [Fact]
    public void Negative_cash_config_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Research.Patents.CashPerActivePatent = -1));
    }
}