using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyOneEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(162, c =>
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
            c.LobbyingCostEraRelief.Enabled = 0;
            c.PublicMetricsConformityEraSlack.Enabled = 0;
            c.SpecialistBranchEraBoost.Enabled = 0;
        });
        Assert.Equal(1000L, LobbyingCostEraRelief.Cost(g.C, g.State, 1000));
        Assert.Equal(10, PublicMetricsConformityEraSlack.ConformityPush(g.C, g.State, 10));
    }

    [Fact]
    public void Save_load_preserves_schema_195_round_trip()
    {
        Game g = Lab(c =>
        {
            c.LobbyingCostEraRelief.Enabled = 1;
            c.LobbyingCostEraRelief.MinEra = 0;
            c.LobbyingCostEraRelief.ReliefPermille = 100;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        });
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 900;
        g.Do(Command.Lobby(g.Id, g.Me.HomeBlocId, Lobbying.Quiet));
        string save = g.Sim.Save();
        Assert.Equal(GameState.CurrentSchema, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Contains(loaded.State.Events, e => e.Kind == EventKind.LobbyingCostEraRelieved);
    }
}