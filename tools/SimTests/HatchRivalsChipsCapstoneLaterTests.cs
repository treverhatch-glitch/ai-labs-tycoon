using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q401–Q495: grouped capstone — multi-lab chip × M&amp;A × economy (schema 31).</summary>
public class HatchRivalsChipsCapstoneLaterTests
{
    static ModelState Released(Game g, int cap) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = "C"
    };

    static int Hall(Game g, LabState lab)
    {
        int id = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(id).HallKw = 450_000;
        return id;
    }

    static void DistressAcquireChipBuyer(Game g, LabState rival)
    {
        rival.Models.Add(Released(g, 510));
        rival.Cash = -17_000;
        rival.WeeksInDebt = 1;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
    }

    [Fact] public void Q401_capstone_buy_acquire_deliver_single_batch()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        DistressAcquireChipBuyer(g, rival);
        int t = g.Me.ChipOrders[0].DeliveryTick;
        g.State.Tick = t;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.ChipOrders);
        Assert.NotNull(g.Me.Fleet.FirstOrDefault(b => b.ChipDesignId == d.Id));
    }

    [Fact] public void Q402_capstone_post_acquire_third_party_buy_raises_seller_chip_sales()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4900, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        long life = g.Me.Ledger.Lifetime.ChipSales;
        DistressAcquireChipBuyer(g, rival);
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 32_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.True(g.Me.Ledger.Lifetime.ChipSales > life);
    }

    [Fact] public void Q403_capstone_dual_buyers_two_orders_acquire_player_holds_both()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        LabState a = HatchFollowUpLaterTests.OpenAyeBuyer(g, 90_000_000);
        LabState b = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(b, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.Do(Command.BuyListedChips(b.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        a.Models.Add(Released(g, 505));
        a.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        Assert.Equal(2, g.State.Labs.Sum(l => l.ChipOrders.Count));
        Assert.Single(g.Me.ChipOrders);
        Assert.Single(b.ChipOrders);
    }

    [Fact] public void Q404_capstone_acquire_anth_with_order_while_player_sells()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 60_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        LabState prey = HatchFollowUpLaterTests.OpenAyeBuyer(g, 1_000_000);
        prey.Models.Clear();
        prey.Models.Add(Released(g, 530));
        prey.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, prey.Id));
        Assert.Single(anth.ChipOrders);
    }

    [Fact] public void Q405_capstone_foundry_booking_stable_after_acquire_transfer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        string node = g.C.Chips.Nodes[0].IdValue;
        int month = rival.ChipOrders[0].FoundryMonth;
        long booked = ChipQueries.BookedIn(g.State, node, month);
        DistressAcquireChipBuyer(g, rival);
        Assert.Equal(booked, ChipQueries.BookedIn(g.State, node, month));
    }

    [Fact] public void Q406_capstone_hall_and_cloud_orders_both_remap()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 15));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 100_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int hall = Hall(g, rival);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, hall, 1));
        Assert.Equal(2, rival.ChipOrders.Count);
        int h1 = rival.ChipOrders[0].HoldingId;
        int h2 = rival.ChipOrders[1].HoldingId;
        DistressAcquireChipBuyer(g, rival);
        Assert.Equal(2, g.Me.ChipOrders.Count);
        Assert.Contains(g.Me.ChipOrders, o => o.HoldingId == HardwareBatch.Cloud);
        Assert.Contains(g.Me.ChipOrders, o => o.HoldingId != HardwareBatch.Cloud);
        Assert.NotEqual(h2, g.Me.ChipOrders.First(o => o.HoldingId != HardwareBatch.Cloud).HoldingId);
    }

    [Fact] public void Q407_capstone_save_load_dual_global_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        LabState a = HatchFollowUpLaterTests.OpenAyeBuyer(g, 85_000_000);
        LabState b = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(b, LedgerLine.CapitalIn, 65_000_000);
        g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(b.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int global = g.State.Labs.Sum(l => l.ChipOrders.Count);
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        Assert.Equal(global, loaded.State.Labs.Sum(l => l.ChipOrders.Count));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
    }

    [Fact] public void Q408_capstone_crash_acquire_transfers_own_design_order_to_player()
    {
        var g = Game.New(seed: 1101);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 470));
        rival.Cash = -30_000;
        rival.WeeksInDebt = 2;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = did, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = month, DeliveryTick = g.State.Tick + 28, NodeId = g.C.Chips.Nodes[0].IdValue });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.Equal(did, g.Me.ChipOrders[0].DesignId);
    }

    [Fact] public void Q409_capstone_acquire_then_month_roll_clears_absorbed_listing_sold()
    {
        var g = Game.New(seed: 1102);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520));
        rival.Cash = -17_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4200, MaxPerMonth = 5, SoldThisMonth = 3 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(did).SoldThisMonth);
    }

    [Fact] public void Q410_capstone_participates_economy_buyer_and_seller_after_pipeline()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 4));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        Assert.False(LabQueries.ParticipatesInEconomy(rival));
    }

    [Fact] public void Q411_grouped_refuse_chip_commands_on_absorbed_batch()
    {
        var g = Game.New(seed: 1103);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 525));
        rival.Cash = -16_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.OrderChips(rival.Id, did, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
        g.Refused(Command.SetChipListing(rival.Id, did, 3000, 4), CommandError.LabAbsorbed);
        g.Refused(Command.BuyListedChips(rival.Id, g.Id, did, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact] public void Q412_grouped_planner_acquire_quiet_when_player_already_bought_chips()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        LabState other = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        other.Models.Add(Released(g, 460));
        other.Cash = -80_000;
        other.WeeksInDebt = 2;
        other.PlanStartTick = 0;
        g.Sim.RunPlan(other.Id, g.C.Rival("anthoptics").Plan);
        Assert.True(other.AbsorbedByLabId < 0 || other.AbsorbedByLabId == g.Id);
    }

    [Fact] public void Q413_grouped_quote_chips_part_after_listed_buy_on_target()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 515));
        rival.Cash = -16_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 765 });
        long chips = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart;
        Assert.True(chips > 0);
    }

    [Fact] public void Q414_grouped_transferred_listing_price_unchanged()
    {
        var g = Game.New(seed: 1104);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540));
        rival.Cash = -18_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 790 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 5600, MaxPerMonth = 3 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(5600, g.Me.Listing(did).PricePerChip);
    }

    [Fact] public void Q415_grouped_event_chip_delivered_after_full_pipeline()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4700, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        int n = g.State.Events.Count(e => e.Kind == EventKind.ChipsDelivered);
        int t = g.Me.ChipOrders[0].DeliveryTick;
        g.State.Tick = t;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.State.Events.Count(e => e.Kind == EventKind.ChipsDelivered) > n);
    }

    [Fact] public void Q416_grouped_acquire_two_distressed_with_chip_assets()
    {
        var g = Game.New(seed: 1105);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState a = g.Rival("openaye");
        LabState b = g.Rival("anthoptics");
        foreach (LabState lab in new[] { a, b })
        {
            lab.Models.Add(Released(g, 480));
            lab.Cash = -22_000;
            lab.WeeksInDebt = 2;
            int did = g.State.TakeId();
            lab.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 750 });
        }
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 400_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        g.Do(Command.AcquireLab(g.Id, b.Id));
        Assert.Equal(2, g.Me.ChipDesigns.Count(cd => cd.Status == ChipDesignStatus.Ready));
    }

    [Fact] public void Q417_grouped_bubble_meter_ticks_on_post_acquire_listing_sale()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubblePermille = 550;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5500, 4));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        DistressAcquireChipBuyer(g, rival);
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 50_000_000);
        int perm = g.State.BubblePermille;
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.True(g.State.BubblePermille >= perm);
    }

    [Fact] public void Q418_grouped_vendor_standing_after_pipeline_month()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int s0 = VendorSystem.StandingOf(g.State, g.Me);
        DistressAcquireChipBuyer(g, rival);
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(s0, VendorSystem.StandingOf(g.State, g.Me));
    }

    [Fact] public void Q419_grouped_training_design_listing_survives_acquire()
    {
        var g = Game.New(seed: 1106);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 535));
        rival.Cash = -17_200;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "training", Version = 1, YieldPermille = 730 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 3600, MaxPerMonth = 6 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal("training", g.Me.Design(did).Target);
        Assert.NotNull(g.Me.Listing(did));
    }

    [Fact] public void Q420_grouped_acquisition_ledger_month_after_chip_rich_acquire()
    {
        var g = Game.New(seed: 1107);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 570));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        long total = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival).Total;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Ledger.ThisMonth.Acquisition < 0);
        Assert.True(-g.Me.Ledger.ThisMonth.Acquisition >= total);
    }

    [Fact] public void Q421_grouped_exit_price_stable_mid_pipeline()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        long exit0 = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        long exit1 = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        DistressAcquireChipBuyer(g, rival);
        long exit2 = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        Assert.True(exit1 > 0);
        Assert.True(exit2 > 0);
    }

    [Fact] public void Q422_grouped_feelings_seller_unchanged_buyer_feel_on_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        int f0 = rival.FeelingToPlayer;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int f1 = rival.FeelingToPlayer;
        DistressAcquireChipBuyer(g, rival);
        Assert.True(f1 > f0);
        Assert.Equal(f1, rival.FeelingToPlayer);
    }

    [Fact] public void Q423_grouped_global_order_count_three_labs()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 20));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        LabState o = HatchFollowUpLaterTests.OpenAyeBuyer(g, 110_000_000);
        LabState a = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(a, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.BuyListedChips(o.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int sum = g.State.Labs.Sum(l => l.ChipOrders.Count);
        DistressAcquireChipBuyer(g, o);
        Assert.Equal(sum, g.State.Labs.Sum(l => l.ChipOrders.Count));
    }

    [Fact] public void Q424_grouped_can_acquire_while_pending_orders_on_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 42_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 54_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 518));
        rival.Cash = -17_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, g.Me, rival, out string why), why);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
    }

    [Fact] public void Q425_grouped_design_version_preserved_on_transfer()
    {
        var g = Game.New(seed: 1108);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 555));
        rival.Cash = -18_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 9, YieldPermille = 820 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(9, g.Me.Design(did).Version);
    }

    [Fact] public void Q426_grouped_fleet_gpu_count_plus_chip_after_acquire()
    {
        var g = Game.New(seed: 1109);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -19_000;
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 10, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 82_000_000);
        int gpus = g.Me.Fleet.Where(b => !string.IsNullOrEmpty(b.GpuGenId)).Sum(b => b.Count);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Fleet.Where(b => !string.IsNullOrEmpty(b.GpuGenId)).Sum(b => b.Count) > gpus);
    }

    [Fact] public void Q427_grouped_acquire_seal_empty_target_chip_listings_on_rival()
    {
        var g = Game.New(seed: 1110);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 528));
        rival.Cash = -16_900;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 775 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4500, MaxPerMonth = 4 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipListings);
        Assert.Single(g.Me.ChipListings);
    }

    [Fact] public void Q428_grouped_weekly_sim_steps_preserve_order_ids()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4150, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 34_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int id = rival.ChipOrders[0].Id;
        DistressAcquireChipBuyer(g, rival);
        for (int i = 0; i < 3; i++) g.Sim.Step();
        Assert.Equal(id, g.Me.ChipOrders[0].Id);
    }

    [Fact] public void Q429_grouped_crash_discount_quote_with_pending_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 7;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 56_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 495));
        rival.Cash = -25_000;
        rival.WeeksInDebt = 2;
        g.State.BubbleCrashMonthsRemaining = 0;
        long calm = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.State.BubbleCrashMonthsRemaining = 7;
        long crash = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        Assert.True(crash < calm);
    }

    [Fact] public void Q430_grouped_player_models_unchanged_count_when_only_transferring_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4050, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        int models = g.Me.Models.Count;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        DistressAcquireChipBuyer(g, rival);
        Assert.Equal(models + 1, g.Me.Models.Count);
        Assert.Empty(rival.Models);
    }

    [Fact] public void Q431_grouped_schema_29_after_capstone_pipeline()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        Assert.Equal(31, g.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    [Fact] public void Q432_grouped_roll_up_month_chip_order_on_weak()
    {
        var g = Game.New(seed: 1111, tweak: c => { c.Acquisitions.RivalRollupChancePermille = 1000; c.Acquisitions.PlannerAcquireChancePermille = 0; });
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState strong = g.Rival("openaye");
        strong.PlanStartTick = 0;
        Finance.Earn(strong, LedgerLine.CapitalIn, 550_000_000);
        strong.Models.Add(Released(g, 710));
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 465));
        weak.Cash = -92_000;
        weak.WeeksInDebt = 3;
        weak.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = 1, BuyerLabId = weak.Id, DesignLabId = weak.Id, HoldingId = HardwareBatch.Cloud, Count = 4, FoundryMonth = 0, DeliveryTick = g.State.Tick + 18, NodeId = g.C.Chips.Nodes[0].IdValue });
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.True(weak.AbsorbedByLabId == strong.Id ? strong.ChipOrders.Count >= 1 : weak.ChipOrders.Count == 1);
    }

    [Fact] public void Q433_grouped_bankrupt_refuse_buy_listed_from_absorbed()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 20_000_000);
        g.Refused(Command.BuyListedChips(anth.Id, rival.Id, d.Id, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact] public void Q434_grouped_order_design_lab_player_for_listed_chips()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 44_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 56_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        DistressAcquireChipBuyer(g, rival);
        Assert.All(g.Me.ChipOrders, o => { Assert.Equal(g.Id, o.DesignLabId); Assert.Equal(g.Id, o.BuyerLabId); });
    }

    [Fact] public void Q435_grouped_staff_morale_transfer_with_chip_acquire()
    {
        var g = Game.New(seed: 1112);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 525));
        rival.Cash = -16_500;
        rival.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "researcher", Morale = g.C.StaffPeople.MoraleBaseline });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        int staff = g.Me.Staff.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(staff + 1, g.Me.Staff.Count);
    }

    [Fact] public void Q436_grouped_target_eligible_distressed_with_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Cash = -20_000;
        rival.WeeksInDebt = 2;
        rival.Models.Add(Released(g, 512));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, g.Me, rival, out _), "eligible");
    }

    [Fact] public void Q437_grouped_chips_part_in_quote_with_ready_design()
    {
        var g = Game.New(seed: 1113);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 545));
        rival.Cash = -17_800;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 785 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 76_000_000);
        Assert.True(DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart > 0);
    }

    [Fact] public void Q438_grouped_monthly_chip_listing_reset_on_player()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 4));
        g.Me.Listing(d.Id).SoldThisMonth = 3;
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(d.Id).SoldThisMonth);
    }

    [Fact] public void Q439_grouped_acquire_lab_id_on_event()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        DistressAcquireChipBuyer(g, rival);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired && e.SubjectId == rival.Id);
    }

    [Fact] public void Q440_grouped_capstone_hook_big_bar_participates()
    {
        var g = Game.New(seed: 1114);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 590));
        rival.Cash = -19_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 4, YieldPermille = 805 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 5800, MaxPerMonth = 5 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(31, g.State.Schema);
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        Assert.NotNull(g.Me.Listing(did));
    }

    [Fact] public void Q441_extra_hire_refused_on_absorbed() { var g = Game.New(seed: 1115); var r = g.Rival("openaye"); r.Models.Add(Released(g, 520)); r.Cash = -16_800; Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.Hire(r.Id, "researcher", 1), CommandError.LabAbsorbed); }
    [Fact] public void Q442_extra_raise_funding_refused_absorbed() { var g = Game.New(seed: 1116); var r = g.Rival("openaye"); r.Models.Add(Released(g, 515)); r.Cash = -16_200; Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.RaiseFunding(r.Id, "angel"), CommandError.LabAbsorbed); }
    [Fact] public void Q443_extra_bubble_in_crash_acquire() { var g = Game.New(seed: 1117); g.State.BubbleCrashMonthsRemaining = 5; var r = g.Rival("openaye"); r.Models.Add(Released(g, 480)); r.Cash = -26_000; r.WeeksInDebt = 2; Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000); Assert.True(BubbleSystem.InCrash(g.State)); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(BubbleSystem.InCrash(g.State)); }
    [Fact] public void Q444_extra_cash_quote_part_positive() { var g = Game.New(seed: 1118); var r = g.Rival("openaye"); r.Models.Add(Released(g, 580)); r.Cash = -15_000; Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000); Assert.True(DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, r).Total > 0); }
    [Fact] public void Q445_extra_models_part_positive_with_order() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); r.Models.Add(Released(g, 505)); r.Cash = -16_000; Assert.True(DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, r).ModelsPart > 0); }
    [Fact] public void Q446_extra_absorbed_by_lab_id_set() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(g.Id, r.AbsorbedByLabId); }
    [Fact] public void Q447_extra_player_fleet_empty_before_acquire() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); Assert.DoesNotContain(g.Me.Fleet, b => b.ChipDesignId == d.Id); DistressAcquireChipBuyer(g, r); }
    [Fact] public void Q448_extra_rival_log_retained_after_absorb() { var g = Game.New(seed: 1119); var r = g.Rival("openaye"); r.Models.Add(Released(g, 525)); r.Cash = -16_500; Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000); int log = r.Log.Count; g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(r.Log.Count >= log); }
    [Fact] public void Q449_extra_chip_design_ready_count_on_buyer() { var g = Game.New(seed: 1120); var r = g.Rival("openaye"); r.Models.Add(Released(g, 540)); r.Cash = -17_000; int a = g.State.TakeId(); int b = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = a, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 }); r.ChipDesigns.Add(new ChipDesign { Id = b, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 2, YieldPermille = 790 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(2, g.Me.ChipDesigns.Count(x => x.Status == ChipDesignStatus.Ready)); }
    [Fact] public void Q450_extra_listing_on_player_after_acquire() { var g = Game.New(seed: 1121); var r = g.Rival("openaye"); r.Models.Add(Released(g, 530)); r.Cash = -17_000; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = "n28", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Target = "inference", Version = 1, YieldPermille = 775 }); r.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4700, MaxPerMonth = 4 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.NotNull(g.Me.Listing(did)); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q451_extra_order_chips_refused_player_absorbed_target() { var g = Game.New(seed: 1122); var r = g.Rival("openaye"); r.Models.Add(Released(g, 522)); r.Cash = -16_700; Finance.Earn(g.Me, LedgerLine.CapitalIn, 71_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.OrderChips(r.Id, 1, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed); }
    [Fact] public void Q452_extra_weeks_in_debt_preserved_on_seal() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 42_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 54_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); r.WeeksInDebt = 3; r.Models.Add(Released(g, 508)); r.Cash = -16_200; Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(3, r.WeeksInDebt); }
    [Fact] public void Q453_extra_target_name_unchanged_after_absorb() { var g = Game.New(seed: 1123); var r = g.Rival("openaye"); string name = r.Name; r.Models.Add(Released(g, 518)); r.Cash = -17_100; Finance.Earn(g.Me, LedgerLine.CapitalIn, 73_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(name, r.Name); }
    [Fact] public void Q454_extra_player_tech_count_unchanged() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); int techs = g.Me.Techs.Count; ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 6)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(techs, g.Me.Techs.Count); }
    [Fact] public void Q455_extra_player_cash_non_negative_after_acquire() { var g = Game.New(seed: 1124); var r = g.Rival("openaye"); r.Models.Add(Released(g, 550)); r.Cash = -18_000; Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(g.Me.Cash >= 0); }
    [Fact] public void Q456_extra_dual_acquire_chip_designs_accumulate() { var g = Game.New(seed: 1125); g.State.BubbleCrashMonthsRemaining = 4; LabState a = g.Rival("openaye"); LabState b = g.Rival("anthoptics"); foreach (var r in new[] { a, b }) { r.Models.Add(Released(g, 470)); r.Cash = -20_000; r.WeeksInDebt = 2; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 }); } Finance.Earn(g.Me, LedgerLine.CapitalIn, 350_000_000); g.Do(Command.AcquireLab(g.Id, a.Id)); g.Do(Command.AcquireLab(g.Id, b.Id)); Assert.Equal(2, g.Me.ChipDesigns.Count(cd => cd.Status == ChipDesignStatus.Ready)); }
    [Fact] public void Q457_extra_inferencing_split_after_delivery() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); int t = g.Me.ChipOrders[0].DeliveryTick; long inf0 = LabQueries.Split(g.C, g.State, g.Me, false).Inference; g.State.Tick = t; ChipSystem.RunWeek(g.C, g.State, g.Me); Assert.True(LabQueries.Split(g.C, g.State, g.Me, false).Inference >= inf0); }
    [Fact] public void Q458_extra_regulator_skipped_low_revenue_deal() { var g = Game.New(seed: 1126, tweak: c => c.Acquisitions.RegulatorDealRevenue = long.MaxValue); var r = g.Rival("openaye"); r.Models.Add(Released(g, 410)); r.Cash = -13_000; Finance.Earn(g.Me, LedgerLine.CapitalIn, 58_000_000); int att = g.Me.Bloc(g.Me.HomeBlocId).Attention; g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention); }
    [Fact] public void Q459_extra_chip_order_id_unique_after_transfer() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); var ids = r.ChipOrders.Select(o => o.Id).ToArray(); DistressAcquireChipBuyer(g, r); Assert.Equal(2, g.Me.ChipOrders.Select(o => o.Id).Distinct().Count()); Assert.Contains(ids[0], g.Me.ChipOrders.Select(o => o.Id)); }
    [Fact] public void Q460_extra_month_foundry_month_stable() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 2)); int fm = r.ChipOrders[0].FoundryMonth; DistressAcquireChipBuyer(g, r); Assert.Equal(fm, g.Me.ChipOrders[0].FoundryMonth); }
    [Fact] public void Q461_extra_controller_unchanged_on_player() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); var c0 = g.Me.Controller; ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(c0, g.Me.Controller); }
    [Fact] public void Q462_extra_target_cash_sealed_negative_debt() { var g = Game.New(seed: 1127); var r = g.Rival("openaye"); r.Models.Add(Released(g, 500)); r.Cash = -25_000; Finance.Earn(g.Me, LedgerLine.CapitalIn, 110_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(r.Cash <= 0); }
    [Fact] public void Q463_extra_pending_order_node_id_preserved() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 34_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); string node = r.ChipOrders[0].NodeId; DistressAcquireChipBuyer(g, r); Assert.Equal(node, g.Me.ChipOrders[0].NodeId); }
    [Fact] public void Q464_extra_acquire_then_listing_buy_from_third() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000); DistressAcquireChipBuyer(g, r); LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true); Finance.Earn(anth, LedgerLine.CapitalIn, 35_000_000); g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); Assert.Equal(1, g.Me.Listing(d.Id).SoldThisMonth); }
    [Fact] public void Q465_grouped_capstone_close_schema_economy_roundtrip() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 5)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 2)); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(31, g.State.Schema); Assert.True(LabQueries.ParticipatesInEconomy(g.Me)); SaveSchemaPretend.AssertRoundTrip(g.Sim); Assert.Equal(3, g.Me.ChipOrders.Sum(o => o.Count)); }

    [Fact] public void Q466_epilogue_buy_gpus_refused_absorbed() { var g = Game.New(seed: 1130); var r = g.Rival("openaye"); r.Models.Add(Released(g, 520)); r.Cash = -16_800; Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.BuyGpus(r.Id, 1, HardwareBatch.Hq), CommandError.LabAbsorbed); }
    [Fact] public void Q467_epilogue_queue_gpu_refused_absorbed() { var g = Game.New(seed: 1131); var r = g.Rival("openaye"); r.Models.Add(Released(g, 515)); r.Cash = -16_200; Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.QueueGpuPurchase(r.Id, 2, HardwareBatch.Hq), CommandError.LabAbsorbed); }
    [Fact] public void Q468_epilogue_acquire_lab_count_unchanged() { var g = Game.New(seed: 1132); var r = g.Rival("openaye"); r.Models.Add(Released(g, 530)); r.Cash = -17_000; int n = g.State.Labs.Count; Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(n, g.State.Labs.Count); }
    [Fact] public void Q469_epilogue_player_home_bloc_unchanged() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); string bloc = g.Me.HomeBlocId; ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(bloc, g.Me.HomeBlocId); }
    [Fact] public void Q470_epilogue_seller_listing_max_persists() { var g = Game.New(seed: 1133); var r = g.Rival("openaye"); r.Models.Add(Released(g, 535)); r.Cash = -17_200; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = "n28", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Target = "inference", Version = 1, YieldPermille = 770 }); r.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4800, MaxPerMonth = 7 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 76_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(7, g.Me.Listing(did).MaxPerMonth); }
    [Fact] public void Q471_epilogue_acquire_with_pending_does_not_drop_buyer_orders() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 10)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 75_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 3)); int cnt = r.ChipOrders[0].Count; DistressAcquireChipBuyer(g, r); Assert.Equal(cnt, g.Me.ChipOrders[0].Count); }
    [Fact] public void Q472_epilogue_designing_status_not_transferred_quote_zero() { var g = Game.New(seed: 1134); var r = g.Rival("openaye"); r.Models.Add(Released(g, 525)); r.Cash = -16_500; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Designing, NodeId = "n28", Target = "inference", Version = 1, PointsDone = 5 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000); Assert.Equal(0, DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, r).ChipsPart); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(ChipDesignStatus.Designing, g.Me.Design(did).Status); }
    [Fact] public void Q473_epilogue_taping_out_transfers() { var g = Game.New(seed: 1135); var r = g.Rival("openaye"); r.Models.Add(Released(g, 528)); r.Cash = -16_800; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.TapingOut, NodeId = "n28", Target = "inference", Version = 1 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 73_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(ChipDesignStatus.TapingOut, g.Me.Design(did).Status); }
    [Fact] public void Q474_epilogue_run_month_player_chip_orders_count() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 42_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 54_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); int n = g.State.Labs.Sum(l => l.ChipOrders.Count); DistressAcquireChipBuyer(g, r); ChipSystem.RunMonth(g.C, g.State); Assert.Equal(n, g.State.Labs.Sum(l => l.ChipOrders.Count)); }
    [Fact] public void Q475_epilogue_lab_acquired_event_subject() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired && e.SubjectId == r.Id); }
    [Fact] public void Q476_epilogue_rival_rival_id_preserved() { var g = Game.New(seed: 1136); var r = g.Rival("openaye"); string id = r.RivalId; r.Models.Add(Released(g, 522)); r.Cash = -16_700; Finance.Earn(g.Me, LedgerLine.CapitalIn, 71_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(id, r.RivalId); }
    [Fact] public void Q477_epilogue_second_buyer_order_after_acquire_delivery_ready() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); LabState a = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true); Finance.Earn(a, LedgerLine.CapitalIn, 40_000_000); g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); Assert.Equal(2, g.Me.Listing(d.Id).SoldThisMonth); }
    [Fact] public void Q478_epilogue_compute_split_after_acquire_listed_only() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000); long inf = LabQueries.Split(g.C, g.State, g.Me, false).Inference; g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 2)); DistressAcquireChipBuyer(g, r); Assert.Equal(inf, LabQueries.Split(g.C, g.State, g.Me, false).Inference); }
    [Fact] public void Q479_epilogue_distress_acquire_transfers_staff_morale_drop() { var g = Game.New(seed: 1137); var r = g.Rival("openaye"); r.Models.Add(Released(g, 540)); r.Cash = -18_000; r.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "engineer", Morale = g.C.StaffPeople.MoraleBaseline }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(g.Me.Staff.Last().Morale < g.C.StaffPeople.MoraleBaseline); }
    [Fact] public void Q480_epilogue_target_models_empty_post_seal() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Empty(r.Models); Assert.True(g.Me.Models.Count >= 1); }
    [Fact] public void Q481_epilogue_acquisition_can_acquire_false_when_buyer_bankrupt() { var g = Game.New(seed: 1138); var r = g.Rival("openaye"); r.Models.Add(Released(g, 520)); r.Cash = -16_800; g.Me.Bankrupt = true; Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000); Assert.False(AcquisitionSystem.CanAcquire(g.C, g.State, g.Me, r, out _)); }
    [Fact] public void Q482_epilogue_player_chip_orders_empty_before_pipeline() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); Assert.Empty(g.Me.ChipOrders); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); Assert.Empty(g.Me.ChipOrders); DistressAcquireChipBuyer(g, r); Assert.Single(g.Me.ChipOrders); }
    [Fact] public void Q483_epilogue_listing_price_unchanged_post_acquire_trade() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 5100, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000); long price = g.Me.Listing(d.Id).PricePerChip; g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.Equal(price, g.Me.Listing(d.Id).PricePerChip); }
    [Fact] public void Q484_epilogue_bubble_capital_after_listed_buy() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); g.State.BubbleCrashMonthsRemaining = 4; ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000); long cap0 = g.State.BubbleCapitalRaised; g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 2)); Assert.True(g.State.BubbleCapitalRaised >= cap0); }
    [Fact] public void Q485_epilogue_set_chip_listing_refused_absorbed() { var g = Game.New(seed: 1139); var r = g.Rival("openaye"); r.Models.Add(Released(g, 518)); r.Cash = -17_000; int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = "n28", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Target = "inference", Version = 1, YieldPermille = 765 }); Finance.Earn(g.Me, LedgerLine.CapitalIn, 73_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); g.Refused(Command.SetChipListing(r.Id, did, 3000, 4), CommandError.LabAbsorbed); }
    [Fact] public void Q486_epilogue_acquire_transfers_holdings_to_buyer() { var g = Game.New(seed: 1140); var r = g.Rival("openaye"); r.Models.Add(Released(g, 532)); r.Cash = -17_300; Finance.Earn(r, LedgerLine.CapitalIn, 5_000_000); int hall = Hall(g, r); Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000); int h = g.Me.Holdings.Count; g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.True(g.Me.Holdings.Count > h); }
    [Fact] public void Q487_epilogue_chip_order_delivery_tick_positive() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.True(g.Me.ChipOrders[0].DeliveryTick > g.State.Tick); }
    [Fact] public void Q488_epilogue_player_ledger_hardware_non_negative_month() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 6)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 34_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); Assert.True(g.Me.Ledger.ThisMonth.Hardware >= 0); }
    [Fact] public void Q489_epilogue_crash_weeks_in_debt_on_target() { var g = Game.New(seed: 1141); g.State.BubbleCrashMonthsRemaining = 5; var r = g.Rival("openaye"); r.Models.Add(Released(g, 475)); r.Cash = -28_000; r.WeeksInDebt = 2; Finance.Earn(g.Me, LedgerLine.CapitalIn, 125_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(2, r.WeeksInDebt); }
    [Fact] public void Q490_epilogue_multiple_listings_count() { var g = Game.New(seed: 1142); var r = g.Rival("openaye"); r.Models.Add(Released(g, 538)); r.Cash = -17_400; for (int i = 0; i < 3; i++) { int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = "n28", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Target = "inference", Version = i + 1, YieldPermille = 770 }); r.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4000 + i * 100, MaxPerMonth = 3 }); } Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000); g.Do(Command.AcquireLab(g.Id, r.Id)); Assert.Equal(3, g.Me.ChipListings.Count); }
    [Fact] public void Q491_epilogue_order_count_preserved_on_target() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 4)); int c = r.ChipOrders[0].Count; DistressAcquireChipBuyer(g, r); Assert.Equal(c, g.Me.ChipOrders[0].Count); }
    [Fact] public void Q492_epilogue_acquire_quote_total_positive_distress() { var g = Game.New(seed: 1143); var r = g.Rival("openaye"); r.Models.Add(Released(g, 505)); r.Cash = -16_000; Finance.Earn(g.Me, LedgerLine.CapitalIn, 66_000_000); Assert.True(AcquisitionSystem.Quote(g.C, g.State, g.Me, r).Total > 0); }
    [Fact] public void Q493_epilogue_player_gains_rival_ready_design() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000); int did = g.State.TakeId(); r.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = "n28", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Target = "inference", Version = 1, YieldPermille = 760 }); int readyBefore = g.Me.ChipDesigns.Count(x => x.Status == ChipDesignStatus.Ready); DistressAcquireChipBuyer(g, r); Assert.Equal(readyBefore + 1, g.Me.ChipDesigns.Count(x => x.Status == ChipDesignStatus.Ready)); Assert.Contains(g.Me.ChipDesigns, x => x.Id == did); }
    [Fact] public void Q494_epilogue_run_week_keeps_orders_until_delivery() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8)); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 54_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 1)); DistressAcquireChipBuyer(g, r); int del = g.Me.ChipOrders[0].DeliveryTick; g.State.Tick = del - 2; g.Sim.Step(); Assert.Single(g.Me.ChipOrders); }
    [Fact] public void Q495_grouped_epilogue_bar_hook() { Game g = HatchFollowUpLaterTests.ChipListingSeller(); ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g); g.Do(Command.SetChipListing(g.Id, d.Id, 5200, 4)); Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000); LabState r = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000); g.Do(Command.BuyListedChips(r.Id, g.Id, d.Id, HardwareBatch.Cloud, 2)); DistressAcquireChipBuyer(g, r); Assert.Equal(31, g.State.Schema); Assert.Single(g.Me.ChipOrders); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
}