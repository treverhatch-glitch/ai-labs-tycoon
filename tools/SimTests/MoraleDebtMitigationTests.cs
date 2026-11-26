using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q113: moraleDebtMitigationPermillePerPoint softens debt morale drift.</summary>
public class MoraleDebtMitigationTests
{
    [Fact]
    public void Shipped_debt_mitigation_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.StaffPeople.MoraleDebtMitigationPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_debt_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal(0, StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Happy_staff_earn_debt_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 50;
        Assert.Equal(150, StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Zero_scale_disables_debt_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoraleDebtMitigationPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Debt_in_weeks_lowers_morale_less_when_happy()
    {
        Game happy = Game.Solo(1, c => c.StaffPeople.MoraleDriftPerWeek = 0).WithStarterLab();
        Game sad = Game.Solo(1, c => c.StaffPeople.MoraleDriftPerWeek = 0).WithStarterLab();
        happy.Me.WeeksInDebt = 5;
        sad.Me.WeeksInDebt = 5;
        foreach (StaffMember p in happy.Me.Staff) p.Morale = happy.C.StaffPeople.MoraleBaseline + 80;
        foreach (StaffMember p in sad.Me.Staff) p.Morale = sad.C.StaffPeople.MoraleBaseline;
        int h0 = happy.Me.Staff[0].Morale;
        int s0 = sad.Me.Staff[0].Morale;
        StaffSystem.RunWeek(happy.C, happy.Me);
        StaffSystem.RunWeek(sad.C, sad.Me);
        Assert.True(happy.Me.Staff[0].Morale > h0 - happy.C.StaffPeople.MoraleDebtDriftPerWeek);
        Assert.Equal(s0 - sad.C.StaffPeople.MoraleDebtDriftPerWeek, sad.Me.Staff[0].Morale);
    }

    [Fact]
    public void No_debt_skips_drift_even_with_mitigation()
    {
        Game g = Game.Solo(1, c => c.StaffPeople.MoraleDriftPerWeek = 0).WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline;
        int before = g.Me.Staff[0].Morale;
        StaffSystem.RunWeek(g.C, g.Me);
        Assert.Equal(before, g.Me.Staff[0].Morale);
    }

    [Fact]
    public void Mitigation_clamped_at_permille()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 500 });
        Assert.Equal(1000, StaffSystem.MoraleDebtMitigationPermille(c, lab));
    }

    [Fact]
    public void Unhappy_staff_no_debt_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 300;
        Assert.Equal(0, StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Save_load_preserves_debt_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 720;
        int mit = StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 720;
        Assert.Equal(mit, StaffSystem.MoraleDebtMitigationPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void Stacks_with_incident_mitigation_hooks()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 40;
        Assert.True(StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me) > 0);
        Assert.True(StaffSystem.MoraleDebtMitigationPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void Plan_lab_uses_same_debt_mitigation()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Staff.Add(new StaffMember { Morale = 800, RoleId = "researcher", TraitId = g.C.StaffPeople.Traits[0].Id });
        Assert.True(StaffSystem.MoraleDebtMitigationPermille(g.C, plan) > 0);
    }

    [Fact]
    public void Negative_debt_mitigation_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.StaffPeople.MoraleDebtMitigationPermillePerPoint = -2));
    }

    [Fact]
    public void Formula_uses_points_above_baseline()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 30 });
        Assert.Equal(90, StaffSystem.MoraleDebtMitigationPermille(c, lab));
    }
}