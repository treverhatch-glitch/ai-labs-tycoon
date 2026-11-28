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

public class TechBetsWaitlistBrandFreeTierLaunchWatchTogetherTests
{
    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.New(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Shocks.Enabled = 0;
            c.PlayerStart.GrantPerWeek = 400_000;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        });
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 500, Status = ModelStatus.Released, Alignment = 500, Name = "Together" });
        g.Me.Reputation = 500;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Rest(Game g)
    {
        g.State.Tick++;
        Brand.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Keeping_a_tech_promise_or_shipping_a_waitlist_raises_the_employer_score_and_missing_them_drops_it()
    {
        Game g = Lab();
        g.Do(Command.OpenCareersPage(g.Id));
        Events.Add(g.State, EventKind.TechBetEnded, g.Id, 0, 0, "x").Pick = 1;
        Events.Add(g.State, EventKind.WaitlistEnded, g.Id, 0, 0, "").Pick = 1;
        Rest(g);
        Rest(g);
        Assert.Equal(g.C.Brand.Start + 2 * g.C.Brand.KeptPoints, g.Me.BrandScore);
        int score = g.Me.BrandScore;
        Events.Add(g.State, EventKind.TechBetEnded, g.Id, 0, 0, "x").Pick = 0;
        Events.Add(g.State, EventKind.WaitlistEnded, g.Id, 0, 0, "").Pick = 0;
        Events.Add(g.State, EventKind.WaitlistEnded, g.Id, 0, 0, "").Pick = 2;
        Rest(g);
        Rest(g);
        Assert.Equal(score - 2 * g.C.Brand.BrokenPoints, g.Me.BrandScore);
    }

    [Fact]
    public void A_real_kept_bet_reaches_the_employer_score_through_the_events()
    {
        Game g = Lab();
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        g.Research("minimax");
        g.Weeks(3);
        Assert.Equal(1, g.Me.TbWins);
        Assert.True(g.Me.BrandScore > g.C.Brand.Start);
    }

    [Fact]
    public void A_big_free_tier_takes_up_part_of_a_rival_launch()
    {
        Game plain = Lab();
        plain.Me.LwRival = 2;
        plain.Me.LwLead = 600;
        int loss = LaunchWatch.Loss(plain.C, plain.Me);
        Game tier = Lab();
        tier.Do(Command.OpenFreeTier(tier.Id, 3));
        tier.Weeks(6);
        tier.Me.LwRival = 2;
        tier.Me.LwLead = 600;
        Assert.True(tier.Me.FtReach > 0);
        Assert.True(LaunchWatch.Loss(tier.C, tier.Me) < loss);
        tier.Me.FtReach = 1000;
        Assert.Equal(MathI.MulPermille(MathI.MulPermille(tier.C.LaunchWatch.LossReputation, 1100), 750), LaunchWatch.Loss(tier.C, tier.Me));
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Lab();
        g.Autopilot(60);
        Assert.Equal(0, g.Me.TbRef + g.Me.WlModel + g.Me.BrandOn + g.Me.FtLevel + g.Me.LwOn);
        foreach (EventKind k in new[] { EventKind.TechBetMade, EventKind.WaitlistOpened, EventKind.CareersOpened, EventKind.FreeTierOpened, EventKind.WatchDeskOpened, EventKind.RivalLaunchSeen })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab(c => c.PlayerStart.GrantPerWeek = 400_000);
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 900, Status = ModelStatus.Trained, Alignment = 500, Name = "Next" });
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        g.Do(Command.OpenWaitlist(g.Id, g.Me.Models.Last().Id, 30));
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.OpenFreeTier(g.Id, 2));
        g.Do(Command.OpenWatchDesk(g.Id));
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
        Game g = Game.New(seed, c =>
        {
            c.PlayerStart.Cash = 10_000_000_000_000L;
            c.PlayerStart.GrantPerWeek = 50_000_000L;
            c.Shocks.Enabled = 0;
        });
        for (int warm = 0; warm < 200 && Count(g, EventKind.TechBetMade) == 0; warm++)
        {
            foreach (TechConfig tech in g.C.Research.Techs)
            {
                if (LabQueries.HasTech(g.Me, tech.Id)) continue;
                if (!EraSystem.TechOpen(g.C, tech, g.State.Tick)) continue;
                if (tech.Cost < g.C.TechBets.MinCost) continue;
                if (g.Try(Command.BetOnTech(g.Id, tech.Id)).Ok) break;
            }
            if (Count(g, EventKind.TechBetMade) > 0) break;
            g.Autopilot(1);
        }
        bool sawTechBet = Count(g, EventKind.TechBetMade) > 0;
        Assert.True(sawTechBet, "no tech open for a public bet in the warm-up");
        g.Autopilot(60);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        string[] techs = { "minimax", "regression-tests", "state-machines", "rules-engines", "decision-trees", "beowulf-clusters" };
        g.Do(Command.OpenWatchDesk(g.Id));
        for (int week = 0; week < 400 && !g.Me.Bankrupt; week++)
        {
            if (week % 40 == 2)
            {
                foreach (string t in techs)
                {
                    if (LabQueries.HasTech(g.Me, t)) continue;
                    if (g.Try(Command.BetOnTech(g.Id, t)).Ok) break;
                }
            }
            if (week % 40 == 30) g.Try(Command.WithdrawTechBet(g.Id));
            if (week % 60 == 5)
            {
                ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Training || x.Status == ModelStatus.Trained);
                if (m != null) g.Try(Command.OpenWaitlist(g.Id, m.Id, 6 + week / 60 % 20));
            }
            if (week % 60 == 50) g.Try(Command.CloseWaitlist(g.Id));
            if (week == 3) g.Try(Command.OpenCareersPage(g.Id));
            if (week % 20 == 11) g.Try(Command.AnswerReview(g.Id, 1 + week / 20 % 3));
            if (week % 70 == 8) g.Try(Command.OpenFreeTier(g.Id, 1 + week / 70 % 3));
            if (week % 70 == 60) g.Try(Command.CloseFreeTier(g.Id));
            if (week % 6 == 4) g.Try(Command.AnswerLaunch(g.Id, 1 + week / 6 % 3));
            if (week == 250) g.Try(Command.CloseWatchDesk(g.Id));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(sawTechBet, "no tech bet was ever made");
        Assert.Equal(1, g.Me.BrandOn);
        Assert.True(g.Me.FtSpent > 0, "no free tier ever ran");
        Assert.True(g.Me.LwAnswers > 0, "no rival launch was ever answered");
    }
}
