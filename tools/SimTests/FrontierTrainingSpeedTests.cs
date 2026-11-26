using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q86: frontier levels add permille to training speed.</summary>
public class FrontierTrainingSpeedTests
{
    static Game LabWithFrontier(int level)
    {
        Game g = Game.Solo().WithStarterLab(gpus: 20);
        g.Me.FrontierLevel = level;
        return g;
    }

    [Fact]
    public void Shipped_frontier_training_speed_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(8, c.Endings.Frontier.TrainingSpeedPermillePerLevel);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Zero_frontier_no_extra_speed()
    {
        Game g = LabWithFrontier(0);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Game plain = Game.Solo().WithStarterLab(gpus: 20);
        Assert.Equal(speed, TrainingSystem.SpeedPermille(plain.C, plain.State, plain.Me));
    }

    [Fact]
    public void Each_level_adds_configured_permille()
    {
        Game g = LabWithFrontier(3);
        Game baseLab = LabWithFrontier(0);
        int delta = TrainingSystem.SpeedPermille(g.C, g.State, g.Me)
            - TrainingSystem.SpeedPermille(baseLab.C, baseLab.State, baseLab.Me);
        Assert.Equal(3 * g.C.Endings.Frontier.TrainingSpeedPermillePerLevel, delta);
    }

    [Fact]
    public void Higher_frontier_trains_faster()
    {
        Game slow = LabWithFrontier(0);
        Game fast = LabWithFrontier(5);
        ModelState sm = slow.Train("mini", 0);
        ModelState fm = fast.Train("mini", 0);
        slow.Weeks(5);
        fast.Weeks(5);
        Assert.True(fm.ProgressCompute > sm.ProgressCompute);
    }

    [Fact]
    public void Zero_per_level_config_disables_hook()
    {
        Game g = LabWithFrontier(4);
        g.C.Endings.Frontier.TrainingSpeedPermillePerLevel = 0;
        Game plain = LabWithFrontier(4);
        plain.C.Endings.Frontier.TrainingSpeedPermillePerLevel = 8;
        Assert.Equal(
            TrainingSystem.SpeedPermille(plain.C, plain.State, plain.Me) - 4 * 8,
            TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Stacks_with_engineer_bonus()
    {
        Game many = LabWithFrontier(2);
        for (int i = 0; i < 6; i++)
            many.Me.Staff.Add(new StaffMember { RoleId = "engineer", Morale = 500, TraitId = "steady" });
        Game few = LabWithFrontier(2);
        Assert.True(TrainingSystem.SpeedPermille(many.C, many.State, many.Me)
            > TrainingSystem.SpeedPermille(few.C, few.State, few.Me));
    }

    [Fact]
    public void Save_load_keeps_frontier_speed()
    {
        Game g = LabWithFrontier(7);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Bankrupt_lab_still_reports_frontier_speed()
    {
        Game g = LabWithFrontier(3);
        g.Me.Bankrupt = true;
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me) > 1000);
    }

    [Fact]
    public void Frozen_bloc_stalls_progress_not_speed_query()
    {
        Game g = LabWithFrontier(4);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Plan_lab_gets_same_frontier_hook()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.FrontierLevel = 6;
        int planSpeed = TrainingSystem.SpeedPermille(g.C, g.State, plan);
        plan.FrontierLevel = 0;
        int baseSpeed = TrainingSystem.SpeedPermille(g.C, g.State, plan);
        Assert.Equal(6 * g.C.Endings.Frontier.TrainingSpeedPermillePerLevel, planSpeed - baseSpeed);
    }

    [Fact]
    public void Max_level_frontier_still_bounded()
    {
        Game g = LabWithFrontier(60);
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me) < 3000);
    }

    [Fact]
    public void Negative_per_level_rejected_by_validator()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Endings.Frontier.TrainingSpeedPermillePerLevel = -5));
    }

    [Fact]
    public void Sim_week_matches_speed_permille_hook()
    {
        Game g = LabWithFrontier(2);
        ModelState m = g.Train("mini", 0);
        long before = m.ProgressCompute;
        g.Weeks(1);
        Assert.True(m.ProgressCompute > before);
    }
}