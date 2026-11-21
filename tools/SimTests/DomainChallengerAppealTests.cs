using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q78: runner-up labs near the domain leader earn a smaller appeal boost.</summary>
public class DomainChallengerAppealTests
{
    static Game Duo(int year) => ExpansionTestSupport.DuoAtYear(year).WithStarterLab(gpus: 24);

    static LabState OpenAye(Game g) => g.Rival("openaye");

    [Fact]
    public void Shipped_challenger_fields_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(60, c.Market.Domain("language").ChallengerAppealPermille);
        Assert.Equal(900, c.Market.Domain("language").ChallengerNearLeaderPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Domain_leader_gets_zero_challenger_boost()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 24);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, g.Me, "code-completion"));
    }

    [Fact]
    public void Near_runner_up_gets_challenger_boost()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 95_000, "code-completion", "Chaser");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, g.Me, "code-completion"));
        Assert.True(CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "code-completion") > 0);
    }

    [Fact]
    public void Distant_runner_up_gets_no_boost()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 45_000, "doc-reading", "Far");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading"));
    }

    [Fact]
    public void Leader_lab_never_challenger_even_in_other_domain_sector()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "code-completion", "Leader");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 95_000, "doc-reading", "Second");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, g.Me, "doc-reading"));
        Assert.True(CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading") > 0);
    }

    [Fact]
    public void Appeal_breakdown_includes_challenger_part()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 80_000, "code-completion", "Leader");
        ModelState chaser = ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 75_000, "doc-reading", "Chaser");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var sector = g.C.Sector("doc-reading");
        var sectorState = g.State.Sector("doc-reading");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, rival, chaser);
        Assert.True(parts.DomainChallengerBoost > 0);
        Assert.True(parts.DomainLeaderBoost == 0);
        Assert.Equal(parts.Total, MarketSystem.Appeal(g.C, g.State, sector, sectorState, rival, chaser));
    }

    [Fact]
    public void Bankrupt_challenger_gets_zero()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 65_000, "doc-reading");
        rival.Bankrupt = true;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading"));
    }

    [Fact]
    public void Zero_challenger_config_disables_boost()
    {
        Game g = Duo(2028);
        g.C.Market.Domain("language").ChallengerAppealPermille = 0;
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 68_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading"));
    }

    [Fact]
    public void Non_domain_sector_zero_challenger()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 52_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "game-ai"));
    }

    [Fact]
    public void Save_load_round_trip_challenger_appeal()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 72_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 68_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int before = CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState rival2 = loaded.State.Labs.First(l => l.Id == rival.Id);
        int after = CapabilityDomainSystem.ChallengerAppealPermille(loaded.C, loaded.State, rival2, "doc-reading");
        Assert.Equal(before, after);
    }

    [Fact]
    public void Leader_boost_exceeds_challenger_boost_in_same_domain()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ModelState leaderModel = ExpansionTestSupport.AddReleasedModel(g, 90_000, "code-completion");
        ModelState chaser = ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 85_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int leader = CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "code-completion");
        int chase = CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading");
        Assert.True(leader > chase);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, leaderModel).DomainLeaderBoost
            > DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("doc-reading"), g.State.Sector("doc-reading"), rival, chaser).DomainChallengerBoost);
    }

    [Fact]
    public void Near_leader_threshold_uses_permille_of_leader_footprint()
    {
        Game below = Duo(2028);
        LabState rivalBelow = OpenAye(below);
        ExpansionTestSupport.AddReleasedModel(below, 100_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(below, rivalBelow, 89_000, "code-completion");
        CapabilityDomainSystem.RunWeek(below.C, below.State);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(below.C, below.State, rivalBelow, "code-completion"));

        Game at = Duo(2028);
        LabState rivalAt = OpenAye(at);
        ExpansionTestSupport.AddReleasedModel(at, 100_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(at, rivalAt, 90_000, "code-completion");
        CapabilityDomainSystem.RunWeek(at.C, at.State);
        Assert.True(CapabilityDomainSystem.ChallengerAppealPermille(at.C, at.State, rivalAt, "code-completion") > 0);
    }

    [Fact]
    public void Back_to_back_run_weeks_keep_challenger_for_runner_up()
    {
        Game g = Duo(2028);
        LabState rival = OpenAye(g);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "code-completion");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 66_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading") > 0);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, rival, "doc-reading") > 0);
        Assert.Equal(0, CapabilityDomainSystem.ChallengerAppealPermille(g.C, g.State, g.Me, "doc-reading"));
    }
}