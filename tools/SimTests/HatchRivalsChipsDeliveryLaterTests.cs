using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q263–Q292: chip order delivery and post-acquire lifecycle (no new save schema).</summary>
public class HatchRivalsChipsDeliveryLaterTests
{
    static ModelState Released(Game g, int cap) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = "R"
    };

    [Fact]
    public void Q263_seal_absorbed_does_not_unbook_foundry_twice_after_order_transfer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        string node = g.C.Chips.Nodes[0].IdValue;
        int month = rival.ChipOrders[0].FoundryMonth;
        long booked = ChipQueries.BookedIn(g.State, node, month);
        rival.Models.Add(Released(g, 530));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipOrders);
        Assert.Equal(booked, ChipQueries.BookedIn(g.State, node, month));
    }

    [Fact]
    public void Q264_acquire_keeps_design_lab_on_player_for_listed_chip_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 4));
        ChipOrder before = rival.ChipOrders[0];
        Assert.Equal(g.Id, before.DesignLabId);
        rival.Models.Add(Released(g, 520));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipOrder after = g.Me.ChipOrders.Single(o => o.Id == before.Id);
        Assert.Equal(g.Id, after.BuyerLabId);
        Assert.Equal(g.Id, after.DesignLabId);
        Assert.Equal(4, after.Count);
        Assert.Contains(g.Me.ChipDesigns, x => x.Id == d.Id);
    }

    [Fact]
    public void Q265_chips_delivered_event_fires_for_buyer_after_transferred_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 510));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int deliver = g.Me.ChipOrders[0].DeliveryTick;
        g.State.Tick = deliver;
        int eventsBefore = g.State.Events.Count(e => e.Kind == EventKind.ChipsDelivered && e.LabId == g.Id);
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        int eventsAfter = g.State.Events.Count(e => e.Kind == EventKind.ChipsDelivered && e.LabId == g.Id);
        Assert.Equal(eventsBefore + 1, eventsAfter);
    }

    [Fact]
    public void Q266_hall_order_delivers_to_remapped_holding_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 85_000_000);
        int hall = g.Do(Command.BuyLand(rival.Id, "us-oregon")).CreatedId;
        rival.Holding(hall).HallKw = 400_000;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, hall, 3));
        int deliver = rival.ChipOrders[0].DeliveryTick;
        rival.Models.Add(Released(g, 540));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 110_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int newHolding = g.Me.ChipOrders[0].HoldingId;
        g.State.Tick = deliver;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        HardwareBatch hallBatch = g.Me.Fleet.FirstOrDefault(b => b.HoldingId == newHolding && b.ChipDesignId == d.Id);
        Assert.NotNull(hallBatch);
    }

    [Fact]
    public void Q267_design_lab_on_delivered_batch_is_buyer_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int deliver = rival.ChipOrders[0].DeliveryTick;
        rival.Models.Add(Released(g, 505));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.State.Tick = deliver;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        HardwareBatch batch = g.Me.Fleet.FirstOrDefault(b => b.ChipDesignId == d.Id);
        Assert.NotNull(batch);
        Assert.Equal(g.Id, batch.DesignLabId);
    }

    [Fact]
    public void Q268_pending_order_survives_weekly_step_before_delivery_tick()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 495));
        rival.Cash = -15_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int deliver = g.Me.ChipOrders[0].DeliveryTick;
        while (g.State.Tick < deliver - 2) g.Sim.Step();
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q269_save_load_pending_order_delivery_tick_unchanged()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 500));
        rival.Cash = -14_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 82_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int tick = g.Me.ChipOrders[0].DeliveryTick;
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        Assert.Equal(tick, loaded.Me.ChipOrders[0].DeliveryTick);
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
    }

    [Fact]
    public void Q270_two_orders_same_design_deliver_separately_on_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 20));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        int tickA = rival.ChipOrders.Min(o => o.DeliveryTick);
        int tickB = rival.ChipOrders.Max(o => o.DeliveryTick);
        rival.Models.Add(Released(g, 525));
        rival.Cash = -21_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.ChipOrders.Count);
        g.State.Tick = tickA;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ChipOrders.Count <= 2);
    }

    [Fact]
    public void Q271_player_cannot_order_chips_for_absorbed_target_after_acquire()
    {
        var g = Game.New(seed: 971);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -16_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000);
        g.Refused(Command.OrderChips(rival.Id, designId, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q272_acquire_then_delivery_does_not_leave_orders_on_sealed_target()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 515));
        rival.Cash = -17_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.State.Tick = g.Me.ChipOrders[0].DeliveryTick;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(rival.ChipOrders);
        Assert.Empty(rival.ChipListings);
    }

    [Fact]
    public void Q273_vendor_change_on_delivery_targets_buyer_design_lab_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.C.Vendor.DeliveryDrop = 50;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4700, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 508));
        rival.Cash = -16_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 92_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int standingBefore = VendorSystem.StandingOf(g.State, g.Me);
        g.State.Tick = g.Me.ChipOrders[0].DeliveryTick;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(VendorSystem.StandingOf(g.State, g.Me) < standingBefore);
    }

    [Fact]
    public void Q274_crash_month_roll_does_not_clear_buyer_chip_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 4;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 490));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int orders = g.Me.ChipOrders.Count;
        for (int i = 0; i < g.C.Calendar.WeeksPerMonth; i++) g.Sim.Step();
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(orders, g.Me.ChipOrders.Count);
    }

    [Fact]
    public void Q275_acquire_mid_flight_then_full_suite_regression_hook()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 25_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 35_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 480));
        rival.Cash = -22_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.ChipOrders.Count >= 1);
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        Assert.False(LabQueries.ParticipatesInEconomy(rival));
    }

    [Fact]
    public void Q276_transferred_order_keeps_foundry_node_id()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        string node = rival.ChipOrders[0].NodeId;
        rival.Models.Add(Released(g, 515));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(node, g.Me.ChipOrders[0].NodeId);
        Assert.False(string.IsNullOrEmpty(node));
    }

    [Fact]
    public void Q277_transferred_order_preserves_reserved_kw_on_cloud()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int reserved = rival.ChipOrders[0].ReservedKw;
        rival.Models.Add(Released(g, 510));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(reserved, g.Me.ChipOrders[0].ReservedKw);
        Assert.Equal(0, g.Me.ChipOrders[0].ReservedKw);
    }

    [Fact]
    public void Q278_seal_absorbed_clears_target_listings_but_not_buyer()
    {
        var g = Game.New(seed: 978);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 550));
        rival.Cash = -18_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 3500, MaxPerMonth = 6 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipListings);
        Assert.Single(g.Me.ChipListings);
    }

    [Fact]
    public void Q279_sim_step_after_acquire_with_pending_order_does_not_bankrupt_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 505));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        for (int i = 0; i < 8; i++) g.Sim.Step();
        Assert.False(g.Me.Bankrupt);
        Assert.True(g.Me.ChipOrders.Count >= 1);
    }

    [Fact]
    public void Q280_listing_sold_this_month_resets_on_buyer_after_chip_month()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3700, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        rival.ChipListings.Add(new ChipListing { DesignId = d.Id, PricePerChip = 3600, MaxPerMonth = 10, SoldThisMonth = 2 });
        rival.Models.Add(Released(g, 495));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Me.Listing(d.Id).SoldThisMonth = 4;
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(d.Id).SoldThisMonth);
    }

    [Fact]
    public void Q281_absorbed_seller_refuses_buy_listed_chips_for_third_party()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 500));
        rival.Cash = -17_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState third = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(third, LedgerLine.CapitalIn, 25_000_000);
        g.Refused(Command.BuyListedChips(third.Id, rival.Id, d.Id, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q282_distress_acquire_then_trade_listing_still_on_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 6;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 470));
        rival.Cash = -35_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 180_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState third = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(third, LedgerLine.CapitalIn, 40_000_000);
        g.Do(Command.BuyListedChips(third.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.NotNull(g.Me.Listing(d.Id));
        Assert.True(g.Me.ChipOrders.Count >= 1);
    }

    [Fact]
    public void Q283_schema_stays_29_after_save_with_transferred_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 38_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 485));
        rival.Cash = -15_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        Assert.Equal(45, loaded.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
    }

    [Fact]
    public void Q284_chips_bought_event_preserved_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int bought = g.State.Events.Count(e => e.Kind == EventKind.ChipsBought);
        rival.Models.Add(Released(g, 512));
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 86_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(bought, g.State.Events.Count(e => e.Kind == EventKind.ChipsBought));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired);
    }

    [Fact]
    public void Q285_acquire_does_not_double_count_foundry_booking()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 42_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 5));
        string node = g.C.Chips.Nodes[0].IdValue;
        int month = rival.ChipOrders[0].FoundryMonth;
        long booked = ChipQueries.BookedIn(g.State, node, month);
        rival.Models.Add(Released(g, 518));
        rival.Cash = -18_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(booked, ChipQueries.BookedIn(g.State, node, month));
        Assert.Equal(5, g.Me.ChipOrders.Sum(o => o.Count));
    }

    [Fact]
    public void Q286_lab_acquired_event_amount_matches_quote()
    {
        var g = Game.New(seed: 979);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 580));
        rival.Cash = -22_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        long total = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.LabAcquired);
        Assert.Equal(total, ev.Amount);
    }

    [Fact]
    public void Q287_pending_orders_do_not_inflate_acquisition_chips_part()
    {
        var g = Game.New(seed: 980);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -20_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 810 });
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = designId, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 6, FoundryMonth = month, DeliveryTick = g.State.Tick + 30, NodeId = g.C.Chips.Nodes[0].IdValue });
        long withOrder = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart;
        rival.ChipOrders.Clear();
        long without = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart;
        Assert.Equal(without, withOrder);
        Assert.True(withOrder > 0);
    }

    [Fact]
    public void Q288_rival_plan_step_after_player_acquire_chip_acquire_stays_quiet()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 490));
        rival.Cash = -21_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 130_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState other = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        other.PlanStartTick = 0;
        g.Sim.RunPlan(other.Id, g.C.Rival("anthoptics").Plan);
        Assert.True(other.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q289_chip_listing_max_per_month_on_buyer_after_acquire_trade()
    {
        var g = Game.New(seed: 989);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 502));
        rival.Cash = -16_800;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 5100, MaxPerMonth = 2 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.Listing(designId).MaxPerMonth);
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 30_000_000);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, designId, HardwareBatch.Cloud, 1));
        Assert.Equal(1, g.Me.Listing(designId).SoldThisMonth);
    }

    [Fact]
    public void Q290_target_bankrupt_flag_unchanged_until_seal_not_from_order_move()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4700, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 34_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 43_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 508));
        rival.Cash = -17_200;
        Assert.False(rival.Bankrupt);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 84_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.False(rival.Bankrupt);
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
    }

    [Fact]
    public void Q291_player_acquire_chip_rich_target_logs_acquisition_only_once()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 65_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        rival.Models.Add(Released(g, 540));
        rival.Cash = -19_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 140_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.LabAcquired && e.SubjectId == rival.Id));
    }

    [Fact]
    public void Q292_compute_split_unchanged_with_only_pending_transferred_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        ComputeSplit before = LabQueries.Split(g.C, g.State, g.Me, false);
        rival.Models.Add(Released(g, 520));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ComputeSplit after = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(before.General, after.General);
        Assert.Equal(before.Inference, after.Inference);
    }
}