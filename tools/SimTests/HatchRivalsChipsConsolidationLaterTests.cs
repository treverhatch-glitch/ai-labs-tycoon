using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q222–Q232: rivals, custom chips, and crash M&amp;A consolidation (no new save schema).</summary>
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
}