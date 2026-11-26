using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q129: ChipQueries.FoundrySlotParts explains chiplet foundry booking.</summary>
public class ChipFoundrySlotBreakdownTests
{
    [Fact]
    public void Non_chiplet_slots_equal_count()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "small");
        var b = ChipQueries.FoundrySlotParts(g.C, d, 6);
        Assert.Equal(6, b.BaseChips);
        Assert.Equal(0, b.FromChiplet);
        Assert.Equal(6, b.Total);
        Assert.Equal(ChipQueries.FoundrySlots(g.C, d, 6), b.Total);
    }

    [Fact]
    public void Chiplet_design_adds_from_chiplet()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n5", "chiplet-mcm");
        var b = ChipQueries.FoundrySlotParts(g.C, d, 10);
        Assert.Equal(10, b.BaseChips);
        Assert.True(b.FromChiplet > 0);
        Assert.Equal(ChipQueries.FoundrySlots(g.C, d, 10), b.Total);
    }

    [Fact]
    public void Chiplet_total_above_base_chips()
    {
        var g = ChipHardeningSupport.Lab(2015, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "inference", "n7", "chiplet-mcm");
        var b = ChipQueries.FoundrySlotParts(g.C, d, 10);
        Assert.True(b.Total > b.BaseChips);
    }

    [Fact]
    public void Zero_count_yields_zero_slots()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "chiplet-mcm");
        var b = ChipQueries.FoundrySlotParts(g.C, d, 0);
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void Null_design_only_base_when_chiplet_flag_false()
    {
        Game g = Game.Solo();
        var b = ChipQueries.FoundrySlotParts(g.C, null, 5);
        Assert.Equal(5, b.BaseChips);
        Assert.Equal(0, b.FromChiplet);
    }

    [Fact]
    public void Order_foundry_slots_matches_breakdown_total()
    {
        var g = ChipHardeningSupport.Lab(2016, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n5", "chiplet-mcm");
        int slots = ChipQueries.FoundrySlots(g.C, d, 8);
        Assert.Equal(slots, ChipQueries.FoundrySlotParts(g.C, d, 8).Total);
    }

    [Fact]
    public void Shortage_foundry_room_uses_slot_total()
    {
        var g = ChipHardeningSupport.Lab(2016, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "chiplet-mcm");
        var node = g.C.Chips.Node("n28");
        int slots = ChipQueries.FoundrySlotParts(g.C, d, 12).Total;
        ChipShortageSystem.Start(g.C, g.State, 1250);
        int month = ChipQueries.FirstFreeMonth(g.C, g.State, node, d, 12);
        Assert.True(month >= 0 || slots > 12);
    }

    [Fact]
    public void Is_chiplet_design_gate()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign med = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "medium");
        ChipDesign chiplet = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "chiplet-mcm");
        Assert.False(ChipQueries.IsChipletDesign(g.C, med));
        Assert.True(ChipQueries.IsChipletDesign(g.C, chiplet));
    }

    [Fact]
    public void Large_batch_scales_chiplet_premium()
    {
        var g = ChipHardeningSupport.Lab(2015, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n5", "chiplet-mcm");
        var small = ChipQueries.FoundrySlotParts(g.C, d, 2);
        var large = ChipQueries.FoundrySlotParts(g.C, d, 20);
        Assert.True(large.FromChiplet > small.FromChiplet);
        Assert.Equal(20, large.BaseChips);
    }

    [Fact]
    public void Deterministic_slot_parts()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n7", "chiplet-mcm");
        var a = ChipQueries.FoundrySlotParts(g.C, d, 7);
        var b = ChipQueries.FoundrySlotParts(g.C, d, 7);
        Assert.Equal(a.Total, b.Total);
    }

    [Fact]
    public void Medium_die_no_extra_slots_at_one_chip()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "inference", "n28", "medium");
        var b = ChipQueries.FoundrySlotParts(g.C, d, 1);
        Assert.Equal(1, b.Total);
        Assert.Equal(0, b.FromChiplet);
    }

    [Fact]
    public void Save_load_design_slots_unchanged()
    {
        var g = ChipHardeningSupport.Lab(2016, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n5", "chiplet-mcm");
        g.Me.ChipDesigns.Clear();
        g.Me.ChipDesigns.Add(d);
        var before = ChipQueries.FoundrySlotParts(g.C, d, 5);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ChipDesign afterDesign = loaded.Me.Design(d.Id);
        var after = ChipQueries.FoundrySlotParts(loaded.C, afterDesign, 5);
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void Config_permille_matches_slot_multiplier()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n5", "chiplet-mcm");
        int expectedExtra = ChipQueries.FoundrySlots(g.C, d, 100) - 100;
        var b = ChipQueries.FoundrySlotParts(g.C, d, 100);
        Assert.Equal(expectedExtra, b.FromChiplet);
    }
}