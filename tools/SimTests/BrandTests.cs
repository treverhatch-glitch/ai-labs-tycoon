using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BrandTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static void Act(Game g, params EventKind[] kinds)
    {
        foreach (EventKind k in kinds) Events.Add(g.State, k, g.Id, 0, 0, "");
        g.State.Tick++;
        Brand.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void A_careers_page_costs_cash_starts_the_score_and_changes_nothing_else()
    {
        Game g = Lab();
        RoleConfig role = g.C.Role("researcher");
        long fee = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        long cash = g.Me.Cash;
        g.Do(Command.OpenCareersPage(g.Id));
        Assert.Equal(cash - g.C.Brand.OpenCost, g.Me.Cash);
        Assert.Equal(g.C.Brand.Start, g.Me.BrandScore);
        Assert.Equal(fee, RolePerks.HireFee(g.C, g.Me, role, g.Tick));
        g.Refused(Command.OpenCareersPage(g.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Without_the_page_nothing_is_scored_and_a_poor_lab_cannot_open_one()
    {
        Game g = Lab();
        Act(g, EventKind.LayoffRound);
        Act(g);
        Assert.Equal(0, g.Me.BrandScore);
        g.Me.Cash = 10;
        g.Refused(Command.OpenCareersPage(g.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Good_people_decisions_raise_the_score_and_cut_the_hire_fee()
    {
        Game g = Lab();
        g.Do(Command.OpenCareersPage(g.Id));
        RoleConfig role = g.C.Role("researcher");
        long fee = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        for (int i = 0; i < 10; i++) Act(g, EventKind.StaffPromoted, EventKind.EquityGranted, EventKind.SabbaticalStarted);
        Assert.True(g.Me.BrandScore > g.C.Brand.Start);
        Assert.True(RolePerks.HireFee(g.C, g.Me, role, g.Tick) < fee);
    }

    [Fact]
    public void Layoffs_quits_and_strikes_drop_it_and_a_bad_name_adds_to_the_fee()
    {
        Game g = Lab();
        g.Do(Command.OpenCareersPage(g.Id));
        RoleConfig role = g.C.Role("researcher");
        long fee = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        Act(g, EventKind.LayoffRound, EventKind.StaffQuit, EventKind.LabourStrike);
        Act(g);
        Assert.Equal(g.C.Brand.Start - 3 * g.C.Brand.BrokenPoints, g.Me.BrandScore);
        Assert.True(RolePerks.HireFee(g.C, g.Me, role, g.Tick) > fee);
    }

    [Fact]
    public void Only_the_labs_own_events_count_and_each_counts_once()
    {
        Game g = Lab();
        g.Do(Command.OpenCareersPage(g.Id));
        Events.Add(g.State, EventKind.StaffQuit, g.Id + 50, 0, 0, "");
        Act(g);
        Act(g);
        Act(g);
        Assert.Equal(g.C.Brand.Start, g.Me.BrandScore);
        Act(g, EventKind.StaffQuit);
        Act(g);
        Act(g);
        Assert.Equal(g.C.Brand.Start - g.C.Brand.BrokenPoints, g.Me.BrandScore);
    }

    static void Sink(Game g)
    {
        g.Do(Command.OpenCareersPage(g.Id));
        for (int i = 0; i < 12 && g.Me.BrandReview == 0; i++)
            Act(g, EventKind.LayoffRound);
        Act(g);
    }

    [Fact]
    public void A_low_score_brings_one_review_that_waits()
    {
        Game g = Lab();
        Sink(g);
        Assert.True(g.Me.BrandScore < g.C.Brand.LowBar);
        Assert.Equal(1, g.Me.BrandReviews);
        Assert.True(g.Me.BrandReview > 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.BadReview);
        Act(g, EventKind.LayoffRound);
        Act(g);
        Assert.Equal(1, g.Me.BrandReviews);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Owning_up_pays_per_person_and_wins_the_score_back()
    {
        Game g = Lab();
        Sink(g);
        int score = g.Me.BrandScore;
        long cost = Brand.RepairCost(g.C, g.Me);
        long cash = g.Me.Cash;
        g.Do(Command.AnswerReview(g.Id, 1));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(score + g.C.Brand.RepairPoints, g.Me.BrandScore);
        Assert.Equal(0, g.Me.BrandReview);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReviewAnswered && e.Pick == 1);
        g.Refused(Command.AnswerReview(g.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Rebutting_costs_reputation_and_leaving_it_costs_score_even_when_nobody_answers()
    {
        Game r = Lab();
        Sink(r);
        long rep = r.Me.Reputation;
        int score = r.Me.BrandScore;
        r.Do(Command.AnswerReview(r.Id, 2));
        Assert.True(r.Me.Reputation < rep);
        Assert.Equal(score, r.Me.BrandScore);
        Game w = Lab();
        Sink(w);
        int before = w.Me.BrandScore;
        for (int i = 0; i < w.C.Brand.ReviewWeeks + 1; i++) Act(w);
        Assert.Equal(0, w.Me.BrandReview);
        Assert.Equal(System.Math.Max(0, before - w.C.Brand.IgnorePoints), w.Me.BrandScore);
        Assert.Contains(w.State.Events, e => e.Kind == EventKind.ReviewAnswered && e.Pick == 0);
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.OpenCareersPage(rival.Id)).Ok);
        Game off = Lab(c => c.Brand.Enabled = 0);
        off.Refused(Command.OpenCareersPage(off.Id), CommandError.InvalidArgument);
        Game s = Lab();
        Sink(s);
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 101, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == s.Id).BrandOn);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
