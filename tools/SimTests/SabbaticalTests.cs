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

/// <summary>Sabbaticals: a person away on pay does no work and cannot quit, and comes back according to how they went.</summary>
public class SabbaticalTests
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

    static StaffMember Person(Game g, string role, int n = 0) { return g.Me.Staff.Where(p => p.RoleId == role).ElementAt(n); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Sabbatical.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Sabbatical.Enabled = 2;
        c.Sabbatical.Weeks = 0;
        c.Sabbatical.MaxAway = 0;
        c.Sabbatical.BurnoutBelow = 900;
        c.Sabbatical.RestlessAbove = 800;
        c.Sabbatical.RefreshedMorale = 300;
        c.Sabbatical.RestlessMorale = 400;
        c.Sabbatical.RestedBonus = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "weeks", "maxAway", "burnoutBelow", "refreshedMorale", "restedBonus" })
            Assert.True(problems.Any(p => p.Contains("sabbatical." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void The_commands_are_checked_and_the_number_away_is_capped()
    {
        Game g = Lab();
        g.Refused(Command.SendOnSabbatical(g.Id, 99999), CommandError.UnknownId);
        g.Refused(Command.RecallFromSabbatical(g.Id, Person(g, "researcher").Id), CommandError.InvalidArgument);
        g.Do(Command.SendOnSabbatical(g.Id, Person(g, "researcher", 0).Id));
        g.Refused(Command.SendOnSabbatical(g.Id, Person(g, "researcher", 0).Id), CommandError.InvalidArgument);
        g.Do(Command.SendOnSabbatical(g.Id, Person(g, "researcher", 1).Id));
        g.Refused(Command.SendOnSabbatical(g.Id, Person(g, "researcher", 2).Id), CommandError.InvalidArgument);
        Assert.Equal(2, Sabbatical.Away(g.Me));
        Assert.Equal(2, Of(g, EventKind.SabbaticalStarted).Count);
        Game off = Lab(c => c.Sabbatical.Enabled = 0);
        off.Refused(Command.SendOnSabbatical(off.Id, Person(off, "researcher").Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_person_in_a_mentoring_pair_cannot_go()
    {
        Game g = Lab();
        StaffMember mentor = Person(g, "researcher", 0);
        StaffMember junior = Person(g, "researcher", 1);
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, junior.Id));
        g.Refused(Command.SendOnSabbatical(g.Id, junior.Id), CommandError.InvalidArgument);
        g.Refused(Command.SendOnSabbatical(g.Id, mentor.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Someone_away_does_no_work()
    {
        Game g = Lab();
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        long heads = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research);
        g.Do(Command.SendOnSabbatical(g.Id, Person(g, "researcher").Id));
        Assert.True(TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick) < before);
        Assert.True(LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research) < heads);
    }

    [Fact]
    public void A_worn_down_person_returns_refreshed_a_happy_one_restless_and_the_rest_a_little_better_than_settled()
    {
        Game g = Lab();
        StaffMember worn = Person(g, "researcher", 0);
        StaffMember happy = Person(g, "researcher", 1);
        worn.Morale = 100;
        happy.Morale = 900;
        g.Do(Command.SendOnSabbatical(g.Id, worn.Id));
        g.Do(Command.SendOnSabbatical(g.Id, happy.Id));
        g.Weeks(g.C.Sabbatical.Weeks);
        Assert.Equal(0, worn.LeaveDue);
        Assert.InRange(worn.Morale, g.C.Sabbatical.RefreshedMorale - 20, g.C.Sabbatical.RefreshedMorale);
        Assert.InRange(happy.Morale, g.C.Sabbatical.RestlessMorale, g.C.Sabbatical.RestlessMorale + 20);
        Assert.Equal(new[] { Sabbatical.Refreshed, Sabbatical.Restless }, Of(g, EventKind.SabbaticalEnded).Select(e => e.Pick).OrderByDescending(x => x).ToArray());
        StaffMember mid = Person(g, "engineer", 0);
        mid.Morale = 500;
        g.Do(Command.SendOnSabbatical(g.Id, mid.Id));
        g.Weeks(g.C.Sabbatical.Weeks);
        Assert.InRange(mid.Morale, Traits.RestingMorale(g.C, g.Me, mid) + g.C.Sabbatical.RestedBonus - 20, Traits.RestingMorale(g.C, g.Me, mid) + g.C.Sabbatical.RestedBonus + 20);
        Assert.Equal(Sabbatical.Rested, Of(g, EventKind.SabbaticalEnded).Last().Pick);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Recalling_early_keeps_the_morale_they_left_with_and_nobody_quits_while_away()
    {
        Game g = Lab();
        StaffMember p = Person(g, "researcher");
        p.Morale = 420;
        g.Do(Command.SendOnSabbatical(g.Id, p.Id));
        g.Weeks(2);
        g.Do(Command.RecallFromSabbatical(g.Id, p.Id));
        Assert.Equal(420, p.Morale);
        Assert.Equal(Sabbatical.Recalled, Of(g, EventKind.SabbaticalEnded).Single().Pick);
        Func<bool, Game> quitters = away =>
        {
            Game x = Lab(c => c.StaffPeople.Traits.First(t => t.QuitBelowMorale <= 0).QuitBelowMorale = c.StaffPeople.MoraleBaseline);
            StaffMember q = Person(x, "researcher");
            q.Morale = 0;
            if (away) x.Do(Command.SendOnSabbatical(x.Id, q.Id));
            x.Weeks(1);
            return x;
        };
        Assert.Contains(quitters(false).State.Events, e => e.Kind == EventKind.StaffQuit);
        Game stayed = quitters(true);
        Assert.DoesNotContain(stayed.State.Events, e => e.Kind == EventKind.StaffQuit && e.SubjectId == Person(stayed, "researcher").Id);
    }

    [Fact]
    public void A_save_with_someone_away_loads_and_plays_on_the_same_and_an_old_save_has_nobody_away()
    {
        Game g = Lab();
        g.Do(Command.SendOnSabbatical(g.Id, Person(g, "researcher").Id));
        g.Weeks(3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Me.Staff[0].LeaveMorale = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("sabbatical"));
        Game fresh = Lab();
        string old = SaveSchemaPretend.At(fresh.C, 78, fresh.Sim.Save());
        Assert.DoesNotContain("leaveDue", old);
        Simulation back = Simulation.Load(fresh.C, old);
        Assert.Equal(0, Sabbatical.Away(back.State.Labs.First(l => l.Id == fresh.Id)));
    }
}
