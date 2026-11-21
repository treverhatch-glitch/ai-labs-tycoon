using System.Linq;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class WorldBreakdownHardeningTests
{
    [Fact]
    public void Weekly_power_bill_parts_sum_to_lab_total()
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
    public void Attention_gain_parts_sum_internally_per_bloc()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = WorldTests.Hall(g, "us-virginia", 120);
        g.Do(Command.BuyGpus(g.Id, 60, hall.Id));
        var gains = DisplayQueries.AttentionGainParts(g.C, g.State, g.Me, 400_000);
        foreach (AttentionGain gain in gains)
        {
            Assert.Equal(gain.FromFootprint + gain.FromIncidents + gain.FromDominance + gain.PolicySaving, gain.Total);
            if (gain.BlocId == g.Me.HomeBlocId)
            {
                Assert.True(gain.FromFootprint >= 0);
            }
            else
            {
                Assert.Equal(0, gain.FromIncidents);
                Assert.Equal(0, gain.FromDominance);
            }
        }
    }

    [Fact]
    public void Power_bill_parts_survive_save_load()
    {
        Game g = WorldTests.Rich();
        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 50)).CreatedId;
        g.Do(Command.BuyGpus(g.Id, 40, colo));
        long before = WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me);
        var partsBefore = DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(before, WorldQueries.WeeklyPowerBill(loaded.C, loaded.State, loaded.Me));
        Assert.Equal(partsBefore.Sum(b => b.Total), DisplayQueries.WeeklyPowerBillParts(loaded.C, loaded.State, loaded.Me).Sum(b => b.Total));
    }

    [Fact]
    public void Empty_holdings_yield_empty_power_parts_and_zero_bill()
    {
        var g = Game.Solo();
        Assert.Empty(DisplayQueries.WeeklyPowerBillParts(g.C, g.State, g.Me));
        Assert.Equal(0, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
    }
}