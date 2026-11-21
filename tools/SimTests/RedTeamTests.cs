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

/// <summary>Red teams: a paid attack on one model that closes some of the alignment gap, fixes a little, and cuts its incident chance for a while.</summary>
public class RedTeamTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, bool attackers = true)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 8);
        if (attackers) StaffSystem.Hire(g.C, g.State, g.Me, g.C.Role("red-teamer"), 1);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    static ModelState Released(Game g, int claimed = 0)
    {
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, claimed, "game-ai", "game-bots"));
        return m;
    }

    static int Chance(Game g, ModelState m) { return SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m); }

    // ---- nothing happens by itself

    [Fact]
    public void No_model_is_red_teamed_until_the_player_pays()
    {
        Game g = Lab();
        ModelState m = Released(g);
        g.Weeks(30);
        Assert.Equal(0, m.RedTeamUntil);
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, g.State, g.Me, m).FromRedTeam);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RedTeamRan);
    }

    // ---- the command

    [Fact]
    public void A_red_team_costs_the_minimum_for_a_model_that_sells_nothing_and_sets_the_weeks()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        long cash = g.Me.Cash;
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(cash - 15000, g.Me.Cash);
        Assert.Equal(g.Tick + 26, m.RedTeamUntil);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.RedTeamRan);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Equal(15000, e.Amount);
        Assert.Contains("paid $15000 for a red team to attack " + m.Name, EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_bill_follows_what_the_model_sells_for_once_that_is_more_than_the_minimum()
    {
        Game g = Lab(c => c.RedTeam.MinCost = 1);
        ModelState m = Released(g);
        g.Weeks(12);
        long revenue = MarketSystem.ModelRevenue(g.State, g.Id, m.Id);
        Assert.True(revenue > 0, "the model should sell for the test to mean something");
        Assert.Equal(revenue * 2, RedTeam.Cost(g.C, g.State, g.Me, m));
        long cash = g.Me.Cash;
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(cash - revenue * 2, g.Me.Cash);
    }

    [Fact]
    public void It_adds_the_data_s_alignment_to_the_real_and_shown_meters_and_culture_to_the_lab()
    {
        Game g = Lab();
        ModelState m = Released(g);
        int real = m.Alignment, shown = m.DisplayedAlignment;
        int culture = SafetyCulture.Level(g.C, g.Me);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(real + 30, m.Alignment);
        Assert.Equal(shown + 30, m.DisplayedAlignment);
        Assert.Equal(culture + 5, SafetyCulture.Level(g.C, g.Me));
    }

    [Fact]
    public void Alignment_never_goes_past_the_top_of_the_meter()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.Alignment = 990;
        m.DisplayedAlignment = 990;
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(1000, m.Alignment);
        Assert.Equal(1000, m.DisplayedAlignment);
    }

    [Fact]
    public void A_model_that_was_sandbagging_comes_back_with_half_its_gap_closed()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.Alignment = 400;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 900, "game-ai"));
        Assert.Equal(900, m.DisplayedAlignment);
        int gap = SafetySystem.AlignmentGap(g.C, m);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(430, m.Alignment);
        Assert.Equal(400 + gap / 2 + 30, m.DisplayedAlignment);
        Assert.True(SafetySystem.AlignmentGap(g.C, m) < gap);
    }

    [Fact]
    public void An_honest_model_has_no_gap_to_close()
    {
        Game g = Lab();
        ModelState m = Released(g);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
    }

    [Fact]
    public void A_gap_the_attack_closes_can_put_the_model_below_the_labs_own_release_gate()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.Alignment = 300;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        g.Do(Command.SetReleaseGate(g.Id, 800));
        Assert.False(SafetyCulture.GateBlocks(g.Me, m));
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.True(SafetyCulture.GateBlocks(g.Me, m));
    }

    // ---- the incident chance

    [Fact]
    public void While_the_weeks_last_the_incident_chance_loses_30_percent_and_the_breakdown_shows_it()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Alignment = 200;
        m.DisplayedAlignment = 200;
        int before = Chance(g, m);
        Assert.True(before > 0);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        int base_ = SafetySystem.IncidentChanceWithoutRush(g.C, g.Tick, m);
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.Equal(-(int)MathI.MulPermille(base_, 300), parts.FromRedTeam);
        Assert.Equal(base_ + parts.FromRedTeam, Chance(g, m));
        Assert.Equal(Chance(g, m), parts.Total);
        Assert.True(Chance(g, m) < base_);
    }

    [Fact]
    public void The_cut_comes_off_the_models_own_chance_and_not_off_overload_or_crunch()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Alignment = 0;
        m.DisplayedAlignment = 0;
        g.Me.CrunchWeeks = 10;
        int crunch = Crunch.IncidentPermille(g.C, g.Me);
        Assert.True(crunch > 0);
        int own = SafetySystem.IncidentChancePermille(g.C, g.Tick, m);
        m.RedTeamUntil = g.Tick + 5;
        int cut = SafetySystem.IncidentChancePermille(g.C, g.Tick, m);
        Assert.Equal(own - (int)MathI.MulPermille(own, 300), cut);
        Assert.Equal(cut + crunch, Chance(g, m));
    }

    [Fact]
    public void The_cut_ends_when_the_weeks_are_over_and_the_model_can_be_attacked_again()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Alignment = 200;
        m.DisplayedAlignment = 200;
        int plain = Chance(g, m);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        int cut = Chance(g, m);
        Assert.True(cut < plain);
        g.Weeks(25);
        Assert.True(RedTeam.Active(g.State, m));
        g.Refused(Command.RedTeamModel(g.Id, m.Id), CommandError.InvalidArgument);
        g.Weeks(1);
        Assert.False(RedTeam.Active(g.State, m));
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, g.State, g.Me, m).FromRedTeam);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.True(RedTeam.Active(g.State, m));
    }

    [Fact]
    public void A_zero_chance_stays_zero_and_a_full_cut_takes_the_model_to_zero()
    {
        Game g = Lab(c => c.RedTeam.IncidentCutPermille = 1000);
        ModelState m = Released(g);
        m.Alignment = 0;
        m.DisplayedAlignment = 0;
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(0, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, g.State, g.Me, m).Total);
        m.Alignment = 1000;
        m.DisplayedAlignment = 1000;
        m.RedTeamUntil = 0;
        Assert.Equal(0, RedTeam.IncidentPermille(g.C, g.Tick, m, 0));
    }

    [Fact]
    public void A_certain_incident_is_cut_to_the_remaining_share_so_the_cap_does_not_hide_it()
    {
        Game g = Lab();
        ModelState m = Released(g);
        g.C.Safety.IncidentChancePermille = 1000;
        m.Alignment = 0;
        m.DisplayedAlignment = 0;
        Assert.Equal(1000, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
        m.RedTeamUntil = g.Tick + 5;
        Assert.Equal(700, SafetySystem.IncidentChancePermille(g.C, g.Tick, m));
    }

    [Fact]
    public void Over_many_weeks_a_red_teamed_model_has_fewer_incidents_than_the_same_model_without_one()
    {
        int plain = 0, covered = 0;
        for (int seed = 1; seed <= 6; seed++)
        {
            foreach (bool red in new[] { false, true })
            {
                Game g = Game.Solo(seed, c => { foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0; c.Office.ChancePermille = 0; }).WithStarterLab(gpus: 8);
                StaffSystem.Hire(g.C, g.State, g.Me, g.C.Role("red-teamer"), 1);
                g.Me.Cash = 1_000_000_000_000;
                ModelState m = Released(g);
                m.Alignment = 300;
                m.DisplayedAlignment = 300;
                if (red) g.Do(Command.RedTeamModel(g.Id, m.Id));
                for (int i = 0; i < 25; i++) g.Weeks(1);
                if (red) covered += m.Incidents; else plain += m.Incidents;
            }
        }
        Assert.True(plain > 0);
        Assert.True(covered < plain);
    }

    // ---- refusals

    [Fact]
    public void A_lab_without_a_red_teamer_is_refused_and_nothing_changes()
    {
        Game g = Lab(attackers: false);
        ModelState m = Trained(g);
        long cash = g.Me.Cash;
        g.Refused(Command.RedTeamModel(g.Id, m.Id), CommandError.NotEnoughStaff);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, m.RedTeamUntil);
    }

    [Fact]
    public void A_missing_model_or_one_still_training_or_retired_is_refused()
    {
        Game g = Lab();
        g.Refused(Command.RedTeamModel(g.Id, 99999), CommandError.UnknownId);
        ModelState training = g.Train("mini", 0);
        g.Refused(Command.RedTeamModel(g.Id, training.Id), CommandError.InvalidArgument);
        g.UntilTrained(training);
        training.Status = ModelStatus.Retired;
        g.Refused(Command.RedTeamModel(g.Id, training.Id), CommandError.InvalidArgument);
        Assert.Equal(0, training.RedTeamUntil);
    }

    [Fact]
    public void A_lab_that_cannot_pay_is_refused_and_a_lab_that_can_just_pay_goes_to_zero()
    {
        Game g = Lab(rich: false);
        ModelState m = Trained(g);
        g.Me.Cash = 14999;
        g.Refused(Command.RedTeamModel(g.Id, m.Id), CommandError.NotEnoughCash);
        Assert.Equal(14999, g.Me.Cash);
        g.Me.Cash = 15000;
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void A_rival_cannot_pay_for_a_red_team()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        long cash = rival.Cash;
        Assert.False(g.Try(Command.RedTeamModel(rival.Id, 0)).Ok);
        Assert.Equal(cash, rival.Cash);
    }

    [Fact]
    public void With_weeks_set_to_zero_there_are_no_red_teams()
    {
        Game g = Lab(c => c.RedTeam.Weeks = 0);
        ModelState m = Trained(g);
        g.Refused(Command.RedTeamModel(g.Id, m.Id), CommandError.InvalidArgument);
        Assert.False(RedTeam.Enabled(g.C));
    }

    [Fact]
    public void An_empty_role_asks_for_no_staff()
    {
        Game g = Lab(c => c.RedTeam.RoleId = "", attackers: false);
        ModelState m = Trained(g);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.True(RedTeam.Active(g.State, m));
    }

    // ---- save, replay, migration, data

    [Fact]
    public void A_saved_game_keeps_the_weeks_and_a_load_plays_on_the_same()
    {
        Game g = Lab();
        ModelState m = Released(g);
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        g.Weeks(7);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(m.RedTeamUntil, back.Player.Model(m.Id).RedTeamUntil);
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(c => c.RedTeam.RoleId = "", rich: false, attackers: false);
        ModelState m = Released(g);
        g.Weeks(6);
        Assert.True(g.Try(Command.RedTeamModel(g.Id, m.Id)).Ok);
        g.Weeks(30);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_schema_44_save_loads_with_no_model_red_teamed()
    {
        Game g = Lab();
        Released(g);
        g.Weeks(4);
        string old = SaveSchemaPretend.At(g.C, 44, g.Sim.Save());
        Assert.DoesNotContain("redTeamUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.Player.Models, m => Assert.Equal(0, m.RedTeamUntil));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_red_team_end_is_reported()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        m.RedTeamUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("red team"));
        m.RedTeamUntil = g.Tick + 27;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("red team"));
        m.RedTeamUntil = g.Tick + 26;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("red team"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(26, c.RedTeam.Weeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.RedTeam.RoleId = "nobody";
        c.RedTeam.MinStaff = -1;
        c.RedTeam.RevenueWeeks = 521;
        c.RedTeam.MinCost = -1;
        c.RedTeam.Weeks = 521;
        c.RedTeam.IncidentCutPermille = 1001;
        c.RedTeam.GapClosePermille = -1;
        c.RedTeam.AlignmentGain = 1001;
        c.RedTeam.CulturePoints = 101;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "roleId", "minStaff", "revenueWeeks", "minCost", "redTeam.weeks", "incidentCutPermille", "gapClosePermille", "alignmentGain", "culturePoints" })
            Assert.Contains(problems, p => p.Contains("redTeam." + key.Replace("redTeam.", "")));
    }

    [Fact]
    public void Rivals_and_the_autopilot_never_use_a_red_team()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RedTeamRan);
        Assert.All(g.State.Labs, lab => Assert.All(lab.Models, m => Assert.Equal(0, m.RedTeamUntil)));
    }
}
