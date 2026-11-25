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

/// <summary>Skunkworks: a team is pulled out of a function for a project, costing its output and a budget, and pays by the person-weeks worked.</summary>
public class SkunkworksTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool withModel = false)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 2, gpus: 8);
        g.Build("safety-lab", 4, 3);
        g.Hire("safety", 3);
        if (withModel)
        {
            ModelState m = g.Train("mini", 100);
            g.UntilTrained(m);
        }
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static long PerResearcher(Game g) { return MathI.MulPermille(g.C.Research.PointsPerResearcherWeek, EraSystem.AtTick(g.C, g.State.Tick).ResearchPermille); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Skunkworks.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Skunkworks.Enabled = 2;
        c.Skunkworks.MinPeople = 0;
        c.Skunkworks.MaxSharePermille = 1000;
        c.Skunkworks.MinWeeks = 0;
        c.Skunkworks.LongWeeks = 99;
        c.Skunkworks.LongBonusPermille = 2001;
        c.Skunkworks.EarlyPermille = 1000;
        c.Skunkworks.CostPerPersonWeek = 0;
        c.Skunkworks.LockWeeks = -1;
        c.Skunkworks.ResearchPayoffPermille = 0;
        c.Skunkworks.AlignmentPerHundred = 501;
        c.Skunkworks.MaxAlignment = 501;
        c.Skunkworks.DebtPerPersonWeek = 51;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minPeople", "maxSharePermille", "minWeeks", "longWeeks", "longBonusPermille", "earlyPermille", "costPerPersonWeek", "lockWeeks", "researchPayoffPermille", "alignmentPerHundred", "maxAlignment", "debtPerPersonWeek" })
            Assert.True(problems.Any(p => p.Contains("skunkworks." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_that_starts_nothing_is_exactly_as_it_was()
    {
        Game g = Lab();
        g.Weeks(60);
        Assert.Equal("", g.Me.SkunkKind);
        Assert.Empty(Of(g, EventKind.SkunkworksStarted));
        Assert.Empty(Of(g, EventKind.SkunkworksDone));
    }

    [Fact]
    public void A_research_project_takes_people_away_costs_the_budget_and_pays_by_the_person_weeks()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long normal = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 13));
        Assert.Equal(cash - 2 * 13 * 600, g.Me.Cash);
        Assert.Equal("research", g.Me.SkunkKind);
        Assert.Equal(2000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        Assert.Equal(4, LabQueries.StaffByFunction(g.C, g.Me, RoleFunction.Research));
        Assert.Equal(normal / 2, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick));
        GameEvent started = Of(g, EventKind.SkunkworksStarted).Single();
        Assert.Equal(1, started.SubjectId);
        Assert.Equal(2, started.Amount);
        Assert.Equal(13, started.Pick);
        Assert.Contains("pulled 2 research people into a 13-week skunkworks project", EventText.Describe(g.C, g.State, started));
        long points = g.Me.ResearchPoints;
        g.Weeks(12);
        Assert.Equal(24, g.Me.SkunkWork);
        Assert.Empty(Of(g, EventKind.SkunkworksDone));
        long per = PerResearcher(g);
        g.Weeks(1);
        GameEvent done = Of(g, EventKind.SkunkworksDone).Single();
        long expected = MathI.MulPermille(MathI.MulPermille(26 * per, 1500), 1250);
        Assert.Equal(expected, done.Amount);
        Assert.Equal(1, done.Pick);
        Assert.Contains("finished its research skunkworks project: " + expected + " research points", EventText.Describe(g.C, g.State, done));
        Assert.Equal("", g.Me.SkunkKind);
        Assert.Equal(0, g.Me.SkunkWork);
        Assert.Equal(g.State.Tick + 13 - 1, g.Me.SkunkLockedUntil);
        Assert.Equal(4000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        Assert.True(g.Me.ResearchPoints - points > expected, "points " + points + " -> " + g.Me.ResearchPoints);
    }

    [Fact]
    public void A_short_project_pays_no_long_bonus()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 12));
        g.Weeks(11);
        long per = PerResearcher(g);
        g.Weeks(1);
        Assert.Equal(MathI.MulPermille(24 * per, 1500), Of(g, EventKind.SkunkworksDone).Single().Amount);
    }

    [Fact]
    public void A_safety_project_adds_real_alignment_to_finished_models_only()
    {
        Game g = Lab(null, true);
        ModelState done = g.Me.Models.First(m => m.Status == ModelStatus.Trained);
        ModelState training = g.Train("mini", 100);
        int before = done.Alignment;
        int displayed = done.DisplayedAlignment;
        int alignWork = TrainingSystem.AlignmentWorkPermille(g.C, g.Me, g.State.Tick);
        g.Do(Command.StartSkunkworks(g.Id, "safety", 2, 13));
        Assert.Equal(4000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Safety));
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me, g.State.Tick) < alignWork);
        g.Weeks(13);
        GameEvent e = Of(g, EventKind.SkunkworksDone).Single();
        long gain = MathI.MulPermille(26 * 100, 1250) / 100;
        Assert.Equal(gain, e.Amount);
        Assert.Equal(before + gain, done.Alignment);
        Assert.True(done.DisplayedAlignment >= displayed);
        Assert.Contains("more alignment on its finished models", EventText.Describe(g.C, g.State, e));
        Assert.NotEqual(ModelStatus.Retired, training.Status);
    }

    [Fact]
    public void An_engineering_project_clears_technical_debt_and_never_below_nothing()
    {
        Game g = Lab();
        TechDebt.Add(g.C, g.State, g.Me, 300);
        Assert.Equal(300, g.Me.TechDebt);
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        g.Do(Command.StartSkunkworks(g.Id, "engineering", 2, 13));
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.Me) < speed);
        g.Weeks(13);
        GameEvent e = Of(g, EventKind.SkunkworksDone).Single();
        Assert.Equal(MathI.MulPermille(26, 1250), e.Amount);
        Assert.Equal(300 - e.Amount, g.Me.TechDebt);
        Assert.Contains("points of technical debt cleared", EventText.Describe(g.C, g.State, e));

        Game small = Lab();
        TechDebt.Add(small.C, small.State, small.Me, 10);
        small.Do(Command.StartSkunkworks(small.Id, "engineering", 2, 26));
        small.Weeks(26);
        Assert.Equal(10, Of(small, EventKind.SkunkworksDone).Single().Amount);
        Assert.Equal(0, small.Me.TechDebt);
    }

    [Fact]
    public void Wrapping_up_early_pays_a_smaller_share_of_the_work_so_far()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 26));
        g.Refused(Command.WrapUpSkunkworks(g.Id), CommandError.InvalidArgument);
        g.Weeks(10);
        long per = PerResearcher(g);
        g.Do(Command.WrapUpSkunkworks(g.Id));
        GameEvent e = Of(g, EventKind.SkunkworksDone).Single();
        Assert.Equal(0, e.Pick);
        Assert.Equal(MathI.MulPermille(MathI.MulPermille(20 * per, 1500), 500), e.Amount);
        Assert.Contains("wrapped up early", EventText.Describe(g.C, g.State, e));
        Assert.Equal("", g.Me.SkunkKind);
        Assert.True(g.Me.SkunkLockedUntil > g.State.Tick);
    }

    [Fact]
    public void Losing_people_mid_project_shrinks_the_work_and_the_pay()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 8));
        g.Weeks(2);
        Assert.Equal(4, g.Me.SkunkWork);
        for (int i = 0; i < 3; i++) g.Me.Staff.Remove(g.Me.Staff.First(p => p.RoleId == "researcher"));
        g.Weeks(2);
        Assert.Equal(6, g.Me.SkunkWork);
    }

    [Fact]
    public void A_finished_project_locks_the_next_for_a_while()
    {
        Game g = Lab(c => c.Skunkworks.LockWeeks = 5);
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 4));
        g.Weeks(4);
        string d = g.Sim.Digest();
        g.Refused(Command.StartSkunkworks(g.Id, "research", 2, 4), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Weeks(4);
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 4));
        Assert.Equal(2, Of(g, EventKind.SkunkworksStarted).Count);
    }

    [Fact]
    public void Refused_projects_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.StartSkunkworks(g.Id, "sales", 2, 13), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "research", 2, 3), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "research", 2, 27), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "research", 1, 13), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "research", 3, 13), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "safety", 2, 13), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "engineering", 2, 13), CommandError.InvalidArgument);
        g.Refused(Command.WrapUpSkunkworks(g.Id), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 13));
        d = g.Sim.Digest();
        g.Refused(Command.StartSkunkworks(g.Id, "research", 2, 13), CommandError.InvalidArgument);
        g.Refused(Command.StartSkunkworks(g.Id, "engineering", 2, 13), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());

        Game poor = Lab();
        poor.Me.Cash = 100;
        d = poor.Sim.Digest();
        Assert.False(poor.Sim.Apply(Command.StartSkunkworks(poor.Id, "research", 2, 13)).Ok);
        Assert.Equal(d, poor.Sim.Digest());
        Game off = Lab(c => c.Skunkworks.Enabled = 0);
        off.Refused(Command.StartSkunkworks(off.Id, "research", 2, 13), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.StartSkunkworks(rival.Id, "research", 2, 13)).Ok);
        Assert.False(w.Sim.Apply(Command.WrapUpSkunkworks(rival.Id)).Ok);
    }

    [Fact]
    public void A_project_and_a_mentoring_pair_both_take_their_toll_on_a_function()
    {
        Game g = Lab(c => c.Seniority.MaxSharePermille = 500);
        StaffMember mentor = g.Me.Staff.First(p => p.RoleId == "engineer");
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, g.Me.Staff.Where(p => p.RoleId == "engineer").Skip(1).First().Id));
        long speed = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering);
        Assert.Equal(2000 + 1000 + 800, speed);
        TechDebt.Add(g.C, g.State, g.Me, 100);
        g.Do(Command.StartSkunkworks(g.Id, "engineering", 2, 8));
        Assert.True(LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering) < speed);
    }

    [Fact]
    public void Save_and_load_keep_the_project_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 15));
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal("research", lab.SkunkKind);
        Assert.Equal(2, lab.SkunkPeople);
        Assert.Equal(12, lab.SkunkWork);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Single(back.State.Events, e => e.Kind == EventKind.SkunkworksDone);
    }

    [Fact]
    public void An_older_save_loads_with_no_project()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 15));
        string old = SaveSchemaPretend.At(g.C, 71, g.Sim.Save());
        foreach (string key in new[] { "skunkKind", "skunkPeople", "skunkStart", "skunkDue", "skunkWork", "skunkLockedUntil" }) Assert.DoesNotContain("\"" + key + "\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal("", lab.SkunkKind));
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.SkunkPeople + lab.SkunkStart + lab.SkunkDue + lab.SkunkWork + lab.SkunkLockedUntil));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_project()
    {
        Game g = Lab();
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 15));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        g.Me.SkunkKind = "marketing";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        g.Me.SkunkKind = "research";
        g.Me.SkunkPeople = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        g.Me.SkunkPeople = 2;
        g.Me.SkunkDue = g.Me.SkunkStart;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        g.Me.SkunkDue = g.Me.SkunkStart + 5;
        g.Me.SkunkWork = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        g.Me.SkunkWork = 0;
        g.Me.SkunkKind = "";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("skunkworks"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.SkunkKind = "research";
        rival.SkunkPeople = 1;
        rival.SkunkDue = 9;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("skunkworks"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(null, true);
        g.Do(Command.StartSkunkworks(g.Id, "safety", 2, 10));
        g.Weeks(4);
        g.Do(Command.WrapUpSkunkworks(g.Id));
        g.Weeks(14);
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 8));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_projects_stays_sound_and_the_books_balance()
    {
        Game g = Lab(null, true);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        string[] kinds = { "research", "safety", "engineering" };
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            if (i % 20 == 0) g.Try(Command.StartSkunkworks(g.Id, kinds[(i / 20) % 3], 2, 4 + i % 20));
            if (i % 20 == 8) g.Try(Command.WrapUpSkunkworks(g.Id));
        }
        Assert.NotEmpty(Of(g, EventKind.SkunkworksDone));
        Assert.Contains(Of(g, EventKind.SkunkworksDone), e => e.Pick == 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
