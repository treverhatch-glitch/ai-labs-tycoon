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

/// <summary>Bankrupt and max-cash edges for era-c systems, and save/load in the middle of multi-week processes.</summary>
public class EraCSystemsLimitsAndMidSaveHardeningTests
{
    static ModelState Trained(Game g, int capability)
    {
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        m.Capability = capability;
        return m;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, long? cash = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        g.Me.Exposure = 0;
        if (cash.HasValue) g.Me.Cash = cash.Value;
        return g;
    }

    static void Offer(Game g)
    {
        g.Me.OfferSeq = 0;
        g.Me.ContractNext = g.State.Tick + 1;
        g.Weeks(1);
        Assert.True(GovContracts.Offered(g.Me));
    }

    [Fact]
    public void Security_build_refuses_without_cash_and_runs_clean_at_max_wealth()
    {
        Game broke = Lab(3, cash: 10);
        broke.Me.SecurityTier = 0;
        broke.Refused(Command.BuildSecurity(broke.Id), CommandError.NotEnoughCash);
        Game rich = Lab(4);
        rich.Do(Command.BuildSecurity(rich.Id));
        rich.Weeks(rich.C.Security.BuildWeeks + 4);
        HardeningInvariants.AssertAllLabs(rich.C, rich.State, eraBounds: true, eraBBounds: true, eraCBounds: true);
    }

    [Fact]
    public void Charter_conversion_refuses_without_cash_and_finishes_when_rich()
    {
        Game broke = Lab(5, cash: 1);
        broke.Refused(Command.ConvertCharter(broke.Id, Charter.Capped), CommandError.NotEnoughCash);
        Game rich = Lab(6);
        rich.Do(Command.ConvertCharter(rich.Id, Charter.Capped));
        rich.Weeks(rich.C.Charter.ReviewWeeks);
        Assert.Equal(Charter.Capped, rich.Me.Charter);
        HardeningInvariants.AssertBooksBalance(rich.C, rich.State);
    }

    [Fact]
    public void Scaling_adopt_refuses_without_cash_and_at_max_cash_holds_tripwire()
    {
        Game broke = Lab(7, cash: 1);
        broke.Refused(Command.AdoptPolicy(broke.Id, 1), CommandError.NotEnoughCash);
        Game rich = Lab(8);
        rich.C.Scaling.SafetyPerTier = 10;
        rich.Do(Command.AdoptPolicy(rich.Id, 2));
        Trained(rich, ScalingPolicy.Tripwire(rich.C, 2) + 50);
        rich.Weeks(1);
        Assert.True(rich.Me.PolicyHold > 0);
        HardeningInvariants.AssertAllLabs(rich.C, rich.State, eraBounds: true, eraBBounds: true, eraCBounds: true);
    }

    [Fact]
    public void Security_build_mid_save_load_matches_uninterrupted_completion()
    {
        Game straight = Lab(9);
        straight.Do(Command.BuildSecurity(straight.Id));
        int weeks = straight.C.Security.BuildWeeks;
        straight.Weeks(weeks);

        Game choppy = Lab(9);
        choppy.Do(Command.BuildSecurity(choppy.Id));
        int half = weeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(weeks - half);

        Assert.Equal(straight.Me.SecurityTier, choppy.Me.SecurityTier);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Charter_review_mid_save_load_matches_uninterrupted_change()
    {
        Game straight = Lab(10);
        straight.Do(Command.ConvertCharter(straight.Id, Charter.Nonprofit));
        straight.Weeks(straight.C.Charter.ReviewWeeks);

        Game choppy = Lab(10);
        choppy.Do(Command.ConvertCharter(choppy.Id, Charter.Nonprofit));
        int half = choppy.C.Charter.ReviewWeeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Charter.ReviewWeeks - half);

        Assert.Equal(straight.Me.Charter, choppy.Me.Charter);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Government_contract_mid_term_save_load_matches_uninterrupted_payouts()
    {
        Game straight = Lab(11);
        Offer(straight);
        straight.Do(Command.AcceptContract(straight.Id));
        int weeks = straight.C.GovContracts.Kind(straight.Me.ContractKind).Weeks;
        straight.Weeks(weeks);

        Game choppy = Lab(11);
        Offer(choppy);
        choppy.Do(Command.AcceptContract(choppy.Id));
        int half = weeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(weeks - half);

        Assert.Equal(straight.Me.ContractPaid, choppy.Me.ContractPaid);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Litigation_hearing_mid_save_load_matches_uninterrupted_outcome()
    {
        Game straight = Lab(12);
        Litigation.File(straight.C, straight.State, straight.Me, Litigation.Incident, 70_000, 400, "mid");
        straight.Weeks(straight.C.Litigation.HearingWeeks + straight.C.Litigation.AppealWeeks);

        Game choppy = Lab(12);
        Litigation.File(choppy.C, choppy.State, choppy.Me, Litigation.Incident, 70_000, 400, "mid");
        int half = choppy.C.Litigation.HearingWeeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Litigation.HearingWeeks + choppy.C.Litigation.AppealWeeks - half);

        Assert.Equal(straight.Me.Cases.Count, choppy.Me.Cases.Count);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }
}