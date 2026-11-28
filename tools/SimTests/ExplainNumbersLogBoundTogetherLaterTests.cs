using System.Linq;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Combined bar: explain numbers + rival log bound (Q25297–Q25302).</summary>
public class ExplainNumbersLogBoundTogetherLaterTests
{
    [Fact]
    public void Q25297_quality_breakdown_matches_lab_query()
    {
        var g = Game.Solo().WithStarterLab();
        g.GrantResearch(500);
        g.Research("state-machines");
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown q = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(LabQueries.Quality(g.C, sector, model, g.Tick), q.Total);
    }

    [Fact]
    public void Q25298_revenue_breakdown_parts_sum()
    {
        var g = Game.Solo().WithStarterLab(sales: 1);
        var m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        g.Weeks(6);
        DisplayQueries.RevenueBreakdown r = DisplayQueries.WeeklyRevenueParts(g.State, g.Me);
        Assert.Equal(g.Me.Ledger.LastWeek.Revenue, r.Total);
        Assert.Equal(r.Parts.Sum(p => p.Amount), r.Total);
    }

    [Fact]
    public void Q25299_log_max_config_matches_shipped_default()
    {
        var c = TestData.Config();
        Assert.True(c.Rivalry.LogMax >= 60);
        Assert.True(c.Vendor.LogMax >= 1);
    }

    [Fact]
    public void Q25300_rival_lines_cap_at_requested_tail()
    {
        Game g = Game.New(25300, c => c.Rivalry.LogMax = 100);
        LabState rival = g.Rival("openaye");
        g.Weeks(40);
        const int want = 5;
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, want);
        Assert.True(lines.Count <= want);
    }

    [Fact]
    public void Q25301_alignment_gap_parts_track_displayed_gap()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 300);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 900, "game-ai"));
        var gap = DisplayQueries.AlignmentGapParts(g.C, model, g.Tick);
        Assert.Equal(SafetySystem.EffectiveAlignment(g.C, model), gap.EffectiveAlignment);
        Assert.Equal(SafetySystem.DisplayedAlignment(model), gap.DisplayedAlignment);
    }

    [Fact]
    public void Q25302_gpu_generations_count_supports_fleet_gen_id()
    {
        var g = Game.Solo().WithStarterLab();
        g.Rent(2);
        Assert.True(g.C.Compute.GpuGenerations.Count >= 7);
        string gen = LabQueries.CurrentGeneration(g.C, g.Tick).Id;
        Assert.All(g.Me.Fleet, slot => Assert.Equal(gen, slot.GpuGenId));
    }
}