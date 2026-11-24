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

/// <summary>Board targets: the player promises a quarter's revenue, and is judged on what the lab actually makes.</summary>
public class RevenueTargetsTests
{
    static Game Selling(int seed = 3, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 3, safety: 2, sales: 3, gpus: 10);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai", "game-bots"));
        g.Weeks(6);
        Assert.True(g.Me.LastWeek.Revenue >= 1000, "the lab should be selling: " + g.Me.LastWeek.Revenue);
        return g;
    }

    static List<GameEvent> Settled(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.TargetSettled).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.RevenueTargets.MaxLevel);
        Assert.Equal(13, c.RevenueTargets.Weeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.RevenueTargets.MaxLevel = 6;
        c.RevenueTargets.Weeks = 3;
        c.RevenueTargets.StretchPerLevel = 1001;
        c.RevenueTargets.MinWeeklyRevenue = 0;
        c.RevenueTargets.RewardPerLevel = 101;
        c.RevenueTargets.PenaltyPerLevel = 5;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "weeks", "stretchPerLevel", "minWeeklyRevenue", "rewardPerLevel", "penaltyPerLevel" })
            Assert.Contains(problems, p => p.Contains("revenueTargets." + key));
    }

    [Fact]
    public void No_target_is_ever_set_by_itself()
    {
        Game g = Selling();
        g.Weeks(30);
        Assert.False(RevenueTargets.Active(g.Me));
        Assert.Empty(Settled(g));
    }

    [Fact]
    public void Setting_a_target_fixes_the_goal_from_last_weeks_pace_and_the_level()
    {
        Game g = Selling();
        long pace = g.Me.LastWeek.Revenue;
        int tick = g.State.Tick;
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        Assert.Equal(2, g.Me.RevenueTargetLevel);
        Assert.Equal(tick + 13, g.Me.RevenueTargetEnd);
        Assert.Equal(MathI.MulPermille(pace * 13, 1300), g.Me.RevenueTargetGoal);
        Assert.Equal(0, g.Me.RevenueTargetSoFar);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.TargetSet);
        Assert.Equal(2, e.SubjectId);
        Assert.Equal(g.Me.RevenueTargetGoal, e.Amount);
        Assert.Contains("level 2", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_harder_level_asks_for_more()
    {
        Game g = Selling();
        Assert.True(RevenueTargets.Goal(g.C, g.Me, 1) < RevenueTargets.Goal(g.C, g.Me, 2));
        Assert.True(RevenueTargets.Goal(g.C, g.Me, 2) < RevenueTargets.Goal(g.C, g.Me, 3));
    }

    [Fact]
    public void The_revenue_so_far_adds_up_each_week()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 1));
        long sum = 0;
        for (int i = 0; i < 5; i++)
        {
            g.Weeks(1);
            sum += g.Me.LastWeek.Revenue;
        }
        Assert.Equal(sum, g.Me.RevenueTargetSoFar);
    }

    [Fact]
    public void A_met_target_pays_reputation_by_level_and_clears_itself()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 3));
        g.Me.RevenueTargetGoal = 1;
        g.Me.Reputation = 500;
        Game twin = Selling();
        twin.Me.Reputation = 500;
        twin.Weeks(13);
        g.Weeks(13);
        Assert.Equal(twin.Me.Reputation + 30, g.Me.Reputation);
        Assert.False(RevenueTargets.Active(g.Me));
        Assert.Equal(0, g.Me.RevenueTargetGoal);
        GameEvent e = Assert.Single(Settled(g));
        Assert.Equal(1, e.Pick);
        Assert.Equal(3, e.SubjectId);
        Assert.Contains("hit its level 3", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_missed_target_costs_more_than_a_met_one_pays()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        g.Me.RevenueTargetGoal = long.MaxValue / 4;
        g.Me.Reputation = 500;
        Game twin = Selling();
        twin.Me.Reputation = 500;
        twin.Weeks(13);
        g.Weeks(13);
        Assert.Equal(twin.Me.Reputation - 30, g.Me.Reputation);
        Assert.Equal(0, Assert.Single(Settled(g)).Pick);
        Assert.Contains("missed", EventText.Describe(g.C, g.State, Settled(g)[0]));
        Assert.True(g.C.RevenueTargets.PenaltyPerLevel >= g.C.RevenueTargets.RewardPerLevel);
    }

    [Fact]
    public void The_target_is_judged_on_the_thirteenth_week_and_not_before()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 1));
        g.Weeks(12);
        Assert.True(RevenueTargets.Active(g.Me));
        Assert.Empty(Settled(g));
        g.Weeks(1);
        Assert.False(RevenueTargets.Active(g.Me));
        Assert.Single(Settled(g));
    }

    [Fact]
    public void Reputation_stays_inside_zero_and_a_thousand()
    {
        Game low = Selling();
        low.Do(Command.SetRevenueTarget(low.Id, 3));
        low.Me.RevenueTargetGoal = long.MaxValue / 4;
        low.Me.Reputation = 10;
        low.Weeks(13);
        Assert.Equal(0, low.Me.Reputation);
        Game high = Selling();
        high.Do(Command.SetRevenueTarget(high.Id, 3));
        high.Me.RevenueTargetGoal = 1;
        high.Me.Reputation = 1000;
        Game twin = Selling();
        twin.Me.Reputation = 1000;
        twin.Weeks(13);
        high.Weeks(13);
        Assert.InRange(high.Me.Reputation, twin.Me.Reputation, 1000);
    }

    [Fact]
    public void A_second_target_can_be_set_once_the_first_is_judged()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 1));
        string before = g.Sim.Digest();
        g.Refused(Command.SetRevenueTarget(g.Id, 2), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        g.Weeks(13);
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        Assert.Equal(2, g.Me.RevenueTargetLevel);
    }

    [Fact]
    public void Bad_levels_and_a_lab_with_no_sales_are_refused_and_change_nothing()
    {
        Game g = Selling();
        string before = g.Sim.Digest();
        g.Refused(Command.SetRevenueTarget(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetRevenueTarget(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetRevenueTarget(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        Game poor = Game.Solo(2).WithStarterLab(researchers: 2, gpus: 4);
        poor.Refused(Command.SetRevenueTarget(poor.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_it_switched_off_cannot_set_one()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.LastWeek.Revenue = 1_000_000;
        Assert.False(w.Sim.Apply(Command.SetRevenueTarget(rival.Id, 1)).Ok);
        Game off = Selling(3, c => c.RevenueTargets.MaxLevel = 0);
        off.Refused(Command.SetRevenueTarget(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Save_and_load_keep_the_target_and_play_on_the_same()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.RevenueTargetLevel);
        Assert.Equal(g.Me.RevenueTargetSoFar, lab.RevenueTargetSoFar);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_target()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        string old = SaveSchemaPretend.At(g.C, 63, g.Sim.Save());
        Assert.DoesNotContain("revenueTarget", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab =>
        {
            Assert.Equal(0, lab.RevenueTargetLevel);
            Assert.Equal(0, lab.RevenueTargetGoal);
        });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_impossible_targets()
    {
        Game g = Selling();
        g.Me.RevenueTargetLevel = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue target"));
        g.Me.RevenueTargetLevel = 0;
        g.Me.RevenueTargetGoal = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue target"));
        g.Me.RevenueTargetGoal = 0;
        g.Me.RevenueTargetLevel = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue target"));
        g.Me.RevenueTargetEnd = g.State.Tick + 5;
        g.Me.RevenueTargetGoal = 100;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("revenue target"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Selling();
        g.Do(Command.SetRevenueTarget(g.Id, 2));
        g.Weeks(14);
        g.Do(Command.SetRevenueTarget(g.Id, 1));
        g.Weeks(8);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Single(Settled(g));
    }

    [Fact]
    public void A_long_game_with_targets_set_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Selling(11);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int set = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 20 == 0 && g.Try(Command.SetRevenueTarget(g.Id, 1 + i / 20 % 3)).Ok) set++;
        }
        Assert.True(set > 0);
        Assert.True(Settled(g).Count > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
