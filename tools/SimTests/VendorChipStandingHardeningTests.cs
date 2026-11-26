using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class VendorChipStandingHardeningTests
{
    [Fact]
    public void First_training_tape_out_drops_standing_once()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        int before = g.State.Vendor.Standing(g.Id).Standing;
        ChipDesign d = ChipHardeningSupport.InjectTapingOut(g, g.Me);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - g.C.Vendor.TrainingTapeOutDrop, g.State.Vendor.Standing(g.Id).Standing);
        Assert.True(g.State.Vendor.Standing(g.Id).TrainingChipSeen);

        int mid = g.State.Vendor.Standing(g.Id).Standing;
        ChipHardeningSupport.InjectTapingOut(g, g.Me);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(mid, g.State.Vendor.Standing(g.Id).Standing);
    }

    [Fact]
    public void Inference_tape_out_never_sets_training_seen_or_drops()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        int before = g.State.Vendor.Standing(g.Id).Standing;
        var d = new ChipDesign
        {
            Id = g.State.TakeId(), Target = ChipHardeningSupport.Inference, NodeId = "n28",
            Status = ChipDesignStatus.TapingOut, StageEndsTick = g.State.Tick,
            PointsDone = 1, PointsRequired = 1, DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air",
        };
        g.Me.ChipDesigns.Add(d);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.State.Vendor.Standing(g.Id).Standing);
        Assert.False(g.State.Vendor.Standing(g.Id).TrainingChipSeen);
    }

    [Fact]
    public void First_chip_listing_drops_standing_once()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int before = g.State.Vendor.Standing(g.Id).Standing;
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 10));
        Assert.Equal(before - g.C.Vendor.ListingDrop, g.State.Vendor.Standing(g.Id).Standing);
        Assert.True(g.State.Vendor.Standing(g.Id).ListingSeen);
        g.Do(Command.SetChipListing(g.Id, d.Id, 6000, 10));
        Assert.Equal(before - g.C.Vendor.ListingDrop, g.State.Vendor.Standing(g.Id).Standing);
    }

    [Fact]
    public void Each_delivery_drops_standing_for_the_design_owner()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 900);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 20));
        ChipOrder order = g.Me.ChipOrders.Single();
        int before = g.State.Vendor.Standing(g.Id).Standing;
        g.State.Tick = order.DeliveryTick;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - g.C.Vendor.DeliveryDrop, g.State.Vendor.Standing(g.Id).Standing);
    }

    [Fact]
    public void Second_delivery_drops_standing_again()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, yield: 900);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 10));
        int tick1 = g.Me.ChipOrders[0].DeliveryTick;
        g.State.Tick = tick1;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        int mid = g.State.Vendor.Standing(g.Id).Standing;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 10));
        g.State.Tick = g.Me.ChipOrders[0].DeliveryTick;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(mid - g.C.Vendor.DeliveryDrop, g.State.Vendor.Standing(g.Id).Standing);
    }

    [Fact]
    public void Delivery_drop_targets_design_lab_not_buyer_when_ids_differ()
    {
        Game g = ChipHardeningSupport.Lab(2013, rivals: true);
        LabState designer = g.Me;
        LabState buyer = g.State.Labs[1];
        ChipDesign d = ChipHardeningSupport.InjectReady(g, designer, yield: 900);
        int buyerHall = ChipHardeningSupport.Hall(g, buyer);
        var order = new ChipOrder
        {
            DesignId = d.Id, DesignLabId = designer.Id, HoldingId = buyerHall, Count = 5,
            DeliveryTick = g.State.Tick, NodeId = d.NodeId,
        };
        buyer.ChipOrders.Add(order);
        int designerBefore = g.State.Vendor.Standing(designer.Id).Standing;
        ChipSystem.RunWeek(g.C, g.State, buyer);
        Assert.Equal(designerBefore - g.C.Vendor.DeliveryDrop, g.State.Vendor.Standing(designer.Id).Standing);
    }

    [Fact]
    public void Standing_flags_survive_save_load()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 1000, 5));
        var seen = g.State.Vendor.Standing(g.Id);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        VendorStanding after = loaded.State.Vendor.Standing(g.Id);
        Assert.Equal(seen.ListingSeen, after.ListingSeen);
        Assert.Equal(seen.TrainingChipSeen, after.TrainingChipSeen);
        Assert.Equal(seen.Standing, after.Standing);
    }

    [Fact]
    public void Training_tape_out_records_chip_ready_with_first_pick_flag()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        g.Me.ChipDesigns.Clear();
        ChipDesign d = ChipHardeningSupport.InjectTapingOut(g, g.Me);
        d.Version = 1;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        GameEvent ready = g.State.Events.Single(e => e.Kind == EventKind.ChipReady);
        Assert.Equal(1, ready.Pick);
    }

    [Fact]
    public void Vendor_from_chips_accumulates_on_training_penalty()
    {
        Game g = ChipHardeningSupport.Lab(2013);
        VendorStanding standing = g.State.Vendor.Standing(g.Id);
        int fromBefore = standing.FromChips;
        ChipHardeningSupport.InjectTapingOut(g, g.Me);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(fromBefore - g.C.Vendor.TrainingTapeOutDrop, standing.FromChips);
    }
}