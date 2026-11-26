using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q93: unserved serving demand cuts lab training speed.</summary>
public class OverworkedTrainingSpeedTests
{
    static Game WithReleasedMini(out ModelState model, int gpus = 8)
    {
        Game g = Game.Solo().WithStarterLab(gpus: gpus);
        model = g.Train("mini", 300);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(1);
        return g;
    }

    [Fact]
    public void Shipped_overworked_training_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(850, c.Safety.OverworkedTrainingSpeedPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Healthy_serving_no_speed_penalty()
    {
        Game g = WithReleasedMini(out _, 24);
        g.Weeks(2);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.LastWeek.ServingDemand = 0;
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Unserved_demand_lowers_speed_permille()
    {
        Game roomy = WithReleasedMini(out _, 32);
        Game starved = WithReleasedMini(out _, 8);
        roomy.Weeks(20);
        starved.Weeks(20);
        starved.Rent(0);
        starved.Weeks(1);
        Assert.True(starved.Me.LastWeek.ServingDemand > 0);
        Assert.True(starved.Me.LastWeek.ServedPermille < 1000);
        int penalized = TrainingSystem.SpeedPermille(starved.C, starved.State, starved.Me);
        starved.Me.LastWeek.ServedPermille = 1000;
        int clear = TrainingSystem.SpeedPermille(starved.C, starved.State, starved.Me);
        Assert.True(penalized < clear);
    }

    [Fact]
    public void Speed_cut_matches_config_permille()
    {
        Game g = WithReleasedMini(out _, 2);
        g.Weeks(25);
        g.Me.LastWeek.ServingDemand = 500;
        g.Me.LastWeek.ServedPermille = 400;
        int penalized = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.LastWeek.ServedPermille = 1000;
        int clear = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Assert.True(penalized < clear);
    }

    [Fact]
    public void Starved_lab_trains_slower_than_roomy()
    {
        Game fast = WithReleasedMini(out ModelState fm, 28);
        Game slow = WithReleasedMini(out ModelState sm, 1);
        ModelState trFast = fast.Train("mini", 0);
        ModelState trSlow = slow.Train("mini", 0);
        fast.Weeks(6);
        slow.Weeks(6);
        Assert.True(trFast.ProgressCompute > trSlow.ProgressCompute);
    }

    [Fact]
    public void Zero_penalty_config_disables_hook()
    {
        Game g = WithReleasedMini(out _, 1);
        g.C.Safety.OverworkedTrainingSpeedPermille = 1000;
        g.Weeks(15);
        int a = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Game g2 = WithReleasedMini(out _, 1);
        g2.Weeks(15);
        Assert.Equal(a, TrainingSystem.SpeedPermille(g2.C, g2.State, g2.Me));
    }

    [Fact]
    public void No_training_runs_still_reports_speed()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 100;
        g.Me.LastWeek.ServedPermille = 100;
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Morale_speed_stacks_before_overworked_cut()
    {
        Game g = WithReleasedMini(out _, 8);
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 80;
        g.Weeks(20);
        g.Rent(0);
        g.Weeks(1);
        int happyPenalized = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.LastWeek.ServedPermille = 1000;
        int happyClear = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Assert.True(happyClear > happyPenalized);
    }

    [Fact]
    public void Save_load_preserves_speed_penalty_state()
    {
        Game g = WithReleasedMini(out _, 2);
        g.Weeks(22);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(1);
        Assert.Equal(speed, TrainingSystem.SpeedPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Full_served_permille_clears_penalty()
    {
        Game g = WithReleasedMini(out _, 4);
        g.Weeks(18);
        g.Me.LastWeek.ServingDemand = 999;
        g.Me.LastWeek.ServedPermille = 999;
        int mild = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.LastWeek.ServedPermille = 1000;
        int full = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Assert.True(full > mild);
    }

    [Fact]
    public void Plan_lab_uses_same_overworked_hook()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.LastWeek.ServingDemand = 200;
        plan.LastWeek.ServedPermille = 100;
        int cut = TrainingSystem.SpeedPermille(g.C, g.State, plan);
        plan.LastWeek.ServedPermille = 1000;
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, plan) > cut);
    }

    [Fact]
    public void Negative_overworked_training_permille_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.OverworkedTrainingSpeedPermille = -1));
    }

    [Fact]
    public void Penalty_only_when_demand_positive()
    {
        Game g = Game.Solo().WithStarterLab();
        int baseSpeed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.LastWeek.ServingDemand = 0;
        g.Me.LastWeek.ServedPermille = 0;
        Assert.Equal(baseSpeed, TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }
}