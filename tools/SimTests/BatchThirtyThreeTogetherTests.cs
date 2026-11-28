using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyThreeTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(173, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static string Home(Game g) => g.Me.HomeBlocId;

    [Fact]
    public void Lobby_backfire_era_relief()
    {
        int rep0 = 800;
        Game g = Lab(c =>
        {
            c.Lobbying.BackfirePermille = 1000;
            c.Lobbying.BackfireFloorPermille = 1000;
            c.Lobbying.BackfireReputation = 100;
            c.LobbyingBackfireEraRelief.Enabled = 1;
            c.LobbyingBackfireEraRelief.MinEra = 0;
            c.LobbyingBackfireEraRelief.ReliefPermille = 500;
        });
        g.Me.Reputation = rep0;
        g.Me.Bloc(Home(g)).Attention = 100;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Lobbied && e.Ref.EndsWith("backfired"));
        Assert.True(g.Me.Reputation > rep0 - 100);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LobbyingBackfireEraRelieved);
    }

    [Fact]
    public void Metrics_gain_era_boost()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.Enabled = 1;
            c.PublicMetrics.Bar = 500;
            c.PublicMetrics.TrustGain = 10;
            c.PublicMetricsGainEraBoost.Enabled = 1;
            c.PublicMetricsGainEraBoost.MinEra = 0;
            c.PublicMetricsGainEraBoost.BoostPermille = 500;
            c.PublicMetrics.MinEra = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        int t0 = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Me.LastWeek.ServedPermille = 900;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        int t1 = TrustSystem.Of(g.C, g.Me, "game-ai");
        Assert.True(t1 - t0 >= 15);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsGainEraBoosted);
    }

    [Fact]
    public void Weights_copy_era_slack()
    {
        Game g = Lab(c =>
        {
            c.OpenWeights.CopyTrustLoss = 40;
            c.OpenWeights.CopyChancePermille = 1000;
            c.OpenWeightsCopyEraRelief.Enabled = 0;
            c.OpenWeightsCopyEraSlack.Enabled = 1;
            c.OpenWeightsCopyEraSlack.MinEra = 0;
            c.OpenWeightsCopyEraSlack.SlackPermille = 500;
            c.OpenWeightsEraThrottle.Enabled = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, OpenWeights.Open));
        int t0 = TrustSystem.Of(g.C, g.Me, "game-ai");
        OpenWeights.RunWeek(g.C, g.State, g.Me);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") > t0 - 40);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OpenWeightsCopyEraSlacked);
    }
}