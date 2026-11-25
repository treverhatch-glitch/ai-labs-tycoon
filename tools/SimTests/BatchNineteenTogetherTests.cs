using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q485 combination wiring for batch nineteen systems.</summary>
public class BatchNineteenTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(86, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.DataSources.MinEra = 0;
            c.PublicMetrics.MinEra = 0;
            c.PublicMetrics.PublishCost = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Data_pairing_with_training_quality_hook()
    {
        Game g = Lab(c =>
        {
            c.DataSourceSpecialistPairing.Enabled = 1;
            c.DataSourceSpecialistPairing.MinEra = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "image"));
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        Assert.True(DataSources.QualityPermille(g.C, g.State, g.Me) > DataSources.QualityPermille(g.C, g.Me));
    }

    [Fact]
    public void Metrics_era_boost_with_publish_path()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.Enabled = 1;
            c.PublicMetricsEraLoad.Enabled = 1;
            c.PublicMetricsEraLoad.MinEra = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MetricsPublished);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsEraBoost);
    }
}