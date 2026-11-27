using System.Linq;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 6 + item 3: hardware generations and explain-number breakdowns (batch 386).</summary>
public class HostExplainNext386LaterTests
{
    [Fact]
    public void Hardware_fleet_parts_match_owned_and_rented_units()
    {
        var g = Game.Solo().WithStarterLab(gpus: 6);
        g.Rent(2);
        DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        Assert.Equal(LabQueries.CurrentGeneration(g.C, g.Tick).Id, f.CurrentGenId);
        Assert.Equal(f.OwnedHqGpus + f.OwnedOffsiteGpus + f.RentedGpus + f.ReservedGpus, f.TotalUnits);
        Assert.True(f.RentedGpus >= 2);
    }

    [Fact]
    public void Shipped_config_lists_seven_envydia_generations()
    {
        var c = TestData.Config();
        Assert.True(c.Compute.GpuGenerations.Count >= 7);
        Assert.All(c.Compute.GpuGenerations, g => Assert.False(string.IsNullOrEmpty(g.Id)));
    }

    [Fact]
    public void Quality_breakdown_parts_sum_to_displayed_quality()
    {
        var g = Game.Solo().WithStarterLab();
        g.GrantResearch(500);
        g.Research("state-machines");
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown b = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(LabQueries.Quality(g.C, sector, model, g.Tick), b.Total);
        Assert.Equal(b.FromCapability + b.FromAlignment + b.SandbagBoost + b.GlitchAdjustment, b.Total);
    }

    [Fact]
    public void Weekly_revenue_parts_sum_to_last_week_revenue()
    {
        var g = Game.Solo().WithStarterLab(sales: 1);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(6);
        DisplayQueries.RevenueBreakdown rev = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(g.Me.Ledger.LastWeek.Revenue, rev.Total);
        Assert.Equal(rev.Parts.Sum(p => p.Amount), rev.Total);
    }
}