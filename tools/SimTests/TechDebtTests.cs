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

/// <summary>Technical debt: cutting corners piles it up, it raises the weekly incident chance, and only paying it down with cash and engineers clears it.</summary>
public class TechDebtTests
{
    const string Sector = "web-ads";

    static Game Lab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.SectorFlavours.Clear();
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab(researchers: 4, engineers: 3, gpus: 10);
        g.Me.Cash = 1_000_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        m.Capability = 1_000_000;
        return m;
    }

    static List<GameEvent> Paid(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.DebtPaid).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.TechDebt.Enabled);
        Assert.Equal(40, c.TechDebt.RushPerWeek);
        Assert.Empty(ConfigValidator.Problems(c));
        c.TechDebt.Enabled = 2;
        c.TechDebt.RushPerWeek = 201;
        c.TechDebt.CrunchPerWeek = -1;
        c.TechDebt.FineTunePatch = 201;
        c.TechDebt.IncidentAtFull = 201;
        c.TechDebt.CostPerPoint = 0;
        c.TechDebt.MinEngineers = 51;
        c.TechDebt.MaxPerCommand = 0;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "rushPerWeek", "crunchPerWeek", "fineTunePatch", "incidentAtFull", "costPerPoint", "minEngineers", "maxPerCommand" })
            Assert.Contains(problems, p => p.Contains("techDebt." + key));
    }

    [Fact]
    public void A_lab_that_cuts_no_corners_never_has_any_debt()
    {
        Game g = Lab();
        Trained(g);
        g.Weeks(10);
        Assert.Equal(0, g.Me.TechDebt);
        Assert.Equal(0, TechDebt.IncidentPermille(g.C, g.Me));
    }

    [Fact]
    public void A_rushed_launch_adds_debt_for_each_week_skipped()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.RushRelease(g.Id, m.Id, 4, Sector));
        Assert.Equal(4 * g.C.TechDebt.RushPerWeek, g.Me.TechDebt);
        Game twin = Lab();
        ModelState t = Trained(twin);
        twin.Do(Command.ReleaseModel(twin.Id, t.Id, Sector));
        Assert.Equal(0, twin.Me.TechDebt);
    }

    [Fact]
    public void A_refused_rush_adds_nothing()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        string before = g.Sim.Digest();
        g.Refused(Command.RushRelease(g.Id, m.Id, g.C.Rush.MaxWeeks + 1, Sector), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        Assert.Equal(0, g.Me.TechDebt);
    }

    [Fact]
    public void Every_week_of_crunch_adds_debt()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(5);
        Assert.Equal(5 * g.C.TechDebt.CrunchPerWeek, g.Me.TechDebt);
        g.Do(Command.SetCrunch(g.Id, false));
        int debt = g.Me.TechDebt;
        g.Weeks(5);
        Assert.Equal(debt, g.Me.TechDebt);
    }

    [Fact]
    public void Patching_a_glitching_model_adds_debt()
    {
        Game g = Lab();
        ModelState older = Trained(g);
        g.Release(older, Sector);
        ModelState newer = Trained(g);
        g.Release(newer, Sector);
        newer.Incidents = 1;
        newer.LastIncidentTick = g.State.Tick;
        g.Do(Command.FineTuneModel(g.Id, newer.Id));
        Assert.Equal(g.C.TechDebt.FineTunePatch, g.Me.TechDebt);
    }

    [Fact]
    public void Debt_stops_at_the_cap()
    {
        Game g = Lab();
        g.Me.TechDebt = 990;
        TechDebt.Add(g.C, g.State, g.Me, 500);
        Assert.Equal(1000, g.Me.TechDebt);
        TechDebt.Add(g.C, g.State, g.Me, -50);
        TechDebt.Add(g.C, g.State, g.Me, 0);
        Assert.Equal(1000, g.Me.TechDebt);
    }

    [Fact]
    public void Debt_never_wears_off_by_itself()
    {
        Game g = Lab();
        g.Me.TechDebt = 300;
        g.Weeks(30);
        Assert.Equal(300, g.Me.TechDebt);
    }

    [Fact]
    public void Debt_raises_the_weekly_incident_chance_in_proportion()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        int clean = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Me.TechDebt = 500;
        int half = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Me.TechDebt = 1000;
        int full = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(20, half - clean);
        Assert.Equal(40, full - clean);
        Assert.Equal(40, TechDebt.IncidentPermille(g.C, g.Me));
    }

    [Fact]
    public void The_breakdown_shows_the_debt_and_still_adds_up()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Me.TechDebt = 750;
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.Equal(30, b.FromDebt);
        Assert.Equal(b.FromConfig + b.FromAlignment + b.FromCorruption + b.FromRush + b.FromOverload + b.FromCrunch + b.FromDebt + b.FromOptimisation + b.FromRedTeam + b.FromChief, b.Total);
        int weekly = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(weekly, (int)MathI.Min(MathI.Permille, b.Total));
    }

    [Fact]
    public void Paying_down_costs_cash_retires_the_points_and_logs_it()
    {
        Game g = Lab();
        g.Me.TechDebt = 400;
        long cash = g.Me.Cash;
        g.Do(Command.PayDownDebt(g.Id, 150));
        Assert.Equal(250, g.Me.TechDebt);
        Assert.Equal(cash - 150L * g.C.TechDebt.CostPerPoint, g.Me.Cash);
        GameEvent e = Assert.Single(Paid(g));
        Assert.Equal(150, e.Amount);
        Assert.Equal(250, e.SubjectId);
        Assert.Equal(0, e.Pick);
        Assert.Contains("250 to go", EventText.Describe(g.C, g.State, e));
        g.Do(Command.PayDownDebt(g.Id, 250));
        Assert.Equal(0, g.Me.TechDebt);
        Assert.Equal(1, Paid(g).Last().Pick);
        Assert.Contains("none left", EventText.Describe(g.C, g.State, Paid(g).Last()));
    }

    [Fact]
    public void Paying_down_is_refused_for_bad_amounts_few_engineers_and_empty_pockets_and_changes_nothing()
    {
        Game g = Lab();
        string clean = g.Sim.Digest();
        g.Refused(Command.PayDownDebt(g.Id, 10), CommandError.InvalidArgument);
        Assert.Equal(clean, g.Sim.Digest());
        g.Me.TechDebt = 600;
        string before = g.Sim.Digest();
        g.Refused(Command.PayDownDebt(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.PayDownDebt(g.Id, -3), CommandError.InvalidArgument);
        g.Refused(Command.PayDownDebt(g.Id, g.C.TechDebt.MaxPerCommand + 1), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        g.Me.Cash = 100;
        g.Refused(Command.PayDownDebt(g.Id, 10), CommandError.NotEnoughCash);
        g.Me.Cash = 1_000_000_000_000_000;
        g.Do(Command.Fire(g.Id, "engineer", 2));
        g.Refused(Command.PayDownDebt(g.Id, 10), CommandError.NotEnoughStaff);
        Assert.Equal(600, g.Me.TechDebt);
    }

    [Fact]
    public void Cannot_pay_down_more_than_is_owed()
    {
        Game g = Lab();
        g.Me.TechDebt = 80;
        string before = g.Sim.Digest();
        g.Refused(Command.PayDownDebt(g.Id, 81), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void A_rival_never_keeps_debt_and_a_game_with_it_switched_off_keeps_none()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        TechDebt.Add(w.C, w.State, rival, 500);
        Assert.Equal(0, rival.TechDebt);
        Assert.False(w.Sim.Apply(Command.PayDownDebt(rival.Id, 5)).Ok);
        Game off = Lab(c => c.TechDebt.Enabled = 0);
        ModelState m = Trained(off);
        off.Do(Command.RushRelease(off.Id, m.Id, 4, Sector));
        off.Do(Command.SetCrunch(off.Id, true));
        off.Weeks(3);
        Assert.Equal(0, off.Me.TechDebt);
        off.Me.TechDebt = 100;
        Assert.Equal(0, TechDebt.IncidentPermille(off.C, off.Me));
        off.Refused(Command.PayDownDebt(off.Id, 5), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_real_incident_roll_is_likelier_with_debt()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 0);
        ModelState m = Trained(g);
        Assert.Equal(0, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
        g.Me.TechDebt = 1000;
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) >= 40);
    }

    [Fact]
    public void Save_and_load_keep_the_debt_and_play_on_the_same()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.RushRelease(g.Id, m.Id, 3, Sector));
        Assert.True(g.Me.TechDebt > 0);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.TechDebt, back.State.Lab(g.Id).TechDebt);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_debt()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(3);
        Assert.True(g.Me.TechDebt > 0);
        string old = SaveSchemaPretend.At(g.C, 60, g.Sim.Save());
        Assert.DoesNotContain("techDebt", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.TechDebt));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_impossible_debt()
    {
        Game g = Lab();
        g.Me.TechDebt = 1001;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("technical debt"));
        g.Me.TechDebt = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("technical debt"));
        g.Me.TechDebt = 5;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("technical debt"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).TechDebt = 5;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("technical debt"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 3, engineers: 3, gpus: 6);
        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        g.Do(Command.RushRelease(g.Id, m.Id, 4, "game-ai"));
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(8);
        g.Do(Command.SetCrunch(g.Id, false));
        Assert.True(g.Me.TechDebt >= 100);
        g.Do(Command.PayDownDebt(g.Id, 100));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_crunch_and_paydowns_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11).WithStarterLab(researchers: 3, engineers: 4, safety: 2, gpus: 8);
        g.Me.Cash += 1_000_000_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int paid = 0;
        for (int i = 0; i < 400; i++)
        {
            if (i % 50 == 0) g.Try(Command.SetCrunch(g.Id, true));
            if (i % 50 == 12) g.Try(Command.SetCrunch(g.Id, false));
            g.Autopilot(1);
            if (i % 40 == 30 && g.Me.TechDebt > 0 && g.Try(Command.PayDownDebt(g.Id, (int)MathI.Min(50, g.Me.TechDebt))).Ok) paid++;
        }
        Assert.True(paid > 0);
        Assert.InRange(g.Me.TechDebt, 0, 1000);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
