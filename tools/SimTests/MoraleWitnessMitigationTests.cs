using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q107: moralePoachWitnessMitigationPermillePerPoint on poach witness drop.</summary>
public class MoraleWitnessMitigationTests
{
    [Fact]
    public void Shipped_witness_mitigation_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.StaffPeople.MoralePoachWitnessMitigationPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_witness_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal(0, StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Happy_staff_earn_witness_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 50;
        Assert.Equal(100, StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Zero_scale_disables_witness_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoralePoachWitnessMitigationPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_reduces_effective_witness_drop()
    {
        GameConfig c = TestData.Config();
        int drop = c.StaffPeople.MoralePoachWitnessDrop;
        var happy = new LabState();
        happy.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 80 });
        int mit = StaffSystem.MoralePoachWitnessMitigationPermille(c, happy);
        int effective = (int)AiLabs.Rules.Core.MathI.MulPermille(drop, AiLabs.Rules.Core.MathI.Permille - mit);
        var sad = new LabState();
        sad.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline });
        int sadDrop = c.StaffPeople.MoralePoachWitnessDrop;
        Assert.True(effective < sadDrop);
        Assert.Equal(160, mit);
    }

    [Fact]
    public void Mitigation_clamped_at_permille()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 900 });
        Assert.Equal(1000, StaffSystem.MoralePoachWitnessMitigationPermille(c, lab));
    }

    [Fact]
    public void Unhappy_staff_no_mitigation()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 300;
        Assert.Equal(0, StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void No_staff_zero_mitigation()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me));
    }

    [Fact]
    public void Save_load_preserves_mitigation_formula()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        int mit = StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 700;
        Assert.Equal(mit, StaffSystem.MoralePoachWitnessMitigationPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void Stacks_with_incident_mitigation_independently()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 40;
        Assert.True(StaffSystem.MoraleIncidentMitigationPermille(g.C, g.Me) > 0);
        Assert.True(StaffSystem.MoralePoachWitnessMitigationPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void Plan_lab_uses_same_witness_formula()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Staff.Add(new StaffMember { Morale = 800, RoleId = "researcher", TraitId = g.C.StaffPeople.Traits[0].Id });
        Assert.True(StaffSystem.MoralePoachWitnessMitigationPermille(g.C, plan) > 0);
    }

    [Fact]
    public void Negative_witness_mitigation_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.StaffPeople.MoralePoachWitnessMitigationPermillePerPoint = -1));
    }

    [Fact]
    public void Points_above_baseline_only()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 20 });
        Assert.Equal(40, StaffSystem.MoralePoachWitnessMitigationPermille(c, lab));
    }
}