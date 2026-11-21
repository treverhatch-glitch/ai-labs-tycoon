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

/// <summary>Internships: a cohort costs a stipend, holds no seat, and is offered jobs at the end for a share of the fee.</summary>
public class InternshipsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        return Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static Game Spare(string role, int seats)
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, role, seats));
        g.Me.Alumni.Clear();
        return g;
    }

    static int Free(Game g, string role) { return LabQueries.Seats(g.C, g.Me, role) - LabQueries.StaffCount(g.Me, role); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Internships.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Internships.Enabled = 2;
        c.Internships.MaxCohort = 0;
        c.Internships.Weeks = 0;
        c.Internships.StipendPermille = 600;
        c.Internships.ConvertPermille = 500;
        c.Internships.ConvertMorale = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxCohort", "weeks", "convertPermille", "convertMorale" })
            Assert.True(problems.Any(p => p.Contains("internships." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_cohort_needs_cash_a_known_role_and_a_sane_size_and_only_one_at_a_time()
    {
        Game g = Lab();
        g.Refused(Command.StartInternships(g.Id, "nobody", 2), CommandError.UnknownId);
        g.Refused(Command.StartInternships(g.Id, "researcher", 0), CommandError.InvalidArgument);
        g.Refused(Command.StartInternships(g.Id, "researcher", g.C.Internships.MaxCohort + 1), CommandError.InvalidArgument);
        g.Do(Command.StartInternships(g.Id, "researcher", 2));
        Assert.Equal(2, g.Me.InternCount);
        Assert.Equal("researcher", g.Me.InternRole);
        g.Refused(Command.StartInternships(g.Id, "engineer", 1), CommandError.InvalidArgument);
        Assert.Single(Of(g, EventKind.InternshipStarted));
        Game poor = Lab(c => c.PlayerStart.Cash = 1_000_000);
        poor.Me.Cash = 0;
        poor.Refused(Command.StartInternships(poor.Id, "researcher", 1), CommandError.NotEnoughCash);
        Game off = Lab(c => c.Internships.Enabled = 0);
        off.Refused(Command.StartInternships(off.Id, "researcher", 1), CommandError.InvalidArgument);
        off.Refused(Command.EndInternships(off.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Interns_cost_a_stipend_each_week_and_hold_no_seat()
    {
        Game g = Lab();
        long fee = Internships.Fee(g.C, g.State, g.Me, "researcher");
        long total = Internships.TotalStipend(g.C, g.State, g.Me, "researcher", 2);
        Assert.Equal(MathI.MulPermille(fee, g.C.Internships.StipendPermille) * 2, total);
        int staff = g.Me.Staff.Count;
        g.Do(Command.StartInternships(g.Id, "researcher", 2));
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Equal(total / g.C.Internships.Weeks, Internships.WeeklyStipend(g.C, g.State, g.Me));
        Game twin = Lab();
        g.Weeks(5);
        twin.Weeks(5);
        Assert.True(g.Me.Cash < twin.Me.Cash);
        Assert.Equal(2, g.Me.InternCount);
    }

    [Fact]
    public void At_the_end_each_intern_is_taken_on_for_a_share_of_the_fee_while_seats_and_cash_last()
    {
        Game g = Spare("engineer", 2);
        int free = Free(g, "engineer");
        Assert.Equal(2, free);
        long fee = MathI.MulPermille(Internships.Fee(g.C, g.State, g.Me, "engineer"), g.C.Internships.ConvertPermille);
        int staff = g.Me.Staff.Count;
        int cohort = (int)MathI.Min(g.C.Internships.MaxCohort, free + 1);
        g.Do(Command.StartInternships(g.Id, "engineer", cohort));
        g.Weeks(g.C.Internships.Weeks);
        GameEvent end = Of(g, EventKind.InternshipsEnded).Single();
        Assert.Equal(1, end.Pick);
        Assert.Equal(cohort, end.SubjectId);
        Assert.Equal(free, (int)end.Amount);
        Assert.Equal(staff + free, g.Me.Staff.Count);
        Assert.Equal(0, g.Me.InternCount);
        Assert.Equal(cohort - free, g.Me.Alumni.Count(a => a.RoleId == "engineer" && a.Terms == Alumni.Good));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.True(fee > 0);
    }

    [Fact]
    public void Ending_early_takes_nobody_on_and_the_cash_wall_stops_a_conversion()
    {
        Game g = Lab();
        g.Do(Command.StartInternships(g.Id, "researcher", 2));
        g.Weeks(3);
        int staff = g.Me.Staff.Count;
        g.Do(Command.EndInternships(g.Id));
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Equal(2, g.Me.Alumni.Count);
        Assert.Equal(0, Of(g, EventKind.InternshipsEnded).Single().Pick);
        Game broke = Spare("engineer", 1);
        Assert.Equal(1, Free(broke, "engineer"));
        broke.Do(Command.StartInternships(broke.Id, "engineer", 1));
        broke.Weeks(broke.C.Internships.Weeks - 1);
        broke.Me.Cash = 0;
        broke.Weeks(1);
        Assert.Equal(0, Of(broke, EventKind.InternshipsEnded).Single().Amount);
        Assert.Single(broke.Me.Alumni);
    }

    [Fact]
    public void A_taken_on_intern_arrives_in_good_spirits_and_a_rival_has_no_interns()
    {
        Game g = Spare("engineer", 1);
        long fee = Internships.Fee(g.C, g.State, g.Me, "engineer");
        g.Do(Command.StartInternships(g.Id, "engineer", 1));
        g.Weeks(g.C.Internships.Weeks);
        StaffMember last = g.Me.Staff.Last(x => x.RoleId == "engineer");
        Assert.Equal("engineer", last.RoleId);
        Assert.True(last.Morale >= Traits.RestingMorale(g.C, g.Me, last));
        Assert.True(fee > 0);
        LabState other = g.State.Labs.FirstOrDefault(l => l.Id != g.Id);
        if (other != null) Assert.Equal(0, other.InternCount);
    }

    [Fact]
    public void A_save_with_interns_loads_and_plays_on_the_same_and_an_old_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.StartInternships(g.Id, "researcher", 2));
        g.Weeks(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        loaded.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Me.InternCount = 99;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("interns"));
        Game fresh = Lab();
        string old = SaveSchemaPretend.At(fresh.C, 76, fresh.Sim.Save());
        Assert.DoesNotContain("internRole", old);
        Simulation back = Simulation.Load(fresh.C, old);
        Assert.Equal(0, back.State.Labs.First(l => l.Id == fresh.Id).InternCount);
    }
}
