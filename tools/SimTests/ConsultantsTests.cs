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

/// <summary>Policy consultants: stand-in policy staff for a few weeks, never counted by the compliance office.</summary>
public class ConsultantsTests
{
    static Game Lab(int policy = 0, Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            if (tweak != null) tweak(c);
        });
        if (rich) g.Me.Cash = 1_000_000_000_000;
        if (policy > 0)
        {
            g.Build("policy-office", 4, 3);
            g.Hire("policy", policy);
        }
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 800; }
        return g;
    }

    static List<GameEvent> Hired(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.ConsultantsHired).ToList(); }

    static AttentionGain Gain(Game g)
    {
        g.Me.IncidentsThisMonth = 6;
        return WorldQueries.GainFor(g.C, g.State, g.Me, g.Me.HomeBlocId, 0);
    }

    // ---- nothing happens by itself

    [Fact]
    public void No_firm_comes_in_until_the_player_hires_one()
    {
        Game g = Lab(policy: 2);
        g.Weeks(30);
        Assert.Equal(0, g.Me.ConsultantLevel);
        Assert.Equal(0, g.Me.ConsultantWeeks);
        Assert.False(PolicyConsultants.Running(g.Me));
        Assert.Equal(0, PolicyConsultants.Staff(g.C, g.Me));
        Assert.Equal(0, PolicyConsultants.WeeklyCost(g.C, g.Me));
        Assert.Equal(WorldQueries.PolicyStaff(g.C, g.Me), WorldQueries.PolicyStaffWithConsultants(g.C, g.Me));
        Assert.Empty(Hired(g));
    }

    // ---- the command

    [Fact]
    public void Hiring_a_firm_costs_the_fee_sets_the_state_locks_the_next_one_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.HireConsultants(g.Id, 2));
        Assert.Equal(cash - 60000, g.Me.Cash);
        Assert.Equal(2, g.Me.ConsultantLevel);
        Assert.Equal(12, g.Me.ConsultantWeeks);
        Assert.Equal(g.Tick + 26, g.Me.ConsultantLockedUntil);
        GameEvent e = Assert.Single(Hired(g));
        Assert.Equal(1, e.Pick);
        Assert.Equal(60000, e.Amount);
        Assert.Contains("hired a level 2 firm of policy consultants for $60000", EventText.Describe(g.C, g.State, e));
        Assert.Equal(6, PolicyConsultants.Staff(g.C, g.Me));
    }

    [Fact]
    public void Bad_levels_a_rival_a_bankrupt_lab_a_poor_lab_and_a_game_without_consultants_refuse_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Refused(Command.HireConsultants(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.HireConsultants(g.Id, 3), CommandError.InvalidArgument);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, g.Me.ConsultantLevel);

        Game game = Game.New(1);
        LabState rival = game.State.Labs.First(l => l.Id != game.State.PlayerLabId);
        game.Refused(Command.HireConsultants(rival.Id, 1), CommandError.InvalidArgument);

        Game off = Lab(tweak: c => c.Consultants.MaxLevel = 0);
        off.Refused(Command.HireConsultants(off.Id, 1), CommandError.InvalidArgument);
        Assert.False(PolicyConsultants.Enabled(off.C));

        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.HireConsultants(broke.Id, 1), CommandError.LabBankrupt);

        Game poor = Lab(rich: false);
        poor.Me.Cash = 29_999;
        poor.Refused(Command.HireConsultants(poor.Id, 1), CommandError.NotEnoughCash);
        Assert.Equal(0, poor.Me.ConsultantLevel);
    }

    [Fact]
    public void Only_one_firm_at_a_time_and_a_cooldown_after_it_starts()
    {
        Game g = Lab();
        g.Do(Command.HireConsultants(g.Id, 1));
        g.Refused(Command.HireConsultants(g.Id, 2), CommandError.InvalidArgument);
        g.Weeks(12);
        Assert.False(PolicyConsultants.Running(g.Me));
        g.Refused(Command.HireConsultants(g.Id, 1), CommandError.ConsultantsLocked);
        g.Weeks(14);
        g.Do(Command.HireConsultants(g.Id, 1));
        Assert.Equal(2, Hired(g).Count(e => e.Pick == 1));
    }

    // ---- what they do

    [Fact]
    public void The_attention_gain_falls_while_a_firm_is_in_and_the_policy_saving_keeps_its_cap()
    {
        Game g = Lab(policy: 1);
        long before = Gain(g).PolicySaving;
        g.Do(Command.HireConsultants(g.Id, 1));
        long during = Gain(g).PolicySaving;
        Assert.True(during < before);
        Game h = Lab(policy: 1, tweak: c => c.Consultants.StaffPerLevel = 50);
        h.Do(Command.HireConsultants(h.Id, 2));
        long cap = Gain(h).PolicySaving;
        AttentionGain gain = Gain(h);
        long raw = gain.FromFootprint + gain.FromIncidents + gain.FromDominance;
        Assert.Equal(MathI.MulPermille(raw, 400) - raw, cap);
    }

    [Fact]
    public void Consultants_make_a_lab_ready_for_a_hearing()
    {
        Game g = Lab(tweak: c => c.World.Attention.PreparedPolicyStaff = 2);
        Assert.False(GovernmentSystem.HearingPrepared(g.C, g.Me));
        g.Do(Command.HireConsultants(g.Id, 1));
        Assert.True(GovernmentSystem.HearingPrepared(g.C, g.Me));
        g.Weeks(12);
        Assert.False(GovernmentSystem.HearingPrepared(g.C, g.Me));
    }

    [Fact]
    public void Lobbying_is_safer_and_likelier_to_win_while_a_firm_is_in()
    {
        Game g = Lab();
        int backfire = Lobbying.BackfireChancePermille(g.C, g.Me);
        int subsidy = Lobbying.SubsidyChancePermille(g.C, g.Me);
        g.Do(Command.HireConsultants(g.Id, 2));
        Assert.True(Lobbying.BackfireChancePermille(g.C, g.Me) < backfire);
        Assert.True(Lobbying.SubsidyChancePermille(g.C, g.Me) > subsidy);
        Assert.True(Lobbying.BackfireChancePermille(g.C, g.Me) >= g.C.Lobbying.BackfireFloorPermille);
        Assert.True(Lobbying.SubsidyChancePermille(g.C, g.Me) <= g.C.Lobbying.SubsidyChanceCapPermille);
    }

    [Fact]
    public void The_compliance_office_never_counts_the_consultants()
    {
        Game g = Lab(policy: 1);
        int covers = Compliance.StaffCovers(g.C, g.Me);
        g.Do(Command.HireConsultants(g.Id, 2));
        Assert.Equal(covers, Compliance.StaffCovers(g.C, g.Me));
        g.Refused(Command.SetCompliance(g.Id, 3), CommandError.NotEnoughStaff);
        Assert.Equal(0, g.Me.ComplianceLevel);
    }

    [Fact]
    public void Consultants_and_the_top_compliance_level_do_not_stack_into_a_free_audit()
    {
        Game plain = Lab(policy: 3, tweak: c => c.Compliance.StaffPerLevel = 1);
        plain.Do(Command.SetCompliance(plain.Id, 3));
        Game both = Lab(policy: 3, tweak: c => c.Compliance.StaffPerLevel = 1);
        both.Do(Command.SetCompliance(both.Id, 3));
        both.Do(Command.HireConsultants(both.Id, 2));
        Assert.Equal(Compliance.AuditReliefPermille(plain.C, plain.Me), Compliance.AuditReliefPermille(both.C, both.Me));
        Assert.True(Compliance.AuditReliefPermille(both.C, both.Me) < 1000);
        Assert.Equal(Compliance.HearingPoints(plain.C, plain.Me), Compliance.HearingPoints(both.C, both.Me));
        Assert.Equal(Compliance.AttentionReliefPermille(plain.C, plain.Me), Compliance.AttentionReliefPermille(both.C, both.Me));
        Assert.True(WorldQueries.PolicyStaffWithConsultants(both.C, both.Me) > WorldQueries.PolicyStaffWithConsultants(plain.C, plain.Me));
    }

    // ---- the weekly bill and the end

    [Fact]
    public void The_firm_is_paid_every_week_then_leaves_with_an_event_and_the_lock_stays()
    {
        Game g = Lab(tweak: c => c.Compliance.StaffPerLevel = 0);
        Game h = Lab(tweak: c => c.Compliance.StaffPerLevel = 0);
        g.Do(Command.HireConsultants(g.Id, 2));
        Assert.Equal(4000, PolicyConsultants.WeeklyCost(g.C, g.Me));
        g.Weeks(11);
        h.Weeks(11);
        Assert.Equal(-44000, g.Me.Ledger.Lifetime.Salaries - h.Me.Ledger.Lifetime.Salaries);
        Assert.Equal(1, g.Me.ConsultantWeeks);
        g.Weeks(1);
        h.Weeks(1);
        Assert.Equal(-48000, g.Me.Ledger.Lifetime.Salaries - h.Me.Ledger.Lifetime.Salaries);
        Assert.Equal(0, g.Me.ConsultantLevel);
        Assert.Equal(0, g.Me.ConsultantWeeks);
        GameEvent end = Hired(g).Last();
        Assert.Equal(0, end.Pick);
        Assert.Contains("gone home", EventText.Describe(g.C, g.State, end));
        Assert.True(g.Me.ConsultantLockedUntil > g.Tick);
        g.Weeks(5);
        h.Weeks(5);
        Assert.Equal(-48000, g.Me.Ledger.Lifetime.Salaries - h.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void Zero_values_in_the_data_make_a_firm_a_cost_with_no_effect_or_no_cost_at_all()
    {
        Game g = Lab(policy: 1, tweak: c => c.Consultants.StaffPerLevel = 0);
        long before = Gain(g).PolicySaving;
        g.Do(Command.HireConsultants(g.Id, 2));
        Assert.Equal(before, Gain(g).PolicySaving);
        Game free = Lab(tweak: c => { c.Consultants.FeePerLevel = 0; c.Consultants.WeeklyPerLevel = 0; });
        Game twin = Lab(tweak: c => { c.Consultants.FeePerLevel = 0; c.Consultants.WeeklyPerLevel = 0; });
        free.Do(Command.HireConsultants(free.Id, 2));
        free.Weeks(12);
        twin.Weeks(12);
        Assert.Equal(twin.Me.Cash, free.Me.Cash);
        Assert.Equal(0, free.Me.ConsultantLevel);
        Assert.Empty(SaveGame.Problems(free.C, free.State));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_firm_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.HireConsultants(g.Id, 2));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(7, back.Player.ConsultantWeeks);
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2, c => c.Consultants.FeePerLevel = 100);
        g.Do(Command.HireConsultants(g.Id, 1));
        g.Weeks(30);
        g.Do(Command.HireConsultants(g.Id, 2));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(2, again.Player.ConsultantLevel);
    }

    [Fact]
    public void An_older_save_loads_with_no_firm()
    {
        Game g = Lab();
        g.Do(Command.HireConsultants(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 56, g.Sim.Save());
        Assert.DoesNotContain("consultantLevel", old);
        Assert.DoesNotContain("consultantWeeks", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.ConsultantLevel); Assert.Equal(0, lab.ConsultantWeeks); Assert.Equal(0, lab.ConsultantLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_firm_that_does_not_fit_the_data_is_reported()
    {
        Game g = Lab();
        g.Do(Command.HireConsultants(g.Id, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
        g.Me.ConsultantLevel = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
        g.Me.ConsultantLevel = 1;
        g.Me.ConsultantWeeks = 13;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
        g.Me.ConsultantWeeks = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
        g.Me.ConsultantLevel = 0;
        g.Me.ConsultantWeeks = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
        g.Me.ConsultantWeeks = 0;
        g.Me.ConsultantLockedUntil = -2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("policy consultants"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.Consultants.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Consultants.MaxLevel = 11;
        c.Consultants.FeePerLevel = -1;
        c.Consultants.WeeklyPerLevel = -1;
        c.Consultants.StaffPerLevel = 51;
        c.Consultants.Weeks = 53;
        c.Consultants.CooldownWeeks = 5;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "feePerLevel", "weeklyPerLevel", "staffPerLevel", "weeks", "cooldownWeeks" })
            Assert.Contains(problems, p => p.Contains("consultants." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_hire_consultants()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => { Assert.Equal(0, lab.ConsultantLevel); Assert.Equal(0, lab.ConsultantWeeks); });
        Assert.Empty(Hired(g));
    }

    [Fact]
    public void A_long_game_with_firms_hired_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(60);
        int hired = 0;
        for (int i = 0; i < 260; i++)
        {
            g.Autopilot(1);
            if (i % 40 == 5 && g.Try(Command.HireConsultants(g.Id, 1 + i / 40 % 2)).Ok) hired++;
        }
        Assert.True(hired >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
