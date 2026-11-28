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

/// <summary>Zero, max, save/load and bankrupt edges for litigation, security, contracts, charter, and scaling (schema 81–84).</summary>
public class EraCSystemsEdgeHardeningTests
{
    static ModelState Trained(Game g, int capability)
    {
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        m.Capability = capability;
        return m;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
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
        g.Me.ContractNext = 0;
        return g;
    }

    static void Offer(Game g, string kind = "public-services")
    {
        g.Me.OfferSeq = g.C.GovContracts.Kinds.FindIndex(k => k.Id == kind);
        g.Me.ContractNext = g.State.Tick + 1;
        g.Weeks(1);
        Assert.True(GovContracts.Offered(g.Me));
    }

    [Fact]
    public void Security_tier_build_save_load_keeps_exposure_non_negative()
    {
        Game g = Lab();
        g.Do(Command.BuildSecurity(g.Id));
        string mid = g.Sim.Save();
        g.Weeks(g.C.Security.BuildWeeks);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.True(back.Player.SecurityBuilding > 0 || back.Player.SecurityTier > 0);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true);
    }

    [Fact]
    public void An_open_contract_survives_save_load_and_keeps_engineers_away()
    {
        Game g = Lab();
        Offer(g);
        g.Do(Command.AcceptContract(g.Id));
        int away = g.Me.Staff.Count(p => GovContracts.Away(g.C, g.Me, p));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(away, back.Player.Staff.Count(p => GovContracts.Away(g.C, back.Player, p)));
        HardeningInvariants.AssertBooksBalance(g.C, back.State);
    }

    [Fact]
    public void A_charter_review_mid_save_load_finishes_on_schedule()
    {
        Game straight = Lab(20);
        straight.Do(Command.ConvertCharter(straight.Id, Charter.Capped));
        straight.Weeks(straight.C.Charter.ReviewWeeks);
        Game choppy = Lab(20);
        choppy.Do(Command.ConvertCharter(choppy.Id, Charter.Capped));
        int half = choppy.C.Charter.ReviewWeeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Charter.ReviewWeeks - half);
        Assert.Equal(straight.Me.Charter, choppy.Me.Charter);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_scaling_hold_from_a_tripwire_survives_save_load()
    {
        Game g = Lab();
        g.C.Scaling.SafetyPerTier = 1;
        g.Do(Command.AdoptPolicy(g.Id, 2));
        Trained(g, ScalingPolicy.Tripwire(g.C, 2) + 20);
        g.Weeks(1);
        Assert.True(g.Me.PolicyHold > 0);
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(g.Me.PolicyHold, back.Player.PolicyHold);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true);
    }

    [Fact]
    public void A_litigation_case_mid_hearing_save_load_matches_judgment_week()
    {
        int weakest = Lab().C.Litigation.JudgeAt + Lab().C.Litigation.InHouseStrength - 1;
        Game straight = Lab(21);
        Litigation.File(straight.C, straight.State, straight.Me, Litigation.Incident, 80_000, weakest, "edge");
        straight.Weeks(straight.C.Litigation.HearingWeeks);
        Game choppy = Lab(21);
        Litigation.File(choppy.C, choppy.State, choppy.Me, Litigation.Incident, 80_000, weakest, "edge");
        int half = choppy.C.Litigation.HearingWeeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Litigation.HearingWeeks - half);
        Assert.Equal(straight.Me.Cases.Count, choppy.Me.Cases.Count);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void All_five_systems_disabled_keeps_the_docket_quiet_through_autopilot()
    {
        Game g = Lab(4, DisableEraC);
        g.Autopilot(120);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(0, g.Me.PolicyLevel);
        Assert.False(GovContracts.InForce(g.Me));
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: false);
    }

    static void DisableEraC(GameConfig c)
    {
        c.Litigation.Enabled = 0;
        c.Security.Enabled = 0;
        c.GovContracts.Enabled = 0;
        c.Charter.Enabled = 0;
        c.Scaling.Enabled = 0;
    }

    [Fact]
    public void Schema_81_through_84_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab();
        g.Do(Command.BuildSecurity(g.Id));
        g.Do(Command.AdoptPolicy(g.Id, 1));
        Litigation.File(g.C, g.State, g.Me, Litigation.Incident, 40_000, 600, "fuzz");
        Offer(g);
        g.Do(Command.AcceptContract(g.Id));
        string current = g.Sim.Save();
        for (int schema = 81; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true);
        }
    }
}