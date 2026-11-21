using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q100: challengerReputationPerWeek for runner-up lab in CapabilityDomainSystem.RunWeek.</summary>
public class ChallengerStipendTests
{
    static Game Duo(int year) => ExpansionTestSupport.DuoAtYear(year).WithStarterLab(gpus: 24);

    static LabState OpenAye(Game g) => g.Rival("openaye");

    [Fact]
    public void Shipped_challenger_stipend_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Market.Domain("language").ChallengerReputationPerWeek);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Solo_lab_no_challenger_stipend()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 24);
        ExpansionTestSupport.AddReleasedModel(g, 60_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(g.Id, CapabilityDomainSystem.Leader(g.State, "language").LeaderLabId);
    }

    [Fact]
    public void Domain_leader_gets_no_challenger_stipend()
    {
        Game g = Duo(2028);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        int repBefore = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int leaderGain = g.C.Market.Domain("language").LeaderReputationPerWeek;
        Assert.Equal(repBefore + leaderGain, g.Me.Reputation);
    }

    [Fact]
    public void Runner_up_gains_challenger_reputation()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 95_000, "code-completion", "Chaser");
        int before = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").ChallengerReputationPerWeek, rival.Reputation);
    }

    [Fact]
    public void Distant_lab_no_challenger_stipend()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 20_000, "doc-reading", "Far");
        int before = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.Reputation);
    }

    [Fact]
    public void Zero_challenger_stipend_config_skips_grant()
    {
        Game g = Duo(2028);
        g.C.Market.Domain("language").ChallengerReputationPerWeek = 0;
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 80_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 75_000, "doc-reading");
        int before = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.Reputation);
    }

    [Fact]
    public void Bankrupt_runner_up_skips_stipend()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 90_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 85_000, "doc-reading");
        rival.Bankrupt = true;
        int before = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.Reputation);
    }

    [Fact]
    public void Second_run_week_stacks_challenger_stipend()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 65_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int afterFirst = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(afterFirst + g.C.Market.Domain("language").ChallengerReputationPerWeek, rival.Reputation);
    }

    [Fact]
    public void Challenger_stipend_clamped_at_permille()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        rival.Reputation = 999;
        ExpansionTestSupport.AddReleasedModel(g, 60_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 55_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(rival.Reputation <= 1000);
    }

    [Fact]
    public void Sim_week_applies_challenger_stipend()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 88_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 82_000, "doc-reading");
        int before = rival.Reputation;
        g.Weeks(1);
        Assert.True(rival.Reputation >= before + g.C.Market.Domain("language").ChallengerReputationPerWeek);
    }

    [Fact]
    public void Save_load_preserves_challenger_stipend_path()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 72_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 68_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int rep = rival.Reputation;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState rivalLoaded = loaded.Rival("openaye");
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Equal(rep + loaded.C.Market.Domain("language").ChallengerReputationPerWeek, rivalLoaded.Reputation);
    }

    [Fact]
    public void Negative_challenger_stipend_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.Domain("language").ChallengerReputationPerWeek = -1));
    }

    [Fact]
    public void Leader_and_challenger_both_stipended_same_week()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        int playerBefore = g.Me.Reputation;
        int rivalBefore = rival.Reputation;
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 95_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(playerBefore + g.C.Market.Domain("language").LeaderReputationPerWeek, g.Me.Reputation);
        Assert.Equal(rivalBefore + g.C.Market.Domain("language").ChallengerReputationPerWeek, rival.Reputation);
    }
}