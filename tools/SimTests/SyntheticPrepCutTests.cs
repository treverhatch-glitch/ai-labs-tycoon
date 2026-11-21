using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q95: syntheticDataLab prepCostCutPermille and StartTraining cost.</summary>
public class SyntheticPrepCutTests
{
    const string Room = "synthetic-data-lab";
    const string Synthetic = "synthetic";

    static Game Harness(int year = 2028) =>
        ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_synthetic_prep_cut_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(80, c.Training.SyntheticDataLab.PrepCostCutPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_cut_without_room_or_synthetic_mix()
    {
        Game g = Harness();
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Room_and_synthetic_mix_enable_prep_cut()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(g.C.Training.SyntheticDataLab.PrepCostCutPermille,
            SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Start_training_cheaper_with_synthetic_lab_gate()
    {
        Game bare = Harness();
        bare.C.Training.DataSource(Synthetic).OpensYear = 0;
        bare.Do(Command.SetTrainingDataSource(bare.Id, Synthetic));
        long cashBare = bare.Me.Cash;
        bare.Do(Command.StartTraining(bare.Id, "mini", 100));
        long spentBare = cashBare - bare.Me.Cash;
        Game wired = Harness();
        wired.C.Training.DataSource(Synthetic).OpensYear = 0;
        wired.Do(Command.SetTrainingDataSource(wired.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(wired, Room);
        long cashWired = wired.Me.Cash;
        wired.Do(Command.StartTraining(wired.Id, "mini", 100));
        long spentWired = cashWired - wired.Me.Cash;
        Assert.True(spentWired < spentBare);
    }

    [Fact]
    public void Internal_mix_ignores_synthetic_prep_cut()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, "internal"));
    }

    [Fact]
    public void Zero_cut_config_disables_hook()
    {
        Game g = Harness();
        g.C.Training.SyntheticDataLab.PrepCostCutPermille = 0;
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Bankruptcy_turns_prep_cut_off()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Me.Bankrupt = true;
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Before_harnesses_era_no_prep_cut()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024).WithStarterLab(gpus: 24);
        g.Me.Rooms.Add(new RoomState { TypeId = Room, X = 0, Y = 0, Width = 4, Height = 4, Id = g.State.TakeId() });
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Prep_cut_matches_config_when_active()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.True(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
        Assert.Equal(80, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Save_load_preserves_prep_cut_query()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        int cut = SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, Synthetic);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(cut, SyntheticDataLabSystem.PrepCostCutPermille(loaded.C, loaded.State, loaded.Me, Synthetic));
    }

    [Fact]
    public void Scrape_mix_unaffected_by_synthetic_room()
    {
        Game g = Harness();
        g.C.Training.DataSource("scrape").OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(0, SyntheticDataLabSystem.PrepCostCutPermille(g.C, g.State, g.Me, "scrape"));
    }

    [Fact]
    public void Negative_prep_cut_rejected_by_validator()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Training.SyntheticDataLab.PrepCostCutPermille = -3));
    }

    [Fact]
    public void Recipe_start_respects_synthetic_prep_cut()
    {
        Game bare = Harness();
        bare.C.Training.DataSource(Synthetic).OpensYear = 0;
        bare.Do(Command.SetTrainingDataSource(bare.Id, Synthetic));
        long cashBare = bare.Me.Cash;
        bare.Do(Command.StartTraining(bare.Id, "mini", 100));
        Game wired = Harness();
        wired.C.Training.DataSource(Synthetic).OpensYear = 0;
        wired.Do(Command.SetTrainingDataSource(wired.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(wired, Room);
        long cashWired = wired.Me.Cash;
        wired.Do(Command.StartTraining(wired.Id, "mini", 100));
        Assert.True(cashBare - bare.Me.Cash > cashWired - wired.Me.Cash);
    }
}