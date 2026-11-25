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

/// <summary>Endowed chairs: a big payment buys research points, a floor under prestige and a stipend, each chair dearer than the last.</summary>
public class ChairsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: 6);
        return g;
    }

    static List<GameEvent> Endowed(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.ChairEndowed).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Chairs.MaxChairs);
        Assert.Equal(150000, c.Chairs.CostBase);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Chairs.MaxChairs = 11;
        c.Chairs.CostBase = 0;
        c.Chairs.StipendPerWeek = -1;
        c.Chairs.ResearchPerWeek = 201;
        c.Chairs.PrestigeFloor = 400;
        c.Chairs.MinResearchers = 51;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxChairs", "costBase", "stipendPerWeek", "researchPerWeek", "prestigeFloor", "minResearchers" })
            Assert.Contains(problems, p => p.Contains("chairs." + key));
    }

    [Fact]
    public void No_chair_is_ever_endowed_by_itself()
    {
        Game g = Lab();
        g.Weeks(20);
        Assert.Equal(0, g.Me.Chairs);
        Assert.Equal(0, Chairs.Floor(g.C, g.Me));
        Assert.Equal(0, Chairs.Research(g.C, g.Me));
        Assert.Equal(0, Chairs.Stipends(g.C, g.Me));
        Assert.Empty(Endowed(g));
    }

    [Fact]
    public void Each_chair_costs_more_than_the_last_and_the_payment_is_the_price()
    {
        Game g = Lab();
        Assert.Equal(150000, Chairs.NextCost(g.C, g.Me));
        long cash = g.Me.Cash;
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(cash - 150000, g.Me.Cash);
        Assert.Equal(300000, Chairs.NextCost(g.C, g.Me));
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(450000, Chairs.NextCost(g.C, g.Me));
        Assert.Equal(2, Endowed(g).Count);
        GameEvent e = Endowed(g).Last();
        Assert.Equal(2, e.SubjectId);
        Assert.Equal(300000, e.Amount);
        Assert.Contains("holds 2 chairs", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_chair_adds_research_points_every_week()
    {
        Game g = Lab();
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(before + 14, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick));
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(before + 28, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick));
    }

    [Fact]
    public void A_chair_puts_a_floor_under_prestige_that_papers_still_climb_from()
    {
        Game g = Lab();
        Assert.Equal(0, Prestige.Level(g.C, g.Me));
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(120, Prestige.Level(g.C, g.Me));
        g.Weeks(50);
        Assert.Equal(120, Prestige.Level(g.C, g.Me));
        g.Me.Prestige = 600;
        Assert.Equal(600, Prestige.Level(g.C, g.Me));
        g.Weeks(150);
        Assert.Equal(120, Prestige.Level(g.C, g.Me));
        g.Do(Command.EndowChair(g.Id));
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(360, Prestige.Level(g.C, g.Me));
    }

    [Fact]
    public void The_stipend_is_in_the_weekly_bill_and_leaves_no_trace_for_a_lab_with_none()
    {
        Game g = Lab();
        long before = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(before + 1200, LabQueries.WeeklySalaries(g.C, g.Me));
        Game twin = Lab();
        Assert.Equal(before, LabQueries.WeeklySalaries(twin.C, twin.Me));
    }

    [Fact]
    public void The_chair_pays_for_itself_only_slowly()
    {
        Game g = Lab();
        long cost = Chairs.NextCost(g.C, g.Me);
        long weeksToRecoverInPoints = cost / MathI.Max(1, g.C.Chairs.ResearchPerWeek - g.C.Chairs.StipendPerWeek / 100);
        Assert.True(weeksToRecoverInPoints > 52);
    }

    [Fact]
    public void A_full_set_of_chairs_cannot_be_exceeded_and_a_lab_needs_researchers_and_cash()
    {
        Game g = Lab();
        g.Do(Command.EndowChair(g.Id));
        g.Do(Command.EndowChair(g.Id));
        g.Do(Command.EndowChair(g.Id));
        g.Refused(Command.EndowChair(g.Id), CommandError.InvalidArgument);
        Game small = Game.Solo(4, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 2, engineers: 1, gpus: 4);
        small.Refused(Command.EndowChair(small.Id), CommandError.NotEnoughStaff);
        Game poor = Lab();
        poor.Me.Cash = 1000;
        poor.Refused(Command.EndowChair(poor.Id), CommandError.NotEnoughCash);
        Assert.Equal(0, poor.Me.Chairs);
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_it_switched_off_cannot_endow_one()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.EndowChair(rival.Id)).Ok);
        Game off = Lab(c => c.Chairs.MaxChairs = 0);
        off.Refused(Command.EndowChair(off.Id), CommandError.InvalidArgument);
        Assert.Equal(0, off.Me.Chairs);
    }

    [Fact]
    public void Chairs_help_a_grant_application_through_the_prestige_floor()
    {
        Game g = Lab();
        g.Me.Reputation = 100;
        int before = ResearchGrants.Score(g.C, g.Me);
        g.Do(Command.EndowChair(g.Id));
        Assert.Equal(before + 120, ResearchGrants.Score(g.C, g.Me));
    }

    [Fact]
    public void Save_and_load_keep_the_chairs_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.EndowChair(g.Id));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(1, back.State.Lab(g.Id).Chairs);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_chairs()
    {
        Game g = Lab();
        g.Do(Command.EndowChair(g.Id));
        string old = SaveSchemaPretend.At(g.C, 64, g.Sim.Save());
        Assert.DoesNotContain("\"chairs\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.Chairs));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_impossible_chairs()
    {
        Game g = Lab();
        g.Me.Chairs = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("university chairs"));
        g.Me.Chairs = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("university chairs"));
        g.Me.Chairs = 2;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("university chairs"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).Chairs = 1;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("university chairs"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        g.Weeks(4);
        g.Do(Command.EndowChair(g.Id));
        g.Weeks(10);
        g.Do(Command.EndowChair(g.Id));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_chairs_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 4, safety: 2, sales: 2, gpus: 8);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i == 10 || i == 150) g.Try(Command.EndowChair(g.Id));
        }
        Assert.Equal(2, g.Me.Chairs);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
