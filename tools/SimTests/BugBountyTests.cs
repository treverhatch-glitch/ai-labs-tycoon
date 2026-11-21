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

/// <summary>The bug bounty: a standing programme with a fee, a weekly bill per model on sale and a share off the incident chance.</summary>
public class BugBountyTests
{
    static Game Shipped(Action<GameConfig> tweak = null, int seed = 6)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 8);
        ModelState m = g.Train("mini", 20);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        return g;
    }

    static ModelState Model(Game g) { return g.Me.Models.First(m => m.Status == ModelStatus.Released); }

    static int Chance(Game g) { return SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, Model(g)); }

    static List<GameEvent> Set(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.BountySet).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.BugBounty.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.BugBounty.MaxLevel = 6;
        c.BugBounty.SetupPerLevel = -1;
        c.BugBounty.WeeklyPerLevel = 0;
        c.BugBounty.CutPerLevel = 400;
        c.BugBounty.CooldownWeeks = 105;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "setupPerLevel", "weeklyPerLevel", "cutPerLevel", "cooldownWeeks" })
            Assert.Contains(problems, p => p.Contains("bugBounty." + key));
    }

    [Fact]
    public void A_lab_with_no_bounty_pays_nothing_and_gains_nothing()
    {
        Game g = Shipped();
        g.Weeks(20);
        Assert.Equal(0, g.Me.BountyLevel);
        Assert.Equal(0, BugBounty.WeeklyCost(g.C, g.Me));
        Assert.Equal(0, BugBounty.IncidentChange(g.C, g.Me, 500));
        Assert.Equal(0, DisplayQueries.IncidentParts(g.C, g.State, g.Me, Model(g)).FromBounty);
        Assert.Empty(Set(g));
    }

    [Fact]
    public void Raising_it_costs_a_fee_for_each_level_and_locks_the_level_for_the_cooldown()
    {
        Game g = Shipped();
        long cash = g.Me.Cash;
        int tick = g.State.Tick;
        g.Do(Command.SetBugBounty(g.Id, 2));
        Assert.Equal(2, g.Me.BountyLevel);
        Assert.Equal(cash - 80000, g.Me.Cash);
        Assert.Equal(tick + 13, g.Me.BountyLockedUntil);
        GameEvent e = Set(g).Single();
        Assert.Equal(2, e.SubjectId);
        Assert.Equal(80000, e.Amount);
        Assert.Equal(1, e.Pick);
        Assert.Contains("raised its bug bounty to level 2", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_level_holds_until_the_cooldown_has_passed_then_lowering_is_free()
    {
        Game g = Shipped();
        g.Do(Command.SetBugBounty(g.Id, 3));
        g.Weeks(5);
        string d = g.Sim.Digest();
        g.Refused(Command.SetBugBounty(g.Id, 1), CommandError.BountyLocked);
        g.Refused(Command.SetBugBounty(g.Id, 0), CommandError.BountyLocked);
        Assert.Equal(d, g.Sim.Digest());
        g.Weeks(g.Me.BountyLockedUntil - g.State.Tick);
        long cash = g.Me.Cash;
        g.Do(Command.SetBugBounty(g.Id, 1));
        Assert.Equal(1, g.Me.BountyLevel);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(0, Set(g).Last().Pick);
        Assert.Contains("lowered its bug bounty to level 1", EventText.Describe(g.C, g.State, Set(g).Last()));
        g.Weeks(g.C.BugBounty.CooldownWeeks);
        g.Do(Command.SetBugBounty(g.Id, 0));
        Assert.Equal(0, BugBounty.WeeklyCost(g.C, g.Me));
        Assert.Contains("closed its bug bounty", EventText.Describe(g.C, g.State, Set(g).Last()));
    }

    [Fact]
    public void Each_level_takes_its_share_off_the_weekly_chance_and_the_breakdown_shows_it()
    {
        Game g = Shipped(c => { c.IncidentFloor.ChanceFloorPermille = 0; });
        int none = Chance(g);
        Assert.True(none >= 10, "chance " + none);
        for (int level = 1; level <= 3; level++)
        {
            g.Do(Command.SetBugBounty(g.Id, level));
            g.Weeks(g.C.BugBounty.CooldownWeeks);
            DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.State, g.Me, Model(g));
            int uncut = parts.Total - parts.FromBounty - parts.FromChief;
            Assert.Equal(-(int)MathI.MulPermille(uncut, 100 * level), parts.FromBounty);
            Assert.True(parts.FromBounty < 0);
            Assert.Equal(parts.Total, Chance(g));
        }
        Assert.True(Chance(g) < none);
    }

    [Fact]
    public void The_weekly_bill_is_the_level_times_the_rate_for_each_model_on_sale()
    {
        Game g = Shipped();
        g.Do(Command.SetBugBounty(g.Id, 2));
        Assert.Equal(2 * 2000 * 1, BugBounty.WeeklyCost(g.C, g.Me));
        ModelState second = g.Train("mini", 20);
        g.UntilTrained(second);
        g.Do(Command.ReleaseModel(g.Id, second.Id, "game-bots"));
        Assert.Equal(2, BugBounty.ReleasedModels(g.Me));
        Assert.Equal(2 * 2000 * 2, BugBounty.WeeklyCost(g.C, g.Me));
    }

    [Fact]
    public void The_bill_comes_out_of_cash_every_week_and_nothing_else_changes_for_a_twin_without_it()
    {
        Action<GameConfig> quiet = c => c.Safety.IncidentChancePermille = 0;
        Game g = Shipped(quiet);
        Game twin = Shipped(quiet);
        g.Do(Command.SetBugBounty(g.Id, 1));
        g.Weeks(10);
        twin.Weeks(10);
        Assert.Equal(twin.Me.Cash - 40000 - 10 * 2000, g.Me.Cash);
    }

    [Fact]
    public void Refused_changes_leave_the_game_exactly_as_it_was()
    {
        Game g = Shipped();
        string d = g.Sim.Digest();
        g.Refused(Command.SetBugBounty(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetBugBounty(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetBugBounty(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Me.Cash = 1000;
        d = g.Sim.Digest();
        g.Refused(Command.SetBugBounty(g.Id, 1), CommandError.NotEnoughCash);
        Assert.Equal(d, g.Sim.Digest());
        Assert.Equal(0, g.Me.BountyLevel);
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.SetBugBounty(g.Id, 1));
        g.Refused(Command.SetBugBounty(g.Id, 1), CommandError.InvalidArgument);

        Game off = Shipped(c => c.BugBounty.MaxLevel = 0);
        off.Refused(Command.SetBugBounty(off.Id, 1), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.SetBugBounty(rival.Id, 1)).Ok);
    }

    [Fact]
    public void A_lab_that_runs_one_has_fewer_incidents_over_a_long_game_than_one_that_does_not()
    {
        Game with = Shipped(null, 9);
        Game without = Shipped(null, 9);
        with.Do(Command.SetBugBounty(with.Id, 3));
        with.Weeks(400);
        without.Weeks(400);
        int a = Model(with).Incidents;
        int b = Model(without).Incidents;
        Assert.True(b > 0, "twin incidents " + b);
        Assert.True(a <= b);
    }

    [Fact]
    public void Save_and_load_keep_the_level_and_the_lock_and_play_on_the_same()
    {
        Game g = Shipped();
        g.Do(Command.SetBugBounty(g.Id, 2));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.BountyLevel);
        Assert.Equal(g.Me.BountyLockedUntil, lab.BountyLockedUntil);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_bounty()
    {
        Game g = Shipped();
        g.Do(Command.SetBugBounty(g.Id, 2));
        string old = SaveSchemaPretend.At(g.C, 65, g.Sim.Save());
        Assert.DoesNotContain("\"bountyLevel\"", old);
        Assert.DoesNotContain("\"bountyLockedUntil\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.BountyLevel); Assert.Equal(0, lab.BountyLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_bounty()
    {
        Game g = Shipped();
        g.Me.BountyLevel = 2;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("bug bounty"));
        g.Me.BountyLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("bug bounty"));
        g.Me.BountyLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("bug bounty"));
        g.Me.BountyLevel = 0;
        g.Me.BountyLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("bug bounty"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).BountyLevel = 1;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("bug bounty"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Shipped();
        g.Do(Command.SetBugBounty(g.Id, 1));
        g.Weeks(20);
        g.Do(Command.SetBugBounty(g.Id, 3));
        g.Weeks(20);
        g.Do(Command.SetBugBounty(g.Id, 0));
        g.Weeks(15);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_a_bounty_stays_sound_and_the_books_balance()
    {
        Game g = Shipped();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            if (i % 40 == 0) g.Try(Command.SetBugBounty(g.Id, 1 + (i / 40) % 3));
        }
        Assert.NotEmpty(Set(g));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
