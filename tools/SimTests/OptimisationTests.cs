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

/// <summary>Inference optimisation: engineers make serving cheaper, which pays when the lab cannot serve everyone and costs reliability.</summary>
public class OptimisationTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, bool strained = false)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Optimise.EngineersPerLevel = 1;
            if (strained) foreach (SectorConfig s in c.Market.Sectors) s.RevenuePerComputeUnit = 1;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, gpus: 6);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        return m;
    }

    static List<GameEvent> Sets(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.OptimisationSet).ToList(); }

    static ModelState Fake() { return new ModelState { Alignment = 500, DisplayedAlignment = 500, Status = ModelStatus.Released }; }

    // ---- nothing happens by itself

    [Fact]
    public void Nothing_is_optimised_until_the_player_asks()
    {
        Game g = Lab();
        ModelState m = Released(g);
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Weeks(20);
        Assert.Equal(0, g.Me.OptimiseLevel);
        Assert.Equal(0, Optimisation.Effective(g.C, g.Me));
        Assert.Equal(0, Optimisation.DemandCutPermille(g.C, g.Me));
        Assert.Equal(0, Optimisation.IncidentPermille(g.C, g.Me));
        Assert.Equal(1000, Optimisation.Serve(g.C, g.Me, 1000));
        Assert.Equal(chance, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
        Assert.Empty(Sets(g));
    }

    // ---- the command

    [Fact]
    public void Raising_a_level_pays_the_setup_fee_per_level_added_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SetOptimisation(g.Id, 1));
        Assert.Equal(cash - 60000, g.Me.Cash);
        GameEvent e = Sets(g).Single();
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(60000, e.Amount);
        Assert.Equal(1, e.Pick);
        Assert.Contains("raised inference optimisation to level 1 for $60000", EventText.Describe(g.C, g.State, e));
        g.Do(Command.SetOptimisation(g.Id, 3));
        Assert.Equal(cash - 180000, g.Me.Cash);
        Assert.Equal(3, g.Me.OptimiseLevel);
    }

    [Fact]
    public void Each_level_needs_its_engineers()
    {
        Game g = Lab();
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer");
        g.Refused(Command.SetOptimisation(g.Id, 1), CommandError.NotEnoughStaff);
        g.Hire("engineer", 1);
        g.Do(Command.SetOptimisation(g.Id, 1));
        g.Refused(Command.SetOptimisation(g.Id, 2), CommandError.NotEnoughStaff);
        Assert.Equal(1, g.Me.OptimiseLevel);
        g.Hire("engineer", 1);
        g.Do(Command.SetOptimisation(g.Id, 2));
        g.Refused(Command.SetOptimisation(g.Id, 3), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void Bad_levels_the_same_level_a_poor_lab_a_rival_a_bankrupt_lab_and_a_game_without_it_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetOptimisation(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetOptimisation(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetOptimisation(g.Id, 0), CommandError.InvalidArgument);

        Game poor = Lab(rich: false);
        poor.Me.Cash = 59_999;
        poor.Refused(Command.SetOptimisation(poor.Id, 1), CommandError.NotEnoughCash);
        Assert.Equal(0, poor.Me.OptimiseLevel);

        Game game = Game.New(1);
        LabState rival = game.State.Labs.First(l => l.Id != game.State.PlayerLabId);
        game.Refused(Command.SetOptimisation(rival.Id, 1), CommandError.InvalidArgument);

        Game off = Lab(c => c.Optimise.MaxLevel = 0);
        off.Refused(Command.SetOptimisation(off.Id, 1), CommandError.InvalidArgument);
        Assert.False(Optimisation.Enabled(off.C));

        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.SetOptimisation(broke.Id, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void Lowering_is_free_and_the_work_has_to_be_paid_for_again()
    {
        Game g = Lab();
        g.Do(Command.SetOptimisation(g.Id, 2));
        long cash = g.Me.Cash;
        g.Do(Command.SetOptimisation(g.Id, 0));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, Sets(g).Last().Pick);
        Assert.Contains("backed inference optimisation down to level 0", EventText.Describe(g.C, g.State, Sets(g).Last()));
        g.Do(Command.SetOptimisation(g.Id, 2));
        Assert.Equal(cash - 120000, g.Me.Cash);
    }

    // ---- what it does

    [Fact]
    public void Each_level_cuts_what_serving_needs_by_the_data_s_share()
    {
        Game g = Lab();
        g.Do(Command.SetOptimisation(g.Id, 1));
        Assert.Equal(100, Optimisation.DemandCutPermille(g.C, g.Me));
        Assert.Equal(900, Optimisation.Serve(g.C, g.Me, 1000));
        g.Do(Command.SetOptimisation(g.Id, 3));
        Assert.Equal(300, Optimisation.DemandCutPermille(g.C, g.Me));
        Assert.Equal(700_000, Optimisation.Serve(g.C, g.Me, 1_000_000));
        Assert.Equal(0, Optimisation.Serve(g.C, g.Me, 0));
    }

    [Fact]
    public void A_lab_that_cannot_serve_everyone_serves_more_and_earns_more_with_it()
    {
        (int served, long revenue) Run(int level)
        {
            Game g = Lab(strained: true);
            Released(g);
            if (level > 0) g.Do(Command.SetOptimisation(g.Id, level));
            g.Weeks(12);
            return (g.Me.LastWeek.ServedPermille, g.Me.LastWeek.Revenue);
        }
        (int servedNone, long revNone) = Run(0);
        (int servedOne, long revOne) = Run(1);
        (int servedThree, long revThree) = Run(3);
        Assert.True(servedNone < 1000);
        Assert.True(servedOne > servedNone);
        Assert.True(servedThree > servedOne);
        Assert.True(revOne > revNone);
        Assert.True(revThree > revOne);
    }

    [Fact]
    public void A_lab_that_already_serves_everyone_gains_nothing_in_revenue_and_still_pays_in_risk()
    {
        long Revenue(int level)
        {
            Game g = Lab(c => c.Optimise.IncidentPermillePerLevel = 0);
            Released(g);
            g.Me.Staff.RemoveAll(p => p.RoleId == "researcher");
            if (level > 0) g.Do(Command.SetOptimisation(g.Id, level));
            g.Weeks(12);
            Assert.Equal(1000, g.Me.LastWeek.ServedPermille);
            return g.Me.LastWeek.Revenue;
        }
        Assert.Equal(Revenue(0), Revenue(2));

        Game h = Lab();
        h.Do(Command.SetOptimisation(h.Id, 2));
        Assert.Equal(16, Optimisation.IncidentPermille(h.C, h.Me));
    }

    [Fact]
    public void The_work_adds_to_the_weekly_incident_chance_and_the_breakdown_shows_it()
    {
        Game g = Lab();
        ModelState m = Fake();
        int before = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.SetOptimisation(g.Id, 2));
        Assert.Equal(before + 16, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.Equal(16, parts.FromOptimisation);
        Assert.Equal(before + 16, parts.Total);
    }

    [Fact]
    public void A_chief_safety_officer_cuts_the_risk_the_work_adds_too()
    {
        Game g = Lab();
        g.Hire("safety", 1);
        ModelState m = Fake();
        g.Do(Command.SetOptimisation(g.Id, 3));
        int without = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        int with = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.Equal(without - (int)MathI.MulPermille(without, 150), with);
        Assert.Equal(with, DisplayQueries.IncidentParts(g.C, g.State, g.Me, m).Total);
    }

    [Fact]
    public void Fewer_engineers_than_the_level_needs_hold_the_effect_down_and_it_comes_back_with_them()
    {
        Game g = Lab();
        g.Do(Command.SetOptimisation(g.Id, 2));
        Assert.Equal(2, Optimisation.Effective(g.C, g.Me));
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer");
        g.Hire("engineer", 1);
        Assert.Equal(2, g.Me.OptimiseLevel);
        Assert.Equal(1, Optimisation.Effective(g.C, g.Me));
        Assert.Equal(100, Optimisation.DemandCutPermille(g.C, g.Me));
        Assert.Equal(8, Optimisation.IncidentPermille(g.C, g.Me));
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer");
        Assert.Equal(0, Optimisation.Effective(g.C, g.Me));
        Assert.Equal(1000, Optimisation.Serve(g.C, g.Me, 1000));
        g.Hire("engineer", 3);
        Assert.Equal(2, Optimisation.Effective(g.C, g.Me));
    }

    [Fact]
    public void A_lab_that_serves_more_has_less_overload_risk()
    {
        int Overload(int level)
        {
            Game g = Lab(strained: true);
            Released(g);
            if (level > 0) g.Do(Command.SetOptimisation(g.Id, level));
            g.Weeks(12);
            return SafetySystem.OverloadPermille(g.C, g.State, g.Me);
        }
        Assert.True(Overload(0) > 0);
        Assert.True(Overload(3) < Overload(0));
    }

    // ---- edge cases

    [Fact]
    public void Zero_values_in_the_data_make_optimisation_a_setup_fee_and_nothing_else()
    {
        Game g = Lab(c => { c.Optimise.DemandCutPermillePerLevel = 0; c.Optimise.IncidentPermillePerLevel = 0; });
        ModelState m = Fake();
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.SetOptimisation(g.Id, 3));
        Assert.Equal(1000, Optimisation.Serve(g.C, g.Me, 1000));
        Assert.Equal(chance, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m));
    }

    [Fact]
    public void A_free_setup_with_no_engineer_rule_works_at_the_top_level()
    {
        Game g = Lab(c => { c.Optimise.SetupCostPerLevel = 0; c.Optimise.EngineersPerLevel = 0; });
        g.Me.Staff.RemoveAll(p => p.RoleId == "engineer");
        long cash = g.Me.Cash;
        g.Do(Command.SetOptimisation(g.Id, 3));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(3, Optimisation.Effective(g.C, g.Me));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void The_level_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab(strained: true);
        Released(g);
        g.Do(Command.SetOptimisation(g.Id, 2));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.OptimiseLevel);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => { c.Optimise.SetupCostPerLevel = 100; c.Optimise.EngineersPerLevel = 0; });
        g.Autopilot(20);
        Assert.True(g.Try(Command.SetOptimisation(g.Id, 2)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(2, again.Player.OptimiseLevel);
    }

    [Fact]
    public void An_older_save_loads_with_nothing_optimised()
    {
        Game g = Lab();
        g.Do(Command.SetOptimisation(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 54, g.Sim.Save());
        Assert.DoesNotContain("optimiseLevel", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.OptimiseLevel));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_level_the_data_does_not_allow_is_reported()
    {
        Game g = Lab();
        g.Do(Command.SetOptimisation(g.Id, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("optimises"));
        g.Me.OptimiseLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("optimises"));
        g.Me.OptimiseLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("optimises"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Optimise.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Optimise.MaxLevel = 11;
        c.Optimise.SetupCostPerLevel = -1;
        c.Optimise.EngineersPerLevel = 101;
        c.Optimise.DemandCutPermillePerLevel = 1000;
        c.Optimise.IncidentPermillePerLevel = 1000;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "setupCostPerLevel", "engineersPerLevel", "demandCutPermillePerLevel", "incidentPermillePerLevel" })
            Assert.Contains(problems, p => p.Contains("optimise." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_optimise()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.OptimiseLevel));
        Assert.Empty(Sets(g));
    }

    [Fact]
    public void A_game_that_raises_and_drops_the_level_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(30);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int raised = 0;
        for (int i = 0; i < 220; i++)
        {
            g.Autopilot(1);
            if (i % 50 == 5 && g.Try(Command.SetOptimisation(g.Id, 2)).Ok) raised++;
            if (i % 50 == 30 && g.Me.OptimiseLevel > 0) g.Try(Command.SetOptimisation(g.Id, 0));
        }
        Assert.True(raised >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
