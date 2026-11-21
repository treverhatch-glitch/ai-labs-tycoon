using System.Linq;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Home-bloc Freeze stalls in-progress training (Q41); new runs remain gated by Q37.</summary>
public class FreezeTrainingStallTests
{
    static ModelState TrainingMini(Game g, int alignment = 100)
    {
        g.Do(Command.StartTraining(g.Id, "mini", alignment));
        return g.Me.Models.Single(m => m.Status == ModelStatus.Training);
    }

    [Fact]
    public void Freeze_stalls_progress_on_active_run()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        ModelState model = TrainingMini(g);
        long before = model.ProgressCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(before, model.ProgressCompute);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void Progress_resumes_after_freeze_lifted()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        ModelState model = TrainingMini(g);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Weeks(2);
        long stalled = model.ProgressCompute;
        home.Level = BlocStanding.Audit;
        g.Weeks(1);
        Assert.True(model.ProgressCompute > stalled);
    }

    [Fact]
    public void Many_frozen_weeks_leave_run_unfinished()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        ModelState model = TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(12);
        Assert.Equal(ModelStatus.Training, model.Status);
        Assert.True(model.ProgressCompute < model.RequiredCompute);
    }

    [Fact]
    public void Freeze_stalls_alignment_compute_too()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        ModelState model = TrainingMini(g, 500);
        long before = model.AlignmentCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(before, model.AlignmentCompute);
    }

    [Fact]
    public void Unfrozen_run_finishes_eventually()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.UntilTrained(model);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void Freeze_mid_run_then_train_completes_after_lift()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        g.Weeks(2);
        home.Level = BlocStanding.Freeze;
        g.Weeks(4);
        home.Level = BlocStanding.Hearing;
        g.UntilTrained(model);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void Last_week_still_records_compute_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.True(g.Me.LastWeek.Compute > 0);
        Assert.True(g.Me.LastWeek.TrainingCompute > 0);
    }

    [Fact]
    public void Two_concurrent_runs_both_stall()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        ModelState a = TrainingMini(g);
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        ModelState b = g.Me.Models.Last(m => m.Status == ModelStatus.Training);
        long pa = a.ProgressCompute;
        long pb = b.ProgressCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(pa, a.ProgressCompute);
        Assert.Equal(pb, b.ProgressCompute);
    }

    [Fact]
    public void Foreign_bloc_freeze_does_not_stall_home_training()
    {
        Game g = WorldTests.Rich("punchline");
        g.WithStarterLab(gpus: 32);
        ModelState model = TrainingMini(g);
        long before = model.ProgressCompute;
        string foreign = g.C.World.Site("china-guizhou").BlocId;
        g.Me.Bloc(foreign).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.True(model.ProgressCompute > before);
    }

    [Fact]
    public void Stall_behavior_survives_save_load()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        ModelState model = TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        long before = model.ProgressCompute;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState model2 = loaded.Me.Models.Single(m => m.Id == model.Id);
        loaded.Weeks(1);
        Assert.Equal(before, model2.ProgressCompute);
    }

    [Fact]
    public void Estimator_unchanged_by_freeze_flag_alone()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        int weeks = TrainingSystem.EstimateWeeks(g.C, g.State, g.Me, g.C.Line("mini"), 100);
        Assert.True(weeks > 0);
    }

    [Fact]
    public void Research_stalls_under_home_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.Hire(g.Id, "researcher", 2));
        long before = g.Me.ResearchPoints;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Finish_event_not_emitted_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        ModelState model = TrainingMini(g);
        model.ProgressCompute = model.RequiredCompute - 1;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void Freeze_then_one_week_unfrozen_can_finish()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        ModelState model = TrainingMini(g);
        model.ProgressCompute = model.RequiredCompute - 1;
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Weeks(1);
        home.Level = BlocStanding.Letter;
        g.Weeks(1);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void Serving_compute_still_computed_when_frozen_with_run()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.True(g.Me.LastWeek.ServingCompute >= 0);
    }

    [Fact]
    public void Speed_permille_unchanged_on_frozen_week()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Player_frozen_does_not_stall_rival_at_same_home_bloc()
    {
        Game g = Game.Solo();
        g.WithStarterLab(gpus: 16);
        int rivalId = -1;
        for (int i = 0; i < g.State.Labs.Count; i++)
            if (g.State.Labs[i].Id != g.Me.Id) { rivalId = g.State.Labs[i].Id; break; }
        if (rivalId < 0) return;
        LabState rival = g.State.Lab(rivalId);
        if (rival.HomeBlocId != g.Me.HomeBlocId) return;
        rival.Bloc(rival.HomeBlocId).Level = 0;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Do(Command.StartTraining(rivalId, "mini", 100));
        ModelState rm = rival.Models.Single(m => m.Status == ModelStatus.Training);
        long before = rm.ProgressCompute;
        g.Weeks(1);
        Assert.True(rm.ProgressCompute > before);
    }

    [Fact]
    public void Training_share_unchanged_while_stalled()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        int share = g.Me.TrainingSharePermille;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(share, g.Me.TrainingSharePermille);
    }

    [Fact]
    public void High_alignment_share_stalls_progress_same_as_low()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        ModelState model = TrainingMini(g, 900);
        long before = model.ProgressCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(before, model.ProgressCompute);
    }

    [Fact]
    public void Capabilities_not_assigned_until_finish_after_thaw()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState model = TrainingMini(g);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(3);
        Assert.Equal(0, model.Capability);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Letter;
        g.UntilTrained(model);
        Assert.True(model.Capability > 0);
    }

    [Fact]
    public void Week_counter_advances_while_training_stalled()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        int tick = g.State.Tick;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        Assert.Equal(tick + 2, g.State.Tick);
    }

    [Fact]
    public void No_extra_models_spawn_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        int count = g.Me.Models.Count;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(4);
        Assert.Equal(count, g.Me.Models.Count);
    }

    [Fact]
    public void Deterministic_stall_across_reload_mid_freeze()
    {
        Game g = Game.Solo(seed: 42).WithStarterLab(gpus: 16);
        ModelState model = TrainingMini(g);
        g.Weeks(1);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        string save = g.Sim.Save();
        var a = new Game(Simulation.Load(g.C, save));
        var b = new Game(Simulation.Load(g.C, save));
        a.Weeks(1);
        b.Weeks(1);
        ModelState ma = a.Me.Models.First(m => m.Id == model.Id);
        ModelState mb = b.Me.Models.First(m => m.Id == model.Id);
        Assert.Equal(ma.ProgressCompute, mb.ProgressCompute);
    }

    [Fact]
    public void Active_run_count_unchanged_while_stalled()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        TrainingMini(g);
        int runs = LabQueries.ActiveRuns(g.Me);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(3);
        Assert.Equal(runs, LabQueries.ActiveRuns(g.Me));
    }

    [Fact]
    public void Required_compute_target_unchanged_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        ModelState model = TrainingMini(g);
        long required = model.RequiredCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(5);
        Assert.Equal(required, model.RequiredCompute);
        Assert.Equal(ModelStatus.Training, model.Status);
    }
}