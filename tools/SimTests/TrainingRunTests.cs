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

/// <summary>How a run is built and checkpointed: experimental designs reach higher and can fail; checkpoints decide what a failure costs.</summary>
public class TrainingRunTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static void Sure(GameConfig c, int failure = 1000, int loss = 500)
    {
        c.Runs.Approaches.First(a => a.IdValue == "experimental").FailurePermillePerWeek = failure;
        c.Runs.Checkpoints.First(k => k.IdValue == "rare").LossPermille = loss;
    }

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

    // ---- the data

    [Fact]
    public void The_shipped_numbers_validate_and_the_defaults_change_nothing()
    {
        GameConfig c = TestData.Config();
        ApproachConfig standard = c.Runs.Approach(c.Runs.DefaultApproachId);
        CheckpointConfig rare = c.Runs.Checkpoint(c.Runs.DefaultCheckpointId);
        Assert.Equal(0, standard.CapabilityPermille);
        Assert.Equal(0, standard.FailurePermillePerWeek);
        Assert.Equal(0, rare.WeeklyCostPermille);
        ApproachConfig experimental = c.Runs.Approach("experimental");
        Assert.True(experimental.CapabilityPermille > 0 && experimental.FailurePermillePerWeek > 0, "a variant reaches higher and fails");
        CheckpointConfig often = c.Runs.Checkpoint("often");
        Assert.True(often.LossPermille < rare.LossPermille && often.WeeklyCostPermille > 0, "checkpointing often loses less and costs money");
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_run_data()
    {
        AssertProblem(Problems(c => c.Runs.DefaultApproachId = "nope"), "runs.defaultApproachId");
        AssertProblem(Problems(c => c.Runs.DefaultCheckpointId = "nope"), "runs.defaultCheckpointId");
        AssertProblem(Problems(c => c.Runs.Approach("standard").CapabilityPermille = 10), "default approach");
        AssertProblem(Problems(c => c.Runs.Approach("standard").FailurePermillePerWeek = 10), "default approach");
        AssertProblem(Problems(c => c.Runs.Checkpoint("rare").WeeklyCostPermille = 10), "must be free");
        AssertProblem(Problems(c => c.Runs.Approach("experimental").FailurePermillePerWeek = 1001), "failurePermillePerWeek");
        AssertProblem(Problems(c => c.Runs.Approach("experimental").CapabilityPermille = -1000), "capabilityPermille");
        AssertProblem(Problems(c => c.Runs.Approach("experimental").Name = ""), "has no name");
        AssertProblem(Problems(c => c.Runs.Approach("experimental").Text = ""), "has no text");
        AssertProblem(Problems(c => c.Runs.Checkpoint("often").LossPermille = 1001), "lossPermille");
        AssertProblem(Problems(c => c.Runs.Checkpoint("often").WeeklyCostPermille = -1), "weeklyCostPermille");
        AssertProblem(Problems(c => c.Runs.Checkpoint("often").Name = ""), "has no name");
        AssertProblem(Problems(c => c.Runs.Checkpoint("often").Text = ""), "has no text");
        AssertProblem(Problems(c => c.Runs.Approaches.Add(new ApproachConfig { IdValue = "standard", Name = "x", Text = "x" })), "runs.approaches");
        AssertProblem(Problems(c => c.Runs = null), "runs is missing");
    }

    // ---- choosing

    [Fact]
    public void An_approach_and_a_checkpoint_habit_can_be_set_while_a_run_trains_and_are_logged()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        Assert.Equal("", m.Approach);
        Assert.Equal("", m.Checkpoints);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "often"));
        Assert.Equal("experimental", m.Approach);
        Assert.Equal("often", m.Checkpoints);
        List<GameEvent> set = g.State.Events.Where(e => e.Kind == EventKind.RunSetUp).ToList();
        Assert.Equal(2, set.Count);
        Assert.Equal("approach|experimental", set[0].Ref);
        Assert.Equal("checkpoints|often", set[1].Ref);
        Assert.Contains("Your own variant", EventText.Describe(g.C, g.State, set[0]));
        Assert.Contains("Checkpoint every few days", EventText.Describe(g.C, g.State, set[1]));
    }

    [Fact]
    public void Choosing_the_default_again_stores_nothing_and_logs_nothing()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "standard"));
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "rare"));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RunSetUp);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        g.Do(Command.SetRunApproach(g.Id, m.Id, "standard"));
        Assert.Equal("", m.Approach);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.RunSetUp));
        Assert.Contains("Known architecture", EventText.Describe(g.C, g.State, g.State.Events.Last(e => e.Kind == EventKind.RunSetUp)));
    }

    [Fact]
    public void Bad_orders_are_refused_and_change_nothing()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Refused(Command.SetRunApproach(g.Id, 9999, "experimental"), CommandError.UnknownId);
        g.Refused(Command.SetRunApproach(g.Id, m.Id, "vibes"), CommandError.UnknownId);
        g.Refused(Command.SetCheckpoints(g.Id, 9999, "often"), CommandError.UnknownId);
        g.Refused(Command.SetCheckpoints(g.Id, m.Id, "never"), CommandError.UnknownId);
        g.UntilTrained(m);
        g.Refused(Command.SetRunApproach(g.Id, m.Id, "experimental"), CommandError.WrongModelStatus);
        g.Refused(Command.SetCheckpoints(g.Id, m.Id, "often"), CommandError.WrongModelStatus);
    }

    // ---- reaching higher

    [Fact]
    public void An_experimental_run_finishes_with_a_higher_capability_by_its_share()
    {
        Game g = Lab(c => Sure(c, 0));
        Game twin = Lab(c => Sure(c, 0));
        ModelState m = g.Train("mini", 600);
        ModelState t = twin.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        g.UntilTrained(m);
        twin.UntilTrained(t);
        int plain = t.Capability;
        Assert.Equal(plain + (int)MathI.MulPermille(plain, g.C.Runs.Approach("experimental").CapabilityPermille), m.Capability);
        Assert.Equal(m.CapBase + m.CapTech + m.CapCarryover + m.CapLuck, m.Capability);
    }

    // ---- failing

    [Fact]
    public void A_failure_throws_away_the_share_the_checkpoint_habit_loses_and_is_logged()
    {
        Game g = Lab(c => Sure(c, 1000, 500));
        ModelState m = g.Train("mini", 600);
        g.Weeks(3);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        long before = m.ProgressCompute;
        long alignBefore = m.AlignmentCompute;
        Assert.True(before > 0 && alignBefore > 0);
        long expected = TrainingRuns.LossIfFailed(g.C, m);
        Assert.Equal(MathI.MulPermille(before, 500), expected);
        g.Weeks(1);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.TrainingFailed);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Equal(expected, e.Amount);
        Assert.True(m.ProgressCompute < before, "the run should have lost ground overall this week");
        Assert.True(m.AlignmentCompute < alignBefore);
        Assert.Contains("diverged", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Checkpointing_often_loses_less_than_checkpointing_now_and_then()
    {
        Game g = Lab(c => Sure(c, 1000, 500));
        Game twin = Lab(c => Sure(c, 1000, 500));
        ModelState m = g.Train("mini", 600);
        ModelState t = twin.Train("mini", 600);
        g.Weeks(3);
        twin.Weeks(3);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        twin.Do(Command.SetRunApproach(twin.Id, t.Id, "experimental"));
        twin.Do(Command.SetCheckpoints(twin.Id, t.Id, "often"));
        g.Weeks(1);
        twin.Weeks(1);
        long lostRare = g.State.Events.Last(x => x.Kind == EventKind.TrainingFailed).Amount;
        long lostOften = twin.State.Events.Last(x => x.Kind == EventKind.TrainingFailed).Amount;
        Assert.True(lostOften < lostRare);
        Assert.True(t.ProgressCompute > m.ProgressCompute);
    }

    [Fact]
    public void A_failure_with_nothing_to_lose_is_not_a_failure()
    {
        Game g = Lab(c => Sure(c, 1000, 500));
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        Assert.Equal(0, m.ProgressCompute);
        g.Weeks(1);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.TrainingFailed);
    }

    [Fact]
    public void The_chance_is_a_hash_of_the_week_so_failures_come_and_go()
    {
        Game g = Lab(c => Sure(c, 300, 100));
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        int fails = 0;
        int weeks = 0;
        while (m.Status == ModelStatus.Training && weeks < 400)
        {
            g.Weeks(1);
            weeks++;
        }
        fails = g.State.Events.Count(e => e.Kind == EventKind.TrainingFailed);
        Assert.True(fails >= 1, "a 30% weekly chance should fail at least once over a whole run");
        Assert.True(fails < weeks, "and not every week");
    }

    // ---- engineers

    [Fact]
    public void Engineers_take_a_share_off_the_failure_chance_up_to_a_cap()
    {
        Game g = Lab(c =>
        {
            Sure(c, 400, 500);
            c.Runs.EngineerSavingPermillePerStaff = 250;
            c.Runs.EngineerSavingCapPermille = 600;
        });
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        Assert.Equal(1, LabQueries.StaffByFunction(g.C, g.Me, RoleFunction.Engineering));
        Assert.Equal(250, TrainingRuns.EngineerSavingPermille(g.C, g.Me));
        Assert.Equal(300, TrainingRuns.FailureChancePermille(g.C, g.Me, m));
        g.Hire("engineer", 3);
        Assert.Equal(600, TrainingRuns.EngineerSavingPermille(g.C, g.Me));
        Assert.Equal(160, TrainingRuns.FailureChancePermille(g.C, g.Me, m));
    }

    [Fact]
    public void Engineers_save_nothing_on_a_run_that_cannot_fail_and_the_shipped_default_is_modest()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        Assert.Equal(0, TrainingRuns.FailureChancePermille(g.C, g.Me, m));
        Assert.True(g.C.Runs.EngineerSavingPermillePerStaff > 0);
        Assert.True(g.C.Runs.EngineerSavingCapPermille < MathI.Permille, "engineers never make a variant run safe");
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        Assert.True(TrainingRuns.FailureChancePermille(g.C, g.Me, m) > 0);
    }

    [Fact]
    public void A_lab_with_engineers_fails_fewer_weeks_than_the_same_lab_without()
    {
        int Failures(int engineers)
        {
            Game g = Lab(c =>
            {
                Sure(c, 400, 100);
                c.Runs.EngineerSavingPermillePerStaff = 300;
                c.Runs.EngineerSavingCapPermille = 900;
            });
            if (engineers > 1) g.Hire("engineer", engineers - 1);
            ModelState m = g.Train("mini", 600);
            g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
            int weeks = 0;
            while (m.Status == ModelStatus.Training && weeks < 600)
            {
                g.Weeks(1);
                weeks++;
            }
            return g.State.Events.Count(e => e.Kind == EventKind.TrainingFailed);
        }

        Assert.True(Failures(4) <= Failures(1), "more engineers should not mean more failures");
    }

    [Fact]
    public void The_validator_catches_a_bad_engineer_saving()
    {
        AssertProblem(Problems(c => c.Runs.EngineerSavingPermillePerStaff = -1), "engineerSavingPermillePerStaff");
        AssertProblem(Problems(c => c.Runs.EngineerSavingCapPermille = 1001), "engineerSavingCapPermille");
        AssertProblem(Problems(c => c.Runs.EngineerSavingCapPermille = -1), "engineerSavingCapPermille");
    }

    [Fact]
    public void The_outcome_uses_none_of_the_shared_dice()
    {
        Game g = Lab(c => Sure(c, 500, 500));
        Game twin = Lab(c => Sure(c, 500, 500));
        ModelState m = g.Train("mini", 600);
        twin.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        g.Weeks(12);
        twin.Weeks(12);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TrainingFailed);
        Assert.Equal(Json.Parse(twin.Sim.Save()).Get("rng").ToString(), Json.Parse(g.Sim.Save()).Get("rng").ToString());
    }

    // ---- what checkpointing costs

    [Fact]
    public void A_frequent_checkpoint_habit_costs_a_share_of_a_week_each_training_week()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = g.Train("mini", 600);
        twin.Train("mini", 600);
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "often"));
        long cost = TrainingRuns.WeeklyCost(g.C, g.State, g.Me, m);
        Assert.Equal(MathI.MulPermille(LabQueries.WeeklyRunningCost(g.C, g.State, g.Me), g.C.Runs.Checkpoint("often").WeeklyCostPermille), cost);
        Assert.True(cost > 0);
        g.Weeks(5);
        twin.Weeks(5);
        Assert.True(g.Me.Cash < twin.Me.Cash);
        Assert.True(g.Me.Ledger.Lifetime.Training < twin.Me.Ledger.Lifetime.Training);
    }

    [Fact]
    public void The_habit_never_spends_more_than_the_lab_has()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = g.Train("mini", 600);
        twin.Train("mini", 600);
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "often"));
        g.Me.Cash = 5;
        twin.Me.Cash = 5;
        g.Weeks(1);
        twin.Weeks(1);
        Assert.True(twin.Me.Cash - g.Me.Cash <= 5, "the checkpoint bill is capped at the cash in hand");
    }

    // ---- nothing else moves

    [Fact]
    public void A_run_nobody_touched_is_exactly_what_it_was()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = g.Train("mini", 600);
        ModelState t = twin.Train("mini", 600);
        g.UntilTrained(m);
        twin.UntilTrained(t);
        Assert.Equal(g.Sim.Digest(), twin.Sim.Digest());
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.TrainingFailed || e.Kind == EventKind.RunSetUp);
    }

    [Fact]
    public void Setting_the_defaults_explicitly_changes_nothing_about_the_run()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = g.Train("mini", 600);
        ModelState t = twin.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "standard"));
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "rare"));
        g.UntilTrained(m);
        twin.UntilTrained(t);
        Assert.Equal(g.Sim.Digest(), twin.Sim.Digest());
    }

    [Fact]
    public void The_chat_has_a_failed_run_line()
    {
        Game g = Lab();
        Assert.Contains(g.C.Chat.Lines, l => l.Kind == "TrainingFailed");
    }

    // ---- saving

    [Fact]
    public void The_setup_survives_a_save_and_the_run_carries_on_the_same()
    {
        Game g = Lab(c => Sure(c, 400, 500));
        ModelState m = g.Train("mini", 600);
        g.Weeks(2);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "often"));
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal("experimental", again.Player.Model(m.Id).Approach);
        Assert.Equal("often", again.Player.Model(m.Id).Checkpoints);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_every_run_built_the_known_way()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 600);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        string old = SaveSchemaPretend.At(g.C, 39, g.Sim.Save());
        Assert.DoesNotContain("\"approach\"", old);
        Assert.DoesNotContain("\"checkpoints\"", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs.SelectMany(l => l.Models), x => { Assert.Equal("", x.Approach); Assert.Equal("", x.Checkpoints); });
    }
}
