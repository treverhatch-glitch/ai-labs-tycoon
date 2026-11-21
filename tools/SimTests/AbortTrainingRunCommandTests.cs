using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q160: AbortTrainingRun command.</summary>
public class AbortTrainingRunCommandTests
{
    static (Game g, ModelState m) Training()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.StartTraining(g.Id, "mini", 200));
        return (g, g.Me.Models.Last(m => m.Status == ModelStatus.Training));
    }

    [Fact]
    public void Q160_main_removes_training_model()
    {
        var (g, m) = Training();
        g.Do(Command.AbortTrainingRun(g.Id, m.Id));
        Assert.DoesNotContain(g.Me.Models, x => x.Id == m.Id);
    }

    [Fact]
    public void Q160_main_refunds_prep_permille()
    {
        var (g, m) = Training();
        long cash = g.Me.Cash;
        g.Do(Command.AbortTrainingRun(g.Id, m.Id));
        Assert.True(g.Me.Cash > cash);
    }

    [Fact]
    public void Q160_zero_refund_when_permille_zero()
    {
        var (g, m) = Training();
        g.C.Training.AbortTrainingRefundPermille = 0;
        long cash = g.Me.Cash;
        g.Do(Command.AbortTrainingRun(g.Id, m.Id));
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void Q160_bankrupt_refused()
    {
        var (g, m) = Training();
        g.Me.Bankrupt = true;
        g.Refused(Command.AbortTrainingRun(g.Id, m.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q160_save_load_model_gone()
    {
        var (g, m) = Training();
        g.Do(Command.AbortTrainingRun(g.Id, m.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.DoesNotContain(loaded.Me.Models, x => x.Status == ModelStatus.Training);
    }

    [Fact]
    public void Q160_cross_frees_concurrent_run_slot()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.StartTraining(g.Id, "mini", 200));
        g.Do(Command.StartTraining(g.Id, "mini", 200));
        g.Refused(Command.StartTraining(g.Id, "mini", 200), CommandError.TooManyRuns);
        var first = g.Me.Models.First(m => m.Status == ModelStatus.Training);
        g.Do(Command.AbortTrainingRun(g.Id, first.Id));
        Assert.True(g.Do(Command.StartTraining(g.Id, "mini", 200)).Ok);
    }

    [Fact]
    public void Q160_wrong_status_refused()
    {
        var g = Game.Solo().WithStarterLab();
        var m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Status = ModelStatus.Trained, Capability = 50 };
        g.Me.Models.Add(m);
        g.Refused(Command.AbortTrainingRun(g.Id, m.Id), CommandError.WrongModelStatus);
    }

    [Fact]
    public void Q160_frozen_refused()
    {
        var (g, m) = Training();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.AbortTrainingRun(g.Id, m.Id), CommandError.PermitFrozen);
    }
}