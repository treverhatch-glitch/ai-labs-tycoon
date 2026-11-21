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

/// <summary>Visiting scholars: seats, a stipend, research points each week and prestige when they leave on schedule.</summary>
public class ScholarsTests
{
    static Game Lab(Action<GameConfig> tweak = null, int spare = 3)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (spare > 0)
        {
            g.Do(Command.Fire(g.Id, "researcher", spare));
            g.Me.Alumni.Clear();
        }
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Scholars.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Scholars.Enabled = 2;
        c.Scholars.MaxScholars = 0;
        c.Scholars.Weeks = 0;
        c.Scholars.StipendPermille = 1001;
        c.Scholars.ResearchPermille = 1001;
        c.Scholars.PrestigeEach = 201;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxScholars", "weeks", "stipendPermille", "researchPermille", "prestigeEach" })
            Assert.True(problems.Any(p => p.Contains("scholars." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_group_needs_free_researcher_seats_cash_and_a_sane_size_and_one_group_at_a_time()
    {
        Game g = Lab();
        g.Refused(Command.HostScholars(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.HostScholars(g.Id, g.C.Scholars.MaxScholars + 1), CommandError.InvalidArgument);
        Game tight = Lab(spare: 1);
        tight.Refused(Command.HostScholars(tight.Id, 2), CommandError.NoSeats);
        g.Do(Command.HostScholars(g.Id, 2));
        Assert.Equal(2, g.Me.ScholarCount);
        g.Refused(Command.HostScholars(g.Id, 1), CommandError.InvalidArgument);
        Assert.Single(Of(g, EventKind.ScholarsArrived));
        Game poor = Lab();
        poor.Me.Cash = 0;
        poor.Refused(Command.HostScholars(poor.Id, 1), CommandError.NotEnoughCash);
        Game off = Lab(c => c.Scholars.Enabled = 0);
        off.Refused(Command.HostScholars(off.Id, 1), CommandError.InvalidArgument);
        off.Refused(Command.SendScholarsHome(off.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Scholars_hold_seats_so_a_hire_cannot_take_them()
    {
        Game g = Lab();
        Assert.Equal(3, LabQueries.FreeSeats(g.C, g.Me, "researcher"));
        g.Do(Command.HostScholars(g.Id, 2));
        Assert.Equal(1, LabQueries.FreeSeats(g.C, g.Me, "researcher"));
        g.Refused(Command.Hire(g.Id, "researcher", 2), CommandError.NoSeats);
        g.Do(Command.Hire(g.Id, "researcher", 1));
    }

    [Fact]
    public void They_cost_a_stipend_and_add_research_points_every_week()
    {
        Game g = Lab();
        Game twin = Lab();
        long total = Scholars.TotalStipend(g.C, g.State, g.Me, 2);
        g.Do(Command.HostScholars(g.Id, 2));
        Assert.Equal(total / g.C.Scholars.Weeks, Scholars.WeeklyStipend(g.C, g.State, g.Me));
        Assert.True(Scholars.WeeklyPoints(g.C, g.State, g.Me) > 0);
        g.Weeks(4);
        twin.Weeks(4);
        Assert.True(g.Me.Cash < twin.Me.Cash);
        Assert.True(g.Me.ResearchPoints > twin.Me.ResearchPoints);
    }

    [Fact]
    public void Leaving_on_schedule_adds_prestige_and_going_home_early_adds_none()
    {
        Game g = Lab();
        Assert.True(Prestige.Enabled(g.C));
        int before = g.Me.Prestige;
        g.Do(Command.HostScholars(g.Id, 2));
        g.Weeks(g.C.Scholars.Weeks);
        Assert.Equal(0, g.Me.ScholarCount);
        Assert.Equal(before + 2 * g.C.Scholars.PrestigeEach, g.Me.Prestige);
        GameEvent end = Of(g, EventKind.ScholarsLeft).Single();
        Assert.Equal(1, end.Pick);
        Assert.Equal(2 * g.C.Scholars.PrestigeEach, end.Amount);
        Game early = Lab();
        early.Do(Command.HostScholars(early.Id, 2));
        early.Weeks(3);
        int prestige = early.Me.Prestige;
        early.Do(Command.SendScholarsHome(early.Id));
        Assert.Equal(prestige, early.Me.Prestige);
        Assert.Equal(0, Of(early, EventKind.ScholarsLeft).Single().Pick);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Prestige_stops_at_its_maximum()
    {
        Game g = Lab();
        g.Me.Prestige = g.C.Prestige.Max - 5;
        g.Do(Command.HostScholars(g.Id, 3));
        g.Weeks(g.C.Scholars.Weeks);
        Assert.Equal(g.C.Prestige.Max, g.Me.Prestige);
        Assert.InRange(Of(g, EventKind.ScholarsLeft).Single().Amount, 1, 3 * g.C.Scholars.PrestigeEach - 1);
    }

    [Fact]
    public void A_save_with_scholars_loads_and_plays_on_the_same_and_an_old_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.HostScholars(g.Id, 2));
        g.Weeks(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Do(Command.HostScholars(g.Id, 1));
        g.Me.ScholarCount = 99;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("scholars"));
        Game fresh = Lab();
        string old = SaveSchemaPretend.At(fresh.C, 79, fresh.Sim.Save());
        Assert.DoesNotContain("scholarCount", old);
        Simulation back = Simulation.Load(fresh.C, old);
        Assert.Equal(0, back.State.Labs.First(l => l.Id == fresh.Id).ScholarCount);
    }
}
