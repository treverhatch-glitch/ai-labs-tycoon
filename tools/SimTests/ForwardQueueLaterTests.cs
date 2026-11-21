using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q97–Q101 forward queue: pop, second place, exit breakdown, era incident load, long soak.</summary>
public class ForwardQueueLaterTests
{
    [Fact]
    public void Bubble_pop_records_the_pop_for_a_weak_player()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.SurvivorRevenuePpm = 900000;
            c.Bubble.WeakCashHaircutPermille = 300;
        });
        g.State.BubblePermille = 960;
        g.Me.Cash = 100_000;
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId && e.LabId == g.Id);
    }

    [Fact]
    public void Bubble_pop_skips_the_pop_ending_when_the_player_survives_revenue_threshold()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.PopAtPermille = 500;
            c.Bubble.SurvivorRevenuePpm = 500000;
            c.Bubble.WeakCashHaircutPermille = 500;
        });
        g.Me.Ledger.ThisMonth.Revenue = 10_000_000;
        g.State.BubblePermille = 960;
        Assert.True(GovernmentSystem.RevenueSharePpm(g.State, g.Me) >= g.C.Bubble.SurvivorRevenuePpm);
        BubbleSystem.RunMonth(g.C, g.State);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void The_pop_lists_acknowledge_in_config()
    {
        Assert.Contains("acknowledge", TestData.Config().Endings.Def("the-pop").ContinueModes);
    }

    [Fact]
    public void Crown_runner_up_streak_increments_when_the_player_is_second()
    {
        var g = Game.New(seed: 40);
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 100, Status = ModelStatus.Released, Alignment = 500, Name = "A" });
        for (int i = 1; i < g.State.Labs.Count; i++)
        {
            g.State.Labs[i].Models.Clear();
            g.State.Labs[i].Models.Add(new ModelState { Id = 10 + i, Capability = 10, Status = ModelStatus.Released, Alignment = 500, Name = "R" + i });
        }
        g.Me.Models[0].Capability = 50;
        g.State.Labs[1].Models[0].Capability = 500;
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(1, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Crown_runner_up_streak_resets_when_the_player_wins()
    {
        var g = Game.Solo();
        g.Me.CrownRunnerUpStreak = 2;
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 900, Status = ModelStatus.Released, Alignment = 900, Name = "Ace" });
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(0, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Second_place_ending_fires_after_configured_streak_years()
    {
        var g = Game.New(seed: 5, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 2);
        g.Me.CrownRunnerUpStreak = 1;
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 50, Status = ModelStatus.Released, Alignment = 500, Name = "B" });
        var rival = g.Rival("openaye");
        rival.Models.Clear();
        rival.Models.Add(new ModelState { Id = 2, Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "Top" });
        for (int i = 1; i < g.State.Labs.Count; i++) if (g.State.Labs[i].Id != rival.Id) g.State.Labs[i].Models.Clear();
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
    }

    [Fact]
    public void Exit_price_breakdown_sums_to_SellPrice()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 2);
        g.Me.Ledger.ThisMonth.Revenue = 120_000;
        g.Me.Cash = 40_000;
        DisplayQueries.ExitPriceBreakdown b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(b.Total, EndingSystem.SellPrice(g.C, g.State, g.Me));
        Assert.Equal(2 * 120_000, b.RevenueYears);
        Assert.Equal(40_000, b.CashOnHand);
    }

    [Fact]
    public void Agents_era_raises_incident_load_above_unchanged_eras()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 600, Corruption = 0, Status = ModelStatus.Released };
        int dialUp = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 1996), model);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2025);
        int agents = SafetySystem.IncidentChancePermille(g.C, g.Tick, model);
        Assert.True(agents > dialUp);
    }

    [Fact]
    public void Zero_incident_load_defaults_to_unchanged()
    {
        var g = Game.Solo(tweak: c => c.Era("dial-up-days").IncidentLoadPermille = 0);
        var model = new ModelState { Alignment = 500, Corruption = 0, Status = ModelStatus.Released };
        int at = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 1996), model);
        Assert.Equal((int)MathI.MulPermille(g.C.Safety.IncidentChancePermille, MathI.Permille - 500), at);
    }

    [Fact]
    public void Schema_17_migration_adds_runner_up_streak()
    {
        var g = Game.Solo();
        string old = SaveSchemaPretend.At(g.C, 16, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.CrownRunnerUpStreak);
    }

    [Fact]
    public void Second_place_continue_mode_acknowledges()
    {
        var g = Game.Solo();
        g.State.Endings.Add(new EndingRecord { EndingId = EndingSystem.SecondPlaceId, LabId = g.Id, Tick = g.Tick });
        int idx = g.State.Endings.Count - 1;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        Assert.Equal("acknowledge", g.State.Endings[idx].Choice);
    }

    [Fact]
    public void Sixty_game_years_run_without_exceptions()
    {
        Game g = Game.New(seed: 77);
        g.Sim.StepMany(60 * g.C.Calendar.WeeksPerYear);
        Assert.True(g.Tick >= 60 * g.C.Calendar.WeeksPerYear);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}