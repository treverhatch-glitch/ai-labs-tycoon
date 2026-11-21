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

/// <summary>External safety audits: outsiders publish a model's real alignment; honest numbers earn a certificate that eases governments, a hidden gap is a scandal.</summary>
public class SafetyAuditTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, int safety = 2)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.World.Attention.PreparedPolicyStaff = 100;
            if (tweak != null) tweak(c);
        }).WithStarterLab(safety: safety, gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Release(m, "game-ai", "game-bots");
        return m;
    }

    static ModelState WithGap(Game g, int gap)
    {
        ModelState m = Released(g);
        m.Alignment = 400;
        m.DisplayedAlignment = 400 + gap;
        return m;
    }

    static GameEvent Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        int before = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        return g.State.Events.Skip(before).First(e => e.Kind == EventKind.Incident);
    }

    static List<GameEvent> Audits(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.AuditDone).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void No_model_is_audited_and_nothing_changes_until_the_player_pays()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 100);
        int gap = SafetySystem.AlignmentGap(g.C, m);
        g.Weeks(30);
        Assert.Equal(gap, SafetySystem.AlignmentGap(g.C, m));
        Assert.Equal(0, m.AuditedUntil);
        Assert.False(SafetyAudit.Certified(g.State, m));
        Assert.Equal(0, SafetyAudit.CertifiedModels(g.State, g.Me));
        Assert.Equal(0, SafetyAudit.AttentionReliefPermille(g.C, g.State, g.Me));
        Assert.Equal(0, SafetyAudit.HearingPoints(g.C, g.State, g.Me));
        Assert.Empty(Audits(g));
    }

    // ---- what an audit finds

    [Fact]
    public void Honest_numbers_earn_a_certificate_for_the_data_s_weeks_reputation_and_culture()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 10);
        g.Me.Reputation = 300;
        int culture = SafetyCulture.Level(g.C, g.Me);
        long cost = SafetyAudit.Cost(g.C, g.State, g.Me, m);
        long cash = g.Me.Cash;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(g.Tick + 52, m.AuditedUntil);
        Assert.True(SafetyAudit.Certified(g.State, m));
        Assert.Equal(315, g.Me.Reputation);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        Assert.Equal(culture + 4, SafetyCulture.Level(g.C, g.Me));
        Assert.Equal(cash - cost, g.Me.Cash);
        GameEvent e = Audits(g).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(cost, e.Amount);
        Assert.Contains("has a certificate", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_gap_exactly_at_the_clean_limit_is_still_honest()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 20);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.True(SafetyAudit.Certified(g.State, m));
    }

    [Fact]
    public void A_gap_in_between_is_published_with_no_certificate_and_no_reputation_change()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 40);
        g.Me.Reputation = 300;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(0, m.AuditedUntil);
        Assert.Equal(300, g.Me.Reputation);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        GameEvent e = Audits(g).Single();
        Assert.Equal(-1, e.Pick);
        Assert.Contains("real score", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_hidden_gap_is_a_scandal_when_it_is_published()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 500);
        g.Me.Reputation = 300;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(300 - PressSystem.Damp(g.C, g.Me, 25), g.Me.Reputation);
        Assert.Equal(0, m.AuditedUntil);
        Assert.Equal(m.Alignment, m.DisplayedAlignment);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        GameEvent e = Audits(g).Single();
        Assert.Equal(0, e.Pick);
        Assert.Contains("not the one the lab had been showing", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Reputation_stops_at_the_ends()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Me.Reputation = 995;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(1000, g.Me.Reputation);
        Game h = Lab();
        ModelState bad = WithGap(h, 500);
        h.Me.Reputation = 3;
        h.Do(Command.CommissionAudit(h.Id, bad.Id));
        Assert.Equal(0, h.Me.Reputation);
    }

    [Fact]
    public void A_model_that_shows_less_than_it_is_gets_the_real_score_published_too()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Alignment = 600;
        m.DisplayedAlignment = 500;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(600, m.DisplayedAlignment);
        Assert.True(SafetyAudit.Certified(g.State, m));
    }

    [Fact]
    public void A_trained_model_that_is_not_on_sale_can_be_audited()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.True(SafetyAudit.Certified(g.State, m));
    }

    // ---- refusals

    [Fact]
    public void An_unknown_model_a_model_in_training_and_a_certified_one_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.CommissionAudit(g.Id, 9999), CommandError.UnknownId);
        ModelState training = g.Train("mini", 0);
        g.Refused(Command.CommissionAudit(g.Id, training.Id), CommandError.InvalidArgument);
        g.UntilTrained(training);
        g.Do(Command.CommissionAudit(g.Id, training.Id));
        long cash = g.Me.Cash;
        g.Refused(Command.CommissionAudit(g.Id, training.Id), CommandError.InvalidArgument);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Single(Audits(g));
    }

    [Fact]
    public void Too_few_safety_staff_and_too_little_cash_are_refused_and_change_nothing()
    {
        Game thin = Lab(safety: 1);
        ModelState m = WithGap(thin, 0);
        thin.Refused(Command.CommissionAudit(thin.Id, m.Id), CommandError.NotEnoughStaff);
        Assert.Equal(0, m.AuditedUntil);
        Game poor = Lab(rich: false);
        ModelState p = WithGap(poor, 0);
        poor.Me.Cash = 100;
        poor.Refused(Command.CommissionAudit(poor.Id, p.Id), CommandError.NotEnoughCash);
        Assert.Equal(100, poor.Me.Cash);
        Assert.Equal(0, p.AuditedUntil);
        Assert.Empty(Audits(poor));
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_audits_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.CommissionAudit(rival.Id, 1), CommandError.InvalidArgument);
        Game broke = Lab();
        ModelState m = WithGap(broke, 0);
        broke.Me.Bankrupt = true;
        broke.Refused(Command.CommissionAudit(broke.Id, m.Id), CommandError.LabBankrupt);
        Game off = Lab(c => c.Audit.Weeks = 0);
        ModelState o = WithGap(off, 0);
        off.Refused(Command.CommissionAudit(off.Id, o.Id), CommandError.InvalidArgument);
        Assert.False(SafetyAudit.Enabled(off.C));
    }

    // ---- the bill

    [Fact]
    public void The_bill_is_the_minimum_until_the_model_sells_for_more()
    {
        Game g = Lab();
        ModelState m = Released(g);
        Assert.Equal(25_000, SafetyAudit.Cost(g.C, g.State, g.Me, m));
    }

    [Fact]
    public void The_bill_follows_what_the_model_sells_for_once_that_is_more_than_the_minimum()
    {
        Game g = Lab(c => c.Audit.MinCost = 1);
        ModelState m = Released(g);
        g.Weeks(12);
        long revenue = MarketSystem.ModelRevenue(g.State, g.Id, m.Id);
        Assert.True(revenue > 0, "the model should sell for the test to mean something");
        Assert.Equal(revenue * 3, SafetyAudit.Cost(g.C, g.State, g.Me, m));
        long training = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.Equal(training - revenue * 3, g.Me.Ledger.Lifetime.Training);
    }

    // ---- the certificate

    [Fact]
    public void A_certificate_runs_out_after_the_data_s_weeks_and_the_model_can_be_audited_again()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.C.Safety.IncidentChancePermille = 0;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        g.Weeks(51);
        Assert.True(SafetyAudit.Certified(g.State, m));
        g.Weeks(1);
        Assert.False(SafetyAudit.Certified(g.State, m));
        Assert.Equal(0, SafetyAudit.AttentionReliefPermille(g.C, g.State, g.Me));
        m.Alignment = m.DisplayedAlignment;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Assert.True(SafetyAudit.Certified(g.State, m));
    }

    [Fact]
    public void While_one_model_holds_a_certificate_attention_gains_are_smaller_and_it_does_not_stack_per_model()
    {
        Game g = Lab();
        ModelState a = WithGap(g, 0);
        ModelState b = g.Train("mini", 0);
        g.UntilTrained(b);
        b.Alignment = b.DisplayedAlignment;
        g.Me.IncidentsThisMonth = 6;
        string bloc = g.Me.HomeBlocId;
        AttentionGain none = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
        long afterPolicy = none.FromFootprint + none.FromIncidents + none.FromDominance + none.PolicySaving;
        Assert.True(afterPolicy > 0);
        g.Do(Command.CommissionAudit(g.Id, a.Id));
        AttentionGain one = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
        Assert.Equal(MathI.MulPermille(afterPolicy, 850) - afterPolicy, one.ComplianceSaving);
        g.Do(Command.CommissionAudit(g.Id, b.Id));
        Assert.Equal(2, SafetyAudit.CertifiedModels(g.State, g.Me));
        Assert.Equal(one.ComplianceSaving, WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving);
    }

    [Fact]
    public void A_certificate_and_a_compliance_office_both_take_their_share_off_the_attention()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Me.IncidentsThisMonth = 6;
        string bloc = g.Me.HomeBlocId;
        AttentionGain none = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
        long afterPolicy = none.FromFootprint + none.FromIncidents + none.FromDominance + none.PolicySaving;
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        g.Me.ComplianceLevel = 1;
        Assert.Equal(MathI.MulPermille(afterPolicy, 770) - afterPolicy, WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving);
    }

    [Fact]
    public void A_hearing_goes_better_while_a_certificate_is_held()
    {
        int Rep(bool certified)
        {
            Game g = Lab();
            ModelState m = WithGap(g, 0);
            if (certified) g.Do(Command.CommissionAudit(g.Id, m.Id));
            g.Me.Reputation = 500;
            g.Me.Bloc("us").Level = BlocStanding.Letter;
            g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
            g.Me.Ledger.ThisMonth.Revenue = 0;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            Assert.Equal(BlocStanding.Hearing, g.Me.Bloc("us").Level);
            return g.Me.Reputation;
        }
        int plain = Rep(false);
        Assert.Equal(plain + 5, Rep(true));
    }

    // ---- incidents

    [Fact]
    public void An_incident_pulls_the_certificate_once_and_logs_it()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        Incident(g, m);
        Assert.Equal(0, m.AuditedUntil);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.AuditRevoked);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Contains("pulled the certificate", EventText.Describe(g.C, g.State, e));
        Assert.Equal(0, SafetyAudit.AttentionReliefPermille(g.C, g.State, g.Me));
        Incident(g, m);
        Assert.Single(g.State.Events, x => x.Kind == EventKind.AuditRevoked);
    }

    [Fact]
    public void An_incident_on_one_model_leaves_the_certificate_of_another_alone()
    {
        Game g = Lab();
        ModelState a = WithGap(g, 0);
        ModelState b = g.Train("mini", 0);
        g.UntilTrained(b);
        b.Alignment = b.DisplayedAlignment;
        g.Do(Command.CommissionAudit(g.Id, a.Id));
        g.Do(Command.CommissionAudit(g.Id, b.Id));
        SafetyAudit.OnIncident(g.State, g.Me, a);
        Assert.Equal(0, a.AuditedUntil);
        Assert.True(SafetyAudit.Certified(g.State, b));
        Assert.Equal(1, SafetyAudit.CertifiedModels(g.State, g.Me));
        Assert.Single(g.State.Events, x => x.Kind == EventKind.AuditRevoked && x.SubjectId == a.Id);
    }

    [Fact]
    public void A_rival_s_incident_never_touches_the_player_s_certificates()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        LabState other = new LabState { Id = g.Id + 99 };
        SafetyAudit.OnIncident(g.State, other, m);
        Assert.True(SafetyAudit.Certified(g.State, m));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_certificate_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(m.AuditedUntil, back.Player.Model(m.Id).AuditedUntil);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Audit.MinSafetyStaff = 0);
        g.Autopilot(120);
        ModelState m = g.Me.Models.First(x => x.Status == ModelStatus.Trained || x.Status == ModelStatus.Released);
        Assert.True(g.Try(Command.CommissionAudit(g.Id, m.Id)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_certificate()
    {
        Game g = Lab();
        ModelState m = WithGap(g, 0);
        g.Do(Command.CommissionAudit(g.Id, m.Id));
        string old = SaveSchemaPretend.At(g.C, 50, g.Sim.Save());
        Assert.DoesNotContain("auditedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.Player.Models, x => Assert.Equal(0, x.AuditedUntil));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_certificate_is_reported()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.AuditedUntil = g.Tick + 53;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("audit certificate"));
        m.AuditedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("audit certificate"));
        m.AuditedUntil = g.Tick + 52;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("audit certificate"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(52, c.Audit.Weeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Audit.Weeks = 521;
        c.Audit.MinSafetyStaff = -1;
        c.Audit.RevenueWeeks = 521;
        c.Audit.MinCost = -1;
        c.Audit.CleanGap = 30;
        c.Audit.ShameGap = 20;
        c.Audit.CleanReputation = 201;
        c.Audit.ShameReputation = 201;
        c.Audit.CulturePoints = 101;
        c.Audit.AttentionReliefPermille = 501;
        c.Audit.HearingPoints = 101;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "weeks", "minSafetyStaff", "revenueWeeks", "minCost", "shameGap", "cleanReputation", "shameReputation", "culturePoints", "attentionReliefPermille", "hearingPoints" })
            Assert.Contains(problems, p => p.Contains("audit." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_hold_a_certificate()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.All(lab.Models, m => Assert.Equal(0, m.AuditedUntil)));
        Assert.Empty(Audits(g));
    }

    [Fact]
    public void An_autopilot_game_with_audits_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c => c.Audit.MinSafetyStaff = 0);
        g.Autopilot(60);
        g.Me.Cash = Math.Max(g.Me.Cash, 5_000_000);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int done = 0;
        for (int i = 0; i < 150; i++)
        {
            g.Autopilot(1);
            if (i % 20 == 0 && g.Me.Cash > 300_000)
            {
                ModelState m = g.Me.Models.FirstOrDefault(x => x.Status != ModelStatus.Training && x.Status != ModelStatus.Retired);
                if (m != null && g.Try(Command.CommissionAudit(g.Id, m.Id)).Ok) done++;
            }
        }
        Assert.True(done > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(done, Audits(g).Count);
    }
}
