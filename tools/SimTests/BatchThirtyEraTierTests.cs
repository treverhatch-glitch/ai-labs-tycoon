using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(152, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        return g;
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.LobbyingEraCooldown.Enabled = 0;
            c.PublicMetricsMissEraRelief.Enabled = 0;
            c.OpenWeightsOpenEraBoost.Enabled = 0;
        });
        Assert.Equal(g.C.Lobbying.CooldownWeeks, LobbyingEraCooldown.EffectiveCooldownWeeks(g.C, g.State));
        Assert.Equal(-10, PublicMetricsMissEraRelief.MissTrust(g.C, g.State, -10));
    }

    [Fact]
    public void Save_load_preserves_schema_191_round_trip()
    {
        Game g = Lab(c =>
        {
            c.LobbyingEraCooldown.Enabled = 1;
            c.LobbyingEraCooldown.MinEra = 0;
            c.LobbyingEraCooldown.CooldownCutWeeks = 2;
        });
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 900;
        g.Do(Command.Lobby(g.Id, g.Me.HomeBlocId, Lobbying.Quiet));
        string save = g.Sim.Save();
        Assert.Equal(194, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(194, loaded.State.Schema);
        Assert.Contains(loaded.State.Events, e => e.Kind == EventKind.LobbyingEraCooldownShortened);
    }
}