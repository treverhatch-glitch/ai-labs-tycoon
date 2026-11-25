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

/// <summary>The interpretability programme: a standing weekly bill and some research for a slowly closing alignment gap and fresher safety techs.</summary>
public class InterpretabilityTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static void Own(Game g, string techId)
    {
        TechConfig tech = g.C.Tech(techId);
        foreach (string need in tech.Requires) Own(g, need);
        if (!g.Me.Techs.Contains(techId)) g.Me.Techs.Add(techId);
    }

    /// <summary>A model on sale whose shown alignment sits 500 above the real one.</summary>
    static ModelState Sandbagging(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.Alignment = 400;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 900, "game-ai"));
        return m;
    }

    static int Gap(Game g, ModelState m) { return SafetySystem.AlignmentGap(g.C, m); }

    static int Incidents(Game g) { return g.State.Events.Count(e => e.Kind == EventKind.Incident); }

    // ---- nothing happens by itself

    [Fact]
    public void Nothing_runs_until_the_player_sets_a_level()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        ModelState m = Sandbagging(g);
        long training = g.Me.Ledger.Lifetime.Training;
        int gap = Gap(g, m);
        g.Weeks(20);
        Assert.Equal(0, g.Me.InterpLevel);
        Assert.Equal(gap, Gap(g, m));
        Assert.Equal(0, Interpretability.WeeklyCost(g.C, g.Me));
        Assert.Equal(0, Interpretability.ResearchCostPermille(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.GapFound);
    }

    // ---- the command and its techs

    [Fact]
    public void A_level_needs_its_tech_and_level_three_needs_the_second_one()
    {
        Game g = Lab();
        g.Refused(Command.SetInterpretability(g.Id, 1), CommandError.MissingPrerequisite);
        Assert.Equal(0, g.Me.InterpLevel);
        Own(g, "interp-v1");
        Assert.Equal(2, Interpretability.HighestAllowed(g.C, g.Me));
        g.Do(Command.SetInterpretability(g.Id, 2));
        Assert.Equal(2, g.Me.InterpLevel);
        g.Refused(Command.SetInterpretability(g.Id, 3), CommandError.MissingPrerequisite);
        Own(g, "interp-v2");
        Assert.Equal(3, Interpretability.HighestAllowed(g.C, g.Me));
        g.Do(Command.SetInterpretability(g.Id, 3));
        Assert.Equal(3, g.Me.InterpLevel);
    }

    [Fact]
    public void A_level_outside_the_range_or_the_same_level_is_refused_and_zero_turns_it_off()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        g.Refused(Command.SetInterpretability(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetInterpretability(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetInterpretability(g.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.SetInterpretability(g.Id, 1));
        g.Refused(Command.SetInterpretability(g.Id, 1), CommandError.InvalidArgument);
        g.Do(Command.SetInterpretability(g.Id, 0));
        Assert.Equal(0, g.Me.InterpLevel);
    }

    [Fact]
    public void A_rival_cannot_set_a_level()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.Techs.Add("interp-v1");
        Assert.False(g.Try(Command.SetInterpretability(rival.Id, 1)).Ok);
        Assert.Equal(0, rival.InterpLevel);
    }

    [Fact]
    public void With_no_level_techs_in_the_data_there_is_no_programme()
    {
        Game g = Lab(c => c.Interp.LevelTechs.Clear());
        Assert.Equal(0, g.C.Interp.MaxLevel);
        g.Refused(Command.SetInterpretability(g.Id, 1), CommandError.InvalidArgument);
        Assert.Equal("", Interpretability.TechFor(g.C, 1));
    }

    // ---- the weekly bill

    [Fact]
    public void Each_level_costs_the_data_s_bill_every_week_on_the_training_line()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        g.Do(Command.SetInterpretability(g.Id, 2));
        g.Weeks(1);
        Assert.Equal(10000, Interpretability.WeeklyCost(g.C, g.Me));
        Assert.Equal(10000, -g.Me.Ledger.LastWeek.Training);
        long training = g.Me.Ledger.Lifetime.Training;
        g.Weeks(9);
        Assert.Equal(training - 90000, g.Me.Ledger.Lifetime.Training);
    }

    [Fact]
    public void A_lab_with_no_cash_still_pays_and_runs_into_debt_like_any_bill()
    {
        Game g = Lab(rich: false);
        Own(g, "interp-v1");
        g.Do(Command.SetInterpretability(g.Id, 1));
        g.Me.Cash = 0;
        g.Weeks(1);
        Assert.True(g.Me.Cash < 0);
        Assert.True(g.Me.WeeksInDebt >= 1);
    }

    // ---- the gap

    [Fact]
    public void Level_one_closes_a_tenth_of_the_gap_each_week_and_level_three_three_tenths()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        ModelState m = Sandbagging(g);
        Assert.Equal(500, Gap(g, m));
        g.Do(Command.SetInterpretability(g.Id, 1));
        g.Weeks(1);
        Assert.Equal(450, Gap(g, m));
        g.Do(Command.SetInterpretability(g.Id, 3));
        g.Weeks(1);
        Assert.Equal(450 - 135, Gap(g, m));
    }

    [Fact]
    public void The_gap_closes_all_the_way_with_at_least_one_point_a_week_and_never_goes_negative()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        ModelState m = Sandbagging(g);
        g.Do(Command.SetInterpretability(g.Id, 1));
        int last = Gap(g, m);
        for (int week = 0; week < 400 && Gap(g, m) > 0; week++)
        {
            g.Weeks(1);
            Assert.True(Gap(g, m) < last);
            Assert.InRange(g.Me.Models.First(x => x.Id == m.Id).DisplayedAlignment, m.Alignment, 1000);
            last = Gap(g, m);
        }
        Assert.Equal(0, Gap(g, m));
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        g.Weeks(5);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.GapFound));
    }

    [Fact]
    public void A_one_point_gap_is_closed_in_one_week_not_left_at_a_fraction()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        ModelState m = Sandbagging(g);
        m.DisplayedAlignment = m.Alignment + 1;
        g.Do(Command.SetInterpretability(g.Id, 1));
        g.Weeks(1);
        Assert.Equal(0, Gap(g, m));
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.GapFound);
        Assert.Equal(1, e.Amount);
        Assert.Contains("read all the way through " + m.Name, EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Honest_models_and_models_not_on_sale_are_left_alone()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        ModelState honest = g.Train("mini", 0);
        g.UntilTrained(honest);
        g.Do(Command.ReleaseModel(g.Id, honest.Id, "game-ai"));
        ModelState waiting = g.Train("mini", 0);
        g.UntilTrained(waiting);
        waiting.DisplayedAlignment = waiting.Alignment + 300;
        g.Do(Command.SetInterpretability(g.Id, 2));
        int shown = honest.DisplayedAlignment;
        g.Weeks(4);
        Assert.Equal(shown, honest.DisplayedAlignment);
        Assert.Equal(waiting.Alignment + 300, waiting.DisplayedAlignment);
    }

    [Fact]
    public void Turning_it_off_stops_the_closing_and_the_bill()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        ModelState m = Sandbagging(g);
        g.Do(Command.SetInterpretability(g.Id, 1));
        g.Weeks(2);
        int gap = Gap(g, m);
        g.Do(Command.SetInterpretability(g.Id, 0));
        long training = g.Me.Ledger.Lifetime.Training;
        g.Weeks(5);
        Assert.Equal(gap, Gap(g, m));
        Assert.Equal(training, g.Me.Ledger.Lifetime.Training);
    }

    [Fact]
    public void Once_the_gap_is_below_the_sandbag_bar_a_bad_incident_is_no_longer_the_sandbagging_rung()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        ModelState m = Sandbagging(g);
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, m));
        g.Do(Command.SetInterpretability(g.Id, 3));
        for (int week = 0; week < 60 && Gap(g, m) >= g.C.Safety.SandbagGapPermille; week++) g.Weeks(1);
        Assert.True(Gap(g, m) < g.C.Safety.SandbagGapPermille);
        Assert.NotEqual(3, SafetySystem.PickIncidentRung(g.C, m));
    }

    // ---- research and stale techs

    [Fact]
    public void Each_level_takes_six_percent_of_research_and_all_three_together_take_eighteen()
    {
        Game g = Lab();
        long none = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(none > 0);
        g.Me.InterpLevel = 1;
        Assert.Equal(MathI.MulPermille(none, 940), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
        g.Me.InterpLevel = 3;
        Assert.Equal(MathI.MulPermille(none, 820), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
        g.Me.InterpLevel = 99;
        Assert.Equal(MathI.MulPermille(none, 820), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void Crunch_and_the_programme_stack_on_the_same_research_base()
    {
        Game g = Lab();
        long none = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        g.Me.CrunchWeeks = 1;
        g.Me.InterpLevel = 2;
        Assert.Equal(MathI.MulPermille(none, 1000 + g.C.StaffPeople.CrunchResearchPermille - 120), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void The_programme_takes_a_share_off_what_stale_safety_techs_have_lost()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        int now = EraSystem.StartTick(g.C, g.C.Era("upward")) + 1;
        int loss = ControlStale.Of(g.C, g.Me, now).LossPermille;
        Assert.True(loss > 0);
        g.Me.InterpLevel = 1;
        Assert.Equal((int)MathI.MulPermille(loss, 850), ControlStale.Of(g.C, g.Me, now).LossPermille);
        g.Me.InterpLevel = 3;
        Assert.Equal((int)MathI.MulPermille(loss, 550), ControlStale.Of(g.C, g.Me, now).LossPermille);
        Assert.True(ControlStale.Of(g.C, g.Me, now).BonusPermille > 0);
    }

    [Fact]
    public void A_lab_with_nothing_stale_loses_nothing_either_way()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        g.Me.InterpLevel = 3;
        Assert.Equal(0, ControlStale.Of(g.C, g.Me, EraSystem.StartTick(g.C, g.C.Era("agents")) + 1).LossPermille);
    }

    // ---- cross-system

    [Fact]
    public void A_red_team_and_the_programme_together_finish_the_gap_sooner_and_log_one_finding()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        StaffSystem.Hire(g.C, g.State, g.Me, g.C.Role("red-teamer"), 1);
        ModelState m = Sandbagging(g);
        g.Do(Command.SetInterpretability(g.Id, 2));
        g.Do(Command.RedTeamModel(g.Id, m.Id));
        Assert.True(Gap(g, m) <= 250);
        for (int week = 0; week < 60 && Gap(g, m) > 0; week++) g.Weeks(1);
        Assert.Equal(0, Gap(g, m));
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.GapFound));
    }

    [Fact]
    public void An_honest_meter_can_put_a_model_under_the_labs_own_release_gate()
    {
        Game g = Lab();
        Own(g, "interp-v2");
        ModelState m = Sandbagging(g);
        g.Do(Command.SetReleaseGate(g.Id, 800));
        Assert.False(SafetyCulture.GateBlocks(g.Me, m));
        g.Do(Command.SetInterpretability(g.Id, 3));
        g.Weeks(8);
        Assert.True(SafetyCulture.GateBlocks(g.Me, m));
    }

    [Fact]
    public void Over_a_few_years_the_programme_costs_research_and_money_but_the_lab_stays_sound()
    {
        foreach (int seed in new[] { 1, 2, 3 })
        {
            Game g = Game.New(seed);
            g.Autopilot(60);
            Own(g, "interp-v2");
            long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
            g.Do(Command.SetInterpretability(g.Id, 1));
            for (int i = 0; i < 150; i++) g.Autopilot(1);
            Assert.False(g.Me.Bankrupt);
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
            Assert.Equal(1, g.Me.InterpLevel);
        }
    }

    [Fact]
    public void Rivals_and_the_autopilot_never_run_a_programme()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.InterpLevel));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.GapFound);
    }

    // ---- save, replay, migration, data

    [Fact]
    public void A_saved_game_keeps_the_level_and_a_load_plays_on_the_same()
    {
        Game g = Lab();
        Own(g, "interp-v1");
        Sandbagging(g);
        g.Do(Command.SetInterpretability(g.Id, 2));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.InterpLevel);
        g.Weeks(12);
        for (int i = 0; i < 12; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Interp.LevelTechs = new List<string> { "state-machines" });
        g.Autopilot(60);
        Assert.True(g.Try(Command.SetInterpretability(g.Id, 1)).Ok);
        g.Autopilot(40);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_schema_45_save_loads_with_no_programme()
    {
        Game g = Lab();
        g.Weeks(4);
        string old = SaveSchemaPretend.At(g.C, 45, g.Sim.Save());
        Assert.DoesNotContain("interpLevel", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.InterpLevel);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_level_is_reported()
    {
        Game g = Lab();
        g.Me.InterpLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("interpretability"));
        g.Me.InterpLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("interpretability"));
        g.Me.InterpLevel = 3;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("interpretability"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Interp.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Interp.LevelTechs[1] = "no-such-tech";
        c.Interp.WeeklyCostPerLevel = -1;
        c.Interp.GapClosePermillePerLevel = 1001;
        c.Interp.ResearchCostPermillePerLevel = 400;
        c.Interp.StaleReliefPermillePerLevel = 400;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "levelTechs", "weeklyCostPerLevel", "gapClosePermillePerLevel", "researchCostPermillePerLevel", "staleReliefPermillePerLevel" })
            Assert.Contains(problems, p => p.Contains("interp." + key));
    }
}
