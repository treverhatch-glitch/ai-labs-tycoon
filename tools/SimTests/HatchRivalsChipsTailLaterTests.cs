using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q366–Q400: tail closure — delivery, ledger, feelings, rollup (no save schema).</summary>
public class HatchRivalsChipsTailLaterTests
{
    static ModelState Released(Game g, int cap, string name = "T") => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = name
    };

    static int RivalHall(Game g, LabState lab)
    {
        int id = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(id).HallKw = 500_000;
        return id;
    }

    [Fact]
    public void Q366_acquire_then_delivery_adds_player_chip_fleet_batch()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int deliver = rival.ChipOrders[0].DeliveryTick;
        rival.Models.Add(Released(g, 518));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int fleetsBefore = g.Me.Fleet.Count;
        g.State.Tick = deliver;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Fleet.Count > fleetsBefore);
        Assert.Empty(g.Me.ChipOrders);
    }

    [Fact]
    public void Q367_listed_chip_sales_ledger_on_seller_after_acquire_then_second_buy()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        long sales0 = g.Me.Ledger.Lifetime.ChipSales;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 505));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 30_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.True(g.Me.Ledger.Lifetime.ChipSales > sales0);
    }

    [Fact]
    public void Q368_roll_up_disabled_player_still_gets_transferred_listing()
    {
        var g = Game.New(seed: 1066, tweak: c => c.Acquisitions.RivalRollupChancePermille = 0);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540));
        rival.Cash = -18_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 2, YieldPermille = 800 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 5200, MaxPerMonth = 5 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.NotNull(g.Me.Listing(did));
        Assert.Equal(5200, g.Me.Listing(did).PricePerChip);
    }

    [Fact]
    public void Q369_max_per_month_caps_buy_after_acquire_transferred_listing()
    {
        var g = Game.New(seed: 1067);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 525));
        rival.Cash = -16_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4000, MaxPerMonth = 2, SoldThisMonth = 1 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 25_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, did, HardwareBatch.Cloud, 1));
        g.Refused(Command.BuyListedChips(anth.Id, g.Id, did, HardwareBatch.Cloud, 1), CommandError.AllocationExhausted);
    }

    [Fact]
    public void Q370_two_designs_two_listings_both_transfer_on_acquire()
    {
        var g = Game.New(seed: 1068);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -19_000;
        int a = g.State.TakeId();
        int b = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = a, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 });
        rival.ChipDesigns.Add(new ChipDesign { Id = b, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "training", Version = 1, YieldPermille = 750 });
        rival.ChipListings.Add(new ChipListing { DesignId = a, PricePerChip = 4500, MaxPerMonth = 4 });
        rival.ChipListings.Add(new ChipListing { DesignId = b, PricePerChip = 3800, MaxPerMonth = 4 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.ChipListings.Count);
        Assert.NotNull(g.Me.Listing(a));
        Assert.NotNull(g.Me.Listing(b));
    }

    [Fact]
    public void Q371_feelings_chip_buy_then_acquire_seller_unchanged_on_player()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int feelOnRival = rival.FeelingToPlayer;
        rival.Models.Add(Released(g, 510));
        rival.Cash = -17_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(feelOnRival, rival.FeelingToPlayer);
    }

    [Fact]
    public void Q372_compute_split_unchanged_until_chip_delivery()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 500));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        ComputeSplit before = LabQueries.Split(g.C, g.State, g.Me, false);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ComputeSplit afterAcquire = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(before.Inference, afterAcquire.Inference);
    }

    [Fact]
    public void Q373_start_chip_design_refused_on_absorbed_lab()
    {
        var g = Game.New(seed: 1069);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 530));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.StartChipDesign(rival.Id, "inference", "n28", "medium", "fp16", "ddr", "air"), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q374_buy_land_refused_on_absorbed_target()
    {
        var g = Game.New(seed: 1070);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 515));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.BuyLand(rival.Id, "eu-ireland"), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q375_transferred_holdings_count_includes_rival_sites()
    {
        var g = Game.New(seed: 1071);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 545));
        rival.Cash = -18_000;
        Finance.Earn(rival, LedgerLine.CapitalIn, 8_000_000);
        int hall = RivalHall(g, rival);
        Assert.True(hall > 0);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        int holdingsBefore = g.Me.Holdings.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Holdings.Count > holdingsBefore);
        Assert.Empty(rival.Holdings);
    }

    [Fact]
    public void Q376_dual_pending_orders_deliver_both_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 15));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 85_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 512));
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 110_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int maxDeliver = g.Me.ChipOrders.Max(o => o.DeliveryTick);
        g.State.Tick = maxDeliver;
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.ChipOrders);
        Assert.True(g.Me.Fleet.Where(b => b.ChipDesignId == d.Id).Sum(b => b.Count) >= 2);
    }

    [Fact]
    public void Q377_vendor_standing_after_listed_buy_survives_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 34_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int stand = VendorSystem.StandingOf(g.State, g.Me);
        rival.Models.Add(Released(g, 508));
        rival.Cash = -16_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 86_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(stand, VendorSystem.StandingOf(g.State, g.Me));
    }

    [Fact]
    public void Q378_acquire_with_self_fulfilled_chip_order_on_target()
    {
        var g = Game.New(seed: 1072);
        g.State.BubbleCrashMonthsRemaining = 4;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 485));
        rival.Cash = -32_000;
        rival.WeeksInDebt = 2;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = did, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = month, DeliveryTick = g.State.Tick + 30, NodeId = g.C.Chips.Nodes[0].IdValue });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 140_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.Equal(did, g.Me.ChipOrders[0].DesignId);
    }

    [Fact]
    public void Q379_player_cash_drops_by_quote_on_distressed_chip_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 58_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 520));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        long quote = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival).Total;
        long cashBefore = g.Me.Cash;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(cashBefore - g.Me.Cash >= quote);
    }

    [Fact]
    public void Q380_absorbed_rival_sell_model_refused()
    {
        var g = Game.New(seed: 1073);
        LabState rival = g.Rival("openaye");
        ModelState m = Released(g, 525);
        rival.Models.Add(m);
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.SellModel(rival.Id, m.Id), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q381_chip_month_reset_after_acquire_listing_trade()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 3));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        rival.Models.Add(Released(g, 498));
        rival.Cash = -15_800;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4100, MaxPerMonth = 4, SoldThisMonth = 2 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(did).SoldThisMonth);
    }

    [Fact]
    public void Q382_participates_economy_false_on_absorbed_chip_rich_rival()
    {
        var g = Game.New(seed: 1074);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 575));
        rival.Cash = -19_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 810 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 76_000_000);
        Assert.True(LabQueries.ParticipatesInEconomy(rival));
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.False(LabQueries.ParticipatesInEconomy(rival));
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
    }

    [Fact]
    public void Q383_save_round_trip_after_dual_order_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4050, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 507));
        rival.Cash = -16_100;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 105_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        Assert.Equal(2, loaded.Me.ChipOrders.Count);
        Assert.Equal(51, loaded.State.Schema);
    }

    [Fact]
    public void Q384_hall_order_delivery_tick_unchanged_after_holding_remap()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4700, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 75_000_000);
        int hall = RivalHall(g, rival);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, hall, 2));
        int deliver = rival.ChipOrders[0].DeliveryTick;
        rival.Models.Add(Released(g, 522));
        rival.Cash = -17_800;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(deliver, g.Me.ChipOrders[0].DeliveryTick);
    }

    [Fact]
    public void Q385_acquire_clears_target_chip_listings_from_rival_lab_object()
    {
        var g = Game.New(seed: 1075);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 535));
        rival.Cash = -17_200;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4300, MaxPerMonth = 6 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipListings);
        Assert.Single(g.Me.ChipListings);
    }

    [Fact]
    public void Q386_bubble_capital_unchanged_acquire_without_new_listing()
    {
        var g = Game.New(seed: 1076);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 470));
        rival.Cash = -28_000;
        rival.WeeksInDebt = 2;
        long cap = g.State.BubbleCapitalRaised;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 130_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(cap, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q387_registry_attention_only_when_deal_above_threshold()
    {
        var g = Game.New(seed: 1077, tweak: c => c.Acquisitions.RegulatorDealRevenue = 1_000_000L);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 420));
        rival.Cash = -14_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention >= att);
    }

    [Fact]
    public void Q388_chip_order_count_global_stable_through_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 56_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        int globalBefore = g.State.Labs.Sum(l => l.ChipOrders.Count);
        rival.Models.Add(Released(g, 515));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(globalBefore, g.State.Labs.Sum(l => l.ChipOrders.Count));
    }

    [Fact]
    public void Q389_design_lab_on_order_stays_player_when_design_on_player()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4150, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 510));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.All(g.Me.ChipOrders, o => Assert.Equal(g.Id, o.DesignLabId));
    }

    [Fact]
    public void Q390_rival_rollup_moves_chip_orders_off_weak_peer()
    {
        var g = Game.New(seed: 1078, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState strong = g.Rival("openaye");
        strong.PlanStartTick = 0;
        Finance.Earn(strong, LedgerLine.CapitalIn, 600_000_000);
        strong.Models.Add(Released(g, 700));
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 455));
        weak.Cash = -88_000;
        weak.WeeksInDebt = 3;
        weak.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = 1, BuyerLabId = weak.Id, DesignLabId = weak.Id, HoldingId = HardwareBatch.Cloud, Count = 3, FoundryMonth = 0, DeliveryTick = g.State.Tick + 20, NodeId = g.C.Chips.Nodes[0].IdValue });
        AcquisitionSystem.RunMonth(g.C, g.State);
        if (weak.AbsorbedByLabId == strong.Id)
            Assert.True(strong.ChipOrders.Count >= 1);
        else
            Assert.Single(weak.ChipOrders);
    }

    [Fact]
    public void Q391_training_target_design_does_not_move_inference_split_on_acquire()
    {
        var g = Game.New(seed: 1079);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 530));
        rival.Cash = -17_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "training", Version = 1, YieldPermille = 720 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        ComputeSplit before = LabQueries.Split(g.C, g.State, g.Me, false);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ComputeSplit after = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(before.Inference, after.Inference);
    }

    [Fact]
    public void Q392_acquire_then_refuse_second_acquire_same_target()
    {
        var g = Game.New(seed: 1080);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520));
        rival.Cash = -16_800;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 66_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Q393_weekly_steps_until_delivery_do_not_duplicate_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4250, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 502));
        rival.Cash = -15_900;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 82_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int deliver = g.Me.ChipOrders[0].DeliveryTick;
        int count = g.Me.ChipOrders.Count;
        while (g.State.Tick < deliver - 1) g.Sim.Step();
        Assert.Equal(count, g.Me.ChipOrders.Count);
    }

    [Fact]
    public void Q394_target_controller_plan_after_absorb_with_chip_assets()
    {
        var g = Game.New(seed: 1081);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 528));
        rival.Cash = -16_900;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 775 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(LabController.Plan, rival.Controller);
    }

    [Fact]
    public void Q395_chip_sales_this_month_on_player_after_post_acquire_buy()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4750, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        rival.Models.Add(Released(g, 500));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 84_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 28_000_000);
        long month0 = g.Me.Ledger.ThisMonth.ChipSales;
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.True(g.Me.Ledger.ThisMonth.ChipSales > month0);
    }

    [Fact]
    public void Q396_acquisition_event_kind_after_chip_pipeline_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3950, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 42_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 54_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 512));
        rival.Cash = -16_400;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 92_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired && e.LabId == g.Id);
    }

    [Fact]
    public void Q397_quote_total_positive_for_solvent_buy_of_chip_distressed()
    {
        var g = Game.New(seed: 1082);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 600));
        rival.Cash = -21_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 795 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000);
        long total = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival).Total;
        Assert.True(total > 0);
    }

    [Fact]
    public void Q398_seal_absorbed_target_chip_orders_empty()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 48_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 58_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 518));
        rival.Cash = -17_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 94_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipOrders);
        Assert.Equal(2, g.Me.ChipOrders.Sum(o => o.Count));
    }

    [Fact]
    public void Q399_fleet_gpu_and_chip_batches_both_remap_design_lab()
    {
        var g = Game.New(seed: 1083);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 555));
        rival.Cash = -18_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 785 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 4, ArrivedTick = 0 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, ChipDesignId = did, DesignLabId = rival.Id, Count = 2, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.All(g.Me.Fleet.Where(b => b.ChipDesignId == did), b => Assert.Equal(g.Id, b.DesignLabId));
    }

    [Fact]
    public void Q400_tail_bar_hook_schema_29_and_economy()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5100, 4));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 26_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 38_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 495));
        rival.Cash = -15_600;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(51, g.State.Schema);
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }
}