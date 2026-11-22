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

/// <summary>Q202–Q214: Hatch economy bar after Q201 (M&amp;A arbitrage, bubble step, chips in deals).</summary>
public class HatchEconomyBarLaterTests
{
    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    static ModelState Released(Game g, int cap) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = "Bot"
    };

    static void StepMonthsWithPlans(Game g, int months)
    {
        int stepped = 0;
        while (stepped < months)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick))
            {
                g.Sim.RunPlan(g.Id, g.C.Autopilot);
                stepped++;
            }
            g.Sim.Step();
        }
    }

    [Fact]
    public void Q202_acquired_gpus_cannot_be_sold_until_integration_lock_expires()
    {
        var g = Game.New(seed: 901, tweak: c => c.Acquisitions.FleetResaleLockWeeks = 8);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 600));
        rival.Cash = -40_000;
        rival.Fleet.Clear();
        rival.Fleet.Add(new HardwareBatch
        {
            Id = g.State.TakeId(),
            HoldingId = HardwareBatch.Hq,
            GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id,
            ChipDesignId = -1,
            Count = 40,
            ArrivedTick = 0
        });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        long cash = g.Me.Cash;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Cash < cash);
        g.Refused(Command.SellGpus(g.Id, 10), CommandError.TargetNotEligible);
        g.Weeks(8);
        g.Do(Command.SellGpus(g.Id, 10));
    }

    [Fact]
    public void Q202_save_load_preserves_resale_lock_on_transferred_fleet()
    {
        var g = Game.New(seed: 902, tweak: c => c.Acquisitions.FleetResaleLockWeeks = 20);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500));
        rival.Cash = -30_000;
        rival.Fleet.Add(new HardwareBatch { Id = 99, HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, ChipDesignId = -1, Count = 12, ArrivedTick = 0 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int lockTick = g.Me.Fleet[0].ResaleLockedUntilTick;
        Assert.True(lockTick > g.Tick);
        string save = g.Sim.Save();
        var g2 = new Game(Simulation.Load(g.C, save));
        Assert.Equal(lockTick, g2.Me.Fleet[0].ResaleLockedUntilTick);
        SaveSchemaPretend.AssertRoundTrip(g2.Sim);
    }

    [Fact]
    public void Q203_hot_bubble_cools_after_strong_market_month_via_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 500;
            c.Bubble.ReturnCoolingPermille = 40;
            c.Bubble.GraceMonthsBeforeCrash = 12;
            c.Bubble.ReturnsNeededPermille = 50;
        });
        g.State.BubblePermille = 700;
        g.State.BubbleCapitalRaised = 1_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 500_000;
        int before = g.State.BubblePermille;
        StepMonthEnd(g);
        Assert.True(g.State.BubblePermille < before);
        Assert.Equal(0, g.State.BubbleGraceMonths);
    }

    [Fact]
    public void Q204_market_leader_skips_weak_haircut_when_crash_pops_via_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 400;
            c.Bubble.SurvivorRevenuePpm = 200_000;
            c.Bubble.WeakCashHaircutPermille = 200;
        });
        g.State.BubblePermille = 450;
        g.Me.Ledger.ThisMonth.Revenue = 10_000_000;
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Equal(0, g.Me.Ledger.ThisMonth.Fines);
    }

    [Fact]
    public void Q205_ready_chip_design_raises_acquisition_quote()
    {
        var g = Game.New(seed: 905);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 550));
        rival.Cash = -20_000;
        ChipNodeConfig node = g.C.Chips.Nodes[0];
        rival.ChipDesigns.Add(new ChipDesign
        {
            Id = g.State.TakeId(),
            Status = ChipDesignStatus.Ready,
            NodeId = node.Id,
            Target = "inference",
            Version = 1,
            YieldPermille = 800
        });
        var bare = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        rival.ChipDesigns.Clear();
        var noChips = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        Assert.True(bare.ChipsPart > 0);
        Assert.True(bare.Total > noChips.Total);
    }

    [Fact]
    public void Q206_acquire_transfers_chip_designs_to_buyer()
    {
        var g = Game.New(seed: 906);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500));
        rival.Cash = -25_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 2, YieldPermille = 700 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 60_000_000);
        int before = g.Me.ChipDesigns.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(before + 1, g.Me.ChipDesigns.Count);
        Assert.Contains(g.Me.ChipDesigns, d => d.Id == designId);
    }

    [Fact]
    public void Q207_planner_acquire_default_is_conservative_after_q201_bar()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Acquisitions.PlannerAcquireChancePermille, 1, 80);
    }

    [Fact]
    public void Q207_ten_seeds_twelve_years_in_crash_no_mass_plan_bankruptcies_with_planner_acquire()
    {
        var bankrupts = new List<string>();
        for (long seed = 1; seed <= 10; seed++)
        {
            var g = new Game(Simulation.NewGame(TestData.Config(c =>
            {
                c.Acquisitions.PlannerAcquireChancePermille = 35;
                c.Acquisitions.RivalRollupChancePermille = 0;
            }), seed));
            g.State.BubbleCrashMonthsRemaining = 36;
            StepMonthsWithPlans(g, 8 * g.C.Calendar.MonthsPerYear);
            foreach (LabState lab in g.State.Labs)
            {
                if (lab.Controller != LabController.Plan) continue;
                if (lab.Bankrupt) bankrupts.Add("seed " + seed + ": " + lab.Name);
            }
        }
        Assert.True(bankrupts.Count <= 3, string.Join("; ", bankrupts));
    }

    [Fact]
    public void Q208_acquire_spends_quote_total_on_acquisition_ledger()
    {
        var g = Game.New(seed: 908);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 650));
        rival.Cash = -35_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 90_000_000);
        long before = g.Me.Cash;
        var quote = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(-(quote.Total + quote.DebtAssumed), g.Me.Ledger.ThisMonth.Acquisition);
        Assert.Equal(before - g.Me.Cash, -g.Me.Ledger.ThisMonth.Acquisition);
        Assert.Equal(0, rival.Cash);
    }

    [Fact]
    public void Q209_crash_month_end_keeps_plans_running_without_exception()
    {
        var g = Game.New(seed: 909, tweak: c => c.Acquisitions.RivalRollupChancePermille = 800);
        g.State.BubbleCrashMonthsRemaining = 6;
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Controller != LabController.Plan) continue;
            lab.Cash = Math.Max(lab.Cash / 4, 1);
        }
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Contains(g.State.Labs, l => l.Controller == LabController.Plan);
    }

    [Fact]
    public void Q210_chip_fleet_batches_raise_acquisition_chips_part()
    {
        var g = Game.New(seed: 910);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 520));
        rival.Cash = -15_000;
        int designId = g.State.TakeId();
        var design = new ChipDesign
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
        };
        rival.ChipDesigns.Add(design);
        rival.Fleet.Add(new HardwareBatch
        {
            Id = g.State.TakeId(),
            HoldingId = HardwareBatch.Hq,
            ChipDesignId = designId,
            DesignLabId = rival.Id,
            Count = 20,
            ArrivedTick = 0
        });
        var withFleet = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        rival.Fleet.Clear();
        var without = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        Assert.True(withFleet.ChipsPart > without.ChipsPart);
    }

    [Fact]
    public void Q211_acquire_moves_chip_listings_to_buyer()
    {
        var g = Game.New(seed: 911);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 500));
        rival.Cash = -20_000;
        int designId = g.State.TakeId();
        rival.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, NodeId = g.C.Chips.Nodes[0].Id, Target = "inference", Version = 1, YieldPermille = 750 });
        rival.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 5000, MaxPerMonth = 8 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 70_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Empty(rival.ChipListings);
        Assert.Contains(g.Me.ChipListings, l => l.DesignId == designId);
    }

    [Fact]
    public void Q212_planner_acquire_skipped_when_buyer_is_distressed()
    {
        var g = Game.New(seed: 912, tweak: c => c.Acquisitions.PlannerAcquireChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 8;
        LabState buyer = g.Rival("openaye");
        buyer.Cash = -50_000;
        buyer.Models.Add(Released(g, 450));
        LabState target = g.Rival("anthoptics");
        target.Models.Add(Released(g, 400));
        target.Cash = -80_000;
        int labsBefore = g.State.Labs.Count(l => l.AbsorbedByLabId < 0);
        g.Sim.RunPlan(buyer.Id, g.C.Rival("openaye").Plan);
        Assert.Equal(labsBefore, g.State.Labs.Count(l => l.AbsorbedByLabId < 0));
    }

    [Fact]
    public void Q213_schema_29_migration_preserves_resale_lock_on_transferred_batch()
    {
        var g = Game.New(seed: 913);
        g.Me.Fleet.Add(new HardwareBatch
        {
            Id = 5,
            HoldingId = HardwareBatch.Hq,
            GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id,
            ChipDesignId = -1,
            Count = 4,
            ArrivedTick = 0
        });
        string json = SaveSchemaPretend.At(g.C, 28, g.Sim.Save());
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(29, loaded.State.Schema);
        Assert.Equal(-1, loaded.Me.Fleet[0].ResaleLockedUntilTick);
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
    }

    [Fact]
    public void Q214_target_positive_cash_transfers_into_buyer_on_acquire()
    {
        var g = Game.New(seed: 914);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 600));
        rival.Cash = 120_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        var quote = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(0, rival.Cash);
        Assert.True(quote.CashPart > 0);
        Assert.Equal(-quote.Total, g.Me.Ledger.ThisMonth.Acquisition);
    }
}