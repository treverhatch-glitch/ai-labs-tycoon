using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q117: DisplayQueries.IncidentParts matches SafetySystem with lab-aware safety-staff cut.</summary>
public class IncidentBreakdownHardeningTests
{
    static ModelState Released(int align = 400, int corruption = 0) =>
        new ModelState { Status = ModelStatus.Released, Alignment = align, Corruption = corruption };

    [Fact]
    public void Shipped_incident_breakdown_validates_config()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(30, c.Safety.IncidentChancePermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Parts_sum_to_displayed_chance_without_lab()
    {
        Game g = Game.Solo();
        ModelState m = Released();
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, m);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, m));
        Assert.Equal(b.FromConfig + b.FromAlignment + b.FromCorruption + b.FromSafetyStaff, b.Total);
    }

    [Fact]
    public void Parts_sum_to_displayed_chance_with_safety_staff()
    {
        Game g = Game.Solo().WithStarterLab(safety: 3);
        ModelState m = Released();
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, m, g.Me);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, m, g.Me));
        Assert.Equal(b.FromConfig + b.FromAlignment + b.FromCorruption + b.FromSafetyStaff, b.Total);
    }

    [Fact]
    public void Safety_staff_part_is_non_positive()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState m = Released();
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, m, g.Me);
        Assert.True(b.FromSafetyStaff <= 0);
        Assert.True(b.Total < SafetySystem.IncidentChancePermille(g.C, m));
    }

    [Fact]
    public void Zero_staff_cut_config_yields_zero_staff_part()
    {
        Game g = Game.Solo().WithStarterLab(safety: 3);
        g.C.Safety.SafetyStaffIncidentCutPermillePerStaff = 0;
        ModelState m = Released();
        int bare = SafetySystem.IncidentChancePermille(g.C, m);
        Assert.Equal(bare, SafetySystem.IncidentChancePermille(g.C, m, g.Me));
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, m, g.Me).FromSafetyStaff);
    }

    [Fact]
    public void Corruption_part_positive_when_corrupted()
    {
        Game g = Game.Solo();
        DisplayQueries.IncidentBreakdown clean = DisplayQueries.IncidentParts(g.C, Released(corruption: 0));
        DisplayQueries.IncidentBreakdown dirty = DisplayQueries.IncidentParts(g.C, Released(corruption: 400));
        Assert.True(dirty.FromCorruption > clean.FromCorruption);
        Assert.True(dirty.Total > clean.Total);
    }

    [Fact]
    public void Alignment_part_tracks_lower_alignment()
    {
        Game g = Game.Solo();
        DisplayQueries.IncidentBreakdown safe = DisplayQueries.IncidentParts(g.C, Released(align: 900));
        DisplayQueries.IncidentBreakdown risk = DisplayQueries.IncidentParts(g.C, Released(align: 100));
        Assert.True(risk.Total > safe.Total);
        Assert.True(risk.FromAlignment > safe.FromAlignment);
    }

    [Fact]
    public void Staff_part_scales_with_headcount()
    {
        Game a = Game.Solo().WithStarterLab(safety: 0);
        Game b = Game.Solo().WithStarterLab(safety: 0);
        string trait = StaffSystem.DefaultTrait(a.C.StaffPeople);
        a.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = trait });
        for (int i = 0; i < 3; i++)
            b.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = trait });
        ModelState m = Released(align: 200);
        int cutOne = DisplayQueries.IncidentParts(a.C, m, a.Me).FromSafetyStaff;
        int cutThree = DisplayQueries.IncidentParts(b.C, m, b.Me).FromSafetyStaff;
        Assert.True(cutThree < cutOne);
    }

    [Fact]
    public void Save_load_preserves_breakdown_equality()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState m = Released();
        DisplayQueries.IncidentParts(g.C, m, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(loaded.C, m, loaded.Me);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(loaded.C, m, loaded.Me));
    }

    [Fact]
    public void Cross_with_training_incident_roll_uses_same_total()
    {
        Game g = Game.Solo().WithStarterLab(safety: 1);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        model = g.Me.Models.First(m => m.Status == ModelStatus.Released);
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, model, g.Me);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, model, g.Me));
    }

    [Fact]
    public void Null_lab_matches_legacy_two_arg_path()
    {
        Game g = Game.Solo().WithStarterLab(safety: 3);
        ModelState m = Released();
        DisplayQueries.IncidentBreakdown noLab = DisplayQueries.IncidentParts(g.C, m);
        DisplayQueries.IncidentBreakdown explicitNull = DisplayQueries.IncidentParts(g.C, m, null);
        Assert.Equal(noLab.Total, explicitNull.Total);
        Assert.Equal(0, noLab.FromSafetyStaff);
    }

    [Fact]
    public void Max_safety_staff_cut_caps_staff_part()
    {
        Game g = Game.Solo().WithStarterLab(safety: 0);
        string trait = StaffSystem.DefaultTrait(g.C.StaffPeople);
        for (int i = 0; i < 12; i++)
            g.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = trait });
        ModelState m = Released(align: 100);
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, m, g.Me);
        int noLab = SafetySystem.IncidentChancePermille(g.C, m);
        Assert.True(b.Total >= 0);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, m, g.Me));
        Assert.True(b.Total <= noLab);
    }

    [Fact]
    public void Breakdown_matches_breakdown_tests_contract()
    {
        Game g = Game.Solo();
        ModelState m = new ModelState { Alignment = 400, Corruption = 200, Status = ModelStatus.Released };
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, m);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, m));
    }
}