using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q157: CancelChipOrder command.</summary>
public class CancelChipOrderCommandTests
{
    static (Game g, ChipOrder order, ChipDesign design) PendingOrder()
    {
        var options = new NewGameOptions { Seed = 3, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 500_000_000L);
        var d = new ChipDesign
        {
            Id = g.State.TakeId(), NodeId = "n28", Status = ChipDesignStatus.Ready, YieldPermille = 800, Target = "inference",
            DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", PointsRequired = 1, PointsDone = 1,
        };
        g.Me.ChipDesigns.Add(d);
        ChipSystem.Book(g.State, d.NodeId, 100, 10);
        var order = new ChipOrder
        {
            Id = g.State.TakeId(), DesignId = d.Id, BuyerLabId = g.Id, DesignLabId = g.Id, NodeId = d.NodeId,
            DeliveryTick = g.Tick + 40, Count = 2, HoldingId = HardwareBatch.Cloud, FoundryMonth = 100,
        };
        g.Me.ChipOrders.Add(order);
        return (g, order, d);
    }

    [Fact]
    public void Q157_main_removes_pending_order()
    {
        var (g, order, _) = PendingOrder();
        g.Do(Command.CancelChipOrder(g.Id, order.Id));
        Assert.Empty(g.Me.ChipOrders);
    }

    [Fact]
    public void Q157_main_refunds_permille_of_order_cost()
    {
        var (g, order, d) = PendingOrder();
        long cost = ChipQueries.OrderCost(ChipQueries.Spec(g.C, d), d.YieldPermille, order.Count);
        long cash = g.Me.Cash;
        g.Do(Command.CancelChipOrder(g.Id, order.Id));
        Assert.Equal(cash + MathI.MulPermille(cost, g.C.Chips.CancelOrderRefundPermille), g.Me.Cash);
    }

    [Fact]
    public void Q157_zero_refund_when_permille_zero()
    {
        var (g, order, _) = PendingOrder();
        g.C.Chips.CancelOrderRefundPermille = 0;
        long cash = g.Me.Cash;
        g.Do(Command.CancelChipOrder(g.Id, order.Id));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Empty(g.Me.ChipOrders);
    }

    [Fact]
    public void Q157_max_refused_when_due()
    {
        var (g, order, _) = PendingOrder();
        order.DeliveryTick = g.Tick;
        g.Refused(Command.CancelChipOrder(g.Id, order.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q157_bankrupt_refused()
    {
        var (g, order, _) = PendingOrder();
        g.Me.Bankrupt = true;
        g.Refused(Command.CancelChipOrder(g.Id, order.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q157_save_load_order_gone()
    {
        var (g, order, _) = PendingOrder();
        g.Do(Command.CancelChipOrder(g.Id, order.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Empty(loaded.Me.ChipOrders);
    }

    [Fact]
    public void Q157_cross_expedite_refused_after_cancel()
    {
        var (g, order, _) = PendingOrder();
        g.Do(Command.CancelChipOrder(g.Id, order.Id));
        g.Refused(Command.ExpediteChipOrder(g.Id, order.Id), CommandError.UnknownId);
    }

    [Fact]
    public void Q157_unknown_order_refused()
    {
        var (g, _, _) = PendingOrder();
        g.Refused(Command.CancelChipOrder(g.Id, 99999), CommandError.UnknownId);
    }
}