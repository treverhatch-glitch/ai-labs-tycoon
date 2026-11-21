using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q106: challengerResearchPerWeek in CapabilityDomainSystem.</summary>
public class ChallengerResearchStipendTests
{
    static Game Duo(int year) => ExpansionTestSupport.DuoAtYear(year).WithStarterLab(gpus: 24);
    static LabState OpenAye(Game g) => g.Rival("openaye");

    [Fact]
    public void Shipped_challenger_research_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Market.Domain("language").ChallengerResearchPerWeek);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Leader_lab_gets_no_challenger_research()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 24);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").LeaderResearchPerWeek, g.Me.ResearchPoints);
    }

    [Fact]
    public void Runner_up_gains_challenger_research()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 95_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long before = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").ChallengerResearchPerWeek, rival.ResearchPoints);
    }

    [Fact]
    public void Zero_config_skips_challenger_research()
    {
        Game g = Duo(2028);
        g.C.Market.Domain("language").ChallengerResearchPerWeek = 0;
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 80_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 75_000, "doc-reading");
        long before = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.ResearchPoints);
    }

    [Fact]
    public void Bankrupt_runner_up_skips_research_grant()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 90_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 85_000, "doc-reading");
        rival.Bankrupt = true;
        long before = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.ResearchPoints);
    }

    [Fact]
    public void Distant_rival_gets_no_challenger_research()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 40_000, "doc-reading");
        long before = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, rival.ResearchPoints);
    }

    [Fact]
    public void Second_week_stacks_challenger_research()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 65_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long afterFirst = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(afterFirst + g.C.Market.Domain("language").ChallengerResearchPerWeek, rival.ResearchPoints);
    }

    [Fact]
    public void Challenger_research_with_rep_stipend_same_week()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 88_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 83_000, "doc-reading");
        long rp0 = rival.ResearchPoints;
        int rep0 = rival.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(rep0 + g.C.Market.Domain("language").ChallengerReputationPerWeek, rival.Reputation);
        Assert.Equal(rp0 + g.C.Market.Domain("language").ChallengerResearchPerWeek, rival.ResearchPoints);
    }

    [Fact]
    public void Save_load_preserves_challenger_research_path()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 72_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 68_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long rp = rival.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState loadedRival = loaded.Rival("openaye");
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Equal(rp + loaded.C.Market.Domain("language").ChallengerResearchPerWeek, loadedRival.ResearchPoints);
    }

    [Fact]
    public void Three_domains_three_runner_ups_can_earn_research()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "image-design");
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "expert-systems");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 45_000, "doc-reading");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 45_000, "image-recognition");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 45_000, "fraud-detection");
        long before = rival.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(rival.ResearchPoints > before);
    }

    [Fact]
    public void Negative_challenger_research_rejected_by_validator()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.Domain("language").ChallengerResearchPerWeek = -1));
    }

    [Fact]
    public void Solo_lab_without_models_gets_no_challenger_research()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 24);
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Formula_matches_domain_config_value()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Market.Domain("vision").ChallengerResearchPerWeek);
        Assert.Equal(c.Market.Domain("language").ChallengerResearchPerWeek,
            c.Market.Domain("enterprise").ChallengerResearchPerWeek);
    }
}
