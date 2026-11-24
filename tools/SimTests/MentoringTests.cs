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

/// <summary>Mentoring: a senior and a junior in one function trade output for a free promotion, if the pair holds and a seat is free.</summary>
public class MentoringTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        return Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Seniority.MaxSharePermille = 500;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 6);
    }

    static StaffMember Of(Game g, string role, int skip = 0) { return g.Me.Staff.Where(p => p.RoleId == role).Skip(skip).First(); }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static (StaffMember mentor, StaffMember mentee) Pair(Game g, string role = "researcher")
    {
        StaffMember mentor = Of(g, role);
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        StaffMember mentee = Of(g, role, 1);
        g.Do(Command.AssignMentor(g.Id, mentor.Id, mentee.Id));
        return (mentor, mentee);
    }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Mentoring.Enabled);
        Assert.Equal(2, c.Mentoring.MaxPairs);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Mentoring.Enabled = 2;
        c.Mentoring.MaxPairs = 0;
        c.Mentoring.Weeks = 0;
        c.Mentoring.MenteeWeightPermille = 1001;
        c.Mentoring.MoralePerWeek = 51;
        c.Mentoring.MoraleCeiling = 0;
        c.Mentoring.GraduateMorale = 501;
        c.Mentoring.MentorMorale = -1;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxPairs", "weeks", "menteeWeightPermille", "moralePerWeek", "moraleCeiling", "graduateMorale", "mentorMorale" })
            Assert.True(problems.Any(p => p.Contains("mentoring." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void Nobody_is_mentored_until_the_player_says_so()
    {
        Game g = Lab();
        g.Weeks(40);
        Assert.All(g.Me.Staff, p => Assert.Equal(0, p.MentorId + p.MentorWeeks));
        Assert.Empty(Of(g, EventKind.MentoringStarted));
        Assert.Empty(Of(g, EventKind.MentoringEnded));
    }

    [Fact]
    public void A_pair_costs_output_while_it_lasts()
    {
        Game g = Lab();
        StaffMember mentor = Of(g, "researcher");
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        Assert.Equal(3000 + 1600, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        long research = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        StaffMember mentee = Of(g, "researcher", 1);
        g.Do(Command.AssignMentor(g.Id, mentor.Id, mentee.Id));
        Assert.Equal(mentor.Id, mentee.MentorId);
        Assert.Equal(0, mentee.MentorWeeks);
        Assert.Equal(2 * 1000 + 800 + 1000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        Assert.True(TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick) < research);
        GameEvent e = Of(g, EventKind.MentoringStarted).Single();
        Assert.Equal(mentee.Id, e.SubjectId);
        Assert.Equal(mentor.Id, e.Amount);
        Assert.Contains("paired a senior with a junior researcher", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_juniors_morale_climbs_each_week_up_to_the_ceiling()
    {
        Game g = Lab();
        var (_, mentee) = Pair(g);
        mentee.Morale = 500;
        Game still = Lab(c => c.Mentoring.MoralePerWeek = 0);
        var (_, flat) = Pair(still);
        flat.Morale = 500;
        still.Weeks(5);
        g.Weeks(5);
        Assert.Equal(5, mentee.MentorWeeks);
        Assert.True(mentee.Morale > flat.Morale, "morale " + mentee.Morale + " against " + flat.Morale);
        Game cap = Lab(c => { c.Mentoring.MoralePerWeek = 30; c.Mentoring.MoraleCeiling = 600; });
        var (_, capped) = Pair(cap);
        Game control = Lab();
        StaffMember bystander = Of(control, "researcher", 1);
        capped.Morale = 900;
        bystander.Morale = 900;
        cap.Weeks(1);
        control.Weeks(1);
        Assert.Equal(bystander.Morale, capped.Morale);
        capped.Morale = 590;
        bystander.Morale = 590;
        cap.Weeks(1);
        control.Weeks(1);
        Assert.InRange(capped.Morale - bystander.Morale, 1, 10);
    }

    [Fact]
    public void A_pair_that_holds_graduates_the_junior_for_nothing_and_lifts_both()
    {
        Game g = Lab(c => c.Mentoring.Weeks = 6);
        var (mentor, mentee) = Pair(g);
        long cash = g.Me.Cash;
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        mentee.Morale = 400;
        mentor.Morale = 500;
        g.Weeks(5);
        Assert.Equal(0, mentee.Senior);
        g.Weeks(1);
        Assert.Equal(1, mentee.Senior);
        Assert.Equal(0, mentee.MentorId);
        Assert.Equal(0, mentee.MentorWeeks);
        GameEvent e = Of(g, EventKind.MentoringEnded).Single();
        Assert.Equal(0, e.Pick);
        Assert.Equal(6, e.Amount);
        Assert.Contains("graduated a junior researcher to senior after 6 weeks", EventText.Describe(g.C, g.State, e));
        Assert.Equal(2 * 1600 + 2 * 1000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        Assert.DoesNotContain(Of(g, EventKind.StaffPromoted), p => p.SubjectId == mentee.Id);
        Assert.True(LabQueries.WeeklySalaries(g.C, g.Me) > salaries, "the new senior draws the senior salary");
        Assert.True(mentor.Morale > 500 - 1);
    }

    [Fact]
    public void A_pair_waits_for_a_senior_seat_and_graduates_when_one_opens()
    {
        Game g = Lab(c => { c.Mentoring.Weeks = 4; c.Seniority.MaxSharePermille = 250; });
        var (mentor, mentee) = Pair(g);
        Assert.Equal(1, Seniority.Allowed(g.C, g.Me, RoleFunction.Research));
        g.Weeks(10);
        Assert.Equal(10, mentee.MentorWeeks);
        Assert.Equal(0, mentee.Senior);
        Assert.Empty(Of(g, EventKind.MentoringEnded));
        g.C.Seniority.MaxSharePermille = 500;
        g.Weeks(1);
        Assert.Equal(1, mentee.Senior);
        Assert.Equal(0, Of(g, EventKind.MentoringEnded).Single().Pick);
    }

    [Fact]
    public void A_pair_breaks_up_when_a_person_leaves_or_the_junior_is_promoted_another_way()
    {
        Game gone = Lab();
        var (mentor, mentee) = Pair(gone);
        gone.Weeks(3);
        gone.Me.Staff.Remove(mentor);
        gone.Weeks(1);
        Assert.Equal(0, mentee.MentorId);
        GameEvent e = Of(gone, EventKind.MentoringEnded).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(3, e.Amount);
        Assert.Contains("broke up", EventText.Describe(gone.C, gone.State, e));

        Game promoted = Lab();
        var (_, junior) = Pair(promoted);
        promoted.Weeks(2);
        promoted.Do(Command.PromoteStaff(promoted.Id, junior.Id));
        promoted.Weeks(1);
        Assert.Equal(0, junior.MentorId);
        Assert.Equal(1, Of(promoted, EventKind.MentoringEnded).Single().Pick);
    }

    [Fact]
    public void Calling_a_pair_off_loses_the_weeks_and_restores_the_weights()
    {
        Game g = Lab();
        var (mentor, mentee) = Pair(g);
        g.Weeks(7);
        g.Do(Command.EndMentoring(g.Id, mentee.Id));
        Assert.Equal(0, mentee.MentorId);
        Assert.Equal(0, mentee.MentorWeeks);
        Assert.Equal(3000 + 1600, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        GameEvent e = Of(g, EventKind.MentoringEnded).Single();
        Assert.Equal(2, e.Pick);
        Assert.Equal(7, e.Amount);
        Assert.Contains("called off", EventText.Describe(g.C, g.State, e));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, mentee.Id));
        Assert.Equal(0, mentee.MentorWeeks);
    }

    [Fact]
    public void Refused_pairings_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab(c => c.Mentoring.MaxPairs = 1);
        StaffMember mentor = Of(g, "researcher");
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        StaffMember mentor2 = Of(g, "researcher", 1);
        g.Do(Command.PromoteStaff(g.Id, mentor2.Id));
        StaffMember junior = Of(g, "researcher", 2);
        StaffMember junior2 = Of(g, "researcher", 3);
        StaffMember engineer = Of(g, "engineer");
        string d = g.Sim.Digest();
        g.Refused(Command.AssignMentor(g.Id, 999, junior.Id), CommandError.UnknownId);
        g.Refused(Command.AssignMentor(g.Id, mentor.Id, 999), CommandError.UnknownId);
        g.Refused(Command.AssignMentor(g.Id, mentor.Id, mentor.Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignMentor(g.Id, junior.Id, junior2.Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignMentor(g.Id, mentor.Id, mentor2.Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignMentor(g.Id, mentor.Id, engineer.Id), CommandError.InvalidArgument);
        g.Refused(Command.EndMentoring(g.Id, junior.Id), CommandError.InvalidArgument);
        g.Refused(Command.EndMentoring(g.Id, 999), CommandError.UnknownId);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.AssignMentor(g.Id, mentor.Id, junior.Id));
        d = g.Sim.Digest();
        g.Refused(Command.AssignMentor(g.Id, mentor2.Id, junior.Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignMentor(g.Id, mentor.Id, junior2.Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignMentor(g.Id, mentor2.Id, junior2.Id), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());

        Game off = Lab(c => c.Mentoring.Enabled = 0);
        StaffMember m = Of(off, "researcher");
        off.Do(Command.PromoteStaff(off.Id, m.Id));
        off.Refused(Command.AssignMentor(off.Id, m.Id, Of(off, "researcher", 1).Id), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.AssignMentor(rival.Id, 1, 2)).Ok);
        Assert.False(w.Sim.Apply(Command.EndMentoring(rival.Id, 1)).Ok);
    }

    [Fact]
    public void A_pair_in_another_function_runs_alongside_and_each_counts_in_its_own()
    {
        Game g = Lab();
        Pair(g, "researcher");
        Pair(g, "engineer");
        Assert.Equal(2 * 1000 + 800 + 1000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
        Assert.Equal(2 * 1000 + 800 + 1000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering));
        Assert.Equal(3000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Safety));
        Assert.Equal(2, Mentoring.Pairs(g.Me));
    }

    [Fact]
    public void Save_and_load_keep_the_pair_and_play_on_the_same()
    {
        Game g = Lab(c => c.Mentoring.Weeks = 12);
        var (mentor, mentee) = Pair(g);
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        StaffMember again = back.State.Lab(g.Id).Staff.First(p => p.Id == mentee.Id);
        Assert.Equal(mentor.Id, again.MentorId);
        Assert.Equal(5, again.MentorWeeks);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Single(back.State.Events, e => e.Kind == EventKind.MentoringEnded);
    }

    [Fact]
    public void An_older_save_loads_with_nobody_mentored()
    {
        Game g = Lab();
        Pair(g);
        string old = SaveSchemaPretend.At(g.C, 70, g.Sim.Save());
        foreach (string key in new[] { "mentorId", "mentorWeeks" }) Assert.DoesNotContain("\"" + key + "\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.All(lab.Staff, p => Assert.Equal(0, p.MentorId + p.MentorWeeks)));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_pair()
    {
        Game g = Lab();
        var (mentor, mentee) = Pair(g);
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("mentor"));
        mentee.MentorId = mentee.Id;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("mentor"));
        mentee.MentorId = mentor.Id;
        mentee.MentorWeeks = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("mentor"));
        mentee.MentorId = 0;
        mentee.MentorWeeks = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("mentor"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.Staff.Add(new StaffMember { Id = 77, RoleId = "researcher", MentorId = 5 });
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("mentor"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(c => c.Mentoring.Weeks = 8);
        var (mentor, mentee) = Pair(g);
        g.Weeks(4);
        g.Do(Command.EndMentoring(g.Id, mentee.Id));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, mentee.Id));
        g.Weeks(12);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_mentoring_stays_sound_and_the_books_balance()
    {
        Game g = Lab(c => c.Mentoring.Weeks = 20);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 260; i++)
        {
            g.Weeks(1);
            if (i % 25 == 0)
            {
                StaffMember mentor = g.Me.Staff.FirstOrDefault(p => p.RoleId == "engineer" && p.Senior == 1);
                if (mentor == null)
                {
                    StaffMember pick = g.Me.Staff.FirstOrDefault(p => p.RoleId == "engineer" && p.Senior == 0 && p.MentorId == 0);
                    if (pick != null && g.Try(Command.PromoteStaff(g.Id, pick.Id)).Ok) mentor = pick;
                }
                StaffMember junior = g.Me.Staff.FirstOrDefault(p => p.RoleId == "engineer" && p.Senior == 0 && p.MentorId == 0);
                if (mentor != null && junior != null) g.Try(Command.AssignMentor(g.Id, mentor.Id, junior.Id));
            }
        }
        Assert.NotEmpty(Of(g, EventKind.MentoringEnded));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
