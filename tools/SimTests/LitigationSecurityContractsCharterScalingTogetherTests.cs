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

/// <summary>Where the litigation docket, security, government contracts, the charter and the scaling policy meet.</summary>
public class LitigationSecurityContractsCharterScalingTogetherTests
{
    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        g.Me.Exposure = 0;
        g.Me.ContractNext = 0;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Offer(Game g, string kind)
    {
        g.Me.OfferSeq = g.C.GovContracts.Kinds.FindIndex(k => k.Id == kind);
        g.Me.ContractNext = g.State.Tick + 1;
        g.Weeks(1);
        Assert.True(GovContracts.Offered(g.Me));
    }

    [Fact]
    public void A_contract_that_needs_clearance_needs_the_security_tier_and_a_leaked_breach_ends_it()
    {
        Game g = Lab();
        Offer(g, "intelligence");
        g.Refused(Command.AcceptContract(g.Id), CommandError.MissingPrerequisite);
        g.Me.SecurityTier = 2;
        g.Do(Command.AcceptContract(g.Id));
        Assert.True(GovContracts.InForce(g.Me));
        g.Me.BreachDue = g.State.Tick + 1;
        g.Weeks(1);
        Assert.Equal(1, Count(g, EventKind.BreachLeaked));
        Assert.False(GovContracts.InForce(g.Me));
        Assert.Equal(2, g.State.Events.Last(e => e.Kind == EventKind.ContractEnded).Pick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.CaseFiled && e.Pick == 2);
        Assert.Single(g.Me.Cases, x => x.Kind == Litigation.Privacy);
    }

    [Fact]
    public void A_contract_that_needs_no_clearance_survives_a_leak_and_a_ransom_files_no_claim()
    {
        Game g = Lab();
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        g.Me.BreachDue = g.State.Tick + 1;
        g.Weeks(1);
        Assert.True(GovContracts.InForce(g.Me));
        Assert.Equal(1, Count(g, EventKind.BreachLeaked));
        g.Me.Cases.Clear();
        g.Me.BreachDue = g.State.Tick + 50;
        g.Do(Command.AnswerBreach(g.Id, Security.Ransom));
        Assert.Empty(g.Me.Cases);
        g.Me.BreachDue = g.State.Tick + 50;
        g.Do(Command.AnswerBreach(g.Id, Security.Disclose));
        Assert.Single(g.Me.Cases);
    }

