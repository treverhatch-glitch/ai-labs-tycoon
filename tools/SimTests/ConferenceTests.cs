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

/// <summary>Conference submissions: papers out for review with a venue and an effort, decided on a date by the lab's prestige then.</summary>
public class ConferenceTests
{
    static Game Lab(Action<GameConfig> tweak = null, int warm = 40)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 2, gpus: 8);
        g.Weeks(warm);
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Conference.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Conference.Enabled = 2;
        c.Conference.MaxPending = 0;
        c.Conference.MainCost = 0;
        c.Conference.WorkshopWeeks = 0;
        c.Conference.MainBar = -1;
        c.Conference.EffortPrestige = 501;
        c.Conference.AcceptPrestige = 501;
        c.Conference.RefundPermille = 1000;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxPending", "mainCost", "workshopWeeks", "mainBar", "effortPrestige", "acceptPrestige", "refundPermille" })
            Assert.True(problems.Any(p => p.Contains("conference." + key)), key + " in " + string.Join(" | ", problems));
        c = TestData.Config();
        c.Conference.WithdrawPermille = 100;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("withdrawPermille"));
    }

    [Fact]
    public void A_lab_that_submits_nothing_is_exactly_as_it_was()
    {
        Game g = Lab();
        g.Weeks(40);
        Assert.Empty(g.Me.Submissions);
        Assert.Empty(Of(g, EventKind.PaperSubmitted));
        Assert.Equal(0, g.Me.Prestige);
    }

    [Fact]
    public void A_workshop_paper_costs_points_and_is_accepted_when_the_review_is_over()
    {
        Game g = Lab();
        long cost = Conference.Cost(g.C, g.State, g.Me, 1, 1);
        Assert.Equal(Prestige.PaperCost(g.C, g.State, g.Me), cost);
        long points = g.Me.ResearchPoints;
        int rep = g.Me.Reputation;
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        Submission paper = g.Me.Submissions.Single();
        Assert.Equal(g.State.Tick + 6, paper.Due);
        Assert.Equal(cost, paper.Cost);
        Assert.Equal(points - cost, g.Me.ResearchPoints);
        GameEvent sent = Of(g, EventKind.PaperSubmitted).Single();
        Assert.Equal(1, sent.Pick);
        Assert.Contains("workshop", EventText.Describe(g.C, g.State, sent));
        g.Weeks(5);
        Assert.Single(g.Me.Submissions);
        g.Weeks(1);
        Assert.Empty(g.Me.Submissions);
        GameEvent decided = Of(g, EventKind.PaperDecided).Single();
        Assert.Equal(1, decided.Pick);
        Assert.Equal(150, decided.Amount);
        Assert.True(g.Me.Prestige > 0);
        Assert.Equal(rep + 20, g.Me.Reputation);
        Assert.Contains("accepted", EventText.Describe(g.C, g.State, decided));
    }

    [Fact]
    public void A_main_conference_paper_needs_effort_to_clear_its_bar_and_a_rejection_returns_half()
    {
        Game weak = Lab();
        Game strong = Lab();
        long cost1 = Conference.Cost(weak.C, weak.State, weak.Me, 2, 1);
        weak.Do(Command.SubmitPaper(weak.Id, 2, 1));
        strong.Do(Command.SubmitPaper(strong.Id, 2, 3));
        Assert.Equal(cost1 * 3, strong.Me.Submissions.Single().Cost);
        weak.Weeks(12);
        strong.Weeks(12);
        GameEvent no = Of(weak, EventKind.PaperDecided).Single();
        Assert.Equal(0, no.Pick);
        Assert.Equal(cost1 / 2, no.Amount);
        Assert.Equal(0, weak.Me.Prestige);
        Assert.Contains("rejected", EventText.Describe(weak.C, weak.State, no));
        GameEvent yes = Of(strong, EventKind.PaperDecided).Single();
        Assert.Equal(1, yes.Pick);
        Assert.Equal(300, yes.Amount);
    }

    [Fact]
    public void Prestige_fades_during_a_long_review_so_the_same_paper_can_be_rejected_later()
    {
        Game g = Lab();
        g.GrantResearch(10_000_000);
        g.Do(Command.PublishPaper(g.Id));
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(400, g.Me.Prestige);
        Assert.True(Conference.Standing(g.C, g.Me, 2) >= 600);
        g.Do(Command.SubmitPaper(g.Id, 3, 2));
        Game quick = Lab();
        quick.GrantResearch(10_000_000);
        quick.Do(Command.PublishPaper(quick.Id));
        quick.Do(Command.PublishPaper(quick.Id));
        quick.Do(Command.SubmitPaper(quick.Id, 3, 3));
        g.Weeks(24);
        quick.Weeks(24);
        Assert.Equal(0, Of(g, EventKind.PaperDecided).Single().Pick);
        Assert.Equal(1, Of(quick, EventKind.PaperDecided).Single().Pick);
        Assert.True(quick.Me.Prestige > g.Me.Prestige);
    }

    [Fact]
    public void Withdrawing_returns_most_of_the_points_and_nothing_else()
    {
        Game g = Lab();
        long points = g.Me.ResearchPoints;
        g.Do(Command.SubmitPaper(g.Id, 2, 2));
        long cost = g.Me.Submissions.Single().Cost;
        g.Do(Command.WithdrawPaper(g.Id, 0));
        Assert.Empty(g.Me.Submissions);
        Assert.Equal(points - cost + cost * 800 / 1000, g.Me.ResearchPoints);
        GameEvent e = Of(g, EventKind.PaperDecided).Single();
        Assert.Equal(2, e.Pick);
        Assert.Contains("withdrew", EventText.Describe(g.C, g.State, e));
        g.Weeks(30);
        Assert.Equal(0, g.Me.Prestige);
        Assert.Single(Of(g, EventKind.PaperDecided));
    }

    [Fact]
    public void The_pipeline_holds_three_papers_and_decides_them_oldest_first()
    {
        Game g = Lab();
        g.Do(Command.SubmitPaper(g.Id, 2, 3));
        g.Weeks(2);
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Refused(Command.SubmitPaper(g.Id, 1, 1), CommandError.InvalidArgument);
        Assert.Equal(3, g.Me.Submissions.Count);
        g.Weeks(8);
        Assert.Equal(2, Of(g, EventKind.PaperDecided).Count);
        Assert.Single(g.Me.Submissions);
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Weeks(12);
        Assert.Empty(g.Me.Submissions);
        Assert.Equal(4, Of(g, EventKind.PaperDecided).Count);
    }

    [Fact]
    public void Refused_submissions_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.SubmitPaper(g.Id, 0, 1), CommandError.InvalidArgument);
        g.Refused(Command.SubmitPaper(g.Id, 4, 1), CommandError.InvalidArgument);
        g.Refused(Command.SubmitPaper(g.Id, 1, 0), CommandError.InvalidArgument);
        g.Refused(Command.SubmitPaper(g.Id, 1, 4), CommandError.InvalidArgument);
        g.Refused(Command.WithdrawPaper(g.Id, 0), CommandError.UnknownId);
        g.Refused(Command.WithdrawPaper(g.Id, -1), CommandError.UnknownId);
        Assert.Equal(d, g.Sim.Digest());
        Game poor = Lab(null, 0);
        poor.Me.ResearchPoints = 0;
        d = poor.Sim.Digest();
        poor.Refused(Command.SubmitPaper(poor.Id, 1, 1), CommandError.NotEnoughResearch);
        Assert.Equal(d, poor.Sim.Digest());
        Game off = Lab(c => c.Conference.Enabled = 0);
        off.Refused(Command.SubmitPaper(off.Id, 1, 1), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.SubmitPaper(rival.Id, 1, 1)).Ok);
        Assert.False(w.Sim.Apply(Command.WithdrawPaper(rival.Id, 0)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_papers_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SubmitPaper(g.Id, 2, 2));
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(2, back.State.Lab(g.Id).Submissions.Count);
        Assert.Equal(g.Me.Submissions[0].Due, back.State.Lab(g.Id).Submissions[0].Due);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.State.Events.Count(e => e.Kind == EventKind.PaperDecided));
    }

    [Fact]
    public void An_older_save_loads_with_no_papers()
    {
        Game g = Lab();
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        string old = SaveSchemaPretend.At(g.C, 75, g.Sim.Save());
        Assert.DoesNotContain("\"submissions\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Empty(lab.Submissions));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_paper()
    {
        Game g = Lab();
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("out for review"));
        Submission paper = g.Me.Submissions[0];
        paper.Tier = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("out for review"));
        paper.Tier = 1;
        paper.Effort = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("out for review"));
        paper.Effort = 1;
        paper.Due = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("out for review"));
        paper.Due = 9;
        g.Me.Submissions.Add(new Submission { Tier = 1, Effort = 1, Cost = 1, Due = 9 });
        g.Me.Submissions.Add(new Submission { Tier = 1, Effort = 1, Cost = 1, Due = 9 });
        g.Me.Submissions.Add(new Submission { Tier = 1, Effort = 1, Cost = 1, Due = 9 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("out for review"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.Submissions.Add(new Submission { Tier = 1, Effort = 1, Cost = 1, Due = 9 });
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("out for review"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        g.Do(Command.SubmitPaper(g.Id, 2, 3));
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Weeks(3);
        g.Do(Command.WithdrawPaper(g.Id, 1));
        g.Weeks(14);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_papers_stays_sound_and_the_books_balance()
    {
        Game g = Lab(null, 20);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 260; i++)
        {
            g.Weeks(1);
            if (i % 13 == 0) g.Try(Command.SubmitPaper(g.Id, 1 + i / 13 % 3, 1 + i / 13 % 3));
            if (i % 52 == 30) g.Try(Command.WithdrawPaper(g.Id, 0));
        }
        Assert.NotEmpty(Of(g, EventKind.PaperDecided));
        Assert.Contains(Of(g, EventKind.PaperDecided), e => e.Pick == 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
