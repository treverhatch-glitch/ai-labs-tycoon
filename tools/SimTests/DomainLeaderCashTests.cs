using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q115: leaderCashPerWeek stipend in CapabilityDomainSystem.</summary>
public class DomainLeaderCashTests
{
    static Game At(int year) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_leader_cash_stipend_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1500, c.Market.Domain("language").LeaderCashPerWeek);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_models_no_leader_cash()
    {
        Game g = At(2028);
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Domain_leader_gains_weekly_cash()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Cash);
    }

    [Fact]
    public void Zero_config_skips_leader_cash()
    {
        Game g = At(2028);
        g.C.Market.Domain("language").LeaderCashPerWeek = 0;
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Bankrupt_leader_skips_cash_grant()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        g.Me.Bankrupt = true;
        long cash = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void Second_week_stacks_leader_cash()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long afterFirst = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(afterFirst + g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Cash);
    }

    [Fact]
    public void Duo_rival_leader_player_gets_no_cash()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 80_000, "code-completion", "Rival");
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Two_domains_double_cash_when_both_led()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 42_000, "image-design");
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int domains = g.State.DomainLeaders.Count;
        Assert.True(domains >= 2);
        Assert.Equal(before + domains * g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Cash);
    }

    [Fact]
    public void Leader_cash_posts_grants_ledger()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Leader_cash_with_rp_stipend_same_week()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        long cash0 = g.Me.Cash;
        long rp0 = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(cash0 + g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Cash);
        Assert.Equal(rp0 + g.C.Market.Domain("language").LeaderResearchPerWeek, g.Me.ResearchPoints);
    }

    [Fact]
    public void Save_load_then_leader_cash_still_runs()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 48_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long cash = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Equal(cash + loaded.C.Market.Domain("language").LeaderCashPerWeek, loaded.Me.Cash);
    }

    [Fact]
    public void Negative_leader_cash_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.Domain("language").LeaderCashPerWeek = -1));
    }

    [Fact]
    public void Domain_board_required_before_cash_stipend()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "code-completion");
        Assert.Empty(g.State.DomainLeaders);
        long before = g.Me.Cash;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Single(g.State.DomainLeaders, d => d.DomainId == "language");
        Assert.Equal(before + g.C.Market.Domain("language").LeaderCashPerWeek, g.Me.Cash);
    }
}