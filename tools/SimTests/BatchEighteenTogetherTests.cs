using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q482 combination wiring for batch eighteen systems.</summary>
public class BatchEighteenTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(82, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.SectorPricing.MinEra = 0;
            c.OpenWeights.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Specialist_era_hook_stacks_with_open_weights()
    {
        Game g = Lab(c =>
        {
            c.SpecialistEraHooks.Enabled = 1;
            c.SpecialistEraHooks.MinEra = 0;
            c.SpecialistEraHooks.OpenWeightsPermille = 50;
            c.OpenWeights.Enabled = 1;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        long boosted = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, m);
        g.C.SpecialistEraHooks.Enabled = 0;
        long plain = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, m);
        Assert.True(boosted > plain);
    }

    [Fact]
    public void Sector_ladder_peak_with_market_revenue_permille()
    {
        Game g = Lab(c =>
        {
            c.SectorPricing.Enabled = 1;
            c.SectorPricingBreakdown.Enabled = 1;
            c.SectorPricing.MaxTier = 3;
            c.SectorPricing.RevenuePerTier = 80;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 3));
        var parts = DisplayQueries.SectorPriceParts(g.C, g.Me, "game-ai");
        Assert.Equal(1240, parts.TotalRevenuePermille);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SectorPriceLadderPeak);
    }
}