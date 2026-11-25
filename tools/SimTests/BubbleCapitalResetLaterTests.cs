using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q171: reset cumulative bubble hot-money after a crash phase ends.</summary>
public class BubbleCapitalResetLaterTests
{
    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    [Fact]
    public void Q171_crash_end_clears_capital_when_config_on()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 2;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleHadCrash = true;
        g.State.BubbleCrashMonthsRemaining = 2;
        g.State.BubbleCapitalRaised = 80_000_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(1, g.State.BubbleCrashMonthsRemaining);
        Assert.Equal(80_000_000, g.State.BubbleCapitalRaised);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.State.BubbleCrashMonthsRemaining);
        Assert.Equal(0, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_config_off_keeps_capital_after_crash()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = false;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleHadCrash = true;
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubbleCapitalRaised = 12_000_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.False(BubbleSystem.InCrash(g.State));
        Assert.Equal(12_000_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_zero_capital_unchanged_at_crash_end()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubbleCapitalRaised = 0;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_mid_crash_month_does_not_clear_capital()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 12;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 6;
        g.State.BubbleCapitalRaised = 9_000_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Equal(9_000_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_full_crash_via_month_end_step_resets_capital()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.WeakCashHaircutPermille = 0;
            c.Bubble.DecayPermillePerMonth = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubbleCapitalRaised = 40_000_000;
        StepMonthEnd(g);
        Assert.False(BubbleSystem.InCrash(g.State));
        Assert.Equal(0, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_save_load_round_trip_preserves_capital_mid_crash()
    {
        var g = Game.Solo();
        g.State.BubbleCapitalRaised = 33_000_000;
        g.State.BubbleCrashMonthsRemaining = 4;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.Equal(33_000_000, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Q171_cross_after_reset_new_funding_rebuilds_hurdle_base()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.ResetCapitalRaisedAfterCrash = true;
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.WeakCashHaircutPermille = 0;
            c.Bubble.HotPermille = 400;
            c.Bubble.GraceMonthsBeforeCrash = 9;
            c.Bubble.ReturnsNeededPermille = 80;
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubbleCapitalRaised = 100_000_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.State.BubbleCapitalRaised);
        FundingRoundConfig round = g.C.Funding.Round("angel");
        BubbleSystem.OnFundingRaise(g.C, g.State, round, 2_000_000);
        Assert.Equal(2_000_000, g.State.BubbleCapitalRaised);
        g.State.BubblePermille = 500;
        g.Me.Ledger.ThisMonth.Revenue = 200_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.State.BubbleGraceMonths);
    }
}