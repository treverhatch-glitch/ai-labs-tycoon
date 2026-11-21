using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q163: weekly patent license fees from labs that already hold a patented tech.</summary>
public class PatentLicenseTests
{
    const string Tech = "state-machines";

    static Game HolderWithInfringer()
    {
        Game g = Game.New().WithStarterLab();
        LabState rival = g.Rival("openaye");
        g.Me.Cash = 2_000_000;
        rival.Cash = 2_000_000;
        rival.ResearchPoints = 500;
        g.GrantResearch(500);
        g.Do(Command.ResearchTech(rival.Id, Tech));
        g.Research(Tech);
        g.Do(Command.FilePatent(g.Id, Tech));
        return g;
    }

    [Fact]
    public void Q163_main_infringer_pays_holder_each_week()
    {
        Game g = HolderWithInfringer();
        LabState rival = g.Rival("openaye");
        long holderBefore = g.Me.Cash;
        long rivalBefore = rival.Cash;
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(holderBefore + fee, g.Me.Cash);
        Assert.Equal(rivalBefore - fee, rival.Cash);
    }

    [Fact]
    public void Q163_main_ledger_uses_patent_license_line()
    {
        Game g = HolderWithInfringer();
        LabState rival = g.Rival("openaye");
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(fee, g.Me.Ledger.ThisWeek.Get(LedgerLine.PatentLicense));
        Assert.Equal(-fee, rival.Ledger.ThisWeek.Get(LedgerLine.PatentLicense));
    }

    [Fact]
    public void Q163_zero_fee_disabled()
    {
        Game g = HolderWithInfringer();
        g.C.Research.Patents.LicenseCashPerInfringerPerWeek = 0;
        long holderBefore = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(holderBefore, g.Me.Cash);
        Assert.Equal(0, PatentLicenseSystem.WeeklyLicenseCash(g.C, g.State, g.Me, g.Tick));
    }

    [Fact]
    public void Q163_zero_no_infringers_when_solo()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 500_000;
        g.GrantResearch(500);
        g.Research(Tech);
        g.Do(Command.FilePatent(g.Id, Tech));
        Assert.Equal(0, PatentLicenseSystem.InfringerCount(g.C, g.State, g.Me, Tech, g.Tick));
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q163_max_two_infringers_pay_twice()
    {
        Game g = Game.New().WithStarterLab();
        LabState open = g.Rival("openaye");
        LabState xay = g.Rival("xay");
        g.Me.Cash = 2_000_000;
        open.Cash = 2_000_000;
        xay.Cash = 2_000_000;
        open.ResearchPoints = 500;
        xay.ResearchPoints = 500;
        g.GrantResearch(500);
        g.Do(Command.ResearchTech(open.Id, Tech));
        g.Do(Command.ResearchTech(xay.Id, Tech));
        g.Research(Tech);
        g.Do(Command.FilePatent(g.Id, Tech));
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        long holderBefore = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(holderBefore + fee * 2, g.Me.Cash);
        Assert.Equal(2, PatentLicenseSystem.InfringerCount(g.C, g.State, g.Me, Tech, g.Tick));
    }

    [Fact]
    public void Q163_bankrupt_holder_skips_without_throw()
    {
        Game g = HolderWithInfringer();
        g.Me.Bankrupt = true;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q163_bankrupt_infringer_skips_payment()
    {
        Game g = HolderWithInfringer();
        LabState rival = g.Rival("openaye");
        rival.Bankrupt = true;
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q163_save_load_fees_continue()
    {
        Game g = HolderWithInfringer();
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        long afterFirst = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        PatentLicenseSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        Assert.Equal(afterFirst + fee, loaded.Me.Cash);
    }

    [Fact]
    public void Q163_cross_renounce_ends_royalty_stream()
    {
        Game g = HolderWithInfringer();
        long afterPay = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        afterPay = g.Me.Cash;
        g.Do(Command.RenouncePatent(g.Id, Tech));
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(afterPay, g.Me.Cash);
    }

    [Fact]
    public void Q163_cross_expired_patent_stops_fees()
    {
        Game g = Game.New(seed: 3, tweak: c => c.Research.Patents.DurationWeeks = 2);
        g.WithStarterLab();
        LabState rival = g.Rival("openaye");
        g.Me.Cash = 2_000_000;
        rival.Cash = 2_000_000;
        rival.ResearchPoints = 500;
        g.GrantResearch(500);
        g.Do(Command.ResearchTech(rival.Id, Tech));
        g.Research(Tech);
        g.Do(Command.FilePatent(g.Id, Tech));
        g.Weeks(3);
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q163_cross_holder_with_own_patent_does_not_pay_self()
    {
        Game g = HolderWithInfringer();
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + fee, g.Me.Cash);
        Assert.True(g.Me.Ledger.ThisWeek.PatentLicense >= fee);
    }

    [Fact]
    public void Q163_infringer_insufficient_cash_skips()
    {
        Game g = HolderWithInfringer();
        LabState rival = g.Rival("openaye");
        rival.Cash = 0;
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q163_weekly_license_cash_matches_run()
    {
        Game g = HolderWithInfringer();
        long expected = PatentLicenseSystem.WeeklyLicenseCash(g.C, g.State, g.Me, g.Tick);
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(expected, g.Me.Cash - before);
    }

    [Fact]
    public void Q163_sim_step_runs_after_patent_grants()
    {
        Game g = HolderWithInfringer();
        long fee = g.C.Research.Patents.LicenseCashPerInfringerPerWeek;
        g.Sim.Step();
        Assert.True(g.Me.Ledger.LastWeek.PatentLicense >= fee);
    }

    [Fact]
    public void Q163_cross_infringer_without_tech_does_not_pay()
    {
        Game g = Game.New().WithStarterLab();
        LabState rival = g.Rival("openaye");
        g.Me.Cash = 2_000_000;
        rival.Cash = 2_000_000;
        g.GrantResearch(500);
        g.Research(Tech);
        g.Do(Command.FilePatent(g.Id, Tech));
        Assert.Equal(0, PatentLicenseSystem.InfringerCount(g.C, g.State, g.Me, Tech, g.Tick));
        long before = g.Me.Cash;
        PatentLicenseSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q163_config_validator_rejects_negative_fee()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Research.Patents.LicenseCashPerInfringerPerWeek = -1;
        var p = ConfigValidator.Problems(c);
        Assert.Contains(p, x => x.Contains("licenseCashPerInfringerPerWeek"));
    }

    [Fact]
    public void Q163_schema_24_migration_adds_patent_license_ledger()
    {
        Game g = Game.Solo().WithStarterLab();
        string json = g.Sim.Save().Replace("\"schema\":25", "\"schema\":23");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(27, loaded.State.Schema);
        Assert.Equal(0, loaded.Me.Ledger.ThisWeek.PatentLicense);
    }

    [Fact]
    public void Q163_blocked_research_still_license_holder()
    {
        Game g = HolderWithInfringer();
        Assert.Contains(Tech, g.Rival("openaye").Techs);
        Assert.True(PatentLicenseSystem.InfringerCount(g.C, g.State, g.Me, Tech, g.Tick) > 0);
    }
}