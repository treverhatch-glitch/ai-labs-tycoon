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

/// <summary>The public safety pledge: a standing promise about launches, a little trust for keeping it and a price for breaking it.</summary>
public class PledgeTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        return Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 8);
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        return m;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Pledge.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Pledge.MaxLevel = 6;
        c.Pledge.BaseFloor = 900;
        c.Pledge.FloorStep = 200;
        c.Pledge.TrustPerLevel = 11;
        c.Pledge.BreachReputation = 501;
        c.Pledge.BreachTrust = 501;
        c.Pledge.WalkBackReputation = 502;
        c.Pledge.LockWeeks = 105;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "baseFloor", "trustPerLevel", "breachReputation", "breachTrust", "walkBackReputation", "lockWeeks" })
            Assert.True(problems.Any(p => p.Contains("pledge." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_with_no_pledge_is_exactly_as_it_was()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Weeks(20);
        Assert.Equal(0, g.Me.PledgeLevel);
        Assert.Equal(0, Pledge.TrustBonus(g.C, g.Me));
        Assert.Empty(Of(g, EventKind.PledgeSet));
        Assert.Empty(Of(g, EventKind.PledgeBroken));
    }

    [Fact]
    public void Each_level_asks_for_more_alignment_than_the_last()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(0, Pledge.Floor(c, 0));
        Assert.Equal(450, Pledge.Floor(c, 1));
        Assert.Equal(600, Pledge.Floor(c, 2));
        Assert.Equal(750, Pledge.Floor(c, 3));
    }

    [Fact]
    public void Making_one_is_free_and_locks_the_level_for_the_lock_weeks()
    {
        Game g = Lab();
        int tick = g.State.Tick;
        int rep = g.Me.Reputation;
        g.Do(Command.SetPledge(g.Id, 2));
        Assert.Equal(2, g.Me.PledgeLevel);
        Assert.Equal(tick + 26, g.Me.PledgeLockedUntil);
        Assert.Equal(rep, g.Me.Reputation);
        GameEvent e = Of(g, EventKind.PledgeSet).Single();
        Assert.Equal(1, e.Pick);
        Assert.Contains("at least 600 alignment (level 2)", EventText.Describe(g.C, g.State, e));
        string d = g.Sim.Digest();
        g.Refused(Command.SetPledge(g.Id, 3), CommandError.PledgeLocked);
        g.Refused(Command.SetPledge(g.Id, 0), CommandError.PledgeLocked);
        Assert.Equal(d, g.Sim.Digest());
    }

    [Fact]
    public void Customers_trust_a_pledging_lab_a_little_more_every_week()
    {
        Game kept = Lab(c => c.Pledge.BaseFloor = 1);
        Game plain = Lab(c => c.Pledge.BaseFloor = 1);
        kept.Do(Command.SetPledge(kept.Id, 1));
        ModelState a = Trained(kept);
        ModelState b = Trained(plain);
        Assert.True(a.Alignment >= 1, "alignment " + a.Alignment);
        kept.Do(Command.ReleaseModel(kept.Id, a.Id, "game-ai"));
        plain.Do(Command.ReleaseModel(plain.Id, b.Id, "game-ai"));
        Assert.Equal(1, kept.Me.PledgeLevel);
        kept.Weeks(30);
        plain.Weeks(30);
        Assert.True(TrustSystem.Of(kept.C, kept.Me, "game-ai") > TrustSystem.Of(plain.C, plain.Me, "game-ai"));
    }

    [Fact]
    public void A_launch_that_shows_less_than_the_pledge_breaks_it()
    {
        Game g = Lab(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        g.Do(Command.SetPledge(g.Id, 1));
        ModelState m = Trained(g);
        int rep = g.Me.Reputation;
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        Assert.Equal(0, g.Me.PledgeLevel);
        Assert.Equal(g.State.Tick + 26, g.Me.PledgeLockedUntil);
        Assert.True(g.Me.Reputation < rep);
        GameEvent e = Of(g, EventKind.PledgeBroken).Single();
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Equal(1000, e.Amount);
        Assert.Contains("broke its public safety pledge", EventText.Describe(g.C, g.State, e));
        g.Refused(Command.SetPledge(g.Id, 1), CommandError.PledgeLocked);
    }

    [Fact]
    public void Breaking_it_costs_trust_in_the_sectors_the_lab_sells_in()
    {
        Game kept = Lab(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        Game none = Lab(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        kept.Do(Command.SetPledge(kept.Id, 1));
        ModelState a = Trained(kept);
        ModelState b = Trained(none);
        kept.Do(Command.ReleaseModel(kept.Id, a.Id, "game-ai"));
        none.Do(Command.ReleaseModel(none.Id, b.Id, "game-ai"));
        Assert.Equal(TrustSystem.Of(none.C, none.Me, "game-ai") - 120, TrustSystem.Of(kept.C, kept.Me, "game-ai"));
    }

    [Fact]
    public void Claiming_enough_keeps_the_pledge_and_the_whistleblower_is_the_check_on_that()
    {
        Game g = Lab(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        g.Do(Command.SetPledge(g.Id, 1));
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 1000, "game-ai"));
        Assert.Equal(1, g.Me.PledgeLevel);
        Assert.Empty(Of(g, EventKind.PledgeBroken));
        Assert.True(SafetySystem.AlignmentGap(g.C, m) > 0);
    }

    [Fact]
    public void Walking_a_pledge_back_costs_reputation_once_the_lock_has_passed()
    {
        Game g = Lab();
        g.Do(Command.SetPledge(g.Id, 3));
        g.Weeks(26);
        int rep = g.Me.Reputation;
        g.Do(Command.SetPledge(g.Id, 1));
        Assert.Equal(rep - 15, g.Me.Reputation);
        GameEvent e = Of(g, EventKind.PledgeSet).Last();
        Assert.Equal(0, e.Pick);
        Assert.Equal(15, e.Amount);
        Assert.Contains("walked its safety pledge back to level 1", EventText.Describe(g.C, g.State, e));
        g.Weeks(26);
        g.Do(Command.SetPledge(g.Id, 0));
        Assert.Contains("dropped its public safety pledge", EventText.Describe(g.C, g.State, Of(g, EventKind.PledgeSet).Last()));
    }

    [Fact]
    public void Raising_a_pledge_later_costs_nothing()
    {
        Game g = Lab();
        g.Do(Command.SetPledge(g.Id, 1));
        g.Weeks(26);
        int rep = g.Me.Reputation;
        g.Do(Command.SetPledge(g.Id, 3));
        Assert.Equal(rep, g.Me.Reputation);
        Assert.Equal(1, Of(g, EventKind.PledgeSet).Last().Pick);
    }

    [Fact]
    public void Refused_changes_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.SetPledge(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetPledge(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetPledge(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        Game off = Lab(c => c.Pledge.MaxLevel = 0);
        off.Refused(Command.SetPledge(off.Id, 1), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.SetPledge(rival.Id, 1)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_pledge_and_the_lock_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetPledge(g.Id, 2));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.PledgeLevel);
        Assert.Equal(g.Me.PledgeLockedUntil, lab.PledgeLockedUntil);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_pledge()
    {
        Game g = Lab();
        g.Do(Command.SetPledge(g.Id, 2));
        string old = SaveSchemaPretend.At(g.C, 67, g.Sim.Save());
        Assert.DoesNotContain("\"pledgeLevel\"", old);
        Assert.DoesNotContain("\"pledgeLockedUntil\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.PledgeLevel); Assert.Equal(0, lab.PledgeLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_pledge()
    {
        Game g = Lab();
        g.Me.PledgeLevel = 2;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("pledge"));
        g.Me.PledgeLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("pledge"));
        g.Me.PledgeLevel = 0;
        g.Me.PledgeLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("pledge"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).PledgeLevel = 1;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("pledge"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        g.Do(Command.SetPledge(g.Id, 1));
        g.Weeks(10);
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Weeks(40);
        g.Do(Command.SetPledge(g.Id, 2));
        g.Weeks(30);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_a_pledge_stays_sound_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            if (i % 30 == 0) g.Try(Command.SetPledge(g.Id, 1 + (i / 30) % 3));
        }
        Assert.NotEmpty(Of(g, EventKind.PledgeSet));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
