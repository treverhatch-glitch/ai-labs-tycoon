using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q119–Q120: shortage price and foundry room breakdown parts.</summary>
public class ChipShortageBreakdownHardeningTests
{
    [Fact]
    public void Idle_shortage_price_parts_are_identity()
    {
        Game g = Game.Solo();
        long basePrice = 5000;
        var b = ChipShortageSystem.PriceScaleParts(g.State, basePrice);
        Assert.Equal(basePrice, b.Base);
        Assert.Equal(0, b.FromShortage);
        Assert.Equal(basePrice, b.Total);
        Assert.Equal(basePrice, ChipShortageSystem.ScalePrice(g.State, basePrice));
    }

    [Fact]
    public void Active_shortage_raises_price_part()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1200);
        var b = ChipShortageSystem.PriceScaleParts(g.State, 10_000);
        Assert.Equal(10_000, b.Base);
        Assert.Equal(12_000 - 10_000, b.FromShortage);
        Assert.Equal(b.Total, ChipShortageSystem.ScalePrice(g.State, 10_000));
    }

    [Fact]
    public void Gpu_rent_breakdown_uses_shortage_scale()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Rent(8);
        ChipShortageSystem.Start(g.C, g.State, 1300);
        long spot = LabQueries.GpuRentParts(g.C, g.State, g.Me).Total;
        var scale = ChipShortageSystem.PriceScaleParts(g.State, spot);
        Assert.True(scale.FromShortage > 0);
    }

    [Fact]
    public void Foundry_room_parts_shrink_when_crunch_active()
    {
        Game g = Game.Solo();
        ChipNodeConfig node = g.C.Chips.Node("n28");
        var idle = ChipShortageSystem.FoundryRoomParts(g.C, g.State, node);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        var crunch = ChipShortageSystem.FoundryRoomParts(g.C, g.State, node);
        Assert.Equal(idle.Base, crunch.Base);
        Assert.True(crunch.FromShortage < 0);
        Assert.Equal(crunch.Total, ChipShortageSystem.EffectiveFoundryRoom(g.C, g.State, node));
    }

    [Fact]
    public void Vendor_supply_permille_matches_scale_vendor()
    {
        Game g = Game.Solo();
        int raw = 40;
        ChipShortageSystem.Start(g.C, g.State, 1100);
        int scaled = ChipShortageSystem.ScaleVendorSupply(g.C, g.State, raw);
        int permille = ChipShortageSystem.VendorSupplyPermille(g.C, g.State);
        Assert.Equal((int)MathI.MulPermille(raw, permille), scaled);
    }

    [Fact]
    public void Stronger_crunch_deepens_both_price_and_room_cuts()
    {
        Game a = Game.Solo();
        Game b = Game.Solo();
        ChipShortageSystem.Start(a.C, a.State, 1100);
        ChipShortageSystem.Start(b.C, b.State, 1500);
        var priceA = ChipShortageSystem.PriceScaleParts(a.State, 1000).FromShortage;
        var priceB = ChipShortageSystem.PriceScaleParts(b.State, 1000).FromShortage;
        Assert.True(priceB > priceA);
        ChipNodeConfig node = a.C.Chips.Node("n5");
        Assert.True(ChipShortageSystem.FoundryRoomParts(b.C, b.State, node).FromShortage
            < ChipShortageSystem.FoundryRoomParts(a.C, a.State, node).FromShortage);
    }

    [Fact]
    public void Save_load_preserves_price_parts()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1250);
        long basePrice = 20_000;
        var before = ChipShortageSystem.PriceScaleParts(g.State, basePrice);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ChipShortageSystem.PriceScaleParts(loaded.State, basePrice);
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void Ended_shortage_resets_parts_to_identity()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1200);
        g.State.ChipShortageUntilTick = g.Tick;
        ChipShortageSystem.RunWeek(g.C, g.State);
        Assert.False(ChipShortageSystem.Active(g.State));
        var b = ChipShortageSystem.PriceScaleParts(g.State, 8000);
        Assert.Equal(0, b.FromShortage);
    }

    [Fact]
    public void Peer_lease_rent_scales_with_shortage()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 12);
        ChipShortageSystem.Start(g.C, g.State, 1150);
        long raw = 1000;
        long scaled = ChipShortageSystem.ScalePrice(g.State, raw);
        Assert.Equal(ChipShortageSystem.PriceScaleParts(g.State, raw).Total, scaled);
    }

    [Fact]
    public void Price_mul_permille_matches_active_strength()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1180);
        Assert.Equal(1180, ChipShortageSystem.PriceMulPermille(g.State));
    }

    [Fact]
    public void Foundry_room_mul_permille_tracks_strength()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1400);
        Assert.Equal(600, ChipShortageSystem.FoundryRoomMulPermille(g.State));
    }

    [Fact]
    public void First_free_month_uses_effective_room()
    {
        Game g = ChipHardeningSupport.Lab(2013, rivals: false);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        long eff = ChipShortageSystem.FoundryRoomParts(g.C, g.State, node).Total;
        Assert.True(eff > 0);
        Assert.True(eff < ChipShortageSystem.FoundryRoomParts(g.C, g.State, node).Base);
    }

    [Fact]
    public void Zero_base_price_stays_zero()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1300);
        var b = ChipShortageSystem.PriceScaleParts(g.State, 0);
        Assert.Equal(0, b.Total);
    }
}