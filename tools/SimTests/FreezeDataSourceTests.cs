using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Home freeze blocks changing the default training data source (Q44).</summary>
public class FreezeDataSourceTests
{
    [Fact]
    public void Frozen_home_refuses_set_training_data_source()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Licensed_source_still_works_when_not_frozen()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        Assert.Equal("licensed", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Default_internal_unchanged_after_refusal()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "synthetic"), CommandError.PermitFrozen);
        Assert.Equal("internal", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Thaw_allows_switch_to_synthetic()
    {
        Game g = Game.Solo().WithStarterLab();
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "synthetic"), CommandError.PermitFrozen);
        home.Level = BlocStanding.Letter;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        Assert.Equal("synthetic", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Unknown_id_checked_after_freeze_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.SetTrainingDataSource(g.Id, "dark-web"), CommandError.UnknownId);
    }

    [Fact]
    public void Empty_id_checked_after_freeze_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.SetTrainingDataSource(g.Id, ""), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_before_freeze_on_data_source()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bankrupt = true;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Gate_survives_save_load()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Refused(Command.SetTrainingDataSource(loaded.Id, "licensed"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Prior_choice_survives_save_under_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal("licensed", loaded.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Active_run_keeps_stamped_source_when_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        ModelState model = g.Me.Models[^1];
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "internal"), CommandError.PermitFrozen);
        Assert.Equal("synthetic", model.DataSourceId);
    }

    [Fact]
    public void Start_training_still_refused_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.PermitFrozen);
    }

    [Fact]
    public void Research_still_refused_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(10_000);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Set_open_weights_not_blocked_by_data_source_freeze_gate()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        Assert.True(model.OpenWeights);
    }

    [Fact]
    public void Set_training_share_not_blocked_by_this_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Do(Command.SetTrainingShare(g.Id, 400));
        Assert.Equal(400, g.Me.TrainingSharePermille);
    }

    [Fact]
    public void Letter_level_allows_data_source_change()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Letter;
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        Assert.Equal("licensed", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Foreign_bloc_freeze_does_not_block_home_source_change()
    {
        Game g = WorldTests.Rich("punchline");
        g.WithStarterLab();
        g.Me.Bloc(g.C.World.Site("china-guizhou").BlocId).Level = BlocStanding.Freeze;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        Assert.Equal("synthetic", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Two_refusals_leave_default_internal()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.PermitFrozen);
        g.Refused(Command.SetTrainingDataSource(g.Id, "synthetic"), CommandError.PermitFrozen);
        Assert.Equal("internal", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Deterministic_refusal_same_seed()
    {
        Game a = Game.Solo(seed: 3).WithStarterLab();
        Game b = Game.Solo(seed: 3).WithStarterLab();
        a.Me.Bloc(a.Me.HomeBlocId).Level = BlocStanding.Freeze;
        b.Me.Bloc(b.Me.HomeBlocId).Level = BlocStanding.Freeze;
        a.Refused(Command.SetTrainingDataSource(a.Id, "licensed"), CommandError.PermitFrozen);
        b.Refused(Command.SetTrainingDataSource(b.Id, "licensed"), CommandError.PermitFrozen);
    }

    [Fact]
    public void New_run_after_thaw_uses_updated_lab_default()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        home.Level = BlocStanding.Audit;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        Assert.Equal("synthetic", model.DataSourceId);
    }

    [Fact]
    public void Switch_back_to_internal_when_thawed()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Letter;
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        Assert.Equal("internal", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Frozen_gate_does_not_clear_training_share()
    {
        Game g = Game.Solo().WithStarterLab();
        int share = g.Me.TrainingSharePermille;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.PermitFrozen);
        Assert.Equal(share, g.Me.TrainingSharePermille);
    }

    [Fact]
    public void Week_tick_advances_on_refusal()
    {
        Game g = Game.Solo().WithStarterLab();
        int tick = g.State.Tick;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.PermitFrozen);
        g.Weeks(1);
        Assert.Equal(tick + 1, g.State.Tick);
    }

    [Fact]
    public void Validator_still_lists_all_data_sources()
    {
        Game g = Game.Solo();
        Assert.True(g.C.Training.DataSources.Count >= 3);
    }

    [Fact]
    public void Hearing_level_allows_licensed_mix()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Hearing;
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        Assert.Equal("licensed", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Rent_gpus_still_refused_under_same_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.BuyGpus(g.Id, 2), CommandError.PermitFrozen);
    }
}