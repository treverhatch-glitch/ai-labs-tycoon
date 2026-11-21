using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q97: moralePoachResistPermillePerPoint lowers StaffSystem.TryPoach chance.</summary>
public class MoralePoachResistTests
{
    [Fact]
    public void Shipped_poach_resist_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.StaffPeople.MoralePoachResistPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_poach_resist()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal(0, StaffSystem.MoralePoachResistPermille(g.C, g.Me));
    }

    [Fact]
    public void Happy_staff_earn_poach_resist_permille()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 40;
        Assert.Equal(40, StaffSystem.MoralePoachResistPermille(g.C, g.Me));
    }

    [Fact]
    public void Unhappy_staff_no_poach_resist()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline - 50;
        Assert.Equal(0, StaffSystem.MoralePoachResistPermille(g.C, g.Me));
    }

    [Fact]
    public void Zero_scale_config_disables_resist()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoralePoachResistPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoralePoachResistPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_blocks_feud_poach_attempt()
    {
        var g = Game.New(seed: 42);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        g.Me.Staff[0].Morale = g.C.StaffPeople.MoraleBaseline + 500;
        g.C.StaffPeople.PoachChancePermille = 500;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        int count = g.Me.Staff.Count;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(count, g.Me.Staff.Count);
    }

    [Fact]
    public void Low_morale_rival_poach_can_succeed()
    {
        var g = Game.New(seed: 77);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        g.Me.Staff[0].Morale = 200;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        g.C.StaffPeople.PoachChancePermille = 1000;
        int before = g.Me.Staff.Count;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.True(g.Me.Staff.Count < before);
    }

    [Fact]
    public void Poaching_never_happens_without_feud()
    {
        var g = Game.New(seed: 88);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = 0;
        g.C.StaffPeople.PoachChancePermille = 1000;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Single(g.Me.Staff);
    }

    [Fact]
    public void Bankrupt_player_skips_poach_even_with_resist()
    {
        var g = Game.New(seed: 99);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        g.Me.Bankrupt = true;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 50;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Single(g.Me.Staff);
    }

    [Fact]
    public void No_staff_zero_resist()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoralePoachResistPermille(g.C, g.Me));
    }

    [Fact]
    public void Formula_uses_points_above_baseline_only()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 30 });
        Assert.Equal(30 * c.StaffPeople.MoralePoachResistPermillePerPoint,
            StaffSystem.MoralePoachResistPermille(c, lab));
    }

    [Fact]
    public void Save_load_preserves_resist_value()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        int resist = StaffSystem.MoralePoachResistPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 800;
        Assert.Equal(resist, StaffSystem.MoralePoachResistPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void Negative_poach_resist_scale_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.StaffPeople.MoralePoachResistPermillePerPoint = -1));
    }
}