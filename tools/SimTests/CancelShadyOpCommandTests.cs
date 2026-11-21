using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q158: CancelShadyOp command.</summary>
public class CancelShadyOpCommandTests
{
    static (Game g, ShadyOp op) ActiveOp()
    {
        var g = Game.Solo();
        var model = new ModelState { Id = g.State.TakeId(), Capability = 400, Status = ModelStatus.Trained, LineId = "mini" };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""));
        return (g, g.Me.ShadyOps[0]);
    }

    [Fact]
    public void Q158_main_removes_pending_op()
    {
        var (g, op) = ActiveOp();
        g.Do(Command.CancelShadyOp(g.Id, op.Id));
        Assert.Empty(g.Me.ShadyOps);
    }

    [Fact]
    public void Q158_main_adds_config_heat_penalty()
    {
        var (g, op) = ActiveOp();
        int heat = g.Me.Heat;
        g.Do(Command.CancelShadyOp(g.Id, op.Id));
        Assert.Equal(heat + g.C.Shady.CancelShadyOpHeatPenalty, g.Me.Heat);
    }

    [Fact]
    public void Q158_zero_refused_unknown_op()
    {
        var g = Game.Solo();
        g.Refused(Command.CancelShadyOp(g.Id, 999), CommandError.UnknownId);
    }

    [Fact]
    public void Q158_max_zero_penalty_still_cancels()
    {
        var (g, op) = ActiveOp();
        g.C.Shady.CancelShadyOpHeatPenalty = 0;
        int heat = g.Me.Heat;
        g.Do(Command.CancelShadyOp(g.Id, op.Id));
        Assert.Equal(heat, g.Me.Heat);
        Assert.Empty(g.Me.ShadyOps);
    }

    [Fact]
    public void Q158_bankrupt_refused()
    {
        var (g, op) = ActiveOp();
        g.Me.Bankrupt = true;
        g.Refused(Command.CancelShadyOp(g.Id, op.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q158_save_load_op_gone()
    {
        var (g, op) = ActiveOp();
        g.Do(Command.CancelShadyOp(g.Id, op.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Empty(loaded.Me.ShadyOps);
    }

    [Fact]
    public void Q158_cross_new_op_after_cancel()
    {
        var (g, op) = ActiveOp();
        g.Do(Command.CancelShadyOp(g.Id, op.Id));
        var model = g.Me.Models[0];
        g.Do(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""));
        Assert.Single(g.Me.ShadyOps);
    }

    [Fact]
    public void Q158_refused_after_resolve_tick()
    {
        var (g, op) = ActiveOp();
        op.ResolveTick = g.Tick;
        g.Refused(Command.CancelShadyOp(g.Id, op.Id), CommandError.InvalidArgument);
    }
}