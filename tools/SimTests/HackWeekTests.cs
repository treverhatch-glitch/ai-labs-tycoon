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

/// <summary>Hack week: the researchers build what they like, so the team is happier and research is faster, training slows, and it needs a cooldown.</summary>
public class HackWeekTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, int researchers = 4)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: researchers, engineers: 2, gpus: 10);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        return g;
    }

    static List<GameEvent> Hacks(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.HackWeek).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void Nobody_has_a_hack_week_until_the_player_says_so()
    {
        Game g = Lab();
        g.Weeks(40);
        Assert.False(HackWeek.Active(g.Me));
        Assert.Equal(0, g.Me.HackWeeks);
        Assert.Equal(0, HackWeek.ResearchPermille(g.C, g.Me));
        Assert.Equal(0, HackWeek.TrainingCutPermille(g.C, g.Me));
        Assert.Empty(Hacks(g));
    }

    // ---- the command

    [Fact]
    public void It_costs_each_person_the_data_s_cash_lifts_morale_and_starts_the_cooldown()
    {
        Game g = Lab();
        int staff = g.Me.Staff.Count;
        long cash = g.Me.Cash;
        long hiring = g.Me.Ledger.Lifetime.Hiring;
        long cost = HackWeek.Cost(g.C, g.Me);
        Assert.Equal(staff * 800L, cost);
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.Equal(2, g.Me.HackWeeks);
        Assert.Equal(g.Tick + 26, g.Me.HackLockedUntil);
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(hiring - cost, g.Me.Ledger.Lifetime.Hiring);
        Assert.All(g.Me.Staff, p => Assert.Equal(580, p.Morale));
        GameEvent e = Hacks(g).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(2, e.SubjectId);
        Assert.Equal(cost, e.Amount);
        Assert.Contains("hack week", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Morale_stops_at_the_top_of_the_scale()
    {
        Game g = Lab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 980;
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.All(g.Me.Staff, p => Assert.Equal(1000, p.Morale));
    }

    [Fact]
    public void Research_runs_a_data_share_faster_while_it_lasts_and_goes_back_after()
    {
        Game g = Lab();
        long plain = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        g.Do(Command.HoldHackWeek(g.Id));
        long during = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(during > plain);
        g.Weeks(1);
        Assert.True(HackWeek.Active(g.Me));
        g.Weeks(1);
        Assert.False(HackWeek.Active(g.Me));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        Assert.Equal(plain, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void The_extra_research_is_exactly_the_data_s_share_of_what_the_team_makes()
    {
        Game g = Lab(c => c.Hack.MoraleLift = 0);
        long plain = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.Equal(MathI.MulPermille(plain, 1400), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void Two_weeks_of_it_make_more_research_points_than_two_ordinary_weeks()
    {
        Game plain = Lab(c => c.Hack.MoraleLift = 0);
        Game hack = Lab(c => c.Hack.MoraleLift = 0);
        hack.Do(Command.HoldHackWeek(hack.Id));
        long p0 = plain.Me.ResearchPoints, h0 = hack.Me.ResearchPoints;
        plain.Weeks(2);
        hack.Weeks(2);
        Assert.True(hack.Me.ResearchPoints - h0 > plain.Me.ResearchPoints - p0);
    }

    [Fact]
    public void Training_speed_falls_by_the_data_s_share_while_it_lasts_and_a_run_takes_longer()
    {
        Game plain = Lab();
        Game hack = Lab();
        ModelState a = plain.Train("mini", 0);
        ModelState b = hack.Train("mini", 0);
        hack.Do(Command.HoldHackWeek(hack.Id));
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) - 400, TrainingSystem.SpeedPermille(hack.C, hack.Me));
        plain.Weeks(2);
        hack.Weeks(2);
        Assert.True(b.ProgressCompute < a.ProgressCompute, "the run should have moved less in two hack weeks");
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me), TrainingSystem.SpeedPermille(hack.C, hack.Me));
    }

    [Fact]
    public void It_ends_by_itself_after_the_data_s_weeks_and_says_so_once()
    {
        Game g = Lab();
        g.Do(Command.HoldHackWeek(g.Id));
        g.Weeks(1);
        Assert.Single(Hacks(g));
        g.Weeks(1);
        Assert.Equal(0, g.Me.HackWeeks);
        GameEvent end = Hacks(g).Last();
        Assert.Equal(2, Hacks(g).Count);
        Assert.Equal(0, end.Pick);
        Assert.Equal(0, end.Amount);
        Assert.Contains("is over", EventText.Describe(g.C, g.State, end));
        g.Weeks(10);
        Assert.Equal(2, Hacks(g).Count);
    }

    [Fact]
    public void A_one_week_hack_week_runs_for_exactly_one_week()
    {
        Game g = Lab(c => c.Hack.Weeks = 1);
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.Equal(1, g.Me.HackWeeks);
        g.Weeks(1);
        Assert.False(HackWeek.Active(g.Me));
        Assert.Equal(2, Hacks(g).Count);
    }

    // ---- refusals

    [Fact]
    public void A_second_one_is_refused_while_one_is_on_and_until_the_cooldown_is_over()
    {
        Game g = Lab();
        g.Do(Command.HoldHackWeek(g.Id));
        g.Refused(Command.HoldHackWeek(g.Id), CommandError.InvalidArgument);
        g.Weeks(2);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        g.Refused(Command.HoldHackWeek(g.Id), CommandError.HackLocked);
        g.Weeks(23);
        g.Refused(Command.HoldHackWeek(g.Id), CommandError.HackLocked);
        g.Weeks(1);
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.True(HackWeek.Active(g.Me));
    }

    [Fact]
    public void Crunch_and_hack_week_cannot_overlap_either_way()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Refused(Command.HoldHackWeek(g.Id), CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.HackWeeks);
        g.Do(Command.SetCrunch(g.Id, false));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        g.Do(Command.HoldHackWeek(g.Id));
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.InvalidArgument);
        Assert.False(Crunch.Active(g.Me));
        g.Weeks(2);
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.True(Crunch.Active(g.Me));
    }

    [Fact]
    public void Too_few_researchers_a_tired_team_and_too_little_cash_are_refused_and_change_nothing()
    {
        Game few = Lab(researchers: 1);
        few.Refused(Command.HoldHackWeek(few.Id), CommandError.NotEnoughStaff);
        Assert.Equal(0, few.Me.HackLockedUntil);

        Game tired = Lab();
        foreach (StaffMember p in tired.Me.Staff) p.Morale = 200;
        tired.Refused(Command.HoldHackWeek(tired.Id), CommandError.TeamTooTired);
        Assert.All(tired.Me.Staff, p => Assert.Equal(200, p.Morale));

        Game poor = Lab(rich: false);
        poor.Me.Cash = 100;
        poor.Refused(Command.HoldHackWeek(poor.Id), CommandError.NotEnoughCash);
        Assert.Equal(100, poor.Me.Cash);
        Assert.Equal(0, poor.Me.HackWeeks);
        Assert.Empty(Hacks(poor));
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_it_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.HoldHackWeek(rival.Id), CommandError.InvalidArgument);
        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.HoldHackWeek(broke.Id), CommandError.LabBankrupt);
        Game off = Lab(c => c.Hack.Weeks = 0);
        off.Refused(Command.HoldHackWeek(off.Id), CommandError.InvalidArgument);
        Assert.False(HackWeek.Enabled(off.C));
    }

    [Fact]
    public void An_empty_lab_can_hold_one_for_nothing_when_the_data_lets_it()
    {
        Game g = Lab(c => { c.Hack.MinResearchers = 0; c.Hack.MinMorale = 0; });
        g.Me.Staff.Clear();
        g.Do(Command.HoldHackWeek(g.Id));
        Assert.Equal(0, HackWeek.Cost(g.C, g.Me));
        g.Weeks(3);
        Assert.False(HackWeek.Active(g.Me));
    }

    [Fact]
    public void A_lab_that_goes_bankrupt_in_the_middle_keeps_a_valid_save()
    {
        Game g = Lab();
        g.Do(Command.HoldHackWeek(g.Id));
        g.Me.Bankrupt = true;
        g.Weeks(5);
        Assert.Equal(2, g.Me.HackWeeks);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    // ---- together

    [Fact]
    public void The_research_bonus_adds_to_the_share_set_aside_and_takes_the_interpretability_cost_off()
    {
        Game g = Lab(c => c.Hack.MoraleLift = 0);
        g.Me.ResearchSharePermille = 100;
        long plain = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        int share = TrainingSystem.ComputeShareBonusPermille(g.C, g.Me);
        g.Do(Command.HoldHackWeek(g.Id));
        long hack = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        long unshared = plain * 1000 / (1000 + share);
        Assert.InRange(hack, MathI.MulPermille(unshared, 1400 + share) - 2, MathI.MulPermille(unshared, 1400 + share) + 2);
    }

    [Fact]
    public void A_hack_week_every_half_year_for_four_years_leaves_a_sound_game()
    {
        Game g = Lab(c => c.Hack.MoraleLift = 40);
        g.Weeks(4);
        int held = 0;
        for (int i = 0; i < 208; i++)
        {
            if (g.Try(Command.HoldHackWeek(g.Id)).Ok) held++;
            g.Weeks(1);
        }
        Assert.InRange(held, 7, 9);
        Assert.Equal(held, Hacks(g).Count(e => e.Pick == 1));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_hack_week_in_progress_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.HoldHackWeek(g.Id));
        g.Weeks(1);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(1, back.Player.HackWeeks);
        Assert.Equal(g.Me.HackLockedUntil, back.Player.HackLockedUntil);
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(0, back.Player.HackWeeks);
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => { c.Hack.CostPerStaff = 10; c.Hack.MinMorale = 0; });
        g.Autopilot(20);
        Assert.True(g.Try(Command.HoldHackWeek(g.Id)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_hack_week()
    {
        Game g = Lab();
        g.Do(Command.HoldHackWeek(g.Id));
        string old = SaveSchemaPretend.At(g.C, 51, g.Sim.Save());
        Assert.DoesNotContain("hackWeeks", old);
        Assert.DoesNotContain("hackLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.HackWeeks); Assert.Equal(0, lab.HackLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_too_many_weeks_left_or_a_negative_lock_is_reported()
    {
        Game g = Lab();
        g.Me.HackWeeks = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("hack week"));
        g.Me.HackWeeks = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("hack week"));
        g.Me.HackWeeks = 2;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("hack week"));
        g.Me.HackLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("hack week"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.Hack.Weeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Hack.Weeks = 53;
        c.Hack.MinResearchers = -1;
        c.Hack.MinMorale = 1001;
        c.Hack.CostPerStaff = -1;
        c.Hack.MoraleLift = 1001;
        c.Hack.ResearchPermille = 2001;
        c.Hack.TrainingCutPermille = 901;
        c.Hack.CooldownWeeks = 521;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "weeks", "minResearchers", "minMorale", "costPerStaff", "moraleLift", "researchPermille", "trainingCutPermille", "cooldownWeeks" })
            Assert.Contains(problems, p => p.Contains("hack." + key));
        GameConfig d = TestData.Config();
        d.Hack.CooldownWeeks = 1;
        Assert.Contains(ConfigValidator.Problems(d), p => p.Contains("hack.cooldownWeeks"));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_hold_one()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => { Assert.Equal(0, lab.HackWeeks); Assert.Equal(0, lab.HackLockedUntil); });
        Assert.Empty(Hacks(g));
    }

    [Fact]
    public void An_autopilot_game_with_hack_weeks_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c => c.Hack.MinMorale = 0);
        g.Autopilot(40);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int held = 0;
        for (int i = 0; i < 240; i++)
        {
            g.Autopilot(1);
            if (i % 30 == 0 && g.Me.Cash > 300_000 && g.Try(Command.HoldHackWeek(g.Id)).Ok) held++;
        }
        Assert.True(held >= 2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(held, Hacks(g).Count(e => e.Pick == 1));
    }
}
