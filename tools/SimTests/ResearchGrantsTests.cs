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

/// <summary>Research grants: apply, wait for the panel, take the money, owe the papers; pay part back when they do not come.</summary>
public class ResearchGrantsTests
{
    static Game Lab(Action<GameConfig> tweak = null, int researchers = 4)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: researchers, engineers: 2, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 500;
        g.Me.GrantPerWeek = 0;
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind k) { return g.State.Events.Where(e => e.Kind == k).ToList(); }

    static void Papers(Game g, int n) { for (int i = 0; i < n; i++) g.Do(Command.PublishPaper(g.Id)); }

    // ---- the data

    [Fact]
    public void Grants_ship_and_the_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.ResearchGrants.MaxTier);
        Assert.Equal(5000, c.ResearchGrants.ApplyCostPerTier);
        Assert.Equal(60000, c.ResearchGrants.AwardPerTier);
        Assert.Equal(250, c.ResearchGrants.BarPerTier);
        Assert.Empty(ConfigValidator.Problems(c));
        c.ResearchGrants.MaxTier = 6;
        c.ResearchGrants.AwardPerTier = 1;
        c.ResearchGrants.BarPerTier = -1;
        c.ResearchGrants.ReputationWeightPermille = 2000;
        c.ResearchGrants.ReviewWeeks = 0;
        c.ResearchGrants.DeliverWeeks = 105;
        c.ResearchGrants.ClawbackPermille = 1001;
        c.ResearchGrants.CompletedReputation = 101;
        c.ResearchGrants.FailedReputation = 201;
        c.ResearchGrants.CooldownWeeks = -1;
        c.ResearchGrants.MinResearchers = -1;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxTier", "awardPerTier", "barPerTier", "reputationWeightPermille", "reviewWeeks", "deliverWeeks", "clawbackPermille", "completedReputation", "failedReputation", "cooldownWeeks", "minResearchers" })
            Assert.Contains(problems, p => p.Contains("researchGrants." + key));
    }

    [Fact]
    public void Nothing_happens_until_the_player_applies()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Weeks(60);
        Assert.Equal(ResearchGrants.None, g.Me.ResearchGrantStage);
        Assert.Empty(Of(g, EventKind.GrantApplied));
        Assert.Empty(Of(g, EventKind.GrantDecided));
        Assert.True(g.Me.Cash <= cash + g.Me.Ledger.Lifetime.Income);
    }

    [Fact]
    public void The_helpers_score_a_lab_by_prestige_and_a_share_of_reputation()
    {
        Game g = Lab();
        Assert.Equal(150, ResearchGrants.Score(g.C, g.Me));
        g.Me.Prestige = 400;
        Assert.Equal(550, ResearchGrants.Score(g.C, g.Me));
        Assert.Equal(250, ResearchGrants.Bar(g.C, 1));
        Assert.Equal(750, ResearchGrants.Bar(g.C, 3));
        Assert.Equal(180000, ResearchGrants.Award(g.C, 3));
        Assert.Equal(10000, ResearchGrants.ApplyCost(g.C, 2));
        Assert.True(ResearchGrants.WouldPass(g.C, g.Me, 2));
        Assert.False(ResearchGrants.WouldPass(g.C, g.Me, 3));
    }

    // ---- applying

    [Fact]
    public void Applying_costs_the_fee_opens_a_review_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long training = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.ApplyForGrant(g.Id, 2));
        Assert.Equal(cash - 10000, g.Me.Cash);
        Assert.Equal(training - 10000, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(ResearchGrants.InReview, g.Me.ResearchGrantStage);
        Assert.Equal(2, g.Me.ResearchGrantTier);
        Assert.Equal(g.State.Tick + 6, g.Me.ResearchGrantTick);
        GameEvent e = Assert.Single(Of(g, EventKind.GrantApplied));
        Assert.Equal(2, e.SubjectId);
        Assert.Equal(10000, e.Amount);
        Assert.Contains("tier 2", EventText.Describe(g.C, g.State, e));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void An_application_is_refused_without_change_for_each_wrong_case()
    {
        Game g = Lab();
        string before = g.Sim.Digest();
        g.Refused(Command.ApplyForGrant(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.ApplyForGrant(g.Id, 4), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        g.Me.Cash = 4999;
        before = g.Sim.Digest();
        g.Refused(Command.ApplyForGrant(g.Id, 1), CommandError.NotEnoughCash);
        Assert.Equal(before, g.Sim.Digest());
        Game few = Lab(researchers: 1);
        few.Refused(Command.ApplyForGrant(few.Id, 1), CommandError.NotEnoughStaff);
        Game off = Lab(c => c.ResearchGrants.MaxTier = 0);
        off.Refused(Command.ApplyForGrant(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Only_one_grant_at_a_time_and_a_rival_cannot_apply()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 1));
        string before = g.Sim.Digest();
        g.Refused(Command.ApplyForGrant(g.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.ApplyForGrant(rival.Id, 1)).Ok);
    }

    // ---- the verdict

    [Fact]
    public void A_lab_that_clears_the_bar_is_paid_and_owes_one_paper_for_each_tier()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 2));
        Papers(g, 2);
        long cash = g.Me.Cash;
        long grants = g.Me.Ledger.Lifetime.Grants;
        g.Weeks(6);
        Assert.Equal(ResearchGrants.Owed, g.Me.ResearchGrantStage);
        Assert.Equal(2, g.Me.ResearchGrantOwed);
        Assert.Equal(grants + 120000, g.Me.Ledger.Lifetime.Grants);
        Assert.True(g.Me.Cash > cash);
        GameEvent e = Assert.Single(Of(g, EventKind.GrantDecided));
        Assert.Equal(1, e.Pick);
        Assert.Equal(120000, e.Amount);
        Assert.Contains("owes the panel 2 papers", EventText.Describe(g.C, g.State, e));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_lab_below_the_bar_loses_only_the_fee_and_waits()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 3));
        long grants = g.Me.Ledger.Lifetime.Grants;
        g.Weeks(6);
        Assert.Equal(ResearchGrants.None, g.Me.ResearchGrantStage);
        Assert.Equal(0, g.Me.ResearchGrantTier);
        Assert.Equal(grants, g.Me.Ledger.Lifetime.Grants);
        Assert.Equal(0, Assert.Single(Of(g, EventKind.GrantDecided)).Pick);
        Assert.Contains("declined", EventText.Describe(g.C, g.State, Of(g, EventKind.GrantDecided)[0]));
        Assert.True(g.Me.ResearchGrantLockedUntil > g.State.Tick);
        g.Refused(Command.ApplyForGrant(g.Id, 1), CommandError.GrantLocked);
    }

    [Fact]
    public void Papers_in_the_review_count_towards_the_score_but_not_towards_what_is_owed()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 1));
        Assert.False(ResearchGrants.WouldPass(g.C, g.Me, 3));
        Papers(g, 1);
        Assert.Equal(0, g.Me.ResearchGrantOwed);
        Assert.Equal(ResearchGrants.InReview, g.Me.ResearchGrantStage);
        g.Weeks(6);
        Assert.Equal(1, g.Me.ResearchGrantOwed);
    }

    [Fact]
    public void The_verdict_uses_the_score_when_the_panel_meets_not_when_the_lab_applied()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 3));
        Papers(g, 4);
        Assert.True(ResearchGrants.WouldPass(g.C, g.Me, 3));
        g.Weeks(5);
        g.Me.Prestige = 0;
        g.Me.Reputation = 0;
        g.Weeks(1);
        Assert.Equal(0, Assert.Single(Of(g, EventKind.GrantDecided)).Pick);
    }

    // ---- delivering and failing

    [Fact]
    public void Publishing_the_papers_owed_settles_the_grant_with_reputation_and_a_wait()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 2));
        Papers(g, 2);
        g.Weeks(6);
        g.Me.Reputation = 400;
        Papers(g, 1);
        Assert.Equal(1, g.Me.ResearchGrantOwed);
        Assert.Equal(ResearchGrants.Owed, g.Me.ResearchGrantStage);
        int rep = g.Me.Reputation;
        Papers(g, 1);
        Assert.Equal(ResearchGrants.None, g.Me.ResearchGrantStage);
        Assert.Equal(rep + 8 + 15, g.Me.Reputation);
        GameEvent e = Assert.Single(Of(g, EventKind.GrantSettled));
        Assert.Equal(1, e.Pick);
        Assert.Equal(0, e.Amount);
        Assert.Equal(g.State.Tick + 20, g.Me.ResearchGrantLockedUntil);
        Assert.Contains("delivered", EventText.Describe(g.C, g.State, e));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Papers_that_do_not_come_cost_half_the_award_and_reputation()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 2));
        Papers(g, 2);
        g.Weeks(6);
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Me.Reputation = 600;
        Assert.Equal(60000, ResearchGrants.Clawback(g.C, g.Me));
        g.Weeks(29);
        Assert.Equal(ResearchGrants.Owed, g.Me.ResearchGrantStage);
        g.Weeks(1);
        Assert.Equal(ResearchGrants.None, g.Me.ResearchGrantStage);
        Assert.Equal(fines - 60000, g.Me.Ledger.Lifetime.Fines);
        GameEvent e = Assert.Single(Of(g, EventKind.GrantSettled));
        Assert.Equal(0, e.Pick);
        Assert.Equal(60000, e.Amount);
        Assert.True(g.Me.Reputation <= 600 - 30 + 5);
        Assert.Contains("paid back $60000", EventText.Describe(g.C, g.State, e));
        Assert.True(g.Me.ResearchGrantLockedUntil > g.State.Tick);
    }

    [Fact]
    public void A_grant_whose_papers_come_late_by_one_tier_is_a_net_gain_only_when_delivered()
    {
        Game kept = Lab();
        kept.Do(Command.ApplyForGrant(kept.Id, 1));
        Papers(kept, 1);
        kept.Weeks(6);
        Papers(kept, 1);
        Game broken = Lab();
        broken.Do(Command.ApplyForGrant(broken.Id, 1));
        Papers(broken, 1);
        broken.Weeks(6);
        broken.Weeks(30);
        Assert.True(kept.Me.Cash > broken.Me.Cash);
        Assert.Equal(1, Of(kept, EventKind.GrantSettled)[0].Pick);
        Assert.Equal(0, Of(broken, EventKind.GrantSettled)[0].Pick);
    }

    [Fact]
    public void A_clawback_never_takes_more_than_the_award_and_a_grant_always_beats_its_fee()
    {
        GameConfig c = TestData.Config();
        for (int tier = 1; tier <= c.ResearchGrants.MaxTier; tier++)
        {
            long award = tier * c.ResearchGrants.AwardPerTier;
            Assert.True(award >= tier * c.ResearchGrants.ApplyCostPerTier);
            Assert.True(MathI.MulPermille(award, c.ResearchGrants.ClawbackPermille) <= award);
        }
    }

    [Fact]
    public void Reputation_never_goes_below_zero_when_the_papers_do_not_come()
    {
        Game g = Lab(c => c.ResearchGrants.FailedReputation = 200);
        g.Do(Command.ApplyForGrant(g.Id, 1));
        Papers(g, 1);
        g.Weeks(6);
        g.Weeks(29);
        g.Me.Reputation = 10;
        g.Weeks(1);
        Assert.Equal(ResearchGrants.None, g.Me.ResearchGrantStage);
        Assert.InRange(g.Me.Reputation, 0, 5);
    }

    [Fact]
    public void After_the_wait_the_lab_can_apply_again()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 3));
        g.Weeks(6);
        g.Weeks(18);
        g.Refused(Command.ApplyForGrant(g.Id, 1), CommandError.GrantLocked);
        g.Weeks(1);
        g.Do(Command.ApplyForGrant(g.Id, 1));
        Assert.Equal(2, Of(g, EventKind.GrantApplied).Count);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_grant_in_each_stage_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 1));
        Papers(g, 1);
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(ResearchGrants.InReview, back.Player.ResearchGrantStage);
        g.Weeks(10);
        for (int i = 0; i < 10; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(ResearchGrants.Owed, back.Player.ResearchGrantStage);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2, c => { c.Prestige.MinPoints = 0; c.Prestige.PaperResearchWeeks = 0; }).WithStarterLab(researchers: 3, engineers: 2, gpus: 6);
        g.Do(Command.PublishPaper(g.Id));
        g.Do(Command.PublishPaper(g.Id));
        g.Do(Command.ApplyForGrant(g.Id, 1));
        g.Weeks(8);
        g.Do(Command.PublishPaper(g.Id));
        g.Weeks(30);
        g.Do(Command.ApplyForGrant(g.Id, 2));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_grant()
    {
        Game g = Lab();
        g.Do(Command.ApplyForGrant(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 59, g.Sim.Save());
        Assert.DoesNotContain("researchGrantStage", old);
        Assert.DoesNotContain("researchGrantLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab =>
        {
            Assert.Equal(0, lab.ResearchGrantStage);
            Assert.Equal(0, lab.ResearchGrantTier);
            Assert.Equal(0, lab.ResearchGrantTick);
            Assert.Equal(0, lab.ResearchGrantOwed);
            Assert.Equal(0, lab.ResearchGrantLockedUntil);
        });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_grant_that_does_not_fit_is_reported()
    {
        Game g = Lab();
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantStage = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantStage = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantTier = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantTier = 2;
        g.Me.ResearchGrantOwed = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantStage = 2;
        g.Me.ResearchGrantOwed = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantOwed = 2;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantStage = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
        g.Me.ResearchGrantTier = 0;
        g.Me.ResearchGrantOwed = 0;
        g.Me.ResearchGrantLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research grant"));
    }

    // ---- a long game

    [Fact]
    public void A_long_game_with_papers_and_grants_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11).WithStarterLab(researchers: 3, engineers: 4, safety: 2, gpus: 8);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int applied = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 25 == 5) g.Try(Command.PublishPaper(g.Id));
            if (i % 25 == 12 && g.Try(Command.ApplyForGrant(g.Id, 1 + i / 25 % 3)).Ok) applied++;
        }
        Assert.True(applied > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
