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

/// <summary>Labour relations: pressure from layoffs, crunch and low morale tips into a drive the player recognises or resists.</summary>
public class LabourTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool restless = true)
    {
        return Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (restless)
            {
                c.Labour.MoraleBelow = 1000;
                c.Labour.LowMoralePerWeek = 100;
            }
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 2, gpus: 8);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static void UntilStage(Game g, int stage)
    {
        for (int i = 0; i < 60 && g.Me.LabourStage != stage; i++) g.Weeks(1);
        Assert.Equal(stage, g.Me.LabourStage);
    }

    static Game Organising(Action<GameConfig> tweak = null)
    {
        Game g = Lab(tweak);
        UntilStage(g, Labour.Organising);
        return g;
    }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Labour.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Labour.Enabled = 2;
        c.Labour.MinStaff = 1;
        c.Labour.MoraleBelow = 1001;
        c.Labour.LowMoralePerWeek = 501;
        c.Labour.LayoffPerFull = 2001;
        c.Labour.DecayPerWeek = 501;
        c.Labour.DrivePressure = 1000;
        c.Labour.WindowWeeks = 0;
        c.Labour.ResistWeeks = 0;
        c.Labour.ResistFactorPermille = 999;
        c.Labour.ResistCost = -1;
        c.Labour.HeldPressure = 1000;
        c.Labour.StrikeWeeks = 0;
        c.Labour.StrikeMorale = 1001;
        c.Labour.StrikeCutPermille = 1000;
        c.Labour.RecogniseMorale = 501;
        c.Labour.PayPermille = 501;
        c.Labour.SeverancePermille = 999;
        c.Labour.ResearchPermille = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minStaff", "moraleBelow", "lowMoralePerWeek", "layoffPerFull", "decayPerWeek", "drivePressure", "windowWeeks", "resistWeeks", "resistFactorPermille", "resistCost", "heldPressure", "strikeWeeks", "strikeMorale", "strikeCutPermille", "recogniseMorale", "payPermille", "severancePermille", "researchPermille" })
            Assert.True(problems.Any(p => p.Contains("labour." + key)), key + " in " + string.Join(" | ", problems));
        c.Labour.HardPayPermille = 0;
        c.Labour.HardSeverancePermille = 0;
        c.Labour.HardResearchPermille = 2000;
        c = TestData.Config();
        c.Labour.PayPermille = 100;
        c.Labour.HardPayPermille = 50;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("hardPayPermille"));
    }

    [Fact]
    public void A_content_lab_is_never_organised()
    {
        Game g = Lab(null, false);
        g.Weeks(80);
        Assert.Equal(0, g.Me.LabourStage);
        Assert.Equal(0, g.Me.LabourPressure);
        Assert.Empty(Of(g, EventKind.LabourDrive));
    }

    [Fact]
    public void Low_morale_builds_pressure_until_the_staff_organise()
    {
        Game g = Lab();
        g.Weeks(3);
        Assert.Equal(270, g.Me.LabourPressure);
        UntilStage(g, Labour.Organising);
        Assert.True(g.Me.LabourPressure >= 700);
        Assert.Equal(g.State.Tick + 8, g.Me.LabourDue);
        GameEvent e = Of(g, EventKind.LabourDrive).Single();
        Assert.Equal(8, e.SubjectId);
        Assert.Contains("organising", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_small_lab_builds_no_pressure()
    {
        Game g = Lab(c => c.Labour.MinStaff = 100);
        g.Weeks(40);
        Assert.Equal(0, g.Me.LabourPressure);
        Assert.Equal(0, g.Me.LabourStage);
    }

    [Fact]
    public void Crunch_time_and_layoffs_add_pressure()
    {
        Game quiet = Lab(c => { c.Labour.MoraleBelow = 0; c.Labour.CrunchPerWeek = 100; });
        Game crunch = Lab(c => { c.Labour.MoraleBelow = 0; c.Labour.CrunchPerWeek = 100; });
        crunch.Do(Command.SetCrunch(crunch.Id, true));
        quiet.Weeks(3);
        crunch.Weeks(3);
        Assert.Equal(0, quiet.Me.LabourPressure);
        Assert.True(crunch.Me.LabourPressure > 0);

        Game g = Lab(c => { c.Labour.MoraleBelow = 0; });
        int share = Layoffs.StaffSharePermille(g.C, g.Me, 300);
        g.Do(Command.Layoff(g.Id, 300));
        Assert.Equal(600L * share / 1000, g.Me.LabourPressure);
        Assert.True(share > 0);
    }

    [Fact]
    public void Recognising_the_union_settles_a_contract_with_pay_severance_crunch_and_research()
    {
        Game g = Organising();
        long pay = LabQueries.WeeklySalaries(g.C, g.Me);
        long package = Layoffs.Package(g.C, g.Me, 200);
        long moraleBefore = g.Me.Staff.Sum(p => p.Morale);
        g.Do(Command.RecogniseUnion(g.Id));
        Assert.Equal(Labour.Union, g.Me.LabourStage);
        Assert.Equal(0, g.Me.LabourPressure);
        Assert.Equal(0, g.Me.LabourDue);
        Assert.Equal(MathI.MulPermille(pay, 1080), LabQueries.WeeklySalaries(g.C, g.Me));
        Assert.Equal(package * 2, Layoffs.Package(g.C, g.Me, 200));
        Assert.True(g.Me.Staff.Sum(p => p.Morale) > moraleBefore);
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.InvalidArgument);
        long with = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.C.Labour.ResearchPermille = 0;
        Assert.True(with > TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick));
        GameEvent e = Of(g, EventKind.LabourRecognised).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(80, e.Amount);
        g.Weeks(60);
        Assert.Equal(Labour.Union, g.Me.LabourStage);
        Assert.Equal(0, g.Me.LabourPressure);
    }

    [Fact]
    public void A_fired_person_costs_double_severance_under_a_contract()
    {
        Game g = Organising();
        RoleConfig role = g.C.Role("engineer");
        g.Do(Command.RecogniseUnion(g.Id));
        long cash = g.Me.Cash;
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Assert.Equal(cash - 2 * role.Severance, g.Me.Cash);
    }

    [Fact]
    public void Resisting_costs_fees_and_a_drive_with_calmer_staff_collapses()
    {
        Game g = Organising();
        long cash = g.Me.Cash;
        g.Do(Command.ResistUnion(g.Id));
        Assert.Equal(cash - 50000, g.Me.Cash);
        Assert.Equal(Labour.Resisting, g.Me.LabourStage);
        Assert.Equal(g.State.Tick + 26, g.Me.LabourDue);
        Assert.Equal(1, Of(g, EventKind.LabourResisted).Single().Pick);
        g.C.Labour.LowMoralePerWeek = 0;
        g.Weeks(26);
        Assert.Equal(Labour.Quiet, g.Me.LabourStage);
        Assert.Equal(250, g.Me.LabourPressure);
        Assert.Equal(0, g.Me.LabourDue);
        Assert.Single(Of(g, EventKind.LabourHeld));
        Assert.Empty(Of(g, EventKind.LabourStrike));
    }

    [Fact]
    public void A_resisted_drive_with_angry_staff_ends_in_a_strike_and_a_harder_contract()
    {
        Game g = Organising();
        g.Do(Command.ResistUnion(g.Id));
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        UntilStage(g, Labour.Strike);
        GameEvent strike = Of(g, EventKind.LabourStrike).Single();
        Assert.Equal(3, strike.Amount);
        Assert.Equal(0, g.Me.LabourPressure);
        Assert.True(TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick) < before / 2 + 1);
        Assert.Equal(-600, Labour.TrainingPermille(g.C, g.Me));
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.InvalidArgument);
        g.Weeks(3);
        Assert.Equal(Labour.Union, g.Me.LabourStage);
        Assert.Equal(1, g.Me.LabourHard);
        GameEvent forced = Of(g, EventKind.LabourRecognised).Single();
        Assert.Equal(0, forced.Pick);
        Assert.Equal(120, forced.Amount);
        Assert.Equal(3000, Labour.SeverancePermille(g.C, g.Me));
        Assert.Equal(0, Labour.ResearchPermille(g.C, g.Me));
    }

    [Fact]
    public void A_drive_nobody_answers_counts_as_resisted_with_no_fees()
    {
        Game g = Organising();
        long cash = g.Me.Cash;
        g.Weeks(8);
        Assert.Equal(Labour.Resisting, g.Me.LabourStage);
        GameEvent e = Of(g, EventKind.LabourResisted).Single();
        Assert.Equal(0, e.Pick);
        Assert.Equal(0, e.Amount);
        Assert.Empty(Of(g, EventKind.LabourRecognised));
    }

    [Fact]
    public void Refused_answers_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.RecogniseUnion(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.ResistUnion(g.Id), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        UntilStage(g, Labour.Organising);
        g.Me.Cash = 1000;
        d = g.Sim.Digest();
        Assert.False(g.Sim.Apply(Command.ResistUnion(g.Id)).Ok);
        Assert.Equal(d, g.Sim.Digest());
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.RecogniseUnion(g.Id));
        d = g.Sim.Digest();
        g.Refused(Command.RecogniseUnion(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.ResistUnion(g.Id), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        Game off = Lab(c => c.Labour.Enabled = 0);
        off.Weeks(30);
        Assert.Equal(0, off.Me.LabourPressure);
        off.Refused(Command.RecogniseUnion(off.Id), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.RecogniseUnion(rival.Id)).Ok);
        Assert.False(w.Sim.Apply(Command.ResistUnion(rival.Id)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_stage_and_play_on_the_same()
    {
        Game g = Organising();
        g.Do(Command.ResistUnion(g.Id));
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(g.Me.LabourStage, lab.LabourStage);
        Assert.Equal(g.Me.LabourPressure, lab.LabourPressure);
        Assert.Equal(g.Me.LabourDue, lab.LabourDue);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(12);
        for (int i = 0; i < 12; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Contains(back.State.Events, e => e.Kind == EventKind.LabourStrike);
    }

    [Fact]
    public void An_older_save_loads_with_no_organising()
    {
        Game g = Organising();
        string old = SaveSchemaPretend.At(g.C, 71, g.Sim.Save());
        foreach (string key in new[] { "labourStage", "labourPressure", "labourDue", "labourHard" }) Assert.DoesNotContain("\"" + key + "\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.LabourStage + lab.LabourPressure + lab.LabourDue + lab.LabourHard));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_impossible_labour_state()
    {
        Game g = Organising();
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        int due = g.Me.LabourDue;
        g.Me.LabourStage = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        g.Me.LabourStage = 1;
        g.Me.LabourDue = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        g.Me.LabourDue = due;
        g.Me.LabourPressure = 1001;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        g.Me.LabourPressure = 800;
        g.Me.LabourHard = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        g.Me.LabourHard = 0;
        g.Me.LabourStage = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("labour"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.LabourPressure = 10;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("labour"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        UntilStage(g, Labour.Organising);
        g.Do(Command.ResistUnion(g.Id));
        g.Weeks(8);
        g.Do(Command.Layoff(g.Id, 300));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_a_union_stays_sound_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        UntilStage(g, Labour.Organising);
        g.Do(Command.RecogniseUnion(g.Id));
        for (int i = 0; i < 200; i++)
        {
            g.Weeks(1);
            if (i % 40 == 20) g.Try(Command.Layoff(g.Id, 100));
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
