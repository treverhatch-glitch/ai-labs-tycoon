using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Weekly training collapse dice and ScrapFailedRun (Q45).</summary>
public class TrainingFailureTests
{
    static Game Risky(long seed = 77) =>
        Game.Solo(seed, c =>
        {
            c.Training.BaseFailPermille = 850;
            c.Training.FailAfterProgressPermille = 0;
            c.Training.FailEngineerCutPermille = 0;
            c.Training.FailAlignmentCutPermille = 0;
            foreach (DataSourceConfig s in c.Training.DataSources)
                if (s.Id == "synthetic") s.WeeklyFailRiskPermille = 150;
        });

    static ModelState TrainingMini(Game g, int align = 0)
    {
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        g.Do(Command.StartTraining(g.Id, "mini", align));
        return g.Me.Models.Single(m => m.Status == ModelStatus.Training);
    }

    [Fact]
    public void WeeklyFail_risk_is_base_plus_source()
    {
        Game g = Game.Solo();
        var m = new ModelState { DataSourceId = "synthetic", AlignmentSharePermille = 0 };
        int risk = TrainingSystem.WeeklyFailPermille(g.C, g.Me, m);
        Assert.Equal(g.C.Training.BaseFailPermille + 8, risk);
    }

    [Fact]
    public void Internal_source_adds_no_extra_risk()
    {
        Game g = Game.Solo();
        var m = new ModelState { DataSourceId = "internal", AlignmentSharePermille = 0 };
        Assert.Equal(g.C.Training.BaseFailPermille, TrainingSystem.WeeklyFailPermille(g.C, g.Me, m));
    }

