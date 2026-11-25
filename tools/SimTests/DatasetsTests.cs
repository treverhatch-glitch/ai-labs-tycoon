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

public class DatasetsTests
{
    static Game Lab(Action<GameConfig> tweak = null, int engineers = 4)
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
        }).WithStarterLab(researchers: 2, engineers: engineers, safety: 1, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static void SuitTick(Game g)
    {
        int w = g.C.Datasets.SuitWeeks;
        int tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.State.Tick = tick - tick % w;
    }

    static void Clean(Game g, int kind, int units)
    {
        g.Do(Command.AcquireData(g.Id, kind, units));
        g.Do(Command.CleanData(g.Id, kind, units));
        g.Weeks(g.C.Datasets.CleanWeeks);
    }

    [Fact]
    public void A_lab_that_never_sources_data_has_no_corpus_and_no_bonus()
    {
        Game g = Lab();
        ModelLineConfig line = g.C.Line("mini");
        CapabilityForecast before = TrainingSystem.Forecast(g.C, g.Me, line);
        g.Weeks(10);
        Assert.Equal(0, Datasets.BonusPermille(g.C, g.Me));
        Assert.Equal(before.Total, TrainingSystem.Forecast(g.C, g.Me, line).Total);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.DataAcquired || e.Kind == EventKind.DataSuit);
    }

    [Fact]
    public void Buying_is_checked_and_costs_by_source()
    {
        Game off = Lab(c => c.Datasets.Enabled = 0);
        off.Refused(Command.AcquireData(off.Id, 1, 10), CommandError.InvalidArgument);
        Game g = Lab();
        g.Refused(Command.AcquireData(g.Id, 0, 10), CommandError.InvalidArgument);
        g.Refused(Command.AcquireData(g.Id, 4, 10), CommandError.InvalidArgument);
        g.Refused(Command.AcquireData(g.Id, 1, 0), CommandError.InvalidArgument);
        g.Refused(Command.AcquireData(g.Id, 1, g.C.Datasets.MaxBuy + 1), CommandError.InvalidArgument);
        g.Refused(Command.AcquireData(g.Id, 3, 10), CommandError.MissingPrerequisite);
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.AcquireData(g.Id, 2, 10));
        Assert.Equal(10, g.Me.DataRaw2);
        Assert.Equal(before - 10L * g.C.Datasets.CostLicense, g.Me.Ledger.Lifetime.Training);
        Game poor = Lab();
        poor.Me.Cash = 0;
        poor.Refused(Command.AcquireData(poor.Id, 2, 10), CommandError.NotEnoughCash);
        Game full = Lab(c => { c.Datasets.MaxBuy = 50; c.Datasets.StockCap = 50; });
        full.Do(Command.AcquireData(full.Id, 1, 40));
        full.Refused(Command.AcquireData(full.Id, 1, 20), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Synthetic_data_needs_a_model_on_sale()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Refused(Command.AcquireData(g.Id, 3, 10), CommandError.MissingPrerequisite);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.AcquireData(g.Id, 3, 10));
        Assert.Equal(10, g.Me.DataRaw3);
    }

    [Fact]
    public void Cleaning_is_one_job_at_a_time_and_each_source_loses_its_own_share()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 100));
        g.Do(Command.AcquireData(g.Id, 2, 100));
        g.Refused(Command.CleanData(g.Id, 1, 0), CommandError.InvalidArgument);
        g.Refused(Command.CleanData(g.Id, 1, 101), CommandError.InvalidArgument);
        g.Refused(Command.CleanData(g.Id, 4, 10), CommandError.InvalidArgument);
        g.Do(Command.CleanData(g.Id, 1, 100));
        Assert.Equal(0, g.Me.DataRaw1);
        Assert.Equal(100, g.Me.DataJobUnits);
        g.Refused(Command.CleanData(g.Id, 2, 10), CommandError.InvalidArgument);
        g.Weeks(g.C.Datasets.CleanWeeks);
        Assert.Equal(100 * g.C.Datasets.YieldScrape / 1000, g.Me.DataClean1);
        Assert.Equal(0, g.Me.DataJobLeft);
        g.Do(Command.CleanData(g.Id, 2, 100));
        g.Weeks(g.C.Datasets.CleanWeeks);
        Assert.Equal(100 * g.C.Datasets.YieldLicense / 1000, g.Me.DataClean2);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataCleaned && e.SubjectId == 2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_job_needs_engineers_and_cash()
    {
        Game g = Lab(c => c.Datasets.PerEngineer = 10, 2);
        g.Do(Command.AcquireData(g.Id, 1, 100));
        g.Refused(Command.CleanData(g.Id, 1, 100), CommandError.NotEnoughStaff);
        g.Do(Command.CleanData(g.Id, 1, 20));
        Game poor = Lab();
        poor.Do(Command.AcquireData(poor.Id, 1, 100));
        poor.Me.Cash = 0;
        poor.Refused(Command.CleanData(poor.Id, 1, 100), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Raw_data_does_nothing_for_a_model_and_clean_data_raises_its_capability()
    {
        Game g = Lab();
        ModelLineConfig line = g.C.Line("mini");
        int plain = TrainingSystem.Forecast(g.C, g.Me, line).Total;
        g.Do(Command.AcquireData(g.Id, 2, 100));
        Assert.Equal(plain, TrainingSystem.Forecast(g.C, g.Me, line).Total);
        g.Do(Command.CleanData(g.Id, 2, 100));
        g.Weeks(g.C.Datasets.CleanWeeks);
        int with = TrainingSystem.Forecast(g.C, g.Me, line).Total;
        Assert.True(with > plain);
        Assert.Equal(plain + (int)MathI.MulPermille(line.BaseCapability, Datasets.BonusPermille(g.C, g.Me)), with);
        ModelState m = g.Train(line.Id, 0);
        g.UntilTrained(m);
        Assert.Equal(m.CapBase + m.CapTech + m.CapCarryover + m.CapLuck, m.Capability);
        Assert.True(m.CapTech > 0);
    }

    [Fact]
    public void Licensed_data_is_worth_more_than_scraped_and_the_bonus_has_a_cap()
    {
        Game a = Lab();
        Clean(a, 1, 100);
        Game b = Lab();
        Clean(b, 2, 100);
        Assert.True(Datasets.BonusPermille(b.C, b.Me) > Datasets.BonusPermille(a.C, a.Me));
        Game big = Lab(c => c.Datasets.BonusCap = 10);
        Clean(big, 2, 100);
        Assert.Equal(10, Datasets.BonusPermille(big.C, big.Me));
    }

    [Fact]
    public void Too_much_synthetic_data_makes_models_strange()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.DataClean2 = 100;
        int licensedOnly = Datasets.BonusPermille(g.C, g.Me);
        g.Me.DataClean3 = 100;
        Assert.True(Datasets.Collapsing(g.C, g.Me));
        int mixed = Datasets.BonusPermille(g.C, g.Me);
        g.Me.DataClean2 = 400;
        g.Me.DataClean3 = 100;
        Assert.False(Datasets.Collapsing(g.C, g.Me));
        Assert.True(mixed < Datasets.BonusPermille(g.C, g.Me));
        Assert.True(licensedOnly > 0);
    }

    [Fact]
    public void A_claim_takes_every_scraped_unit_and_fines_it_but_not_licensed_ones()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 60));
        g.Do(Command.AcquireData(g.Id, 2, 40));
        g.Me.DataClean1 = 30;
        long before = g.Me.Ledger.Lifetime.Fines;
        SuitTick(g);
        Datasets.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.DataRaw1 + g.Me.DataClean1);
        Assert.Equal(40, g.Me.DataRaw2);
        Assert.Equal(1, g.Me.DataSuits);
        Assert.Equal(before - 90L * g.C.Datasets.SuitFine, g.Me.Ledger.Lifetime.Fines);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSuit && e.SubjectId == 90);
    }

    [Fact]
    public void No_claim_comes_before_its_era_or_off_the_cycle_or_with_nothing_scraped()
    {
        Game early = Lab(c => c.Datasets.SuitEra = c.Eras.Count - 1);
        early.Do(Command.AcquireData(early.Id, 1, 60));
        SuitTick(early);
        Datasets.RunWeek(early.C, early.State, early.Me);
        Assert.Equal(60, early.Me.DataRaw1);
        Game off = Lab();
        off.Do(Command.AcquireData(off.Id, 1, 60));
        SuitTick(off);
        off.State.Tick++;
        Datasets.RunWeek(off.C, off.State, off.Me);
        Assert.Equal(60, off.Me.DataRaw1);
        Game none = Lab();
        none.Do(Command.AcquireData(none.Id, 2, 60));
        SuitTick(none);
        Datasets.RunWeek(none.C, none.State, none.Me);
        Assert.Equal(0, none.Me.DataSuits);
    }

    [Fact]
    public void A_cleaning_job_on_scraped_data_is_lost_to_a_claim()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 50));
        g.Do(Command.CleanData(g.Id, 1, 50));
        SuitTick(g);
        Datasets.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.DataSuits);
        Assert.Equal(0, g.Me.DataJobUnits);
        Assert.Equal(0, g.Me.DataJobLeft);
        Assert.Equal(0, g.Me.DataJobKind);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Licensing_scraped_units_after_the_fact_saves_them_from_a_claim()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 60));
        g.Me.DataClean1 = 20;
        g.Refused(Command.LicenseScraped(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.LicenseScraped(g.Id, 81), CommandError.InvalidArgument);
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.LicenseScraped(g.Id, 50));
        Assert.Equal(before - 50L * g.C.Datasets.BackfillCost, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(0, g.Me.DataClean1);
        Assert.Equal(20, g.Me.DataClean2);
        Assert.Equal(30, g.Me.DataRaw2);
        Assert.Equal(30, g.Me.DataRaw1);
        Assert.Equal(50, g.Me.DataBackfilled);
        SuitTick(g);
        Datasets.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(50, g.Me.DataClean2 + g.Me.DataRaw2);
        Assert.Equal(0, g.Me.DataRaw1);
    }

    [Fact]
    public void Saves_round_trip_in_the_middle_of_a_job_and_older_saves_have_no_data()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 80));
        g.Do(Command.CleanData(g.Id, 1, 50));
        g.Weeks(1);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 114, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.DataRaw1 + lab.DataJobUnits);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
