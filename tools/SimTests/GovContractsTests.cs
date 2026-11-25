using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class GovContractsTests
{
    static Game Lab(bool released = true, int engineers = 4)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        }).WithStarterLab(researchers: 4, engineers: engineers, safety: 3, sales: 3, gpus: 8);
        if (released)
        {
            ModelState m = g.Train("mini", 300);
            g.UntilTrained(m);
            g.Release(m, "game-ai");
        }
        g.Me.Exposure = 0;
        g.Me.ContractNext = 0;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Offer(Game g, string kind = null)
    {
        if (kind != null) g.Me.OfferSeq = g.C.GovContracts.Kinds.FindIndex(k => k.Id == kind);
        g.Me.ContractNext = g.State.Tick + 1;
        g.Weeks(1);
        Assert.True(GovContracts.Offered(g.Me));
    }

    [Fact]
    public void An_offer_comes_after_the_wait_in_turn_and_lapses_if_nobody_answers()
    {
        Game g = Lab();
        Assert.False(GovContracts.Offered(g.Me));
        g.Weeks(g.C.GovContracts.OfferEveryWeeks + 1);
        Assert.Equal(g.C.GovContracts.Kinds[0].Id, g.Me.OfferKind);
        Assert.Equal(1, Count(g, EventKind.ContractOffered));
        g.Weeks(g.C.GovContracts.OfferWeeks);
        Assert.False(GovContracts.Offered(g.Me));
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.ContractDeclined).Pick);
        Assert.Equal(1, g.Me.OfferSeq);
        g.Weeks(g.C.GovContracts.OfferEveryWeeks + 1);
        Assert.Equal(g.C.GovContracts.Kinds[1].Id, g.Me.OfferKind);
    }

    [Fact]
    public void Nothing_is_offered_to_a_lab_with_nothing_on_sale()
    {
        Game g = Lab(released: false);
        g.Weeks(g.C.GovContracts.OfferEveryWeeks * 2);
        Assert.False(GovContracts.Offered(g.Me));
        Assert.Equal(0, Count(g, EventKind.ContractOffered));
    }

    [Fact]
    public void Declining_ends_the_offer_and_starts_the_wait_again()
    {
        Game g = Lab();
        Offer(g);
        g.Do(Command.DeclineContract(g.Id));
        Assert.False(GovContracts.Offered(g.Me));
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.ContractDeclined).Pick);
        g.Weeks(g.C.GovContracts.OfferEveryWeeks - 2);
        Assert.False(GovContracts.Offered(g.Me));
        g.Refused(Command.DeclineContract(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.AcceptContract(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_cleared_contract_needs_the_security_tier_and_enough_free_engineers()
    {
        Game g = Lab();
        Offer(g, "intelligence");
        g.Refused(Command.AcceptContract(g.Id), CommandError.MissingPrerequisite);
        g.Me.SecurityTier = 2;
        g.Do(Command.Fire(g.Id, "engineer", 2));
        g.Me.Alumni.Clear();
        Assert.Equal(2, g.Me.Staff.Count(p => p.RoleId == "engineer"));
        g.Refused(Command.AcceptContract(g.Id), CommandError.MissingPrerequisite);
        Game h = Lab();
        h.Me.SecurityTier = 2;
        Offer(h, "intelligence");
        h.Do(Command.AcceptContract(h.Id));
        Assert.True(GovContracts.InForce(h.Me));
    }

    [Fact]
    public void Accepting_costs_the_safety_staff_morale_and_reputation_and_keeps_the_last_engineers_away()
    {
        Game g = Lab();
        g.Me.SecurityTier = 2;
        Offer(g, "intelligence");
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        int rep = g.Me.Reputation;
        g.Do(Command.AcceptContract(g.Id));
        ContractKindConfig kind = g.C.GovContracts.Kind("intelligence");
        Assert.All(g.Me.Staff.Where(p => p.RoleId == "safety"), p => Assert.Equal(800 - kind.Morale, p.Morale));
        Assert.All(g.Me.Staff.Where(p => p.RoleId == "researcher"), p => Assert.Equal(800, p.Morale));
        Assert.Equal(rep + kind.AcceptReputation, g.Me.Reputation);
        Assert.False(GovContracts.Offered(g.Me));
        Assert.Equal(kind.Weeks, g.Me.ContractDue - g.Me.ContractStart);
        var engineers = g.Me.Staff.Where(p => p.RoleId == "engineer").ToList();
        Assert.Equal(kind.People, engineers.Count(p => GovContracts.Away(g.C, g.Me, p)));
        Assert.True(GovContracts.Away(g.C, g.Me, engineers.Last()));
        Assert.False(GovContracts.Away(g.C, g.Me, engineers.First()));
        g.Refused(Command.AcceptContract(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_contract_paid_in_instalments_completes_and_adds_reputation()
    {
        Game g = Lab();
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        ContractKindConfig kind = g.C.GovContracts.Kind("public-services");
        g.Weeks(kind.Weeks / g.C.GovContracts.Installments);
        Assert.Equal(1, g.Me.ContractPaid);
        Assert.Equal(kind.Pay / g.C.GovContracts.Installments, g.State.Events.Single(e => e.Kind == EventKind.ContractInstalment).Amount);
        g.Weeks(kind.Weeks);
        Assert.False(GovContracts.InForce(g.Me));
        Assert.Equal(g.C.GovContracts.Installments, Count(g, EventKind.ContractInstalment));
        Assert.Equal(GovContracts.Done, g.State.Events.Last(e => e.Kind == EventKind.ContractEnded).Pick);
        Assert.Equal(kind.Pay / g.C.GovContracts.Installments * g.C.GovContracts.Installments, g.State.Events.Where(e => e.Kind == EventKind.ContractInstalment).Sum(e => e.Amount));
    }

    [Fact]
    public void Missing_instalments_ends_the_contract_with_a_penalty_and_lost_reputation()
    {
        Game g = Lab();
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        ContractKindConfig kind = g.C.GovContracts.Kind("public-services");
        g.Do(Command.Fire(g.Id, "engineer", 3));
        g.Me.Alumni.Clear();
        long fines = g.Me.Ledger.Lifetime.Fines;
        int rep = g.Me.Reputation;
        g.Weeks(kind.Weeks);
        Assert.False(GovContracts.InForce(g.Me));
        Assert.Equal(0, g.Me.ContractPaid);
        GameEvent ended = g.State.Events.Last(e => e.Kind == EventKind.ContractEnded);
        Assert.Equal(GovContracts.Ended, ended.Pick);
        Assert.Equal(GovContracts.Penalty(g.C, kind), ended.Amount);
        Assert.Equal(fines - GovContracts.Penalty(g.C, kind), g.Me.Ledger.Lifetime.Fines);
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(g.C.GovContracts.MissLimit, g.State.Events.Count(e => e.Kind == EventKind.ContractInstalment && e.Pick == 0));
    }

    [Fact]
    public void Walking_away_costs_the_penalty_and_more_reputation_than_being_let_go()
    {
        Game g = Lab();
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        ContractKindConfig kind = g.C.GovContracts.Kind("public-services");
        int rep = g.Me.Reputation;
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Do(Command.AbandonContract(g.Id));
        Assert.False(GovContracts.InForce(g.Me));
        Assert.Equal(fines - GovContracts.Penalty(g.C, kind), g.Me.Ledger.Lifetime.Fines);
        Assert.True(rep - g.Me.Reputation > 0);
        Assert.Equal(GovContracts.Abandoned, g.State.Events.Last(e => e.Kind == EventKind.ContractEnded).Pick);
        g.Refused(Command.AbandonContract(g.Id), CommandError.InvalidArgument);
        Assert.Equal(g.State.Tick + g.C.GovContracts.OfferEveryWeeks, g.Me.ContractNext);
    }

    [Fact]
    public void A_leaked_breach_ends_a_cleared_contract_but_not_an_open_one()
    {
        Game cleared = Lab();
        cleared.Me.SecurityTier = 2;
        Offer(cleared, "intelligence");
        cleared.Do(Command.AcceptContract(cleared.Id));
        Game open = Lab();
        Offer(open, "public-services");
        open.Do(Command.AcceptContract(open.Id));
        foreach (Game g in new[] { cleared, open })
        {
            g.C.Security.ReliefPerTier = 0;
            g.Me.Exposure = g.C.Security.Threshold - 1;
            g.Weeks(1 + g.C.Security.AnswerWeeks);
            Assert.Equal(1, Count(g, EventKind.BreachLeaked));
        }
        Assert.False(GovContracts.InForce(cleared.Me));
        Assert.Equal(GovContracts.Leaked, cleared.State.Events.Last(e => e.Kind == EventKind.ContractEnded).Pick);
        Assert.True(GovContracts.InForce(open.Me));
    }

    [Fact]
    public void People_on_a_contract_cannot_go_on_sabbatical_and_leave_the_desk_fewer_engineers()
    {
        Game g = Lab();
        int most = SupportDesk.MostPeople(g.C, g.Me);
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        Assert.Equal(most - 2, SupportDesk.MostPeople(g.C, g.Me));
        var engineers = g.Me.Staff.Where(p => p.RoleId == "engineer").ToList();
        g.Refused(Command.SendOnSabbatical(g.Id, engineers.Last().Id), CommandError.InvalidArgument);
        g.Do(Command.SendOnSabbatical(g.Id, engineers.First().Id));
        Assert.Equal(1, Sabbatical.Away(g.Me));
        g.Refused(Command.StaffSupportDesk(g.Id, 99), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_contract_in_force_saves_loads_and_plays_on_the_same_and_old_saves_have_none()
    {
        Game g = Lab();
        g.Me.SecurityTier = 2;
        Offer(g, "intelligence");
        g.Do(Command.AcceptContract(g.Id));
        g.Weeks(10);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 82, g.Sim.Save());
        Assert.DoesNotContain("contractKind", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal("", lab.ContractKind);
        Assert.Equal("", lab.OfferKind);
        Assert.Equal(0, lab.ContractNext);
    }
}
