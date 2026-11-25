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

public class ConformityTests
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
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static ModelState Selling(Game g, int alignment = 800, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        m.Alignment = alignment;
        m.DisplayedAlignment = alignment;
        return m;
    }

    static void Evidence(Game g, ModelState m, bool audit)
    {
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        if (audit) m.AuditedUntil = g.State.Tick + g.C.Audit.Weeks;
    }

    static void ToEvidence(Game g, ModelState m, int scope)
    {
        g.Do(Command.StartConformity(g.Id, m.Id, scope));
        g.Weeks(g.C.Conformity.DossierWeeks);
        Assert.Equal(Conformity.Evidence, m.CertStage);
    }

    [Fact]
    public void A_dossier_costs_research_points_and_needs_a_released_model_and_a_free_slot()
    {
        Game g = Lab(c => c.Conformity.MaxActive = 1);
        ModelState idle = g.Train("mini", 0);
        g.UntilTrained(idle);
        g.Refused(Command.StartConformity(g.Id, idle.Id, 1), CommandError.WrongModelStatus);
        ModelState m = Selling(g);
        ModelState b = Selling(g, 800, "game-bots");
        g.Refused(Command.StartConformity(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.StartConformity(g.Id, 999, 1), CommandError.UnknownId);
        long points = g.Me.ResearchPoints;
        g.Do(Command.StartConformity(g.Id, m.Id, 2));
        Assert.Equal(points - g.C.Conformity.DossierPoints - g.C.Conformity.BroadExtraPoints, g.Me.ResearchPoints);
        g.Refused(Command.StartConformity(g.Id, m.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.StartConformity(g.Id, b.Id, 1), CommandError.InvalidArgument);
        g.Me.ResearchPoints = 1;
        g.Refused(Command.WithdrawConformity(g.Id, b.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_dossier_takes_weeks_and_evidence_cannot_be_submitted_without_a_red_team_result()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.StartConformity(g.Id, m.Id, 1));
        g.Refused(Command.SubmitEvidence(g.Id, m.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Conformity.DossierWeeks);
        g.Refused(Command.SubmitEvidence(g.Id, m.Id), CommandError.MissingPrerequisite);
        Evidence(g, m, false);
        g.Do(Command.SubmitEvidence(g.Id, m.Id));
        Assert.Equal(Conformity.Review, m.CertStage);
        Assert.Equal(g.C.Conformity.ReviewWeeks, m.CertLeft);
    }

    [Fact]
    public void A_broad_scope_also_needs_a_live_external_audit()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        ToEvidence(g, m, 2);
        Evidence(g, m, false);
        g.Refused(Command.SubmitEvidence(g.Id, m.Id), CommandError.MissingPrerequisite);
        Evidence(g, m, true);
        g.Do(Command.SubmitEvidence(g.Id, m.Id));
        Assert.Equal(Conformity.Review, m.CertStage);
    }

    [Fact]
    public void A_model_that_meets_the_bar_is_certified_and_its_sectors_trust_it_more()
    {
        Game g = Lab();
        ModelState m = Selling(g, 800);
        ToEvidence(g, m, 1);
        Evidence(g, m, false);
        m.Alignment = 800;
        m.DisplayedAlignment = 800;
        int trust = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Do(Command.SubmitEvidence(g.Id, m.Id));
        g.Weeks(g.C.Conformity.ReviewWeeks);
        Assert.Equal(Conformity.Certified, m.CertStage);
        Assert.Equal(g.C.Conformity.TermWeeks, m.CertLeft);
        Assert.Equal(1, g.Me.CfGranted);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") > trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ConformityDecided && e.Pick == 1);
    }

    [Fact]
    public void A_low_alignment_or_a_big_claim_gap_ends_in_findings_and_a_remediation_delay()
    {
        Game g = Lab();
        ModelState m = Selling(g, 800);
        ToEvidence(g, m, 1);
        Evidence(g, m, false);
        m.Alignment = g.C.Conformity.BarNarrow - 1;
        g.Do(Command.SubmitEvidence(g.Id, m.Id));
        g.Weeks(g.C.Conformity.ReviewWeeks);
        Assert.Equal(Conformity.Evidence, m.CertStage);
        Assert.Equal(1, m.CertTries);
        Assert.Equal(1, g.Me.CfFindings);
        Assert.Equal(g.C.Conformity.ResubmitWeeks - 0, m.CertLeft);
        g.Refused(Command.SubmitEvidence(g.Id, m.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Conformity.ResubmitWeeks);
        m.Alignment = 700;
        m.DisplayedAlignment = 700 + g.C.Conformity.MaxGap + 1;
        Assert.False(Conformity.Passes(g.C, m, 1));
        m.DisplayedAlignment = 700 + g.C.Conformity.MaxGap;
        Assert.True(Conformity.Passes(g.C, m, 1));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_certified_model_has_a_lower_incident_chance_by_scope()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 400);
        ModelState m = Selling(g, 300);
        int plain = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.True(plain > 0);
        m.CertStage = Conformity.Certified;
        m.CertLeft = 50;
        m.CertScope = 1;
        int narrow = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        m.CertScope = 2;
        int broad = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.True(narrow < plain);
        Assert.True(broad < narrow);
        Assert.Equal((int)MathI.MulPermille(plain, 1000 - g.C.Conformity.CutBroad), broad);
    }

    [Fact]
    public void Surveillance_can_suspend_a_certificate_and_it_expires_at_the_end_of_its_term()
    {
        Game g = Lab();
        ModelState m = Selling(g, 800);
        ToEvidence(g, m, 1);
        Evidence(g, m, false);
        m.Alignment = 800;
        m.DisplayedAlignment = 800;
        g.Do(Command.SubmitEvidence(g.Id, m.Id));
        g.Weeks(g.C.Conformity.ReviewWeeks);
        g.Weeks(g.C.Conformity.SurveillanceAt - 1);
        m.Alignment = g.C.Conformity.BarNarrow - 100;
        m.DisplayedAlignment = m.Alignment;
        int trust = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Weeks(1);
        Assert.Equal(0, m.CertStage);
        Assert.Equal(1, g.Me.CfSuspended);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ConformityEnded && e.Pick == 2);
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") < trust);

        Game e = Lab();
        ModelState em = Selling(e, 800);
        ToEvidence(e, em, 1);
        Evidence(e, em, false);
        em.Alignment = 800;
        em.DisplayedAlignment = 800;
        e.Do(Command.SubmitEvidence(e.Id, em.Id));
        e.Weeks(e.C.Conformity.ReviewWeeks + e.C.Conformity.TermWeeks);
        Assert.Equal(0, em.CertStage);
        Assert.Contains(e.State.Events, x => x.Kind == EventKind.ConformityEnded && x.Pick == 1);
    }

    [Fact]
    public void Withdrawing_or_losing_the_model_from_sale_ends_the_assessment()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.StartConformity(g.Id, m.Id, 1));
        g.Do(Command.WithdrawConformity(g.Id, m.Id));
        Assert.Equal(0, m.CertStage);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ConformityEnded && e.Pick == 0);
        g.Do(Command.StartConformity(g.Id, m.Id, 1));
        m.Status = ModelStatus.Retired;
        g.Weeks(1);
        Assert.Equal(0, m.CertStage);
    }

    [Fact]
    public void The_switch_rivals_and_older_saves_are_handled()
    {
        Game off = Lab(c => c.Conformity.Enabled = 0);
        ModelState om = Selling(off);
        off.Refused(Command.StartConformity(off.Id, om.Id, 1), CommandError.InvalidArgument);
        Game g = Lab();
        ModelState m = Selling(g);
        ToEvidence(g, m, 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 110, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.CertStage));
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
