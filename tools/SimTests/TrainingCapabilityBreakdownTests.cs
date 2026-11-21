using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TrainingCapabilityBreakdownTests
{
    [Fact]
    public void Trained_model_capability_parts_sum_to_stored_capability()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.Equal(parts.FromBase + parts.FromTech + parts.FromCarryover + parts.FromLuck + parts.FromDataSource + parts.FromExpansion, parts.Total);
        Assert.Equal(0, parts.FromDataSource);
    }

    [Fact]
    public void Licensed_data_source_shows_positive_capability_part()
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
    public void Synthetic_data_source_shows_negative_capability_part()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.True(parts.FromDataSource < 0);
        Assert.Equal(model.Capability, parts.Total);
    }

    [Fact]
    public void Training_in_progress_returns_empty_breakdown()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 100);
        Assert.Equal(0, DisplayQueries.TrainedCapabilityParts(model).Total);
    }

    [Fact]
    public void Released_model_breakdown_matches_trained_totals()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 120);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        Assert.True(parts.FromBase > 0);
    }

    [Fact]
    public void Second_run_carryover_appears_in_breakdown_parts()
    {
        var g = Game.Solo().WithStarterLab(gpus: 24);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 2_000_000);
        ModelState first = g.Train("mini", 200);
        g.UntilTrained(first);
        g.Release(first, "game-ai");
        ModelState second = g.Train("mini", 200);
        g.UntilTrained(second);
        var parts = DisplayQueries.TrainedCapabilityParts(second);
        Assert.True(parts.FromCarryover > 0);
        Assert.Equal(second.Capability, parts.Total);
    }

    [Fact]
    public void Licensed_prep_cost_and_capability_boost_both_apply()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        long cash = g.Me.Cash;
        ModelState model = g.Train("mini", 100);
        long prepSpent = cash - g.Me.Cash;
        long expectedPrep = MathI.MulPermille(EraSystem.RunPrepCost(g.C, g.C.Line("mini"), g.State.Tick), 1300);
        Assert.Equal(expectedPrep, prepSpent);
        g.UntilTrained(model);
        Assert.True(DisplayQueries.TrainedCapabilityParts(model).FromDataSource > 0);
    }

    [Fact]
    public void Quality_display_uses_final_capability_after_data_source_scaling()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 150);
        g.UntilTrained(model);
        var sector = g.C.Sector("game-ai");
        long quality = DisplayQueries.Quality(g.C, sector, model, g.Tick);
        Assert.True(quality >= MathI.MulPermille(model.CapBase, sector.CapabilityWeightPermille));
    }

    [Fact]
    public void Bankrupt_lab_keeps_breakdown_on_existing_trained_model()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Me.Bankrupt = true;
        var parts = DisplayQueries.TrainedCapabilityParts(model);
        Assert.Equal(model.Capability, parts.Total);
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.LabBankrupt);
    }

    [Fact]
    public void Breakdown_survives_save_load_and_continue_stepping()
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
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 4);
    }
}