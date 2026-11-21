using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SellModelLaterTests
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

    [Fact]
    public void Unprofitable_model_quote_below_training_basis()
    {
        var g = Game.Solo();
        ModelState m = Released(g, 400, "Alpha");
        g.Me.Models.Add(m);
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, m);
        Assert.True(q.OfferCash < q.TrainingCostBasis);
        Assert.Equal(0, q.WeeklyIncome);
    }

    [Fact]
    public void Profitable_model_quote_uses_income_multiple()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 500, "CashCow");
        ModelState b = Released(g, 300, "Spare");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.Me.LastWeek.ServingCompute = 1_000_000_000;
        MarketSystem.RunWeek(g.C, g.State);
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a);
        Assert.True(q.WeeklyIncome > 0);
        Assert.Equal(q.WeeklyIncome * g.C.ModelSales.EarningsMultipleWeeks, q.OfferCash);
        Assert.Equal(q.WeeklyIncome, q.IncomeForegonePerWeek);
    }

    [Fact]
    public void Sell_pays_cash_and_retires_the_model()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        long cash = g.Me.Cash;
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.True(g.Me.Cash > cash);
        Assert.Equal(ModelStatus.Retired, a.Status);
    }

    [Fact]
    public void Sell_removes_sector_entries()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai", "game-bots"));
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.Null(g.State.Sector("game-ai").Entry(g.Id));
    }

    [Fact]
    public void Sell_refused_while_bankrupt()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        Finance.GoBankrupt(g.State, g.Me);
        g.Refused(Command.SellModel(g.Id, b.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Cannot_sell_the_last_active_model()
    {
        var g = Game.Solo();
        ModelState only = Released(g, 600, "Only");
        g.Me.Models.Add(only);
        g.Refused(Command.SellModel(g.Id, only.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Sell_hits_reputation_in_sectors_touched()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        int rep = g.Me.Reputation;
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.Do(Command.SellModel(g.Id, a.Id));
        Assert.True(g.Me.Reputation < rep);
    }

    [Fact]
    public void Sell_refused_for_model_in_training()
    {
        var g = Game.Solo();
        ModelState train = Released(g, 100, "T");
        train.Status = ModelStatus.Training;
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(train);
        g.Me.Models.Add(b);
        g.Refused(Command.SellModel(g.Id, train.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Sell_during_bubble_crash_still_works_when_two_models_exist()
    {
        var g = Game.Solo();
        g.State.BubbleCrashMonthsRemaining = 6;
        ModelState a = Released(g, 500, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.SellModel(g.Id, b.Id));
        Assert.Equal(ModelStatus.Retired, b.Status);
    }

    [Fact]
    public void Save_load_after_sell_round_trips()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.SellModel(g.Id, b.Id));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    [Fact]
    public void Quote_lists_sectors_for_a_released_model()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a);
        Assert.Contains("game-ai", q.Sectors);
    }

    [Fact]
    public void Sell_interacts_with_exit_price_breakdown_after_income_lost()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 1);
        ModelState a = Released(g, 600, "A");
        ModelState b = Released(g, 400, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        Finance.Earn(g.Me, LedgerLine.Revenue, 100_000);
        Finance.CloseMonth(g.C, g.State, g.Me);
        long before = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).RevenueYears;
        g.Do(Command.SellModel(g.Id, a.Id));
        Finance.CloseMonth(g.C, g.State, g.Me);
        long after = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).RevenueYears;
        Assert.True(after <= before);
    }

    [Fact]
    public void Profitable_sale_removes_income_foregone_on_next_market_week()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 700, "Cow");
        ModelState b = Released(g, 400, "Spare");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        g.Do(Command.ReleaseModel(g.Id, a.Id, "game-ai"));
        g.Me.LastWeek.ServingCompute = 1_000_000_000;
        MarketSystem.RunWeek(g.C, g.State);
        Assert.True(DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a).WeeklyIncome > 0);
        g.Do(Command.SellModel(g.Id, a.Id));
        MarketSystem.RunWeek(g.C, g.State);
        Assert.Equal(0, ModelSalesSystem.WeeklyIncome(g.State, g.Me, a));
        Assert.Null(g.State.Sector("game-ai").Entry(g.Id));
    }

    [Fact]
    public void Unproven_trained_model_quote_below_prep_basis()
    {
        var g = Game.Solo();
        ModelState m = Released(g, 200, "Sketch");
        m.Status = ModelStatus.Trained;
        ModelState keep = Released(g, 400, "Keep");
        g.Me.Models.Add(m);
        g.Me.Models.Add(keep);
        var q = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, m);
        Assert.Equal(0, q.WeeklyIncome);
        Assert.True(q.OfferCash < q.TrainingCostBasis);
    }

    [Fact]
    public void Bubble_crash_does_not_change_unprofitable_scrap_quote()
    {
        var g = Game.Solo();
        ModelState a = Released(g, 300, "A");
        ModelState b = Released(g, 300, "B");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        long calm = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a).OfferCash;
        g.State.BubbleCrashMonthsRemaining = 8;
        long crash = DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a).OfferCash;
        Assert.Equal(calm, crash);
    }

    [Fact]
    public void Min_offer_applies_to_tiny_unprofitable_models()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.ModelSales.MinOffer = 50_000;
            c.Line("mini").PrepCost = 1;
        });
        ModelState a = Released(g, 50, "Tiny");
        ModelState b = Released(g, 50, "Keep");
        g.Me.Models.Add(a);
        g.Me.Models.Add(b);
        Assert.Equal(50_000, DisplayQueries.ModelSaleParts(g.C, g.State, g.Me, a).OfferCash);
    }
}