using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(151, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.PublicMetrics.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static BlocStanding Standing(Game g) => g.Me.Bloc(Home(g));

    static string Home(Game g) => g.Me.HomeBlocId;

    [Fact]
    public void Lobby_cooldown_shortens_after_push()
    {
        Game g = Lab(c =>
        {
            c.LobbyingEraCooldown.Enabled = 1;
            c.LobbyingEraCooldown.MinEra = 0;
            c.LobbyingEraCooldown.CooldownCutWeeks = 5;
            c.Lobbying.CooldownWeeks = 12;
        });
        Standing(g).Attention = 900;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(g.State.Tick + 7, Standing(g).LobbyUntilTick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LobbyingEraCooldownShortened);
    }

    [Fact]
    public void Metrics_miss_era_relief()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.MissTrust = -20;
            c.PublicMetricsMissEraRelief.Enabled = 1;
            c.PublicMetricsMissEraRelief.MinEra = 0;
            c.PublicMetricsMissEraRelief.ReliefPermille = 500;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        PublicMetrics.OnIncident(g.C, g.State, g.Me, m);
        Assert.Equal(1, m.PubMiss);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsMissEraRelieved);
        Assert.Equal(-10, PublicMetricsMissEraRelief.MissTrust(g.C, g.State, -20));
    }

    [Fact]
    public void Open_weights_era_boost_on_open()
    {
        Game g = Lab(c =>
        {
            c.OpenWeightsOpenEraBoost.Enabled = 1;
            c.OpenWeightsOpenEraBoost.MinEra = 0;
            c.OpenWeightsOpenEraBoost.TrustBoostPermille = 30;
            c.OpenWeights.TrustGain = 50;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        int t0 = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, OpenWeights.Open));
        int t1 = TrustSystem.Of(g.C, g.Me, "game-ai");
        Assert.True(t1 - t0 >= 50 + 30);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OpenWeightsOpenEraBoosted);
    }
}