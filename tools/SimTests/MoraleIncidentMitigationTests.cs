using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q104: moraleIncidentMitigationPermillePerPoint on StaffSystem.OnIncident.</summary>
public class MoraleIncidentMitigationTests
{
    [Fact]
    public void Shipped_incident_mitigation_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.StaffPeople.MoraleIncidentMitigationPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_incident_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal(0, StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Happy_staff_earn_mitigation_permille()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 50;
        Assert.Equal(100, StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Unhappy_staff_no_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 300;
        Assert.Equal(0, StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Zero_scale_disables_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoraleIncidentMitigationPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_softens_incident_morale_drop()
    {
        Game g = Game.Solo().WithStarterLab();
        int drop = g.C.StaffPeople.MoraleIncidentDrop;
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 100;
        int morale = g.Me.Staff[0].Morale;
        StaffSystem.OnIncident(g.C, g.Me);
        int mitigated = g.Me.Staff[0].Morale;
        Game sad = Game.Solo().WithStarterLab();
        foreach (StaffMember p in sad.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline;
        int baseMorale = sad.Me.Staff[0].Morale;
        StaffSystem.OnIncident(sad.C, sad.Me);
        Assert.True(mitigated > morale - drop);
        Assert.Equal(baseMorale - drop, sad.Me.Staff[0].Morale);
    }

    [Fact]
    public void Mitigation_clamped_at_permille_cap()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 800 });
        int mit = StaffSystem.MoraleIncidentMitigationPermille(c, lab);
        Assert.Equal(1000, mit);
    }

    [Fact]
    public void No_staff_zero_mitigation()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Formula_uses_points_above_baseline()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 25 });
        Assert.Equal(50, StaffSystem.MoraleIncidentMitigationPermille(c, lab));
    }

    [Fact]
    public void Zero_incident_drop_config_skips_morale_change()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoraleIncidentDrop = 0;
        int morale = g.Me.Staff[0].Morale;
        StaffSystem.OnIncident(g.C, g.Me);
        Assert.Equal(morale, g.Me.Staff[0].Morale);
    }

    [Fact]
    public void Save_load_preserves_mitigation_value()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 750;
        int mit = StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 750;
        Assert.Equal(mit, StaffSystem.MoraleIncidentMitigationPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void All_staff_share_same_incident_drop()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 2);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        StaffSystem.OnIncident(g.C, g.Me);
        int first = g.Me.Staff[0].Morale;
        foreach (StaffMember p in g.Me.Staff) Assert.Equal(first, p.Morale);
    }

    [Fact]
    public void Negative_mitigation_scale_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.StaffPeople.MoraleIncidentMitigationPermillePerPoint = -1));
    }
}