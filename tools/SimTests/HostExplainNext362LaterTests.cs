using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 8: alignment gap and incident host slices (batch 362).</summary>
public class HostExplainNext362LaterTests
{
    [Fact]
    public void Alignment_gap_parts_match_effective_and_displayed()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 80);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"));
        DisplayQueries.AlignmentGapBreakdown gap = DisplayQueries.AlignmentGapParts(g.C, model, g.Tick);
        Assert.Equal(SafetySystem.EffectiveAlignment(g.C, model), gap.EffectiveAlignment);
        Assert.Equal(SafetySystem.DisplayedAlignment(model), gap.DisplayedAlignment);
    }

    [Fact]
    public void Incident_parts_sum_to_configured_chance()
    {
        var model = new ModelState { Alignment = 400, DisplayedAlignment = 400, Corruption = 200, Status = ModelStatus.Released };
        var g = Game.Solo();
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.Equal(SafetySystem.IncidentChancePermille(g.C, g.Tick, model), parts.Total);
        Assert.Equal(parts.FromConfig + parts.FromAlignment + parts.FromCorruption, parts.Total);
    }

    [Fact]
    public void Sandbag_boost_appears_in_gap_breakdown()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 70);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 900, "game-ai"));
        DisplayQueries.AlignmentGapBreakdown gap = DisplayQueries.AlignmentGapParts(g.C, model, g.Tick);
        SectorConfig sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown quality = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(quality.SandbagBoost, gap.SandbagBoost);
    }
}