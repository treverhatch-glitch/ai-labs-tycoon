using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q162: ReviveChipDesign command.</summary>
public class ReviveChipDesignCommandTests
{
    static (Game g, ChipDesign d) CancelledDesign()
    {
        var g = RushChipDesignCommandTests.ChipLab();
        var d = RushChipDesignCommandTests.Start(g);
        g.Do(Command.CancelChipDesign(g.Id, d.Id));
        return (g, d);
    }

    [Fact]
    public void Q162_main_restores_designing_status()
    {
        var (g, d) = CancelledDesign();
        g.Do(Command.ReviveChipDesign(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.Designing, d.Status);
    }

    [Fact]
    public void Q162_main_spends_config_cash()
    {
        var (g, d) = CancelledDesign();
        long cash = g.Me.Cash;
        g.Do(Command.ReviveChipDesign(g.Id, d.Id));
        Assert.Equal(cash - g.C.Chips.ReviveDesignCashCost, g.Me.Cash);
    }

    [Fact]
    public void Q162_zero_refused_when_not_cancelled()
    {
        var g = RushChipDesignCommandTests.ChipLab();
        var d = RushChipDesignCommandTests.Start(g);
        g.Refused(Command.ReviveChipDesign(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q162_max_refused_while_other_design_active()
    {
        var (g, d) = CancelledDesign();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        g.Refused(Command.ReviveChipDesign(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q162_bankrupt_refused()
    {
        var (g, d) = CancelledDesign();
        g.Me.Bankrupt = true;
        g.Refused(Command.ReviveChipDesign(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q162_save_load_designing()
    {
        var (g, d) = CancelledDesign();
        g.Do(Command.ReviveChipDesign(g.Id, d.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(ChipDesignStatus.Designing, loaded.Me.ChipDesigns[0].Status);
    }

    [Fact]
    public void Q162_cross_rush_after_revive()
    {
        var (g, d) = CancelledDesign();
        g.Do(Command.ReviveChipDesign(g.Id, d.Id));
        long pts = d.PointsDone;
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        Assert.True(d.PointsDone > pts);
    }

    [Fact]
    public void Q162_needs_cash()
    {
        var (g, d) = CancelledDesign();
        g.Me.Cash = g.C.Chips.ReviveDesignCashCost - 1;
        g.Refused(Command.ReviveChipDesign(g.Id, d.Id), CommandError.NotEnoughCash);
    }
}