using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Host explain for NEXT #8–#11: alignment gap, incidents, staff roster, balance soak preview.</summary>
public class HostExplainSystemsLaterTests
{
    [Fact]
    public void Alignment_gap_parts_track_displayed_versus_effective()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 80);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"));
        DisplayQueries.AlignmentGapBreakdown gap = DisplayQueries.AlignmentGapParts(g.C, model, g.Tick);
        Assert.Equal(SafetySystem.EffectiveAlignment(g.C, model), gap.EffectiveAlignment);
        Assert.Equal(SafetySystem.DisplayedAlignment(model), gap.DisplayedAlignment);
        Assert.True(SafetySystem.AlignmentGap(g.C, model) <= gap.GapPermille);
        SectorConfig sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown quality = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(quality.SandbagBoost, gap.SandbagBoost);
    }

    [Fact]
    public void Incident_parts_sum_to_model_incident_chance()
    {
        var model = new ModelState { Alignment = 400, DisplayedAlignment = 400, Corruption = 200, Status = ModelStatus.Released };
        var g = Game.Solo();
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.Equal(parts.FromConfig + parts.FromAlignment + parts.FromCorruption, parts.Total);
        Assert.Equal(SafetySystem.IncidentChancePermille(g.C, g.Tick, model), parts.Total);
        Assert.True(SafetySystem.PickIncidentRung(g.C, model) >= 1);
    }

    [Fact]
    public void Staff_roster_parts_match_headcount_and_payroll()
    {
        var g = Game.Solo();
        DisplayQueries.StaffRosterBreakdown roster = DisplayQueries.StaffRosterParts(g.C, g.Me);
        Assert.Equal(g.Me.Staff.Count, roster.Headcount);
        Assert.Equal(LabQueries.WeeklySalaries(g.C, g.Me), roster.WeeklySalaries);
        if (roster.Headcount > 0) Assert.True(roster.AverageMoralePermille >= 0);
    }

    [Fact]
    public void Balance_soak_preview_lists_default_presets()
    {
        GameConfig c = TestData.Config();
        DisplayQueries.BalanceSoakHostPreview preview = DisplayQueries.BalanceSoakPreview(c);
        Assert.Equal(BalanceSoak.DefaultPresets(c).Length, preview.PresetCount);
        Assert.Equal(24, preview.DefaultGamesPerPreset);
    }
}