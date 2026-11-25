using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Crunch time: more research and faster training for the player's team, paid for in morale, incidents and, at the end, quits.</summary>
public class CrunchTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        return g;
    }

    static List<GameEvent> Ended(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.CrunchEnded).ToList(); }

    // ---- the command

    [Fact]
    public void Nobody_is_in_crunch_until_the_player_says_so()
    {
        Game g = Lab();
        Assert.False(Crunch.Active(g.Me));
        Assert.Equal(0, g.Me.CrunchWeeks);
        Assert.Equal(0, Crunch.ResearchPermille(g.C, g.Me));
        Assert.Equal(0, Crunch.TrainingPermille(g.C, g.Me));
        Assert.Equal(0, Crunch.IncidentPermille(g.C, g.Me));
    }

    [Fact]
    public void Starting_crunch_marks_the_first_week_and_stopping_clears_it_and_logs_the_end()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.Equal(1, g.Me.CrunchWeeks);
        g.Weeks(3);
        Assert.Equal(4, g.Me.CrunchWeeks);
        g.Do(Command.SetCrunch(g.Id, false));
        Assert.Equal(0, g.Me.CrunchWeeks);
        GameEvent e = Ended(g).Single();
        Assert.Equal(3, e.Amount);
        Assert.Equal("stopped", e.Ref);
        Assert.Contains("crunch time ended after 3 weeks", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Starting_twice_or_stopping_when_not_started_is_refused_and_changes_nothing()
    {
        Game g = Lab();
        g.Refused(Command.SetCrunch(g.Id, false), CommandError.InvalidArgument);
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(2);
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.InvalidArgument);
        Assert.Equal(3, g.Me.CrunchWeeks);
        Assert.Empty(Ended(g));
    }

    [Fact]
    public void A_count_that_is_not_on_or_off_is_refused()
    {
        Game g = Lab();
        Command bad = Command.SetCrunch(g.Id, true);
        bad.Count = 2;
        g.Refused(bad, CommandError.InvalidArgument);
        bad.Count = -1;
        g.Refused(bad, CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.CrunchWeeks);
    }

    [Fact]
    public void A_lab_with_nobody_on_the_payroll_cannot_crunch()
    {
        Game g = Lab();
        g.Me.Staff.Clear();
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.NotEnoughStaff);
        Assert.Equal(0, g.Me.CrunchWeeks);
        Assert.Equal(0, Crunch.AverageMorale(g.Me));
    }

    [Fact]
    public void A_team_that_is_already_tired_cannot_start()
    {
        Game g = Lab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 449;
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.TeamTooTired);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 450;
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.True(Crunch.Active(g.Me));
    }

    [Fact]
    public void Only_the_players_lab_can_start_it()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Assert.False(g.Try(Command.SetCrunch(rival.Id, true)).Ok);
        Assert.Equal(0, rival.CrunchWeeks);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_start_it_and_its_weeks_stop_counting()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(2);
        int weeks = g.Me.CrunchWeeks;
        g.Me.Bankrupt = true;
        g.Refused(Command.SetCrunch(g.Id, false), CommandError.LabBankrupt);
        g.Weeks(3);
        Assert.Equal(weeks, g.Me.CrunchWeeks);
    }

    [Fact]
    public void A_game_without_crunch_in_the_data_refuses_it()
    {
        Game g = Lab(c => c.StaffPeople.CrunchMaxWeeks = 0);
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.InvalidArgument);
    }

    // ---- what it buys

    [Fact]
    public void Research_goes_faster_by_the_data_s_share()
    {
        Game g = Lab();
        long plain = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.Do(Command.SetCrunch(g.Id, true));
        long crunch = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        Assert.Equal(MathI.MulPermille(plain, 1000 + g.C.StaffPeople.CrunchResearchPermille), crunch);
        Assert.True(crunch > plain);
    }

    [Fact]
    public void Training_speed_rises_and_a_run_finishes_sooner()
    {
        Game plain = Lab();
        Game crunch = Lab();
        crunch.Do(Command.SetCrunch(crunch.Id, true));
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) + crunch.C.StaffPeople.CrunchTrainingPermille, TrainingSystem.SpeedPermille(crunch.C, crunch.Me));
        ModelLineConfig line = plain.C.Line("mini");
        Assert.True(TrainingSystem.EstimateWeeks(crunch.C, crunch.State, crunch.Me, line, 100) <= TrainingSystem.EstimateWeeks(plain.C, plain.State, plain.Me, line, 100));
    }

    [Fact]
    public void A_released_model_is_more_likely_to_have_an_incident_and_the_breakdown_says_so()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        int before = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.SetCrunch(g.Id, true));
        int after = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(before + g.C.StaffPeople.CrunchIncidentPerWeek, after);
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.Equal(g.C.StaffPeople.CrunchIncidentPerWeek, b.FromCrunch);
    }

    [Fact]
    public void The_incident_addition_grows_each_week_and_stops_at_the_cap()
    {
        Game g = Lab(c => { c.StaffPeople.CrunchMoraleDrop = 0; c.StaffPeople.CrunchMaxWeeks = 100; });
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.Equal(2, Crunch.IncidentPermille(g.C, g.Me));
        g.Weeks(4);
        Assert.Equal(10, Crunch.IncidentPermille(g.C, g.Me));
        g.Weeks(60);
        Assert.Equal(40, Crunch.IncidentPermille(g.C, g.Me));
    }

    // ---- what it costs

    [Fact]
    public void Everyone_loses_morale_each_week_less_the_drift_back()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(1);
        int expect = 500 - g.C.StaffPeople.CrunchMoraleDrop + g.C.StaffPeople.MoraleDriftPerWeek;
        Assert.All(g.Me.Staff, p => Assert.Equal(expect, p.Morale));
    }

    [Fact]
    public void Crunch_ends_by_itself_when_the_team_is_worn_out()
    {
        Game g = Lab(c => { c.StaffPeople.CrunchMoraleDrop = 60; });
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(10);
        Assert.False(Crunch.Active(g.Me));
        GameEvent e = Ended(g).Single();
        Assert.Equal("tired", e.Ref);
        Assert.True(Crunch.AverageMorale(g.Me) < g.C.StaffPeople.CrunchBreakMorale + g.C.StaffPeople.MoraleDriftPerWeek * 10);
        Assert.Contains("worn out", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Crunch_ends_by_itself_after_the_longest_run()
    {
        Game g = Lab(c => { c.StaffPeople.CrunchMoraleDrop = 0; c.StaffPeople.CrunchMaxWeeks = 6; });
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(5);
        Assert.True(Crunch.Active(g.Me));
        Assert.Equal(6, g.Me.CrunchWeeks);
        g.Weeks(1);
        Assert.False(Crunch.Active(g.Me));
        GameEvent e = Ended(g).Single();
        Assert.Equal("spent", e.Ref);
        Assert.Equal(6, e.Amount);
    }

    [Fact]
    public void Crunch_ends_when_the_last_person_has_gone()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Me.Staff.Clear();
        g.Weeks(1);
        Assert.False(Crunch.Active(g.Me));
        Assert.Equal("empty", Ended(g).Single().Ref);
    }

    [Fact]
    public void A_person_who_quits_by_blog_post_goes_when_crunch_drags_morale_below_their_line()
    {
        Game g = Lab(c => { c.StaffPeople.CrunchMoraleDrop = 40; c.StaffPeople.CrunchBreakMorale = 0; c.StaffPeople.CrunchMinMorale = 0; c.StaffPeople.CrunchMaxWeeks = 100; });
        g.Me.Staff[0].TraitId = "quits-by-blog-post";
        int before = g.Me.Staff.Count;
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(8);
        Assert.Equal(before - 1, g.Me.Staff.Count);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.StaffQuit);
    }

    [Fact]
    public void Morale_lost_in_crunch_comes_back_only_at_the_drift()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(10);
        g.Do(Command.SetCrunch(g.Id, false));
        int low = g.Me.Staff[0].Morale;
        Assert.True(low < 500);
        g.Weeks(5);
        Assert.Equal(low + 5 * g.C.StaffPeople.MoraleDriftPerWeek, g.Me.Staff[0].Morale);
    }

    // ---- who it is for

    [Fact]
    public void Rivals_never_get_the_bonus_even_when_their_field_is_set()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        long before = TrainingSystem.ResearchPerWeek(g.C, rival, g.State.Tick);
        rival.CrunchWeeks = 3;
        Assert.True(TrainingSystem.ResearchPerWeek(g.C, rival, g.State.Tick) >= before);
        g.Weeks(1);
        Assert.True(rival.CrunchWeeks == 0 || rival.CrunchWeeks == 4);
    }

    [Fact]
    public void An_untouched_game_never_changes_the_crunch_field()
    {
        Game g = Lab();
        g.Weeks(60);
        Assert.All(g.State.Labs, l => Assert.Equal(0, l.CrunchWeeks));
        Assert.Empty(Ended(g));
    }

    // ---- save, load, replay

    [Fact]
    public void A_save_in_the_middle_of_crunch_loads_back_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(7);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(8, back.Player.CrunchWeeks);
    }

    [Fact]
    public void Playing_on_after_loading_gives_the_same_game()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(30);
        for (int i = 0; i < 30; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void A_schema_43_save_loads_with_nobody_in_crunch()
    {
        Game g = Lab();
        g.Weeks(4);
        string old = SaveSchemaPretend.At(g.C, 43, g.Sim.Save());
        Assert.DoesNotContain("crunchWeeks", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.CrunchWeeks);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_negative_or_too_long_crunch_is_reported()
    {
        Game g = Lab();
        g.Me.CrunchWeeks = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("crunch time"));
        g.Me.CrunchWeeks = g.C.StaffPeople.CrunchMaxWeeks + 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("crunch time"));
        g.Me.CrunchWeeks = g.C.StaffPeople.CrunchMaxWeeks;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("crunch time"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(250, c.StaffPeople.CrunchResearchPermille);
        Assert.Equal(26, c.StaffPeople.CrunchMaxWeeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.StaffPeople.CrunchResearchPermille = -1;
        c.StaffPeople.CrunchTrainingPermille = 5001;
        c.StaffPeople.CrunchMoraleDrop = 1001;
        c.StaffPeople.CrunchBreakMorale = 600;
        c.StaffPeople.CrunchMaxWeeks = 521;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("crunchResearchPermille"));
        Assert.Contains(problems, p => p.Contains("crunchTrainingPermille"));
        Assert.Contains(problems, p => p.Contains("crunchMoraleDrop"));
        Assert.Contains(problems, p => p.Contains("crunchBreakMorale is above"));
        Assert.Contains(problems, p => p.Contains("crunchMaxWeeks"));
    }
}
