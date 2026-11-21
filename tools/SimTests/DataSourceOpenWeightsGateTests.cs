using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Data sources that cannot ship open weights (synthetic mix) vs sources that can.</summary>
public class DataSourceOpenWeightsGateTests
{
    static Game TrainedOn(string sourceId)
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, sourceId));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        return g;
    }

    [Fact]
    public void Synthetic_model_refuses_open_weights()
    {
        Game g = TrainedOn("synthetic");
        ModelState model = g.Me.Models[0];
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.DataSourceBlocksOpenWeights);
        Assert.False(model.OpenWeights);
    }

    [Fact]
    public void Internal_and_licensed_may_open_weights()
    {
        foreach (string source in new[] { "internal", "licensed" })
        {
            Game g = TrainedOn(source);
            ModelState model = g.Me.Models[0];
            g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
            Assert.True(model.OpenWeights);
        }
    }

    [Fact]
    public void Closing_open_weights_still_allowed_on_synthetic()
    {
        Game g = TrainedOn("synthetic");
        ModelState model = g.Me.Models[0];
        g.Do(Command.SetOpenWeights(g.Id, model.Id, false));
        Assert.False(model.OpenWeights);
    }

    [Fact]
    public void Gate_applies_only_when_turning_on()
    {
        Game g = TrainedOn("synthetic");
        ModelState model = g.Me.Models[0];
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.DataSourceBlocksOpenWeights);
        g.Do(Command.SetOpenWeights(g.Id, model.Id, false));
    }

    [Fact]
    public void Untrained_model_still_needs_release_before_open_weights()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 100);
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.WrongModelStatus);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void Config_flag_blocks_custom_sources()
    {
        Game g = Game.Solo(tweak: c => c.Training.DataSource("internal").BlocksOpenWeights = true);
        g.WithStarterLab(gpus: 16);
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.DataSourceBlocksOpenWeights);
    }

    [Fact]
    public void Open_weights_on_licensed_still_changes_revenue()
    {
        Game g = TrainedOn("licensed");
        ModelState model = g.Me.Models[0];
        var entry = g.State.Sector("game-ai").Entry(g.Id);
        g.Weeks(4);
        long before = entry.LastRevenue;
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Weeks(4);
        Assert.True(entry.LastRevenue < before || before > 0);
    }

    [Fact]
    public void Gate_survives_save_load()
    {
        Game g = TrainedOn("synthetic");
        ModelState model = g.Me.Models[0];
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState m = loaded.Me.Model(model.Id);
        loaded.Refused(Command.SetOpenWeights(loaded.Id, m.Id, true), CommandError.DataSourceBlocksOpenWeights);
    }

    [Fact]
    public void Switching_lab_default_does_not_retrofit_finished_model()
    {
        Game g = TrainedOn("synthetic");
        ModelState model = g.Me.Models[0];
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.DataSourceBlocksOpenWeights);
        Assert.Equal("synthetic", model.DataSourceId);
    }
}