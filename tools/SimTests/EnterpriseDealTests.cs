using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EnterpriseDealTests
{
    const string Sector = "enterprise-knowledge";

    static Game At(int year = 2025) =>
        ExpansionTestSupport.SoloAtYear(year, tweak: c => c.EnterpriseDeals.OpensYear = 2018)
            .WithStarterLab(safety: 0, sales: 2, gpus: 20);

    static ModelState Released(Game g, int cap = 90_000) =>
        ExpansionTestSupport.AddReleasedModel(g, cap, Sector);

    [Fact]
    public void Before_opens_year_pursuit_refused()
    {
        var g = ExpansionTestSupport.SoloAtYear(2023, tweak: c => c.EnterpriseDeals.OpensYear = 2030).WithStarterLab(sales: 2);
        ExpansionTestSupport.AddReleasedModel(g, 80_000, "customer-support");
        g.Refused(Command.PursueEnterpriseSeat(g.Id, "customer-support"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pursue_requires_model_on_sale()
    {
        var g = At();
        g.Refused(Command.PursueEnterpriseSeat(g.Id, Sector), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pursue_requires_sales_staff()
    {
        var g = At();
        g = ExpansionTestSupport.SoloAtYear(2025, tweak: c => { c.EnterpriseDeals.OpensYear = 2018; c.EnterpriseDeals.MinSalesStaff = 3; })
            .WithStarterLab(sales: 1, gpus: 20);
        Released(g);
        g.Refused(Command.PursueEnterpriseSeat(g.Id, Sector), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void Pursue_and_sign_start_active_term()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector, "standard"));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        var deal = g.Me.EnterpriseDeals.Single(d => d.SectorId == Sector);
        Assert.Equal(EnterpriseDealPhase.Active, deal.Phase);
        Assert.Equal(m.Id, deal.ModelId);
        Assert.True(deal.EndsTick > g.Tick);
    }

    [Fact]
    public void Sign_refuses_without_prospect()
    {
        var g = At();
        var m = Released(g);
        g.Refused(Command.SignEnterpriseSeat(g.Id, Sector, m.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Duplicate_prospect_refused()
    {
        var g = At();
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Refused(Command.PursueEnterpriseSeat(g.Id, Sector), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_refuses_pursue()
    {
        var g = At();
        Released(g);
        g.Me.Bankrupt = true;
        g.Refused(Command.PursueEnterpriseSeat(g.Id, Sector), CommandError.LabBankrupt);
    }

    [Fact]
    public void Walk_abandons_prospect()
    {
        var g = At();
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.WalkEnterpriseSeat(g.Id, Sector));
        Assert.Equal(EnterpriseDealPhase.Churned, g.Me.EnterpriseDeals[0].Phase);
    }

    [Fact]
    public void SLA_pass_pays_revenue_bonus()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        g.Me.LastWeek.ServedPermille = 900;
        var entry = g.State.Sector(Sector).Entry(g.Id);
        entry.LastRevenue = 100_000;
        long cash = g.Me.Cash;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Cash > cash);
    }

    [Fact]
    public void SLA_miss_moves_to_at_risk()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        g.Me.LastWeek.ServedPermille = 100;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(EnterpriseDealPhase.AtRisk, g.Me.EnterpriseDeals[0].Phase);
        Assert.Equal(1, g.Me.EnterpriseDeals[0].Strikes);
    }

    [Fact]
    public void Strike_cap_churns_and_hits_reputation()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "premium"));
        int rep = g.Me.Reputation;
        g.Me.LastWeek.ServedPermille = 0;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(EnterpriseDealPhase.Churned, g.Me.EnterpriseDeals[0].Phase);
        Assert.True(g.Me.Reputation < rep);
    }

    [Fact]
    public void Terminate_pays_break_penalty()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        long cash = g.Me.Cash;
        g.Do(Command.TerminateEnterpriseSeat(g.Id, 0));
        Assert.True(g.Me.Cash < cash);
        Assert.Equal(EnterpriseDealPhase.Churned, g.Me.EnterpriseDeals[0].Phase);
    }

    [Fact]
    public void Terminate_refuses_prospect()
    {
        var g = At();
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Refused(Command.TerminateEnterpriseSeat(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Term_completes_on_ends_tick()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        var deal = g.Me.EnterpriseDeals[0];
        g.State.Tick = deal.EndsTick;
        g.Me.LastWeek.ServedPermille = 1000;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(EnterpriseDealPhase.Completed, deal.Phase);
    }

    [Fact]
    public void Renew_after_completed()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        var deal = g.Me.EnterpriseDeals[0];
        deal.Phase = EnterpriseDealPhase.Completed;
        g.Do(Command.RenewEnterpriseSeat(g.Id, Sector, "standard"));
        Assert.Equal(EnterpriseDealPhase.Active, deal.Phase);
    }

    [Fact]
    public void Prospect_expires_to_churned()
    {
        var g = At();
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        var deal = g.Me.EnterpriseDeals[0];
        g.State.Tick = deal.ProspectEndsTick;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(EnterpriseDealPhase.Churned, deal.Phase);
    }

    [Fact]
    public void Premium_tier_stricter_sla_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.EnterpriseDeals.Tier("premium").SlaServedPermille > c.EnterpriseDeals.Tier("standard").SlaServedPermille);
    }

    [Fact]
    public void Rival_never_enabled()
    {
        var g = ExpansionTestSupport.DuoAtYear(2025);
        var rival = g.Rival("openaye");
        Assert.False(EnterpriseDealSystem.Enabled(g.C, g.State, rival));
    }

    [Fact]
    public void Zero_deals_run_week_is_noop()
    {
        var g = At();
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.EnterpriseDeals);
    }

    [Fact]
    public void Schema_31_migration()
    {
        var g = At();
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(34, loaded.State.Schema);
        Assert.Single(loaded.Player.EnterpriseDeals);
    }

    [Fact]
    public void Save_roundtrip_active_deal()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(EnterpriseDealPhase.Active, loaded.Player.EnterpriseDeals[0].Phase);
    }

    [Fact]
    public void Not_enough_cash_sign_refused()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Me.Cash = 0;
        g.Refused(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Max_tier_premium_sign()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector, "premium"));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "premium"));
        Assert.Equal("premium", g.Me.EnterpriseDeals[0].TierId);
        Assert.Equal(2, g.Me.EnterpriseDeals[0].StrikeLimit);
    }

    [Fact]
    public void Events_on_sign_and_churn()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EnterpriseSeatSigned);
        g.Me.LastWeek.ServedPermille = 0;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EnterpriseSeatChurned);
    }

    [Fact]
    public void Market_cross_served_permille_from_step()
    {
        var g = At();
        var m = Released(g);
        g.Rent(40);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        g.Weeks(1);
        Assert.True(g.Me.LastWeek.ServedPermille > 0);
    }

    [Fact]
    public void Ineligible_sector_refused()
    {
        var g = At();
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "spam-search");
        g.Refused(Command.PursueEnterpriseSeat(g.Id, "spam-search"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pledge_served_breach_suppresses_deal_strike_Q214()
    {
        var g = ExpansionTestSupport.SoloAtYear(2025, tweak: c =>
        {
            c.EnterpriseDeals.OpensYear = 2018;
            c.Sla.MaxLevel = 3;
            c.Sla.BreachBelowPermille = 900;
        }).WithStarterLab(sales: 2, gpus: 20);
        Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, g.Me.Models[0].Id, "standard"));
        g.Me.SlaLevel = 2;
        g.Me.LastWeek.ServingDemand = 1000;
        g.Me.LastWeek.ServedPermille = 500;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.EnterpriseDeals[0].Strikes);
        Assert.Equal(EnterpriseDealPhase.Active, g.Me.EnterpriseDeals[0].Phase);
        g.Me.SlaLevel = 0;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.EnterpriseDeals[0].Strikes);
    }

    [Fact]
    public void Deal_bonus_does_not_inflate_sector_last_revenue()
    {
        var g = At();
        var m = Released(g);
        g.Do(Command.PursueEnterpriseSeat(g.Id, Sector));
        g.Do(Command.SignEnterpriseSeat(g.Id, Sector, m.Id, "standard"));
        var entry = g.State.Sector(Sector).Entry(g.Id);
        entry.LastRevenue = 200_000;
        long before = entry.LastRevenue;
        g.Me.LastWeek.ServedPermille = 1000;
        EnterpriseDealSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, entry.LastRevenue);
        Assert.True(g.Me.Cash > 0);
    }

    [Fact]
    public void Ten_years_with_enterprise_deals_deterministic()
    {
        const int years = 10;
        string a = RunSoak(5151, years);
        string b = RunSoak(5151, years);
        Assert.Equal(a, b);
    }

    static string RunSoak(long seed, int years)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.EnterpriseDeals.OpensYear = 2018), options));
        g.WithStarterLab(sales: 2, gpus: 16);
        int weeks = years * g.C.Calendar.WeeksPerYear;
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        return g.Sim.Digest();
    }
}