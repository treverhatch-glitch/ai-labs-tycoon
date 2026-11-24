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

/// <summary>Retraining: one person moves to another role for a share of its hire fee and some morale, if a seat is free.</summary>
public class RetrainTests
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
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 6);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 600; }
        return g;
    }

    static StaffMember First(Game g, string roleId) { return g.Me.Staff.First(p => p.RoleId == roleId); }

    static List<GameEvent> Moves(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.StaffRetrained).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void Nobody_changes_role_until_the_player_says_so()
    {
        Game g = Lab();
        string[] roles = g.Me.Staff.Select(p => p.RoleId).ToArray();
        g.Weeks(30);
        Assert.Equal(roles, g.Me.Staff.Select(p => p.RoleId).ToArray());
        Assert.Empty(Moves(g));
    }

    // ---- the command

    [Fact]
    public void Changing_function_costs_half_the_hire_fee_and_some_morale_and_keeps_the_person()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        StaffMember p = First(g, "sales");
        p.TraitId = "steady";
        int id = p.Id;
        RoleConfig policy = g.C.Role("policy");
        long fee = MathI.MulPermille(RolePerks.HireFee(g.C, g.Me, policy, g.Tick), 500);
        long cash = g.Me.Cash;
        long hiring = g.Me.Ledger.Lifetime.Hiring;
        int staff = g.Me.Staff.Count;
        g.Do(Command.RetrainStaff(g.Id, id, "policy"));
        Assert.Equal("policy", p.RoleId);
        Assert.Equal("steady", p.TraitId);
        Assert.Equal(480, p.Morale);
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(hiring - fee, g.Me.Ledger.Lifetime.Hiring);
        GameEvent e = Moves(g).Single();
        Assert.Equal(0, e.Pick);
        Assert.Equal(id, e.SubjectId);
        Assert.Equal(fee, e.Amount);
        Assert.Contains("retrained a Sales Rep as a Policy Lead", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_move_inside_a_function_costs_a_quarter_of_the_fee_and_less_morale()
    {
        Game g = Lab();
        g.Build("labelling-room", 4, 3);
        StaffMember p = First(g, "engineer");
        RoleConfig labeller = g.C.Role("labeller");
        long fee = MathI.MulPermille(RolePerks.HireFee(g.C, g.Me, labeller, g.Tick), 250);
        long cash = g.Me.Cash;
        g.Do(Command.RetrainStaff(g.Id, p.Id, "labeller"));
        Assert.Equal("labeller", p.RoleId);
        Assert.Equal(560, p.Morale);
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(1, Moves(g).Single().Pick);
        Assert.DoesNotContain("not thrilled", EventText.Describe(g.C, g.State, Moves(g).Single()));
    }

    [Fact]
    public void It_is_cheaper_than_letting_someone_go_and_hiring_for_the_same_job()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        RoleConfig sales = g.C.Role("sales");
        RoleConfig policy = g.C.Role("policy");
        long retrain = Retraining.Fee(g.C, g.Me, sales, policy, g.Tick);
        long swap = sales.Severance + RolePerks.HireFee(g.C, g.Me, policy, g.Tick);
        Assert.True(retrain < swap);
    }

    [Fact]
    public void The_weekly_salary_follows_the_new_role()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        long before = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.RetrainStaff(g.Id, First(g, "sales").Id, "policy"));
        Assert.Equal(before + (1200 - 800), LabQueries.WeeklySalaries(g.C, g.Me));
        long bill = LabQueries.WeeklySalaries(g.C, g.Me);
        long paid = g.Me.Ledger.Lifetime.Salaries;
        g.Weeks(1);
        Assert.Equal(paid - bill, g.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void The_pay_policy_and_equity_apply_to_the_new_salary_too()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        g.Me.PayLevel = 1;
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.RetrainStaff(g.Id, First(g, "sales").Id, "policy"));
        long raw = g.Me.Staff.Sum(p => g.C.Role(p.RoleId).WeeklySalary);
        int step = g.C.StaffPeople.PaySalaryStepPermille;
        Assert.Equal(MathI.MulPermille(raw, 1000 + step - 60), LabQueries.WeeklySalaries(g.C, g.Me));
    }

    // ---- seats

    [Fact]
    public void The_new_role_needs_a_free_seat_and_the_old_one_is_freed()
    {
        Game g = Lab();
        int researchSeats = LabQueries.Seats(g.C, g.Me, "researcher");
        g.Hire("researcher", researchSeats - LabQueries.StaffCount(g.Me, "researcher"));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 600;
        StaffMember engineer = First(g, "engineer");
        g.Refused(Command.RetrainStaff(g.Id, engineer.Id, "researcher"), CommandError.NoSeats);
        Assert.Equal("engineer", engineer.RoleId);

        int engineerSeats = LabQueries.Seats(g.C, g.Me, "engineer");
        int freeEngineer = engineerSeats - LabQueries.StaffCount(g.Me, "engineer");
        g.Do(Command.RetrainStaff(g.Id, First(g, "researcher").Id, "engineer"));
        Assert.Equal(freeEngineer - 1, engineerSeats - LabQueries.StaffCount(g.Me, "engineer"));
        Assert.Equal(1, LabQueries.Seats(g.C, g.Me, "researcher") - LabQueries.StaffCount(g.Me, "researcher"));
        g.Do(Command.Hire(g.Id, "researcher", 1));
    }

    // ---- refusals

    [Fact]
    public void An_unknown_person_an_unknown_role_and_the_same_role_are_refused()
    {
        Game g = Lab();
        StaffMember p = First(g, "engineer");
        g.Refused(Command.RetrainStaff(g.Id, 99999, "sales"), CommandError.UnknownId);
        g.Refused(Command.RetrainStaff(g.Id, p.Id, "wizard"), CommandError.UnknownId);
        g.Refused(Command.RetrainStaff(g.Id, p.Id, "engineer"), CommandError.InvalidArgument);
        Assert.Equal("engineer", p.RoleId);
        Assert.Empty(Moves(g));
    }

    [Fact]
    public void A_person_who_is_too_low_and_too_little_cash_are_refused_and_change_nothing()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        StaffMember p = First(g, "sales");
        p.Morale = 299;
        g.Refused(Command.RetrainStaff(g.Id, p.Id, "policy"), CommandError.TeamTooTired);
        Assert.Equal("sales", p.RoleId);
        Assert.Equal(299, p.Morale);
        p.Morale = 300;
        g.Me.Cash = 10;
        g.Refused(Command.RetrainStaff(g.Id, p.Id, "policy"), CommandError.NotEnoughCash);
        Assert.Equal(10, g.Me.Cash);
        Assert.Equal("sales", p.RoleId);
    }

    [Fact]
    public void A_second_move_straight_after_the_first_is_refused_because_the_person_is_too_low()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        g.Build("labelling-room", 4, 3);
        StaffMember p = First(g, "sales");
        p.Morale = 400;
        g.Do(Command.RetrainStaff(g.Id, p.Id, "policy"));
        Assert.Equal(280, p.Morale);
        g.Refused(Command.RetrainStaff(g.Id, p.Id, "labeller"), CommandError.TeamTooTired);
        Assert.Equal("policy", p.RoleId);
    }

    [Fact]
    public void Morale_never_goes_below_zero()
    {
        Game g = Lab(c => c.Retrain.MinMorale = 0);
        g.Build("policy-office", 4, 3);
        StaffMember p = First(g, "sales");
        p.Morale = 50;
        g.Do(Command.RetrainStaff(g.Id, p.Id, "policy"));
        Assert.Equal(0, p.Morale);
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_it_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.RetrainStaff(rival.Id, 1, "sales"), CommandError.InvalidArgument);
        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.RetrainStaff(broke.Id, First(broke, "sales").Id, "engineer"), CommandError.LabBankrupt);
        Game off = Lab(c => c.Retrain.Enabled = false);
        off.Refused(Command.RetrainStaff(off.Id, First(off, "sales").Id, "engineer"), CommandError.InvalidArgument);
        Assert.False(Retraining.Enabled(off.C));
    }

    [Fact]
    public void A_free_retrain_works_with_no_cash_at_all()
    {
        Game g = Lab(c => { c.Retrain.FeePermille = 0; c.Retrain.SameFunctionFeePermille = 0; }, rich: false);
        g.Build("policy-office", 4, 3);
        g.Me.Cash = 0;
        g.Do(Command.RetrainStaff(g.Id, First(g, "sales").Id, "policy"));
        Assert.Equal(0, g.Me.Cash);
    }

    // ---- other systems

    [Fact]
    public void Moving_a_safety_tester_out_of_safety_costs_culture_but_moving_them_to_another_listener_does_not()
    {
        Game g = Lab(c => c.Culture.ListeningRoles.Add("engineer"));
        int culture = SafetyCulture.Level(g.C, g.Me);
        g.Do(Command.RetrainStaff(g.Id, First(g, "safety").Id, "sales"));
        Assert.Equal(culture - 15, SafetyCulture.Level(g.C, g.Me));
        int again = SafetyCulture.Level(g.C, g.Me);
        g.Do(Command.RetrainStaff(g.Id, First(g, "sales").Id, "safety"));
        Assert.Equal(again, SafetyCulture.Level(g.C, g.Me));
        g.Do(Command.RetrainStaff(g.Id, First(g, "safety").Id, "engineer"));
        Assert.Equal(again, SafetyCulture.Level(g.C, g.Me));
    }

    [Fact]
    public void Losing_the_policy_person_drops_the_compliance_office_the_next_week()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        g.Hire("policy", 1);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 600;
        g.Do(Command.SetCompliance(g.Id, 1));
        g.Do(Command.RetrainStaff(g.Id, First(g, "policy").Id, "sales"));
        g.Weeks(1);
        Assert.Equal(0, g.Me.ComplianceLevel);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.ComplianceLapsed);
    }

    [Fact]
    public void An_audit_needs_safety_staff_after_a_move_too()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 600;
        g.Do(Command.RetrainStaff(g.Id, First(g, "safety").Id, "sales"));
        g.Refused(Command.CommissionAudit(g.Id, m.Id), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void A_hack_week_needs_its_researchers_after_a_move_too()
    {
        Game g = Lab();
        g.Do(Command.RetrainStaff(g.Id, First(g, "researcher").Id, "engineer"));
        g.Do(Command.RetrainStaff(g.Id, First(g, "researcher").Id, "engineer"));
        Assert.Equal(1, LabQueries.StaffCount(g.Me, "researcher"));
        g.Refused(Command.HoldHackWeek(g.Id), CommandError.NotEnoughStaff);
    }

    // ---- saving, loading

    [Fact]
    public void A_retrained_person_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        StaffMember p = First(g, "sales");
        g.Do(Command.RetrainStaff(g.Id, p.Id, "policy"));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal("policy", back.Player.Staff.First(x => x.Id == p.Id).RoleId);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2);
        g.Autopilot(30);
        StaffMember p = g.Me.Staff.First(x => x.Morale >= 300 && x.RoleId == "engineer" || x.Morale >= 300 && x.RoleId == "researcher");
        string other = p.RoleId == "engineer" ? "researcher" : "engineer";
        g.Try(Command.RetrainStaff(g.Id, p.Id, other));
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_save_with_a_person_in_a_role_the_data_has_stays_valid()
    {
        Game g = Lab();
        g.Build("policy-office", 4, 3);
        g.Do(Command.RetrainStaff(g.Id, First(g, "sales").Id, "policy"));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.All(g.Me.Staff, p => Assert.NotNull(g.C.Role(p.RoleId)));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Retrain.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Retrain.FeePermille = 1001;
        c.Retrain.SameFunctionFeePermille = 1002;
        c.Retrain.MoraleDrop = 1001;
        c.Retrain.SameFunctionMoraleDrop = 1002;
        c.Retrain.MinMorale = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "feePermille", "sameFunctionFeePermille", "moraleDrop", "sameFunctionMoraleDrop", "minMorale" })
            Assert.Contains(problems, p => p.Contains("retrain." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_retrain_anyone()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.Empty(Moves(g));
    }

    [Fact]
    public void An_autopilot_game_with_the_odd_retrain_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(40);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int moved = 0;
        for (int i = 0; i < 200; i++)
        {
            g.Autopilot(1);
            if (i % 25 != 0) continue;
            foreach (StaffMember p in g.Me.Staff.ToList())
            {
                string to = p.RoleId == "engineer" ? "researcher" : p.RoleId == "researcher" ? "engineer" : null;
                if (to != null && g.Try(Command.RetrainStaff(g.Id, p.Id, to)).Ok) { moved++; break; }
            }
        }
        Assert.True(moved >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(moved, Moves(g).Count);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
    }
}
