using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BreakdownTests
{
    [Fact]
    public void Quality_breakdown_sums_to_the_displayed_quality()
    {
        var g = Game.Solo().WithStarterLab();
        g.GrantResearch(500);
        g.Research("state-machines");
        var model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown b = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(b.Total, LabQueries.Quality(g.C, sector, model, g.Tick));
        Assert.True(b.FromCapability > 0);
    }

    [Fact]
    public void Appeal_breakdown_sums_to_the_displayed_appeal()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(2);
        SectorState sectorState = g.State.Sector("game-ai");
        var sector = g.C.Sector("game-ai");
        DisplayQueries.AppealBreakdown b = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        long appeal = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.Equal(appeal, b.Total);
        Assert.Equal(b.Core + b.ReputationBoost + b.SalesBoost + b.ChampionBoost, appeal);
    }

    [Fact]
    public void Incident_odds_breakdown_sums_to_the_displayed_permille()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 400, Corruption = 200, Status = ModelStatus.Released };
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, model);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, model));
        Assert.Equal(b.FromConfig + b.FromAlignment + b.FromCorruption, b.Total);
    }

    [Fact]
    public void Weekly_revenue_breakdown_sums_to_last_week_revenue()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai", "game-bots");
        g.Weeks(8);
        DisplayQueries.RevenueBreakdown b = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(g.Me.LastWeek.Revenue, b.Total);
        Assert.Equal(b.Parts.Sum(p => p.Amount), b.Total);
        Assert.True(b.Total > 0);
    }
}