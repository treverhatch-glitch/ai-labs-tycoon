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

/// <summary>Incident response: an on-call rota that softens what an incident costs and wears the team down.</summary>
public class OnCallTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, int engineers = 4, int safety = 2)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Scaling.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: engineers, safety: safety, gpus: 6);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        return m;
    }

    static GameEvent Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        int before = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        return g.State.Events.Skip(before).First(e => e.Kind == EventKind.Incident);
    }

    static long Salaries(Game g) { return g.Me.Ledger.Lifetime.Salaries; }

    // ---- nothing happens by itself

    [Fact]
    public void No_rota_runs_until_the_player_sets_one()
    {
        Game g = Lab();
        g.Weeks(20);
        Assert.Equal(0, g.Me.OnCallLevel);
        Assert.Equal(0, OnCall.Level(g.C, g.Me));
        Assert.Equal(0, OnCall.Effective(g.C, g.Me));
        Assert.Equal(0, OnCall.WeeklyCost(g.C, g.Me));
        Assert.Equal(0, OnCall.FineCutPermille(g.C, g.Me));
        Assert.Equal(0, OnCall.ReputationCutPermille(g.C, g.Me));
        Assert.Equal(0, OnCall.MoraleDropCutPermille(g.C, g.Me));
        Assert.Equal(0, OnCall.MoralePenalty(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OnCallSet);
    }

    // ---- the command

    [Fact]
    public void Setting_a_level_costs_nothing_up_front_locks_the_rota_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SetOnCall(g.Id, 2));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(2, g.Me.OnCallLevel);
        Assert.Equal(g.Tick + 8, g.Me.OnCallLockedUntil);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.OnCallSet);
        Assert.Equal(1, e.Pick);
        Assert.Equal(2, e.SubjectId);
        Assert.Contains("on-call rota (level 2)", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Bad_levels_the_same_level_a_rival_a_bankrupt_lab_and_a_game_without_a_rota_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetOnCall(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetOnCall(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetOnCall(g.Id, 0), CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.OnCallLevel);
        Assert.Equal(0, g.Me.OnCallLockedUntil);

        Game game = Game.New(1);
        LabState rival = game.State.Labs.First(l => l.Id != game.State.PlayerLabId);
        game.Refused(Command.SetOnCall(rival.Id, 1), CommandError.InvalidArgument);

        Game off = Lab(c => c.OnCall.MaxLevel = 0);
        off.Refused(Command.SetOnCall(off.Id, 1), CommandError.InvalidArgument);
        Assert.False(OnCall.Enabled(off.C));

        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.SetOnCall(broke.Id, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void A_lab_without_enough_safety_people_and_engineers_cannot_raise_the_level()
    {
        Game g = Lab(engineers: 1, safety: 2);
        Assert.Equal(1, OnCall.StaffCover(g.C, g.Me));
        g.Refused(Command.SetOnCall(g.Id, 2), CommandError.NotEnoughStaff);
        Assert.Equal(0, g.Me.OnCallLevel);
        g.Do(Command.SetOnCall(g.Id, 1));
        Assert.Equal(1, g.Me.OnCallLevel);
    }

    [Fact]
    public void Any_change_locks_the_rota_for_the_data_s_weeks_and_then_it_can_be_changed_again()
    {
        Game g = Lab();
        g.Do(Command.SetOnCall(g.Id, 1));
        g.Refused(Command.SetOnCall(g.Id, 2), CommandError.OnCallLocked);
        g.Refused(Command.SetOnCall(g.Id, 0), CommandError.OnCallLocked);
        Assert.Equal(1, g.Me.OnCallLevel);
        g.Weeks(7);
        g.Refused(Command.SetOnCall(g.Id, 2), CommandError.OnCallLocked);
        g.Weeks(1);
        g.Do(Command.SetOnCall(g.Id, 0));
        Assert.Equal(0, g.Me.OnCallLevel);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.OnCallSet);
        Assert.Equal(0, e.Pick);
        Assert.Contains("took its on-call rota down to level 0", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_zero_lock_lets_the_rota_change_every_week()
    {
        Game g = Lab(c => c.OnCall.LockWeeks = 0);
        g.Do(Command.SetOnCall(g.Id, 1));
        g.Do(Command.SetOnCall(g.Id, 3));
        g.Do(Command.SetOnCall(g.Id, 0));
        Assert.Equal(3, g.State.Events.Count(e => e.Kind == EventKind.OnCallSet));
    }

    // ---- the weekly bill and the people behind it

    [Fact]
    public void The_rota_is_paid_for_every_week_as_salaries()
    {
        Game g = Lab(c => c.Marketing.MaxLevel = 0);
        Game h = Lab(c => c.Marketing.MaxLevel = 0);
        g.Do(Command.SetOnCall(g.Id, 3));
        g.Weeks(10);
        h.Weeks(10);
        Assert.Equal(4500, OnCall.WeeklyCost(g.C, g.Me));
        Assert.Equal(-45000, Salaries(g) - Salaries(h));
    }

    [Fact]
    public void The_level_that_counts_is_held_down_by_the_people_and_the_bill_follows_it()
    {
        Game g = Lab();
        g.Do(Command.SetOnCall(g.Id, 3));
        Assert.Equal(3, OnCall.Effective(g.C, g.Me));
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer");
        Assert.Equal(1, OnCall.StaffCover(g.C, g.Me));
        Assert.Equal(3, OnCall.Level(g.C, g.Me));
        Assert.Equal(1, OnCall.Effective(g.C, g.Me));
        Assert.Equal(1500, OnCall.WeeklyCost(g.C, g.Me));
        Assert.Equal(100, OnCall.FineCutPermille(g.C, g.Me));
        g.Do(Command.Hire(g.Id, "engineer", 4));
        Assert.Equal(3, OnCall.Effective(g.C, g.Me));
    }

    [Fact]
    public void With_nobody_on_the_payroll_the_rota_does_nothing_and_costs_nothing()
    {
        Game g = Lab();
        g.Do(Command.SetOnCall(g.Id, 2));
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer" || p.RoleId == "safety");
        Assert.Equal(0, OnCall.Effective(g.C, g.Me));
        Assert.Equal(0, OnCall.WeeklyCost(g.C, g.Me));
        Assert.Equal(0, OnCall.MoralePenalty(g.C, g.Me));
        Assert.Equal(2, g.Me.OnCallLevel);
    }

    // ---- what it does when an incident lands

    [Fact]
    public void An_incident_costs_a_smaller_fine_with_a_rota()
    {
        Game plain = Lab();
        Game rota = Lab();
        ModelState pm = Selling(plain);
        ModelState rm = Selling(rota);
        rota.Do(Command.SetOnCall(rota.Id, 3));
        GameEvent a = Incident(plain, pm);
        GameEvent b = Incident(rota, rm);
        Assert.Equal(a.Pick, b.Pick);
        Assert.True(a.Amount > 0);
        Assert.Equal(MathI.MulPermille(a.Amount, 700), b.Amount);
    }

    [Fact]
    public void An_incident_costs_less_reputation_with_a_rota()
    {
        int Loss(int level)
        {
            Game g = Lab();
            ModelState m = Selling(g);
            if (level > 0) g.Do(Command.SetOnCall(g.Id, level));
            g.Me.Reputation = 500;
            Incident(g, m);
            return 500 - g.Me.Reputation;
        }
        int none = Loss(0);
        Assert.True(none > 0);
        Assert.Equal(MathI.MulPermille(none, 900), Loss(1));
        Assert.True(Loss(3) < Loss(2) && Loss(2) < Loss(1));
        Assert.Equal(MathI.MulPermille(none, 700), Loss(3));
    }

    [Fact]
    public void The_team_loses_less_morale_when_an_incident_lands_and_settles_lower()
    {
        Game g = Lab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 70;
        StaffSystem.OnIncident(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(55, p.Morale));

        g.Do(Command.SetOnCall(g.Id, 3));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 70;
        StaffSystem.OnIncident(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(67, p.Morale));

        Game h = Lab();
        Assert.Equal(Workplace.RestingMorale(h.C, h.Me) - 9, Workplace.RestingMorale(g.C, g.Me));
        Assert.Equal(Workplace.Report(h.C, h.Me).RestingMorale - 9, Workplace.Report(g.C, g.Me).RestingMorale);
    }

    [Fact]
    public void A_rota_and_an_ethics_board_stack_on_the_reputation_cost()
    {
        int Loss(bool board, int level)
        {
            Game g = Lab();
            ModelState m = Selling(g);
            if (board) g.Me.EthicsLevel = 1;
            if (level > 0) g.Do(Command.SetOnCall(g.Id, level));
            g.Me.Reputation = 500;
            Incident(g, m);
            return 500 - g.Me.Reputation;
        }
        int none = Loss(false, 0);
        int onlyBoard = Loss(true, 0);
        int onlyRota = Loss(false, 2);
        int both = Loss(true, 2);
        Assert.True(onlyBoard < none && onlyRota < none);
        Assert.True(both < onlyBoard && both < onlyRota);
    }

    [Fact]
    public void Cuts_are_capped_even_if_the_data_asks_for_more_than_everything()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.C.OnCall.FineCutPermillePerLevel = 1000;
        g.C.OnCall.ReputationCutPermillePerLevel = 1000;
        g.C.OnCall.MoraleDropCutPermillePerLevel = 1000;
        g.Do(Command.SetOnCall(g.Id, 3));
        Assert.Equal(900, OnCall.FineCutPermille(g.C, g.Me));
        Assert.Equal(900, OnCall.ReputationCutPermille(g.C, g.Me));
        Assert.Equal(1000, OnCall.MoraleDropCutPermille(g.C, g.Me));
        GameEvent e = Incident(g, m);
        Assert.True(e.Amount >= 0);
        Assert.True(g.Me.Reputation >= 0);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 70;
        StaffSystem.OnIncident(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(70, p.Morale));
    }

    [Fact]
    public void Zero_values_in_the_data_make_the_rota_a_cost_with_no_effect()
    {
        Game g = Lab(c => { c.OnCall.FineCutPermillePerLevel = 0; c.OnCall.ReputationCutPermillePerLevel = 0; c.OnCall.MoraleDropCutPermillePerLevel = 0; c.OnCall.MoralePenaltyPerLevel = 0; c.OnCall.WeeklyCostPerLevel = 0; });
        g.Do(Command.SetOnCall(g.Id, 3));
        Assert.Equal(0, OnCall.FineCutPermille(g.C, g.Me));
        Assert.Equal(0, OnCall.MoralePenalty(g.C, g.Me));
        Assert.Equal(0, OnCall.WeeklyCost(g.C, g.Me));
        Game h = Lab(c => { c.OnCall.FineCutPermillePerLevel = 0; c.OnCall.ReputationCutPermillePerLevel = 0; });
        Assert.Equal(Workplace.RestingMorale(h.C, h.Me), Workplace.RestingMorale(g.C, g.Me));
    }

    [Fact]
    public void A_rota_changes_nothing_about_how_often_incidents_happen()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        int before = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.SetOnCall(g.Id, 3));
        Assert.Equal(before, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_rota_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.SetOnCall(g.Id, 2));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.OnCallLevel);
        Assert.Equal(g.Me.OnCallLockedUntil, back.Player.OnCallLockedUntil);
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(rich: false);
        g.Do(Command.SetOnCall(g.Id, 2));
        g.Weeks(12);
        g.Do(Command.SetOnCall(g.Id, 1));
        g.Weeks(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(1, again.Player.OnCallLevel);
    }

    [Fact]
    public void An_older_save_loads_with_no_rota()
    {
        Game g = Lab();
        g.Do(Command.SetOnCall(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 55, g.Sim.Save());
        Assert.DoesNotContain("onCallLevel", old);
        Assert.DoesNotContain("onCallLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.OnCallLevel); Assert.Equal(0, lab.OnCallLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_rota_outside_the_data_is_reported()
    {
        Game g = Lab();
        g.Do(Command.SetOnCall(g.Id, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("on-call"));
        g.Me.OnCallLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("on-call rota of level 4"));
        g.Me.OnCallLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("on-call"));
        g.Me.OnCallLevel = 1;
        g.Me.OnCallLockedUntil = -5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("on-call"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.OnCall.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.OnCall.MaxLevel = 11;
        c.OnCall.WeeklyCostPerLevel = -1;
        c.OnCall.StaffPerLevel = 101;
        c.OnCall.FineCutPermillePerLevel = 1000;
        c.OnCall.ReputationCutPermillePerLevel = 1000;
        c.OnCall.MoraleDropCutPermillePerLevel = 1000;
        c.OnCall.MoralePenaltyPerLevel = 10;
        c.OnCall.LockWeeks = 105;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "weeklyCostPerLevel", "staffPerLevel", "fineCutPermillePerLevel", "reputationCutPermillePerLevel", "moraleDropCutPermillePerLevel", "moralePenaltyPerLevel", "lockWeeks" })
            Assert.Contains(problems, p => p.Contains("onCall." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_set_a_rota()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.OnCallLevel));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OnCallSet);
    }

    [Fact]
    public void A_long_game_with_a_changing_rota_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c =>
        {
            c.OnCall.LockWeeks = 4;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Scaling.Enabled = 0;
        });
        g.Autopilot(60);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int set = 0;
        for (int i = 0; i < 260; i++)
        {
            g.Autopilot(1);
            if (i % 20 == 5 && g.Try(Command.SetOnCall(g.Id, (i / 20) % 2 == 0 ? 1 : 0)).Ok) set++;
        }
        Assert.Equal(set, g.State.Events.Count(e => e.Kind == EventKind.OnCallSet));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
