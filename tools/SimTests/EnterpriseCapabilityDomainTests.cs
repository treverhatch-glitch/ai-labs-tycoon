using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q70: enterprise capability domain (expert-systems, spam-search).</summary>
public class EnterpriseCapabilityDomainTests
{
    static Game At(int year, Action<GameConfig> tweak = null) =>
        ExpansionTestSupport.SoloAtYear(year, tweak: tweak).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_enterprise_domain_validates()
    {
        GameConfig c = TestData.Config();
        CapabilityDomainConfig ent = c.Market.Domain("enterprise");
        Assert.NotNull(ent);
        Assert.Contains("expert-systems", ent.SectorIds);
        Assert.Contains("spam-search", ent.SectorIds);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Main_path_expert_systems_leads_enterprise()
    {
        Game g = At(2028);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 50_000, "expert-systems");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        CapabilityDomainLeaderState row = CapabilityDomainSystem.Leader(g.State, "enterprise");
        Assert.NotNull(row);
        Assert.Equal(g.Id, row.LeaderLabId);
        Assert.Equal(m.Capability, row.BestCapability);
    }

    [Fact]
    public void Zero_models_no_enterprise_leader()
    {
        Game g = At(2028);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Null(CapabilityDomainSystem.Leader(g.State, "enterprise"));
    }

    [Fact]
    public void Max_spam_search_sets_best_capability()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 30_000, "expert-systems", "Low");
        ModelState high = ExpansionTestSupport.AddReleasedModel(g, 88_000, "spam-search", "High");
        Assert.Equal(high.Capability, CapabilityDomainQueries.BestCapability(g.C, g.Me, "enterprise"));
    }

    [Fact]
    public void Bankrupt_lab_excluded_from_enterprise_leader()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 55_000, "expert-systems");
        g.Me.Bankrupt = true;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Null(CapabilityDomainSystem.Leader(g.State, "enterprise"));
    }

    [Fact]
    public void Save_load_preserves_enterprise_leader_snapshot()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 47_000, "spam-search");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.State.DomainLeaders.Count, loaded.State.DomainLeaders.Count);
        Assert.Equal(CapabilityDomainSystem.Leader(g.State, "enterprise").Footprint,
            CapabilityDomainSystem.Leader(loaded.State, "enterprise").Footprint);
    }

    [Fact]
    public void Schema_20_migration_enterprise_leader_after_run_week()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        ExpansionTestSupport.AddReleasedModel(g, 42_000, "expert-systems");
        g.State.Schema = 20;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.NotNull(CapabilityDomainSystem.Leader(loaded.State, "enterprise"));
    }

    [Fact]
    public void Game_ai_sector_not_in_enterprise_footprint()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 70_000, "game-ai");
        Assert.Equal(0, CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "enterprise"));
    }

    [Fact]
    public void Display_leader_boost_on_expert_systems()
    {
        Game g = At(2028);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 46_000, "expert-systems");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("expert-systems"),
            g.State.Sector("expert-systems"), g.Me, m);
        Assert.True(parts.DomainLeaderBoost > 0);
    }

    [Fact]
    public void Market_appeal_rises_when_enterprise_leader()
    {
        Game g = At(2028);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 48_000, "spam-search");
        var sector = g.C.Sector("spam-search");
        var sectorState = g.State.Sector("spam-search");
        long before = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, m);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long after = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, m);
        Assert.True(after > before);
    }

    [Fact]
    public void Sim_step_keeps_three_domain_leaders_independent()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 41_000, "image-design");
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "expert-systems");
        g.Weeks(1);
        Assert.Equal(3, g.State.DomainLeaders.Count);
        Assert.NotNull(CapabilityDomainSystem.Leader(g.State, "enterprise"));
    }

    [Fact]
    public void Zero_leader_appeal_config_disables_enterprise_boost()
    {
        Game g = At(2028, tweak: c => c.Market.Domain("enterprise").LeaderAppealPermille = 0);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 45_000, "expert-systems");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "expert-systems"));
        Assert.Equal(0, DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("expert-systems"),
            g.State.Sector("expert-systems"), g.Me, m).DomainLeaderBoost);
    }

    [Fact]
    public void Both_enterprise_sectors_combine_footprint()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "expert-systems");
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "spam-search");
        long foot = CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "enterprise");
        Assert.True(foot > 0);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.True(CapabilityDomainSystem.Leader(g.State, "enterprise").Footprint >= foot);
    }
}