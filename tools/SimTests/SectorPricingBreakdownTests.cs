using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SectorPricingBreakdownTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(80, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SectorPricing.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Breakdown_matches_revenue_and_overload_permille()
    {
        Game g = Lab(c =>
        {
            c.SectorPricing.Enabled = 1;
            c.SectorPricing.MaxTier = 3;
            c.SectorPricing.RevenuePerTier = 80;
            c.SectorPricing.OverloadPerTier = 12;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 2));
        var p = DisplayQueries.SectorPriceParts(g.C, g.Me, "game-ai");
        Assert.Equal(2, p.Tier);
        Assert.Equal(1000 + 160, p.TotalRevenuePermille);
        Assert.Equal(SectorPricing.RevenuePermille(g.C, g.Me, "game-ai"), p.TotalRevenuePermille);
        Assert.Equal(24, p.OverloadPermille);
    }

    [Fact]
    public void Max_tier_fires_ladder_peak_when_enabled()
    {
        Game g = Lab(c =>
        {
            c.SectorPricing.Enabled = 1;
            c.SectorPricingBreakdown.Enabled = 1;
            c.SectorPricingBreakdown.PeakTierNotify = 1;
            c.SectorPricing.MaxTier = 3;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 3));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SectorPriceLadderPeak);
    }
}