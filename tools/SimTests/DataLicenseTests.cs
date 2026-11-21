using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DataLicenseTests
{
    const string Source = "scrape";

    static Game At(int year = 2010)
    {
        var g = Game.Solo(17, c =>
        {
            c.DataLicenses.OpensYear = 2005;
            c.Training.DataSource("scrape").OpensYear = 0;
            c.Training.DataSource("licensed").OpensYear = 0;
        });
        g.State.Tick = AiLabs.Rules.Systems.GameDate.TickOfYear(g.C.Calendar, year);
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        return g;
    }

    static void StartScrapeTraining(Game g)
    {
        g.Do(Command.SetTrainingDataSource(g.Id, Source));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
    }

    [Fact]
    public void Before_opens_year_pursuit_refused()
    {
        var g = ExpansionTestSupport.SoloAtYear(2003, tweak: c => c.DataLicenses.OpensYear = 2010).WithStarterLab();
        g.Refused(Command.PursueDataLicense(g.Id, Source), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pursue_requires_researchers()
    {
        var g = At();
        g = ExpansionTestSupport.SoloAtYear(2010, tweak: c =>
        {
            c.DataLicenses.OpensYear = 2005;
            c.DataLicenses.MinResearchers = 3;
        }).WithStarterLab();
        g.Refused(Command.PursueDataLicense(g.Id, Source), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void Pursue_and_sign_start_active_term()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source, "standard"));
        g.Do(Command.SignDataLicense(g.Id, Source, "standard"));
        var lic = g.Me.DataLicenses.Single(d => d.SourceId == Source);
        Assert.Equal(DataLicensePhase.Active, lic.Phase);
        Assert.True(lic.EndsTick > g.Tick);
    }

    [Fact]
    public void Training_on_scrape_refused_without_license_after_opens()
    {
        var g = At();
        g.Do(Command.SetTrainingDataSource(g.Id, Source));
        g.Refused(Command.StartTraining(g.Id, "mini", 400), CommandError.InvalidArgument);
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        g.Do(Command.SetTrainingDataSource(g.Id, Source));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        Assert.Single(g.Me.Models, m => m.Status == ModelStatus.Training);
    }

    [Fact]
    public void Active_training_on_source_grants_research_bonus()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        StartScrapeTraining(g);
        long rp = g.Me.ResearchPoints;
        g.Sim.Step();
        Assert.True(g.Me.ResearchPoints >= rp + g.C.DataLicenses.Tier("standard").WeeklyResearchBonus);
    }

    [Fact]
    public void Missed_week_adds_strike()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.DataLicenses[0].Strikes);
        Assert.Equal(DataLicensePhase.AtRisk, g.Me.DataLicenses[0].Phase);
    }

    [Fact]
    public void Strike_cap_revokes()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source, "premium"));
        g.Do(Command.SignDataLicense(g.Id, Source, "premium"));
        int rep = g.Me.Reputation;
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(DataLicensePhase.Revoked, g.Me.DataLicenses[0].Phase);
        Assert.True(g.Me.Reputation < rep);
    }

    [Fact]
    public void Bankrupt_refuses_pursue()
    {
        var g = At();
        g.Me.Bankrupt = true;
        g.Refused(Command.PursueDataLicense(g.Id, Source), CommandError.LabBankrupt);
    }

    [Fact]
    public void Walk_abandons_prospect()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.WalkDataLicense(g.Id, Source));
        Assert.Equal(DataLicensePhase.Revoked, g.Me.DataLicenses[0].Phase);
    }

    [Fact]
    public void Terminate_pays_break_penalty()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        long cash = g.Me.Cash;
        g.Do(Command.TerminateDataLicense(g.Id, 0));
        Assert.True(g.Me.Cash < cash);
    }

    [Fact]
    public void Term_completes_on_ends_tick()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        var lic = g.Me.DataLicenses[0];
        g.State.Tick = lic.EndsTick;
        StartScrapeTraining(g);
        g.Me.LastWeek.TrainingSourceIds.Add(Source);
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(DataLicensePhase.Completed, lic.Phase);
    }

    [Fact]
    public void Renew_after_completed()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        g.Me.DataLicenses[0].Phase = DataLicensePhase.Completed;
        g.Do(Command.RenewDataLicense(g.Id, Source));
        Assert.Equal(DataLicensePhase.Active, g.Me.DataLicenses[0].Phase);
    }

    [Fact]
    public void Schema_32_migration()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(32, loaded.State.Schema);
        Assert.Single(loaded.Player.DataLicenses);
    }

    [Fact]
    public void Save_roundtrip_active_license()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(DataLicensePhase.Active, loaded.Player.DataLicenses[0].Phase);
    }

    [Fact]
    public void Rival_never_enabled()
    {
        var g = ExpansionTestSupport.DuoAtYear(2010, tweak: c => c.DataLicenses.OpensYear = 2005);
        var rival = g.Rival("openaye");
        Assert.False(DataLicenseSystem.Enabled(g.C, g.State, rival));
    }

    [Fact]
    public void Zero_licenses_run_week_noop()
    {
        var g = At();
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.DataLicenses);
    }

    [Fact]
    public void Internal_source_never_needs_license()
    {
        var g = At();
        Assert.True(DataLicenseSystem.MayTrainOnSource(g.C, g.State, g.Me, "internal"));
    }

    [Fact]
    public void Prep_discount_applies_when_licensed()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source, "standard"));
        g.Do(Command.SetTrainingDataSource(g.Id, Source));
        int cut = DataLicenseSystem.PrepDiscountPermille(g.C, g.State, g.Me, Source);
        Assert.Equal(g.C.DataLicenses.Tier("standard").PrepDiscountPermille, cut);
    }

    [Fact]
    public void Training_system_records_source_in_last_week()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        StartScrapeTraining(g);
        g.Sim.Step();
        Assert.Contains(Source, g.Me.LastWeek.TrainingSourceIds);
    }

    [Fact]
    public void Max_tier_premium_sign()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source, "premium"));
        g.Do(Command.SignDataLicense(g.Id, Source, "premium"));
        Assert.Equal(2, g.Me.DataLicenses[0].StrikeLimit);
    }

    [Fact]
    public void Ineligible_source_refused()
    {
        var g = At();
        g.Refused(Command.PursueDataLicense(g.Id, "internal"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Not_enough_cash_sign_refused()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Me.Cash = 0;
        g.Refused(Command.SignDataLicense(g.Id, Source), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Duplicate_prospect_refused()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Refused(Command.PursueDataLicense(g.Id, Source), CommandError.InvalidArgument);
    }

    [Fact]
    public void Licensed_source_in_eligible_list()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, "licensed"));
        g.Do(Command.SignDataLicense(g.Id, "licensed"));
        Assert.Equal(DataLicensePhase.Active, g.Me.DataLicenses[0].Phase);
    }

    [Fact]
    public void Events_on_sign_and_revoke()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        g.Do(Command.SignDataLicense(g.Id, Source));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataLicenseSigned);
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataLicenseRevoked);
    }

    [Fact]
    public void Prospect_expires_to_revoked()
    {
        var g = At();
        g.Do(Command.PursueDataLicense(g.Id, Source));
        var lic = g.Me.DataLicenses[0];
        g.State.Tick = lic.ProspectEndsTick;
        DataLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(DataLicensePhase.Revoked, lic.Phase);
    }

    [Fact]
    public void Ten_years_with_data_licenses_deterministic()
    {
        const int years = 10;
        string a = RunSoak(6262, years);
        string b = RunSoak(6262, years);
        Assert.Equal(a, b);
    }

    static string RunSoak(long seed, int years)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.DataLicenses.OpensYear = 2005), options));
        g.WithStarterLab(gpus: 16);
        int weeks = years * g.C.Calendar.WeeksPerYear;
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        return g.Sim.Digest();
    }
}