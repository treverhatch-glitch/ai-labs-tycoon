using AiLabs.Rules.Core;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SectorPricingTests
{
    static Game Lab()
    {
        Game g = Game.Solo(11, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.SectorPricing.MinEra = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Set_sector_price_tier_persists()
    {
        Game g = Lab();
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 2));
        Assert.Equal(2, SectorPricing.Tier(g.C, g.Me, "game-ai"));
        Assert.Equal(MathI.Permille + 2 * g.C.SectorPricing.RevenuePerTier, SectorPricing.RevenuePermille(g.C, g.Me, "game-ai"));
    }

    [Fact]
    public void Older_save_has_no_price_tiers()
    {
        Game g = Lab();
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 1));
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 137, g.Sim.Save()));
        Assert.Empty(older.Player.PriceSectors);
    }
}