    [Fact]
    public void Engineers_cut_weekly_fail_risk()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 4);
        var m = new ModelState { DataSourceId = "internal", AlignmentSharePermille = 0 };
        int bare = TrainingSystem.WeeklyFailPermille(g.C, g.Me, m);
        Assert.True(bare < g.C.Training.BaseFailPermille);
    }

    [Fact]
    public void Alignment_share_cuts_fail_risk()
    {
        Game g = Game.Solo();
        var low = new ModelState { DataSourceId = "internal", AlignmentSharePermille = 0 };
        var high = new ModelState { DataSourceId = "internal", AlignmentSharePermille = 800 };
        Assert.True(TrainingSystem.WeeklyFailPermille(g.C, g.Me, high) < TrainingSystem.WeeklyFailPermille(g.C, g.Me, low));
    }

    [Fact]
    public void No_fail_roll_before_progress_threshold()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 2);
        var model = TrainingMini(g);
        g.C.Training.FailAfterProgressPermille = 950;
        g.Weeks(1);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void High_risk_run_fails_within_a_few_weeks()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
    }

    [Fact]
    public void Failure_emits_TrainingFailed_event()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TrainingFailed && e.SubjectId == model.Id);
    }

    [Fact]
    public void Failed_run_stops_gaining_progress()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
        long progress = model.ProgressCompute;
        g.Weeks(2);
        Assert.Equal(progress, model.ProgressCompute);
    }

    [Fact]
    public void Failed_run_frees_concurrent_slot()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
        Assert.Equal(0, LabQueries.ActiveRuns(g.Me));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
    }

    [Fact]
    public void ScrapFailedRun_removes_model()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
        int id = model.Id;
        g.Do(Command.ScrapFailedRun(g.Id, id));
        Assert.Null(g.Me.Model(id));
    }

    [Fact]
    public void Scrap_refuses_training_model()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState model = TrainingMini(g);
        g.Refused(Command.ScrapFailedRun(g.Id, model.Id), CommandError.WrongModelStatus);
    }

    [Fact]
    public void Scrap_refuses_trained_model()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.UntilTrained(model);
        g.Refused(Command.ScrapFailedRun(g.Id, model.Id), CommandError.WrongModelStatus);
    }

    [Fact]
    public void Scrap_unknown_model_id()
    {
        Game g = Game.Solo();
        g.Refused(Command.ScrapFailedRun(g.Id, 999), CommandError.UnknownId);
    }

    [Fact]
    public void After_scrap_new_version_can_start()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "run failing");
        g.Do(Command.ScrapFailedRun(g.Id, model.Id));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        Assert.Equal(1, LabQueries.ActiveRuns(g.Me));
    }

    [Fact]
    public void Failed_models_do_not_carry_over_capability()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState trained = TrainingMini(g);
        g.UntilTrained(trained);
        Game g2 = Risky(88);
        g2.WithStarterLab(gpus: 24);
        g2.Train("mini", 100);
        ModelState failed = g2.Me.Models.Single(m => m.Status == ModelStatus.Training);
        TrainingSystem.FailRun(g2.C, g2.State, g2.Me, failed);
        var forecast = TrainingSystem.Forecast(g2.C, g2.Me, g2.C.Line("mini"));
        Assert.Equal(0, forecast.Carryover);
    }

    [Fact]
    public void Freeze_prevents_fail_roll_without_progress()
    {
        Game g = Game.Solo(91, c =>
        {
            c.Training.BaseFailPermille = 900;
            c.Training.FailAfterProgressPermille = 500;
        });
        g.WithStarterLab(gpus: 8);
        ModelState model = TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(10);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void Zero_risk_config_never_fails()
    {
        Game g = Game.Solo(33, c =>
        {
            c.Training.BaseFailPermille = 0;
            c.Training.FailAfterProgressPermille = 0;
            foreach (DataSourceConfig s in c.Training.DataSources) s.WeeklyFailRiskPermille = 0;
        });
        g.WithStarterLab(gpus: 8);
        ModelState model = TrainingMini(g);
        g.Weeks(30);
        Assert.NotEqual(ModelStatus.Failed, model.Status);
    }

    [Fact]
    public void Same_seed_same_fail_week()
    {
        Game a = Risky(1001);
        a.WithStarterLab(gpus: 24);
        ModelState ma = TrainingMini(a);
        a.Until(() => ma.Status == ModelStatus.Failed, 12, "fail");
        int weekA = ma.StartedTick;

        Game b = Risky(1001);
        b.WithStarterLab(gpus: 24);
        ModelState mb = TrainingMini(b);
        b.Until(() => mb.Status == ModelStatus.Failed, 12, "fail");
        Assert.Equal(a.Tick, b.Tick);
        Assert.Equal(weekA, mb.StartedTick);
    }

    [Fact]
    public void EventText_mentions_collapsed_run()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        GameEvent e = g.State.Events.Last(ev => ev.Kind == EventKind.TrainingFailed);
        string line = EventText.Describe(g.C, g.State, e);
        Assert.Contains("collapsed", line);
        Assert.Contains(model.Name, line);
    }

    [Fact]
    public void Two_runs_one_can_fail_other_continues()
    {
        Game g = Game.Solo(202, c =>
        {
            c.Training.BaseFailPermille = 30;
            c.Training.FailAfterProgressPermille = 0;
            c.Training.FailEngineerCutPermille = 0;
            c.Training.FailAlignmentCutPermille = 8;
            c.Training.MaxConcurrentRuns = 2;
            foreach (DataSourceConfig s in c.Training.DataSources)
            {
                if (s.Id == "synthetic") s.WeeklyFailRiskPermille = 920;
                if (s.Id == "internal") s.WeeklyFailRiskPermille = 0;
            }
        });
        g.WithStarterLab(gpus: 48);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState risky = g.Me.Model(g.Do(Command.StartTraining(g.Id, "mini", 0)).CreatedId);
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        ModelState safe = g.Me.Model(g.Do(Command.StartTraining(g.Id, "mini", 900)).CreatedId);
        g.Weeks(16);
        Assert.Equal(ModelStatus.Failed, risky.Status);
        Assert.Equal(ModelStatus.Training, safe.Status);
    }

    [Fact]
    public void Licensed_source_lower_risk_than_synthetic()
    {
        Game g = Game.Solo();
        var syn = new ModelState { DataSourceId = "synthetic", AlignmentSharePermille = 0 };
        var lic = new ModelState { DataSourceId = "licensed", AlignmentSharePermille = 0 };
        Assert.True(TrainingSystem.WeeklyFailPermille(g.C, g.Me, syn) > TrainingSystem.WeeklyFailPermille(g.C, g.Me, lic));
    }

    [Fact]
    public void Risk_clamped_at_950()
    {
        Game g = Game.Solo(1, c =>
        {
            c.Training.BaseFailPermille = 900;
            foreach (DataSourceConfig s in c.Training.DataSources)
                if (s.Id == "synthetic") s.WeeklyFailRiskPermille = 200;
        });
        var m = new ModelState { DataSourceId = "synthetic", AlignmentSharePermille = 0 };
        Assert.Equal(950, TrainingSystem.WeeklyFailPermille(g.C, g.Me, m));
    }

    [Fact]
    public void Failure_records_progress_in_event_amount()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        GameEvent e = g.State.Events.Last(ev => ev.Kind == EventKind.TrainingFailed);
        Assert.Equal(model.ProgressCompute, e.Amount);
    }

    [Fact]
    public void Shady_jobs_still_refuse_failed_models()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        Command shady = Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, "");
        CommandResult r = g.Try(shady);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Save_round_trip_preserves_failed_status()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        string json = g.Sim.Save();
        Game loaded = new Game(Simulation.Load(g.C, json));
        ModelState again = loaded.Me.Model(model.Id);
        Assert.Equal(ModelStatus.Failed, again.Status);
        Assert.Equal(19, loaded.State.Schema);
    }

    [Fact]
    public void Migrate_16_to_17_bumps_schema()
    {
        Game g = Game.Solo();
        string json = g.Sim.Save().Replace("\"schema\":19", "\"schema\":16");
        Game loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(19, loaded.State.Schema);
    }

    [Fact]
    public void Default_config_validates_fail_fields()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Training.BaseFailPermille > 0);
        Assert.True(c.Training.DataSources.All(s => s.WeeklyFailRiskPermille >= 0));
    }

    [Fact]
    public void Internal_data_finish_still_possible_under_low_risk()
    {
        Game g = Game.Solo(12, c =>
        {
            c.Training.BaseFailPermille = 0;
            foreach (DataSourceConfig s in c.Training.DataSources) s.WeeklyFailRiskPermille = 0;
        });
        g.WithStarterLab(gpus: 24);
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void Failed_run_reserves_next_version_until_scrapped()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        int version = model.Version;
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        Assert.Equal(version + 1, LabQueries.NextVersion(g.Me, "mini"));
        g.Do(Command.ScrapFailedRun(g.Id, model.Id));
        Assert.Equal(version, LabQueries.NextVersion(g.Me, "mini"));
    }

    [Fact]
    public void High_alignment_lowers_computed_fail_risk()
    {
        Game g = Game.Solo();
        var low = new ModelState { DataSourceId = "synthetic", AlignmentSharePermille = 0 };
        var high = new ModelState { DataSourceId = "synthetic", AlignmentSharePermille = 900 };
        Assert.True(TrainingSystem.WeeklyFailPermille(g.C, g.Me, low) > TrainingSystem.WeeklyFailPermille(g.C, g.Me, high));
    }

    [Fact]
    public void Failed_run_not_counted_in_serving_mood()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Until(() => model.Status == ModelStatus.Failed, 8, "fail");
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(g.C, g.Me, model, g.Tick));
    }
}