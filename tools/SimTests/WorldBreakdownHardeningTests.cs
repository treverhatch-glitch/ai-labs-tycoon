using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class WorldBreakdownHardeningTests
{
    [Fact]
    public void Weekly_power_bill_parts_sum_to_lab_total_and_ledger_power_line()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 80, hall.Id));
        g.Weeks(1);
        var parts = DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me);
        Assert.NotEmpty(parts);
        long summed = parts.Sum(b => b.Total);
        Assert.Equal(WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me), summed);
        Assert.Equal(summed, -g.Me.Ledger.LastWeek.Power);
        foreach (PowerBill bill in parts)
            Assert.Equal(bill.GridCost + bill.PrivateCost + bill.Cooling + bill.ColoMarkup, bill.Total);
    }

    [Fact]
    public void Two_holdings_each_contribute_one_bill_part_row()
    {
        Game g = WorldTests.Rich("punchline");
        Holding tex = WorldTests.Hall(g, "us-texas", 80);
        Holding ice = WorldTests.Hall(g, "nordics-iceland", 60);
        g.Do(Command.BuyGpus(g.Id, 40, tex.Id));
        g.Do(Command.BuyGpus(g.Id, 30, ice.Id));
        var parts = DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me);
        Assert.Equal(2, parts.Count);
        Assert.Equal(parts.Sum(b => b.Total), WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
    }

    [Fact]
    public void Colo_markup_appears_only_in_the_colo_holding_part()
    {
        Game g = WorldTests.Rich();
        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 80)).CreatedId;
        g.Do(Command.BuyGpus(g.Id, 50, colo));
        PowerBill bill = WorldQueries.BillFor(g.C, g.State, g.Me, g.Me.Holding(colo));
        Assert.True(bill.ColoMarkup > 0);
        Assert.Equal(bill.Total, DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me).Single().Total);
    }

    [Fact]
    public void Liquid_cooling_shrinks_cooling_part_not_grid_cost()
    {
        Game g = WorldTests.Rich("punchline");
        Holding texas = WorldTests.Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 100, texas.Id));
        PowerBill before = WorldQueries.BillFor(g.C, g.State, g.Me, texas);
        g.Me.Techs.Add("liquid-cooling");
        PowerBill after = WorldQueries.BillFor(g.C, g.State, g.Me, texas);
        Assert.Equal(before.GridCost, after.GridCost);
        Assert.True(after.Cooling < before.Cooling);
        Assert.Equal(after.Total, before.GridCost + after.Cooling);
    }

    [Fact]
    public void Attention_gain_parts_sum_and_home_bloc_earns_dominance_at_high_share()
    {
        Game g = WorldTests.Rich(null, c => c.World.Attention.DominanceFromPpm = 300_000);
        AttentionGain gain = WorldQueries.GainFor(g.C, g.State, g.Me, g.Me.HomeBlocId, 800_000);
        Assert.Equal(gain.FromFootprint + gain.FromIncidents + gain.FromDominance + gain.PolicySaving, gain.Total);
        Assert.Equal((800_000 - 300_000) / g.C.World.Attention.DominanceDivisor, gain.FromDominance);
        Assert.Equal(0, WorldQueries.GainFor(g.C, g.State, g.Me, "eu", 800_000).FromDominance);
    }

    [Fact]
    public void Policy_staff_reduces_attention_gain_parts_before_month_close()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "us-virginia", 150);
        g.Do(Command.BuyGpus(g.Id, 80, hall.Id));
        AttentionGain plain = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        g.Build("policy-office", 5, 5);
        g.Hire("policy", 3);
        AttentionGain staffed = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        Assert.True(staffed.PolicySaving < 0);
        Assert.True(staffed.Total < plain.Total);
    }

    [Fact]
    public void Month_close_applies_gain_total_to_stored_attention()
    {
        Game g = WorldTests.Duo();
        Holding hall = WorldTests.Hall(g, "us-oregon", 120);
        g.Do(Command.BuyGpus(g.Id, 60, hall.Id));
        BlocStanding standing = g.Me.Bloc("us");
        int before = standing.Attention;
        AttentionGain gain = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        g.Weeks(4);
        Assert.True(standing.Attention >= before + gain.Total / 2);
    }

    [Fact]
    public void Bankrupt_lab_power_parts_stay_zero_without_holdings()
    {
        var g = Game.Solo();
        g.Me.Bankrupt = true;
        Assert.Empty(DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me));
        Assert.Equal(0, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
    }

    [Fact]
    public void Power_and_attention_parts_survive_save_load()
    {
        Game g = WorldTests.Rich();
        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 50)).CreatedId;
        g.Do(Command.BuyGpus(g.Id, 40, colo));
        long bill = WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me);
        var gains = DisplayQueries.AttentionGainParts(g.C, g.State, g.Me, 100_000);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(bill, WorldQueries.WeeklyPowerBill(loaded.C, loaded.State, loaded.Me));
        Assert.Equal(
            gains.Sum(x => x.Total),
            DisplayQueries.AttentionGainParts(loaded.C, loaded.State, loaded.Me, 100_000).Sum(x => x.Total));
    }
}