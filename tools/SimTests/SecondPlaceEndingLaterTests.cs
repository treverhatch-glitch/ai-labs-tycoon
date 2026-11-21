using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SecondPlaceEndingLaterTests
{
    static void CrownRace(Game g, int playerCap, int rivalCap)
    {
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = 1, LineId = "mini", Capability = playerCap, Status = ModelStatus.Released, Alignment = 500, Name = "Player" });
        LabState top = g.Rival("openaye");
        top.Models.Clear();
        top.Models.Add(new ModelState { Id = 2, LineId = "mini", Capability = rivalCap, Status = ModelStatus.Released, Alignment = 500, Name = "Top" });
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (lab.Id == g.Id || lab.Id == top.Id) continue;
            lab.Models.Clear();
        }
    }

    [Fact]
    public void Runner_up_streak_increments_when_the_player_is_second_in_capability()
    {
        var g = Game.New(seed: 11);
        CrownRace(g, 100, 900);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(1, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Winning_the_crown_resets_runner_up_streak()
    {
        var g = Game.Solo();
        g.Me.CrownRunnerUpStreak = 3;
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 950, Status = ModelStatus.Released, Alignment = 800, Name = "Ace" });
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(0, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Third_place_resets_runner_up_streak()
    {
        var g = Game.New(seed: 13);
        g.Me.CrownRunnerUpStreak = 2;
        CrownRace(g, 50, 900);
        g.Rival("anthoptics").Models.Clear();
        g.Rival("anthoptics").Models.Add(new ModelState { Id = 3, LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "Mid" });
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(0, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Second_place_ending_fires_when_streak_meets_config()
    {
        var g = Game.New(seed: 5, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 2);
        g.Me.CrownRunnerUpStreak = 1;
        CrownRace(g, 100, 700);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId && e.LabId == g.Id);
    }

    [Fact]
    public void Second_place_does_not_fire_one_year_short_of_the_threshold()
    {
        var g = Game.New(seed: 6, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 3);
        g.Me.CrownRunnerUpStreak = 1;
        CrownRace(g, 100, 700);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
        Assert.Equal(2, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Disabling_runner_up_years_never_fires_second_place()
    {
        var g = Game.New(seed: 7, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 0);
        g.Me.CrownRunnerUpStreak = 5;
        CrownRace(g, 100, 900);
        EndingSystem.TrySecondPlace(g.C, g.State, g.Me);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
    }

    [Fact]
    public void Acknowledge_continue_closes_an_open_second_place_record()
    {
        var g = Game.Solo();
        g.State.Endings.Add(new EndingRecord { EndingId = EndingSystem.SecondPlaceId, LabId = g.Id, Tick = g.Tick });
        int idx = g.State.Endings.Count - 1;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        Assert.Equal("acknowledge", g.State.Endings[idx].Choice);
    }

    [Fact]
    public void Crown_winner_still_shifts_rival_feelings_when_player_wins()
    {
        var g = Game.New(seed: 9);
        CrownRace(g, 950, 100);
        int before = g.Rival("openaye").FeelingToPlayer;
        GauntletSystem.RunYear(g.C, g.State);
        Assert.True(g.Rival("openaye").FeelingToPlayer < before);
    }

    [Fact]
    public void Save_round_trip_preserves_runner_up_streak_and_second_place_record()
    {
        var g = Game.New(seed: 8, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 1);
        g.Me.CrownRunnerUpStreak = 0;
        CrownRace(g, 100, 800);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
        string json = g.Sim.Save();
        Simulation loaded = Simulation.Load(g.C, json);
        Assert.Equal(0, loaded.Player.CrownRunnerUpStreak);
        Assert.Contains(loaded.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
    }

    [Fact]
    public void Second_place_does_not_fire_again_after_acknowledge_even_with_high_streak()
    {
        var g = Game.New(seed: 14, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 1);
        g.Me.CrownRunnerUpStreak = 0;
        CrownRace(g, 100, 800);
        GauntletSystem.RunYear(g.C, g.State);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == EndingSystem.SecondPlaceId);
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        g.Me.CrownRunnerUpStreak = 5;
        EndingSystem.TrySecondPlace(g.C, g.State, g.Me);
        Assert.Single(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
    }

    [Fact]
    public void Firing_second_place_resets_runner_up_streak()
    {
        var g = Game.New(seed: 15, tweak: c => c.Endings.SecondPlaceRunnerUpYears = 1);
        g.Me.CrownRunnerUpStreak = 0;
        CrownRace(g, 100, 800);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(0, g.Me.CrownRunnerUpStreak);
    }

    [Fact]
    public void Schema_17_migration_adds_zero_runner_up_streak()
    {
        var g = Game.Solo();
        JsonValue root = Json.Parse(g.Sim.Save());
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "crownRunnerUpStreak"));
        root.Set("schema", JsonValue.Of(16L));
        string old = Json.Write(root, false);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.CrownRunnerUpStreak);
    }

    [Fact]
    public void Schema_16_fixture_migrates_to_17_with_streak_default()
    {
        Simulation sim = Simulation.Load(TestData.Config(), SaveSchemaPretend.At(TestData.Config(), 16, Game.Solo().Sim.Save()));
        Assert.Equal(0, sim.Player.CrownRunnerUpStreak);
    }

    [Fact]
    public void Bankrupt_player_lab_does_not_receive_second_place_from_TrySecondPlace()
    {
        var g = Game.Solo();
        g.Me.Bankrupt = true;
        g.Me.CrownRunnerUpStreak = 5;
        EndingSystem.TrySecondPlace(g.C, g.State, g.Me);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.SecondPlaceId);
    }
}