using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q176–Q179: rival distress gameplay (model quotes, fire-sales, chip listings, planner M&amp;A).</summary>
public class HatchRivalDistressLaterTests
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

    static void WeakShare(Game g, LabState lab)
    {
        lab.Cash = -50_000;
        lab.Models.Add(Released(g, 650, "ShareBot"));
        lab.WeeksInDebt = 3;
    }

    static void WeakCashOnly(LabState lab) { lab.Cash = -40_000; }

    static void RunPlanner(Game g, LabState lab, string rivalId)
    {
        PlanConfig plan = g.C.Rival(rivalId).Plan;
        LabPlanner.Run(g.C, g.State, lab, plan, cmd => g.Try(cmd));
    }

    static LabState DistressedVictim(Game g, string rivalId = "deepsneak")
    {
        LabState weak = WorldSetup.AddRival(g.C, g.State, g.C.Rival(rivalId), EraSystem.AtTick(g.C, g.Tick), true);
        weak.PlanStartTick = 0;
        return weak;
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
        var d = new ChipDesign
        {
            Id = g.State.TakeId(),
            Version = 1,
            Name = "Distress Chip",
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
            StageEndsTick = -1,
            ReadyTick = g.Tick
        };
        goggle.ChipDesigns.Add(d);
        if (goggle.SoftwarePermille == 0) goggle.SoftwarePermille = g.C.Chips.SoftwareStart;
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        goggle.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), Count = 24, GpuGenId = gen.Id, HoldingId = HardwareBatch.Hq, ChipDesignId = -1 });
        goggle.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = g.C.Chips.ArchitectRoleId, TraitId = "steady", Morale = 500 });
        goggle.LastWeek.Revenue = 250_000;
        return goggle;
    }

    static Game IsolatedOpenAye(long seed, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string> { "openaye" } };
        GameConfig cfg = TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            tweak?.Invoke(c);
        });
        return new Game(Simulation.NewGame(cfg, options));
    }

    // --- Q176 crash distress model sale quotes ---

    [Fact]
    public void Q176_distressed_crash_quote_below_normal()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.CrashDistressOfferPermille = 650);
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.Me.LastWeek.ServingCompute = 2_000_000_000;
        MarketSystem.RunWeek(g.C, g.State);
        g.Me.Cash = 500_000;
        long normal = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        g.State.BubbleCrashMonthsRemaining = 8;
        WeakShare(g, g.Me);
        long cut = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        Assert.Equal(MathI.MulPermille(normal, 650), cut);
        Assert.True(cut < normal);
    }

    [Fact]
    public void Q176_healthy_giant_keeps_full_quote_in_crash()
    {
        var g = Game.Solo(tweak: c => c.Bubble.SurvivorRevenuePpm = 100_000);
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.State.BubbleCrashMonthsRemaining = 6;
        g.Me.Cash = 80_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 8_000_000;
        long full = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a).OfferCash;
        Assert.True(full > g.C.ModelSales.MinOffer);
    }

    [Fact]
    public void Q176_no_haircut_outside_crash_even_when_distressed()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        WeakShare(g, g.Me);
        g.State.BubbleCrashMonthsRemaining = 0;
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a);
        long week = ModelSalesSystem.WeeklyIncome(g.State, g.Me, a);
        long expected = week > 0 ? week * g.C.ModelSales.EarningsMultipleWeeks : MathI.MulPermille(q.TrainingCostBasis, g.C.ModelSales.UnprofitableSalePermille);
        if (expected < g.C.ModelSales.MinOffer) expected = g.C.ModelSales.MinOffer;
        Assert.Equal(expected, q.OfferCash);
    }

    [Fact]
    public void Q176_sell_books_distress_cash_on_ledger()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.CrashDistressOfferPermille = 600);
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.State.BubbleCrashMonthsRemaining = 5;
        WeakShare(g, g.Me);
        long offer = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        long cash = g.Me.Cash;
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.Equal(cash + offer, g.Me.Cash);
        Assert.True(g.Me.Ledger.ThisWeek.AssetSale >= offer);
    }

    [Fact]
    public void Q176_min_offer_floor_after_haircut()
    {
        var g = Game.Solo(tweak: c => { c.ModelSales.CrashDistressOfferPermille = 50; c.ModelSales.MinOffer = 50_000; });
        ModelState a = Released(g, 300, "Tiny");
        ModelState b = Released(g, 280, "Spare");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.State.BubbleCrashMonthsRemaining = 4;
        WeakShare(g, g.Me);
        Assert.Equal(50_000, ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash);
    }

    [Fact]
    public void Q176_bankrupt_lab_cannot_sell_even_with_crash_haircut()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Me.Bankrupt = true;
        g.State.BubbleCrashMonthsRemaining = 3;
        g.Refused(Command.SellModel(g.Id, a.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q176_cross_save_load_quote_unchanged()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 620, "A");
        ModelState b = Released(g, 380, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.State.BubbleCrashMonthsRemaining = 6;
        WeakShare(g, g.Me);
        long before = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        string save = g.Sim.Save();
        var g2 = new Game(Simulation.Load(g.C, save));
        long after = ModelSalesSystem.Quote(g2.C, g2.State, g2.Me, a).OfferCash;
        Assert.Equal(before, after);
    }

    // --- Q177 rival planner model fire-sales ---

    [Fact]
    public void Q177_planner_sells_weakest_released_model()
    {
        var g = Game.New(seed: 901, tweak: c => c.ModelSales.CrashDistressOfferPermille = 700);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 800, "Strong"));
        rival.Models.Add(Released(g, 450, "Weak"));
        g.State.BubbleCrashMonthsRemaining = 6;
        WeakShare(g, rival);
        long cash = rival.Cash;
        int minBefore = rival.Models.Where(m => m.Status == ModelStatus.Released || m.Status == ModelStatus.Trained).Min(m => m.Capability);
        RunPlanner(g, rival, "openaye");
        Assert.True(rival.Cash > cash);
        Assert.Contains(rival.Log, e => e.Action == DecisionAction.SoldModel);
        int minAfter = rival.Models.Where(m => m.Status == ModelStatus.Released || m.Status == ModelStatus.Trained).Min(m => m.Capability);
        Assert.True(minAfter > minBefore);
    }

    [Fact]
    public void Q177_fire_sale_once_per_quarter()
    {
        var g = Game.New(seed: 902);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 700, "A"));
        rival.Models.Add(Released(g, 500, "B"));
        rival.Models.Add(Released(g, 400, "C"));
        g.State.BubbleCrashMonthsRemaining = 8;
        WeakShare(g, rival);
        RunPlanner(g, rival, "openaye");
        int sold = rival.Log.Count(e => e.Action == DecisionAction.SoldModel);
        for (int i = 0; i < 6; i++) RunPlanner(g, rival, "openaye");
        Assert.Equal(sold, rival.Log.Count(e => e.Action == DecisionAction.SoldModel));
    }

    [Fact]
    public void Q177_skips_when_only_one_active_model()
    {
        var g = Game.New(seed: 903);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 600, "Only"));
        g.State.BubbleCrashMonthsRemaining = 5;
        WeakShare(g, rival);
        RunPlanner(g, rival, "openaye");
        Assert.DoesNotContain(rival.Log, e => e.Action == DecisionAction.SoldModel);
    }

    [Fact]
    public void Q177_no_fire_sale_without_crash()
    {
        var g = Game.New(seed: 904);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 700, "A"));
        rival.Models.Add(Released(g, 500, "B"));
        WeakShare(g, rival);
        g.State.BubbleCrashMonthsRemaining = 0;
        RunPlanner(g, rival, "openaye");
        Assert.DoesNotContain(rival.Log, e => e.Action == DecisionAction.SoldModel);
    }

    [Fact]
    public void Q177_healthy_rival_does_not_fire_sell()
    {
        var g = Game.New(seed: 905, tweak: c => c.Bubble.SurvivorRevenuePpm = 50_000);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 700, "A"));
        rival.Models.Add(Released(g, 500, "B"));
        rival.Cash = 40_000_000;
        rival.Ledger.ThisMonth.Revenue = 4_000_000;
        g.State.BubbleCrashMonthsRemaining = 6;
        RunPlanner(g, rival, "openaye");
        Assert.DoesNotContain(rival.Log, e => e.Action == DecisionAction.SoldModel);
    }

    [Fact]
    public void Q177_simulation_step_planner_fire_sale()
    {
        var g = Game.New(seed: 906);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        rival.Models.Add(Released(g, 720, "A"));
        rival.Models.Add(Released(g, 480, "B"));
        g.State.BubbleCrashMonthsRemaining = 4;
        WeakShare(g, rival);
        g.Sim.Step();
        Assert.Contains(rival.Log, e => e.Action == DecisionAction.SoldModel);
    }

    [Fact]
    public void Q177_keeps_higher_capability_model()
    {
        var g = Game.New(seed: 907);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        ModelState strong = Released(g, 900, "Crown");
        ModelState weak = Released(g, 420, "Tail");
        rival.Models.Add(strong);
        rival.Models.Add(weak);
        g.State.BubbleCrashMonthsRemaining = 7;
        WeakShare(g, rival);
        RunPlanner(g, rival, "openaye");
        Assert.Contains(rival.Models, m => m.Id == strong.Id);
    }

    // --- Q178 distressed chip listing prices ---

    [Fact]
    public void Q178_distress_listing_below_plan_markup()
    {
        var g = Game.New(seed: 981, tweak: c => c.World.Attention.DominanceFromPpm = MathI.Ppm);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        g.State.BubbleCrashMonthsRemaining = 6;
        WeakCashOnly(goggle);
        PlanConfig plan = g.C.Rival("goggle").Plan;
        long full = MathI.MulPermille(ChipQueries.CostPerGoodChip(ChipQueries.Spec(g.C, d), d.YieldPermille), MathI.Permille + plan.ListsChipsMarkupPermille);
        RunPlanner(g, goggle, "goggle");
        ChipListing listing = goggle.Listing(d.Id);
        Assert.NotNull(listing);
        Assert.True(listing.PricePerChip < full);
        Assert.Equal(MathI.MulPermille(ChipQueries.CostPerGoodChip(ChipQueries.Spec(g.C, d), d.YieldPermille), g.C.Chips.DistressListingPricePermille), listing.PricePerChip);
    }

    [Fact]
    public void Q178_listing_log_includes_crash_distress_reason()
    {
        var g = Game.New(seed: 982);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        g.State.BubbleCrashMonthsRemaining = 5;
        WeakCashOnly(goggle);
        RunPlanner(g, goggle, "goggle");
        DecisionEntry e = goggle.Log.LastOrDefault(x => x.Action == DecisionAction.ListedChips);
        Assert.NotNull(e);
        Assert.True(e.Has(ReasonCode.CrashDistress));
    }

    [Fact]
    public void Q178_healthy_lister_keeps_markup_during_crash()
    {
        var g = Game.New(seed: 983);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        goggle.Cash = 90_000_000;
        goggle.Ledger.ThisMonth.Revenue = 9_000_000;
        goggle.LastWeek.Revenue = 90_000_000;
        g.State.BubbleCrashMonthsRemaining = 6;
        int markup = g.C.Rival("goggle").Plan.ListsChipsMarkupPermille;
        RunPlanner(g, goggle, "goggle");
        long expected = MathI.MulPermille(ChipQueries.CostPerGoodChip(ChipQueries.Spec(g.C, d), d.YieldPermille), MathI.Permille + markup);
        Assert.Equal(expected, goggle.Listing(d.Id).PricePerChip);
    }

    [Fact]
    public void Q178_player_buys_distress_listed_chips()
    {
        var g = Game.New(seed: 984);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        g.State.BubbleCrashMonthsRemaining = 4;
        goggle.Cash = 8_000_000;
        goggle.WeeksInDebt = 2;
        RunPlanner(g, goggle, "goggle");
        long price = goggle.Listing(d.Id).PricePerChip;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        long cashBefore = g.Me.Cash;
        g.Do(Command.BuyListedChips(g.Id, goggle.Id, d.Id, HardwareBatch.Cloud, 2));
        Assert.True(g.Me.Cash < cashBefore);
        Assert.True(g.State.BubbleCrashMonthsRemaining > 0);
    }

    [Fact]
    public void Q178_no_distress_price_outside_crash()
    {
        var g = Game.New(seed: 985);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        WeakCashOnly(goggle);
        g.State.BubbleCrashMonthsRemaining = 0;
        RunPlanner(g, goggle, "goggle");
        DecisionEntry e = goggle.Log.LastOrDefault(x => x.Action == DecisionAction.ListedChips);
        if (e != null) Assert.False(e.Has(ReasonCode.CrashDistress));
    }

    [Fact]
    public void Q178_distress_config_zero_disables_override()
    {
        var g = Game.New(seed: 986, tweak: c => c.Chips.DistressListingPricePermille = 0);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        g.State.BubbleCrashMonthsRemaining = 6;
        WeakCashOnly(goggle);
        int markup = g.C.Rival("goggle").Plan.ListsChipsMarkupPermille;
        RunPlanner(g, goggle, "goggle");
        long expected = MathI.MulPermille(ChipQueries.CostPerGoodChip(ChipQueries.Spec(g.C, d), d.YieldPermille), MathI.Permille + markup);
        Assert.Equal(expected, goggle.Listing(d.Id).PricePerChip);
    }

    [Fact]
    public void Q178_cross_listed_trade_raises_bubble_capital_in_crash()
    {
        var g = Game.New(seed: 987);
        LabState goggle = GoggleWithReadyChip(g);
        ChipDesign d = goggle.ChipDesigns[0];
        g.State.BubbleCrashMonthsRemaining = 8;
        goggle.Cash = 8_000_000;
        goggle.WeeksInDebt = 2;
        RunPlanner(g, goggle, "goggle");
        long raised = g.State.BubbleCapitalRaised;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.BuyListedChips(g.Id, goggle.Id, d.Id, HardwareBatch.Cloud, 3));
        Assert.True(g.State.BubbleCapitalRaised > raised);
    }

    // --- Q179 rival planner crash acquisitions ---

    [Fact]
    public void Q179_planner_acquires_distressed_peer()
    {
        var g = IsolatedOpenAye(911, c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState weak = DistressedVictim(g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 600_000_000);
        buyer.Ledger.ThisMonth.Revenue = 12_000_000;
        Assert.False(AcquisitionSystem.IsDistressed(g.C, g.State, buyer));
        weak.Models.Add(Released(g, 650, "W"));
        weak.Models.Add(Released(g, 500, "W2"));
        weak.Cash = -120_000;
        weak.WeeksInDebt = 4;
        g.State.BubbleCrashMonthsRemaining = 6;
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, buyer, weak, out string why), why);
        RunPlanner(g, buyer, "openaye");
        Assert.Equal(buyer.Id, weak.AbsorbedByLabId);
        Assert.Contains(buyer.Log, e => e.Action == DecisionAction.AcquiredLab);
    }

    [Fact]
    public void Q179_distressed_buyer_does_not_acquire()
    {
        var g = IsolatedOpenAye(912, c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState weak = DistressedVictim(g);
        weak.Models.Add(Released(g, 600, "W"));
        weak.Cash = -90_000;
        WeakShare(g, buyer);
        WeakShare(g, weak);
        g.State.BubbleCrashMonthsRemaining = 6;
        RunPlanner(g, buyer, "openaye");
        Assert.True(weak.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q179_skips_when_buyer_cash_below_deal()
    {
        var g = IsolatedOpenAye(913, c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        buyer.Cash = 1_000;
        LabState weak = DistressedVictim(g);
        weak.Models.Add(Released(g, 700, "W"));
        weak.Models.Add(Released(g, 500, "W2"));
        weak.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "engineer", TraitId = "steady", Morale = 500 });
        weak.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), Count = 200, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, HoldingId = HardwareBatch.Hq, ChipDesignId = -1 });
        weak.Cash = -200_000;
        g.State.BubbleCrashMonthsRemaining = 6;
        RunPlanner(g, buyer, "openaye");
        Assert.True(weak.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q179_once_per_quarter_acquire_cap()
    {
        var g = Game.New(seed: 914, tweak: c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        LabState buyer = g.Rival("openaye");
        Finance.Earn(buyer, LedgerLine.CapitalIn, 500_000_000);
        buyer.PlanStartTick = 0;
        foreach (string id in new[] { "deepsneak", "macrosoft", "metta" })
        {
            LabState t = DistressedVictim(g, id);
            t.Models.Add(Released(g, 620, "M" + id));
            t.Cash = -80_000;
        }
        g.State.BubbleCrashMonthsRemaining = 8;
        for (int i = 0; i < 3; i++) RunPlanner(g, buyer, "openaye");
        Assert.True(buyer.Log.Count(e => e.Action == DecisionAction.AcquiredLab) <= 1);
    }

    [Fact]
    public void Q179_zero_chance_disables_planner_acquire()
    {
        var g = IsolatedOpenAye(915, c => c.Acquisitions.PlannerAcquireChancePermille = 0);
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState weak = DistressedVictim(g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 300_000_000);
        weak.Models.Add(Released(g, 640, "W"));
        weak.Cash = -100_000;
        g.State.BubbleCrashMonthsRemaining = 5;
        RunPlanner(g, buyer, "openaye");
        Assert.True(weak.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q179_healthy_target_ignored_in_crash()
    {
        var g = IsolatedOpenAye(916, c => { c.Acquisitions.PlannerAcquireChancePermille = 1000; c.Bubble.SurvivorRevenuePpm = 10_000; });
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState giant = DistressedVictim(g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 400_000_000);
        giant.Models.Add(Released(g, 800, "G"));
        giant.Cash = 60_000_000;
        giant.Ledger.ThisMonth.Revenue = 6_000_000;
        g.State.BubbleCrashMonthsRemaining = 6;
        RunPlanner(g, buyer, "openaye");
        Assert.True(giant.AbsorbedByLabId < 0);
    }

    [Fact]
    public void Q179_cross_step_month_rollup_still_separate()
    {
        var g = IsolatedOpenAye(917, c => { c.Acquisitions.PlannerAcquireChancePermille = 1000; c.Acquisitions.RivalRollupChancePermille = 0; });
        LabState buyer = g.Rival("openaye");
        buyer.PlanStartTick = 0;
        LabState weak = DistressedVictim(g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 250_000_000);
        buyer.Ledger.ThisMonth.Revenue = 8_000_000;
        weak.Models.Add(Released(g, 610, "W"));
        weak.Cash = -95_000;
        g.State.BubbleCrashMonthsRemaining = 4;
        weak.Models.Add(Released(g, 605, "W2"));
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, buyer, weak, out _), "precheck");
        g.Sim.Step();
        Assert.Equal(buyer.Id, weak.AbsorbedByLabId);
        Assert.Contains(buyer.Log, e => e.Action == DecisionAction.AcquiredLab);
    }
}