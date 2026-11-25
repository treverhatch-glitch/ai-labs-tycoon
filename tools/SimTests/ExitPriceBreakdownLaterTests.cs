using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ExitPriceBreakdownLaterTests
{
    [Fact]
    public void Breakdown_total_matches_SellPrice()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 3);
        g.Me.Ledger.ThisMonth.Revenue = 80_000;
        g.Me.Cash = 25_000;
        DisplayQueries.ExitPriceBreakdown b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(b.RevenueYears + b.FleetResale + b.CashOnHand - b.StaffShare - b.Loans - b.Notes - b.Rbf - b.CovenantCalls, b.Total);
        Assert.Equal(EndingSystem.SellPrice(g.C, g.State, g.Me), b.Total);
    }

    [Fact]
    public void Revenue_years_uses_ledger_revenue_times_exit_years()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 2);
        g.Me.Ledger.ThisMonth.Revenue = 50_000;
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(100_000, b.RevenueYears);
    }

    [Fact]
    public void Cash_on_hand_is_the_labs_current_cash()
    {
        var g = Game.Solo();
        g.Me.Cash = 333_333;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(333_333, b.CashOnHand);
        Assert.Equal(333_333, b.Total);
    }

    [Fact]
    public void Fleet_resale_respects_exit_fleet_permille()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitFleetPermille = 400);
        g.Me.Cash = 0;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        long fleet = LabQueries.FleetResaleValue(g.C, g.State, g.Me);
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(MathI.MulPermille(fleet, 400), b.FleetResale);
    }

    [Fact]
    public void Zero_exit_years_zeros_revenue_part_only()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 0);
        g.Me.Ledger.ThisMonth.Revenue = 1_000_000;
        g.Me.Cash = 10_000;
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(0, b.RevenueYears);
        Assert.Equal(b.CashOnHand + b.FleetResale, b.Total);
    }

    [Fact]
    public void Historical_ledger_months_feed_revenue_last_twelve_months()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 10_000;
        var month = new MonthRecord();
        month.Totals.Add(LedgerLine.Revenue, 30_000);
        g.Me.Ledger.History.Add(month);
        long rev = EndingSystem.RevenueLast12Months(g.Me);
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(rev, b.RevenueYears);
    }

    [Fact]
    public void SellLab_found_again_uses_the_same_price_as_the_breakdown()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        long price = EndingSystem.SellPrice(g.C, g.State, g.Me);
        Assert.Equal(price, DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total);
        g.Do(Command.SellLab(g.Id, "found-again"));
        Assert.True(g.Me.Ledger.Lifetime.CapitalIn >= price - 1);
    }

    [Fact]
    public void Save_load_keeps_sell_price_breakdown_identical()
    {
        var g = Game.Solo();
        g.Me.Ledger.ThisMonth.Revenue = 90_000;
        g.Me.Cash = 12_000;
        var before = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        var after = DisplayQueries.ExitPriceParts(loaded.Config, loaded.State, loaded.Player);
        Assert.Equal(before.Total, after.Total);
        Assert.Equal(before.FleetResale, after.FleetResale);
    }

    [Fact]
    public void Schema_17_migration_save_still_round_trips_exit_price()
    {
        var g = Game.Solo();
        g.Me.Ledger.ThisMonth.Revenue = 40_000;
        string old = SaveSchemaPretend.At(g.C, 16, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        SaveSchemaPretend.AssertRoundTrip(sim);
        Assert.Equal(DisplayQueries.ExitPriceParts(sim.Config, sim.State, sim.Player).Total,
            EndingSystem.SellPrice(sim.Config, sim.State, sim.Player));
    }

    [Fact]
    public void Sell_refused_when_price_is_not_above_cash_already_in_the_lab()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 0);
        g.Me.Cash = 1_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        Assert.True(EndingSystem.SellPrice(g.C, g.State, g.Me) <= g.Me.Cash);
        g.Refused(Command.SellLab(g.Id, "found-again"), CommandError.SellRefused);
    }

    [Fact]
    public void A_week_of_revenue_changes_the_breakdown_after_close_month()
    {
        var g = Game.Solo();
        g.Me.Ledger.ThisMonth.Revenue = 0;
        Finance.Earn(g.Me, LedgerLine.Revenue, 50_000);
        Finance.CloseMonth(g.C, g.State, g.Me);
        long after = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).RevenueYears;
        Assert.Equal(50_000 * g.C.Endings.ExitRevenueYears, after);
    }

    [Fact]
    public void Incident_breakdown_still_sums_after_exit_price_query()
    {
        var g = Game.Solo();
        g.Me.Ledger.ThisMonth.Revenue = 20_000;
        var exit = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        var model = new ModelState { Alignment = 400, Corruption = 100, Status = ModelStatus.Released };
        var incident = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.Equal(EndingSystem.SellPrice(g.C, g.State, g.Me), exit.Total);
        Assert.Equal(incident.Total, SafetySystem.IncidentChancePermille(g.C, g.Tick, model));
    }
}