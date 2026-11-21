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

/// <summary>The public roadmap: a promised model by a date, hype now, a reward for delivering and a larger loss for missing.</summary>
public class RoadmapTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool withModel = true)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 8);
        if (withModel)
        {
            ModelState m = g.Train("mini", 100);
            g.UntilTrained(m);
        }
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static void Deliver(Game g)
    {
        if (!LabQueries.LineUnlocked(g.C, g.Me, "mid"))
        {
            g.GrantResearch(5000);
            g.Research("rules-engines");
            g.Research("expert-shells");
        }
        ModelState m = g.Train("mid", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
    }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Roadmap.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Roadmap.MaxLevel = 6;
        c.Roadmap.MinWeeks = 0;
        c.Roadmap.MaxWeeks = 157;
        c.Roadmap.StretchPerLevel = 0;
        c.Roadmap.HypePerLevel = 101;
        c.Roadmap.DeliverPerLevel = 201;
        c.Roadmap.MissPerLevel = 100;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "minWeeks", "stretchPerLevel", "hypePerLevel", "deliverPerLevel", "missPerLevel" })
            Assert.True(problems.Any(p => p.Contains("roadmap." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_that_announces_nothing_is_exactly_as_it_was()
    {
        Game g = Lab();
        g.Weeks(60);
        Assert.Equal(0, g.Me.RoadmapLevel);
        Assert.Empty(Of(g, EventKind.RoadmapAnnounced));
        Assert.Empty(Of(g, EventKind.RoadmapSettled));
    }

    [Fact]
    public void Announcing_asks_for_more_than_the_best_model_and_earns_hype_at_once()
    {
        Game g = Lab();
        int best = Roadmap.Best(g.Me);
        Assert.True(best >= 90, "best " + best);
        for (int level = 1; level <= 3; level++)
            Assert.Equal(best + (int)((best * (long)g.C.Roadmap.StretchPerLevel * level + 999) / 1000), Roadmap.Goal(g.C, g.Me, level));
        int rep = g.Me.Reputation;
        int tick = g.State.Tick;
        g.Do(Command.AnnounceRoadmap(g.Id, 2, 26));
        Assert.Equal(2, g.Me.RoadmapLevel);
        Assert.Equal(tick, g.Me.RoadmapStart);
        Assert.Equal(tick + 26, g.Me.RoadmapDue);
        Assert.Equal(Roadmap.Goal(g.C, g.Me, 2), g.Me.RoadmapGoal);
        Assert.Equal(rep + 16, g.Me.Reputation);
        GameEvent e = Of(g, EventKind.RoadmapAnnounced).Single();
        Assert.Equal(26, e.Pick);
        Assert.Contains("level 2 roadmap", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_model_that_clears_the_bar_in_time_settles_it_with_the_reward()
    {
        Game g = Lab(c => c.Roadmap.MaxWeeks = 156);
        g.GrantResearch(5000);
        g.Research("rules-engines");
        g.Research("expert-shells");
        ModelState m = g.Train("mid", 100);
        g.Do(Command.AnnounceRoadmap(g.Id, 3, 120));
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        Assert.True(m.Capability >= g.Me.RoadmapGoal, "capability " + m.Capability + " goal " + g.Me.RoadmapGoal);
        int rep = g.Me.Reputation;
        int due = g.Me.RoadmapDue;
        g.Weeks(1);
        Assert.Equal(0, g.Me.RoadmapLevel);
        GameEvent e = Of(g, EventKind.RoadmapSettled).Single();
        Assert.True(e.Pick == 1, "released at " + m.ReleasedTick + " due " + due + " now " + g.State.Tick);
        Assert.Contains("delivered its roadmap", EventText.Describe(g.C, g.State, e));
        Assert.True(g.Me.Reputation >= rep + 20 - 5, "reputation " + rep + " -> " + g.Me.Reputation);
    }

    [Fact]
    public void A_deadline_that_passes_first_costs_more_than_the_hype_earned()
    {
        Game promised = Lab();
        Game quiet = Lab();
        promised.Do(Command.AnnounceRoadmap(promised.Id, 2, 13));
        promised.Weeks(14);
        quiet.Weeks(14);
        GameEvent e = Of(promised, EventKind.RoadmapSettled).Single();
        Assert.Equal(0, e.Pick);
        Assert.Contains("missed its roadmap", EventText.Describe(promised.C, promised.State, e));
        Assert.Equal(0, promised.Me.RoadmapLevel);
        Assert.True(promised.Me.Reputation < quiet.Me.Reputation, "reputation " + promised.Me.Reputation + " against " + quiet.Me.Reputation);
        Assert.True(promised.Me.Reputation <= quiet.Me.Reputation + 16 - 90 + 15, "reputation " + promised.Me.Reputation + " against " + quiet.Me.Reputation);
    }

    [Fact]
    public void A_model_released_after_the_deadline_gets_nothing()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 13));
        g.Weeks(14);
        Assert.Equal(0, Of(g, EventKind.RoadmapSettled).Single().Pick);
        Deliver(g);
        g.Weeks(3);
        Assert.Single(Of(g, EventKind.RoadmapSettled));
    }

    [Fact]
    public void A_model_below_the_bar_does_not_deliver()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 3, 20));
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Weeks(20);
        Assert.Equal(0, Of(g, EventKind.RoadmapSettled).Single().Pick);
    }

    [Fact]
    public void Refused_announcements_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.AnnounceRoadmap(g.Id, 0, 26), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceRoadmap(g.Id, 4, 26), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceRoadmap(g.Id, 1, 12), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceRoadmap(g.Id, 1, 53), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 26));
        d = g.Sim.Digest();
        g.Refused(Command.AnnounceRoadmap(g.Id, 2, 26), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());

        Game empty = Lab(null, false);
        d = empty.Sim.Digest();
        empty.Refused(Command.AnnounceRoadmap(empty.Id, 1, 26), CommandError.InvalidArgument);
        Assert.Equal(d, empty.Sim.Digest());
        Game off = Lab(c => c.Roadmap.MaxLevel = 0);
        off.Refused(Command.AnnounceRoadmap(off.Id, 1, 26), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.AnnounceRoadmap(rival.Id, 1, 26)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_promise_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 2, 30));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.RoadmapLevel);
        Assert.Equal(g.Me.RoadmapDue, lab.RoadmapDue);
        Assert.Equal(g.Me.RoadmapGoal, lab.RoadmapGoal);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(30);
        for (int i = 0; i < 30; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Single(back.State.Events, e => e.Kind == EventKind.RoadmapSettled);
    }

    [Fact]
    public void An_older_save_loads_with_no_roadmap()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 2, 30));
        string old = SaveSchemaPretend.At(g.C, 68, g.Sim.Save());
        foreach (string key in new[] { "roadmapLevel", "roadmapStart", "roadmapDue", "roadmapGoal" }) Assert.DoesNotContain("\"" + key + "\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.RoadmapLevel + lab.RoadmapStart + lab.RoadmapDue + lab.RoadmapGoal));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_roadmap()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 26));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("roadmap"));
        g.Me.RoadmapGoal = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("roadmap"));
        g.Me.RoadmapGoal = 200;
        g.Me.RoadmapDue = g.Me.RoadmapStart;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("roadmap"));
        g.Me.RoadmapDue = g.Me.RoadmapStart + 5;
        g.Me.RoadmapLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("roadmap"));
        g.Me.RoadmapLevel = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("roadmap"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.RoadmapLevel = 1;
        rival.RoadmapStart = 1;
        rival.RoadmapDue = 9;
        rival.RoadmapGoal = 150;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("roadmap"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 40));
        g.Weeks(5);
        ModelState next = g.Train("mini", 100);
        g.UntilTrained(next);
        g.Do(Command.ReleaseModel(g.Id, next.Id, "game-ai"));
        g.Weeks(10);
        g.Do(Command.AnnounceRoadmap(g.Id, 3, 13));
        g.Weeks(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_roadmaps_stays_sound_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            if (i % 30 == 0) g.Try(Command.AnnounceRoadmap(g.Id, 1 + (i / 30) % 3, 13 + i % 20));
        }
        Assert.NotEmpty(Of(g, EventKind.RoadmapSettled));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
