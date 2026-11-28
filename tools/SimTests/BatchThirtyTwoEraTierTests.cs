using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyTwoEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(172, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        return g;
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.LobbyingGoalEraAmplify.Enabled = 0;
            c.PublicMetricsBarEraSlack.Enabled = 0;
            c.OpenWeightsCloseEraRelief.Enabled = 0;
        });
        int bar = g.C.PublicMetrics.Bar;
        Assert.Equal(bar, PublicMetricsBarEraSlack.EffectiveBar(g.C, g.State));
        Assert.Equal(-50, OpenWeightsCloseEraRelief.TrustLoss(g.C, g.State, -50));
    }

    [Fact]
    public void Save_load_preserves_schema_198round_trip()
    {
        Game g = Lab(c =>
        {
            c.PublicMetricsBarEraSlack.Enabled = 1;
            c.PublicMetricsBarEraSlack.MinEra = 0;
            c.PublicMetricsBarEraSlack.SlackPermille = 100;
            c.PublicMetrics.MinEra = 0;
        });
        g.Me.Cash = 1_000_000_000_000;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        string save = g.Sim.Save();
        Assert.Equal(197, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(197, loaded.State.Schema);
    }
}