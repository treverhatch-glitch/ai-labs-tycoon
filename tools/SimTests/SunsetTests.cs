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

/// <summary>Model end-of-life notice: a dated retirement whose customers move to the successor the old model helped design.</summary>
public class SunsetTests
{
    const string A = "game-ai";
    const string B = "game-bots";

    static Game Lab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        return g;
    }

    /// <summary>A released helper in two sectors and a successor it helped design, released in one of them (so the helper still sells in the other).</summary>
    static (Game g, ModelState old, ModelState next) Fleet(Action<GameConfig> tweak = null, bool assisted = true)
    {
        Game g = Lab(tweak);
        ModelState old = g.Train("mini", 300);
        g.UntilTrained(old);
        g.Release(old, A, B);
        ModelState next = g.Train("mini", 300);
        if (assisted) g.Do(Command.DesignWithModel(g.Id, next.Id, old.Id));
        g.UntilTrained(next);
        g.Release(next, A);
        g.Weeks(2);
        return (g, old, next);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Sunset.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Sunset.Enabled = 2;
        c.Sunset.MinWeeks = 0;
        c.Sunset.MaxWeeks = 0;
        c.Sunset.SupportPermille = 1001;
        c.Sunset.TrustDropLong = 200;
        c.Sunset.TrustDropShort = 100;
        c.Sunset.AssistedCutPermille = 1001;
        c.Sunset.DroppedPermille = 999;
        c.Sunset.CancelReputation = 201;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minWeeks", "supportPermille", "trustDropShort", "assistedCutPermille", "droppedPermille", "cancelReputation" })
            Assert.True(problems.Any(p => p.Contains("sunset." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_that_gives_no_notice_is_exactly_as_it_was()
    {
        var (g, old, next) = Fleet();
        g.Weeks(60);
        Assert.Equal(0, old.SunsetTick);
        Assert.Equal(ModelStatus.Released, old.Status);
        Assert.Empty(Of(g, EventKind.SunsetAnnounced));
        Assert.Empty(Of(g, EventKind.SunsetDone));
    }

    [Fact]
    public void The_successor_is_the_model_the_old_one_helped_design()
    {
        var (g, old, next) = Fleet();
        Assert.Same(next, Sunset.SuccessorOf(g.Me, old));
        Assert.True(SuccessorDesign.IsAssisted(next));
        Assert.Null(Sunset.SuccessorOf(g.Me, next));
        var (plain, p1, p2) = Fleet(null, false);
        Assert.Null(Sunset.SuccessorOf(plain.Me, p1));
        plain.Refused(Command.AnnounceSunset(plain.Id, p1.Id, 13), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_notice_runs_for_its_weeks_costs_support_and_then_the_customers_move()
    {
        var (g, old, next) = Fleet();
        var (twin, tOld, tNext) = Fleet();
        int start = g.State.Tick;
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 13));
        Assert.Equal(start, old.SunsetStart);
        Assert.Equal(start + 13, old.SunsetTick);
        GameEvent a = Of(g, EventKind.SunsetAnnounced).Single();
        Assert.Equal(13, a.Amount);
        Assert.Equal(1, a.Pick);
        Assert.Contains("retired in 13 weeks", EventText.Describe(g.C, g.State, a));
        Assert.True(Sunset.WeeklyRevenue(g.State, g.Me, old) > 0);
        g.Weeks(6);
        twin.Weeks(6);
        Assert.True(g.Me.Cash < twin.Me.Cash);
        Assert.Equal(ModelStatus.Released, old.Status);
        SectorEntry before = g.State.Sector(B).Entry(g.Id);
        Assert.Equal(old.Id, before.ModelId);
        int share = before.SharePpm;
        int trust = TrustSystem.Of(g.C, g.Me, B);
        g.Weeks(7);
        Assert.Equal(ModelStatus.Retired, old.Status);
        Assert.Empty(old.Sectors);
        Assert.Equal(0, old.SunsetTick);
        SectorEntry after = g.State.Sector(B).Entry(g.Id);
        Assert.Equal(next.Id, after.ModelId);
        Assert.Contains(B, next.Sectors);
        Assert.True(TrustSystem.Of(g.C, g.Me, B) < trust);
        GameEvent done = Of(g, EventKind.SunsetDone).Single();
        Assert.Equal(next.Id, done.SubjectId);
        Assert.Equal(1, done.Amount);
        Assert.Equal(1, done.Pick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ModelRetired && e.SubjectId == old.Id);
        Assert.Contains("retired", EventText.Describe(g.C, g.State, done));
        Assert.True(share > 0);
    }

    [Fact]
    public void A_long_notice_costs_less_trust_than_a_short_one_and_a_designed_successor_less_than_a_stranger()
    {
        GameConfig c = TestData.Config();
        int shortNotice = Sunset.TrustDrop(c, 4, false);
        int longNotice = Sunset.TrustDrop(c, 26, false);
        Assert.Equal(150, shortNotice);
        Assert.Equal(20, longNotice);
        Assert.True(Sunset.TrustDrop(c, 13, false) < shortNotice && Sunset.TrustDrop(c, 13, false) > longNotice);
        Assert.Equal(75, Sunset.TrustDrop(c, 4, true));
        Assert.Equal(shortNotice, Sunset.TrustDrop(c, 1, false));
        Assert.Equal(longNotice, Sunset.TrustDrop(c, 99, false));

        var (quick, qOld, _) = Fleet();
        var (slow, sOld, _) = Fleet();
        int trust = TrustSystem.Of(quick.C, quick.Me, B);
        quick.Do(Command.AnnounceSunset(quick.Id, qOld.Id, 4));
        slow.Do(Command.AnnounceSunset(slow.Id, sOld.Id, 26));
        quick.Weeks(4);
        slow.Weeks(26);
        Assert.Equal(ModelStatus.Retired, qOld.Status);
        Assert.Equal(ModelStatus.Retired, sOld.Status);
        Assert.True(TrustSystem.Of(slow.C, slow.Me, B) > TrustSystem.Of(quick.C, quick.Me, B));
        Assert.True(trust > TrustSystem.Of(quick.C, quick.Me, B));
    }

    [Fact]
    public void A_notice_is_withdrawn_when_the_successor_is_gone_on_the_day()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 8));
        long reputation = g.Me.Reputation;
        g.Do(Command.PullOutOfSector(g.Id, next.Id, A));
        Assert.Equal(ModelStatus.Retired, next.Status);
        g.Weeks(8);
        Assert.Equal(ModelStatus.Released, old.Status);
        Assert.Equal(0, old.SunsetTick);
        Assert.Equal(B, old.Sectors.Single());
        GameEvent e = Of(g, EventKind.SunsetCalledOff).Single();
        Assert.Equal(0, e.Pick);
        Assert.True(e.Amount > 0);
        Assert.Contains("successor was gone", EventText.Describe(g.C, g.State, e));
        Assert.True(g.Me.Reputation < reputation);
        Assert.Empty(Of(g, EventKind.SunsetDone));
    }

    [Fact]
    public void Calling_a_notice_off_costs_reputation_and_clears_it()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 13));
        g.Weeks(3);
        long reputation = g.Me.Reputation;
        g.Do(Command.CancelSunset(g.Id, old.Id));
        Assert.Equal(0, old.SunsetTick);
        Assert.Equal(0, old.SunsetStart);
        Assert.Equal(reputation - PressSystem.Damp(g.C, g.Me, 20), g.Me.Reputation);
        GameEvent e = Of(g, EventKind.SunsetCalledOff).Single();
        Assert.Equal(1, e.Pick);
        g.Weeks(20);
        Assert.Equal(ModelStatus.Released, old.Status);
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 4));
    }

    [Fact]
    public void A_sector_the_successor_cannot_serve_is_dropped_with_a_bigger_trust_loss()
    {
        var (g, old, next) = Fleet();
        var (twin, tOld, tNext) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 8));
        twin.Do(Command.AnnounceSunset(twin.Id, tOld.Id, 8));
        g.C.Market.Sectors.First(s => s.Id == B).MinCapability = next.Capability + 1;
        g.Weeks(8);
        twin.Weeks(8);
        Assert.Equal(ModelStatus.Retired, old.Status);
        Assert.Null(g.State.Sector(B).Entry(g.Id));
        Assert.DoesNotContain(B, next.Sectors);
        GameEvent done = Of(g, EventKind.SunsetDone).Single();
        Assert.Equal(0, done.Amount);
        Assert.Equal(0, done.Pick);
        Assert.Equal(1, Of(twin, EventKind.SunsetDone).Single().Pick);
        Assert.True(TrustSystem.Of(g.C, g.Me, B) < TrustSystem.Of(twin.C, twin.Me, B));
    }

    [Fact]
    public void A_model_replaced_by_a_new_release_loses_its_notice()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 13));
        g.Release(next, B);
        Assert.Equal(ModelStatus.Retired, old.Status);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(1);
        Assert.Equal(0, old.SunsetTick);
        Assert.Empty(Of(g, EventKind.SunsetDone));
        Assert.Empty(Of(g, EventKind.SunsetCalledOff));
    }

    [Fact]
    public void Refused_notices_leave_the_game_exactly_as_it_was()
    {
        var (g, old, next) = Fleet();
        string d = g.Sim.Digest();
        g.Refused(Command.AnnounceSunset(g.Id, old.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceSunset(g.Id, old.Id, 27), CommandError.InvalidArgument);
        g.Refused(Command.AnnounceSunset(g.Id, 9999, 13), CommandError.UnknownId);
        g.Refused(Command.AnnounceSunset(g.Id, next.Id, 13), CommandError.InvalidArgument);
        g.Refused(Command.CancelSunset(g.Id, old.Id), CommandError.InvalidArgument);
        g.Refused(Command.CancelSunset(g.Id, 9999), CommandError.UnknownId);
        Assert.Equal(d, g.Sim.Digest());
        ModelState fresh = g.Train("mini", 300);
        g.Refused(Command.AnnounceSunset(g.Id, fresh.Id, 13), CommandError.WrongModelStatus);
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 13));
        d = g.Sim.Digest();
        g.Refused(Command.AnnounceSunset(g.Id, old.Id, 13), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        var (off, o1, _) = Fleet(c => c.Sunset.Enabled = 0);
        off.Refused(Command.AnnounceSunset(off.Id, o1.Id, 13), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.AnnounceSunset(rival.Id, 1, 13)).Ok);
        Assert.False(w.Sim.Apply(Command.CancelSunset(rival.Id, 1)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_notice_and_play_on_the_same()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 10));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        ModelState again = back.State.Lab(g.Id).Model(old.Id);
        Assert.Equal(old.SunsetStart, again.SunsetStart);
        Assert.Equal(old.SunsetTick, again.SunsetTick);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Single(back.State.Events, e => e.Kind == EventKind.SunsetDone);
    }

    [Fact]
    public void An_older_save_loads_with_no_notice()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 10));
        string older = SaveSchemaPretend.At(g.C, 73, g.Sim.Save());
        foreach (string key in new[] { "sunsetStart", "sunsetTick" }) Assert.DoesNotContain("\"" + key + "\"", older);
        Simulation sim = Simulation.Load(g.C, older);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs.SelectMany(l => l.Models), m => Assert.Equal(0, m.SunsetStart + m.SunsetTick));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_notice()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 10));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("end-of-life"));
        old.SunsetTick = old.SunsetStart;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("end-of-life"));
        old.SunsetTick = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("end-of-life"));
        old.SunsetStart = 0;
        old.SunsetTick = -4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("end-of-life"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        var (g, old, next) = Fleet();
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 8));
        g.Weeks(3);
        g.Do(Command.CancelSunset(g.Id, old.Id));
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 6));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_a_retirement_stays_sound_and_the_books_balance()
    {
        var (g, old, next) = Fleet();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 12));
        g.Weeks(150);
        Assert.Equal(ModelStatus.Retired, old.Status);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
