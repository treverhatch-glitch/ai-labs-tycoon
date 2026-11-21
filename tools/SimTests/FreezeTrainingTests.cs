using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FreezeTrainingTests
{
    [Fact]
    public void Frozen_home_bloc_refuses_new_training()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.PermitFrozen);
    }

    [Fact]
    public void Training_starts_when_home_is_not_frozen()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        Assert.Single(g.Me.Models, m => m.Status == ModelStatus.Training);
    }

    [Fact]
    public void Release_model_still_works_under_home_freeze()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        Assert.Equal(ModelStatus.Released, model.Status);
    }

    [Fact]
    public void After_freeze_lifted_training_resumes()
    {
        Game g = Game.Solo().WithStarterLab();
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.PermitFrozen);
        home.Level = BlocStanding.Audit;
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        Assert.NotEmpty(g.Me.Models);
    }

    [Fact]
    public void Bankrupt_lab_hits_bankrupt_before_freeze_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bankrupt = true;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.LabBankrupt);
    }

    [Fact]
    public void Line_locked_still_checked_after_freeze_passes()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.StartTraining(g.Id, "flagship", 100), CommandError.LineLocked);
    }

    [Fact]
    public void Foreign_site_freeze_does_not_block_home_training()
    {
        Game g = WorldTests.Rich("punchline");
        int hall = g.Do(Command.BuyLand(g.Id, "china-guizhou")).CreatedId;
        g.Me.Bloc(g.C.World.Site("china-guizhou").BlocId).Level = BlocStanding.Freeze;
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        Assert.Single(g.Me.Models, m => m.Status == ModelStatus.Training);
        Assert.True(hall > 0);
    }

    [Fact]
    public void Freeze_gate_survives_save_load()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Refused(Command.StartTraining(loaded.Id, "mini", 100), CommandError.PermitFrozen);
    }

    [Fact]
    public void Rent_gpus_still_blocked_alongside_training_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.BuyGpus(g.Id, 4), CommandError.PermitFrozen);
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.PermitFrozen);
    }
}