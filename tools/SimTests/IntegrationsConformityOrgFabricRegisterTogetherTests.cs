using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class IntegrationsConformityOrgFabricRegisterTogetherTests
{
    static void Quiet(GameConfig c)
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
        c.Fabric.EraA = 0;
        c.Fabric.EraB = 0;
        c.Fabric.EraC = 0;
    }

    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(4, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 500;
        foreach (StaffMember p in g.Me.Staff) { p.Senior = 0; p.Morale = 500; }
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 10);
        return g;
    }

    static ModelState Selling(Game g, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        return m;
    }

    static void LeadSafety(Game g)
    {
        g.Me.Staff.First(p => g.C.Role(p.RoleId).Function == RoleFunction.Safety).Senior = 1;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void An_incident_on_a_model_with_a_conformity_mark_leaves_a_finding_at_once()
    {
        Game g = Lab();
        ModelState certified = Selling(g);
        ModelState plain = Selling(g, "game-bots");
        g.Do(Command.JoinRegister(g.Id));
        certified.CertStage = Conformity.Certified;
        certified.CertLeft = 50;
        certified.CertScope = 1;
        Disclosure.OnIncident(g.C, g.Me, plain);
        Assert.Equal(0, g.Me.DiscFindings);
        Disclosure.OnIncident(g.C, g.Me, certified);
        Assert.Equal(g.C.Disclosure.CertifiedFindings, g.Me.DiscFindings);
        Assert.Equal(2, g.Me.DiscOpen);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_functional_lab_with_a_senior_safety_person_prepares_a_full_report_a_week_sooner()
    {
        Game plain = Lab();
        ModelState pm = Selling(plain);
        plain.Do(Command.JoinRegister(plain.Id));
        Disclosure.OnIncident(plain.C, plain.Me, pm);
        plain.Do(Command.FileReport(plain.Id, 2));
        Assert.Equal(plain.C.Disclosure.FullWeeks, plain.Me.DiscPrep);

        Game led = Lab();
        ModelState lm = Selling(led);
        led.Do(Command.JoinRegister(led.Id));
        led.Me.OrgForm = Org.Functional;
        LeadSafety(led);
        Disclosure.OnIncident(led.C, led.Me, lm);
        led.Do(Command.FileReport(led.Id, 2));
        Assert.Equal(led.C.Disclosure.FullWeeks - led.C.Disclosure.OrgFasterWeeks, led.Me.DiscPrep);

        Game flat = Lab();
        ModelState fm = Selling(flat);
        flat.Do(Command.JoinRegister(flat.Id));
        flat.Me.OrgForm = Org.Flat;
        LeadSafety(flat);
        Disclosure.OnIncident(flat.C, flat.Me, fm);
        flat.Do(Command.FileReport(flat.Id, 2));
        Assert.Equal(flat.C.Disclosure.FullWeeks, flat.Me.DiscPrep);

        Game settling = Lab();
        ModelState sm = Selling(settling);
        settling.Do(Command.JoinRegister(settling.Id));
        settling.Me.OrgForm = Org.Functional;
        settling.Me.OrgDisrupt = 3;
        LeadSafety(settling);
        Disclosure.OnIncident(settling.C, settling.Me, sm);
        settling.Do(Command.FileReport(settling.Id, 2));
        Assert.Equal(settling.C.Disclosure.FullWeeks, settling.Me.DiscPrep);
    }

    [Fact]
    public void Squads_that_are_all_led_take_half_the_cutover_loss_off_a_fabric_build()
    {
        Game g = Lab();
        g.Do(Command.OrderFabric(g.Id));
        g.Weeks(g.C.Fabric.LeadA - g.C.Fabric.CutoverWeeks);
        Assert.True(Fabric.Cutover(g.C, g.Me));
        Assert.Equal(-g.C.Fabric.CutoverLoss, Fabric.Permille(g.C, g.Me));
        g.Me.OrgForm = Org.Squads;
        Assert.Equal(-g.C.Fabric.CutoverLoss, Fabric.Permille(g.C, g.Me));
        int squads = Org.Squad(g.C, g.Me);
        for (int i = 0; i < squads; i++) g.Me.Staff.First(p => p.Senior == 0).Senior = 1;
        Assert.True(Org.TrainingBonus(g.C, g.Me) > 0);
        Assert.Equal(-(int)MathI.MulPermille(g.C.Fabric.CutoverLoss, 1000 - g.C.Fabric.SquadCutPermille), Fabric.Permille(g.C, g.Me));
        g.Me.OrgForm = Org.Functional;
        Assert.Equal(-g.C.Fabric.CutoverLoss, Fabric.Permille(g.C, g.Me));
    }

    [Fact]
    public void An_incident_on_a_model_with_integrations_costs_some_of_them_and_only_then()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        ModelState bare = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, m.Id));
        m.Integrations = 500;
        Integrations.OnIncident(g.C, g.State, g.Me, m);
        Assert.Equal(500 - 500 * g.C.Integrations.IncidentPermille / 1000, m.Integrations);
        Integrations.OnIncident(g.C, g.State, g.Me, bare);
        Assert.Equal(0, bare.Integrations);
        Game off = Lab(c => c.Integrations.Enabled = 0);
        ModelState om = Selling(off);
        om.Integrations = 500;
        Integrations.OnIncident(off.C, off.State, off.Me, om);
        Assert.Equal(500, om.Integrations);
    }

    [Fact]
    public void A_real_incident_reaches_the_register_and_the_integrations()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 1000);
        ModelState m = Selling(g);
        m.Alignment = 0;
        m.DisplayedAlignment = 0;
        g.Do(Command.OpenApi(g.Id, m.Id));
        m.Integrations = 500;
        g.Do(Command.JoinRegister(g.Id));
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(m.Incidents > 0);
        Assert.Equal(1, g.Me.DiscOpen);
        Assert.True(m.Integrations < 500);
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Game.New(5, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Assert.Equal(0, g.Me.ApiMoved + g.Me.ApiLost + g.Me.CfGranted + g.Me.CfFindings + g.Me.OrgForm + g.Me.OrgReorgs + g.Me.FabTier + g.Me.FabBuild + g.Me.DiscOn + g.Me.DiscFindings + g.Me.DiscFiled);
        Assert.All(g.Me.Models, m => Assert.Equal(0, m.Integrations + m.ApiState + m.CertStage));
        foreach (EventKind k in new[] { EventKind.ApiOpened, EventKind.ConformityStarted, EventKind.OrgChanged, EventKind.FabricOrdered, EventKind.RegisterChanged, EventKind.ReportLate, EventKind.InspectionHeld })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        ModelState next = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, m.Id));
        g.Do(Command.OpenApi(g.Id, next.Id));
        g.Do(Command.DeprecateApi(g.Id, m.Id, next.Id, 20));
        g.Do(Command.SetOrg(g.Id, 1));
        g.Do(Command.OrderFabric(g.Id));
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        g.Do(Command.StartConformity(g.Id, next.Id, 1));
        g.Weeks(8);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 12);
        g.State.Tick = (2022 - 1995) * g.C.Calendar.WeeksPerYear;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int api = 0, cert = 0, org = 0, fabric = 0, joined = 0, filed = 0;
        for (int week = 0; week < 300 && !g.Me.Bankrupt; week++)
        {
            g.Me.ResearchPoints += 20_000;
            List<ModelState> selling = g.Me.Models.Where(x => x.Status == ModelStatus.Released).OrderBy(x => x.Id).ToList();
            ModelState first = selling.FirstOrDefault();
            ModelState last = selling.LastOrDefault();
            if (week % 25 == 3) g.Try(Command.StartTraining(g.Id, "mini", 0));
            if (first != null && week % 40 == 5 && g.Try(Command.OpenApi(g.Id, first.Id)).Ok) api++;
            if (first != null && last != null && first != last && week % 60 == 30)
            {
                g.Try(Command.OpenApi(g.Id, last.Id));
                g.Try(Command.DeprecateApi(g.Id, first.Id, last.Id, 8 + week / 60 % 3 * 8));
            }
            if (first != null && week % 60 == 45) g.Try(Command.CancelDeprecation(g.Id, first.Id));
            if (first != null && week % 70 == 7)
            {
                first.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
                if (g.Try(Command.StartConformity(g.Id, first.Id, 1 + week / 70 % 2)).Ok) cert++;
            }
            if (first != null && week % 70 == 20) g.Try(Command.SubmitEvidence(g.Id, first.Id));
            if (first != null && week % 70 == 60) g.Try(Command.WithdrawConformity(g.Id, first.Id));
            if (week % 45 == 2 && g.Try(Command.SetOrg(g.Id, 1 + week / 45 % 3)).Ok) org++;
            if (week % 50 == 8 && g.Try(Command.OrderFabric(g.Id)).Ok) fabric++;
            if (week % 50 == 30) g.Try(Command.CancelFabric(g.Id));
            if (week % 80 == 1 && g.Try(Command.JoinRegister(g.Id)).Ok) joined++;
            if (week % 6 == 4 && g.Try(Command.FileReport(g.Id, 1 + week / 6 % 2)).Ok) filed++;
            if (week % 80 == 70) g.Try(Command.LeaveRegister(g.Id));
            if (week % 9 == 0 && first != null && g.Me.DiscOn > 0) Disclosure.OnIncident(g.C, g.Me, first);
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(api > 0, "no API was ever opened");
        Assert.True(cert > 0, "no conformity assessment was ever started");
        Assert.True(org > 0, "no structure was ever chosen");
        Assert.True(fabric > 0, "no fabric was ever ordered");
        Assert.True(joined > 0, "the register was never joined");
        Assert.True(filed > 0, "no report was ever filed");
    }
}
