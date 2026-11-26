using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q96: leaderReputationPerWeek in CapabilityDomainSystem.RunWeek.</summary>
public class DomainLeaderStipendTests
{
    static Game At(int year) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_leader_reputation_stipend_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.Market.Domain("language").LeaderReputationPerWeek);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_models_no_leader_stipend()
    {
        Game g = At(2028);
        int before = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Domain_leader_gains_weekly_reputation()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        int before = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").LeaderReputationPerWeek, g.Me.Reputation);
    }

    [Fact]
    public void Zero_stipend_config_skips_grant()
    {
        Game g = At(2028);
        g.C.Market.Domain("language").LeaderReputationPerWeek = 0;
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        int before = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Bankrupt_leader_gets_no_stipend()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        g.Me.Bankrupt = true;
        int rep = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(rep, g.Me.Reputation);
    }

    [Fact]
    public void Second_run_week_stacks_stipend_again()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int afterFirst = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(afterFirst + g.C.Market.Domain("language").LeaderReputationPerWeek, g.Me.Reputation);
    }

    [Fact]
    public void Duo_rival_leader_player_gets_no_stipend()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 80_000, "code-completion", "Rival");
        int before = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.Reputation);
        Assert.Equal(rival.Id, CapabilityDomainSystem.Leader(g.State, "language").LeaderLabId);
    }

    [Fact]
    public void Two_domains_double_stipend_when_both_led()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 42_000, "image-design");
        int before = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int domains = g.State.DomainLeaders.Count;
        Assert.True(domains >= 2);
        Assert.Equal(before + domains * g.C.Market.Domain("language").LeaderReputationPerWeek, g.Me.Reputation);
    }

    [Fact]
    public void Leader_stipend_then_safety_drift_when_at_baseline()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        int before = g.Me.Reputation;
        Assert.Equal(g.C.Safety.ReputationBaseline, before);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").LeaderReputationPerWeek, g.Me.Reputation);
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Safety.ReputationBaseline, g.Me.Reputation);
    }

    [Fact]
    public void Sim_week_applies_leader_stipend()
    {
        int repStart = TestData.Config().Safety.ReputationBaseline + 40;
        Game bare = At(2028);
        bare.Me.Reputation = repStart;
        bare.Weeks(1);
        long bareAfter = bare.Me.Reputation;

        Game g = At(2028);
        g.Me.Reputation = repStart;
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "code-completion");
        int stipend = g.C.Market.Domain("language").LeaderReputationPerWeek;
        g.Weeks(1);
        Assert.True(g.Me.Reputation >= bareAfter + stipend);
    }

    [Fact]
    public void Save_load_then_stipend_still_runs()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 48_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int rep = g.Me.Reputation;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Equal(rep + loaded.C.Market.Domain("language").LeaderReputationPerWeek, loaded.Me.Reputation);
    }

    [Fact]
    public void Leader_stipend_clamped_at_permille_cap()
    {
        Game g = At(2028);
        g.Me.Reputation = 998;
        ExpansionTestSupport.AddReleasedModel(g, 60_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(g.Me.Reputation <= 1000);
    }

    [Fact]
    public void Negative_leader_stipend_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.Domain("language").LeaderReputationPerWeek = -1));
    }

    [Fact]
    public void Domain_board_required_before_stipend()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "code-completion");
        Assert.Empty(g.State.DomainLeaders);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Single(g.State.DomainLeaders, d => d.DomainId == "language");
    }
}