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

/// <summary>The pay policy: better pay costs more every week, lifts morale and turns offers away; worse pay does the opposite, and a cut shocks everyone.</summary>
public class PayPolicyTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: 10);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 500; }
        return g;
    }

    static long Base(Game g)
    {
        long total = 0;
        foreach (StaffMember p in g.Me.Staff) total += g.C.Role(p.RoleId).WeeklySalary;
        return total;
    }

    static List<GameEvent> Changed(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.PayChanged).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void Everyone_pays_market_rates_until_the_player_changes_it()
    {
        Game g = Lab();
        g.Weeks(30);
        Assert.Equal(0, g.Me.PayLevel);
        Assert.Equal(Base(g), LabQueries.WeeklySalaries(g.C, g.Me));
        Assert.Equal(0, Pay.SalaryPermille(g.C, g.Me));
        Assert.Equal(0, Pay.MoraleOffset(g.C, g.Me));
        Assert.Empty(Changed(g));
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.PayLevel));
    }

    // ---- the command

    [Fact]
    public void A_raise_sets_the_level_logs_it_and_locks_changes_for_the_data_s_weeks()
    {
        Game g = Lab();
        g.Do(Command.SetPayLevel(g.Id, 1));
        Assert.Equal(1, g.Me.PayLevel);
        Assert.Equal(g.Tick + 26, g.Me.PayLockedUntil);
        GameEvent e = Changed(g).Single();
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(MathI.MulPermille(Base(g), 1150), e.Amount);
        Assert.Contains("15% above market", EventText.Describe(g.C, g.State, e));
        g.Refused(Command.SetPayLevel(g.Id, 2), CommandError.PayLocked);
        g.Refused(Command.SetPayLevel(g.Id, 0), CommandError.PayLocked);
        g.Weeks(26);
        g.Do(Command.SetPayLevel(g.Id, 2));
        Assert.Equal(2, g.Me.PayLevel);
    }

    [Fact]
    public void A_level_outside_the_limits_or_the_same_level_is_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetPayLevel(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetPayLevel(g.Id, -2), CommandError.InvalidArgument);
        g.Refused(Command.SetPayLevel(g.Id, 0), CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.PayLockedUntil);
        Assert.Empty(Changed(g));
    }

    [Fact]
    public void A_rival_cannot_set_its_pay_here()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Assert.False(g.Try(Command.SetPayLevel(rival.Id, 1)).Ok);
        Assert.Equal(0, rival.PayLevel);
    }

    [Fact]
    public void With_both_limits_at_zero_there_is_no_pay_policy()
    {
        Game g = Lab(c => { c.StaffPeople.PayMinLevel = 0; c.StaffPeople.PayMaxLevel = 0; });
        Assert.False(Pay.Enabled(g.C));
        g.Refused(Command.SetPayLevel(g.Id, 1), CommandError.InvalidArgument);
    }

    // ---- salaries

    [Theory]
    [InlineData(-1, 850)]
    [InlineData(1, 1150)]
    [InlineData(2, 1300)]
    public void The_weekly_salary_bill_follows_the_level(int level, int permille)
    {
        Game g = Lab();
        g.Do(Command.SetPayLevel(g.Id, level));
        Assert.Equal(MathI.MulPermille(Base(g), permille), LabQueries.WeeklySalaries(g.C, g.Me));
        g.Weeks(1);
        Assert.Equal(MathI.MulPermille(Base(g), permille), -g.Me.Ledger.LastWeek.Salaries);
    }

    [Fact]
    public void A_lab_with_nobody_on_the_payroll_pays_nothing_at_any_level_and_nobody_is_shocked()
    {
        Game g = Lab();
        g.Me.Staff.Clear();
        g.Do(Command.SetPayLevel(g.Id, 2));
        Assert.Equal(0, LabQueries.WeeklySalaries(g.C, g.Me));
        g.Weeks(27);
        g.Do(Command.SetPayLevel(g.Id, -1));
        Assert.Equal(0, LabQueries.WeeklySalaries(g.C, g.Me));
    }

    [Fact]
    public void The_salary_bill_feeds_the_offsite_and_the_ledger_stays_explained_across_changes()
    {
        Game g = Lab(rich: false);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.SetPayLevel(g.Id, 2));
        Assert.Equal(MathI.MulPermille(Base(g), 1300), Offsite.Cost(g.C, g.Me));
        g.Weeks(30);
        g.Do(Command.SetPayLevel(g.Id, -1));
        g.Weeks(30);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }

    // ---- morale

    [Fact]
    public void Each_level_moves_the_level_morale_settles_at_by_forty_points_and_people_drift_there()
    {
        Game g = Lab(c => c.StaffPeople.MoraleDriftPerWeek = 50);
        int resting = Workplace.RestingMorale(g.C, g.Me);
        g.Do(Command.SetPayLevel(g.Id, 2));
        Assert.Equal(resting + 80, Workplace.RestingMorale(g.C, g.Me));
        g.Weeks(5);
        Assert.All(g.Me.Staff, p => Assert.Equal(Traits.RestingMorale(g.C, g.Me, p), p.Morale));
        Assert.All(g.Me.Staff, p => Assert.True(p.Morale > resting));
    }

    [Fact]
    public void A_cut_shocks_everyone_by_a_hundred_for_each_level_and_the_floor_is_zero()
    {
        Game g = Lab();
        g.Do(Command.SetPayLevel(g.Id, 2));
        g.Weeks(26);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        g.Me.Staff[0].Morale = 150;
        g.Do(Command.SetPayLevel(g.Id, -1));
        Assert.Equal(0, g.Me.Staff[0].Morale);
        Assert.All(g.Me.Staff.Skip(1), p => Assert.Equal(200, p.Morale));
    }

    [Fact]
    public void A_raise_gives_no_jolt_and_only_the_resting_level_moves()
    {
        Game g = Lab();
        g.Do(Command.SetPayLevel(g.Id, 1));
        Assert.All(g.Me.Staff, p => Assert.Equal(500, p.Morale));
    }

    [Fact]
    public void A_person_who_quits_when_unhappy_leaves_after_a_cut_but_not_without_one()
    {
        int quits = 0;
        foreach (bool cut in new[] { false, true })
        {
            Game g = Lab();
            g.Me.Staff[0].TraitId = "quits-by-blog-post";
            g.Me.Staff[0].Morale = 350;
            int id = g.Me.Staff[0].Id;
            if (cut) g.Do(Command.SetPayLevel(g.Id, -1));
            g.Weeks(2);
            bool gone = g.Me.Staff.All(p => p.Id != id);
            if (cut) { Assert.True(gone); quits++; }
            else Assert.False(gone);
        }
        Assert.Equal(1, quits);
    }

    [Fact]
    public void A_cut_can_make_the_team_too_tired_for_crunch_and_a_raise_brings_it_back_in_time()
    {
        Game g = Lab(c => c.StaffPeople.MoraleDriftPerWeek = 100);
        g.Do(Command.SetPayLevel(g.Id, -1));
        Assert.Equal(400, Crunch.AverageMorale(g.Me));
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.TeamTooTired);
        g.Weeks(26);
        g.Do(Command.SetPayLevel(g.Id, 2));
        g.Weeks(3);
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.True(Crunch.Active(g.Me));
    }

    [Fact]
    public void The_offsite_ceiling_moves_with_the_pay_level()
    {
        Game g = Lab();
        StaffMember p = g.Me.Staff[0];
        int before = Offsite.Ceiling(g.C, g.Me, p);
        g.Do(Command.SetPayLevel(g.Id, 2));
        Assert.Equal(before + 80, Offsite.Ceiling(g.C, g.Me, p));
    }

    // ---- poaching

    [Fact]
    public void Each_level_turns_away_fifteen_percent_more_of_a_rival_s_offers_within_the_limits()
    {
        Game g = Lab();
        int basePoach = g.C.StaffPeople.PoachChancePermille;
        Assert.Equal(basePoach, Workplace.PoachChancePermille(g.C, g.Me));
        g.Me.PayLevel = 2;
        Assert.Equal(MathI.MulPermille(basePoach, 700), Workplace.PoachChancePermille(g.C, g.Me));
        g.Me.PayLevel = -1;
        Assert.Equal(MathI.MulPermille(basePoach, 1150), Workplace.PoachChancePermille(g.C, g.Me));
    }

    [Fact]
    public void Pay_and_room_defences_together_never_rule_an_offer_out_or_raise_it_past_one_and_a_half_times()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.PayLevel = 2;
        c.StaffPeople.PayPoachStepPermille = 300;
        c.StaffPeople.PayMaxLevel = 5;
        lab.PayLevel = 5;
        Assert.Equal(900, Workplace.PoachDefencePermille(c, lab));
        c.StaffPeople.PayMinLevel = -5;
        lab.PayLevel = -5;
        Assert.Equal(-500, Workplace.PoachDefencePermille(c, lab));
    }

    // ---- save, replay, migration, data

    [Fact]
    public void A_saved_game_keeps_the_level_and_the_lock_and_a_load_plays_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetPayLevel(g.Id, -1));
        g.Weeks(7);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(-1, back.Player.PayLevel);
        Assert.Equal(g.Me.PayLockedUntil, back.Player.PayLockedUntil);
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2);
        g.Autopilot(40);
        Assert.True(g.Try(Command.SetPayLevel(g.Id, 1)).Ok);
        g.Autopilot(60);
        Assert.True(g.Try(Command.SetPayLevel(g.Id, -1)).Ok);
        g.Autopilot(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_schema_46_save_loads_at_market_pay_unlocked()
    {
        Game g = Lab();
        g.Weeks(4);
        string old = SaveSchemaPretend.At(g.C, 46, g.Sim.Save());
        Assert.DoesNotContain("payLevel", old);
        Assert.DoesNotContain("payLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.PayLevel);
        Assert.Equal(0, sim.Player.PayLockedUntil);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_level_is_reported()
    {
        Game g = Lab();
        g.Me.PayLevel = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("pays at level"));
        g.Me.PayLevel = -2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("pays at level"));
        g.Me.PayLevel = -1;
        g.Me.PayLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("pays at level"));
        g.Me.PayLockedUntil = 0;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("pays at level"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(-1, c.StaffPeople.PayMinLevel);
        Assert.Equal(2, c.StaffPeople.PayMaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.StaffPeople.PayMinLevel = -6;
        c.StaffPeople.PayMaxLevel = 6;
        c.StaffPeople.PaySalaryStepPermille = -1;
        c.StaffPeople.PayMoraleStep = 301;
        c.StaffPeople.PayPoachStepPermille = 301;
        c.StaffPeople.PayLockWeeks = 521;
        c.StaffPeople.PayCutShock = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "payMinLevel", "payMaxLevel", "paySalaryStepPermille", "payMoraleStep", "payPoachStepPermille", "payLockWeeks", "payCutShock" })
            Assert.Contains(problems, p => p.Contains("staffPeople." + key));
        GameConfig low = TestData.Config();
        low.StaffPeople.PayMinLevel = -5;
        low.StaffPeople.PaySalaryStepPermille = 200;
        Assert.Contains(ConfigValidator.Problems(low), p => p.Contains("paySalaryStepPermille"));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_change_their_pay()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.PayLevel));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.PayChanged);
    }

    [Fact]
    public void An_autopilot_game_played_with_better_and_worse_pay_stays_sound()
    {
        foreach (int level in new[] { -1, 1 })
        {
            Game g = Game.New(2);
            g.Autopilot(48);
            long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
            g.Do(Command.SetPayLevel(g.Id, level));
            for (int i = 0; i < 150; i++) g.Autopilot(1);
            Assert.Equal(level, g.Me.PayLevel);
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
            Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        }
    }
}
