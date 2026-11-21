using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q91: device workshop cuts training prep cost when gated.</summary>
public class DeviceWorkshopPrepCostTests
{
    const string Workshop = "device-workshop";
    const string Devices = "devices";

    static Game HarnessLab(int year = 2028) => HarnessLab(year, null);

    static Game HarnessLab(int year, System.Action<GameConfig> tweak)
    {
        return ExpansionTestSupport.SoloAtYear(year, tweak: tweak).WithStarterLab(gpus: 24);
    }

    [Fact]
    public void Shipped_workshop_prep_cut_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(100, c.Market.DeviceWorkshop.PrepCostCutPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_cut_without_room_or_release()
    {
        Game g = HarnessLab();
        Assert.Equal(0, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Workshop_plus_release_enables_prep_cut()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(g.C.Market.DeviceWorkshop.PrepCostCutPermille,
            DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Start_training_cost_lower_with_workshop_gate()
    {
        Game bare = HarnessLab();
        long cashBare = bare.Me.Cash;
        bare.Do(Command.StartTraining(bare.Id, "mini", 100));
        long spentBare = cashBare - bare.Me.Cash;
        Game wired = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(wired, Workshop);
        ExpansionTestSupport.AddReleasedModel(wired, 120_000, Devices);
        long cashWired = wired.Me.Cash;
        wired.Do(Command.StartTraining(wired.Id, "mini", 100));
        long spentWired = cashWired - wired.Me.Cash;
        Assert.True(spentWired < spentBare);
    }

    [Fact]
    public void Recipe_start_respects_prep_cut()
    {
        Game g = HarnessLab(2028, c => TrainingQueries.Recipe(c, "fast-mini").OpensYear = 0);
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Game bare = HarnessLab(2028, c => TrainingQueries.Recipe(c, "fast-mini").OpensYear = 0);
        long cashBare = bare.Me.Cash;
        bare.Do(Command.StartTrainingRecipe(bare.Id, "fast-mini"));
        long cashWired = g.Me.Cash;
        g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini"));
        Assert.True(cashBare - bare.Me.Cash > cashWired - g.Me.Cash);
    }

    [Fact]
    public void Zero_cut_config_disables_hook()
    {
        Game g = HarnessLab();
        g.C.Market.DeviceWorkshop.PrepCostCutPermille = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Bankruptcy_turns_prep_cut_off()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Me.Bankrupt = true;
        Assert.Equal(0, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Prep_cut_matches_workshop_config_when_active()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(g.C.Market.DeviceWorkshop.PrepCostCutPermille, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Cut_applies_after_data_source_prep_multiplier()
    {
        Game g = HarnessLab();
        g.C.Training.DataSource("scrape").OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ExpansionTestSupport.EnsureDataLicense(g, "scrape");
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Game plain = HarnessLab();
        plain.C.Training.DataSource("scrape").OpensYear = 0;
        plain.Do(Command.SetTrainingDataSource(plain.Id, "scrape"));
        ExpansionTestSupport.EnsureDataLicense(plain, "scrape");
        long a = plain.Me.Cash;
        plain.Do(Command.StartTraining(plain.Id, "mini", 50));
        long b = g.Me.Cash;
        g.Do(Command.StartTraining(g.Id, "mini", 50));
        Assert.True(a - plain.Me.Cash > b - g.Me.Cash);
    }

    [Fact]
    public void Save_load_preserves_prep_cut_behavior()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        int cut = DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(cut, DeviceWorkshopSystem.PrepCostCutPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Plan_lab_never_gets_player_workshop_cut()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        Assert.Equal(0, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, plan));
    }

    [Fact]
    public void Release_in_other_sector_no_prep_cut()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 100_000, "game-ai");
        Assert.Equal(0, DeviceWorkshopSystem.PrepCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Negative_prep_cut_rejected_by_validator()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Market.DeviceWorkshop.PrepCostCutPermille = -5));
    }
}