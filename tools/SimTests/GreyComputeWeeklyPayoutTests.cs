using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class GreyComputeWeeklyPayoutTests
{
    [Fact]
    public void Each_week_during_grey_compute_earns_kw_scaled_cash()
    {
        var g = Game.Solo(seed: 7);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 4000;
        g.Do(Command.BuyGpus(g.Id, 20, hall));
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        ShadyCardConfig card = g.C.Shady.Card("grey-compute");
        card.SuccessPermille = 1000;
        card.TracePermille = 0;
        card.Weeks = 4;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        long before = g.Me.Ledger.Lifetime.GreyRevenue;
        g.Sim.Step();
        int halfKw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall) / 2;
        long weekPay = g.C.Shady.Card("grey-compute").BasePayout * halfKw;
        Assert.Equal(before + weekPay, g.Me.Ledger.Lifetime.GreyRevenue);
    }

    [Fact]
    public void Resolve_does_not_double_pay_grey_compute_lump()
    {
        var g = Game.Solo(seed: 8);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 10_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 1000;
        g.Do(Command.BuyGpus(g.Id, 4, hall));
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        ShadyCardConfig card = g.C.Shady.Card("grey-compute");
        card.SuccessPermille = 1000;
        card.TracePermille = 0;
        card.Weeks = 1;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        long before = g.Me.Ledger.Lifetime.GreyRevenue;
        g.Until(() => g.Me.ShadyOps.Count == 0, 4, "grey");
        int halfKw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall) / 2;
        Assert.Equal(before + card.BasePayout * halfKw, g.Me.Ledger.Lifetime.GreyRevenue);
    }

    [Fact]
    public void Multi_week_op_pays_each_active_week()
    {
        var g = Game.Solo(seed: 9);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 20_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 2000;
        g.Do(Command.BuyGpus(g.Id, 8, hall));
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        ShadyCardConfig card = g.C.Shady.Card("grey-compute");
        card.SuccessPermille = 1000;
        card.TracePermille = 0;
        card.Weeks = 3;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        g.Weeks(2);
        int halfKw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall) / 2;
        Assert.Equal(2 * card.BasePayout * halfKw, g.Me.Ledger.Lifetime.GreyRevenue);
    }

    [Fact]
    public void Bankrupt_lab_skips_grey_payout()
    {
        var g = Game.Solo();
        g.Me.Bankrupt = true;
        g.Me.ShadyOps.Add(new ShadyOp { Id = 1, CardId = "grey-compute", AuxId = 1, ResolveTick = g.State.Tick + 2 });
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.Ledger.Lifetime.GreyRevenue);
    }

    [Fact]
    public void Ledger_identity_holds_through_grey_weeks()
    {
        var g = Game.Solo(seed: 10);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 500_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 800;
        g.Rent(4);
        var model = new ModelState { Id = g.State.TakeId(), Capability = 400, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.C.Shady.Card("grey-compute").TracePermille = 0;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        long baseCash = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Sim.Step();
        Assert.Equal(baseCash, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }

    [Fact]
    public void Save_load_keeps_grey_op_mid_payout_schedule()
    {
        var g = Game.Solo(seed: 12);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 10_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 1000;
        g.Do(Command.BuyGpus(g.Id, 4, hall));
        var model = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 300, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.C.Shady.Card("grey-compute").Weeks = 4;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        g.Weeks(2);
        long paid = g.Me.Ledger.Lifetime.GreyRevenue;
        Assert.True(paid > 0);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Single(loaded.Me.ShadyOps);
        loaded.Weeks(2);
        Assert.True(loaded.Me.Ledger.Lifetime.GreyRevenue >= paid);
    }

    [Fact]
    public void No_active_grey_op_pays_nothing()
    {
        var g = Game.Solo();
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.Ledger.Lifetime.GreyRevenue);
    }
}