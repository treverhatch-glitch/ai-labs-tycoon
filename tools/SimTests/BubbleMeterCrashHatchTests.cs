using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BubbleMeterCrashHatchTests
{
    [Fact]
    public void Funding_raise_tracks_capital_raised_on_the_bubble()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
        });
        long cash = g.C.Funding.Round("angel").Cash;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(cash, g.State.BubbleCapitalRaised);
    }

    [Fact]
    public void Weak_returns_while_hot_increment_grace_months()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 300;
            c.Bubble.GraceMonthsBeforeCrash = 3;
            c.Bubble.ReturnsNeededPermille = 500;
            c.Bubble.PopAtPermille = 1000;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 400;
        g.State.BubbleCapitalRaised = 10_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 1;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(1, g.State.BubbleGraceMonths);
        Assert.False(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Strong_returns_reset_grace_and_cool_the_meter()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 300;
            c.Bubble.ReturnCoolingPermille = 30;
            c.Bubble.PostCrashResidualPermille = 50;
            c.Bubble.PopAtPermille = 1000;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 400;
        g.State.BubbleGraceMonths = 2;
        g.State.BubbleCapitalRaised = 100_000;
        g.Me.Ledger.ThisMonth.Revenue = 50_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, g.State.BubbleGraceMonths);
        Assert.Equal(355, g.State.BubblePermille);
    }

    [Fact]
    public void Grace_exhaustion_starts_a_crash_with_duration()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 200;
            c.Bubble.GraceMonthsBeforeCrash = 2;
            c.Bubble.CrashDurationMonths = 6;
            c.Bubble.PopAtPermille = 1000;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 300;
        g.State.BubbleGraceMonths = 1;
        g.State.BubbleCapitalRaised = 5_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(6, g.State.BubbleCrashMonthsRemaining);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Pop_threshold_starts_crash_instead_of_zeroing_the_meter()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 400;
            c.Bubble.PostCrashResidualPermille = 80;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubblePermille = 450;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Equal(80, g.State.BubblePermille);
    }

    [Fact]
    public void Crash_ends_leaving_residual_permille_not_zero()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.CrashDurationMonths = 1;
            c.Bubble.PostCrashResidualPermille = 90;
            c.Bubble.WeakCashHaircutPermille = 0;
        });
        g.State.BubbleCrashMonthsRemaining = 1;
        g.State.BubblePermille = 200;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.False(BubbleSystem.InCrash(g.State));
        Assert.Equal(90, g.State.BubblePermille);
    }

    [Fact]
    public void During_crash_funding_cash_is_discounted()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Bubble.CrashFundingCashPermille = 500;
        });
        g.State.BubbleCrashMonthsRemaining = 3;
        long raw = g.C.Funding.Round("angel").Cash;
        Assert.Equal(MathI.MulPermille(raw, 500), FundingSystem.EffectiveCash(g.C, g.State, g.C.Funding.Round("angel")));
    }

    [Fact]
    public void Display_bubble_meter_shows_warning_three_in_crash()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 2;
        Assert.Equal(3, DisplayQueries.BubbleMeterParts(g.C, g.State).WarningLevel);
    }

    [Fact]
    public void Schema_19_migration_round_trips_crash_state()
    {
        var g = Game.Solo();
        g.State.BubbleCapitalRaised = 9_000_000;
        g.State.BubbleGraceMonths = 2;
        g.State.BubbleCrashMonthsRemaining = 5;
        string old = SaveSchemaPretend.At(g.C, 18, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.State.BubbleCapitalRaised);
        sim.State.BubbleCapitalRaised = 9_000_000;
        sim.State.BubbleCrashMonthsRemaining = 5;
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void Save_load_mid_crash_preserves_months_remaining()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 11;
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(11, loaded.State.BubbleCrashMonthsRemaining);
    }

    [Fact]
    public void Rival_public_float_falls_each_month_of_a_crash()
    {
        var g = Game.New(seed: 4);
        LabState rival = g.Rival("openaye");
        rival.PublicFloatPpm = 200_000;
        g.State.BubbleCrashMonthsRemaining = 2;
        g.C.Bubble.CrashFloatCutPpm = 40_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(160_000, rival.PublicFloatPpm);
    }

    [Fact]
    public void Customer_spend_permille_drops_during_crash_in_market_week()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 1;
        g.C.Bubble.CrashCustomerSpendPermille = 600;
        Assert.Equal(600, BubbleSystem.CustomerSpendPermille(g.C, g.State));
    }
}