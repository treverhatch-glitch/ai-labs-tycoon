using System.Linq;
using AiLabs.Rules.Config;
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
    public void Open_weights_off_leaves_boost_at_zero_but_parts_still_sum()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(0, parts.OpenWeightsBoost);
        Assert.Equal(parts.Total, LabQueries.Quality(g.C, sector, model, g.Tick));
    }

    [Fact]
    public void Specialist_quality_parts_sum_when_code_bonus_applies()
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
    }

    [Fact]
    public void Glitching_model_quality_breakdown_matches_discounted_total()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        model.Incidents = 1;
        model.LastIncidentTick = g.Tick - 1;
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.True(parts.GlitchAdjustment < 0);
        Assert.Equal(parts.Total, DisplayQueries.Quality(g.C, sector, model, g.Tick));
    }

    [Fact]
    public void Sector_price_min_permille_lowers_weekly_revenue_breakdown_total()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(6);
        long mid = g.Me.LastWeek.Revenue;
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", g.C.Market.SectorPriceMinPermille));
        g.Weeks(6);
        Assert.True(g.Me.LastWeek.Revenue < mid);
        var b = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(g.Me.LastWeek.Revenue, b.Total);
        Assert.Single(b.Parts);
    }

    [Fact]
    public void Sector_price_max_and_open_weights_stack_on_revenue()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", g.C.Market.SectorPriceMaxPermille));
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Weeks(8);
        Assert.True(g.Me.LastWeek.Revenue > 0);
        Assert.Equal(g.Me.LastWeek.Revenue, DisplayQueries.WeeklyRevenueParts(g.State, g.Me).Total);
    }

    [Fact]
    public void Appeal_parts_still_sum_with_expansion_room_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 180_000, "robotics-industry");
        ExpansionTestSupport.BuildExpansionRoom(g, "robotics-arena");
        var sector = g.C.Sector("robotics-industry");
        var sectorState = g.State.Sector("robotics-industry");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.True(parts.ExpansionBoost > 0);
        long appeal = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.Equal(appeal, parts.Total);
    }

    [Fact]
    public void Bankrupt_lab_still_queries_quality_breakdown_for_spectate()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Me.Bankrupt = true;
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(parts.Total, DisplayQueries.Quality(g.C, sector, model, g.Tick));
    }

    [Fact]
    public void Quality_and_revenue_breakdowns_survive_save_load_after_open_weights()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Weeks(4);
        var sector = g.C.Sector("game-ai");
        long qBefore = DisplayQueries.QualityParts(g.C, sector, model, g.Tick).Total;
        long revBefore = DisplayQueries.WeeklyRevenueParts(g.State, g.Me).Total;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState again = loaded.Me.Model(model.Id);
        Assert.Equal(qBefore, DisplayQueries.QualityParts(loaded.C, loaded.C.Sector("game-ai"), again, loaded.State.Tick).Total);
        Assert.Equal(revBefore, DisplayQueries.WeeklyRevenueParts(loaded.State, loaded.Me).Total);
    }

    [Fact]
    public void Weekly_revenue_breakdown_omits_sectors_with_no_lab_revenue()
    {
        var g = Game.Solo().WithStarterLab();
        var b = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(0, b.Total);
        Assert.Empty(b.Parts);
    }
}