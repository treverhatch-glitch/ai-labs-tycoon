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

public class AnnotationTests
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
            c.Shocks.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 1, sales: 2, gpus: 6);
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

    static int Clean(Game g, int units)
    {
        g.Do(Command.AcquireData(g.Id, 1, units));
        g.Do(Command.CleanData(g.Id, 1, units));
        g.Weeks(g.C.Datasets.CleanWeeks);
        return g.Me.DataClean1;
    }

    [Fact]
    public void A_lab_with_no_vendor_cleans_at_the_plain_yield_and_pays_nothing()
    {
        Game g = Lab();
        long before = g.Me.Ledger.Lifetime.Training;
        int clean = Clean(g, 50);
        Assert.Equal(50 * Datasets.Yield(g.C, 1) / 1000, clean);
        Assert.Equal(0, g.Me.AnnCleaned);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.AnnotatorsContracted);
        Assert.True(before >= g.Me.Ledger.Lifetime.Training);
    }

    [Fact]
    public void The_contract_is_checked()
    {
        Game g = Lab();
        g.Refused(Command.ContractAnnotators(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.ContractAnnotators(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.EndAnnotators(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.AuditVendor(g.Id), CommandError.InvalidArgument);
        g.Do(Command.ContractAnnotators(g.Id, 2));
        g.Refused(Command.ContractAnnotators(g.Id, 3), CommandError.InvalidArgument);
        Game off = Lab(c => c.Annotation.Enabled = 0);
        off.Refused(Command.ContractAnnotators(off.Id, 1), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = 10;
        poor.Refused(Command.ContractAnnotators(poor.Id, 3), CommandError.NotEnoughCash);
        Assert.Equal(0, poor.Me.AnnVendor);
    }

    [Fact]
    public void The_fee_is_paid_every_week_used_or_not_and_the_term_ends_the_contract()
    {
        Game g = Lab(c => c.Annotation.Term = 4);
        g.Do(Command.ContractAnnotators(g.Id, 2));
        long before = g.Me.Ledger.Lifetime.Training;
        g.Weeks(4);
        Assert.Equal(before - 4L * g.C.Annotation.Weekly2, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(0, g.Me.AnnVendor);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AnnotatorsEnded && e.Pick == 1);
    }

    [Fact]
    public void A_vendor_lifts_the_yield_within_its_size_and_not_beyond_it()
    {
        Game plain = Lab();
        int none = Clean(plain, 50);
        Game g = Lab();
        g.Do(Command.ContractAnnotators(g.Id, 3));
        int with = Clean(g, 50);
        Assert.True(with > none);
        Assert.Equal(50 * Math.Min(1000, Datasets.Yield(g.C, 1) + g.C.Annotation.Bonus3) / 1000, with);
        Assert.Equal(50, g.Me.AnnCleaned);
        Game big = Lab(c => c.Annotation.Cap3 = 40);
        big.Do(Command.ContractAnnotators(big.Id, 3));
        Assert.Equal(none, Clean(big, 50));
        Assert.Equal(0, big.Me.AnnCleaned);
    }

    [Fact]
    public void Only_the_volume_vendor_builds_exposure()
    {
        Game g = Lab();
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.C.Annotation.ExposeDiv = 100000;
        g.Weeks(3);
        Assert.Equal(3 * g.C.Annotation.Risk1, g.Me.AnnExposure);
        Game fair = Lab();
        fair.Do(Command.ContractAnnotators(fair.Id, 2));
        fair.Weeks(3);
        Assert.Equal(0, fair.Me.AnnExposure);
    }

    [Fact]
    public void An_audit_takes_most_of_the_exposure_away_and_waits_before_the_next()
    {
        Game g = Lab(c => c.Annotation.ExposeDiv = 100000);
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Me.AnnExposure = 500;
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.AuditVendor(g.Id));
        Assert.Equal(500 - 500 * g.C.Annotation.AuditCut / 1000, g.Me.AnnExposure);
        Assert.Equal(before - g.C.Annotation.AuditCost, g.Me.Ledger.Lifetime.Training);
        g.Refused(Command.AuditVendor(g.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Annotation.AuditWeeks);
        g.Do(Command.AuditVendor(g.Id));
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.VendorAudited));
    }

    [Fact]
    public void A_scandal_fines_the_lab_costs_trust_ends_the_contract_and_resets_the_exposure()
    {
        Game g = Lab(c => c.Annotation.ExposeDiv = 1);
        ModelState m = Selling(g);
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Me.AnnExposure = 1000;
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        long fines = g.Me.Ledger.Lifetime.Fines;
        Annotation.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.AnnStrikes);
        Assert.Equal(0, g.Me.AnnVendor);
        Assert.Equal(g.C.Annotation.ExposeReset, g.Me.AnnExposure);
        Assert.Equal(fines - g.C.Annotation.ExposeFine, g.Me.Ledger.Lifetime.Fines);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) < trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.VendorScandal);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.AnnotatorsEnded);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Ending_early_costs_most_of_the_fees_still_to_run()
    {
        Game g = Lab();
        g.Do(Command.ContractAnnotators(g.Id, 3));
        g.Weeks(6);
        int left = g.Me.AnnLeft;
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.EndAnnotators(g.Id));
        Assert.Equal(before - left * (long)g.C.Annotation.Weekly3 * g.C.Annotation.EarlyPermille / 1000, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(0, g.Me.AnnVendor);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AnnotatorsEnded && e.Pick == 0);
        Game poor = Lab();
        poor.Do(Command.ContractAnnotators(poor.Id, 3));
        poor.Me.Cash = 1;
        poor.Refused(Command.EndAnnotators(poor.Id), CommandError.NotEnoughCash);
        Assert.Equal(3, poor.Me.AnnVendor);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_vendor()
    {
        Game g = Lab();
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 120, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.AnnVendor + lab.AnnLeft + lab.AnnExposure + lab.AnnStrikes);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
