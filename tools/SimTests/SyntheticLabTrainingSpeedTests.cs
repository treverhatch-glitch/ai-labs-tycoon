using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q85: synthetic-data-lab grants training speed on the synthetic mix.</summary>
public class SyntheticLabTrainingSpeedTests
{
    const string Room = "synthetic-data-lab";
    const string Synthetic = "synthetic";

    static Game Harness(int year = 2028)
    {
        Game g = ExpansionTestSupport.SoloAtYear(year);
        g.WithStarterLab(gpus: 32);
        return g;
    }

    [Fact]
    public void Shipped_synthetic_speed_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(120, c.Training.SyntheticDataLab.TrainingSpeedBonusPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Inactive_without_room_no_speed_bonus()
    {
        Game g = Harness();
        g.C.Training.DataSource("synthetic").OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState m = g.Train("mini", 0);
        g.Weeks(3);
        long withoutRoom = m.ProgressCompute;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        m.ProgressCompute = 0;
        g.Weeks(3);
        Assert.True(m.ProgressCompute > withoutRoom);
    }

    [Fact]
    public void Internal_mix_unaffected_by_synthetic_lab_room()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        ModelState a = g.Train("mini", 0);
        Game g2 = Harness(2028);
        ExpansionTestSupport.BuildExpansionRoom(g2, Room);
        ModelState b = g2.Train("mini", 0);
        g.Weeks(5);
        g2.Weeks(5);
        Assert.Equal(a.ProgressCompute, b.ProgressCompute);
    }

    [Fact]
    public void Active_synthetic_run_faster_than_without_room()
    {
        Game g = Harness(2028);
        g.C.Training.SyntheticDataLab.TrainingSpeedBonusPermille = 250;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState m = g.Train("mini", 0);
        g.Weeks(5);
        long withoutRoom = m.ProgressCompute;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.True(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
        m.ProgressCompute = 0;
        g.Weeks(5);
        Assert.True(m.ProgressCompute > withoutRoom);
    }

    [Fact]
    public void Zero_bonus_config_disables_speed_hook()
    {
        Game g = Harness();
        g.C.Training.SyntheticDataLab.TrainingSpeedBonusPermille = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState m = g.Train("mini", 0);
        Game plain = Harness();
        plain.Do(Command.SetTrainingDataSource(plain.Id, Synthetic));
        ModelState p = plain.Train("mini", 0);
        g.Weeks(4);
        plain.Weeks(4);
        Assert.Equal(p.ProgressCompute, m.ProgressCompute);
    }

    [Fact]
    public void Bankruptcy_turns_speed_bonus_off()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Me.Bankrupt = true;
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Before_harnesses_era_no_speed_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024);
        g.WithStarterLab(gpus: 24);
        g.Me.Rooms.Add(new RoomState { TypeId = Room, X = 0, Y = 0, Width = 4, Height = 4, Id = g.State.TakeId() });
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Per_run_bonus_only_on_synthetic_source()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.C.Training.DataSource("synthetic").OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState syn = g.Train("mini", 0);
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState lic = g.Train("mini", 0);
        g.Weeks(4);
        Assert.True(syn.ProgressCompute > lic.ProgressCompute);
    }

    [Fact]
    public void Save_load_preserves_synthetic_speed_progress()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState m = g.Train("mini", 0);
        g.Weeks(3);
        long progress = m.ProgressCompute;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(2);
        Assert.True(loaded.Me.Model(m.Id).ProgressCompute > progress);
    }

    [Fact]
    public void Speed_permille_unchanged_lab_wide()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Game bare = Harness();
        Assert.Equal(speed, TrainingSystem.SpeedPermille(bare.C, bare.State, bare.Me));
    }

    [Fact]
    public void Scrape_distill_unaffected_by_synthetic_speed()
    {
        Game g = Harness(2028);
        g.C.Training.DataSource("scrape").OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 0);
        g.Weeks(4);
        long beforeRoom = m.ProgressCompute;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, "scrape"));
        m.ProgressCompute = 0;
        g.Weeks(4);
        Assert.Equal(beforeRoom, m.ProgressCompute);
    }

    [Fact]
    public void Config_negative_speed_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Training.SyntheticDataLab.TrainingSpeedBonusPermille = -1));
    }

    [Fact]
    public void Both_runs_synthetic_gets_bonus_when_lab_active()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState a = g.Train("mini", 0);
        ModelState b = g.Train("mini", 0);
        g.Weeks(3);
        Assert.True(a.ProgressCompute > 0);
        Assert.True(b.ProgressCompute > 0);
    }
}