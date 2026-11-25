using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q172–Q175: Hatch acquisition/bubble gameplay beyond the first bar.</summary>
public class HatchAcquisitionGameplayLaterTests
{
    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    static ModelState Released(Game g, int cap, string name) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = name
    };

    static void DistressedDebt(Game g, LabState lab)
    {
        lab.Models.Add(Released(g, 600, "DebtBot"));
        lab.Cash = -80_000;
    }

    // --- Q172 crash distress uses weakness not blanket InCrash ---

    [Fact]
    public void Q172_healthy_giant_not_distressed_during_crash()
    {
        var g = Game.New(seed: 801, tweak: c => c.Bubble.SurvivorRevenuePpm = 400_000);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState rival = g.Rival("openaye");
        rival.Cash = 50_000_000;
        rival.Ledger.ThisMonth.Revenue = 5_000_000;
        Assert.False(AcquisitionSystem.IsDistressed(g.C, g.State, rival));
    }

    [Fact]
    public void Q172_weak_revenue_share_distressed_in_crash()
    {
        var g = Game.New(seed: 802, tweak: c => c.Bubble.SurvivorRevenuePpm = 800_000);
        g.State.BubbleCrashMonthsRemaining = 4;
        LabState rival = g.Rival("openaye");
        rival.Cash = 10_000_000;
        rival.Ledger.ThisMonth.Revenue = 1;
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (lab.Id != rival.Id) lab.Ledger.ThisMonth.Revenue = 20_000_000;
        }
        Assert.True(AcquisitionSystem.IsDistressed(g.C, g.State, rival));
    }

    [Fact]
    public void Q172_debt_still_distressed_without_crash()
    {
        var g = Game.Solo();
        DistressedDebt(g, g.Me);
        Assert.True(AcquisitionSystem.IsDistressed(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q172_crash_discount_only_when_distressed_quote()
    {
        var g = Game.New(seed: 803);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 700, "Bot"));
        rival.Cash = 20_000_000;
        rival.Ledger.ThisMonth.Revenue = 10_000_000;
        g.State.BubbleCrashMonthsRemaining = 8;
        var weak = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.False(weak.Distressed);
        rival.Cash = -50_000;
        rival.Ledger.ThisMonth.Revenue = 0;
        var debt = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(debt.Distressed);
        Assert.True(debt.Total < weak.Total);
    }

    [Fact]
    public void Q172_player_cannot_acquire_healthy_rival_in_crash()
    {
        var g = Game.New(seed: 804, tweak: c => c.Bubble.SurvivorRevenuePpm = 300_000);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 800, "Strong"));
        rival.Cash = 30_000_000;
        rival.Ledger.ThisMonth.Revenue = 20_000_000;
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (lab.Id != rival.Id) lab.Ledger.ThisMonth.Revenue = 1;
        }
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Q172_rival_rollup_skips_healthy_only_crash_month()
    {
        var g = Game.New(seed: 805, tweak: c =>
        {
            c.Acquisitions.RivalRollupChancePermille = 1000;
            c.Bubble.SurvivorRevenuePpm = 500_000;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        g.State.BubbleCrashMonthsRemaining = 6;
        LabState buyer = g.Rival("openaye");
        buyer.Cash = 200_000_000;
        LabState healthy = g.Rival("anthoptics");
        healthy.Cash = 40_000_000;
        healthy.Ledger.ThisMonth.Revenue = 8_000_000;
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(-1, healthy.AbsorbedByLabId);
    }

    [Fact]
    public void Q172_step_month_end_rollup_only_hits_weak_distressed_target()
    {
        var g = Game.New(seed: 806, tweak: c =>
        {
            c.Acquisitions.RivalRollupChancePermille = 1000;
            c.Bubble.SurvivorRevenuePpm = 600_000;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        g.State.BubbleCrashMonthsRemaining = 5;
        LabState buyer = g.Rival("openaye");
        buyer.Cash = 300_000_000;
        LabState weak = g.Rival("anthoptics");
        DistressedDebt(g, weak);
        StepMonthEnd(g);
        Assert.Equal(buyer.Id, weak.AbsorbedByLabId);
    }

    // --- Q173 absorbed labs cannot issue player commands ---

    [Fact]
    public void Q173_absorbed_lab_hire_refused()
    {
        var g = Game.New(seed: 811);
        LabState rival = g.Rival("openaye");
        DistressedDebt(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.Hire(rival.Id, "researcher", 1), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q173_absorbed_lab_buy_gpus_refused()
    {
        var g = Game.New(seed: 812);
        LabState rival = g.Rival("openaye");
        DistressedDebt(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.BuyGpus(rival.Id, 1, HardwareBatch.Hq), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q173_absorbed_seller_cannot_list_chips()
    {
        var g = Game.New(seed: 813);
        LabState rival = g.Rival("openaye");
        DistressedDebt(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        g.Refused(Command.StartChipDesign(rival.Id, "inference", "n28", "medium", "fp16", "ddr", "air"), CommandError.LabAbsorbed);
    }

    [Fact]
    public void Q173_absorbed_target_acquire_refused()
    {
        var g = Game.New(seed: 814);
        LabState a = g.Rival("openaye");
        DistressedDebt(g, a);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, a.Id));
        g.Refused(Command.AcquireLab(g.Id, a.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Q173_buyer_still_commands_normally()
    {
        var g = Game.New(seed: 815);
        LabState rival = g.Rival("openaye");
        DistressedDebt(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 1_000_000);
        g.Do(Command.SetTrainingShare(g.Id, 800));
    }

    [Fact]
    public void Q173_cross_rollup_skips_absorbed_buyer()
    {
        var g = Game.New(seed: 816, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.BubbleCrashMonthsRemaining = 4;
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        DistressedDebt(g, target);
        buyer.Cash = 200_000_000;
        buyer.AbsorbedByLabId = g.Me.Id;
        AcquisitionSystem.RunMonth(g.C, g.State);
        Assert.Equal(-1, target.AbsorbedByLabId);
    }

    // --- Q174 training model survives acquisition ---

    [Fact]
    public void Q174_training_model_moves_to_buyer_with_same_id()
    {
        var g = Game.New(seed: 821);
        LabState rival = g.Rival("openaye");
        int modelId = g.State.TakeId();
        rival.Models.Add(new ModelState
        {
            Id = modelId,
            LineId = "mini",
            Name = "Trainee",
            Status = ModelStatus.Training,
            RequiredCompute = 1_000_000,
            Alignment = 400
        });
        rival.Cash = -20_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ModelState m = g.Me.Model(modelId);
        Assert.NotNull(m);
        Assert.Equal(ModelStatus.Training, m.Status);
    }

    [Fact]
    public void Q174_training_compute_progresses_on_buyer_after_acquire()
    {
        var g = Game.New(seed: 822, tweak: c => c.Training.MaxConcurrentRuns = 4);
        LabState rival = g.Rival("openaye");
        string line = g.C.Training.Lines[0].Id;
        Finance.Earn(rival, LedgerLine.CapitalIn, 5_000_000);
        g.Do(Command.StartTraining(rival.Id, line, 100));
        ModelState train = rival.Models.Last(m => m.Status == ModelStatus.Training);
        rival.Cash = -30_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        ModelState m = g.Me.Model(train.Id);
        Assert.NotNull(m);
        Assert.Equal(ModelStatus.Training, m.Status);
        Assert.True(m.RequiredCompute > 0);
        for (int i = 0; i < 8; i++) g.Sim.Step();
        Assert.Equal(ModelStatus.Training, g.Me.Model(train.Id).Status);
    }

    [Fact]
    public void Q174_quote_ignores_training_models_in_parts()
    {
        var g = Game.New(seed: 823);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(Released(g, 700, "Rel"));
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Status = ModelStatus.Training,
            Capability = 900,
            Name = "Train"
        });
        rival.Cash = -40_000;
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.ModelsPart > 0);
        Assert.True(q.ModelsPart < 700L * g.C.Acquisitions.CapabilityValuePerPoint * 2);
    }

    [Fact]
    public void Q174_sell_last_released_while_another_trains()
    {
        var g = Game.Solo();
        ModelState train = new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Status = ModelStatus.Training,
            Capability = 300,
            Name = "Train",
            Alignment = 500
        };
        ModelState rel = Released(g, 500, "SellMe");
        g.Me.Models.Add(train);
        g.Me.Models.Add(rel);
        g.Do(Command.SellModel(g.Id, rel.Id));
        Assert.Equal(ModelStatus.Retired, rel.Status);
        Assert.Equal(ModelStatus.Training, train.Status);
    }

    [Fact]
    public void Q174_cannot_sell_training_model_as_last_active()
    {
        var g = Game.Solo();
        ModelState train = new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Status = ModelStatus.Training,
            Capability = 300,
            Name = "Only",
            Alignment = 500
        };
        g.Me.Models.Add(train);
        CommandResult r = g.Try(Command.SellModel(g.Id, train.Id));
        Assert.False(r.Ok);
    }

    [Fact]
    public void Q174_cross_acquire_then_sell_transferred_released()
    {
        var g = Game.New(seed: 824);
        LabState rival = g.Rival("openaye");
        ModelState rel = Released(g, 550, "Cow");
        ModelState spare = Released(g, 400, "Spare");
        rival.Models.Add(rel);
        rival.Models.Add(spare);
        rival.Cash = -25_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        long cash = g.Me.Cash;
        g.Do(Command.SellModel(g.Id, rel.Id));
        Assert.True(g.Me.Cash > cash);
    }

    // --- Q175 bubble / chips / sales during crash ---

    [Fact]
    public void Q175_max_permille_pop_via_step_emits_crash_news_text()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 450;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = MathI.Permille;
        StepMonthEnd(g);
        GameEvent news = g.State.Events.LastOrDefault(e => e.Kind == EventKind.WorldNews && e.Ref == "bubble-crash");
        Assert.NotNull(news);
        string line = EventText.Describe(g.C, g.State, news);
        Assert.Contains("valuations", line);
    }

    [Fact]
    public void Q175_listed_chip_during_crash_raises_capital_and_meter()
    {
        Game g = HatchFollowUpLaterTests.ChipListingSeller();
        g.State.BubbleCrashMonthsRemaining = 3;
        g.C.Bubble.ListedChipBoostPermille = 15;
        ChipDesign d = HatchFollowUpLaterTests.ReadyInferenceListing(g);
        LabState buyer = HatchFollowUpLaterTests.OpenAyeBuyer(g, 300_000);
        g.Do(Command.SetChipListing(g.Id, d.Id, 6000, 20));
        g.Me.Cash = 25_000_000;
        long cap = g.State.BubbleCapitalRaised;
        int meter = g.State.BubblePermille;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 3));
        Assert.Equal(cap + 18_000, g.State.BubbleCapitalRaised);
        Assert.Equal(meter + 15, g.State.BubblePermille);
    }

    [Fact]
    public void Q175_crash_gpu_price_permille_applies_while_roll_up_runs()
    {
        var g = Game.New(seed: 831, tweak: c =>
        {
            c.Bubble.CrashGpuPricePermille = 500;
            c.Acquisitions.RivalRollupChancePermille = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 2;
        Assert.Equal(500, BubbleSystem.GpuPricePermille(g.C, g.State));
    }

    [Fact]
    public void Q175_model_sale_during_crash_uses_asset_sale_ledger()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.State.BubbleCrashMonthsRemaining = 6;
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.True(g.Me.Ledger.ThisMonth.AssetSale > 0);
        Assert.Equal(0, g.Me.Ledger.ThisMonth.CapitalIn);
    }

    [Fact]
    public void Q175_bankrupt_player_crash_step_records_pop_when_configured()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 400;
            c.Bubble.ThePopEndingForPlayer = true;
            c.Bubble.WeakCashHaircutPermille = 500;
        });
        g.Me.Bankrupt = true;
        g.State.BubblePermille = 500;
        StepMonthEnd(g);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Q175_after_crash_ends_capital_reset_enables_new_listed_chip_hurdle()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubbleCapitalRaised = 90_000_000;
        StepMonthEnd(g);
        Assert.Equal(0, g.State.BubbleCapitalRaised);
        BubbleSystem.OnListedChipTrade(g.C, g.State, 100_000);
        Assert.Equal(100_000, g.State.BubbleCapitalRaised);
    }
}