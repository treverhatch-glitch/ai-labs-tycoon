using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q155: AbortChipRespin command.</summary>
public class AbortChipRespinCommandTests
{
    static (Game g, ChipDesign d) InRespinWait()
    {
        var g = RushChipDesignCommandTests.ChipLab();
        var d = new ChipDesign
        {
            Id = g.State.TakeId(), Name = "test", Version = 1, Target = "inference", NodeId = "n28",
            DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air",
            PointsRequired = 100, PointsDone = 100, Status = ChipDesignStatus.TapingOut,
            Respins = 1, StageEndsTick = g.Tick + 25, YieldPermille = 0,
        };
        g.Me.ChipDesigns.Add(d);
        return (g, d);
    }

    [Fact]
    public void Q155_main_finishes_ready_with_crash_keep_yield()
    {
        var (g, d) = InRespinWait();
        ChipSpec spec = ChipQueries.Spec(g.C, d);
        long cash = g.Me.Cash;
        g.Do(Command.AbortChipRespin(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.Ready, d.Status);
        Assert.Equal((int)MathI.MulPermille(spec.InitialYieldPermille, g.C.Chips.YieldCrashKeepPermille), d.YieldPermille);
        Assert.Equal(cash - g.C.Chips.AbortRespinCashCost, g.Me.Cash);
    }

    [Fact]
    public void Q155_zero_respins_refused()
    {
        var (g, d) = InRespinWait();
        d.Respins = 0;
        g.Refused(Command.AbortChipRespin(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q155_max_free_abort_when_cost_zero()
    {
        var (g, d) = InRespinWait();
        g.C.Chips.AbortRespinCashCost = 0;
        g.Me.Cash = 0;
        Assert.True(g.Do(Command.AbortChipRespin(g.Id, d.Id)).Ok);
        Assert.Equal(ChipDesignStatus.Ready, d.Status);
    }

    [Fact]
    public void Q155_bankrupt_refused()
    {
        var (g, d) = InRespinWait();
        g.Me.Bankrupt = true;
        g.Refused(Command.AbortChipRespin(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q155_save_load_ready_design()
    {
        var (g, d) = InRespinWait();
        g.Do(Command.AbortChipRespin(g.Id, d.Id));
        int y = d.YieldPermille;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(y, loaded.Me.ChipDesigns[0].YieldPermille);
        Assert.Equal(ChipDesignStatus.Ready, loaded.Me.ChipDesigns[0].Status);
    }

    [Fact]
    public void Q155_cross_ready_allows_boost_yield()
    {
        var (g, d) = InRespinWait();
        g.Do(Command.AbortChipRespin(g.Id, d.Id));
        int y0 = d.YieldPermille;
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        Assert.True(d.YieldPermille > y0);
    }

    [Fact]
    public void Q155_refused_when_not_taping_out()
    {
        var (g, d) = InRespinWait();
        d.Status = ChipDesignStatus.Designing;
        g.Refused(Command.AbortChipRespin(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q155_needs_cash()
    {
        var (g, d) = InRespinWait();
        g.Me.Cash = g.C.Chips.AbortRespinCashCost - 1;
        g.Refused(Command.AbortChipRespin(g.Id, d.Id), CommandError.NotEnoughCash);
    }
}