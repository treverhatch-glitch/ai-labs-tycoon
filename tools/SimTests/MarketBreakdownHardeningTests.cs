using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Breakdown queries for the market specialist slice (open weights, sector price, specialist line).</summary>
public class MarketBreakdownHardeningTests
{
    [Fact]
    public void Open_weights_quality_parts_sum_to_displayed_quality()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.True(parts.OpenWeightsBoost > 0);
        Assert.Equal(parts.Total, DisplayQueries.Quality(g.C, sector, model, g.Tick));
        Assert.Equal(
            parts.FromCapability + parts.FromAlignment + parts.SandbagBoost + parts.GlitchAdjustment + parts.OpenWeightsBoost,
            parts.Total);
    }

    [Fact]
    public void Specialist_quality_parts_still_sum_when_bonus_applies()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2020);
        g.Me.Techs.Add("code-models");
        ModelState model = g.Train("code-specialist", 300);
        g.UntilTrained(model);
        var sector = g.C.Sector("code-completion");
        var qParts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.True(SpecialistSystem.QualityBonusPermille(g.C, model.LineId, sector.Id) > 0);
        Assert.Equal(qParts.Total, DisplayQueries.Quality(g.C, sector, model, g.Tick));
        Assert.Equal(
            qParts.FromCapability + qParts.FromAlignment + qParts.SandbagBoost + qParts.GlitchAdjustment + qParts.OpenWeightsBoost,
            qParts.Total);
    }

    [Fact]
    public void Sector_price_scales_last_revenue_in_weekly_breakdown()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(4);
        var baseline = g.Me.LastWeek.Revenue;
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 500));
        g.Weeks(4);
        Assert.True(g.Me.LastWeek.Revenue < baseline);
        var b = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(g.Me.LastWeek.Revenue, b.Total);
    }

    [Fact]
    public void Appeal_uses_quality_including_open_weights_boost()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Weeks(2);
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        long q = DisplayQueries.Quality(g.C, sector, model, g.Tick);
        var qParts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(q, qParts.Total);
        Assert.True(qParts.OpenWeightsBoost > 0);
        long appeal = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.True(appeal >= q * q);
    }
}