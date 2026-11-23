using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class PublicMetricsEraLoadTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(85, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PublicMetrics.MinEra = 0;
            c.PublicMetrics.PublishCost = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Publish_emits_era_boost_when_enabled()
    {
        Game g = Lab(c =>
        {
            c.PublicMetricsEraLoad.Enabled = 1;
            c.PublicMetricsEraLoad.MinEra = 0;
            c.PublicMetricsEraLoad.TrustBoostPermille = 20;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsEraBoost);
    }
}