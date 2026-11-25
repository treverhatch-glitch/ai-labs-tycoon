using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q170: acquisition and model-sale ledger lines (Opus hatch review).</summary>
public class HatchLedgerLaterTests
{
    static ModelState Released(Game g, int cap, string name) => new ModelState
    {
        Id = g.State.TakeId(),
        LineId = "mini",
        Capability = cap,
        Status = ModelStatus.Released,
        Alignment = 500,
        Name = name
    };

    static void DistressedRival(Game g, LabState rival)
    {
        rival.Models.Add(Released(g, 700, "Bot"));
        rival.Cash = -50_000;
        rival.Ledger.ThisMonth.Revenue = 1;
    }

    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    [Fact]
    public void Q170_acquire_books_acquisition_not_hardware()
    {
        var g = Game.New(seed: 701);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        var q = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Ledger.ThisMonth.Acquisition < 0);
        Assert.Equal(0, g.Me.Ledger.ThisMonth.Hardware);
        long debt = q.DebtAssumed;
        Assert.Equal(-(q.Total + debt), g.Me.Ledger.ThisMonth.Acquisition);
    }

    [Fact]
    public void Q170_assumed_debt_adds_to_acquisition_line()
    {
        var g = Game.New(seed: 702);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        rival.Cash = -120_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        var q = AcquisitionSystem.Quote(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Ledger.ThisMonth.Acquisition <= -q.Total);
        Assert.True(q.DebtAssumed > 0);
    }

    [Fact]
    public void Q170_sell_model_books_asset_sale_not_capital_in()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a);
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.Equal(q.OfferCash, g.Me.Ledger.ThisMonth.AssetSale);
        Assert.Equal(0, g.Me.Ledger.ThisMonth.CapitalIn);
    }

    [Fact]
    public void Q170_cash_delta_matches_acquisition_ledger_net()
    {
        var g = Game.New(seed: 703);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        long cash = g.Me.Cash;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(cash - g.Me.Cash, -g.Me.Ledger.ThisMonth.Acquisition);
    }

    [Fact]
    public void Q170_save_load_preserves_new_ledger_totals()
    {
        var g = Game.New(seed: 704);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.True(g.Me.Ledger.ThisMonth.Acquisition < 0);
    }

    [Fact]
    public void Q170_rival_rollup_via_month_end_step_uses_acquisition_line()
    {
        var g = Game.New(seed: 705, tweak: c => c.Acquisitions.RivalRollupChancePermille = 1000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2018);
        g.State.BubbleCrashMonthsRemaining = 8;
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        DistressedRival(g, target);
        buyer.Cash = 100_000_000;
        StepMonthEnd(g);
        Assert.Equal(buyer.Id, target.AbsorbedByLabId);
        Assert.True(buyer.Ledger.ThisMonth.Acquisition < 0);
    }

    [Fact]
    public void Q170_migration_28_adds_zeroed_ledger_lines()
    {
        var g = Game.Solo();
        string json = SaveSchemaPretend.At(g.C, 24, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, json);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        Assert.Equal(0, loaded.Player.Ledger.ThisMonth.Acquisition);
        Assert.Equal(0, loaded.Player.Ledger.ThisMonth.AssetSale);
    }

    [Fact]
    public void Q170_cross_exit_price_still_excludes_asset_sale_income()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 500, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.SellModel(g.Id, a.Id));
        var exit = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.True(g.Me.Ledger.ThisMonth.AssetSale > 0);
        Assert.Equal(g.Me.Cash, exit.CashOnHand);
    }
}