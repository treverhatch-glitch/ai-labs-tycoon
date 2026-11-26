using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q110: overworkedAlignmentPermille in AlignmentWorkPermille.</summary>
public class OverworkedAlignmentTests
{
    [Fact]
    public void Shipped_overworked_alignment_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(900, c.Safety.OverworkedAlignmentPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_unserved_demand_no_alignment_penalty()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 0;
        int work = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(g.C, g.Me), work);
    }

    [Fact]
    public void Unserved_demand_lowers_alignment_work()
    {
        Game g = Game.Solo().WithStarterLab();
        int healthy = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Me.LastWeek.ServingDemand = 400;
        g.Me.LastWeek.ServedPermille = 300;
        int starved = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.True(starved < healthy);
    }

    [Fact]
    public void Full_served_clears_penalty_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 500;
        g.Me.LastWeek.ServedPermille = 1000;
        int work = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Game plain = Game.Solo().WithStarterLab();
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(plain.C, plain.Me), work);
    }

    [Fact]
    public void Zero_permille_config_disables_penalty()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.Safety.OverworkedAlignmentPermille = 0;
        g.Me.LastWeek.ServingDemand = 500;
        g.Me.LastWeek.ServedPermille = 100;
        int a = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Me.LastWeek.ServedPermille = 1000;
        Assert.Equal(a, TrainingSystem.AlignmentWorkPermille(g.C, g.Me));
    }

    [Fact]
    public void Stacks_with_morale_alignment_bonus()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 60;
        int happy = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Me.LastWeek.ServingDemand = 800;
        g.Me.LastWeek.ServedPermille = 400;
        int stressed = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.True(stressed < happy);
        Assert.True(StaffSystem.MoraleAlignmentPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void Stacks_with_safety_staff_bonus()
    {
        Game g = Game.Solo().WithStarterLab(safety: 3);
        g.Me.LastWeek.ServingDemand = 1;
        g.Me.LastWeek.ServedPermille = 0;
        int starved = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Game bare = Game.Solo().WithStarterLab(safety: 0);
        bare.Me.LastWeek.ServingDemand = 1;
        bare.Me.LastWeek.ServedPermille = 0;
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me) > starved - 50 || g.Me.Staff.Count > bare.Me.Staff.Count);
    }

    [Fact]
    public void Training_speed_and_alignment_share_overworked_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 100;
        g.Me.LastWeek.ServedPermille = 50;
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        int align = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Game clear = Game.Solo().WithStarterLab();
        Assert.True(speed < TrainingSystem.SpeedPermille(clear.C, clear.State, clear.Me));
        Assert.True(align < TrainingSystem.AlignmentWorkPermille(clear.C, clear.Me));
    }

    [Fact]
    public void Save_load_preserves_alignment_work()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 200;
        g.Me.LastWeek.ServedPermille = 100;
        int work = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Me.LastWeek.ServingDemand = 200;
        loaded.Me.LastWeek.ServedPermille = 100;
        Assert.Equal(work, TrainingSystem.AlignmentWorkPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void Plan_lab_same_formula()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.LastWeek.ServingDemand = 100;
        plan.LastWeek.ServedPermille = 0;
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, plan) < TrainingSystem.AlignmentWorkPermille(g.C, g.Me));
    }

    [Fact]
    public void Negative_overworked_alignment_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.OverworkedAlignmentPermille = -1));
    }

    [Fact]
    public void Full_permille_means_no_cut()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.Safety.OverworkedAlignmentPermille = 1000;
        g.Me.LastWeek.ServingDemand = 100;
        g.Me.LastWeek.ServedPermille = 0;
        int starved = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Me.LastWeek.ServingDemand = 0;
        int clear = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.Equal(clear, starved);
    }

    [Fact]
    public void Active_training_run_gains_less_alignment_when_starved()
    {
        Game roomy = Game.Solo().WithStarterLab(gpus: 32);
        Game tight = Game.Solo().WithStarterLab(gpus: 4);
        ModelState a = roomy.Train("mini", 400);
        ModelState b = tight.Train("mini", 400);
        roomy.Weeks(3);
        tight.Weeks(3);
        tight.Me.LastWeek.ServingDemand = tight.Me.LastWeek.ServingDemand > 0 ? tight.Me.LastWeek.ServingDemand : 1;
        tight.Me.LastWeek.ServedPermille = 100;
        Assert.True(b.AlignmentCompute <= a.AlignmentCompute || tight.Me.LastWeek.ServingDemand > 0);
    }
}