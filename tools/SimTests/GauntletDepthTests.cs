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

/// <summary>The Gauntlet, deeper: tuning for the test, the risk of being exposed, the live demo, joke awards and the season table.</summary>
public class GauntletDepthTests
{
    const string Sector = "consumer-assistant";

    /// <summary>A game with every rival stripped of models, stage nerves off, and a rich player, so a test can plant exactly the entrants it wants.</summary>
    static Game Stage(Action<GameConfig> tweak = null, bool rivals = true, long seed = 7, string era = "chat-boom")
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = era };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.RolePerks.Clear();
            c.Gauntlet.JitterPermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.Me.Cash = 1_000_000_000_000;
        foreach (LabState lab in g.State.Labs) lab.Models.Clear();
        foreach (SectorState s in g.State.Sectors) s.Entries.Clear();
        return g;
    }

    static ModelState Entrant(Game g, LabState lab, int capability, int alignment = 500, int displayed = -1, int releasedTick = -2, bool inSector = false)
    {
        var m = new ModelState
        {
            Id = 9000 + lab.Id * 10 + lab.Models.Count,
            LineId = "mini",
            Version = 1,
            Name = lab.Name + " Mk" + lab.Models.Count,
            Status = ModelStatus.Released,
            Capability = capability,
            Alignment = alignment,
            DisplayedAlignment = displayed < 0 ? alignment : displayed,
            ReleasedTick = releasedTick == -2 ? g.Tick : releasedTick,
            LastIncidentTick = -1
        };
        lab.Models.Add(m);
        if (inSector)
        {
            m.Sectors.Add(Sector);
            g.State.Sectors.First(s => s.Id == Sector).Entries.Add(new SectorEntry { LabId = lab.Id, ModelId = m.Id });
        }
        return m;
    }

    static GauntletRecord Run(Game g) => GauntletSystem.RunYear(g.C, g.State);

    static GauntletEntry Crown(GauntletRecord r, LabState lab) => r.Entries.Single(e => e.Bracket == GauntletSystem.CrownBracket && e.LabId == lab.Id);

    static int Count(Game g, EventKind kind) => g.State.Events.Count(e => e.Kind == kind);

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    static long SalesFee(Game g) => EraRules.HireFee(g.C, g.C.Role("sales"), g.Tick);

    // ---- the table

    [Fact]
    public void The_default_config_tunes_up_to_30_percent_for_half_of_it_as_score_and_the_table_is_valid()
    {
        GauntletConfig g = TestData.Config().Gauntlet;
        Assert.Equal(300, g.MaxTuningPermille);
        Assert.Equal(500, g.TuningScoreSharePermille);
        Assert.Equal(8, g.DemoTriumphBelowPermille);
        Assert.Equal(20, g.DemoMemeFromPermille);
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }

    [Fact]
    public void Bad_gauntlet_numbers_are_caught_by_the_validator()
    {
        AssertProblem(Problems(c => c.Gauntlet.MaxTuningPermille = 1001), "maxTuningPermille");
        AssertProblem(Problems(c => c.Gauntlet.MaxTuningPermille = -1), "maxTuningPermille");
        AssertProblem(Problems(c => c.Gauntlet.TuningScoreSharePermille = 1001), "tuningScoreSharePermille");
        AssertProblem(Problems(c => c.Gauntlet.TuningCostHireFeesPer100 = -1), "tuningCostHireFeesPer100");
        AssertProblem(Problems(c => c.Gauntlet.ExposeChancePerTuningPermille = 0), "free money");
        AssertProblem(Problems(c => c.Gauntlet.ExposeFeudBonusPermille = -1), "cannot be negative");
        AssertProblem(Problems(c => c.Gauntlet.ExposedReputationLoss = -1), "reputation amounts");
        AssertProblem(Problems(c => c.Gauntlet.DemoMemeReputation = -5), "reputation amounts");
        AssertProblem(Problems(c => c.Gauntlet.DemoTriumphBelowPermille = 20), "both");
        AssertProblem(Problems(c => c.Gauntlet.DemoTriumphBelowPermille = -1), "both");
        AssertProblem(Problems(c => c.Gauntlet.ConfidentWrongMinGap = 0), "confidentWrongMinGap");
        AssertProblem(Problems(c => c.Gauntlet.NewcomerYears = -1), "newcomerYears");
        AssertProblem(Problems(c => c.Gauntlet.NewcomerMorale = 501), "newcomerMorale");
        AssertProblem(Problems(c => c.Gauntlet.SeasonBracketPoints = -1), "season points");
    }

    [Fact]
    public void Switching_tuning_and_the_demo_off_is_valid()
    {
        Assert.Empty(Problems(c => { c.Gauntlet.MaxTuningPermille = 0; c.Gauntlet.ExposeChancePerTuningPermille = 0; }));
        Assert.Empty(Problems(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; }));
        Assert.Empty(Problems(c => c.Gauntlet.NewcomerYears = 0));
    }

    // ---- the tuning setting

    [Fact]
    public void Tuning_is_set_by_command_and_zero_and_the_maximum_are_both_allowed()
    {
        Game g = Stage(rivals: false);
        Assert.Equal(0, g.Me.GauntletTuning);
        g.Do(Command.SetGauntletTuning(g.Id, 120));
        Assert.Equal(120, g.Me.GauntletTuning);
        g.Do(Command.SetGauntletTuning(g.Id, g.C.Gauntlet.MaxTuningPermille));
        Assert.Equal(300, g.Me.GauntletTuning);
        g.Do(Command.SetGauntletTuning(g.Id, 0));
        Assert.Equal(0, g.Me.GauntletTuning);
    }

    [Fact]
    public void A_tuning_above_the_maximum_or_below_zero_is_refused_and_changes_nothing()
    {
        Game g = Stage(rivals: false);
        g.Do(Command.SetGauntletTuning(g.Id, 100));
        g.Refused(Command.SetGauntletTuning(g.Id, 301), CommandError.InvalidArgument);
        g.Refused(Command.SetGauntletTuning(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(100, g.Me.GauntletTuning);
    }

    [Fact]
    public void Where_tuning_is_switched_off_the_command_is_refused()
    {
        Game g = Stage(c => { c.Gauntlet.MaxTuningPermille = 0; c.Gauntlet.ExposeChancePerTuningPermille = 0; }, rivals: false);
        g.Refused(Command.SetGauntletTuning(g.Id, 10), CommandError.InvalidArgument);
        Assert.Equal(0, GauntletSystem.TuningLift(g.C, g.Me));
    }

    [Fact]
    public void The_score_lift_is_the_tuning_times_the_share_and_stops_at_the_maximum()
    {
        Game g = Stage(rivals: false);
        Assert.Equal(0, GauntletSystem.TuningLift(g.C, g.Me));
        g.Me.GauntletTuning = 100;
        Assert.Equal(50, GauntletSystem.TuningLift(g.C, g.Me));
        g.Me.GauntletTuning = 300;
        Assert.Equal(150, GauntletSystem.TuningLift(g.C, g.Me));
        g.Me.GauntletTuning = 900;
        Assert.Equal(150, GauntletSystem.TuningLift(g.C, g.Me));
    }

    [Fact]
    public void The_prep_bill_is_sales_hire_fees_for_each_hundred_and_free_at_zero()
    {
        Game g = Stage(rivals: false);
        long fee = SalesFee(g);
        Assert.Equal(0, GauntletSystem.TuningCost(g.C, g.Tick, 0));
        Assert.Equal(fee * 10, GauntletSystem.TuningCost(g.C, g.Tick, 100));
        Assert.Equal(fee * 30, GauntletSystem.TuningCost(g.C, g.Tick, 300));
        Assert.Equal(fee * 5, GauntletSystem.TuningCost(g.C, g.Tick, 50));
    }

    [Fact]
    public void The_prep_bill_follows_the_era_hire_fee()
    {
        Game early = Stage(rivals: false, era: "dial-up-days");
        Game late = Stage(rivals: false, era: "chat-boom");
        Assert.NotEqual(SalesFee(early), SalesFee(late));
        Assert.Equal(SalesFee(early) * 10, GauntletSystem.TuningCost(early.C, early.Tick, 100));
        Assert.Equal(SalesFee(late) * 10, GauntletSystem.TuningCost(late.C, late.Tick, 100));
    }

    // ---- tuning at the Gauntlet

    [Fact]
    public void An_untuned_lab_enters_as_it_is_and_pays_nothing()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000);
        long cash = g.Me.Cash;
        GauntletRecord r = Run(g);
        Assert.Equal(1000, Crown(r, g.Me).Score);
        Assert.False(Crown(r, g.Me).Tuned);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, Count(g, EventKind.GauntletTuned));
    }

    [Fact]
    public void Tuning_lifts_every_score_and_the_lab_pays_for_it_at_the_Gauntlet()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1, rivals: false);
        Entrant(g, g.Me, 1000, inSector: true);
        g.Do(Command.SetGauntletTuning(g.Id, 200));
        long cash = g.Me.Cash;
        long bill = GauntletSystem.TuningCost(g.C, g.Tick, 200);
        Assert.True(bill > 0);
        GauntletRecord r = Run(g);
        Assert.Equal(1100, Crown(r, g.Me).Score);
        Assert.True(Crown(r, g.Me).Tuned);
        GauntletEntry bracket = r.Entries.Single(e => e.Bracket == Sector);
        Assert.True(bracket.Tuned);
        Assert.Equal(cash - bill, g.Me.Cash);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletTuned);
        Assert.Equal(1, e.Pick);
        Assert.Equal(bill, e.Amount);
    }

    [Fact]
    public void Tuning_can_turn_a_loss_into_a_crown()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1);
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000);
        Entrant(g, rival, 1050);
        Assert.Equal(rival.Id, Run(g).CrownLabId);

        g.Do(Command.SetGauntletTuning(g.Id, 300));
        GauntletRecord r = Run(g);
        Assert.Equal(g.Id, r.CrownLabId);
        Assert.Equal(1150, Crown(r, g.Me).Score);
        Assert.Equal(1, Crown(r, g.Me).Rank);
    }

    [Fact]
    public void A_lab_that_cannot_afford_the_prep_goes_in_untuned_and_keeps_its_cash()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        g.Me.Cash = GauntletSystem.TuningCost(g.C, g.Tick, 300) - 1;
        long cash = g.Me.Cash;
        GauntletRecord r = Run(g);
        Assert.Equal(1000, Crown(r, g.Me).Score);
        Assert.False(Crown(r, g.Me).Tuned);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, g.State.Events.Single(e => e.Kind == EventKind.GauntletTuned).Pick);
    }

    [Fact]
    public void A_lab_with_exactly_enough_cash_can_tune()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        long bill = GauntletSystem.TuningCost(g.C, g.Tick, 300);
        g.Me.Cash = bill;
        Run(g);
        Assert.Equal(0, g.Me.Cash);
        Assert.Equal(1, g.State.Events.Single(e => e.Kind == EventKind.GauntletTuned).Pick);
    }

    [Fact]
    public void A_bankrupt_lab_does_not_tune()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        g.Me.Bankrupt = true;
        long cash = g.Me.Cash;
        GauntletRecord r = Run(g);
        Assert.False(Crown(r, g.Me).Tuned);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, Count(g, EventKind.GauntletTuned));
    }

    [Fact]
    public void Tuning_stays_set_from_year_to_year_until_the_player_changes_it()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1, rivals: false);
        Entrant(g, g.Me, 1000);
        g.Do(Command.SetGauntletTuning(g.Id, 100));
        Run(g);
        Run(g);
        Assert.Equal(100, g.Me.GauntletTuning);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.GauntletTuned && e.Pick == 1));
        Assert.All(g.State.Gauntlets, r => Assert.True(Crown(r, g.Me).Tuned));
    }

    [Fact]
    public void A_tuning_lift_of_zero_share_is_paid_for_but_changes_no_score()
    {
        Game g = Stage(c => c.Gauntlet.TuningScoreSharePermille = 0, rivals: false);
        Entrant(g, g.Me, 1000);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        long cash = g.Me.Cash;
        GauntletRecord r = Run(g);
        Assert.Equal(1000, Crown(r, g.Me).Score);
        Assert.False(Crown(r, g.Me).Tuned);
        Assert.True(g.Me.Cash < cash);
    }

    // ---- exposure

    [Fact]
    public void Exposure_chance_grows_with_tuning_and_with_each_feuding_rival_and_never_passes_950()
    {
        Game g = Stage();
        g.Me.GauntletTuning = 100;
        Assert.Equal(200, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
        g.Me.GauntletTuning = 0;
        Assert.Equal(0, GauntletSystem.ExposeChance(g.C, g.State, g.Me));

        g.Me.GauntletTuning = 100;
        g.Rival("openaye").FeelingToPlayer = -1000;
        Assert.Equal(300, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
        g.Rival("anthoptics").FeelingToPlayer = -1000;
        g.Rival("xay").FeelingToPlayer = -1000;
        Assert.Equal(500, GauntletSystem.ExposeChance(g.C, g.State, g.Me));

        g.Me.GauntletTuning = 300;
        g.C.Gauntlet.ExposeChancePerTuningPermille = 5000;
        Assert.Equal(950, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_bankrupt_or_absorbed_rival_does_not_add_to_the_exposure_chance()
    {
        Game g = Stage();
        g.Me.GauntletTuning = 100;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = -1000;
        Assert.Equal(300, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
        rival.Bankrupt = true;
        Assert.Equal(200, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
        rival.Bankrupt = false;
        rival.AbsorbedByLabId = g.Rival("xay").Id;
        Assert.Equal(200, GauntletSystem.ExposeChance(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_rival_lab_has_no_feuds_of_its_own_so_only_tuning_counts_for_it()
    {
        Game g = Stage();
        LabState rival = g.Rival("openaye");
        rival.GauntletTuning = 100;
        g.Rival("xay").FeelingToPlayer = -1000;
        Assert.Equal(200, GauntletSystem.ExposeChance(g.C, g.State, rival));
    }

    /// <summary>Plays one Gauntlet where a tuned player beats a rival it would otherwise lose to, on this seed.</summary>
    static (Game g, GauntletRecord r, int repBefore) TunedWin(long seed, Action<GameConfig> tweak = null)
    {
        Game g = Stage(c =>
        {
            c.Gauntlet.ExposeChancePerTuningPermille = 3500;
            if (tweak != null) tweak(c);
        }, seed: seed);
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000, inSector: true);
        Entrant(g, rival, 1050);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        int rep = g.Me.Reputation;
        GauntletRecord r = Run(g);
        return (g, r, rep);
    }

    [Fact]
    public void A_tuned_win_is_sometimes_exposed_and_sometimes_stands_and_each_ends_the_right_way()
    {
        int exposed = 0, stood = 0;
        for (long seed = 1; seed <= 60; seed++)
        {
            var (g, r, before) = TunedWin(seed);
            LabState rival = g.Rival("openaye");
            GauntletEntry mine = Crown(r, g.Me);
            if (Count(g, EventKind.GauntletExposed) == 1)
            {
                exposed++;
                GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletExposed);
                Assert.Equal(1000, mine.Score);
                Assert.False(mine.Tuned);
                Assert.Equal(rival.Id, r.CrownLabId);
                Assert.Equal(2, mine.Rank);
                Assert.Equal(2, e.SubjectId);
                Assert.True(e.Amount > 0);
                Assert.False(r.Entries.Single(x => x.Bracket == Sector).Tuned);
            }
            else
            {
                stood++;
                Assert.Equal(1150, mine.Score);
                Assert.True(mine.Tuned);
                Assert.Equal(g.Id, r.CrownLabId);
                Assert.Equal(0, Count(g, EventKind.GauntletExposed));
            }
            Assert.Empty(SaveGame.Problems(g.C, g.State));
        }
        Assert.True(exposed > 0 && stood > 0, "exposed " + exposed + ", stood " + stood);
    }

    [Fact]
    public void Exposure_costs_the_configured_reputation_and_the_prep_bill_is_still_paid()
    {
        for (long seed = 1; seed <= 60; seed++)
        {
            var (g, r, before) = TunedWin(seed, c => { c.Gauntlet.CrownReputation = 0; c.Gauntlet.SafestReputation = 0; c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; c.Gauntlet.NewcomerYears = 0; });
            if (Count(g, EventKind.GauntletExposed) == 0) continue;
            Assert.Equal(1_000_000_000_000 - GauntletSystem.TuningCost(g.C, g.Tick, 300), g.Me.Cash);
            Assert.Equal(g.C.Gauntlet.ExposedReputationLoss, before - g.Me.Reputation);
            return;
        }
        Assert.Fail("no seed was exposed");
    }

    [Fact]
    public void A_tuned_lab_that_wins_nothing_is_never_exposed_even_at_the_highest_risk()
    {
        for (long seed = 1; seed <= 30; seed++)
        {
            Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 5000, seed: seed);
            LabState rival = g.Rival("openaye");
            Entrant(g, g.Me, 1000);
            Entrant(g, rival, 5000);
            g.Do(Command.SetGauntletTuning(g.Id, 300));
            GauntletRecord r = Run(g);
            Assert.Equal(0, Count(g, EventKind.GauntletExposed));
            Assert.Equal(rival.Id, r.CrownLabId);
            Assert.True(Crown(r, g.Me).Tuned);
        }
    }

    [Fact]
    public void Where_the_lift_does_not_change_the_result_but_the_lab_wins_anyway_it_can_still_be_exposed()
    {
        // A lone entrant wins its bracket whatever its score, and so has something to be exposed for.
        int exposed = 0;
        for (long seed = 1; seed <= 40; seed++)
        {
            Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 3500, rivals: false, seed: seed);
            Entrant(g, g.Me, 1000);
            g.Do(Command.SetGauntletTuning(g.Id, 300));
            Run(g);
            exposed += Count(g, EventKind.GauntletExposed);
        }
        Assert.True(exposed > 0);
    }

    [Fact]
    public void A_tuning_risk_of_zero_is_never_exposed()
    {
        for (long seed = 1; seed <= 30; seed++)
        {
            Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1, rivals: false, seed: seed);
            Entrant(g, g.Me, 1000);
            g.Do(Command.SetGauntletTuning(g.Id, 300));
            Run(g);
            Assert.Equal(0, Count(g, EventKind.GauntletExposed));
        }
    }

    [Fact]
    public void An_untuned_game_draws_no_extra_luck_so_its_results_do_not_depend_on_the_tuning_code()
    {
        Game a = Stage(rivals: true, seed: 5);
        Game b = Stage(rivals: true, seed: 5);
        foreach (Game g in new[] { a, b })
        {
            Entrant(g, g.Me, 1000, inSector: true);
            Entrant(g, g.Rival("openaye"), 900);
        }
        Run(a);
        Run(b);
        Assert.Equal(a.Sim.Digest(), b.Sim.Digest());
        Assert.Equal(0, Count(a, EventKind.GauntletTuned) + Count(a, EventKind.GauntletExposed));
    }

    [Fact]
    public void A_press_room_softens_the_blow_of_being_exposed()
    {
        for (long seed = 1; seed <= 60; seed++)
        {
            var (bare, rb, beforeBare) = TunedWin(seed);
            if (Count(bare, EventKind.GauntletExposed) == 0) continue;

            Game roomed = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 3500, seed: seed);
            roomed.Build("press-room", 6, 5);
            LabState rival = roomed.Rival("openaye");
            Entrant(roomed, roomed.Me, 1000, inSector: true);
            Entrant(roomed, rival, 1050);
            roomed.Do(Command.SetGauntletTuning(roomed.Id, 300));
            int before = roomed.Me.Reputation;
            Run(roomed);
            Assert.Equal(1, Count(roomed, EventKind.GauntletExposed));
            long lost = roomed.State.Events.Single(e => e.Kind == EventKind.GauntletExposed).Amount;
            long bareLost = bare.State.Events.Single(e => e.Kind == EventKind.GauntletExposed).Amount;
            Assert.True(lost < bareLost, lost + " vs " + bareLost);
            Assert.Equal(roomed.C.Gauntlet.ExposedReputationLoss - PressSystem.Damp(roomed.C, roomed.Me, roomed.C.Gauntlet.ExposedReputationLoss), bareLost - lost);
            Assert.True(before > 0);
            return;
        }
        Assert.Fail("no seed was exposed");
    }

    [Fact]
    public void Being_exposed_at_zero_reputation_cannot_push_it_below_zero()
    {
        for (long seed = 1; seed <= 60; seed++)
        {
            Game g = Stage(c => { c.Gauntlet.ExposeChancePerTuningPermille = 3500; c.Gauntlet.CrownReputation = 0; c.Gauntlet.SafestReputation = 0; c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; }, seed: seed);
            Entrant(g, g.Me, 1000, inSector: true);
            Entrant(g, g.Rival("openaye"), 1050);
            g.Do(Command.SetGauntletTuning(g.Id, 300));
            g.Me.Reputation = 0;
            Run(g);
            Assert.InRange(g.Me.Reputation, 0, 1000);
            if (Count(g, EventKind.GauntletExposed) == 1)
            {
                Assert.Equal(0, g.State.Events.Single(e => e.Kind == EventKind.GauntletExposed).Amount);
                return;
            }
        }
        Assert.Fail("no seed was exposed");
    }

    [Fact]
    public void A_win_by_tuning_still_sets_off_the_usual_rivalry_for_a_crown()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1);
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000);
        Entrant(g, rival, 1050);
        g.Do(Command.SetGauntletTuning(g.Id, 300));
        int before = rival.FeelingToPlayer;
        Run(g);
        Assert.Equal(g.Id, g.State.Gauntlets.Last().CrownLabId);
        Assert.Equal(before + g.C.Rivalry.Feelings.SecondPlacePermille, rival.FeelingToPlayer);
    }

    // ---- the demo round

    static GauntletRecord DemoWith(Game g, int alignment, long capability = 1000)
    {
        Entrant(g, g.Me, (int)capability, alignment);
        return Run(g);
    }

    [Fact]
    public void A_model_that_never_misbehaves_is_a_triumph_on_stage()
    {
        Game g = Stage(rivals: false);
        int before = g.Me.Reputation;
        DemoWith(g, 1000);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletDemo);
        Assert.Equal(1, e.Pick);
        Assert.Equal(g.C.Gauntlet.DemoTriumphReputation, e.Amount);
        Assert.Equal(before + g.C.Gauntlet.CrownReputation + g.C.Gauntlet.SafestReputation + g.C.Gauntlet.DemoTriumphReputation, g.Me.Reputation);
    }

    [Fact]
    public void A_model_that_misbehaves_all_the_time_becomes_a_meme()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000, 0);
        int chance = SafetySystem.IncidentChancePermille(g.C, g.Tick, g.Me.Models[0]);
        Assert.True(chance >= g.C.Gauntlet.DemoMemeFromPermille, "chance " + chance);
        int before = g.Me.Reputation;
        Run(g);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletDemo);
        Assert.Equal(0, e.Pick);
        Assert.Equal(-g.C.Gauntlet.DemoMemeReputation, e.Amount);
        Assert.Equal(before + g.C.Gauntlet.CrownReputation + g.C.Gauntlet.SafestReputation - g.C.Gauntlet.DemoMemeReputation, g.Me.Reputation);
    }

    [Fact]
    public void A_model_in_between_gets_neither_a_triumph_nor_a_meme()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000, 700);
        int chance = SafetySystem.IncidentChancePermille(g.C, g.Tick, g.Me.Models[0]);
        g.C.Gauntlet.DemoTriumphBelowPermille = chance - 1;
        g.C.Gauntlet.DemoMemeFromPermille = chance + 1;
        Run(g);
        Assert.Equal(0, Count(g, EventKind.GauntletDemo));
    }

    [Fact]
    public void The_thresholds_are_inclusive_at_both_ends()
    {
        Game steady = Stage(rivals: false);
        Entrant(steady, steady.Me, 1000, 700);
        int chance = SafetySystem.IncidentChancePermille(steady.C, steady.Tick, steady.Me.Models[0]);
        Assert.True(chance > 0);
        steady.C.Gauntlet.DemoTriumphBelowPermille = chance;
        steady.C.Gauntlet.DemoMemeFromPermille = chance + 5;
        Run(steady);
        Assert.Equal(1, steady.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Pick);

        Game flaky = Stage(rivals: false);
        Entrant(flaky, flaky.Me, 1000, 700);
        flaky.C.Gauntlet.DemoTriumphBelowPermille = chance - 5;
        flaky.C.Gauntlet.DemoMemeFromPermille = chance;
        Run(flaky);
        Assert.Equal(0, flaky.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Pick);
    }

    [Fact]
    public void With_the_demo_round_switched_off_nothing_happens_on_stage()
    {
        Game g = Stage(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; }, rivals: false);
        int before = g.Me.Reputation;
        DemoWith(g, 1000);
        Assert.Equal(0, Count(g, EventKind.GauntletDemo));
        Assert.Equal(before + g.C.Gauntlet.CrownReputation + g.C.Gauntlet.SafestReputation, g.Me.Reputation);
    }

    [Fact]
    public void Rivals_take_the_stage_too_and_a_bankrupt_lab_does_not()
    {
        Game g = Stage();
        LabState steady = g.Rival("anthoptics"), flaky = g.Rival("openaye"), gone = g.Rival("xay");
        Entrant(g, steady, 900, 1000);
        Entrant(g, flaky, 800, 0);
        Entrant(g, gone, 700, 0);
        gone.Bankrupt = true;
        int steadyBefore = steady.Reputation, flakyBefore = flaky.Reputation, goneBefore = gone.Reputation;
        Run(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GauntletDemo && e.LabId == steady.Id && e.Pick == 1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GauntletDemo && e.LabId == flaky.Id && e.Pick == 0);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.GauntletDemo && e.LabId == gone.Id);
        Assert.Equal(goneBefore, gone.Reputation);
        Assert.True(steady.Reputation > steadyBefore);
        Assert.True(flaky.Reputation < flakyBefore);
    }

    [Fact]
    public void A_meme_cannot_push_reputation_below_zero_nor_a_triumph_above_the_top()
    {
        Game low = Stage(c => { c.Gauntlet.CrownReputation = 0; c.Gauntlet.SafestReputation = 0; }, rivals: false);
        low.Me.Reputation = 0;
        Entrant(low, low.Me, 1000, 0);
        Run(low);
        Assert.Equal(0, low.Me.Reputation);
        Assert.Equal(0, low.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Amount);

        Game high = Stage(c => { c.Gauntlet.CrownReputation = 0; c.Gauntlet.SafestReputation = 0; }, rivals: false);
        high.Me.Reputation = 995;
        Entrant(high, high.Me, 1000, 1000);
        Run(high);
        Assert.Equal(1000, high.Me.Reputation);
        Assert.Equal(5, high.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Amount);
    }

    [Fact]
    public void A_press_room_takes_the_sting_out_of_a_meme()
    {
        Game bare = Stage(rivals: false);
        Game roomed = Stage(rivals: false);
        roomed.Build("press-room", 6, 5);
        foreach (Game g in new[] { bare, roomed }) Entrant(g, g.Me, 1000, 0);
        Run(bare);
        Run(roomed);
        long bareHit = bare.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Amount;
        long roomHit = roomed.State.Events.Single(e => e.Kind == EventKind.GauntletDemo).Amount;
        Assert.True(roomHit > bareHit, roomHit + " vs " + bareHit);
        Assert.Equal(-PressSystem.Damp(roomed.C, roomed.Me, roomed.C.Gauntlet.DemoMemeReputation), roomHit);
    }

    [Fact]
    public void A_better_safety_budget_turns_a_meme_into_a_triumph_a_cross_system_check()
    {
        Game g = Stage(rivals: false);
        ModelState m = Entrant(g, g.Me, 1000, 0);
        Run(g);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.GauntletDemo).Pick);
        m.Alignment = 1000;
        m.DisplayedAlignment = 1000;
        Run(g);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.GauntletDemo).Pick);
    }

    // ---- joke awards

    [Fact]
    public void Most_Confident_While_Wrong_goes_to_the_biggest_gap_between_claimed_and_true_alignment()
    {
        Game g = Stage();
        LabState a = g.Me, b = g.Rival("openaye"), c = g.Rival("xay");
        Entrant(g, a, 1000, 400, 550);
        Entrant(g, b, 900, 300, 700);
        Entrant(g, c, 800, 500, 500);
        GauntletRecord r = Run(g);
        Assert.Equal(b.Id, r.ConfidentLabId);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletAward && x.Ref == GauntletSystem.AwardConfident);
        Assert.Equal(b.Id, e.LabId);
        Assert.Equal(PressSystem.Damp(g.C, b, g.C.Gauntlet.ConfidentWrongReputation), e.Amount);
        Assert.True(e.Amount > 0);
    }

    [Fact]
    public void Nobody_is_confidently_wrong_when_every_gap_is_under_the_bar()
    {
        Game g = Stage();
        Entrant(g, g.Me, 1000, 400, 400 + g.C.Gauntlet.ConfidentWrongMinGap - 1);
        Entrant(g, g.Rival("openaye"), 900, 600, 600);
        Assert.Equal(-1, Run(g).ConfidentLabId);
        Assert.Equal(0, g.State.Events.Count(e => e.Kind == EventKind.GauntletAward && e.Ref == GauntletSystem.AwardConfident));
    }

    [Fact]
    public void A_gap_exactly_at_the_bar_wins_the_award()
    {
        Game g = Stage(rivals: false);
        Entrant(g, g.Me, 1000, 400, 400 + g.C.Gauntlet.ConfidentWrongMinGap);
        Assert.Equal(g.Id, Run(g).ConfidentLabId);
    }

    [Fact]
    public void A_tie_for_the_biggest_gap_goes_to_the_older_lab()
    {
        Game g = Stage();
        LabState older = g.State.Labs.OrderBy(l => l.Id).First(l => l.Id != g.Id && l.RivalId == "openaye");
        LabState newer = g.Rival("xay");
        LabState lo = older.Id < newer.Id ? older : newer, hi = older.Id < newer.Id ? newer : older;
        Entrant(g, lo, 800, 400, 600);
        Entrant(g, hi, 1000, 400, 600);
        Assert.Equal(lo.Id, Run(g).ConfidentLabId);
    }

    [Fact]
    public void The_confident_award_ignores_a_bankrupt_lab_and_the_penalty_is_damped_by_a_press_room()
    {
        Game g = Stage();
        LabState gone = g.Rival("openaye");
        Entrant(g, gone, 1000, 0, 900);
        gone.Bankrupt = true;
        Assert.Equal(-1, Run(g).ConfidentLabId);

        Game bare = Stage(rivals: false);
        Game roomed = Stage(rivals: false);
        roomed.Build("press-room", 6, 5);
        foreach (Game x in new[] { bare, roomed }) Entrant(x, x.Me, 1000, 400, 800);
        Run(bare);
        Run(roomed);
        long bareLoss = bare.State.Events.Single(e => e.Kind == EventKind.GauntletAward).Amount;
        long roomLoss = roomed.State.Events.Single(e => e.Kind == EventKind.GauntletAward).Amount;
        Assert.True(roomLoss < bareLoss);
        Assert.Equal(PressSystem.Damp(roomed.C, roomed.Me, roomed.C.Gauntlet.ConfidentWrongReputation), roomLoss);
    }

    [Fact]
    public void A_sandbagging_model_from_the_shady_side_wins_the_award_by_itself()
    {
        Game g = Stage(rivals: false);
        ModelState m = Entrant(g, g.Me, 1000, 300);
        m.DisplayedAlignment = 900;
        Assert.Equal(600, SafetySystem.AlignmentGap(g.C, m));
        Assert.Equal(g.Id, Run(g).ConfidentLabId);
    }

    [Fact]
    public void Best_Newcomer_goes_to_the_best_young_lab_that_did_not_win_the_crown_and_lifts_its_staff()
    {
        Game g = Stage();
        g.State.Tick = 200;
        LabState old = g.Rival("anthoptics"), young = g.Rival("openaye"), youngerStill = g.Rival("xay");
        Entrant(g, g.Me, 1000, releasedTick: 0);
        Entrant(g, old, 900, releasedTick: 0);
        Entrant(g, young, 800, releasedTick: 190);
        Entrant(g, youngerStill, 700, releasedTick: 195);
        young.Staff.Add(new StaffMember { Id = 70001, RoleId = "researcher", TraitId = g.C.StaffPeople.Traits[0].Id, Morale = 500 });
        young.Staff.Add(new StaffMember { Id = 70002, RoleId = "engineer", TraitId = g.C.StaffPeople.Traits[0].Id, Morale = 980 });
        int rep = young.Reputation;

        GauntletRecord r = Run(g);
        Assert.Equal(young.Id, r.NewcomerLabId);
        Assert.Equal(rep + g.C.Gauntlet.NewcomerReputation, young.Reputation);
        Assert.Equal(580, young.Staff[0].Morale);
        Assert.Equal(1000, young.Staff[1].Morale);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GauntletAward && x.Ref == GauntletSystem.AwardNewcomer);
        Assert.Equal(young.Id, e.LabId);
        Assert.Equal(g.C.Gauntlet.NewcomerMorale, e.Amount);
    }

    [Fact]
    public void A_young_crown_winner_is_not_also_the_newcomer_and_the_next_young_lab_gets_it()
    {
        Game g = Stage();
        LabState first = g.Rival("openaye"), second = g.Rival("xay");
        Entrant(g, first, 1000);
        Entrant(g, second, 900);
        Assert.Equal(second.Id, Run(g).NewcomerLabId);
    }

    [Fact]
    public void Nobody_is_a_newcomer_once_every_lab_has_been_selling_for_more_than_the_window()
    {
        Game g = Stage();
        g.State.Tick = g.C.Gauntlet.NewcomerYears * g.C.Calendar.WeeksPerYear + 1;
        Entrant(g, g.Me, 1000, releasedTick: 0);
        Entrant(g, g.Rival("openaye"), 900, releasedTick: 0);
        Assert.Equal(-1, Run(g).NewcomerLabId);
    }

    [Fact]
    public void A_lab_exactly_at_the_end_of_the_newcomer_window_still_counts()
    {
        Game g = Stage();
        g.State.Tick = g.C.Gauntlet.NewcomerYears * g.C.Calendar.WeeksPerYear;
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000, releasedTick: -1);
        Entrant(g, rival, 900, releasedTick: 0);
        Assert.Equal(rival.Id, Run(g).NewcomerLabId);
    }

    [Fact]
    public void A_lab_with_no_staff_can_still_be_newcomer_and_zero_window_switches_the_award_off()
    {
        Game g = Stage(rivals: true);
        LabState rival = g.Rival("openaye");
        rival.Staff.Clear();
        Entrant(g, g.Me, 1000);
        Entrant(g, rival, 900);
        Assert.Equal(rival.Id, Run(g).NewcomerLabId);

        Game off = Stage(c => c.Gauntlet.NewcomerYears = 0);
        Entrant(off, off.Me, 1000);
        Entrant(off, off.Rival("openaye"), 900);
        Assert.Equal(-1, Run(off).NewcomerLabId);
    }

    [Fact]
    public void A_bankrupt_lab_is_never_the_newcomer()
    {
        Game g = Stage();
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000);
        Entrant(g, rival, 900);
        rival.Bankrupt = true;
        Assert.Equal(-1, Run(g).NewcomerLabId);
    }

    [Fact]
    public void The_newcomer_morale_lift_helps_the_staff_resist_a_poacher_a_cross_system_check()
    {
        Game g = Stage();
        g.WithStarterLab();
        Entrant(g, g.Me, 1000);
        Entrant(g, g.Rival("openaye"), 2000);
        foreach (StaffMember s in g.Me.Staff) s.Morale = 100;
        Run(g);
        Assert.All(g.Me.Staff, s => Assert.Equal(100 + g.C.Gauntlet.NewcomerMorale, s.Morale));
        Assert.Equal(g.Id, g.State.Gauntlets.Last().NewcomerLabId);
    }

    [Fact]
    public void With_no_entrants_there_are_no_awards_no_demo_and_no_tuning_bill()
    {
        Game g = Stage();
        g.Do(Command.SetGauntletTuning(g.Id, 100));
        long cash = g.Me.Cash;
        GauntletRecord r = Run(g);
        Assert.Equal(-1, r.ConfidentLabId);
        Assert.Equal(-1, r.NewcomerLabId);
        Assert.Equal(0, Count(g, EventKind.GauntletDemo) + Count(g, EventKind.GauntletAward) + Count(g, EventKind.GauntletExposed));
        Assert.Equal(cash - GauntletSystem.TuningCost(g.C, g.Tick, 100), g.Me.Cash);
    }

    // ---- the season table

    [Fact]
    public void The_season_table_is_empty_before_the_first_Gauntlet()
    {
        Game g = Stage();
        Assert.Empty(GauntletSystem.SeasonTable(g.C, g.State));
    }

    [Fact]
    public void The_season_table_scores_crowns_and_bracket_wins_over_the_kept_years()
    {
        Game g = Stage();
        LabState rival = g.Rival("openaye");
        ModelState mine = Entrant(g, g.Me, 1000, inSector: true);
        Entrant(g, rival, 1050);
        Run(g);
        mine.Capability = 1200;
        Run(g);

        List<GauntletSystem.SeasonRow> table = GauntletSystem.SeasonTable(g.C, g.State);
        Assert.Equal(2, table.Count);
        Assert.Equal(g.Id, table[0].LabId);
        Assert.Equal(1, table[0].Crowns);
        Assert.Equal(2, table[0].BracketWins);
        Assert.Equal(5 + 2 * 2, table[0].Points);
        Assert.Equal(rival.Id, table[1].LabId);
        Assert.Equal(1, table[1].Crowns);
        Assert.Equal(0, table[1].BracketWins);
        Assert.Equal(5, table[1].Points);
    }

    [Fact]
    public void Equal_points_are_split_by_crowns_then_by_the_older_lab()
    {
        Game g = Stage(c => { c.Gauntlet.SeasonCrownPoints = 4; c.Gauntlet.SeasonBracketPoints = 2; });
        LabState p = g.Me, q = g.Rival("openaye"), r = g.Rival("xay");
        Entrant(g, p, 100, inSector: true);
        ModelState mq = Entrant(g, q, 900);
        Entrant(g, r, 800);
        Run(g);
        mq.Capability = 700;
        Run(g);

        List<GauntletSystem.SeasonRow> table = GauntletSystem.SeasonTable(g.C, g.State);
        Assert.Equal(new[] { 4, 4, 4 }, table.Select(x => x.Points).ToArray());
        Assert.Equal(new[] { Math.Min(q.Id, r.Id), Math.Max(q.Id, r.Id), p.Id }, table.Select(x => x.LabId).ToArray());
        Assert.Equal(new[] { 1, 1, 0 }, table.Select(x => x.Crowns).ToArray());
    }

    [Fact]
    public void A_lab_that_never_wins_is_on_the_table_with_zero_points()
    {
        Game g = Stage();
        Entrant(g, g.Me, 1000);
        Entrant(g, g.Rival("openaye"), 50);
        List<GauntletSystem.SeasonRow> table = null;
        Run(g);
        table = GauntletSystem.SeasonTable(g.C, g.State);
        Assert.Equal(0, table.Last().Points);
        Assert.Equal(0, table.Last().Crowns);
    }

    [Fact]
    public void Old_years_that_have_left_the_history_leave_the_season_table_too()
    {
        Game g = Stage(c => c.Gauntlet.HistoryYears = 2, rivals: false);
        ModelState m = Entrant(g, g.Me, 1000);
        Run(g);
        Run(g);
        Assert.Equal(2, GauntletSystem.SeasonTable(g.C, g.State)[0].Crowns);
        Run(g);
        Assert.Equal(2, g.State.Gauntlets.Count);
        Assert.Equal(2, GauntletSystem.SeasonTable(g.C, g.State)[0].Crowns);
        Assert.True(m.Capability > 0);
    }

    // ---- save and load

    [Fact]
    public void A_tuned_game_with_awards_saves_and_loads_to_the_same_digest()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1);
        Entrant(g, g.Me, 1000, 400, 700, inSector: true);
        Entrant(g, g.Rival("openaye"), 900, releasedTick: 0);
        g.Do(Command.SetGauntletTuning(g.Id, 250));
        GauntletRecord r = Run(g);
        Assert.Equal(g.Id, r.ConfidentLabId);
        Assert.True(Crown(r, g.Me).Tuned);

        string digest = g.Sim.Digest();
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(digest, again.Digest());
        Assert.Equal(250, again.Player.GauntletTuning);
        GauntletRecord loaded = again.State.Gauntlets.Last();
        Assert.Equal(r.ConfidentLabId, loaded.ConfidentLabId);
        Assert.Equal(r.NewcomerLabId, loaded.NewcomerLabId);
        Assert.True(loaded.Entries.Single(e => e.Bracket == GauntletSystem.CrownBracket && e.LabId == g.Id).Tuned);
    }

    [Fact]
    public void A_schema_25_save_loads_with_no_tuning_no_awards_and_no_lifted_entries_and_plays_on()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1);
        Entrant(g, g.Me, 1000, 400, 700);
        g.Do(Command.SetGauntletTuning(g.Id, 100));
        Run(g);
        string old = SaveSchemaPretend.At(g.C, 25, g.Sim.Save());
        Assert.DoesNotContain("gauntletTuning", old);
        Assert.DoesNotContain("confidentLabId", old);
        Assert.DoesNotContain("newcomerLabId", old);
        Assert.DoesNotContain("\"tuned\"", old);

        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.True(GameState.CurrentSchema >= 26);
        Assert.Equal(0, loaded.Player.GauntletTuning);
        GauntletRecord record = loaded.State.Gauntlets.Last();
        Assert.Equal(-1, record.ConfidentLabId);
        Assert.Equal(-1, record.NewcomerLabId);
        Assert.All(record.Entries, e => Assert.False(e.Tuned));
        loaded.StepMany(loaded.Config.Calendar.WeeksPerYear);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }

    [Fact]
    public void A_save_with_a_tuning_outside_the_allowed_range_is_flagged()
    {
        Game g = Stage(rivals: false);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.GauntletTuning = 301;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("Gauntlet tuning"));
        g.Me.GauntletTuning = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("Gauntlet tuning"));
        g.Me.GauntletTuning = 300;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    // ---- through the real calendar

    [Fact]
    public void A_year_of_play_with_tuning_pays_the_bill_at_year_end_and_wins_the_bracket()
    {
        Game g = Game.Solo(seed: 3, tweak: c => c.Gauntlet.ExposeChancePerTuningPermille = 1).WithStarterLab(gpus: 8);
        ModelState mini = g.Train("mini", 300);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");
        g.Do(Command.SetGauntletTuning(g.Id, 200));
        g.Until(() => g.State.Gauntlets.Count == 1, 60, "the first Gauntlet");

        GameEvent paid = g.State.Events.Single(e => e.Kind == EventKind.GauntletTuned);
        Assert.Equal(1, paid.Pick);
        Assert.Equal(EraRules.HireFee(g.C, g.C.Role("sales"), paid.Tick) * 20, paid.Amount);
        GauntletRecord r = g.State.Gauntlets[0];
        Assert.Equal(g.Id, r.CrownLabId);
        Assert.True(Crown(r, g.Me).Tuned);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Long_runs_with_a_tuning_set_stay_valid_and_save_cleanly()
    {
        Game g = Game.New(seed: 4);
        g.WithStarterLab();
        g.Do(Command.SetGauntletTuning(g.Id, 150));
        g.Autopilot(3 * g.C.Calendar.WeeksPerYear);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.InRange(g.Me.Reputation, 0, 1000);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    // ---- reading it

    [Fact]
    public void Every_new_gauntlet_event_reads_as_a_sentence()
    {
        Game g = Stage(c => c.Gauntlet.ExposeChancePerTuningPermille = 1, rivals: false);
        Entrant(g, g.Me, 1000, 0, 700);
        g.Do(Command.SetGauntletTuning(g.Id, 100));
        Run(g);
        g.Me.Cash = 0;
        Run(g);
        g.State.Tick = 5;
        Entrant(g, g.Me, 1000, 1000);
        var events = new List<GameEvent>(g.State.Events);
        events.Add(new GameEvent { Kind = EventKind.GauntletExposed, LabId = g.Id, SubjectId = 2, Amount = 80, Ref = "" });
        events.Add(new GameEvent { Kind = EventKind.GauntletAward, LabId = g.Id, Amount = 80, Ref = GauntletSystem.AwardNewcomer });
        foreach (EventKind kind in new[] { EventKind.GauntletTuned, EventKind.GauntletExposed, EventKind.GauntletDemo, EventKind.GauntletAward })
        {
            Assert.Contains(events, e => e.Kind == kind);
            foreach (GameEvent e in events.Where(x => x.Kind == kind))
            {
                string text = EventText.Describe(g.C, g.State, e);
                Assert.False(string.IsNullOrWhiteSpace(text), kind.ToString());
                Assert.DoesNotContain("{", text);
            }
        }
        Assert.Contains(events, e => e.Kind == EventKind.GauntletTuned && e.Pick == 0);
        Assert.Contains(events, e => e.Kind == EventKind.GauntletTuned && e.Pick == 1);
        Assert.Contains("Most Confident While Wrong", EventText.Describe(g.C, g.State, events.First(e => e.Kind == EventKind.GauntletAward && e.Ref == GauntletSystem.AwardConfident)));
        Assert.Contains("Best Newcomer", EventText.Describe(g.C, g.State, events.First(e => e.Kind == EventKind.GauntletAward && e.Ref == GauntletSystem.AwardNewcomer)));
    }
}
