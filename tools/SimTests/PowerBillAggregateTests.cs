using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q76: WorldQueries.WeeklyPowerBillTotals aggregates holding power lines.</summary>
public class PowerBillAggregateTests
{
    [Fact]
    public void Shipped_sites_and_power_config_validate()
    {
        Assert.NotEmpty(TestData.Config().World.Sites);
        ConfigValidator.Validate(TestData.Config());
    }

    [Fact]
    public void Main_path_totals_match_weekly_power_bill()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 80, hall.Id));
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        Assert.Equal(t.Total, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
        Assert.Equal(t.GridCost + t.PrivateCost + t.Cooling + t.ColoMarkup, t.Total);
    }

    [Fact]
    public void Zero_holdings_zero_totals()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        Assert.Equal(0, t.Total);
        Assert.Equal(0, LabQueries.RunningCostParts(g.C, g.State, g.Me).Power);
    }

    [Fact]
    public void Max_kw_hall_raises_grid_line()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "us-texas", 500);
        g.Do(Command.BuyGpus(g.Id, 400, hall.Id));
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        Assert.True(t.GridCost > 0);
        Assert.True(t.Total >= t.GridCost);
    }

    [Fact]
    public void Bankrupt_lab_with_holding_still_queries_bill()
    {
        Game g = WorldTests.Rich();
        Holding hall = WorldTests.Hall(g, "us-oregon", 80);
        g.Do(Command.BuyGpus(g.Id, 40, hall.Id));
        g.Me.Bankrupt = true;
        Assert.True(WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Total >= 0);
    }

    [Fact]
    public void Save_load_preserves_aggregated_totals()
    {
        Game g = WorldTests.Rich();
        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 60)).CreatedId;
        g.Do(Command.BuyGpus(g.Id, 50, colo));
        long total = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Total;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(total, WorldQueries.WeeklyPowerBillTotals(loaded.C, loaded.State, loaded.Me).Total);
    }

    [Fact]
    public void Colo_markup_only_in_aggregate_colo_line()
    {
        Game g = WorldTests.Rich();
        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 80)).CreatedId;
        g.Do(Command.BuyGpus(g.Id, 50, colo));
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        Assert.True(t.ColoMarkup > 0);
        Assert.Equal(WorldQueries.BillFor(g.C, g.State, g.Me, g.Me.Holding(colo)).ColoMarkup, t.ColoMarkup);
    }

    [Fact]
    public void Two_holdings_sum_in_aggregate()
    {
        Game g = WorldTests.Rich("punchline");
        Holding a = WorldTests.Hall(g, "us-texas", 80);
        Holding b = WorldTests.Hall(g, "nordics-iceland", 60);
        g.Do(Command.BuyGpus(g.Id, 50, a.Id));
        g.Do(Command.BuyGpus(g.Id, 30, b.Id));
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        long perHolding = DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me).Sum(bill => bill.Total);
        Assert.Equal(perHolding, t.Total);
    }

    [Fact]
    public void Finance_week_power_spend_matches_aggregate_after_step()
    {
        Game g = WorldTests.Rich();
        Holding hall = WorldTests.Hall(g, "us-virginia", 120);
        g.Do(Command.BuyGpus(g.Id, 70, hall.Id));
        long bill = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Total;
        long before = g.Me.Ledger.Lifetime.Power;
        g.Weeks(1);
        Assert.Equal(before - bill, g.Me.Ledger.Lifetime.Power);
    }

    [Fact]
    public void Running_cost_power_line_matches_aggregate()
    {
        Game g = WorldTests.Rich();
        Holding hall = WorldTests.Hall(g, "us-oregon", 90);
        g.Do(Command.BuyGpus(g.Id, 60, hall.Id));
        Assert.Equal(
            WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Total,
            LabQueries.RunningCostParts(g.C, g.State, g.Me).Power);
    }

    [Fact]
    public void Liquid_cooling_shrinks_cooling_aggregate_line()
    {
        Game g = WorldTests.Rich("punchline");
        Holding texas = WorldTests.Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 90, texas.Id));
        long before = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Cooling;
        g.Me.Techs.Add("liquid-cooling");
        long after = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Cooling;
        Assert.True(after < before);
    }

    [Fact]
    public void Owned_hall_upkeep_separate_from_power_aggregate()
    {
        Game g = WorldTests.Rich();
        Holding hall = WorldTests.Hall(g, "us-oregon", 120);
        g.Do(Command.BuyGpus(g.Id, 30, hall.Id));
        long power = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Total;
        long upkeep = WorldQueries.WeeklyHallUpkeep(g.C, g.Me);
        Assert.True(power > 0);
        Assert.True(upkeep > 0);
        Assert.True(LabQueries.RunningCostParts(g.C, g.State, g.Me).Rooms >= upkeep);
    }

    [Fact]
    public void Single_holding_aggregate_matches_bill_for()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "nordics-iceland", 80);
        g.Do(Command.BuyGpus(g.Id, 25, hall.Id));
        PowerBill one = WorldQueries.BillFor(g.C, g.State, g.Me, hall);
        WorldQueries.PowerBillTotals t = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me);
        Assert.Equal(one.GridCost, t.GridCost);
        Assert.Equal(one.Cooling, t.Cooling);
        Assert.Equal(one.Total, t.Total);
    }
}