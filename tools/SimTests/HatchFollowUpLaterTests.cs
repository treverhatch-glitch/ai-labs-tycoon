using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q154–Q159 hatch follow-ups: main path, edges, save/load, and cross-system via Simulation.Step where it matters.</summary>
public class HatchFollowUpLaterTests
{
    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    static void Distressed(LabState lab, Game g)
    {
        lab.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 500,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "DistressedModel"
        });
        lab.Cash = -80_000;
        lab.Ledger.ThisMonth.Revenue = 1;
    }

    static ModelState ReleasedForSale(Game g, int cap, string name, string line = "mini") => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = line,
        Name = name,
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
    };

    static void SectorRevenue(Game g, LabState lab, ModelState model, long week)
    {
        for (int s = 0; s < g.State.Sectors.Count; s++)
        {
            SectorState sector = g.State.Sectors[s];
            for (int i = 0; i < sector.Entries.Count; i++)
            {
                SectorEntry e = sector.Entries[i];
                if (e.LabId == lab.Id && e.ModelId == model.Id) e.LastRevenue = week;
            }
        }
    }

    static Game ChipListingSeller(long sellerCash = 50_000_000)
    {
        var options = new NewGameOptions { Seed = 88, StartEraId = "punchline" };
        options.RivalIds = new System.Collections.Generic.List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DominanceFromPpm = MathI.Ppm), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, sellerCash);
        foreach (string tech in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling" })
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        return g;
    }

    static ChipDesign ReadyInferenceListing(Game g)
    {
        ChipSpec spec = ChipQueries.Derive(g.C, "inference", "n28", "medium", "fp16", "ddr", "air");
        var d = new ChipDesign();
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(g.Me);
        d.Name = g.Me.FamilyName + " Silicon " + d.Version;
        d.Target = "inference";
        d.NodeId = "n28";
        d.DieId = "medium";
        d.FormatId = "fp16";
        d.MemoryId = "ddr";
        d.CoolingId = "air";
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = 850;
        d.StageEndsTick = -1;
        d.ReadyTick = g.Tick;
        g.Me.ChipDesigns.Add(d);
        if (g.Me.SoftwarePermille == 0) g.Me.SoftwarePermille = g.C.Chips.SoftwareStart;
        return d;
    }

    static LabState OpenAyeBuyer(Game g, long cash)
    {
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("openaye"), EraSystem.AtTick(g.C, g.Tick), true);
        buyer.PlanStartTick = 0;
        Finance.Earn(buyer, LedgerLine.CapitalIn, cash);
        return buyer;
    }

    static LabState EnsureRival(Game g, string rivalId, bool arrival = true)
    {
        try { return g.Rival(rivalId); }
        catch (ArgumentException)
        {
            g.State.PendingRivals.Remove(rivalId);
            return WorldSetup.AddRival(g.C, g.State, g.C.Rival(rivalId), EraSystem.AtTick(g.C, g.Tick), arrival);
        }
    }

    // --- Q154 pop before decay ---

    [Fact]
    public void Q154_pop_at_threshold_fires_before_monthly_decay()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 950;
            c.Bubble.DecayPermillePerMonth = 200;
            c.Bubble.GraceMonthsBeforeCrash = 99;
        });
        g.State.BubblePermille = 950;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q154_pop_at_threshold_through_month_end_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 450;
            c.Bubble.DecayPermillePerMonth = 200;
            c.Bubble.GraceMonthsBeforeCrash = 99;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 450;
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q154_pop_at_permille_zero_never_pops_from_level_alone()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 0;
            c.Bubble.DecayPermillePerMonth = 0;
            c.Bubble.GraceMonthsBeforeCrash = 99;
            c.Bubble.HotPermille = 1000;
        });
        g.State.BubbleCapitalRaised = 0;
        g.State.BubblePermille = MathI.Permille;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.False(BubbleSystem.InCrash(g.State));
        Assert.True(g.State.BubblePermille > 900);
    }

    [Fact]
    public void Q154_one_point_below_pop_decays_instead_of_bursting()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.DecayPermillePerMonth = 40;
            c.Bubble.GraceMonthsBeforeCrash = 99;
            c.Bubble.HotPermille = 1000;
        });
        g.State.BubblePermille = 499;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.False(BubbleSystem.InCrash(g.State));
        Assert.Equal(459, g.State.BubblePermille);
    }

    [Fact]
    public void Q154_max_permille_triggers_pop_when_configured()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 950;
            c.Bubble.DecayPermillePerMonth = 5;
            c.Bubble.GraceMonthsBeforeCrash = 99;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = MathI.Permille;
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q154_pop_still_fires_after_prior_crash_residual_bubble()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 200;
            c.Bubble.PostCrashResidualPermille = 180;
            c.Bubble.DecayPermillePerMonth = 0;
            c.Bubble.GraceMonthsBeforeCrash = 99;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleHadCrash = true;
        g.State.BubblePermille = 200;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q154_cross_hot_grace_month_not_consumed_when_pop_fires_first()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 400;
            c.Bubble.HotPermille = 300;
            c.Bubble.GraceMonthsBeforeCrash = 6;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 400;
        g.State.BubbleGraceMonths = 4;
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.True(g.State.BubbleGraceMonths < 4);
    }

    // --- Q155 closed-month market revenue display ---

    [Fact]
    public void Q155_market_revenue_last_closed_ignores_open_month_tally()
    {
        var g = Game.Solo();
        LabState lab = g.Me;
        lab.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 5_000_000 } });
        lab.Ledger.ThisMonth.Revenue = 99;
        Assert.Equal(5_000_000, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
        Assert.Equal(5_000_000, DisplayQueries.BubbleMeterParts(g.C, g.State).MarketRevenueLastMonth);
    }

    [Fact]
    public void Q155_no_history_falls_back_to_this_month_revenue()
    {
        var g = Game.Solo();
        g.Me.Ledger.ThisMonth.Revenue = 2_500_000;
        Assert.Equal(2_500_000, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
    }

    [Fact]
    public void Q155_bankrupt_labs_excluded_from_market_revenue_sum()
    {
        var g = Game.New(seed: 52);
        g.Me.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 9_000_000 } });
        g.Me.Bankrupt = true;
        LabState rival = g.Rival("openaye");
        rival.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 1_000_000 } });
        Assert.Equal(1_000_000, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
    }

    [Fact]
    public void Q155_multi_lab_sums_last_closed_histories()
    {
        var g = Game.New(seed: 51);
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (!LabQueries.ParticipatesInEconomy(lab)) continue;
            lab.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 100_000 * (lab.Id + 1) } });
            lab.Ledger.ThisMonth.Revenue = 1;
        }
        long expected = g.State.Labs.Where(LabQueries.ParticipatesInEconomy).Sum(l => 100_000L * (l.Id + 1));
        Assert.Equal(expected, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
    }

    [Fact]
    public void Q155_month_end_step_roll_moves_closed_revenue_into_history()
    {
        var g = Game.Solo(tweak: c => c.Bubble.DecayPermillePerMonth = 0);
        Finance.Earn(g.Me, LedgerLine.Revenue, 3_000_000);
        long open = g.Me.Ledger.ThisMonth.Revenue;
        StepMonthEnd(g);
        Assert.True(g.Me.Ledger.History.Count > 0);
        Assert.Equal(3_000_000, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
        Assert.True(DisplayQueries.BubbleMeterParts(g.C, g.State).MarketRevenueLastMonth >= open);
    }

    [Fact]
    public void Q155_save_load_preserves_closed_month_revenue_derived_query()
    {
        var g = Game.Solo();
        g.Me.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 4_200_000 } });
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.Equal(4_200_000, BubbleSystem.MarketRevenueLastClosedMonth(g.State));
    }

    [Fact]
    public void Q155_cross_bubble_meter_parts_match_query_after_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 800;
            c.Bubble.DecayPermillePerMonth = 0;
        });
        g.Me.Ledger.History.Add(new MonthRecord { Totals = new LedgerTotals { Revenue = 800_000 } });
        StepMonthEnd(g);
        long closed = BubbleSystem.MarketRevenueLastClosedMonth(g.State);
        Assert.Equal(closed, DisplayQueries.BubbleMeterParts(g.C, g.State).MarketRevenueLastMonth);
    }

    // --- Q156 pop warning margin config ---

    [Fact]
    public void Q156_warning_margin_comes_from_config()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.PopWarningMarginPermille = 20;
            c.Bubble.HotPermille = 1000;
        });
        g.State.BubblePermille = 485;
        Assert.Equal(2, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Q156_tight_margin_only_warns_near_pop()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 600;
            c.Bubble.PopWarningMarginPermille = 1;
            c.Bubble.HotPermille = 1000;
        });
        g.State.BubblePermille = 598;
        Assert.Equal(0, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
        g.State.BubblePermille = 599;
        Assert.Equal(2, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Q156_large_margin_warns_earlier()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.PopWarningMarginPermille = 120;
            c.Bubble.HotPermille = 200;
        });
        g.State.BubblePermille = 400;
        Assert.Equal(2, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Q156_hot_permille_sets_level_one_outside_pop_margin()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 900;
            c.Bubble.PopWarningMarginPermille = 10;
            c.Bubble.HotPermille = 400;
        });
        g.State.BubblePermille = 410;
        Assert.Equal(1, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Q156_in_crash_warning_is_always_level_three()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 3;
        g.State.BubblePermille = 100;
        Assert.Equal(3, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Q156_cross_warning_updates_after_month_end_pop_via_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 420;
            c.Bubble.PopWarningMarginPermille = 30;
            c.Bubble.HotPermille = 800;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 420;
        StepMonthEnd(g);
        Assert.Equal(3, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    // --- Q157 rival rollup config and ordering ---

    [Fact]
    public void Q157_rollup_skipped_when_chance_permille_is_zero()
    {
        var g = Game.New(seed: 44, tweak: c =>
        {
            c.Acquisitions.RivalRollupChancePermille = 0;
            c.Bubble.CrashDurationMonths = 12;
        });
        g.State.BubbleCrashMonthsRemaining = 6;
        foreach (LabState lab in g.State.Labs)
        {
            lab.AbsorbedByLabId = -1;
            if (lab.Controller == LabController.Plan && !lab.Bankrupt) lab.Cash = 50_000;
            if (lab.Controller == LabController.Plan) Distressed(lab, g);
        }
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.All(g.State.Labs, lab => Assert.Equal(-1, lab.AbsorbedByLabId));
    }

    [Fact]
    public void Q157_rollup_always_when_chance_is_max()
    {
        var g = Game.New(seed: 55, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 4;
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        Distressed(target, g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 80_000_000);
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(buyer.Id, target.AbsorbedByLabId);
    }

    [Fact]
    public void Q157_bankrupt_buyer_never_acquires()
    {
        var g = Game.New(seed: 56, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState buyer = g.Rival("openaye");
        buyer.Bankrupt = true;
        LabState target = g.Rival("anthoptics");
        Distressed(target, g);
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(-1, target.AbsorbedByLabId);
    }

    [Fact]
    public void Q157_richest_rival_buyer_tries_first()
    {
        var g = Game.New(seed: 57, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        g.State.BubbleCrashMonthsRemaining = 8;
        LabState poor = g.Rival("openaye");
        LabState rich = g.Rival("anthoptics");
        LabState target = EnsureRival(g, "macrosoft");
        Distressed(target, g);
        poor.Cash = 2_000_000;
        rich.Cash = 90_000_000;
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(rich.Id, target.AbsorbedByLabId);
    }

    [Fact]
    public void Q157_weakest_distressed_target_acquired_first()
    {
        var g = Game.New(seed: 58, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState buyer = g.Rival("openaye");
        Finance.Earn(buyer, LedgerLine.CapitalIn, 120_000_000);
        LabState deep = g.Rival("anthoptics");
        LabState shallow = EnsureRival(g, "macrosoft");
        Distressed(deep, g);
        Distressed(shallow, g);
        deep.Cash = -400_000;
        shallow.Cash = -40_000;
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(buyer.Id, deep.AbsorbedByLabId);
        Assert.Equal(-1, shallow.AbsorbedByLabId);
    }

    [Fact]
    public void Q157_month_end_step_can_trigger_rival_rollup()
    {
        var g = Game.New(seed: 59, tweak: c =>
        {
            c.Acquisitions.RivalRollupChancePermille = 1000;
            c.Bubble.CrashDurationMonths = 12;
            c.Bubble.PopAtPermille = 1000;
        });
        g.State.BubbleCrashMonthsRemaining = 10;
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        Distressed(target, g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 100_000_000);
        StepMonthEnd(g);
        Assert.Equal(buyer.Id, target.AbsorbedByLabId);
    }

    [Fact]
    public void Q157_save_load_preserves_absorption_after_rollup()
    {
        var g = Game.New(seed: 60, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 7;
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        Distressed(target, g);
        Finance.Earn(buyer, LedgerLine.CapitalIn, 50_000_000);
        AcquisitionSystem.Apply(g.C, g.State, buyer, target);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.Equal(buyer.Id, target.AbsorbedByLabId);
    }

    // --- Q158 listed-chip trade and bubble capital ---

    [Fact]
    public void Q158_listed_chip_trade_adds_trade_cash_to_capital_raised()
    {
        var g = Game.Solo(tweak: c => c.Bubble.ListedChipBoostPermille = 0);
        long before = g.State.BubbleCapitalRaised;
        BubbleSystem.OnListedChipTrade(g.C, g.State, 2_500_000);
        Assert.Equal(before + 2_500_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q158_zero_trade_cash_skips_capital_but_can_still_boost_meter()
    {
        var g = Game.Solo(tweak: c => c.Bubble.ListedChipBoostPermille = 35);
        long cap = g.State.BubbleCapitalRaised;
        int meter = g.State.BubblePermille;
        BubbleSystem.OnListedChipTrade(g.C, g.State, 0);
        Assert.Equal(cap, g.State.BubbleCapitalRaised);
        Assert.Equal(meter + 35, g.State.BubblePermille);
    }

    [Fact]
    public void Q158_buy_listed_chips_command_raises_capital_by_price()
    {
        Game g = ChipListingSeller();
        ChipDesign d = ReadyInferenceListing(g);
        LabState buyer = OpenAyeBuyer(g, 500_000);
        long capBefore = g.State.BubbleCapitalRaised;
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 100));
        g.Me.Cash = 15_000_000;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 5));
        Assert.Equal(capBefore + 20_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q158_save_load_preserves_bubble_capital_raised()
    {
        var g = Game.Solo();
        g.State.BubbleCapitalRaised = 77_000_000;
        BubbleSystem.OnListedChipTrade(g.C, g.State, 1_000_000);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.Equal(78_000_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q158_funding_raise_and_chip_trade_both_accumulate_capital()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
        });
        FundingRoundConfig round = g.C.Funding.Round("angel");
        long cap = g.State.BubbleCapitalRaised;
        BubbleSystem.OnFundingRaise(g.C, g.State, round, 5_000_000);
        BubbleSystem.OnListedChipTrade(g.C, g.State, 800_000);
        Assert.True(g.State.BubbleCapitalRaised >= cap + 5_000_000 + 800_000);
    }

    [Fact]
    public void Q158_bankrupt_seller_blocks_listed_chip_buy()
    {
        Game g = ChipListingSeller();
        ChipDesign d = ReadyInferenceListing(g);
        LabState buyer = OpenAyeBuyer(g, 500_000);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 100));
        g.Me.Bankrupt = true;
        long capBefore = g.State.BubbleCapitalRaised;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 1), CommandError.LabBankrupt);
        Assert.Equal(capBefore, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q158_cross_chip_buy_during_hot_bubble_moves_meter_and_capital()
    {
        Game g = ChipListingSeller();
        g.C.Bubble.ListedChipBoostPermille = 20;
        ChipDesign d = ReadyInferenceListing(g);
        LabState buyer = OpenAyeBuyer(g, 250_000);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 20));
        g.Me.Cash = 20_000_000;
        int meter = g.State.BubblePermille;
        long cap = g.State.BubbleCapitalRaised;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 4));
        Assert.Equal(cap + 20_000, g.State.BubbleCapitalRaised);
        Assert.Equal(meter + 20, g.State.BubblePermille);
    }

    // --- Q159 model-sale income smoothing ---

    [Fact]
    public void Q159_quote_smooths_toward_lab_last_week_revenue_share()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 1000);
        LabState lab = g.Me;
        ModelState spare = ReleasedForSale(g, 400, "Spare");
        ModelState model = ReleasedForSale(g, 500, "Cow");
        lab.Models.Add(spare);
        lab.Models.Add(model);
        g.Me.Ledger.LastWeek.Revenue = 400_000;
        SectorRevenue(g, lab, model, 10_000);
        var q = ModelSalesSystem.Quote(g.C, g.State, lab, model);
        Assert.True(q.WeeklyIncome > 10_000);
    }

    [Fact]
    public void Q159_zero_smoothing_uses_raw_sector_week()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 0);
        ModelState m = ReleasedForSale(g, 500, "Raw");
        ModelState spare = ReleasedForSale(g, 400, "Spare");
        g.Me.Models.Add(m);
        g.Me.Models.Add(spare);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Me.LastWeek.ServingCompute = 1_000_000_000;
        MarketSystem.RunWeek(g.C, g.State);
        g.Me.Ledger.LastWeek.Revenue = 900_000;
        var q = ModelSalesSystem.Quote(g.C, g.State, g.Me, m);
        Assert.True(q.WeeklyIncome > 0);
        Assert.Equal(q.WeeklyIncome, ModelSalesSystem.WeeklyIncome(g.State, g.Me, m));
    }

    [Fact]
    public void Q159_full_smoothing_uses_lab_week_when_sectors_are_cold()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 1000);
        ModelState a = ReleasedForSale(g, 500, "A");
        ModelState b = ReleasedForSale(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        SectorRevenue(g, g.Me, a, 0);
        SectorRevenue(g, g.Me, b, 0);
        long smooth = ModelSalesSystem.SmoothedWeeklyIncome(g.C, g.State, g.Me, a);
        Assert.True(smooth >= 40_000);
    }

    [Fact]
    public void Q159_profitable_offer_uses_smoothed_weekly_income()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.ModelSales.IncomeSmoothingPermille = 800;
            c.ModelSales.EarningsMultipleWeeks = 10;
        });
        ModelState a = ReleasedForSale(g, 500, "A");
        ModelState b = ReleasedForSale(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Me.Ledger.LastWeek.Revenue = 500_000;
        SectorRevenue(g, g.Me, a, 5_000);
        var raw = ModelSalesSystem.Quote(g.C, g.State, g.Me, a);
        g.C.ModelSales.IncomeSmoothingPermille = 0;
        var nosmooth = ModelSalesSystem.Quote(g.C, g.State, g.Me, a);
        Assert.True(raw.OfferCash > nosmooth.OfferCash);
        Assert.Equal(raw.WeeklyIncome * 10, raw.OfferCash);
    }

    [Fact]
    public void Q159_bankrupt_lab_quote_still_returns_min_offer()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 500);
        ModelState m = ReleasedForSale(g, 400, "Legacy");
        ModelState spare = ReleasedForSale(g, 300, "Spare");
        g.Me.Models.Add(m);
        g.Me.Models.Add(spare);
        g.Me.Bankrupt = true;
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, m);
        Assert.True(q.OfferCash >= g.C.ModelSales.MinOffer);
    }

    [Fact]
    public void Q159_cross_sell_model_via_command_uses_smoothed_quote_cash()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 700);
        ModelState a = ReleasedForSale(g, 600, "A");
        ModelState b = ReleasedForSale(g, 500, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Me.Ledger.LastWeek.Revenue = 300_000;
        SectorRevenue(g, g.Me, a, 8_000);
        long cash = g.Me.Cash;
        var quote = ModelSalesSystem.Quote(g.C, g.State, g.Me, a);
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.Equal(cash + quote.OfferCash, g.Me.Cash);
    }

    [Fact]
    public void Q159_save_load_model_smoothing_config_unchanged_quote()
    {
        var g = Game.Solo(tweak: c => c.ModelSales.IncomeSmoothingPermille = 600);
        ModelState a = ReleasedForSale(g, 500, "A");
        ModelState b = ReleasedForSale(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Me.Ledger.LastWeek.Revenue = 200_000;
        SectorRevenue(g, g.Me, a, 6_000);
        long before = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        long after = ModelSalesSystem.Quote(g.C, g.State, g.Me, a).OfferCash;
        Assert.Equal(before, after);
    }
}