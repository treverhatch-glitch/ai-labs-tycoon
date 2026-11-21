using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q65: weekly domain leaders and appeal hook (schema 21).</summary>
public class CapabilityDomainSystemTests
{
    static Game At(int year, Action<GameConfig> tweak = null) => ExpansionTestSupport.SoloAtYear(year, tweak: tweak).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_domain_leader_appeal_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(120, c.Market.Domain("language").LeaderAppealPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void New_game_schema_23_and_empty_leaders()
    {
        Game g = Game.Solo();
        Assert.Equal(23, g.State.Schema);
        Assert.Empty(g.State.DomainLeaders);
    }

    [Fact]
    public void No_models_no_leaders_after_run_week()
    {
        Game g = At(2028);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Empty(g.State.DomainLeaders);
    }

    [Fact]
    public void Run_week_picks_highest_footprint_lab()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion", "A");
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "doc-reading", "B");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        CapabilityDomainLeaderState lang = CapabilityDomainSystem.Leader(g.State, "language");
        Assert.NotNull(lang);
        Assert.Equal(g.Id, lang.LeaderLabId);
        Assert.True(lang.Footprint > 0);
    }

    [Fact]
    public void Bankrupt_lab_not_leader()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 30_000, "code-completion");
        g.Me.Bankrupt = true;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(g.State.DomainLeaders.Count == 0 || CapabilityDomainSystem.Leader(g.State, "language")?.LeaderLabId != g.Me.Id);
    }

    [Fact]
    public void Leader_appeal_zero_when_not_in_sector()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 30_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "game-ai"));
    }

    [Fact]
    public void Leader_appeal_positive_in_domain_sector()
    {
        Game g = At(2028);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 35_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, m).DomainLeaderBoost > 0);
    }

    [Fact]
    public void Sim_step_refreshes_leaders()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        g.Weeks(1);
        Assert.NotEmpty(g.State.DomainLeaders);
    }

    [Fact]
    public void Save_load_round_trip_leaders()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 33_000, "image-design");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.State.DomainLeaders.Count, loaded.State.DomainLeaders.Count);
        Assert.Equal(CapabilityDomainSystem.Leader(g.State, "vision").LeaderLabId,
            CapabilityDomainSystem.Leader(loaded.State, "vision").LeaderLabId);
    }

    [Fact]
    public void Schema_20_save_migrates_domain_leaders()
    {
        Game g = Game.Solo();
        g.State.Schema = 20;
        string json = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(23, loaded.State.Schema);
        Assert.NotNull(loaded.State.DomainLeaders);
    }

    [Fact]
    public void Tie_breaker_uses_best_capability_then_lab_id()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion", "Low");
        ExpansionTestSupport.AddReleasedModel(g, 90_000, "code-completion", "High");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(g.Id, CapabilityDomainSystem.Leader(g.State, "language").LeaderLabId);
    }

    [Fact]
    public void Zero_leader_appeal_config_disables_boost()
    {
        Game g = At(2028, tweak: c => c.Market.Domain("language").LeaderAppealPermille = 0);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 30_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "code-completion"));
        Assert.Equal(0, DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, m).DomainLeaderBoost);
    }

    [Fact]
    public void Vision_and_language_leaders_independent()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 45_000, "image-design", "V");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.NotNull(CapabilityDomainSystem.Leader(g.State, "language"));
        Assert.NotNull(CapabilityDomainSystem.Leader(g.State, "vision"));
    }

    [Fact]
    public void Coding_agents_sector_counts_in_language_domain()
    {
        Game g = At(2030);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 60_000, "coding-agents");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(m.Capability, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
        Assert.True(CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "coding-agents") > 0);
    }

}