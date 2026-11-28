using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyThreeEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(174, c =>
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
            c.LobbyingBackfireEraRelief.Enabled = 0;
            c.PublicMetricsGainEraBoost.Enabled = 0;
            c.OpenWeightsCopyEraSlack.Enabled = 0;
        });
        Assert.Equal(100, LobbyingBackfireEraRelief.ReputationLoss(g.C, g.State, 100));
        Assert.Equal(10, PublicMetricsGainEraBoost.TrustGain(g.C, g.State, 10));
        Assert.Equal(40, OpenWeightsCopyEraSlack.CopyTrustLoss(g.C, g.State, 40));
    }

    [Fact]
    public void Save_load_preserves_schema_201_round_trip()
    {
        Game g = Lab(c =>
        {
            c.LobbyingBackfireEraRelief.Enabled = 1;
            c.LobbyingBackfireEraRelief.MinEra = 0;
            c.LobbyingBackfireEraRelief.ReliefPermille = 100;
        });
        g.Me.Cash = 1_000_000_000_000;
        string save = g.Sim.Save();
        Assert.Equal(201, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(201, loaded.State.Schema);
    }
}