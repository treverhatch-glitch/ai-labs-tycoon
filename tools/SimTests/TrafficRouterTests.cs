using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TrafficRouterTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(9, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.TrafficRouter.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Sell(Game g, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.Capability = 800;
        m.Alignment = 900;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        return m;
    }

    [Fact]
    public void Route_shares_default_to_even_split()
    {
        Game g = Lab();
        ModelState a = Sell(g);
        ModelState b = g.Train("mini", 0);
        g.UntilTrained(b);
        g.Do(Command.ReleaseModel(g.Id, b.Id, 0, "game-bots"));
        Assert.Equal(MathI.Ppm / 2, TrafficRouter.SharePpm(g.C, g.Me, a));
    }

    [Fact]
    public void Set_route_share_is_checked()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Refused(Command.SetRouteShare(g.Id, 999, 100), CommandError.UnknownId);
        g.Refused(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm + 1), CommandError.InvalidArgument);
        Game bare = Lab();
        ModelState tm = bare.Train("mini", 0);
        bare.UntilTrained(tm);
        bare.Refused(Command.SetRouteShare(bare.Id, tm.Id, 100), CommandError.WrongModelStatus);
        Game off = Lab(c => c.TrafficRouter.Enabled = 0);
        off.Refused(Command.SetRouteShare(off.Id, Sell(off).Id, 100), CommandError.InvalidArgument);
    }

    [Fact]
    public void Route_ppm_cannot_exceed_all_traffic()
    {
        Game g = Lab();
        ModelState a = Sell(g, "game-ai");
        ModelState b = Sell(g, "game-bots");
        g.Do(Command.SetRouteShare(g.Id, a.Id, MathI.Ppm));
        g.Refused(Command.SetRouteShare(g.Id, b.Id, 1), CommandError.InvalidArgument);
        g.Do(Command.SetRouteShare(g.Id, a.Id, MathI.Ppm / 2));
        g.Do(Command.SetRouteShare(g.Id, b.Id, MathI.Ppm / 2));
        Assert.Equal(MathI.Ppm, TrafficRouter.RouteTotal(g.Me));
    }

    [Fact]
    public void Skewed_routes_are_detected()
    {
        Game g = Lab(c => c.TrafficRouter.SkewBar = 600000);
        ModelState a = Sell(g, "game-ai");
        ModelState b = Sell(g, "game-bots");
        Assert.False(TrafficRouter.Skewed(g.C, g.Me));
        g.Do(Command.SetRouteShare(g.Id, a.Id, 700000));
        g.Do(Command.SetRouteShare(g.Id, b.Id, 300000));
        Assert.True(TrafficRouter.Skewed(g.C, g.Me));
    }

    [Fact]
    public void Skewed_under_served_labs_gain_incident_risk()
    {
        Game g = Lab(c => { c.TrafficRouter.SkewBar = 500000; c.TrafficRouter.ServedBelow = 900; c.TrafficRouter.OverloadAdd = 50; });
        ModelState a = Sell(g);
        g.Do(Command.SetRouteShare(g.Id, a.Id, MathI.Ppm));
        g.Me.LastWeek.ServedPermille = 500;
        Assert.Equal(50, TrafficRouter.IncidentAdd(g.C, g.State, g.Me));
        g.Me.LastWeek.ServedPermille = 950;
        Assert.Equal(0, TrafficRouter.IncidentAdd(g.C, g.State, g.Me));
    }

    [Fact]
    public void Setting_a_share_emits_an_event()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.SetRouteShare(g.Id, m.Id, 400000));
        Assert.Equal(400000, m.RoutePpm);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RouteShareSet);
    }

    [Fact]
    public void Only_the_player_lab_sets_routes()
    {
        Game g = Lab();
        g.Refused(Command.SetRouteShare(9999, 1, 100), CommandError.UnknownLab);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_route_ppm()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.SetRouteShare(g.Id, m.Id, 250000));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 132, g.Sim.Save()));
        Assert.All(older.Player.Models, x => Assert.Equal(0, x.RoutePpm));
    }
}