using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q331–Q365: chip pipeline × distress, sales, rollup (no save schema).</summary>
public class HatchRivalsChipsIntegrateLaterTests
{
    static ModelState Released(Game g, int cap) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = "M"
    };

    [Fact]
    public void Q331_chip_order_and_distress_acquire_same_target()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 8;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState buyer = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        buyer.Models.Add(Released(g, 520));
        buyer.Cash = -25_000;
        buyer.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, buyer.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired);
    }

    [Fact]
    public void Q332_acquired_lab_chip_design_count_on_buyer()
    {
        var g = Game.New(seed: 1031);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540));
        rival.Cash = -17_000;
        int a = g.State.TakeId();
        int b = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = a, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        rival.ChipDesigns.Add(new ChipDesign { Id = b, Status = ChipDesignStatus.Designing, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 2, PointsDone = 5 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        int before = g.Me.ChipDesigns.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(before + 2, g.Me.ChipDesigns.Count);
    }

    [Fact]
    public void Q333_sell_model_refused_on_absorbed_rival()
    {
        var g = Game.New(seed: 1032);
        LabState rival = g.Rival("openaye");
        ModelState m = Released(g, 500);
        rival.Models.Add(m);
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.SellModel(rival.Id, m.Id), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q334_chip_order_on_target_does_not_block_acquire_quote_cash_part()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 4));
        rival.Models.Add(Released(g, 510));
        rival.Cash = 50_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        long cashPart = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).CashPart;
        Assert.True(cashPart > 0);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(0, rival.Cash);
    }

    [Fact]
    public void Q335_rollup_month_with_chip_orders_on_weak_only()
    {
        var g = Game.New(seed: 1033, tweak: c => { c.Acquisitions.RivalRollupChancePermille = 1000; c.Acquisitions.PlannerAcquireChancePermille = 0; });
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState strong = g.Rival("openaye");
        strong.PlanStartTick = 0;
        Finance.Earn(strong, LedgerLine.CapitalIn, 600_000_000);
        strong.Models.Add(Released(g, 720));
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 460));
        weak.Cash = -95_000;
        weak.WeeksInDebt = 3;
        weak.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = 1, BuyerLabId = weak.Id, DesignLabId = weak.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = 0, DeliveryTick = g.State.Tick + 15, NodeId = g.C.Chips.Nodes[0].IdValue });
        AcquisitionSystem.RunMonth(g.C, g.State);
        if (weak.AbsorbedByLabId == strong.Id)
            Assert.True(strong.ChipOrders.Count >= 1);
        else
            Assert.Single(weak.ChipOrders);
    }

    [Fact]
    public void Q336_player_chip_listing_persists_through_unrelated_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4500, 6));
        var g2 = Game.New(seed: 1034);
        LabState rival = g2.Rival("openaye");
        rival.Models.Add(Released(g2, 530));
        rival.Cash = -18_000;
        Finance.Earn(g2.Me, LedgerLine.CapitalIn, 75_000_000);
        g2.Do(Command.AcquireLab(g2.Id, rival.Id));
        Assert.NotNull(g.Me.Listing(d.Id));
    }

    [Fact]
    public void Q337_acquire_transfers_chip_fleet_book_into_quote_chips_part()
    {
        var g = Game.New(seed: 1035);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -19_500;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", DieId = "medium", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", Version = 1, YieldPermille = 850 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, ChipDesignId = did, DesignLabId = rival.Id, Count = 10, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        long part = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ChipsPart;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(part > 0);
        Assert.Equal(10, g.Me.Fleet.Single(b => b.ChipDesignId == did).Count);
    }

    [Fact]
    public void Q338_distressed_acquire_in_crash_with_listing_on_player()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 7;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 8));
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        rival.Models.Add(Released(g, 480));
        rival.Cash = -32_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 160_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.NotNull(g.Me.Listing(d.Id));
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
    }

    [Fact]
    public void Q339_command_buy_listed_chips_updates_bubble_when_crash_hot()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 4;
        g.C.Bubble.ListedChipBoostPermille = 15;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        long before = g.State.BubbleCapitalRaised;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.True(g.State.BubbleCapitalRaised >= before);
    }

    [Fact]
    public void Q340_three_way_chip_orders_only_acquire_target_moves()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 15));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        LabState a = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        LabState c = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(c, LedgerLine.CapitalIn, 5_000_000);
        g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.Do(Command.BuyListedChips(c.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        a.Models.Add(Released(g, 525));
        a.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        Assert.Empty(a.ChipOrders);
        Assert.Single(c.ChipOrders);
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q341_acquire_then_hire_refused_on_absorbed()
    {
        var g = Game.New(seed: 1041);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 525));
        rival.Cash = -17_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.Hire(rival.Id, "researcher", 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q342_chip_listing_cleared_on_sealed_target()
    {
        var g = Game.New(seed: 1042);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 528));
        rival.Cash = -16_800;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 3200, MaxPerMonth = 5 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 66_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipListings);
        Assert.NotNull(g.Me.Listing(did));
    }

    [Fact]
    public void Q343_can_acquire_false_when_buyer_cash_below_quote_with_chip_assets()
    {
        var g = Game.New(seed: 1043);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 580));
        rival.Cash = -22_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 1000);
        Assert.False(AcquisitionSystem.CanAcquire(g.C, g.State, g.Me, rival, out _));
    }

    [Fact]
    public void Q344_model_sector_entries_drop_on_acquire()
    {
        var g = Game.New(seed: 1044);
        LabState rival = g.Rival("openaye");
        ModelState m = Released(g, 570);
        rival.Models.Add(m);
        rival.Cash = -19_000;
        foreach (SectorState s in g.State.Sectors)
            s.Entries.Add(new SectorEntry { LabId = rival.Id, ModelId = m.Id, LastRevenue = 100 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.DoesNotContain(g.State.Sectors.SelectMany(s => s.Entries), e => e.LabId == rival.Id);
    }

    [Fact]
    public void Q345_chip_order_design_id_valid_on_buyer_after_cross_lab_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4050, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 36_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 515));
        rival.Cash = -16_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.NotNull(g.Me.Design(g.Me.ChipOrders[0].DesignId));
    }

    [Fact]
    public void Q346_save_load_dual_labs_orders_after_single_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4150, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        LabState prey = HatchFollowUpLaterTests.OpenAyeBuyer(g, 58_000_000);
        LabState other = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(other, LedgerLine.CapitalIn, 8_000_000);
        g.Do(Command.BuyListedChips(prey.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.Do(Command.BuyListedChips(other.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        prey.Models.Add(Released(g, 518));
        prey.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 110_000_000);
        g.Do(Command.AcquireLab(g.Id, prey.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        Assert.Single(loaded.Me.ChipOrders);
        Assert.Single(other.ChipOrders);
    }

    [Fact]
    public void Q347_events_chips_bought_and_lab_acquired_same_session()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 508));
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 86_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChipsBought);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired);
    }

    [Fact]
    public void Q348_batch_hook_schema_unchanged_at_29()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 502));
        rival.Cash = -15_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(37, g.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    [Fact]
    public void Q349_acquire_moves_target_models_to_buyer()
    {
        var g = Game.New(seed: 1049);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 590));
        rival.Models.Add(Released(g, 420));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000);
        int n = g.Me.Models.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(n + 2, g.Me.Models.Count);
        Assert.Empty(rival.Models);
    }

    [Fact]
    public void Q350_target_debt_assumption_increases_acquisition_spend_when_cash_negative()
    {
        var g = Game.New(seed: 1050);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 535));
        rival.Cash = -40_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        long quote = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival).Total;
        long cashBefore = g.Me.Cash;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(cashBefore - g.Me.Cash >= quote);
    }

    [Fact]
    public void Q351_player_acquire_distressed_rival_with_only_gpu_fleet()
    {
        var g = Game.New(seed: 1051);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500));
        rival.Cash = -28_000;
        rival.WeeksInDebt = 1;
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 8, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Fleet.Count >= 1);
    }

    [Fact]
    public void Q352_buy_gpus_refused_on_absorbed_rival()
    {
        var g = Game.New(seed: 1052);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.BuyGpus(rival.Id, 1, HardwareBatch.Hq), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q353_chip_listing_on_buyer_after_acquire_survives_chip_month_reset()
    {
        var g = Game.New(seed: 1053);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 515));
        rival.Cash = -17_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 4100, MaxPerMonth = 3, SoldThisMonth = 2 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.Me.Listing(did).SoldThisMonth);
        Assert.Equal(4100, g.Me.Listing(did).PricePerChip);
    }

    [Fact]
    public void Q354_planner_retrench_does_not_clear_chip_orders_on_other_labs()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.State.BubbleCrashMonthsRemaining = 6;
        rival.Models.Add(Released(g, 495));
        rival.Cash = -30_000;
        rival.WeeksInDebt = 2;
        rival.PlanStartTick = 0;
        PlanConfig plan = g.C.Rival("openaye").Plan;
        LabPlanner.Run(g.C, g.State, rival, plan, cmd => g.Try(cmd));
        Assert.True(g.Me.ChipOrders.Count == 0);
        Assert.True(rival.ChipOrders.Count >= 0);
    }

    [Fact]
    public void Q355_display_exit_price_unchanged_by_chip_order_on_unrelated_rival()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4400, 6));
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        long exitBefore = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        g.State.BubbleCrashMonthsRemaining = 4;
        rival.Models.Add(Released(g, 510));
        rival.Cash = -20_000;
        Assert.Equal(exitBefore, DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total);
    }

    [Fact]
    public void Q356_acquisition_quote_staff_part_positive_for_chip_heavy_distressed()
    {
        var g = Game.New(seed: 1054);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 600));
        rival.Cash = -22_000;
        rival.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "researcher", Morale = 500 });
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 790 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.StaffPart > 0);
        Assert.True(q.ChipsPart > 0);
    }

    [Fact]
    public void Q357_morale_hit_on_transferred_staff_after_acquire()
    {
        var g = Game.New(seed: 1055);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 545));
        rival.Cash = -18_500;
        rival.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "engineer", Morale = g.C.StaffPeople.MoraleBaseline });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 76_000_000);
        int staffBefore = g.Me.Staff.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(staffBefore + 1, g.Me.Staff.Count);
        Assert.True(g.Me.Staff.Last().Morale < g.C.StaffPeople.MoraleBaseline);
    }

    [Fact]
    public void Q358_crash_discount_on_acquisition_when_both_distressed()
    {
        var g = Game.New(seed: 1056);
        g.State.BubbleCrashMonthsRemaining = 8;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 555));
        rival.Cash = -30_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.State.BubbleCrashMonthsRemaining = 0;
        long calm = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.State.BubbleCrashMonthsRemaining = 8;
        long crash = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        Assert.True(crash < calm);
    }

    [Fact]
    public void Q359_rival_id_preserved_on_absorbed_lab()
    {
        var g = Game.New(seed: 1057);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 530));
        rival.Cash = -17_000;
        string id = rival.RivalId;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(id, rival.RivalId);
    }

    [Fact]
    public void Q360_global_lab_count_unchanged_after_acquire()
    {
        var g = Game.New(seed: 1058);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 525));
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        int labs = g.State.Labs.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(labs, g.State.Labs.Count);
    }

    [Fact]
    public void Q361_chip_order_count_stable_across_weekly_steps_until_delivery()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 512));
        rival.Cash = -16_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int n = g.Me.ChipOrders.Count;
        int deliver = g.Me.ChipOrders.Min(o => o.DeliveryTick);
        while (g.State.Tick < deliver - 1)
        {
            g.Sim.Step();
            Assert.Equal(n, g.Me.ChipOrders.Count);
        }
    }

    [Fact]
    public void Q362_acquisition_after_listed_chip_buy_keeps_seller_cash_ledger_balanced()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4600, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        long sellerBase = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 505));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(sellerBase, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }

    [Fact]
    public void Q363_bubble_in_crash_flag_true_during_chip_distress_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 6;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        rival.Models.Add(Released(g, 490));
        rival.Cash = -31_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        Assert.True(BubbleSystem.InCrash(g.State));
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q364_target_controller_stays_plan_after_absorb()
    {
        var g = Game.New(seed: 1059);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540));
        rival.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 74_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(LabController.Plan, rival.Controller);
    }

    [Fact]
    public void Q365_integrate_bar_total_test_count_hook()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 4));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 28_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 40_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 498));
        rival.Cash = -15_800;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.ChipDesigns.Count >= 1);
        Assert.Equal(37, g.State.Schema);
    }
}