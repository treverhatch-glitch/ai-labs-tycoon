using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q105: salesReputationRecoveryPermillePerStaff in SafetySystem.RunWeek.</summary>
public class SalesReputationRecoveryTests
{
    static void AddSalesStaff(Game g, int count)
    {
        string trait = StaffSystem.DefaultTrait(g.C.StaffPeople);
        for (int i = 0; i < count; i++)
            g.Me.Staff.Add(new StaffMember { RoleId = "sales", Morale = g.C.StaffPeople.MoraleBaseline, TraitId = trait });
    }

    [Fact]
    public void Shipped_sales_reputation_recovery_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Safety.SalesReputationRecoveryPermillePerStaff);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Below_baseline_recovers_each_week()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Reputation = g.C.Safety.ReputationBaseline - 40;
        int before = g.Me.Reputation;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation > before);
    }

    [Fact]
    public void Sales_staff_add_extra_recovery_step()
    {
        Game bare = Game.Solo().WithStarterLab(sales: 0);
        Game staffed = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(staffed, 3);
        bare.Me.Reputation = 400;
        staffed.Me.Reputation = 400;
        SafetySystem.RunWeek(bare.C, bare.State, bare.Me);
        SafetySystem.RunWeek(staffed.C, staffed.State, staffed.Me);
        int bareStep = bare.Me.Reputation - 400;
        int staffedStep = staffed.Me.Reputation - 400;
        Assert.True(staffedStep > bareStep);
        Assert.Equal(bareStep + 3 * staffed.C.Safety.SalesReputationRecoveryPermillePerStaff, staffedStep);
    }

    [Fact]
    public void Zero_sales_recovery_config_ignores_staff()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 4);
        g.C.Safety.SalesReputationRecoveryPermillePerStaff = 0;
        g.Me.Reputation = 420;
        int before = g.Me.Reputation;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Game plain = Game.Solo().WithStarterLab(sales: 0);
        plain.Me.Reputation = 420;
        SafetySystem.RunWeek(plain.C, plain.State, plain.Me);
        Assert.Equal(plain.Me.Reputation - 420, g.Me.Reputation - before);
    }

    [Fact]
    public void At_baseline_reputation_does_not_rise_from_recovery()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 2);
        g.Me.Reputation = g.C.Safety.ReputationBaseline;
        int before = g.Me.Reputation;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Above_baseline_drifts_down_not_sales_boosted()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 3);
        g.Me.Reputation = g.C.Safety.ReputationBaseline + 30;
        int before = g.Me.Reputation;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation < before);
    }

    [Fact]
    public void Recovery_clamped_at_baseline()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 5);
        g.Me.Reputation = g.C.Safety.ReputationBaseline - 3;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Safety.ReputationBaseline, g.Me.Reputation);
    }

    [Fact]
    public void Sim_week_applies_sales_recovery()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 2);
        g.Me.Reputation = 450;
        int before = g.Me.Reputation;
        g.Weeks(1);
        Assert.True(g.Me.Reputation > before);
    }

    [Fact]
    public void Save_load_preserves_recovery_behavior()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 2);
        g.Me.Reputation = 430;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        int rep = g.Me.Reputation;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Me.Reputation = 430;
        SafetySystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(rep, loaded.Me.Reputation);
    }

    [Fact]
    public void Plan_lab_uses_same_sales_recovery()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Staff.Add(new StaffMember { RoleId = "sales", Morale = 500, TraitId = g.C.StaffPeople.Traits[0].Id });
        plan.Reputation = 410;
        SafetySystem.RunWeek(g.C, g.State, plan);
        Game solo = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(solo, 1);
        solo.Me.Reputation = 410;
        SafetySystem.RunWeek(solo.C, solo.State, solo.Me);
        Assert.Equal(solo.Me.Reputation, plan.Reputation);
    }

    [Fact]
    public void Bankrupt_lab_still_runs_reputation_recovery()
    {
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 1);
        g.Me.Bankrupt = true;
        g.Me.Reputation = 440;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation > 440);
    }

    [Fact]
    public void Negative_sales_recovery_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.SalesReputationRecoveryPermillePerStaff = -1));
    }

    [Fact]
    public void Recovery_step_includes_baseline_plus_sales()
    {
        GameConfig c = TestData.Config();
        int step = c.Safety.ReputationRecoveryPerWeek + 2 * c.Safety.SalesReputationRecoveryPermillePerStaff;
        Game g = Game.Solo().WithStarterLab(sales: 0);
        AddSalesStaff(g, 2);
        g.Me.Reputation = c.Safety.ReputationBaseline - step;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(c.Safety.ReputationBaseline, g.Me.Reputation);
    }
}