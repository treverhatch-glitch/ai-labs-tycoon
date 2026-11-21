using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q102: leaderResearchPerWeek stipend in CapabilityDomainSystem.RunWeek.</summary>
public class DomainLeaderResearchTests
{
    static Game At(int year) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_leader_research_stipend_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(5, c.Market.Domain("language").LeaderResearchPerWeek);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_leader_no_research_stipend()
    {
        Game g = At(2028);
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Domain_leader_gains_weekly_research()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before + g.C.Market.Domain("language").LeaderResearchPerWeek, g.Me.ResearchPoints);
    }

    [Fact]
    public void Zero_config_skips_research_stipend()
    {
        Game g = At(2028);
        g.C.Market.Domain("language").LeaderResearchPerWeek = 0;
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Bankrupt_leader_skips_research_grant()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        g.Me.Bankrupt = true;
        long points = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(points, g.Me.ResearchPoints);
    }

    [Fact]
    public void Second_week_stacks_research_stipend()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long afterFirst = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(afterFirst + g.C.Market.Domain("language").LeaderResearchPerWeek, g.Me.ResearchPoints);
    }

    [Fact]
    public void Duo_runner_up_gets_no_leader_research()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 80_000, "code-completion", "Rival");
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Multiple_domains_multiply_research_grant()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 42_000, "image-design");
        long before = g.Me.ResearchPoints;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int domains = g.State.DomainLeaders.Count(d => d.LeaderLabId == g.Id);
        Assert.True(domains >= 2);
        Assert.Equal(before + domains * g.C.Market.Domain("language").LeaderResearchPerWeek, g.Me.ResearchPoints);
    }

    [Fact]
    public void Sim_week_applies_leader_research()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 45_000, "doc-reading");
        long before = g.Me.ResearchPoints;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints >= before + g.C.Market.Domain("language").LeaderResearchPerWeek);
    }

    [Fact]
    public void Save_load_then_research_stipend_runs()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 48_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long points = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Equal(points + loaded.C.Market.Domain("language").LeaderResearchPerWeek, loaded.Me.ResearchPoints);
    }

    [Fact]
    public void Research_stipend_independent_of_reputation_stipend()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "code-completion");
        long rpBefore = g.Me.ResearchPoints;
        int repBefore = g.Me.Reputation;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(g.Me.ResearchPoints > rpBefore);
        Assert.True(g.Me.Reputation > repBefore);
    }

    [Fact]
    public void Negative_leader_research_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.Domain("language").LeaderResearchPerWeek = -1));
    }

    [Fact]
    public void Overtaken_leader_loses_research_stipend()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long asLeader = g.Me.ResearchPoints;
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 200_000, "code-completion", "King");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(asLeader, g.Me.ResearchPoints);
        Assert.True(rival.ResearchPoints > 0);
    }
}