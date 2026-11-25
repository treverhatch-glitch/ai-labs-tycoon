using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class IntegrationsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Selling(Game g, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        return m;
    }

    static ModelState Pair(Game g, out ModelState next)
    {
        ModelState old = Selling(g);
        next = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, old.Id));
        g.Do(Command.OpenApi(g.Id, next.Id));
        return old;
    }

    [Fact]
    public void An_api_needs_a_model_on_sale_and_a_free_slot()
    {
        Game g = Lab(c => c.Integrations.MaxOpen = 1);
        ModelState idle = g.Train("mini", 0);
        g.UntilTrained(idle);
        g.Refused(Command.OpenApi(g.Id, idle.Id), CommandError.WrongModelStatus);
        g.Refused(Command.OpenApi(g.Id, 999), CommandError.UnknownId);
        ModelState a = Selling(g);
        ModelState b = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, a.Id));
        g.Refused(Command.OpenApi(g.Id, a.Id), CommandError.InvalidArgument);
        g.Refused(Command.OpenApi(g.Id, b.Id), CommandError.InvalidArgument);
        Assert.Equal(1, a.ApiState);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Integrations_grow_on_an_open_api_and_not_on_a_closed_one()
    {
        Game g = Lab();
        ModelState a = Selling(g);
        ModelState b = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, a.Id));
        g.Weeks(10);
        Assert.True(a.Integrations >= 10 * g.C.Integrations.SeedPerWeek);
        Assert.Equal(0, b.Integrations);
        int before = a.Integrations;
        g.Weeks(10);
        Assert.True(a.Integrations > before + 10 * g.C.Integrations.SeedPerWeek - 1);
    }

    [Fact]
    public void Integrations_lift_revenue_and_cost_upkeep_that_grows_with_them()
    {
        Game plain = Lab();
        Selling(plain);
        plain.Weeks(30);
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.OpenApi(g.Id, m.Id));
        g.Weeks(30);
        Assert.True(g.Me.Ledger.Lifetime.Revenue > plain.Me.Ledger.Lifetime.Revenue);
        Assert.True(g.Me.Ledger.Lifetime.Salaries < plain.Me.Ledger.Lifetime.Salaries);
        Assert.True(Integrations.UpkeepPermille(g.C, m) > g.C.Integrations.UpkeepBasePermille);
        m.Integrations = 100000;
        Assert.Equal(g.C.Integrations.LiftCapPermille, Integrations.LiftPermille(g.C, m));
    }

    [Fact]
    public void A_newer_model_does_not_inherit_integrations()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        g.Do(Command.OpenApi(g.Id, old.Id));
        g.Weeks(20);
        int had = old.Integrations;
        Assert.True(had > 0);
        ModelState next = Selling(g);
        Assert.Equal(0, next.Integrations);
        Assert.Equal(0, next.ApiState);
    }

    [Fact]
    public void A_deprecation_needs_an_open_api_a_sane_deadline_and_a_successor_with_an_open_api()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        ModelState next = Selling(g, "game-bots");
        g.Refused(Command.DeprecateApi(g.Id, old.Id, 0, 12), CommandError.InvalidArgument);
        g.Do(Command.OpenApi(g.Id, old.Id));
        g.Refused(Command.DeprecateApi(g.Id, old.Id, 0, 2), CommandError.InvalidArgument);
        g.Refused(Command.DeprecateApi(g.Id, old.Id, 0, 500), CommandError.InvalidArgument);
        g.Refused(Command.DeprecateApi(g.Id, old.Id, next.Id, 12), CommandError.InvalidArgument);
        g.Refused(Command.DeprecateApi(g.Id, old.Id, old.Id, 12), CommandError.InvalidArgument);
        g.Refused(Command.DeprecateApi(g.Id, old.Id, 999, 12), CommandError.UnknownId);
        g.Do(Command.OpenApi(g.Id, next.Id));
        g.Do(Command.DeprecateApi(g.Id, old.Id, next.Id, 12));
        Assert.Equal(2, old.ApiState);
        Assert.Equal(12, old.ApiLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_long_deadline_moves_nearly_everyone_and_loses_a_share_on_the_way()
    {
        Game g = Lab();
        ModelState next;
        ModelState old = Pair(g, out next);
        g.Weeks(40);
        int had = old.Integrations;
        int nextHad = next.Integrations;
        Assert.True(had > 20);
        g.Do(Command.DeprecateApi(g.Id, old.Id, next.Id, 52));
        g.Weeks(52);
        Assert.Equal(0, old.ApiState);
        Assert.Equal(0, old.Integrations);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ApiEnded && e.Pick == 1);
        Assert.True(g.Me.ApiMoved > 0);
        Assert.True(g.Me.ApiLost > 0);
        Assert.True(g.Me.ApiLost < g.Me.ApiMoved);
        Assert.True(next.Integrations > nextHad);
    }

    [Fact]
    public void A_short_deadline_strands_integrations_and_costs_trust()
    {
        Game g = Lab();
        ModelState next;
        ModelState old = Pair(g, out next);
        g.Weeks(60);
        int trust = TrustSystem.Of(g.C, g.Me, "game-ai");
        int had = old.Integrations;
        Assert.True(had > 100);
        g.Do(Command.DeprecateApi(g.Id, old.Id, next.Id, g.C.Integrations.MinWeeks));
        g.Weeks(g.C.Integrations.MinWeeks);
        Assert.Equal(0, old.ApiState);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ApiEnded && e.Pick == 0 && e.Amount > 0);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") < trust);
        Assert.True(g.Me.ApiLost > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void With_no_successor_everything_is_stranded_at_the_deadline()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        g.Do(Command.OpenApi(g.Id, old.Id));
        g.Weeks(30);
        int had = old.Integrations;
        g.Do(Command.DeprecateApi(g.Id, old.Id, 0, 4));
        g.Weeks(4);
        Assert.Equal(had, g.Me.ApiLost);
        Assert.Equal(0, g.Me.ApiMoved);
    }

    [Fact]
    public void A_deprecation_can_be_called_off_and_the_switch_and_older_saves_are_handled()
    {
        Game g = Lab();
        ModelState next;
        ModelState old = Pair(g, out next);
        g.Weeks(20);
        g.Do(Command.DeprecateApi(g.Id, old.Id, next.Id, 20));
        g.Weeks(3);
        g.Do(Command.CancelDeprecation(g.Id, old.Id));
        Assert.Equal(1, old.ApiState);
        Assert.Equal(0, old.ApiLeft);
        g.Refused(Command.CancelDeprecation(g.Id, old.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 109, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.ApiState + x.Integrations));
        Game off = Lab(c => c.Integrations.Enabled = 0);
        ModelState om = Selling(off);
        off.Refused(Command.OpenApi(off.Id, om.Id), CommandError.InvalidArgument);
    }
}
