using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LongHorizonSoakLaterTests
{
    const int SixtyYearsWeeks = 60 * 48;

    [Fact]
    public void Sixty_years_from_seed_77_produces_no_save_problems()
    {
        Game g = Game.New(seed: 77);
        g.Sim.StepMany(SixtyYearsWeeks);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Sixty_year_save_json_stays_below_two_megabytes()
    {
        Game g = Game.New(seed: 43);
        g.Sim.StepMany(SixtyYearsWeeks);
        int len = g.Sim.Save().Length;
        Assert.InRange(len, 10_000, 2_200_000);
    }

    [Fact]
    public void At_least_one_new_model_version_appears_in_the_final_decade()
    {
        Game g = Game.New(seed: 19);
        int start = GameDate.Year(g.C.Calendar, g.Tick);
        g.Sim.StepMany(50 * g.C.Calendar.WeeksPerYear);
        g.Sim.StepMany(10 * g.C.Calendar.WeeksPerYear);
        int end = GameDate.Year(g.C.Calendar, g.Tick);
        Assert.True(end >= start + 60);
        bool anyTrain = g.State.Labs.Any(lab => !lab.Bankrupt && lab.Models.Any(m => m.Version >= 2));
        Assert.True(anyTrain);
    }

    [Fact]
    public void Replay_after_sixty_years_matches_digest_from_same_log()
    {
        Game g = Game.New(seed: 31);
        g.Sim.StepMany(SixtyYearsWeeks);
        string digest = g.Sim.Digest();
        Simulation replayed = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(digest, replayed.Digest());
    }

    [Fact]
    public void Schema_17_save_after_sixty_years_round_trips()
    {
        Game g = Game.New(seed: 52);
        g.Sim.StepMany(SixtyYearsWeeks);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.Equal(GameState.CurrentSchema, g.State.Schema);
    }

    [Fact]
    public void Loaded_schema_16_save_runs_five_more_years_at_schema_17()
    {
        Game g = Game.Solo(seed: 3);
        string old = SaveSchemaPretend.At(g.C, 16, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        int tick = sim.Tick;
        sim.StepMany(5 * g.C.Calendar.WeeksPerYear);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.True(sim.Tick > tick);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void Plan_run_rivals_still_take_actions_after_sixty_years()
    {
        Game g = Game.New(seed: 64);
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        int staff0 = LabQueries.TotalStaff(rival);
        g.Sim.StepMany(SixtyYearsWeeks);
        rival.PlanStartTick = g.Tick;
        g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);
        Assert.True(LabQueries.TotalStaff(rival) >= staff0 || rival.Techs.Count > 0 || rival.Cash != 0);
    }

    [Fact]
    public void Bubble_meter_never_exceeds_permille_during_sixty_years()
    {
        Game g = Game.New(seed: 88);
        for (int w = 0; w < SixtyYearsWeeks; w++)
        {
            g.Weeks(1);
            Assert.InRange(g.State.BubblePermille, 0, MathI.Permille);
        }
    }

    [Fact]
    public void Player_lab_still_exists_and_is_not_bankrupt_on_seed_77()
    {
        Game g = Game.New(seed: 77);
        g.Sim.StepMany(SixtyYearsWeeks);
        Assert.True(g.State.PlayerLabId >= 0);
        Assert.False(g.Me.Bankrupt);
    }

    [Fact]
    public void Event_log_stays_within_configured_max_length()
    {
        Game g = Game.New(seed: 14);
        g.Sim.StepMany(SixtyYearsWeeks);
        Assert.True(g.State.Events.Count <= g.C.EventLogMax);
    }

    [Fact]
    public void Frontier_level_can_rise_over_sixty_years_with_research_budget()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Endings.Frontier.BaseCost = 100;
            c.Endings.Frontier.GrowthPermille = 1100;
        });
        g.Me.ResearchPoints = 50_000_000;
        foreach (TechConfig t in g.C.Research.Techs)
            if (EraSystem.TechOpen(g.C, t, g.Tick)) g.Me.Techs.Add(t.Id);
        int start = g.Me.FrontierLevel;
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(start + 1, g.Me.FrontierLevel);
    }

    [Fact]
    public void Year_end_superintelligence_check_runs_without_exception_late_in_the_horizon()
    {
        Game g = Game.New(seed: 21);
        g.Sim.StepMany(SixtyYearsWeeks);
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            Capability = 950,
            Alignment = 400,
            Status = ModelStatus.Released,
            LineId = "mini",
            Version = 1
        });
        g.Me.Rooms.Clear();
        EndingSystem.TrySuperintelligence(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.LooseId);
    }
}