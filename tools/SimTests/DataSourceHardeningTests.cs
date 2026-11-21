using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DataSourceHardeningTests
{
    [Fact]
    public void New_lab_defaults_to_internal_data_source()
    {
        Game g = Game.Solo();
        Assert.Equal("internal", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Licensed_source_raises_prep_cost_on_start()
    {
        Game g = Game.Solo().WithStarterLab();
        long cash = g.Me.Cash;
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 100);
        Assert.Equal("licensed", model.DataSourceId);
        Assert.True(cash - g.Me.Cash > 4000);
    }

    [Fact]
    public void Synthetic_source_lowers_capability_after_training()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState synthetic = g.Train("mini", 100);
        g.UntilTrained(synthetic);
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        ModelState baseline = g.Train("mini", 100);
        g.UntilTrained(baseline);
        Assert.True(synthetic.Capability < baseline.Capability);
    }

    [Fact]
    public void Unknown_data_source_on_set_is_refused()
    {
        Game g = Game.Solo();
        g.Refused(Command.SetTrainingDataSource(g.Id, "dark-web"), CommandError.UnknownId);
    }

    [Fact]
    public void Empty_id_on_set_is_refused()
    {
        Game g = Game.Solo();
        g.Refused(Command.SetTrainingDataSource(g.Id, ""), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_lab_cannot_change_data_source()
    {
        Game g = Game.Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.SetTrainingDataSource(g.Id, "licensed"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Zero_prep_multiplier_still_validates_at_one()
    {
        var c = TestData.Config(cfg => cfg.Training.DataSource("synthetic").PrepCostPermille = 1);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Max_prep_multiplier_is_reflected_in_spend()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        long cash = g.Me.Cash;
        g.Train("mini", 100);
        long spent = cash - g.Me.Cash;
        long expected = MathI.MulPermille(EraSystem.RunPrepCost(g.C, g.C.Line("mini"), g.State.Tick), 1300);
        Assert.Equal(expected, spent);
    }

    [Fact]
    public void Save_load_round_trip_keeps_lab_source_and_model_stamp()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 100);
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        Assert.Equal("licensed", loaded.Me.TrainingDataSourceId);
        Assert.Equal("licensed", loaded.Me.Model(model.Id).DataSourceId);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 6);
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixtures_gain_internal_data_source_fields(string file)
    {
        GameConfig c = TestData.Config();
        Simulation sim = SimHardeningSupport.LoadFixture(c, file);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs)
        {
            Assert.Equal("internal", lab.TrainingDataSourceId);
            foreach (ModelState model in lab.Models)
                Assert.Equal("internal", model.DataSourceId);
        }
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 4);
    }

    [Fact]
    public void Release_and_market_week_still_run_after_licensed_train()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(1);
        Assert.True(g.Me.LastWeek.Revenue >= 0);
    }
}