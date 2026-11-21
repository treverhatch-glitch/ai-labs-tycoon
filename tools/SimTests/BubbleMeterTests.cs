using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BubbleMeterTests
{
    [Fact]
    public void RaiseFunding_adds_bubble_pressure_from_the_round()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Funding.Round("angel").BubbleBoostPermille = 100;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(100, g.State.BubblePermille);
    }

    [Fact]
    public void The_meter_decays_each_month()
    {
        var g = Game.Solo(tweak: c => c.Bubble.HotPermille = 1000);
        g.State.BubblePermille = 500;
        g.Weeks(4);
        Assert.Equal(500 - g.C.Bubble.DecayPermillePerMonth, g.State.BubblePermille);
    }

    [Fact]
    public void A_high_meter_pops_and_haircuts_weak_labs()
    {
        var g = Game.New(seed: 1, tweak: c => c.Bubble.PopAtPermille = 400);
        g.State.BubblePermille = 450;
        LabState weak = g.Rival("openaye");
        weak.Cash = 100_000;
        weak.Ledger.ThisMonth.Revenue = 1;
        g.Me.Ledger.ThisMonth.Revenue = 500_000;
        long weakCash = weak.Cash;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Equal(g.C.Bubble.PostCrashResidualPermille, g.State.BubblePermille);
        Assert.True(weak.Cash < weakCash);
    }

    [Fact]
    public void Revenue_leaders_survive_a_pop_without_a_haircut()
    {
        var g = Game.Solo(tweak: c => c.Bubble.PopAtPermille = 300);
        g.State.BubblePermille = 400;
        g.Me.Ledger.ThisMonth.Revenue = 900_000;
        g.Me.Cash = 50_000;
        long cash = g.Me.Cash;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void The_bubble_pops_world_event_triggers_a_crash()
    {
        var g = Game.Solo();
        g.State.BubblePermille = 600;
        WorldEventConfig pop = g.C.WorldEvent("bubble-pops");
        g.Me.Ledger.ThisMonth.Revenue = 900_000;
        BubbleSystem.OnWorldEvent(g.C, g.State, pop);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Listed_chip_buys_nudge_the_meter_up()
    {
        var g = Game.Solo(tweak: c => c.Bubble.ListedChipBoostPermille = 40);
        int before = g.State.BubblePermille;
        BubbleSystem.OnListedChipTrade(g.C, g.State);
        Assert.Equal(before + 40, g.State.BubblePermille);
    }

    [Fact]
    public void Default_funding_boost_applies_when_a_round_leaves_boost_at_zero()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Funding.Round("angel").BubbleBoostPermille = 0;
            c.Bubble.DefaultFundingBoostPermille = 77;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(77, g.State.BubblePermille);
    }

    [Fact]
    public void Bubble_permille_caps_at_one_thousand()
    {
        var g = Game.Solo();
        g.State.BubblePermille = 200;
        FundingRoundConfig round = g.C.Funding.Round("angel");
        round.BubbleBoostPermille = 900;
        BubbleSystem.OnFundingRaise(g.C, g.State, round, round.Cash);
        Assert.Equal(1000, g.State.BubblePermille);
    }

    [Fact]
    public void Bubble_state_survives_a_save_round_trip()
    {
        var g = Game.Solo();
        g.State.BubblePermille = 612;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(612, again.State.BubblePermille);
    }

    [Fact]
    public void Pop_at_threshold_runs_after_monthly_decay()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.DecayPermillePerMonth = 10;
            c.Bubble.WeakCashHaircutPermille = 0;
            c.Bubble.HotPermille = 1000;
        });
        g.State.BubblePermille = 510;
        g.Me.Ledger.ThisMonth.Revenue = 1_000_000;
        g.Weeks(4);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Config_validation_requires_a_real_pop_world_event()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Bubble.PopWorldEventId = "not-an-event";
        var problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("popWorldEventId"));
    }

    [Fact]
    public void A_bankrupt_lab_is_ignored_by_pop_haircuts()
    {
        var g = Game.New(seed: 2, tweak: c => c.Bubble.WeakCashHaircutPermille = 500);
        g.State.BubblePermille = 800;
        LabState rival = g.Rival("openaye");
        rival.Bankrupt = true;
        rival.Cash = 1_000_000;
        rival.Ledger.ThisMonth.Revenue = 0;
        long cash = rival.Cash;
        BubbleSystem.Pop(g.C, g.State);
        Assert.Equal(cash, rival.Cash);
    }
}