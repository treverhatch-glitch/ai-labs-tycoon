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

/// <summary>The compliance office: a weekly retainer and policy staff buy less government attention, smaller audit fines and a better hearing.</summary>
public class ComplianceTests
{
    static Game Lab(int policy, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            c.World.Attention.PreparedPolicyStaff = 100;
            if (tweak != null) tweak(c);
        });
        g.Me.Cash = 1_000_000_000_000;
        if (policy > 0)
        {
            g.Build("policy-office", 4, 3);
            g.Hire("policy", policy);
        }
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 800; }
        return g;
    }

    static List<GameEvent> Lapsed(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.ComplianceLapsed).ToList(); }

    static void Audit(Game g)
    {
        string bloc = g.Me.HomeBlocId;
        g.Me.LastWeek.Revenue = 50_000;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        g.Me.Bloc(bloc).Level = BlocStanding.Hearing;
        g.Me.Bloc(bloc).Attention = g.C.World.Attention.AuditAt + 100;
    }

    // ---- nothing happens by itself

    [Fact]
    public void No_office_runs_and_nothing_changes_until_the_player_sets_one()
    {
        Game g = Lab(policy: 3);
        g.Weeks(30);
        Assert.Equal(0, g.Me.ComplianceLevel);
        Assert.Equal(0, Compliance.WeeklyCost(g.C, g.Me));
        Assert.Equal(0, Compliance.AttentionReliefPermille(g.C, g.Me));
        Assert.Equal(0, Compliance.AuditReliefPermille(g.C, g.Me));
        Assert.Equal(0, Compliance.HearingPoints(g.C, g.Me));
        Assert.Empty(Lapsed(g));
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.ComplianceLevel));
        Assert.Equal(0, WorldQueries.GainFor(g.C, g.State, g.Me, g.Me.HomeBlocId, 0).ComplianceSaving);
    }

    // ---- the command

    [Fact]
    public void An_office_needs_policy_staff_for_every_level()
    {
        Game g = Lab(policy: 2);
        g.Refused(Command.SetCompliance(g.Id, 3), CommandError.NotEnoughStaff);
        Assert.Equal(0, g.Me.ComplianceLevel);
        g.Do(Command.SetCompliance(g.Id, 2));
        Assert.Equal(2, g.Me.ComplianceLevel);
        Assert.Equal(2, Compliance.StaffCovers(g.C, g.Me));
    }

    [Fact]
    public void A_lab_without_policy_staff_cannot_open_one()
    {
        Game g = Lab(policy: 0);
        g.Refused(Command.SetCompliance(g.Id, 1), CommandError.NotEnoughStaff);
        Assert.Equal(0, g.Me.ComplianceLevel);
    }

    [Fact]
    public void A_level_outside_the_range_or_the_same_level_is_refused()
    {
        Game g = Lab(policy: 3);
        g.Refused(Command.SetCompliance(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetCompliance(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetCompliance(g.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.SetCompliance(g.Id, 1));
        g.Refused(Command.SetCompliance(g.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_cannot_run_one_here()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.SetCompliance(rival.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(0, rival.ComplianceLevel);
    }

    [Fact]
    public void The_office_can_be_turned_off_and_costs_nothing_after()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 3));
        g.Weeks(1);
        g.Do(Command.SetCompliance(g.Id, 0));
        long before = g.Me.Ledger.Lifetime.Salaries;
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Weeks(4);
        Assert.Equal(before - 4 * salaries, g.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_open_one()
    {
        Game g = Lab(policy: 3);
        g.Me.Bankrupt = true;
        g.Refused(Command.SetCompliance(g.Id, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void An_office_off_in_the_data_refuses_every_level()
    {
        Game g = Lab(policy: 3, c => c.Compliance.MaxLevel = 0);
        g.Refused(Command.SetCompliance(g.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(0, Compliance.StaffCovers(g.C, g.Me));
    }

    // ---- the retainer

    [Fact]
    public void The_retainer_is_the_data_s_cost_for_each_level_on_the_salaries_line()
    {
        Game control = Lab(policy: 3);
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 2));
        control.Weeks(10);
        g.Weeks(10);
        Assert.Equal(12_000, Compliance.WeeklyCost(g.C, g.Me));
        Assert.Equal(control.Me.Ledger.Lifetime.Salaries - 10 * 12_000, g.Me.Ledger.Lifetime.Salaries);
        Assert.Equal(control.Me.Cash - 10 * 12_000, g.Me.Cash);
    }

    [Fact]
    public void A_lab_with_no_cash_pays_the_retainer_into_debt()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 1));
        g.Me.Cash = 0;
        g.Weeks(1);
        Assert.True(g.Me.Cash < 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    // ---- what it does

    [Fact]
    public void Each_level_takes_a_share_off_the_attention_gain_after_the_policy_staff()
    {
        Game g = Lab(policy: 3);
        g.Me.IncidentsThisMonth = 6;
        string bloc = g.Me.HomeBlocId;
        AttentionGain none = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
        Assert.True(none.FromIncidents > 0);
        Assert.Equal(0, none.ComplianceSaving);
        long afterPolicy = none.FromFootprint + none.FromIncidents + none.FromDominance + none.PolicySaving;
        for (int level = 1; level <= 3; level++)
        {
            g.Me.ComplianceLevel = level;
            AttentionGain gain = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
            Assert.Equal(MathI.MulPermille(afterPolicy, 1000 - 80 * level) - afterPolicy, gain.ComplianceSaving);
            Assert.Equal(afterPolicy + gain.ComplianceSaving, gain.Total);
            Assert.Equal(none.PolicySaving, gain.PolicySaving);
        }
    }

    [Fact]
    public void A_month_closes_with_less_attention_when_the_office_runs()
    {
        int Attention(int level)
        {
            Game g = Lab(policy: 3);
            g.Me.ComplianceLevel = level;
            g.Me.IncidentsThisMonth = 6;
            g.Me.Ledger.ThisMonth.Revenue = 0;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            return g.Me.Bloc(g.Me.HomeBlocId).Attention;
        }
        int none = Attention(0);
        Assert.True(none > 0);
        Assert.True(Attention(1) < none);
        Assert.True(Attention(3) < Attention(1));
    }

    [Fact]
    public void The_office_trims_an_audit_fine_by_the_data_s_share()
    {
        long Fine(int level)
        {
            Game g = Lab(policy: 3);
            g.Me.ComplianceLevel = level;
            Audit(g);
            long cash = g.Me.Cash;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            Assert.Equal(BlocStanding.Audit, g.Me.Bloc(g.Me.HomeBlocId).Level);
            return cash - g.Me.Cash;
        }
        long full = Fine(0);
        Assert.Equal(50_000 * TestData.Config().World.Attention.AuditFineRevenueWeeks, full);
        Assert.Equal(MathI.MulPermille(full, 850), Fine(1));
        Assert.Equal(MathI.MulPermille(full, 550), Fine(3));
    }

    [Fact]
    public void A_hearing_goes_better_by_the_data_s_points_for_each_level()
    {
        int Rep(int level)
        {
            Game g = Lab(policy: 3);
            g.Me.ComplianceLevel = level;
            g.Me.Reputation = 500;
            g.Me.Bloc("us").Level = BlocStanding.Letter;
            g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
            g.Me.Ledger.ThisMonth.Revenue = 0;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            Assert.Equal(BlocStanding.Hearing, g.Me.Bloc("us").Level);
            return g.Me.Reputation;
        }
        int none = Rep(0);
        Assert.Equal(none + 5, Rep(1));
        Assert.Equal(none + 15, Rep(3));
    }

    [Fact]
    public void A_hearing_at_zero_or_full_reputation_stays_in_range()
    {
        foreach (int rep in new[] { 0, 1000 })
        {
            Game g = Lab(policy: 3);
            g.Me.ComplianceLevel = 3;
            g.Me.Reputation = rep;
            g.Me.Bloc("us").Level = BlocStanding.Letter;
            g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            Assert.InRange(g.Me.Reputation, 0, 1000);
        }
    }

    [Fact]
    public void The_most_the_data_allows_still_leaves_a_tenth_of_the_gain_and_some_fine()
    {
        Game g = Lab(policy: 3, c => { c.Compliance.AttentionPermillePerLevel = 300; c.Compliance.AuditFinePermillePerLevel = 333; });
        g.Me.ComplianceLevel = 3;
        Assert.Equal(900, Compliance.AttentionReliefPermille(g.C, g.Me));
        Assert.Equal(999, Compliance.AuditReliefPermille(g.C, g.Me));
        g.Me.IncidentsThisMonth = 6;
        Assert.True(WorldQueries.GainFor(g.C, g.State, g.Me, g.Me.HomeBlocId, 0).Total > 0);
    }

    // ---- staff

    [Fact]
    public void Losing_all_the_policy_staff_drops_the_office_and_logs_it()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 3));
        g.Me.Staff.RemoveAll(p => p.RoleId == "policy");
        g.Weeks(1);
        Assert.Equal(0, g.Me.ComplianceLevel);
        GameEvent e = Lapsed(g).Single();
        Assert.Equal(0, e.SubjectId);
        Assert.Equal(3, e.Amount);
        Assert.Contains("fell from level 3 to level 0", EventText.Describe(g.C, g.State, e));
        long salaries = g.Me.Ledger.Lifetime.Salaries;
        g.Weeks(3);
        Assert.Equal(salaries - 3 * LabQueries.WeeklySalaries(g.C, g.Me), g.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void Losing_one_of_three_policy_staff_drops_a_single_level_and_pays_for_the_lower_one()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 3));
        g.Me.Staff.Remove(g.Me.Staff.First(p => p.RoleId == "policy"));
        long before = g.Me.Ledger.Lifetime.Salaries;
        g.Weeks(1);
        Assert.Equal(2, g.Me.ComplianceLevel);
        Assert.Equal(before - LabQueries.WeeklySalaries(g.C, g.Me) - 12_000, g.Me.Ledger.Lifetime.Salaries);
        Assert.Single(Lapsed(g));
        g.Weeks(5);
        Assert.Single(Lapsed(g));
    }

    [Fact]
    public void Staff_needed_per_level_is_the_data_s_number()
    {
        Game g = Lab(policy: 3, c => c.Compliance.StaffPerLevel = 2);
        Assert.Equal(1, Compliance.StaffCovers(g.C, g.Me));
        g.Refused(Command.SetCompliance(g.Id, 2), CommandError.NotEnoughStaff);
        g.Do(Command.SetCompliance(g.Id, 1));
    }

    [Fact]
    public void A_level_that_needs_no_staff_is_always_covered()
    {
        Game g = Lab(policy: 0, c => c.Compliance.StaffPerLevel = 0);
        Assert.Equal(3, Compliance.StaffCovers(g.C, g.Me));
        g.Do(Command.SetCompliance(g.Id, 3));
        g.Weeks(3);
        Assert.Equal(3, g.Me.ComplianceLevel);
        Assert.Empty(Lapsed(g));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_running_office_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 2));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.ComplianceLevel);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Compliance.StaffPerLevel = 0);
        g.Autopilot(60);
        Assert.True(g.Try(Command.SetCompliance(g.Id, 2)).Ok);
        g.Autopilot(40);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_office()
    {
        Game g = Lab(policy: 3);
        g.Do(Command.SetCompliance(g.Id, 2));
        string old = SaveSchemaPretend.At(g.C, 47, g.Sim.Save());
        Assert.DoesNotContain("complianceLevel", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.All(loaded.State.Labs, lab => Assert.Equal(0, lab.ComplianceLevel));
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(SaveGame.Problems(loaded.Config, loaded.State));
    }

    [Fact]
    public void A_save_with_an_office_the_data_does_not_have_is_a_problem()
    {
        Game g = Lab(policy: 3);
        g.Me.ComplianceLevel = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("compliance office at level 9"));
        g.Me.ComplianceLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("compliance office"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Compliance.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Compliance.MaxLevel = 11;
        c.Compliance.StaffPerLevel = -1;
        c.Compliance.WeeklyCostPerLevel = -1;
        c.Compliance.AttentionPermillePerLevel = 400;
        c.Compliance.AuditFinePermillePerLevel = 400;
        c.Compliance.HearingPointsPerLevel = 40;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "staffPerLevel", "weeklyCostPerLevel", "attentionPermillePerLevel", "auditFinePermillePerLevel", "hearingPointsPerLevel" })
            Assert.Contains(problems, p => p.Contains("compliance." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_run_an_office()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.ComplianceLevel));
        Assert.Empty(Lapsed(g));
    }

    [Fact]
    public void An_autopilot_game_with_an_office_stays_sound_and_the_books_still_balance()
    {
        Game g = Game.New(2, c => c.Compliance.StaffPerLevel = 0);
        g.Autopilot(48);
        g.Me.Cash = Math.Max(g.Me.Cash, 5_000_000);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.SetCompliance(g.Id, 1));
        for (int i = 0; i < 100; i++) g.Autopilot(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
