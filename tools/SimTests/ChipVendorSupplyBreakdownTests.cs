using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q133: ChipShortageSystem.VendorSupplyParts matches scaled Envydia supply.</summary>
public class ChipVendorSupplyBreakdownTests
{
    [Fact]
    public void Shipped_vendor_supply_parts_match_scale()
    {
        Game g = Game.Solo();
        int raw = 40;
        var b = ChipShortageSystem.VendorSupplyParts(g.C, g.State, raw);
        Assert.Equal(raw, b.Base);
        Assert.Equal(ChipShortageSystem.ScaleVendorSupply(g.C, g.State, raw), b.Scaled);
        Assert.Equal(b.Scaled - b.Base, b.FromShortage);
    }

    [Fact]
    public void Idle_shortage_identity_scale()
    {
        Game g = Game.Solo();
        var b = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 100);
        Assert.Equal(100, b.Scaled);
        Assert.Equal(0, b.FromShortage);
    }

    [Fact]
    public void Active_shortage_reduces_supply()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1250);
        var b = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 80);
        Assert.True(b.Scaled < b.Base);
        Assert.True(b.FromShortage < 0);
    }

    [Fact]
    public void Stronger_shortage_deeper_cut()
    {
        Game a = Game.Solo(seed: 1);
        Game b = Game.Solo(seed: 2);
        ChipShortageSystem.Start(a.C, a.State, 1100);
        ChipShortageSystem.Start(b.C, b.State, 1500);
        int fromA = ChipShortageSystem.VendorSupplyParts(a.C, a.State, 100).FromShortage;
        int fromB = ChipShortageSystem.VendorSupplyParts(b.C, b.State, 100).FromShortage;
        Assert.True(fromB < fromA);
    }

    [Fact]
    public void Vendor_supply_permille_matches_parts_ratio()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1300);
        int permille = ChipShortageSystem.VendorSupplyPermille(g.C, g.State);
        var b = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 200);
        Assert.Equal(b.Scaled, b.Base * permille / MathI.Permille);
    }

    [Fact]
    public void Zero_base_zero_scaled()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1200);
        var b = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 0);
        Assert.Equal(0, b.Scaled);
        Assert.Equal(0, b.FromShortage);
    }

    [Fact]
    public void Save_load_vendor_parts_stable()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1180);
        var before = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 55);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ChipShortageSystem.VendorSupplyParts(loaded.C, loaded.State, 55);
        Assert.Equal(before.Scaled, after.Scaled);
    }

    [Fact]
    public void Shortage_end_restores_vendor_supply()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1200);
        int crunch = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 60).Scaled;
        g.State.ChipShortageUntilTick = g.Tick;
        ChipShortageSystem.RunWeek(g.C, g.State);
        int normal = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 60).Scaled;
        Assert.True(crunch < normal);
        Assert.Equal(60, normal);
    }

    [Fact]
    public void Price_and_vendor_permille_both_active_under_shortage()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1350);
        Assert.True(ChipShortageSystem.PriceMulPermille(g.State) > MathI.Permille);
        Assert.True(ChipShortageSystem.VendorSupplyPermille(g.C, g.State) < MathI.Permille);
    }

    [Fact]
    public void Deterministic_vendor_parts()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1220);
        var x = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 33);
        var y = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 33);
        Assert.Equal(x.Scaled, y.Scaled);
    }

    [Fact]
    public void Large_base_supply_integer_round_trip()
    {
        Game g = Game.Solo();
        ChipShortageSystem.Start(g.C, g.State, 1400);
        int raw = 10_000;
        Assert.Equal(ChipShortageSystem.ScaleVendorSupply(g.C, g.State, raw),
            ChipShortageSystem.VendorSupplyParts(g.C, g.State, raw).Scaled);
    }

    [Fact]
    public void Foundry_room_parts_share_shortage_episode()
    {
        Game g = Game.Solo();
        var node = g.C.Chips.Node("n28");
        ChipShortageSystem.Start(g.C, g.State, 1250);
        var vendor = ChipShortageSystem.VendorSupplyParts(g.C, g.State, 50);
        var foundry = ChipShortageSystem.FoundryRoomParts(g.C, g.State, node);
        Assert.True(vendor.FromShortage < 0);
        Assert.True(foundry.FromShortage < 0);
    }

    [Fact]
    public void Gpu_buy_path_uses_vendor_scale_function()
    {
        Game g = Game.Solo();
        int stock = 12;
        ChipShortageSystem.Start(g.C, g.State, 1280);
        int scaled = ChipShortageSystem.ScaleVendorSupply(g.C, g.State, stock);
        Assert.Equal(ChipShortageSystem.VendorSupplyParts(g.C, g.State, stock).Scaled, scaled);
    }
}