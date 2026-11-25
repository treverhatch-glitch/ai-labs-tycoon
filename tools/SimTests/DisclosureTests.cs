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

public class DisclosureTests
{
    static Game Lab(Action<GameConfig> tweak = null, int safety = 3)
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
        }).WithStarterLab(researchers: 2, engineers: 3, safety: safety, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static int Trust(Game g, ModelState m) { return TrustSystem.Of(g.C, g.Me, m.Sectors[0]); }

    [Fact]
    public void A_lab_that_never_joins_has_no_register_and_incidents_open_nothing()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        Disclosure.OnIncident(g.C, g.Me, m);
        g.Weeks(60);
        Assert.Equal(0, g.Me.DiscOpen);
        Assert.Equal(0, g.Me.DiscFindings);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ReportLate || e.Kind == EventKind.InspectionHeld);
        g.Refused(Command.FileReport(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.LeaveRegister(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Joining_is_checked_and_an_incident_opens_a_report()
    {
        Game off = Lab(c => c.Disclosure.Enabled = 0);
        off.Refused(Command.JoinRegister(off.Id), CommandError.InvalidArgument);
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        g.Refused(Command.JoinRegister(g.Id), CommandError.InvalidArgument);
        Assert.Equal(g.C.Disclosure.InspectWeeks, g.Me.DiscInspect);
        g.Refused(Command.FileReport(g.Id, 1), CommandError.InvalidArgument);
        Disclosure.OnIncident(g.C, g.Me, m);
        Assert.Equal(1, g.Me.DiscOpen);
        Assert.Equal(g.C.Disclosure.ReportWeeks, g.Me.DiscLeft);
        g.Refused(Command.FileReport(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.LeaveRegister(g.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_real_incident_opens_a_report_through_the_safety_system()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 1000);
        ModelState m = Selling(g);
        m.Alignment = 0;
        m.DisplayedAlignment = 0;
        g.Do(Command.JoinRegister(g.Id));
        int before = m.Incidents;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(m.Incidents > before);
        Assert.Equal(1, g.Me.DiscOpen);
        Assert.Equal(m.Id, g.Me.DiscModel);
    }

    [Fact]
    public void A_summary_closes_the_report_now_earns_trust_and_leaves_a_finding()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        int trust = Trust(g, m);
        g.Do(Command.FileReport(g.Id, 1));
        Assert.Equal(0, g.Me.DiscOpen);
        Assert.Equal(0, g.Me.DiscLeft);
        Assert.Equal(1, g.Me.DiscFindings);
        Assert.Equal(1, g.Me.DiscFiled);
        Assert.True(Trust(g, m) > trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReportFiled && e.Pick == 1 && e.SubjectId == m.Id);
    }

    [Fact]
    public void A_full_report_takes_weeks_and_safety_people_and_clears_a_finding()
    {
        Game thin = Lab(null, 1);
        ModelState tm = Selling(thin);
        thin.Do(Command.JoinRegister(thin.Id));
        Disclosure.OnIncident(thin.C, thin.Me, tm);
        thin.Refused(Command.FileReport(thin.Id, 2), CommandError.NotEnoughStaff);
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        g.Me.DiscFindings = 2;
        Disclosure.OnIncident(g.C, g.Me, m);
        int trust = Trust(g, m);
        g.Do(Command.FileReport(g.Id, 2));
        g.Refused(Command.FileReport(g.Id, 2), CommandError.InvalidArgument);
        Assert.Equal(1, g.Me.DiscOpen);
        g.Weeks(g.C.Disclosure.FullWeeks);
        Assert.Equal(0, g.Me.DiscOpen);
        Assert.Equal(1, g.Me.DiscFindings);
        Assert.True(Trust(g, m) - trust >= g.C.Disclosure.TrustFull - 5);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReportFiled && e.Pick == 2);
    }

    [Fact]
    public void A_report_left_too_long_is_late_fined_and_leaves_a_finding()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        long before = g.Me.Ledger.Lifetime.Fines;
        g.Weeks(g.C.Disclosure.ReportWeeks);
        Assert.Equal(0, g.Me.DiscOpen);
        Assert.Equal(1, g.Me.DiscFindings);
        Assert.Equal(before - g.C.Disclosure.LateFine, g.Me.Ledger.Lifetime.Fines);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReportLate);
    }

    [Fact]
    public void A_full_report_that_starts_too_late_still_clears_the_finding_after_the_fine()
    {
        Game g = Lab(c => { c.Disclosure.ReportWeeks = 2; c.Disclosure.FullWeeks = 3; });
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        g.Do(Command.FileReport(g.Id, 2));
        g.Weeks(2);
        Assert.Equal(1, g.Me.DiscFindings);
        Assert.Equal(0, g.Me.DiscOpen);
        g.Weeks(1);
        Assert.Equal(0, g.Me.DiscFindings);
        Assert.Equal(0, g.Me.DiscFiled);
    }

    [Fact]
    public void Too_many_incidents_at_once_go_unreported_and_count_as_findings()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        for (int i = 0; i < g.C.Disclosure.MaxOpen + 2; i++) Disclosure.OnIncident(g.C, g.Me, m);
        Assert.Equal(g.C.Disclosure.MaxOpen, g.Me.DiscOpen);
        Assert.Equal(2, g.Me.DiscFindings);
    }

    [Fact]
    public void An_inspection_fines_a_lab_with_too_many_findings_and_halves_them()
    {
        Game g = Lab();
        g.Do(Command.JoinRegister(g.Id));
        g.Me.DiscFindings = 4;
        long before = g.Me.Ledger.Lifetime.Fines;
        g.Weeks(g.C.Disclosure.InspectWeeks);
        Assert.Equal(before - 4L * g.C.Disclosure.InspectFine, g.Me.Ledger.Lifetime.Fines);
        Assert.Equal(2, g.Me.DiscFindings);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.InspectionHeld && e.Pick == 0);
        g.Weeks(g.C.Disclosure.InspectWeeks);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.InspectionHeld && e.Pick == 1);
        Assert.Equal(2, g.Me.DiscFindings);
        g.Do(Command.LeaveRegister(g.Id));
        Assert.Equal(0, g.Me.DiscInspect);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_register()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        g.Weeks(1);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 113, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.DiscOn);
        Assert.Equal(0, lab.DiscOpen);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
