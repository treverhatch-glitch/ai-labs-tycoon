using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q69: fraud-detection sector rolls into vision capability domain.</summary>
public class VisionDomainBreadthTests
{
    static Game At(int year) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_fraud_detection_in_vision_domain()
    {
        GameConfig c = TestData.Config();
        CapabilityDomainConfig vision = c.Market.Domain("vision");
        Assert.Contains("fraud-detection", vision.SectorIds);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Main_path_fraud_release_leads_vision()
    {
        Game g = At(2030);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 55_000, "fraud-detection");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        CapabilityDomainLeaderState row = CapabilityDomainSystem.Leader(g.State, "vision");
        Assert.NotNull(row);
        Assert.Equal(g.Id, row.LeaderLabId);
        Assert.Equal(m.Capability, row.BestCapability);
    }

    [Fact]
    public void Zero_unreleased_model_no_vision_footprint()
    {
        Game g = At(2030);
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Capability = 90_000, Status = ModelStatus.Trained,
        });
        Assert.Equal(0, CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "vision"));
    }

    [Fact]
    public void Max_capability_picked_across_vision_sectors()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "image-recognition", "Low");
        ModelState high = ExpansionTestSupport.AddReleasedModel(g, 95_000, "fraud-detection", "High");
        Assert.Equal(high.Capability, CapabilityDomainQueries.BestCapability(g.C, g.Me, "vision"));
    }

    [Fact]
    public void Bankrupt_lab_not_vision_leader()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "fraud-detection");
        g.Me.Bankrupt = true;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Null(CapabilityDomainSystem.Leader(g.State, "vision"));
    }

    [Fact]
    public void Save_load_preserves_fraud_vision_leader()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 48_000, "fraud-detection");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(CapabilityDomainSystem.Leader(g.State, "vision").LeaderLabId,
            CapabilityDomainSystem.Leader(loaded.State, "vision").LeaderLabId);
        Assert.True(CapabilityDomainQueries.BestCapability(loaded.C, loaded.Me, "vision") > 0);
    }

    [Fact]
    public void Schema_21_round_trip_empty_leaders_then_fraud_run_week()
    {
        Game g = Game.Solo();
        Assert.Equal(24, g.State.Schema);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2030);
        g.WithStarterLab(gpus: 20);
        ExpansionTestSupport.AddReleasedModel(g, 44_000, "fraud-detection");
        g.Weeks(1);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.NotNull(CapabilityDomainSystem.Leader(loaded.State, "vision"));
    }

    [Fact]
    public void Fraud_not_counted_in_language_or_enterprise()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 60_000, "fraud-detection");
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "enterprise"));
    }

    [Fact]
    public void Display_queries_leader_boost_on_fraud_sector()
    {
        Game g = At(2030);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 52_000, "fraud-detection");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("fraud-detection"),
            g.State.Sector("fraud-detection"), g.Me, m);
        Assert.True(parts.DomainLeaderBoost > 0);
    }

    [Fact]
    public void Market_appeal_includes_domain_leader_on_fraud()
    {
        Game g = At(2030);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 50_000, "fraud-detection");
        var sector = g.C.Sector("fraud-detection");
        var sectorState = g.State.Sector("fraud-detection");
        long flat = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, m);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long boosted = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, m);
        Assert.True(boosted > flat);
    }

    [Fact]
    public void Sim_week_refreshes_vision_with_fraud_model()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 46_000, "fraud-detection");
        g.Weeks(1);
        Assert.NotNull(CapabilityDomainSystem.Leader(g.State, "vision"));
    }

    [Fact]
    public void Zero_leader_appeal_when_rival_leads_vision()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2030).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 80_000, "fraud-detection");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, CapabilityDomainSystem.LeaderAppealPermille(g.C, g.State, g.Me, "fraud-detection"));
    }

    [Fact]
    public void Image_and_fraud_footprint_sums_in_vision_domain()
    {
        Game g = At(2030);
        ExpansionTestSupport.AddReleasedModel(g, 35_000, "image-design");
        ExpansionTestSupport.AddReleasedModel(g, 35_000, "fraud-detection");
        long foot = CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "vision");
        Assert.True(foot > CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "language"));
    }
}