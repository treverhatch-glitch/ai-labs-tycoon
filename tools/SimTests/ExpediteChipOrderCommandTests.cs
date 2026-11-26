using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q152: ExpediteChipOrder command.</summary>
public class ExpediteChipOrderCommandTests
{
    static (Game g, ChipOrder order) OrderAt(int deliveryOffset)
    {
        var options = new NewGameOptions { Seed = 2, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        var d = new ChipDesign { Id = g.State.TakeId(), NodeId = "n28", Status = ChipDesignStatus.Ready, YieldPermille = 800, Target = "inference",
            DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", PointsRequired = 1, PointsDone = 1 };
        g.Me.ChipDesigns.Add(d);
        var order = new ChipOrder
        {
            Id = g.State.TakeId(), DesignId = d.Id, BuyerLabId = g.Id, DesignLabId = g.Id, NodeId = d.NodeId,
            DeliveryTick = g.Tick + deliveryOffset, Count = 1, HoldingId = HardwareBatch.Cloud, FoundryMonth = 1,
        };
        g.Me.ChipOrders.Add(order);
        return (g, order);
    }

    [Fact]
    public void Q152_main_pulls_delivery_tick_forward()
    {
        var (g, order) = OrderAt(40);
        int before = order.DeliveryTick;
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        Assert.True(order.DeliveryTick < before);
        Assert.True(order.DeliveryTick >= g.Tick + 1);
    }

    [Fact]
    public void Q152_zero_weeks_disabled()
    {
        var (g, order) = OrderAt(30);
        g.C.Chips.ExpediteDeliveryWeeks = 0;
        g.Refused(Command.ExpediteChipOrder(g.Id, order.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q152_max_clamps_delivery_not_before_next_week()
    {
        var (g, order) = OrderAt(3);
        g.C.Chips.ExpediteDeliveryWeeks = 99;
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        Assert.Equal(g.Tick + 1, order.DeliveryTick);
    }

    [Fact]
    public void Q152_bankrupt_refused()
    {
        var (g, order) = OrderAt(40);
        g.Me.Bankrupt = true;
        g.Refused(Command.ExpediteChipOrder(g.Id, order.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q152_save_load_delivery_persist()
    {
        var (g, order) = OrderAt(35);
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        int tick = order.DeliveryTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(tick, loaded.Me.ChipOrders[0].DeliveryTick);
    }

    [Fact]
    public void Q152_cross_shortage_does_not_block_expedite()
    {
        var (g, order) = OrderAt(50);
        g.State.ChipShortageUntilTick = g.Tick + 200;
        g.Do(Command.ExpediteChipOrder(g.Id, order.Id));
        Assert.True(order.DeliveryTick < g.Tick + 50);
    }

    [Fact]
    public void Q152_refused_when_already_due()
    {
        var (g, order) = OrderAt(0);
        order.DeliveryTick = g.Tick;
        g.Refused(Command.ExpediteChipOrder(g.Id, order.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q152_needs_cash()
    {
        var (g, order) = OrderAt(40);
        g.Me.Cash = g.C.Chips.ExpediteOrderCashCost - 1;
        g.Refused(Command.ExpediteChipOrder(g.Id, order.Id), CommandError.NotEnoughCash);
    }
}