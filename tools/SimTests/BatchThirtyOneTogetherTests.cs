using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyOneTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(161, c =>
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

    static string Home(Game g) => g.Me.HomeBlocId;

    [Fact]
    public void Lobby_cost_era_relief()
    {
        Game g = Lab(c =>
        {
            c.LobbyingEraCooldown.Enabled = 0;
            c.LobbyingCostEraRelief.Enabled = 1;
            c.LobbyingCostEraRelief.MinEra = 0;
            c.LobbyingCostEraRelief.ReliefPermille = 500;
        });
        g.Me.Bloc(Home(g)).Attention = 900;
        long list = Lobbying.Cost(g.C, g.State, g.Me, Lobbying.Quiet);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LobbyingCostEraRelieved);
        Assert.Equal(list / 2, LobbyingCostEraRelief.Cost(g.C, g.State, list));
    }

    [Fact]
    public void Conformity_era_slack_on_bad_metrics()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.Bar = 900;
            c.PublicMetrics.ConformityPush = 10;
            c.PublicMetricsConformityEraSlack.Enabled = 1;
            c.PublicMetricsConformityEraSlack.MinEra = 0;
            c.PublicMetricsConformityEraSlack.SlackPermille = 500;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.CertStage = Conformity.Review;
        m.CertLeft = 4;
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Me.LastWeek.ServedPermille = 400;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(9, m.CertLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsConformityEraSlacked);
    }

    [Fact]
    public void Specialist_branch_era_boost()
    {
        Game g = Lab(c =>
        {
            c.SpecialistLines.MinEra = 0;
            c.SpecialistBranchEraBoost.Enabled = 1;
            c.SpecialistBranchEraBoost.MinEra = 0;
            c.SpecialistBranchEraBoost.TrustBoostPermille = 25;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        int t0 = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") > t0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SpecialistBranchEraBoosted);
    }
}