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

/// <summary>Zero, max, save/load and bankrupt edges for tax, standards, GPU reliability, covenants, and allowances (schema 85–89).</summary>
public class EraDSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, int extraGpus = 0, long? cash = null)
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
            c.PlayerStart.Cash = 50_000_000;
            c.Tax.AllowancePerWeek = 0;
            c.PlayerStart.GrantPerWeek = 0;
            c.Reliability.BaseFailPpm = 0;
            c.Reliability.AgePpm = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (extraGpus > 0)
            Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, extraGpus);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        g.Me.TaxAccrued = 0;
        if (cash.HasValue) g.Me.Cash = cash.Value;
        return g;
    }

    static int Year(Game g) => g.C.Calendar.WeeksPerYear;

    static void Draft(Game g)
    {
        g.Me.StdNext = g.State.Tick + 1;
        g.Weeks(1);
    }

    [Fact]
    public void Capital_allowance_pool_survives_save_load()
    {
        Game g = Lab();
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        g.Me.Ledger.LastWeek.Hardware = -500_000;
        CapitalAllowances.Adjust(g.C, g.State, g.Me);
        g.Me.Ledger.LastWeek.Hardware = 0;
        long pool = g.Me.CapPool;
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(pool, back.Player.CapPool);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true);
    }

    [Fact]
    public void A_warranty_level_mid_save_load_matches_uninterrupted_failures()
    {
        Game g = Lab(7, c => c.Reliability.BaseFailPpm = 125_000);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, 8);
        g.Do(Command.SetWarranty(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(g.Me.RmaUnits, back.Player.RmaUnits);
        HardeningInvariants.AssertBooksBalance(g.C, back.State);
    }

    [Fact]
    public void A_loan_covenant_breach_mid_save_load_keeps_counts()
    {
        Game g = Lab(extraGpus: 100);
        g.Do(Command.TakeLoan(g.Id, 1000));
        g.C.Covenants.MinCoverPermille = 5000;
        g.Weeks(2);
        int breaches = g.Me.CovBreach;
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(breaches, back.Player.CovBreach);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true);
    }

    [Fact]
    public void Standards_adoption_mid_save_load_matches_uninterrupted_upkeep()
    {
        Game straight = Lab(12);
        Draft(straight);
        straight.Do(Command.TakeStandardsRole(straight.Id, Standards.Member));
        straight.Weeks(straight.C.Standards.SupportNeeded);
        straight.Do(Command.AdoptStandards(straight.Id));
        straight.Weeks(8);

        Game choppy = Lab(12);
        Draft(choppy);
        choppy.Do(Command.TakeStandardsRole(choppy.Id, Standards.Member));
        choppy.Weeks(choppy.C.Standards.SupportNeeded);
        choppy.Do(Command.AdoptStandards(choppy.Id));
        choppy.Weeks(4);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(4);

        Assert.Equal(straight.Me.StdLevel, choppy.Me.StdLevel);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Tax_accrual_survives_year_end_save_load()
    {
        Game g = Lab(13);
        g.Weeks(Year(g) - 8);
        long accrued = g.Me.TaxAccrued;
        string mid = g.Sim.Save();
        g.Weeks(12);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(accrued, back.Player.TaxAccrued);
        HardeningInvariants.AssertBooksBalance(g.C, back.State);
    }

    [Fact]
    public void All_five_systems_disabled_keeps_tax_and_standards_idle()
    {
        Game g = Lab(14, DisableEraD);
        g.Autopilot(Year(g) * 2);
        Assert.Equal(0, g.Me.StdLevel);
        Assert.Equal(0, g.Me.CapPool);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true, eraCBounds: false, eraDBounds: false);
    }

    static void DisableEraD(GameConfig c)
    {
        c.Tax.Enabled = 0;
        c.Standards.Enabled = 0;
        c.Reliability.Enabled = 0;
        c.Covenants.Enabled = 0;
        c.Allowances.Enabled = 0;
    }

    [Fact]
    public void Sponsor_standards_role_refuses_without_cash_and_rich_lab_adopts()
    {
        Game broke = Lab(15, cash: 10);
        Draft(broke);
        broke.Refused(Command.TakeStandardsRole(broke.Id, Standards.Sponsor), CommandError.NotEnoughCash);
        Game rich = Lab(16);
        Draft(rich);
        rich.Do(Command.TakeStandardsRole(rich.Id, Standards.Sponsor));
        HardeningInvariants.AssertBooksBalance(rich.C, rich.State);
    }

    [Fact]
    public void Schema_85_through_89_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(17, extraGpus: 16);
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Enhanced));
        g.Do(Command.SetWarranty(g.Id, 1));
        g.Do(Command.TakeLoan(g.Id, 500));
        string current = g.Sim.Save();
        for (int schema = 85; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true);
        }
    }
}