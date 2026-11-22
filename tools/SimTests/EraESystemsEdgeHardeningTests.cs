using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Save/load and edge coverage for listing, revenue financing, foundation, leaseback, and convertible notes (schema 90–94).</summary>
public class EraESystemsEdgeHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, int extraGpus = 0)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
            c.Charter.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.PlayerStart.Cash = 50_000_000;
            c.PlayerStart.GrantPerWeek = 0;
            c.Listing.MinWeeklyRevenue = 1;
            c.Rbf.MinWeeklyRevenue = 1;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Reputation = 600;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        if (extraGpus > 0)
            Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, extraGpus);
        g.Me.Reputation = Math.Max(g.Me.Reputation, 600);
        g.Me.Ledger.LastWeek.Revenue = Math.Max(g.Me.Ledger.LastWeek.Revenue, 100_000);
        return g;
    }

    [Fact]
    public void A_listed_company_mid_quarter_save_load_keeps_guidance()
    {
        Game g = Lab();
        g.Do(Command.ListCompany(g.Id));
        int next = g.Me.ListNext;
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(next, back.Player.ListNext);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true);
    }

    [Fact]
    public void A_revenue_advance_mid_save_load_matches_cleared_debt()
    {
        const int run = 16;
        Game straight = Lab(11);
        straight.Do(Command.ListCompany(straight.Id));
        straight.Do(Command.TakeRevenueAdvance(straight.Id, 2, 800));
        straight.Weeks(run);
        Game choppy = Lab(11);
        choppy.Do(Command.ListCompany(choppy.Id));
        choppy.Do(Command.TakeRevenueAdvance(choppy.Id, 2, 800));
        choppy.Weeks(run / 2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(run - run / 2);
        Assert.Equal(straight.Me.RbfOwed, choppy.Me.RbfOwed);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_foundation_mid_save_load_matches_uninterrupted_payouts()
    {
        Game straight = Lab(12);
        straight.Do(Command.EndowFoundation(straight.Id, 1000, Foundation.Safety));
        straight.Do(Command.SetFoundationPayout(straight.Id, 2));
        straight.Weeks(16);
        Game choppy = Lab(12);
        choppy.Do(Command.EndowFoundation(choppy.Id, 1000, Foundation.Safety));
        choppy.Do(Command.SetFoundationPayout(choppy.Id, 2));
        choppy.Weeks(8);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(8);
        Assert.Equal(straight.Me.FoundGiven, choppy.Me.FoundGiven);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_leaseback_mid_save_load_matches_buyback_week()
    {
        Game straight = Lab(13, extraGpus: 10);
        straight.Do(Command.SaleLeaseback(straight.Id, 5));
        straight.Weeks(straight.C.Leaseback.TermWeeks);
        Game choppy = Lab(13, extraGpus: 10);
        choppy.Do(Command.SaleLeaseback(choppy.Id, 5));
        int half = choppy.C.Leaseback.TermWeeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Leaseback.TermWeeks - half);
        Assert.Equal(straight.Me.LeaseUnits, choppy.Me.LeaseUnits);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_convertible_note_mid_save_load_matches_maturity()
    {
        Game straight = Lab(14);
        FundingSystem.ApplyRaise(straight.C, straight.State, straight.Me, straight.C.Funding.Round("angel"));
        straight.Do(Command.IssueNote(straight.Id, 1, 1000));
        int term = straight.Me.NoteLeft;
        straight.Weeks(term);
        Game choppy = Lab(14);
        FundingSystem.ApplyRaise(choppy.C, choppy.State, choppy.Me, choppy.C.Funding.Round("angel"));
        choppy.Do(Command.IssueNote(choppy.Id, 1, 1000));
        int half = choppy.Me.NoteLeft / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(term - half);
        Assert.Equal(straight.Me.NoteOwed, choppy.Me.NoteOwed);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void All_five_systems_disabled_keeps_financing_fields_idle()
    {
        Game g = Lab(15, DisableEraE);
        g.Autopilot(80);
        Assert.False(Listing.Listed(g.Me));
        Assert.Equal(0, g.Me.RbfAdvance);
        Assert.Equal(0, g.Me.FoundFund);
        Assert.Equal(0, g.Me.LeaseUnits);
        Assert.Equal(0, g.Me.NoteOwed);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: false, eraDBounds: false, eraEBounds: false);
    }

    static void DisableEraE(GameConfig c)
    {
        c.Listing.Enabled = 0;
        c.Rbf.Enabled = 0;
        c.Foundation.Enabled = 0;
        c.Leaseback.Enabled = 0;
        c.ConvertibleNotes.Enabled = 0;
    }

    [Fact]
    public void Listing_refuses_without_reputation_and_lists_when_rich()
    {
        Game broke = Lab(16, c => c.Listing.MinReputation = 900);
        broke.Me.Reputation = 100;
        broke.Refused(Command.ListCompany(broke.Id), CommandError.MissingPrerequisite);
        Game rich = Lab(17);
        rich.Do(Command.ListCompany(rich.Id));
        Assert.True(Listing.Listed(rich.Me));
        HardeningInvariants.AssertBooksBalance(rich.C, rich.State);
    }

    [Fact]
    public void Schema_90_through_94_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(18, extraGpus: 8);
        FundingSystem.ApplyRaise(g.C, g.State, g.Me, g.C.Funding.Round("angel"));
        g.Do(Command.ListCompany(g.Id));
        g.Do(Command.TakeRevenueAdvance(g.Id, 1, 500));
        g.Do(Command.EndowFoundation(g.Id, 1000, Foundation.OpenScience));
        g.Do(Command.SaleLeaseback(g.Id, 3));
        g.Do(Command.IssueNote(g.Id, 1, 400));
        string current = g.Sim.Save();
        for (int schema = 90; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true);
        }
    }
}