using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q108: safetyStaffIncidentCutPermillePerStaff on incident odds.</summary>
public class SafetyStaffIncidentCutTests
{
    static ModelState Released(Game g) =>
        new ModelState { Status = ModelStatus.Released, Alignment = 400, Corruption = 0 };

    [Fact]
    public void Shipped_safety_staff_incident_cut_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(15, c.Safety.SafetyStaffIncidentCutPermillePerStaff);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_safety_staff_no_cut()
    {
        Game g = Game.Solo().WithStarterLab(safety: 0);
        ModelState m = Released(g);
        int bare = SafetySystem.IncidentChancePermille(g.C, m);
        int withLab = SafetySystem.IncidentChancePermille(g.C, m, g.Me);
        Assert.Equal(bare, withLab);
    }

    [Fact]
    public void Safety_staff_lowers_incident_chance()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState m = Released(g);
        int bare = SafetySystem.IncidentChancePermille(g.C, m);
        int staffed = SafetySystem.IncidentChancePermille(g.C, m, g.Me);
        Assert.True(staffed < bare);
    }

    [Fact]
    public void Cut_scales_with_staff_count()
    {
        Game a = Game.Solo().WithStarterLab(safety: 0);
        Game b = Game.Solo().WithStarterLab(safety: 0);
        string trait = StaffSystem.DefaultTrait(a.C.StaffPeople);
        a.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = trait });
        for (int i = 0; i < 3; i++)
            b.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = trait });
        ModelState m = Released(a);
        m.Alignment = 200;
        int one = SafetySystem.IncidentChancePermille(a.C, m, a.Me);
        int three = SafetySystem.IncidentChancePermille(b.C, m, b.Me);
        Assert.True(three < one);
        Assert.True(one > 0);
    }

    [Fact]
    public void Zero_config_disables_staff_cut()
    {
        Game g = Game.Solo().WithStarterLab(safety: 3);
        g.C.Safety.SafetyStaffIncidentCutPermillePerStaff = 0;
        ModelState m = Released(g);
        int bare = SafetySystem.IncidentChancePermille(g.C, m);
        Assert.Equal(bare, SafetySystem.IncidentChancePermille(g.C, m, g.Me));
    }

    [Fact]
    public void Corruption_still_raises_odds_with_staff()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState clean = Released(g);
        ModelState corrupt = Released(g);
        corrupt.Corruption = 300;
        int cleanOdds = SafetySystem.IncidentChancePermille(g.C, clean, g.Me);
        int corruptOdds = SafetySystem.IncidentChancePermille(g.C, corrupt, g.Me);
        Assert.True(corruptOdds > cleanOdds);
    }

    [Fact]
    public void Cut_clamped_at_permille()
    {
        Game g = Game.Solo().WithStarterLab(safety: 0);
        g.C.Safety.SafetyStaffIncidentCutPermillePerStaff = 200;
        for (int i = 0; i < 8; i++)
            g.Me.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = g.C.StaffPeople.Traits[0].Id });
        ModelState m = Released(g);
        int odds = SafetySystem.IncidentChancePermille(g.C, m, g.Me);
        Assert.True(odds >= 0);
    }

    [Fact]
    public void Save_load_preserves_incident_formula()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState m = Released(g);
        int odds = SafetySystem.IncidentChancePermille(g.C, m, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(odds, SafetySystem.IncidentChancePermille(loaded.C, m, loaded.Me));
    }

    [Fact]
    public void Plan_lab_uses_same_staff_cut()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Staff.Add(new StaffMember { RoleId = "safety", Morale = 500, TraitId = g.C.StaffPeople.Traits[0].Id });
        ModelState m = Released(g);
        int odds = SafetySystem.IncidentChancePermille(g.C, m, plan);
        Assert.True(odds < SafetySystem.IncidentChancePermille(g.C, m));
    }

    [Fact]
    public void Negative_staff_cut_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.SafetyStaffIncidentCutPermillePerStaff = -1));
    }

    [Fact]
    public void Null_lab_model_only_path_unchanged()
    {
        ModelState m = new ModelState { Status = ModelStatus.Released, Alignment = 400 };
        GameConfig c = TestData.Config();
        Assert.Equal(SafetySystem.IncidentChancePermille(c, m), SafetySystem.IncidentChancePermille(c, m, null));
    }

    [Fact]
    public void High_alignment_keeps_odds_low_with_staff()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2);
        ModelState m = Released(g);
        m.Alignment = 950;
        Assert.True(SafetySystem.IncidentChancePermille(g.C, m, g.Me) < 50);
    }

    [Fact]
    public void Staff_cut_independent_of_sales_recovery()
    {
        Game g = Game.Solo().WithStarterLab(safety: 2, sales: 2);
        g.Me.Reputation = 400;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        ModelState m = Released(g);
        Assert.True(SafetySystem.IncidentChancePermille(g.C, m, g.Me) > 0);
    }
}