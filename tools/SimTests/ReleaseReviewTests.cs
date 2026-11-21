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

public class ReleaseReviewTests
{
    const string Strict = "enterprise-knowledge";
    const string Open = "spam-search";

    static Game GateYear(long seed = 77, int safety = 2, Action<GameConfig> tweak = null)
    {
        return ExpansionTestSupport.SoloAtYear(2025, tweak: c =>
        {
            c.ReleaseReview.OpensYear = 2017;
            c.ReleaseReview.AppealSuccessPermille = 1000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(safety: safety, gpus: 12);
    }

    static void AdvanceToMonthStart(Game g)
    {
        int w = g.C.Calendar.WeeksPerMonth;
        int rem = g.Tick % w;
        if (rem == 0) g.Weeks(w);
        else g.Weeks(w - rem);
    }

    static ModelState Trained(Game g, int align = 520, int cap = 80_000, int displayed = 0)
    {
        var m = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Gate", Status = ModelStatus.Trained,
            Capability = cap, CapBase = cap, Alignment = align, DisplayedAlignment = displayed > 0 ? displayed : align,
        };
        g.Me.Models.Add(m);
        return m;
    }

    [Fact]
    public void Before_opens_year_release_needs_no_review()
    {
        var g = ExpansionTestSupport.SoloAtYear(2010).WithStarterLab();
        var m = Trained(g, 600, 50_000);
        g.Do(Command.ReleaseModel(g.Id, m.Id, Open));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Consumer_honest_release_skips_review()
    {
        var g = GateYear();
        var m = Trained(g, 600, 50_000);
        g.Do(Command.ReleaseModel(g.Id, m.Id, Open));
        Assert.Equal(ModelStatus.Released, m.Status);
        Assert.Empty(g.Me.ReleaseReviews);
    }

    [Fact]
    public void Strict_sector_requires_review_before_release()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, Strict), CommandError.ReleaseReviewRequired);
    }

    [Fact]
    public void Sandbag_into_consumer_requires_review()
    {
        var g = GateYear();
        var m = Trained(g, 400, 50_000);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 700, Open), CommandError.ReleaseReviewRequired);
    }

    [Fact]
    public void Submit_queues_and_month_approves_honest_strict_package()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        Assert.Single(g.Me.ReleaseReviews);
        Assert.Equal(ReleaseReviewPhase.Submitted, g.Me.ReleaseReviews[0].Phase);
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.Approved, g.Me.ReleaseReviews[0].Phase);
        g.Do(Command.ReleaseModel(g.Id, m.Id, Strict));
        Assert.Equal(ModelStatus.Released, m.Status);
        Assert.Empty(g.Me.ReleaseReviews);
    }

    [Fact]
    public void Sandbag_sent_back_then_revise_and_approve()
    {
        var g = GateYear();
        var m = Trained(g, 400, 50_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 700, Open));
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.NeedsRevision, g.Me.ReleaseReviews[0].Phase);
        g.Do(Command.ReviseReleaseReview(g.Id, m.Id, 400));
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.Approved, g.Me.ReleaseReviews[0].Phase);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 400, Open));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Appeal_can_approve_from_revision()
    {
        var g = GateYear(tweak: c => c.ReleaseReview.AppealSuccessPermille = 1000);
        var m = Trained(g, 300, 50_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Open));
        AdvanceToMonthStart(g);
        g.Do(Command.AppealReleaseReview(g.Id, m.Id));
        Assert.Equal(ReleaseReviewPhase.Approved, g.Me.ReleaseReviews[0].Phase);
    }

    [Fact]
    public void Appeal_failure_rejects()
    {
        var g = GateYear(tweak: c => c.ReleaseReview.AppealSuccessPermille = 0);
        var m = Trained(g, 300, 50_000);
        int rep = g.Me.Reputation;
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Open));
        AdvanceToMonthStart(g);
        g.Do(Command.AppealReleaseReview(g.Id, m.Id));
        Assert.Equal(ReleaseReviewPhase.Rejected, g.Me.ReleaseReviews[0].Phase);
        Assert.True(g.Me.Reputation < rep);
    }

    [Fact]
    public void Low_alignment_strict_package_rejected()
    {
        var g = GateYear(tweak: c => c.ReleaseReview.MinAlignmentStrictSector = 500);
        var m = Trained(g, 300, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 300, Strict));
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.Rejected, g.Me.ReleaseReviews[0].Phase);
    }

    [Fact]
    public void Rejected_strict_sector_marks_incident_month()
    {
        var g = GateYear(tweak: c => c.ReleaseReview.MinAlignmentStrictSector = 500);
        var m = Trained(g, 300, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 300, Strict));
        AdvanceToMonthStart(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReleaseReviewRejected && e.LabId == g.Id);
    }

    [Fact]
    public void Withdraw_clears_non_approved_case()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        g.Do(Command.WithdrawReleaseReview(g.Id, m.Id));
        Assert.Equal(ReleaseReviewPhase.Withdrawn, g.Me.ReleaseReviews[0].Phase);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, Strict), CommandError.ReleaseReviewRequired);
    }

    [Fact]
    public void Expedite_paid_and_runs_first()
    {
        var g = GateYear(safety: 0, tweak: c =>
        {
            c.ReleaseReview.BaseSlotsPerMonth = 1;
            c.ReleaseReview.MaxSlotsPerMonth = 1;
        });
        var a = Trained(g, 600, 90_000);
        var b = Trained(g, 620, 90_000);
        b.Name = "Second";
        g.Do(Command.SubmitReleaseReview(g.Id, a.Id, 600, Strict));
        g.Do(Command.SubmitReleaseReview(g.Id, b.Id, 620, Strict));
        g.Do(Command.ExpediteReleaseReview(g.Id, b.Id));
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.Submitted, g.Me.ReleaseReviews.First(r => r.ModelId == a.Id).Phase);
        Assert.Equal(ReleaseReviewPhase.Approved, g.Me.ReleaseReviews.First(r => r.ModelId == b.Id).Phase);
    }

    [Fact]
    public void Safety_staff_adds_monthly_slots()
    {
        var g = GateYear(safety: 0, tweak: c =>
        {
            c.ReleaseReview.BaseSlotsPerMonth = 1;
            c.ReleaseReview.SlotsPerSafetyStaff = 1;
            c.ReleaseReview.MaxSlotsPerMonth = 3;
        });
        g.Hire("safety", 2);
        var models = new ModelState[3];
        for (int i = 0; i < 3; i++)
        {
            models[i] = Trained(g, 600, 90_000);
            models[i].Name = "M" + i;
            g.Do(Command.SubmitReleaseReview(g.Id, models[i].Id, 600, Strict));
        }
        AdvanceToMonthStart(g);
        int approved = g.Me.ReleaseReviews.Count(r => r.Phase == ReleaseReviewPhase.Approved);
        Assert.Equal(3, approved);
    }

    [Fact]
    public void Backlog_over_cap_hits_reputation()
    {
        var g = GateYear(safety: 0, tweak: c =>
        {
            c.ReleaseReview.MaxBacklogBeforePenalty = 1;
            c.ReleaseReview.BacklogReputationPenalty = 50;
            c.ReleaseReview.BaseSlotsPerMonth = 0;
            c.ReleaseReview.SlotsPerSafetyStaff = 0;
        });
        int rep = g.Me.Reputation;
        var a = Trained(g, 600, 90_000);
        var b = Trained(g, 610, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, a.Id, 600, Strict));
        g.Do(Command.SubmitReleaseReview(g.Id, b.Id, 610, Strict));
        Assert.Equal(2, ReleaseReviewSystem.PendingCount(g.Me));
        ReleaseReviewSystem.RunMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation <= rep - 50);
    }

    [Fact]
    public void Release_must_match_approved_sectors_and_claim()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        AdvanceToMonthStart(g);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 650, Strict), CommandError.ReleaseReviewRequired);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 600, "customer-support"), CommandError.ReleaseReviewRequired);
    }

    [Fact]
    public void Duplicate_open_submit_refused()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        g.Refused(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Submit_refuses_training_model()
    {
        var g = GateYear();
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        int id = g.Me.Models[0].Id;
        g.Refused(Command.SubmitReleaseReview(g.Id, id, 400, Strict), CommandError.WrongModelStatus);
    }

    [Fact]
    public void Submit_not_needed_refused()
    {
        var g = GateYear();
        var m = Trained(g, 600, 50_000);
        g.Refused(Command.SubmitReleaseReview(g.Id, m.Id, 600, Open), CommandError.InvalidArgument);
    }

    [Fact]
    public void Bankrupt_refuses_submit()
    {
        var g = GateYear();
        g.Me.Bankrupt = true;
        var m = Trained(g, 600, 90_000);
        g.Refused(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict), CommandError.LabBankrupt);
    }

    [Fact]
    public void Rival_release_never_needs_review()
    {
        var g = ExpansionTestSupport.DuoAtYear(2025, tweak: c => c.ReleaseReview.OpensYear = 2017).WithStarterLab();
        var rival = g.Rival("openaye");
        var m = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Capability = 90_000, Status = ModelStatus.Trained, Alignment = 500,
        };
        rival.Models.Add(m);
        g.Do(Command.ReleaseModel(rival.Id, m.Id, Strict));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Events_fire_on_submit_approve_and_revision()
    {
        var g = GateYear();
        var m = Trained(g, 300, 50_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Open));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReleaseReviewSubmitted);
        AdvanceToMonthStart(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReleaseReviewRevision);
        g.Do(Command.ReviseReleaseReview(g.Id, m.Id, 300));
        AdvanceToMonthStart(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReleaseReviewApproved);
    }

    [Fact]
    public void Schema_30_migration_adds_empty_reviews()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        string json = g.Sim.Save();
        var loaded = Simulation.Load(g.C, json);
        Assert.Equal(30, loaded.State.Schema);
        Assert.Single(loaded.Player.ReleaseReviews);
    }

    [Fact]
    public void Save_roundtrip_keeps_approved_package()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        AdvanceToMonthStart(g);
        string json = g.Sim.Save();
        var loaded = Simulation.Load(g.C, json);
        Assert.Equal(ReleaseReviewPhase.Approved, loaded.Player.ReleaseReviews[0].Phase);
        var result = loaded.Apply(Command.ReleaseModel(loaded.Player.Id, m.Id, Strict));
        Assert.True(result.Ok);
    }

    [Fact]
    public void Max_revision_rounds_then_reject()
    {
        var g = GateYear(tweak: c =>
        {
            c.ReleaseReview.MaxRevisionRounds = 1;
            c.ReleaseReview.SandbagSendBackGapPermille = 50;
        });
        var m = Trained(g, 400, 50_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 700, Open));
        AdvanceToMonthStart(g);
        g.Do(Command.ReviseReleaseReview(g.Id, m.Id, 650));
        AdvanceToMonthStart(g);
        Assert.Equal(ReleaseReviewPhase.Rejected, g.Me.ReleaseReviews[0].Phase);
    }

    [Fact]
    public void Disclose_does_not_replace_review_for_strict_sector()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        AdvanceToMonthStart(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, Strict));
        g.Refused(Command.DiscloseModelAlignment(g.Id, m.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q207_failed_release_after_approval_keeps_package()
    {
        var g = GateYear();
        var m = Trained(g, 600, 90_000);
        g.Do(Command.SubmitReleaseReview(g.Id, m.Id, 600, Strict));
        AdvanceToMonthStart(g);
        Assert.Single(g.Me.ReleaseReviews, r => r.Phase == ReleaseReviewPhase.Approved);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 600, "customer-support"), CommandError.ReleaseReviewRequired);
        Assert.Single(g.Me.ReleaseReviews, r => r.Phase == ReleaseReviewPhase.Approved);
        Assert.Equal(600, m.Alignment);
    }

    [Fact]
    public void GateOpen_is_player_only()
    {
        var g = GateYear();
        var rival = g.Rival("openaye");
        Assert.False(ReleaseReviewSystem.GateOpen(g.C, g.State, rival));
        Assert.True(ReleaseReviewSystem.GateOpen(g.C, g.State, g.Me));
    }

    [Fact]
    public void Monthly_slots_formula()
    {
        var g = GateYear(safety: 3);
        Assert.Equal(4, ReleaseReviewSystem.MonthlySlots(g.C, g.Me));
    }

    [Fact]
    public void Ten_years_from_2017_with_review_deterministic()
    {
        const int years = 10;
        string a = RunReviewSoak(4242, years);
        string b = RunReviewSoak(4242, years);
        Assert.Equal(a, b);
    }

    static string RunReviewSoak(long seed, int years)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.ReleaseReview.OpensYear = 2017), options));
        g.WithStarterLab(safety: 1, gpus: 16);
        g.GrantResearch(500_000);
        g.ResearchAllReachable();
        int weeks = years * g.C.Calendar.WeeksPerYear;
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        return g.Sim.Digest();
    }
}