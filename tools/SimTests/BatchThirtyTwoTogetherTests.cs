using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyTwoTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(171, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.LobbyingCostEraRelief.Enabled = 0;
            c.LobbyingGoalEraAmplify.Enabled = 0;
            c.PublicMetrics.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static string Home(Game g) => g.Me.HomeBlocId;

    [Fact]
    public void Quiet_goal_era_amplify()
    {
        Game g = Lab(c =>
        {
            c.LobbyingGoalEraAmplify.Enabled = 1;
            c.LobbyingGoalEraAmplify.MinEra = 0;
            c.LobbyingGoalEraAmplify.AmplifyPermille = 500;
            c.Lobbying.QuietAttention = 100;
        });
        g.Me.Bloc(Home(g)).Attention = 500;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(350, g.Me.Bloc(Home(g)).Attention);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LobbyingGoalEraAmplified);
    }

    [Fact]
    public void Metrics_bar_era_slack()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.Enabled = 1;
            c.PublicMetrics.Bar = 900;
            c.PublicMetricsBarEraSlack.Enabled = 1;
            c.PublicMetricsBarEraSlack.MinEra = 0;
            c.PublicMetricsBarEraSlack.SlackPermille = 200;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Me.LastWeek.ServedPermille = 800;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(720, PublicMetricsBarEraSlack.EffectiveBar(g.C, g.State));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsBarEraSlacked);
    }

    [Fact]
    public void Close_weights_era_relief()
    {
        Game g = Lab(c =>
        {
            c.OpenWeights.TrustGain = 50;
            c.OpenWeightsCloseEraRelief.Enabled = 1;
            c.OpenWeightsCloseEraRelief.MinEra = 0;
            c.OpenWeightsCloseEraRelief.ReliefPermille = 500;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, OpenWeights.Open));
        int t0 = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, OpenWeights.Closed));
        int t1 = TrustSystem.Of(g.C, g.Me, "game-ai");
        Assert.True(t1 > t0 - 50);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OpenWeightsCloseEraRelieved);
    }
}