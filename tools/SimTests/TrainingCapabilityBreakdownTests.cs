using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TrainingCapabilityBreakdownTests
{
    [Fact]
    public void Trained_model_capability_parts_sum_to_capability()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.Equal(parts.FromBase + parts.FromTech + parts.FromCarryover + parts.FromLuck + parts.FromDataSource, parts.Total);
        Assert.Equal(0, parts.FromDataSource);
    }

    [Fact]
    public void Licensed_source_shows_positive_data_source_part()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.True(parts.FromDataSource > 0);
        Assert.Equal("licensed", model.DataSourceId);
    }

    [Fact]
    public void Synthetic_source_can_show_negative_data_source_part()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.True(parts.FromDataSource < 0);
    }

    [Fact]
    public void Untrained_model_returns_empty_breakdown()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 100);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(0, parts.Total);
    }

    [Fact]
    public void Breakdown_survives_save_load()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 120);
        g.UntilTrained(model);
        var before = DisplayQueries.TrainedCapabilityParts(model);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState again = loaded.Me.Model(model.Id);
        var after = DisplayQueries.TrainedCapabilityParts(again);
        Assert.Equal(before.Total, after.Total);
        Assert.Equal(before.FromDataSource, after.FromDataSource);
    }
}