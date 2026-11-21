using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q99: deviceWorkshop trainingSpeedBonusPermille in TrainingSystem.SpeedPermille.</summary>
public class WorkshopTrainingSpeedTests
{
    const string Workshop = "device-workshop";
    const string Devices = "devices";

    static Game Harness(int year = 2028) =>
        ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_workshop_training_speed_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(60, c.Market.DeviceWorkshop.TrainingSpeedBonusPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_bonus_without_room_or_devices_release()
    {
        Game g = Harness();
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Workshop_gate_adds_speed_permille()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        int bare = TrainingSystem.SpeedPermille(g.C, g.State, g.Me) - g.C.Market.DeviceWorkshop.TrainingSpeedBonusPermille;
        Assert.Equal(bare + g.C.Market.DeviceWorkshop.TrainingSpeedBonusPermille,
            TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Gated_lab_trains_faster()
    {
        Game bare = Harness();
        Game wired = Harness();
        ExpansionTestSupport.BuildExpansionRoom(wired, Workshop);
        ExpansionTestSupport.AddReleasedModel(wired, 120_000, Devices);
        ModelState mBare = bare.Train("mini", 0);
        ModelState mWired = wired.Train("mini", 0);
        bare.Weeks(4);
        wired.Weeks(4);
        Assert.True(mWired.ProgressCompute > mBare.ProgressCompute);
    }

    [Fact]
    public void Zero_bonus_config_disables_hook()
    {
        Game g = Harness();
        g.C.Market.DeviceWorkshop.TrainingSpeedBonusPermille = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
        Game bare = Harness();
        Assert.Equal(TrainingSystem.SpeedPermille(bare.C, bare.State, bare.Me),
            TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Bankruptcy_turns_training_bonus_off()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Me.Bankrupt = true;
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Wrong_sector_release_no_speed_bonus()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 90_000, "game-ai");
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Bonus_matches_workshop_config_when_active()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(60, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Save_load_preserves_speed_permille()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Plan_lab_never_gets_workshop_speed()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, plan));
    }

    [Fact]
    public void Before_harnesses_era_no_training_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024).WithStarterLab(gpus: 24);
        Assert.Equal(0, DeviceWorkshopSystem.TrainingSpeedBonusPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Negative_training_bonus_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.DeviceWorkshop.TrainingSpeedBonusPermille = -2));
    }

    [Fact]
    public void Sim_week_progress_reflects_workshop_speed()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        ModelState m = g.Train("mini", 0);
        long before = m.ProgressCompute;
        g.Weeks(1);
        Assert.True(m.ProgressCompute > before);
    }
}