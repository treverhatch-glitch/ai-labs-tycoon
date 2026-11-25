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

public class PledgesTests
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

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static ModelState Second(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    [Fact]
    public void A_pledge_needs_a_model_on_sale_a_sensible_term_and_a_free_slot()
    {
        Game g = Lab();
        ModelState idle = g.Train("mini", 0);
        g.UntilTrained(idle);
        g.Refused(Command.PledgeSupport(g.Id, idle.Id, 52), CommandError.WrongModelStatus);
        g.Refused(Command.PledgeSupport(g.Id, 999, 52), CommandError.UnknownId);
        ModelState m = Selling(g);
        g.Refused(Command.PledgeSupport(g.Id, m.Id, 10), CommandError.InvalidArgument);
        g.Refused(Command.PledgeSupport(g.Id, m.Id, 500), CommandError.InvalidArgument);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        Assert.Equal(52, m.LtsLeft);
        g.Refused(Command.PledgeSupport(g.Id, m.Id, 52), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_slots_are_limited()
    {
        Game g = Lab(c => c.Pledges.MaxPledges = 1);
        ModelState a = Selling(g);
        ModelState b = Second(g);
        g.Do(Command.ReleaseModel(g.Id, b.Id, 0, "game-bots"));
        g.Do(Command.PledgeSupport(g.Id, a.Id, 52));
        g.Refused(Command.PledgeSupport(g.Id, b.Id, 52), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_pledged_model_earns_a_lift_that_grows_with_the_weeks_left_and_costs_support_each_week()
    {
        Game plain = Lab();
        Selling(plain);
        plain.Weeks(12);
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 104));
        Assert.Equal(160, Pledges.LiftPermille(g.C, m));
        g.Weeks(12);
        Assert.Equal(92, m.LtsLeft);
        Assert.Equal(120, Pledges.LiftPermille(g.C, m));
        Assert.True(g.Me.Ledger.Lifetime.Revenue > plain.Me.Ledger.Lifetime.Revenue);
        Assert.True(g.Me.Ledger.Lifetime.Salaries < plain.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void A_short_pledge_gives_less_lift_than_it_costs()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 26));
        Assert.Equal(40, Pledges.LiftPermille(g.C, m));
        Assert.True(Pledges.LiftPermille(g.C, m) < g.C.Pledges.SupportPermille);
    }

    [Fact]
    public void A_pledged_model_cannot_be_replaced_in_its_sector_or_given_notice()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        ModelState next = Second(g);
        g.Do(Command.PledgeSupport(g.Id, old.Id, 52));
        g.Refused(Command.ReleaseModel(g.Id, next.Id, 0, "game-ai"), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceSunset(g.Id, old.Id, 13), CommandError.InvalidArgument);
        g.Do(Command.ReleaseModel(g.Id, next.Id, 0, "game-bots"));
        Assert.Contains("game-ai", old.Sectors);
        Assert.Equal(old.Id, g.State.Sector("game-ai").Entry(g.Id).ModelId);
    }

    [Fact]
    public void A_notice_blocks_a_pledge()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        ModelState next = Second(g);
        next.DesignerId = old.Id;
        g.Do(Command.ReleaseModel(g.Id, next.Id, 0, "game-bots"));
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 13));
        g.Refused(Command.PledgeSupport(g.Id, old.Id, 52), CommandError.InvalidArgument);
    }

    [Fact]
    public void Breaking_a_pledge_costs_a_fee_trust_and_reputation_and_frees_the_sector()
    {
        Game g = Lab();
        ModelState old = Selling(g);
        ModelState next = Second(g);
        g.Weeks(8);
        g.Do(Command.PledgeSupport(g.Id, old.Id, 52));
        long fee = Pledges.BreachFee(g.C, g.State, g.Me, old);
        Assert.True(fee > 0);
        int trust = TrustSystem.Of(g.C, g.Me, "game-ai");
        long rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        g.Do(Command.BreakSupportPledge(g.Id, old.Id));
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") < trust);
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(0, old.LtsLeft);
        Assert.Equal(1, g.Me.LtsBroken);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SupportPledgeEnded && e.Pick == 0);
        g.Refused(Command.BreakSupportPledge(g.Id, old.Id), CommandError.InvalidArgument);
        g.Do(Command.ReleaseModel(g.Id, next.Id, 0, "game-ai"));
    }

    [Fact]
    public void A_pledge_kept_to_its_end_earns_trust()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 26));
        int trust = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Weeks(26);
        Assert.Equal(0, m.LtsLeft);
        Assert.Equal(1, g.Me.LtsKept);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") >= trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SupportPledgeEnded && e.Pick == 1);
    }

    [Fact]
    public void A_pledged_model_that_leaves_sale_counts_as_a_breach()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        m.Status = ModelStatus.Retired;
        g.Weeks(1);
        Assert.Equal(0, m.LtsLeft);
        Assert.Equal(1, g.Me.LtsBroken);
    }

    [Fact]
    public void Rivals_the_switch_and_older_saves_are_handled()
    {
        Game off = Lab(c => c.Pledges.Enabled = 0);
        ModelState om = Selling(off);
        off.Refused(Command.PledgeSupport(off.Id, om.Id, 52), CommandError.InvalidArgument);
        Game s = Lab();
        ModelState m = Selling(s);
        s.Do(Command.PledgeSupport(s.Id, m.Id, 52));
        s.Weeks(4);
        Assert.Empty(SaveGame.Problems(s.C, s.State));
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        Simulation old = Simulation.Load(s.C, SaveSchemaPretend.At(s.C, 108, s.Sim.Save()));
        Assert.All(old.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.LtsLeft));
        Assert.Empty(SaveGame.Problems(s.C, old.State));
    }
}
