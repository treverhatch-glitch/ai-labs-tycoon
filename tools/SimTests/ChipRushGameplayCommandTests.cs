using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q151–Q154: cash rush commands on chip design, orders, tapeout, and yield.</summary>
public class ChipRushGameplayCommandTests
{
    static Game ChipLab(long cash = 50_000_000_000L)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        string[] techs = { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling", "chip-codesign" };
        foreach (string t in techs) if (!g.Me.Techs.Contains(t)) g.Me.Techs.Add(t);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        g.Do(Command.Hire(g.Id, "chip-architect", 2));
        return g;
    }

  static ChipDesign Designing(Game g)
    {
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        return ChipQueries.DesignInProgress(g.Me);
    }

    [Fact]
    public void Rush_design_adds_points()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        long before = d.PointsDone;
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        Assert.Equal(before + g.C.Chips.RushDesignPoints, d.PointsDone);
    }

    [Fact]
    public void Rush_design_refused_when_complete()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        d.PointsDone = d.PointsRequired;
        g.Refused(Command.RushChipDesign(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Rush_design_bankrupt()
    {
        Game g = ChipLab(cash: 1);
        ChipDesign d = Designing(g);
        g.Me.Bankrupt = true;
        g.Refused(Command.RushChipDesign(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Rush_design_save_load()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        long pts = d.PointsDone;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(pts, loaded.Me.ChipDesigns.Find(x => x.Id == d.Id).PointsDone);
    }

    [Fact]
    public void Cross_rush_then_tapeout()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        while (d.PointsDone < d.PointsRequired)
            g.Do(Command.RushChipDesign(g.Id, d.Id));
        g.Do(Command.TapeOut(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
    }

    [Fact]
    public void Accelerate_tapeout_pulls_stage_tick()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        d.PointsDone = d.PointsRequired;
        g.Do(Command.TapeOut(g.Id, d.Id));
        int before = d.StageEndsTick;
        g.Do(Command.AccelerateChipTapeout(g.Id, d.Id));
        Assert.True(d.StageEndsTick < before);
    }

    [Fact]
    public void Boost_yield_respects_cap()
    {
        Game g = ChipLab();
        ChipDesign d = ReadyDesign(g);
        d.YieldPermille = 400;
        int max = ChipQueries.Spec(g.C, d).MaxYieldPermille;
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        Assert.True(d.YieldPermille <= max);
        Assert.True(d.YieldPermille > 400);
    }

    [Fact]
    public void Boost_yield_zero_boost_disabled()
    {
        Game g = ChipLab();
        ChipDesign d = Designing(g);
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = 100;
        g.C.Chips.BoostYieldPermille = 0;
        g.Refused(Command.BoostChipYield(g.Id, d.Id), CommandError.InvalidArgument);
    }

    static ChipDesign ReadyDesign(Game g)
    {
        ChipDesign d = Designing(g);
        d.PointsDone = d.PointsRequired;
        g.Do(Command.TapeOut(g.Id, d.Id));
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = 700;
        d.ReadyTick = g.Tick;
        return d;
    }

    [Fact]
    public void Expedite_order_pulls_delivery()
    {
        Game g = ChipLab();
        ChipDesign d = ReadyDesign(g);
        var order = new ChipOrder
        {
            Id = g.State.TakeId(), DesignId = d.Id, BuyerLabId = g.Id, DesignLabId = g.Id,
            DeliveryTick = g.Tick + 40, Count = 1, HoldingId = HardwareBatch.Cloud, FoundryMonth = 1, NodeId = d.NodeId,
        };
        g.Me.ChipOrders.Add(order);
        int before = order.DeliveryTick;
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        Assert.True(order.DeliveryTick < before);
    }

    [Fact]
    public void Expedite_save_load()
    {
        Game g = ChipLab();
        ChipDesign d = ReadyDesign(g);
        var order = new ChipOrder
        {
            Id = g.State.TakeId(), DesignId = d.Id, BuyerLabId = g.Id, DesignLabId = g.Id,
            DeliveryTick = g.Tick + 30, Count = 1, HoldingId = HardwareBatch.Cloud, FoundryMonth = 1, NodeId = d.NodeId,
        };
        g.Me.ChipOrders.Add(order);
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        int tick = order.DeliveryTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(tick, loaded.Me.ChipOrders[0].DeliveryTick);
    }

    [Fact]
    public void Cross_expedite_while_shortage_active()
    {
        Game g = ChipLab();
        ChipDesign d = ReadyDesign(g);
        var order = new ChipOrder
        {
            Id = g.State.TakeId(), DesignId = d.Id, BuyerLabId = g.Id, DesignLabId = g.Id,
            DeliveryTick = g.Tick + 50, Count = 10, HoldingId = HardwareBatch.Cloud, FoundryMonth = 2, NodeId = d.NodeId,
        };
        g.Me.ChipOrders.Add(order);
        g.State.ChipShortageUntilTick = g.Tick + 100;
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        Assert.True(order.DeliveryTick < g.Tick + 50);
    }
}