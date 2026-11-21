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

public class ThePopEndingLaterTests
{
    static Game WeakPlayerPopGame(Action<GameConfig> tweak = null)
    {
        return Game.New(seed: 101, tweak: c =>
        {
            c.Bubble.PopAtPermille = 400;
            c.Bubble.SurvivorRevenuePpm = 800_000;
            c.Bubble.WeakCashHaircutPermille = 250;
            c.Bubble.ThePopEndingForPlayer = true;
            if (tweak != null) tweak(c);
        });
    }

    static void MakeWeakPlayer(Game g)
    {
        g.Me.Ledger.ThisMonth.Revenue = 100;
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (lab.Id == g.Id) continue;
            lab.Ledger.ThisMonth.Revenue = 2_000_000;
        }
        g.State.BubblePermille = 450;
        g.Me.Cash = 200_000;
    }

    [Fact]
    public void A_weak_player_gets_the_pop_ending_when_the_bubble_pops()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId && e.Choice == "");
    }

    [Fact]
    public void A_survivor_player_gets_no_pop_ending()
    {
        var g = Game.Solo(tweak: c => { c.Bubble.SurvivorRevenuePpm = 500_000; c.Bubble.PopAtPermille = 400; });
        g.Me.Ledger.ThisMonth.Revenue = 20_000_000;
        g.State.BubblePermille = 500;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void Turning_off_the_pop_flag_skips_the_ending_even_after_a_haircut()
    {
        var g = WeakPlayerPopGame(c => c.Bubble.ThePopEndingForPlayer = false);
        MakeWeakPlayer(g);
        long before = g.Me.Cash;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(g.Me.Cash < before);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void A_bankrupt_player_is_not_given_the_pop_ending()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        g.Me.Bankrupt = true;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void The_pop_ending_fires_at_most_once_while_choice_is_open()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        MakeWeakPlayer(g);
        g.State.BubblePermille = 500;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(1, g.State.Endings.Count(e => e.EndingId == EndingSystem.ThePopId));
    }

    [Fact]
    public void Acknowledge_continues_after_the_pop()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == EndingSystem.ThePopId);
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        Assert.Equal("acknowledge", g.State.Endings[idx].Choice);
        Assert.Equal(g.Id, g.State.PlayerLabId);
    }

    [Fact]
    public void Pop_applies_the_configured_cash_haircut_to_a_weak_lab()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        long before = g.Me.Cash;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Equal(MathI.MulPermille(before, MathI.Permille - 250), g.Me.Cash);
    }

    [Fact]
    public void Pop_resets_the_bubble_meter_and_posts_bubble_crash_news()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == "bubble-crash");
    }

    [Fact]
    public void Pop_ending_reaches_the_event_log_with_ending_reached()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EndingReached && e.Ref == EndingSystem.ThePopId);
    }

    [Fact]
    public void Funding_before_pop_still_built_pressure_then_the_pop_clears_it()
    {
        var g = WeakPlayerPopGame(c =>
        {
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            c.Bubble.PopAtPermille = 150;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        MakeWeakPlayer(g);
        Assert.True(g.State.BubblePermille >= 150);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.True(BubbleSystem.InCrash(g.State));
    }

    [Fact]
    public void Save_load_round_trip_keeps_the_pop_ending_record()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        string json = g.Sim.Save();
        Simulation loaded = Simulation.Load(g.C, json);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Contains(loaded.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void Schema_17_save_still_loads_after_the_pop_ending()
    {
        var g = WeakPlayerPopGame();
        MakeWeakPlayer(g);
        BubbleSystem.RunMonth(g.C, g.State);
        string old = SaveSchemaPretend.At(g.C, 16, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        SaveSchemaPretend.AssertRoundTrip(sim);
        Assert.Contains(sim.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }
}