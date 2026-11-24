using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Hatch bubble bar: edges and cross-system hooks beyond the base crash suite.</summary>
public class BubbleMeterHatchBarTests
{
    [Fact]
    public void Meter_caps_at_max_permille()
    {
        var g = Game.Solo();
        BubbleSystem.Boost(g.C, g.State, 2000);
        Assert.Equal(MathI.Permille, g.State.BubblePermille);
    }

    [Fact]
    public void Decay_before_first_crash_can_go_below_post_crash_residual()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PostCrashResidualPermille = 120;
            c.Bubble.HotPermille = 1000;
            c.Bubble.DecayPermillePerMonth = 50;
            c.Bubble.PopAtPermille = 1000;
        });
        g.State.BubblePermille = 130;
        Assert.False(g.State.BubbleHadCrash);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(80, g.State.BubblePermille);
    }

    [Fact]
    public void Decay_after_first_crash_never_drops_below_post_crash_residual()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PostCrashResidualPermille = 120;
            c.Bubble.DecayPermillePerMonth = 50;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.PopAtPermille = 200;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleHadCrash = true;
        g.State.BubbleCrashMonthsRemaining = 0;
        g.State.BubblePermille = 130;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(120, g.State.BubblePermille);
    }

    [Fact]
    public void Zero_cash_weak_lab_takes_crash_haircut()
    {
        var g = Game.New(seed: 20, tweak: c =>
        {
            c.Bubble.PopAtPermille = 300;
            c.Bubble.WeakCashHaircutPermille = 500;
            c.Bubble.SurvivorRevenuePpm = 800_000;
        });
        g.State.BubblePermille = 400;
        LabState weak = g.Rival("openaye");
        weak.Cash = 0;
        weak.Ledger.ThisMonth.Revenue = 0;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, weak.Cash);
    }

    [Fact]
    public void Revenue_leader_survives_crash_haircut()
    {
        var g = Game.New(seed: 21, tweak: c => c.Bubble.PopAtPermille = 300);
        g.State.BubblePermille = 400;
        long cash = g.Me.Cash;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void Bankrupt_lab_ignored_by_crash_haircut()
    {
        var g = Game.New(seed: 22, tweak: c => c.Bubble.PopAtPermille = 300);
        LabState rival = g.Rival("openaye");
        rival.Bankrupt = true;
        rival.Cash = 1_000_000;
        g.State.BubblePermille = 500;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(1_000_000, rival.Cash);
    }

    [Fact]
    public void Save_load_mid_crash_then_migration_18_to_19()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 7;
        g.State.BubbleCapitalRaised = 4_000_000;
        string json = g.Sim.Save();
        string old = SaveSchemaPretend.At(g.C, 18, json);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(0, sim.State.BubbleCrashMonthsRemaining);
        Assert.Equal(0, sim.State.BubbleCapitalRaised);
        sim.State.BubbleCrashMonthsRemaining = 7;
        sim.State.BubbleCapitalRaised = 4_000_000;
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void After_crash_ends_funding_can_reinflate_the_meter()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Bubble.PostCrashResidualPermille = 80;
        });
        g.State.BubblePermille = 80;
        g.State.BubbleCrashMonthsRemaining = 0;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.True(g.State.BubblePermille > 80);
    }

    [Fact]
    public void Down_round_and_crash_stack_on_funding_cash()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Funding.FollowOn.DownRoundBubblePermille = 100;
            c.Bubble.CrashFundingCashPermille = 500;
        });
        g.State.BubblePermille = 800;
        g.State.BubbleCrashMonthsRemaining = 3;
        long raw = g.C.Funding.Round("angel").Cash;
        long eff = FundingSystem.EffectiveCash(g.C, g.State, g.C.Funding.Round("angel"));
        Assert.True(eff < raw);
    }

    [Fact]
    public void Hot_bubble_keeps_regulator_scrutiny_monthly()
    {
        var g = Game.New(seed: 23);
        g.State.BubblePermille = 800;
        g.Me.Ledger.ThisMonth.Revenue = 5_000_000;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        RegulatorSystem.RunMonth(g.C, g.State);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention > before);
    }

    [Fact]
    public void Weak_player_can_still_get_the_pop_during_crash()
    {
        var g = Game.New(seed: 101, tweak: c =>
        {
            c.Bubble.PopAtPermille = 200;
            c.Bubble.ThePopEndingForPlayer = true;
            c.Bubble.SurvivorRevenuePpm = 900_000;
        });
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            lab.Ledger.ThisMonth.Revenue = lab.Id == g.Id ? 1 : 500_000;
        }
        g.State.BubblePermille = 300;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void Sector_pools_shrink_during_crash_weeks()
    {
        var g = Game.Solo(tweak: c => c.Bubble.CrashCustomerSpendPermille = 500);
        ModelState m = new ModelState { Id = 1, LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 600, Name = "M" };
        g.Me.Models.Add(m);
        g.Me.LastWeek.ServingCompute = 1_000_000_000;
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.State.BubbleCrashMonthsRemaining = 0;
        MarketSystem.RunWeek(g.C, g.State);
        long normal = g.State.Sector("game-ai").Entries[0].LastRevenue;
        g.State.BubbleCrashMonthsRemaining = 4;
        MarketSystem.RunWeek(g.C, g.State);
        long crash = g.State.Sector("game-ai").Entries[0].LastRevenue;
        Assert.True(crash < normal);
    }

    [Fact]
    public void Gpu_prices_fall_during_crash()
    {
        var g = Game.Solo(tweak: c => c.Bubble.CrashGpuPricePermille = 700);
        int normal = VendorSystem.PricePermille(g.C, g.State, g.Me);
        g.State.BubbleCrashMonthsRemaining = 5;
        int crash = VendorSystem.PricePermille(g.C, g.State, g.Me);
        Assert.True(crash < normal);
    }

    [Fact]
    public void Over_leveraged_lab_loses_more_cash_than_cash_rich_survivor()
    {
        var g = Game.New(seed: 24, tweak: c =>
        {
            c.Bubble.PopAtPermille = 250;
            c.Bubble.WeakCashHaircutPermille = 400;
            c.Bubble.SurvivorRevenuePpm = 600_000;
        });
        g.State.BubblePermille = 300;
        g.Me.Cash = 2_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 50_000;
        LabState rival = g.Rival("openaye");
        rival.Cash = 200_000;
        rival.Ledger.ThisMonth.Revenue = 2_000_000;
        long weakBefore = g.Me.Cash;
        long strongBefore = rival.Cash;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(g.Me.Cash < weakBefore);
        Assert.Equal(strongBefore, rival.Cash);
    }

    [Fact]
    public void Display_parts_expose_grace_and_capital_for_ui()
    {
        var g = Game.Solo();
        g.State.BubblePermille = 900;
        g.State.BubbleGraceMonths = 3;
        g.State.BubbleCapitalRaised = 2_000_000;
        var p = DisplayQueries.BubbleMeterParts(g.C, g.State);
        Assert.Equal(900, p.Permille);
        Assert.Equal(3, p.GraceMonths);
        Assert.Equal(2_000_000, p.CapitalRaised);
        Assert.Equal(2, p.WarningLevel);
    }
}