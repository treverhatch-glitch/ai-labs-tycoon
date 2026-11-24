using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q293–Q330: crash M&amp;A × chip planner, vendor, cloud cross-bars (no save schema).</summary>
public class HatchRivalsChipsCrossBarLaterTests
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

    static void RunPlanner(Game g, LabState lab, string rivalId)
    {
        PlanConfig plan = g.C.Rival(rivalId).Plan;
        LabPlanner.Run(g.C, g.State, lab, plan, cmd => g.Try(cmd));
    }

    [Fact]
    public void Q293_acquire_rent_only_rival_keeps_cloud_holding_on_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4800, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 80_000_000);
        Assert.Empty(rival.Holdings);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        rival.Models.Add(Released(g, 540));
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ChipOrder o = g.Me.ChipOrders.Single();
        Assert.Equal(HardwareBatch.Cloud, o.HoldingId);
        Assert.Empty(rival.Holdings);
    }

    [Fact]
    public void Q294_openaye_planner_quiet_after_player_absorbs_weak_peer()
    {
        var g = Game.New(seed: 1001, tweak: c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 7;
        LabState openaye = g.Rival("openaye");
        openaye.PlanStartTick = 0;
        Finance.Earn(openaye, LedgerLine.CapitalIn, 400_000_000);
        openaye.Models.Add(Released(g, 680));
        LabState prey = g.Rival("anthoptics");
        prey.Models.Add(Released(g, 450));
        prey.Cash = -90_000;
        prey.WeeksInDebt = 3;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        g.Do(Command.AcquireLab(g.Id, prey.Id));
        RunPlanner(g, openaye, "openaye");
        Assert.DoesNotContain(openaye.Log, e => e.Action == DecisionAction.AcquiredLab);
        Assert.True(prey.AbsorbedByLabId == g.Id);
    }

    [Fact]
    public void Q295_bubble_capital_rises_on_listed_chip_buy_then_acquire_preserves_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 5;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5200, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 70_000_000);
        long cap0 = g.State.BubbleCapitalRaised;
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.True(g.State.BubbleCapitalRaised >= cap0);
        rival.Models.Add(Released(g, 530));
        rival.Cash = -19_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 130_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q296_cannot_acquire_twice_same_distressed_target()
    {
        var g = Game.New(seed: 1002);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500));
        rival.Cash = -25_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Q297_third_rival_buy_after_acquire_does_not_touch_absorbed_lab_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4100, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 55_000_000);
        LabState prey = HatchFollowUpLaterTests.OpenAyeBuyer(g, 60_000_000);
        g.Do(Command.BuyListedChips(prey.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        prey.Models.Add(Released(g, 510));
        prey.Cash = -18_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 100_000_000);
        g.Do(Command.AcquireLab(g.Id, prey.Id));
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 35_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.Empty(prey.ChipOrders);
        Assert.True(g.Me.ChipOrders.Count >= 1);
        Assert.Single(anth.ChipOrders);
    }

    [Fact]
    public void Q298_vendor_standing_on_seller_unchanged_by_acquire_of_buyer_only()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 55_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        int stand = VendorSystem.StandingOf(g.State, g.Me);
        rival.Models.Add(Released(g, 505));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 85_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(stand, VendorSystem.StandingOf(g.State, g.Me));
    }

    [Fact]
    public void Q299_save_load_after_acquire_with_two_cross_lab_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4300, 12));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        LabState a = HatchFollowUpLaterTests.OpenAyeBuyer(g, 65_000_000);
        LabState b = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(b, LedgerLine.CapitalIn, 8_000_000);
        g.Do(Command.BuyListedChips(a.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        g.Do(Command.BuyListedChips(b.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        a.Models.Add(Released(g, 520));
        a.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 140_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        Assert.Single(loaded.Me.ChipOrders);
        Assert.Single(b.ChipOrders);
    }

    [Fact]
    public void Q300_distress_acquire_quote_includes_ready_fleet_not_orders()
    {
        var g = Game.New(seed: 1003);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 560));
        rival.Cash = -30_000;
        rival.WeeksInDebt = 2;
        g.State.BubbleCrashMonthsRemaining = 6;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 800 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, ChipDesignId = designId, DesignLabId = rival.Id, Count = 5, ArrivedTick = 0 });
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = designId, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 3, FoundryMonth = ChipQueries.MonthIndex(g.C, g.State.Tick), DeliveryTick = g.State.Tick + 30, NodeId = g.C.Chips.Nodes[0].IdValue });
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.ChipsPart > 0);
        rival.ChipOrders.Clear();
        var q2 = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.Equal(q.Total, q2.Total);
    }

    [Fact]
    public void Q301_player_bankrupt_refused_acquire_even_with_distressed_chip_rival()
    {
        var g = Game.New(seed: 1004);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 480));
        rival.Cash = -40_000;
        g.Me.Bankrupt = true;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 1);
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q302_acquire_moves_sector_champion_from_target_to_buyer()
    {
        var g = Game.New(seed: 1005);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 650));
        rival.Cash = -18_000;
        foreach (SectorState s in g.State.Sectors) s.ChampionLabId = rival.Id;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.State.Sectors.All(s => s.ChampionLabId == g.Id));
    }

    [Fact]
    public void Q303_player_chip_listing_unchanged_after_unrelated_distress_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 8;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3000, 6));
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 45_000_000);
        rival.Models.Add(Released(g, 470));
        rival.Cash = -32_000;
        rival.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 160_000_000);
        int max = g.Me.Listing(d.Id).MaxPerMonth;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(max, g.Me.Listing(d.Id).MaxPerMonth);
    }

    [Fact]
    public void Q304_order_id_stable_on_acquire_transfer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4200, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        int id = rival.ChipOrders[0].Id;
        rival.Models.Add(Released(g, 512));
        rival.Cash = -16_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.Me.ChipOrders, o => o.Id == id);
    }

    [Fact]
    public void Q305_openaye_plan_does_not_acquire_player()
    {
        var g = Game.New(seed: 1006, tweak: c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState openaye = g.Rival("openaye");
        openaye.PlanStartTick = 0;
        Finance.Earn(openaye, LedgerLine.CapitalIn, 500_000_000);
        openaye.Models.Add(Released(g, 700));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 1000);
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = 2;
        RunPlanner(g, openaye, "openaye");
        Assert.True(g.Me.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q306_chip_listing_price_survives_acquire_on_player_from_target_design()
    {
        var g = Game.New(seed: 1007);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 530));
        rival.Cash = -17_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 6200, MaxPerMonth = 4 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(6200, g.Me.Listing(designId).PricePerChip);
    }

    [Fact]
    public void Q307_target_positive_cash_transfers_on_acquire_with_pending_order()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 525));
        rival.Cash = 75_000;
        long quote = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        long cashBefore = g.Me.Cash;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(0, rival.Cash);
        Assert.True(g.Me.Cash >= cashBefore + 75_000 - quote);
    }

    [Fact]
    public void Q308_acquire_lab_event_subject_is_target_id()
    {
        var g = Game.New(seed: 1008);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 545));
        rival.Cash = -19_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.LabAcquired);
        Assert.Equal(rival.Id, ev.SubjectId);
        Assert.Equal(g.Id, ev.LabId);
    }

    [Fact]
    public void Q309_participates_in_economy_false_for_absorbed_rival_with_fleet()
    {
        var g = Game.New(seed: 1009);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 550));
        rival.Cash = -18_000;
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 4, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.False(LabQueries.ParticipatesInEconomy(rival));
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
    }

    [Fact]
    public void Q310_chip_order_on_acquired_lab_does_not_block_player_commands()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3800, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 30_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 42_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 498));
        rival.Cash = -15_500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Do(Command.SetChipListing(g.Id, d.Id, 3900, 8));
        Assert.Equal(3900, g.Me.Listing(d.Id).PricePerChip);
    }

    [Fact]
    public void Q311_acquire_with_no_chip_assets_still_works_when_rival_chip_rich_peer_exists()
    {
        var g = Game.New(seed: 1010);
        LabState poor = g.Rival("openaye");
        poor.Models.Add(Released(g, 420));
        poor.Cash = -22_000;
        LabState rich = g.Rival("anthoptics");
        rich.Models.Add(Released(g, 600));
        int did = g.State.TakeId();
        rich.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 750 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        g.Do(Command.AcquireLab(g.Id, poor.Id));
        Assert.Equal(g.Id, poor.AbsorbedByLabId);
        Assert.Single(rich.ChipDesigns);
    }

    [Fact]
    public void Q312_rollup_chance_zero_leaves_chip_orders_on_weak_lab()
    {
        var g = Game.New(seed: 1011, tweak: c => c.Acquisitions.RivalRollupChancePermille = 0);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState strong = g.Rival("openaye");
        LabState weak = g.Rival("anthoptics");
        weak.Models.Add(Released(g, 460));
        weak.Cash = -88_000;
        weak.WeeksInDebt = 3;
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        weak.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = 1, BuyerLabId = weak.Id, DesignLabId = weak.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = month, DeliveryTick = g.State.Tick + 20, NodeId = g.C.Chips.Nodes[0].IdValue });
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.True(weak.AbsorbedByLabId < 0);
        Assert.Single(weak.ChipOrders);
    }

    [Fact]
    public void Q313_retrenching_rival_chip_order_survives_until_acquire()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 6;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3700, 10));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 35_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 50_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 485));
        rival.Cash = -28_000;
        rival.WeeksInDebt = 1;
        rival.PlanStartTick = 0;
        RunPlanner(g, rival, "openaye");
        Assert.True(rival.ChipOrders.Count >= 1);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 120_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipOrders);
        Assert.True(g.Me.ChipOrders.Count >= 1);
    }

    [Fact]
    public void Q314_chip_design_version_survives_acquire_transfer()
    {
        var g = Game.New(seed: 1012);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 555));
        rival.Cash = -19_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 7, YieldPermille = 810 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 72_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(7, g.Me.Design(designId).Version);
    }

    [Fact]
    public void Q315_acquire_in_crash_with_only_ready_chips_part_in_quote()
    {
        var g = Game.New(seed: 1013);
        g.State.BubbleCrashMonthsRemaining = 8;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 540));
        rival.Cash = -25_000;
        rival.WeeksInDebt = 2;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 790 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.ModelsPart > 0);
        Assert.True(q.ChipsPart > 0);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.Me.ChipDesigns, x => x.Id == designId);
    }

    [Fact]
    public void Q316_transferred_gpu_and_chip_batches_share_buyer_fleet()
    {
        var g = Game.New(seed: 1014);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 565));
        rival.Cash = -18_500;
        int cid = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = cid, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 780 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 6, ArrivedTick = 0 });
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, ChipDesignId = cid, DesignLabId = rival.Id, Count = 4, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 78_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(2, g.Me.Fleet.Count);
        Assert.Equal(g.Id, g.Me.Fleet.Single(b => b.ChipDesignId == cid).DesignLabId);
    }

    [Fact]
    public void Q317_listed_chip_trade_after_acquire_updates_sold_this_month_on_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5100, 5));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 45_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 58_000_000);
        rival.Models.Add(Released(g, 515));
        rival.Cash = -17_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 95_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        LabState anth = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), true);
        Finance.Earn(anth, LedgerLine.CapitalIn, 28_000_000);
        g.Do(Command.BuyListedChips(anth.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        Assert.Equal(1, g.Me.Listing(d.Id).SoldThisMonth);
    }

    [Fact]
    public void Q318_non_crash_acquire_solvent_rival_with_models_succeeds()
    {
        var g = Game.New(seed: 1015);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 600));
        rival.Models.Add(Released(g, 550));
        rival.Cash = 200_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, g.Me, rival, out string why), why);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
    }

    [Fact]
    public void Q319_acquire_ledger_acquisition_line_matches_negative_quote()
    {
        var g = Game.New(seed: 1016);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 570));
        rival.Cash = 0;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        long total = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival).Total;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(-total, g.Me.Ledger.ThisMonth.Acquisition);
    }

    [Fact]
    public void Q320_weak_target_chip_order_does_not_change_models_part_of_quote()
    {
        var g = Game.New(seed: 1017);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 620));
        rival.Cash = -19_000;
        long models = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ModelsPart;
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = 1, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 5, FoundryMonth = 0, DeliveryTick = g.State.Tick + 40, NodeId = g.C.Chips.Nodes[0].IdValue });
        Assert.Equal(models, DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).ModelsPart);
    }

    [Fact]
    public void Q321_month_step_after_acquire_preserves_buyer_chip_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4050, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 38_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 48_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 508));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 82_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int n = g.Me.ChipOrders.Count;
        for (int i = 0; i < g.C.Calendar.WeeksPerMonth; i++) g.Sim.Step();
        Assert.Equal(n, g.Me.ChipOrders.Count);
    }

    [Fact]
    public void Q322_absorbed_rival_cannot_set_new_chip_listing()
    {
        var g = Game.New(seed: 1018);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 535));
        rival.Cash = -16_500;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 770 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 68_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.SetChipListing(rival.Id, designId, 3000, 4), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q323_player_acquire_then_save_preserves_schema_29()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4150, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 32_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 44_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 502));
        rival.Cash = -15_800;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 76_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(31, loaded.State.Schema);
    }

    [Fact]
    public void Q324_crash_acquire_target_with_chip_listing_and_order()
    {
        var g = Game.New(seed: 1019);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 490));
        rival.Cash = -33_000;
        rival.WeeksInDebt = 2;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 760 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 4400, MaxPerMonth = 3 });
        int month = ChipQueries.MonthIndex(g.C, g.State.Tick);
        rival.ChipOrders.Add(new ChipOrder { Id = g.State.TakeId(), DesignId = designId, BuyerLabId = rival.Id, DesignLabId = rival.Id, HoldingId = HardwareBatch.Cloud, Count = 2, FoundryMonth = month, DeliveryTick = g.State.Tick + 25, NodeId = g.C.Chips.Nodes[0].IdValue });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 150_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Single(g.Me.ChipOrders);
        Assert.NotNull(g.Me.Listing(designId));
    }

    [Fact]
    public void Q325_registrar_skipped_when_crash_acquire_below_threshold()
    {
        var g = Game.New(seed: 1020, tweak: c => c.Acquisitions.RegulatorDealRevenue = 9_999_999_999L);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 400));
        rival.Cash = -12_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Q326_two_distressed_peers_player_can_acquire_both_when_cash_rich()
    {
        var g = Game.New(seed: 1021);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState a = g.Rival("openaye");
        LabState b = g.Rival("anthoptics");
        a.Models.Add(Released(g, 480));
        b.Models.Add(Released(g, 470));
        a.Cash = -20_000;
        b.Cash = -22_000;
        a.WeeksInDebt = 2;
        b.WeeksInDebt = 2;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 500_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        g.Do(Command.AcquireLab(g.Id, b.Id));
        Assert.Equal(g.Id, a.AbsorbedByLabId);
        Assert.Equal(g.Id, b.AbsorbedByLabId);
    }

    [Fact]
    public void Q327_chip_order_buyer_lab_id_is_player_when_acquiring_rival_buyer()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4250, 8));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 52_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 2));
        rival.Models.Add(Released(g, 518));
        rival.Cash = -17_200;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 88_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.All(g.Me.ChipOrders, o => Assert.Equal(g.Id, o.BuyerLabId));
    }

    [Fact]
    public void Q328_bubble_meter_unchanged_by_acquire_alone_without_listing_trade()
    {
        var g = Game.New(seed: 1022);
        g.State.BubblePermille = 600;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520));
        rival.Cash = -18_000;
        int perm = g.State.BubblePermille;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 65_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(perm, g.State.BubblePermille);
    }

    [Fact]
    public void Q329_target_staff_empty_after_seal_still_leaves_buyer_chip_orders()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 3950, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 33_000_000);
        LabState rival = HatchFollowUpLaterTests.OpenAyeBuyer(g, 46_000_000);
        g.Do(Command.BuyListedChips(rival.Id, g.Id, d.Id, HardwareBatch.Cloud, 1));
        rival.Models.Add(Released(g, 505));
        rival.Cash = -16_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.Staff);
        Assert.Empty(rival.Models);
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Q330_cross_bar_suite_hook_participates_economy_after_chip_rich_acquire()
    {
        var g = Game.New(seed: 1023);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 580));
        rival.Cash = -19_000;
        int did = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = did, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 3, YieldPermille = 800 });
        rival.ChipListings.Add(new ChipListing { DesignId = did, PricePerChip = 5500, MaxPerMonth = 4 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 75_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(LabQueries.ParticipatesInEconomy(g.Me));
        Assert.NotNull(g.Me.Listing(did));
        Assert.False(LabQueries.ParticipatesInEconomy(rival));
    }
}