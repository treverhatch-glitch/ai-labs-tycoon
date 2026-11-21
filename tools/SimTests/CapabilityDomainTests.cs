using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q63: market.capabilityDomains group sectors for explainable domain scores.</summary>
public class CapabilityDomainTests
{
    static Game At(int year, int gpus = 20) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: gpus);

    [Fact]
    public void Shipped_language_and_vision_domains_validate()
    {
        GameConfig c = TestData.Config();
        Assert.NotNull(c.Market.Domain("language"));
        Assert.NotNull(c.Market.Domain("vision"));
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_release_zero_capability()
    {
        Game g = At(2028, gpus: 8);
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Released_sector_in_domain_sets_best()
    {
        Game g = At(2028, gpus: 16);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 20_000, "code-completion");
        Assert.Equal(m.Capability, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Unrelated_sector_does_not_count_in_language()
    {
        Game g = At(1998, gpus: 16);
        ExpansionTestSupport.AddReleasedModel(g, 500, "game-ai");
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Best_picks_highest_across_domain_sectors()
    {
        Game g = At(2028, gpus: 24);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, "doc-reading");
        ExpansionTestSupport.AddReleasedModel(g, 180_000, "code-completion");
        Assert.Equal(180_000, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Retired_model_does_not_count()
    {
        Game g = At(2028, gpus: 16);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 20_000, "code-completion");
        m.Status = ModelStatus.Retired;
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Bankruptcy_returns_zero()
    {
        Game g = At(2028, gpus: 16);
        ExpansionTestSupport.AddReleasedModel(g, 20_000, "image-recognition");
        g.Me.Bankrupt = true;
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "vision"));
    }

    [Fact]
    public void Domain_footprint_sums_quality_in_domain_sectors()
    {
        Game g = At(2028, gpus: 24);
        ExpansionTestSupport.AddReleasedModel(g, 25_000, "code-completion", "Dual");
        long foot = CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "language");
        Assert.True(foot > 0);
    }

    [Fact]
    public void Footprint_zero_outside_domain()
    {
        Game g = At(1998, gpus: 16);
        ExpansionTestSupport.AddReleasedModel(g, 500, "game-ai");
        Assert.Equal(0, CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "vision"));
    }

    [Fact]
    public void Unknown_domain_returns_zero()
    {
        Game g = At(2028, gpus: 8);
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "robotics"));
    }

    [Fact]
    public void Vision_domain_uses_image_sectors()
    {
        Game g = At(2028, gpus: 20);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 25_000, "image-design");
        Assert.Equal(m.Capability, CapabilityDomainQueries.BestCapability(g.C, g.Me, "vision"));
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Gauntlet_cross_sector_still_scores_per_bracket()
    {
        Game g = At(2028, gpus: 24);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 30_000, "code-completion");
        long quality = LabQueries.Quality(g.C, g.C.Sector("code-completion"), m, g.Tick);
        Assert.True(CapabilityDomainQueries.DomainFootprint(g.C, g.State, g.Me, "language") >= quality);
    }

    [Fact]
    public void Save_load_preserves_domain_scores()
    {
        Game g = At(2028, gpus: 20);
        ExpansionTestSupport.AddReleasedModel(g, 22_000, "doc-reading");
        int best = CapabilityDomainQueries.BestCapability(g.C, g.Me, "language");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(best, CapabilityDomainQueries.BestCapability(loaded.C, loaded.Me, "language"));
    }

    [Fact]
    public void Max_capability_across_lines_still_one_domain_value()
    {
        Game g = At(2028, gpus: 32);
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "code-completion", "A");
        ExpansionTestSupport.AddReleasedModel(g, 90_000, "code-completion", "B");
        Assert.Equal(90_000, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }

    [Fact]
    public void Device_workshop_cross_does_not_change_domain_capability()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028);
        g.WithStarterLab(gpus: 16);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 130_000, "devices");
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "vision"));
        Assert.Equal(0, CapabilityDomainQueries.BestCapability(g.C, g.Me, "language"));
    }
}