    [Fact]
    public void Contract_engineers_come_off_the_support_desk_and_cannot_go_on_sabbatical()
    {
        Game g = Lab(c => c.Support.Enabled = 1);
        int before = SupportDesk.MostPeople(g.C, g.Me);
        Offer(g, "public-services");
        g.Do(Command.AcceptContract(g.Id));
        int people = g.C.GovContracts.Kind("public-services").People;
        Assert.Equal(before - people, SupportDesk.MostPeople(g.C, g.Me));
        StaffMember onContract = g.Me.Staff.Last(p => p.RoleId == GovContracts.RoleId);
        StaffMember free = g.Me.Staff.First(p => p.RoleId == GovContracts.RoleId);
        Assert.True(GovContracts.Away(g.C, g.Me, onContract));
        Assert.False(GovContracts.Away(g.C, g.Me, free));
        g.Refused(Command.SendOnSabbatical(g.Id, onContract.Id), CommandError.InvalidArgument);
        g.Do(Command.SendOnSabbatical(g.Id, free.Id));
        Assert.Equal(1, Sabbatical.Away(g.Me));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_held_model_is_cleared_by_a_security_tier_however_the_tier_arrives()
    {
        Game g = Lab(c => c.Scaling.SafetyPerTier = 1);
        g.Do(Command.AdoptPolicy(g.Id, 1));
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        m.Capability = ScalingPolicy.Tripwire(g.C, 1) + 10;
        g.Weeks(1);
        Assert.Equal(1, g.Me.PolicyHold);
        g.Me.Exposure = g.C.Security.Threshold;
        g.Weeks(1);
        Assert.True(Security.Open(g.Me));
        g.Do(Command.AnswerBreach(g.Id, Security.Lockdown));
        Assert.Equal(1, g.Me.SecurityTier);
        g.Weeks(1);
        Assert.Equal(0, g.Me.PolicyHold);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.TripwireCleared).Pick);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
    }

    [Fact]
    public void A_ransom_leaves_a_held_model_held_and_the_next_ransom_costs_more()
    {
        Game g = Lab(c => c.Scaling.SafetyPerTier = 1);
        g.Do(Command.AdoptPolicy(g.Id, 1));
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        m.Capability = ScalingPolicy.Tripwire(g.C, 1) + 10;
        g.Weeks(1);
        long first = Security.RansomCost(g.C, g.Me);
        g.Me.BreachDue = g.State.Tick + 50;
        g.Do(Command.AnswerBreach(g.Id, Security.Ransom));
        Assert.True(Security.RansomCost(g.C, g.Me) > first);
        g.Weeks(2);
        Assert.Equal(1, g.Me.PolicyHold);
        Assert.Equal(CommandError.InvalidArgument, g.Try(Command.ReleaseModel(g.Id, m.Id, "game-ai")).Error);
    }

    [Fact]
    public void A_policy_makes_the_lab_harder_to_beat_in_court_and_softer_to_judge()
    {
        Game g = Lab();
        ModelState m = g.Me.Models.First();
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        int bare = g.Me.Cases.Single().Strength;
        g.Me.Cases.Clear();
        g.Do(Command.AdoptPolicy(g.Id, 2));
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        Assert.True(g.Me.Cases.Single().Strength < bare);
        int rep = g.Me.Reputation;
        g.Me.BreachDue = g.State.Tick + 1;
        g.Weeks(1);
        int dropWithPolicy = rep - g.Me.Reputation;
        Game plain = Lab();
        int repPlain = plain.Me.Reputation;
        plain.Me.BreachDue = plain.State.Tick + 1;
        plain.Weeks(1);
        Assert.True(dropWithPolicy <= repPlain - plain.Me.Reputation);
    }

    [Fact]
    public void A_nonprofit_that_turns_for_profit_is_sued_and_the_claim_reads_the_policy()
    {
        Game g = Lab();
        g.Me.Charter = Charter.Nonprofit;
        g.Do(Command.ConvertCharter(g.Id, Charter.ForProfit));
        g.Weeks(g.C.Charter.ReviewWeeks);
        Assert.Equal(Charter.ForProfit, g.Me.Charter);
        LawCase claim = g.Me.Cases.Single(x => x.Kind == Litigation.Charter);
        Assert.Equal(g.C.Charter.ChallengeClaim, claim.Claim);
        Assert.Equal(g.C.Charter.ChallengeStrength, claim.Strength);
    }

    [Fact]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay()
    {
        int cases = 0, tiers = 0, contracts = 0, charters = 0, policies = 0;
        foreach (int seed in new[] { 1, 2, 3, 4 })
        {
            Game g = Game.New(seed);
            g.Autopilot(60);
            g.Me.Cash += 5_000_000;
            long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
            for (int week = 0; week < 300 && !g.Me.Bankrupt; week++)
            {
                if (week % 30 == 3) g.Try(Command.BuildSecurity(g.Id));
                if (Security.Open(g.Me)) g.Try(Command.AnswerBreach(g.Id, week % 3));
                if (GovContracts.Offered(g.Me) && week % 2 == 0) g.Try(Command.AcceptContract(g.Id));
                foreach (LawCase c in g.Me.Cases.ToList()) g.Try(week % 2 == 0 ? Command.SettleCase(g.Id, c.Id) : Command.RetainFirm(g.Id, c.Id));
                if (week % 45 == 5) g.Try(Command.AdoptPolicy(g.Id, (week / 45) % 4));
                if (week == 70) g.Try(Command.ConvertCharter(g.Id, Charter.Capped));
                if (week == 150) g.Try(Command.ConvertCharter(g.Id, Charter.Nonprofit));
                if (week == 230) g.Try(Command.ConvertCharter(g.Id, Charter.ForProfit));
                g.Autopilot(1);
            }
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
            Simulation back = Simulation.Load(g.C, g.Sim.Save());
            Assert.Equal(g.Sim.Digest(), back.Digest());
            back.StepMany(20);
            g.Weeks(20);
            Assert.Equal(g.Sim.Digest(), back.Digest());
            cases += Count(g, EventKind.CaseFiled);
            tiers += Count(g, EventKind.SecurityBuilt);
            contracts += Count(g, EventKind.ContractAccepted);
            charters += Count(g, EventKind.CharterChanged);
            policies += Count(g, EventKind.PolicyChanged);
        }
        Assert.True(cases > 0, "no claim was ever filed");
        Assert.True(tiers > 0, "no security was ever built");
        Assert.True(contracts > 0, "no contract was ever accepted");
        Assert.True(charters > 0, "no charter ever changed");
        Assert.True(policies > 0, "no policy was ever set");
    }
}
