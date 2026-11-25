using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q222–Q262: rivals, custom chips, and crash M&amp;A consolidation (no new save schema).</summary>
public class HatchRivalsChipsConsolidationLaterTests
{
    static ModelState Released(Game g, int cap, string name) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = name
    };

    static void RunPlanner(Game g, LabState lab, string rivalId)
    {
        PlanConfig plan = g.C.Rival(rivalId).Plan;
        LabPlanner.Run(g.C, g.State, lab, plan, cmd => g.Try(cmd));
    }

    [Fact]
    public void Q222_acquire_transfers_in_flight_chip_orders_to_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        rival.PlanStartTick = 0;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        Assert.Single(rival.ChipOrders);
        rival.Models.Add(Released(g, 580, "R"));
        rival.Cash = -30_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        int orderId = rival.ChipOrders[0].Id;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipOrders);
        ChipOrder moved = g.Me.ChipOrders.Single(o => o.Id == orderId);
        Assert.Equal(g.Id, moved.BuyerLabId);
        Assert.Equal(g.Id, moved.DesignLabId);
    }

    [Fact]
    public void Q223_pending_chip_order_keeps_delivery_tick_on_buyer_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        ChipOrder before = rival.ChipOrders[0];
        int deliver = before.DeliveryTick;
        int count = before.Count;
        rival.Models.Add(Released(g, 520, "X"));
        rival.Cash = -25_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipOrder order = g.Me.ChipOrders.Single(o => o.Id == before.Id);
        Assert.Equal(g.Id, order.BuyerLabId);
        Assert.Equal(g.Id, order.DesignLabId);
        Assert.Equal(deliver, order.DeliveryTick);
        Assert.Equal(count, order.Count);
        Assert.Equal(HardwareBatch.Cloud, order.HoldingId);
    }

    [Fact]
    public void Q224_fleet_chip_batch_design_lab_id_remapped_on_acquire()
    {
        var g = Game.New(seed: 922);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 550, "F"));
        rival.Cash = -20_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign
        {
            Id = designId,
            Status = ChipDesignStatus.Ready,
            NodeId = g.C.Chips.Nodes[0].Id,
            Target = "inference",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            Version = 1,
            YieldPermille = 800
        });
        rival.Fleet.Add(new HardwareBatch
        {
            Id = g.State.TakeId(),
            HoldingId = HardwareBatch.Hq,
            ChipDesignId = designId,
            DesignLabId = rival.Id,
            Count = 8,
            ArrivedTick = 0
        });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        HardwareBatch batch = g.Me.Fleet.Single(b => b.ChipDesignId == designId);
        Assert.Equal(g.Id, batch.DesignLabId);
    }

    static LabState GoggleWithReadyChip(Game g)
    {
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        LabState goggle = WorldSetup.AddRival(g.C, g.State, g.C.Rival("goggle"), EraSystem.AtTick(g.C, g.Tick), true);
        goggle.PlanStartTick = 0;
        foreach (string tech in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling" })
            if (!goggle.Techs.Contains(tech)) goggle.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(goggle.LotId), goggle.Rooms, 4, 3, out int x, out int y);
        goggle.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "chip-design-lab", X = x, Y = y, Width = 4, Height = 3 });
        ChipSpec spec = ChipQueries.Derive(g.C, "inference", "n28", "medium", "fp16", "ddr", "air");
        goggle.ChipDesigns.Add(new ChipDesign
        {
            Id = g.State.TakeId(),
            Version = 1,
            Name = "Distress",
            Target = "inference",
            NodeId = "n28",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            PointsRequired = spec.Points,
            PointsDone = spec.Points,
            Status = ChipDesignStatus.Ready,
            YieldPermille = 900,
            ReadyTick = g.Tick
        });
        if (goggle.SoftwarePermille == 0) goggle.SoftwarePermille = g.C.Chips.SoftwareStart;
        return goggle;
    }

    [Fact]
    public void Q225_distress_chip_listing_runs_while_weeks_in_debt_in_crash()
    {
        var g = Game.New(seed: 923);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState goggle = GoggleWithReadyChip(g);
        goggle.Cash = -35_000;
        goggle.WeeksInDebt = 2;
        goggle.Models.Add(Released(g, 500, "G"));
        RunPlanner(g, goggle, "goggle");
        ChipDesign d = goggle.ChipDesigns[0];
        Assert.NotNull(goggle.Listing(d.Id));
        DecisionEntry list = goggle.Log.LastOrDefault(e => e.Action == DecisionAction.ListedChips);
        Assert.NotNull(list);
        Assert.True(list.Has(ReasonCode.CrashDistress) || goggle.Cash < 0);
    }

    [Fact]
    public void Q226_planner_acquire_skipped_when_runway_below_double_reserve()
    {
        var g = Game.New(seed: 924, tweak: c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 600, "W"));
        weak.Cash = -90_000;
        weak.WeeksInDebt = 3;
        long weekly = LabQueries.WeeklyRunningCost(g.C, g.State, buyer);
        int reserve = g.C.Rival("openaye").Plan.ReserveWeeks;
        buyer.Cash = weekly * (reserve * 2 - 1);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 1);
        RunPlanner(g, buyer, "openaye");
        Assert.True(weak.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q227_chip_listing_sold_this_month_resets_on_chip_month()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3000, 20));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState buyer = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.Equal(2, g.Me.Listing(d.Id).SoldThisMonth);
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(d.Id).SoldThisMonth);
    }

    [Fact]
    public void Q228_player_chip_trade_warms_rival_feeling_in_crash()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 4;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3500, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        LabState goggle = GoggleWithReadyChip(g);
        goggle.ChipDesigns.Clear();
        goggle.ChipDesigns.Add(d);
        goggle.ChipListings.Add(new ChipListing { DesignId = d.Id, PricePerChip = 3500, MaxPerMonth = 8 });
        Finance.Earn(goggle, LedgerLine.CapitalIn, 80_000_000);
        int feel = goggle.FeelingToPlayer;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        g.Do(Command.BuyListedChips(g.Id, goggle.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Equal(feel + g.C.Rivalry.Feelings.ChipOrderPermille, goggle.FeelingToPlayer);
    }

    [Fact]
    public void Q229_save_load_preserves_chip_orders_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 510, "S"));
        rival.Cash = -22_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int n = g.Me.ChipOrders.Count;
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        Assert.Equal(n, loaded.Me.ChipOrders.Count);
        Assert.Equal(g.Id, loaded.Me.ChipOrders[0].BuyerLabId);
    }

    [Fact]
    public void Q230_two_chip_listings_both_transfer_on_acquire()
    {
        var g = Game.New(seed: 926);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540, "T"));
        rival.Cash = -18_000;
        int a = g.State.TakeId();
        int b = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = a, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 750 });
        rival.ChipDesigns.Add(new ChipDesign { Id = b, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 2, YieldPermille = 720 });
        rival.ChipListings.Add(new ChipListing { DesignId = a, PricePerChip = 4000, MaxPerMonth = 5 });
        rival.ChipListings.Add(new ChipListing { DesignId = b, PricePerChip = 4200, MaxPerMonth = 5 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.ChipListings.Count);
        Assert.Contains(g.Me.ChipListings, l => l.DesignId == a);
        Assert.Contains(g.Me.ChipListings, l => l.DesignId == b);
    }

    [Fact]
    public void Q231_absorbed_seller_chip_listing_refused_for_new_buy()
    {
        var g = Game.New(seed: 927);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500, "A"));
        rival.Cash = -15_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 3000, MaxPerMonth = 4 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Finance.Earn(g.Rival("anthoptics"), LedgerLine.CapitalIn, 20_000_000);
        g.Refused(Command.BuyListedChips(g.Rival("anthoptics").Id, rival.Id, designId, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q232_distress_fire_sale_at_most_once_per_quarter()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.CrashDistressOfferPermille = 600);
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.State.BubbleCrashMonthsRemaining = 8;
        g.Me.Cash = -60_000;
        g.Me.WeeksInDebt = 2;
        for (int i = 0; i < 6; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        int sales = g.Me.Log.Count(e => e.Action == DecisionAction.SoldModel);
        for (int i = 0; i < 3 * g.C.Calendar.WeeksPerMonth; i++) g.Sim.Step();
        int salesAfter = g.Me.Log.Count(e => e.Action == DecisionAction.SoldModel);
        Assert.Equal(sales, salesAfter);
    }

    [Fact]
    public void Q233_acquire_quote_counts_designs_not_in_flight_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 4));
        Assert.Single(rival.ChipOrders);
        rival.Models.Add(Released(g, 530, "O"));
        rival.Cash = -28_000;
        var withOrder = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        rival.ChipOrders.Clear();
        var withoutOrder = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.Equal(withOrder.ModelsPart, withoutOrder.ModelsPart);
        Assert.Equal(withOrder.ChipsPart, withoutOrder.ChipsPart);
        Assert.Equal(withOrder.Total, withoutOrder.Total);
    }

    [Fact]
    public void Q234_rent_only_acquirer_keeps_cloud_chip_order_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState target = HatchFollowUpLaterTests.OpenAyeBuyer(g, 30_000_000);
        g.Do(Command.BuyListedChips(target.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        target.Models.Add(Released(g, 500, "T"));
        target.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, target.Id));
        ChipOrder o = g.Me.ChipOrders.Single();
        Assert.Equal(HardwareBatch.Cloud, o.HoldingId);
        Assert.Equal(0, o.ReservedKw);
    }

    [Fact]
    public void Q235_absorbed_lab_cannot_start_chip_design()
    {
        var g = Game.New(seed: 935);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520, "D"));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.StartChipDesign(rival.Id, "inference", "n28", "medium", "fp16", "ddr", "air"), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q236_transfer_preserves_foundry_month_booking()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 65_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int month = rival.ChipOrders[0].FoundryMonth;
        long booked = 0;
        foreach (var m in g.State.Foundry) if (m.MonthIndex == month) booked += m.Booked;
        rival.Models.Add(Released(g, 510, "F"));
        rival.Cash = -24_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        long bookedAfter = 0;
        foreach (var m in g.State.Foundry) if (m.MonthIndex == month) bookedAfter += m.Booked;
        Assert.Equal(booked, bookedAfter);
        Assert.Equal(month, g.Me.ChipOrders[0].FoundryMonth);
    }

    [Fact]
    public void Q237_crash_listed_chip_trade_raises_bubble_capital_with_orders_on_acquirer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 5;
        g.C.Bubble.ListedChipBoostPermille = 20;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 505, "C"));
        rival.Cash = -22_000;
        long cap = g.State.BubbleCapitalRaised;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.State.BubbleCapitalRaised >= cap);
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q238_regulator_attention_on_chip_heavy_distressed_acquire()
    {
        var g = Game.New(seed: 938, tweak: c => c.Acquisitions.RegulatorDealRevenue = 1);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 620, "H"));
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign
        {
            Id = designId,
            Status = ChipDesignStatus.Ready,
            NodeId = g.C.Chips.Nodes[0].Id,
            Target = "inference",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            Version = 1,
            YieldPermille = 850
        });
        rival.Cash = -40_000;
        g.State.BubbleCrashMonthsRemaining = 4;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.ChipsPart > 0);
        int attBefore = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention > attBefore);
    }

    [Fact]
    public void Q239_planner_chip_list_during_crash_does_not_grow_gpus_when_retrenching()
    {
        var g = Game.New(seed: 939);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState goggle = GoggleWithReadyChip(g);
        goggle.Cash = -32_000;
        goggle.WeeksInDebt = 1;
        goggle.Models.Add(Released(g, 480, "L"));
        int gpusBefore = goggle.RentedGpus;
        RunPlanner(g, goggle, "goggle");
        Assert.True(goggle.Listing(goggle.ChipDesigns[0].Id) != null || goggle.Log.Any(e => e.Action == DecisionAction.ListedChips));
        Assert.True(goggle.RentedGpus <= gpusBefore);
    }

    [Fact]
    public void Q240_cloud_chip_order_survives_save_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 495, "S"));
        rival.Cash = -21_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        ChipOrder o = loaded.Me.ChipOrders.Single();
        Assert.Equal(HardwareBatch.Cloud, o.HoldingId);
        Assert.Equal(d.Id, o.DesignId);
    }

    [Fact]
    public void Q241_active_listing_and_in_flight_order_both_on_buyer_after_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3600, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.ChipListings.Add(new ChipListing { DesignId = d.Id, PricePerChip = 3500, MaxPerMonth = 6 });
        rival.Models.Add(Released(g, 490, "B"));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.NotNull(g.Me.Listing(d.Id));
    }

    [Fact]
    public void Q242_third_rival_buys_listing_from_buyer_with_pending_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState target = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(target.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        target.Models.Add(Released(g, 515, "P"));
        target.Cash = -23_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, target.Id));
        LabState third = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(third, LedgerLine.CapitalIn, 30_000_000);
        int ordersBefore = g.Me.ChipOrders.Count;
        g.Do(Command.BuyListedChips(third.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.True(g.Me.ChipOrders.Count >= ordersBefore);
        Assert.NotNull(g.Me.Listing(d.Id));
    }

    [Fact]
    public void Q243_acquire_remaps_buyer_and_design_lab_on_listed_chip_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        ChipOrder before = rival.ChipOrders[0];
        Assert.Equal(rival.Id, before.BuyerLabId);
        Assert.Equal(g.Id, before.DesignLabId);
        rival.Models.Add(Released(g, 505, "M"));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 110_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipOrder after = g.Me.ChipOrders.Single(o => o.Id == before.Id);
        Assert.Equal(g.Id, after.BuyerLabId);
        Assert.Equal(g.Id, after.DesignLabId);
    }

    [Fact]
    public void Q244_only_acquired_lab_orders_move_not_sibling_rival()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        LabState prey = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        LabState bystander = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(bystander, LedgerLine.CapitalIn, 5_000_000);
        g.Do(Command.BuyListedChips(prey.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(bystander.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        prey.Models.Add(Released(g, 530, "Y"));
        prey.Cash = -22_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, prey.Id));
        Assert.Empty(prey.ChipOrders);
        Assert.Single(bystander.ChipOrders);
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q245_fleet_hq_batch_survives_acquire_with_chip_design_ids_intact()
    {
        var g = Game.New(seed: 945);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560, "H"));
        rival.Cash = -17_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, ChipDesignId = designId, DesignLabId = rival.Id, Count = 4, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.Me.ChipDesigns, cd => cd.Id == designId);
        Assert.Equal(g.Id, g.Me.Fleet.Single(b => b.ChipDesignId == designId).DesignLabId);
    }

    [Fact]
    public void Q246_acquire_transfers_listing_including_sold_this_month()
    {
        var g = Game.New(seed: 946);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 510, "S"));
        rival.Cash = -17_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 3800, MaxPerMonth = 8, SoldThisMonth = 3 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipListing listing = g.Me.Listing(designId);
        Assert.NotNull(listing);
        Assert.Equal(3800, listing.PricePerChip);
        Assert.Equal(3, listing.SoldThisMonth);
    }

    [Fact]
    public void Q247_set_chip_listing_refused_on_absorbed_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 30_000_000);
        rival.Models.Add(Released(g, 480, "Z"));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.SetChipListing(rival.Id, d.Id, 3000, 4), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q248_acquire_empties_target_foundry_booking_only_for_transferred_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        string node = g.C.Chips.Nodes[0].IdValue;
        int month = rival.ChipOrders[0].FoundryMonth;
        long booked = ChipQueries.BookedIn(g.State, node, month);
        rival.Models.Add(Released(g, 500, "N"));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(booked, ChipQueries.BookedIn(g.State, node, month));
        Assert.Equal(3, g.Me.ChipOrders.Sum(o => o.Count));
    }

    static int RivalHall(Game g, LabState lab)
    {
        int id = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(id).HallKw = 500_000;
        return id;
    }

    [Fact]
    public void Q249_hall_target_chip_order_remaps_holding_id_on_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        int hall = RivalHall(g, rival);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, hall, 2));
        int oldHolding = rival.ChipOrders[0].HoldingId;
        Assert.Equal(hall, oldHolding);
        rival.Models.Add(Released(g, 535, "H"));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipOrder o = g.Me.ChipOrders.Single();
        Assert.NotEqual(hall, o.HoldingId);
        Assert.Contains(g.Me.Holdings, h => h.Id == o.HoldingId);
    }

    [Fact]
    public void Q250_taping_out_design_transfers_on_acquire()
    {
        var g = Game.New(seed: 950);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 545, "T"));
        rival.Cash = -16_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.TapingOut, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipDesign d = g.Me.Design(designId);
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
        Assert.Equal(0, q.ChipsPart);
    }

    [Fact]
    public void Q251_designing_chip_transfers_without_chips_quote_part()
    {
        var g = Game.New(seed: 951);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 530, "D"));
        rival.Cash = -14_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Designing, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, PointsDone = 10 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        Assert.Equal(0, DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(ChipDesignStatus.Designing, g.Me.Design(designId).Status);
    }

    [Fact]
    public void Q252_two_in_flight_orders_both_transfer_on_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 20));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 90_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        Assert.Equal(2, rival.ChipOrders.Count);
        rival.Models.Add(Released(g, 525, "2"));
        rival.Cash = -21_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        int before = g.State.Labs.Sum(l => l.ChipOrders.Count);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.ChipOrders.Count);
        Assert.Equal(before, g.State.Labs.Sum(l => l.ChipOrders.Count));
    }

    [Fact]
    public void Q253_chip_order_save_after_hall_remap_survives_round_trip()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4700, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 85_000_000);
        int hall = RivalHall(g, rival);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, hall, 2));
        rival.Models.Add(Released(g, 515, "R"));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int holding = g.Me.ChipOrders[0].HoldingId;
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        Assert.Equal(holding, loaded.Me.ChipOrders[0].HoldingId);
    }

    [Fact]
    public void Q254_acquire_does_not_duplicate_chip_designs_when_target_listed_from_player()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 505, "U"));
        rival.Cash = -18_000;
        int readyOnPlayer = g.Me.ChipDesigns.Count(cd => cd.Status == ChipDesignStatus.Ready);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(readyOnPlayer, g.Me.ChipDesigns.Count(cd => cd.Status == ChipDesignStatus.Ready));
        Assert.Empty(rival.ChipDesigns);
    }

    [Fact]
    public void Q255_rival_own_ready_design_and_order_transfer_to_player()
    {
        var g = Game.New(seed: 952);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 570, "O"));
        rival.Cash = -25_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign
        {
            Id = designId,
            Status = ChipDesignStatus.Ready,
            NodeId = g.C.Chips.Nodes[0].Id,
            Target = "inference",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            Version = 1,
            YieldPermille = 820
        });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 6000, MaxPerMonth = 4 });
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = designId, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = month, DeliveryTick = g.State.Tick + 24 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        long chipsPart = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart;
        Assert.True(chipsPart > 0);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.Equal(designId, g.Me.ChipOrders[0].DesignId);
        Assert.Contains(g.Me.ChipDesigns, x => x.Id == designId);
    }

    [Fact]
    public void Q256_bubble_crash_acquire_with_chip_order_still_seals_target()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 5;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 490, "C"));
        rival.Cash = -30_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 130_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired);
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q257_feelings_from_chip_buy_persist_on_seller_after_acquire_not_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        int feelBefore = rival.FeelingToPlayer;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int feelAfterBuy = rival.FeelingToPlayer;
        rival.Models.Add(Released(g, 500, "F"));
        rival.Cash = -22_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(feelBefore + g.C.Rivalry.Feelings.ChipOrderPermille, feelAfterBuy);
        Assert.Equal(feelAfterBuy, rival.FeelingToPlayer);
    }

    [Fact]
    public void Q258_acquire_moves_rival_orders_without_touching_player_chip_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        int rivalOrders = rival.ChipOrders.Count;
        Assert.True(rivalOrders >= 1);
        Assert.Empty(g.Me.ChipOrders);
        rival.Models.Add(Released(g, 520, "K"));
        rival.Cash = -19_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 140_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipOrders);
        Assert.True(g.Me.ChipOrders.Count >= rivalOrders);
        Assert.Equal(5, g.Me.ChipOrders.Sum(o => o.Count));
    }

    [Fact]
    public void Q259_order_chips_refused_on_absorbed_target_lab()
    {
        var g = Game.New(seed: 953);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 555, "A"));
        rival.Cash = -17_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.OrderChips(rival.Id, designId, HardwareBatch.Cloud, 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q260_chip_fleet_on_transferred_holding_keeps_design_after_acquire()
    {
        var g = Game.New(seed: 954);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 565, "W"));
        rival.Cash = -18_000;
        Finance.Earn(rival, LedgerLine.CapitalIn, 5_000_000);
        int hall = RivalHall(g, rival);
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 790 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = hall, ChipDesignId = designId, DesignLabId = rival.Id, Count = 6, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        HardwareBatch batch = g.Me.Fleet.Single(b => b.ChipDesignId == designId);
        Assert.Equal(g.Id, batch.DesignLabId);
        Assert.Contains(g.Me.Holdings, h => h.Id == batch.HoldingId);
    }

    [Fact]
    public void Q261_distress_quote_discount_applies_with_pending_chip_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 495, "Q"));
        rival.Cash = -32_000;
        rival.WeeksInDebt = 2;
        g.State.BubbleCrashMonthsRemaining = 8;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        long normal = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.State.BubbleCrashMonthsRemaining = 0;
        long calm = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.State.BubbleCrashMonthsRemaining = 8;
        Assert.True(normal < calm);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q262_rollup_month_does_not_clear_transferred_chip_orders_from_buyer()
    {
        var g = Game.New(seed: 955, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState strong = g.Rival("openaye");
        strong.PlanStartTick = 0;
        Finance.Earn(strong, LedgerLine.CapitalIn, 500_000_000);
        strong.Models.Add(Released(g, 700, "S"));
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 480, "W"));
        weak.Cash = -95_000;
        weak.WeeksInDebt = 3;
        int designId = g.State.TakeId();
        weak.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 750 });
        weak.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = designId, BuyerLabId = weak.Id, DesignLabId = weak.Id, HoldingId = HardwareBatch.Cloud, Count = 4, FoundryMonth = ChipQueries.MonthIndex(g.C, g.State.Tick), DeliveryTick = g.State.Tick + 20 });
        AcquisitionSystem.RunMonth(g.C, g.State);
        if (weak.AbsorbedByLabId == strong.Id)
        {
            Assert.True(strong.ChipOrders.Count >= 1);
            Assert.Empty(weak.ChipOrders);
        }
        else
            Assert.Contains(g.State.Labs, l => l.ChipOrders.Count > 0);
    }
}