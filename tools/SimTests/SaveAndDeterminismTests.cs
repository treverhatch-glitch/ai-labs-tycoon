using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;
using Xunit.Abstractions;

namespace AiLabs.SimTests;

/// <summary>Saves, replays and the promise that the same seed and the same orders always give the same game.</summary>
public class SaveAndDeterminismTests
{
    readonly ITestOutputHelper _out;

    public SaveAndDeterminismTests(ITestOutputHelper output) { _out = output; }

    static Game PlayedFor(int weeks, long seed = 1)
    {
        Game g = Game.New(seed);
        g.Autopilot(weeks);
        return g;
    }

    [Fact]
    public void A_save_loads_back_into_exactly_the_same_game()
    {
        Game g = PlayedFor(3 * 48);
        string save = g.Sim.Save();

        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(save, loaded.Save());
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        Assert.Equal(g.Me.Cash, loaded.Player.Cash);
        Assert.Equal(g.Me.Models.Count, loaded.Player.Models.Count);
    }

    [Fact]
    public void A_game_saved_in_the_middle_of_a_training_run_carries_on_as_if_nothing_happened()
    {
        Game original = Game.New(seed: 8);
        original.WithStarterLab(gpus: 8);
        ModelState model = original.Train("mini", 200);
        original.Weeks(4);
        Assert.Equal(ModelStatus.Training, model.Status);
        Assert.True(model.ProgressCompute > 0);

        var resumed = new Game(Simulation.Load(TestData.Config(), original.Sim.Save()));
        original.Weeks(30);
        resumed.Weeks(30);

        Assert.Equal(original.Sim.Digest(), resumed.Sim.Digest());
        ModelState theirs = resumed.Me.Model(model.Id);
        Assert.Equal(ModelStatus.Trained, theirs.Status);
        Assert.Equal(model.Capability, theirs.Capability);
        Assert.Equal(model.Alignment, theirs.Alignment);
    }

    [Fact]
    public void Saving_every_week_and_loading_it_back_never_changes_the_story()
    {
        Game straight = Game.New(seed: 12);
        Game choppy = Game.New(seed: 12);
        for (int week = 0; week < 2 * 48; week++)
        {
            straight.Autopilot(1);
            choppy.Autopilot(1);
            choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        }
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_broken_save_says_what_is_wrong_instead_of_loading_half_a_game()
    {
        Game g = PlayedFor(48);
        GameConfig c = g.C;
        string save = g.Sim.Save();

        Assert.Contains("not readable", Assert.Throws<SaveException>(() => Simulation.Load(c, save.Substring(0, save.Length / 2))).Message);
        Assert.Contains("no schema", Assert.Throws<SaveException>(() => Simulation.Load(c, "{\"hello\":1}")).Message);
        Assert.Contains("newer version", Assert.Throws<SaveException>(() => Simulation.Load(c, save.Replace("\"schema\":" + GameState.CurrentSchema + ",", "\"schema\":99,"))).Message);
        Assert.Contains("unknown key", Assert.Throws<SaveException>(() => Simulation.Load(c, save.Replace("\"tick\":", "\"tock\":0,\"tick\":"))).Message);
        Assert.Contains("research-lab-deluxe", Assert.Throws<SaveException>(() => Simulation.Load(c, save.Replace("\"research-lab\"", "\"research-lab-deluxe\""))).Message);
    }

    [Fact]
    public void A_save_from_before_a_sector_was_added_loads_and_gains_the_new_sector()
    {
        Game g = PlayedFor(48);
        string save = g.Sim.Save();

        GameConfig newer = TestData.Config(c =>
        {
            SectorConfig extra = Syncer.Read<SectorConfig>(Syncer.Write(c.Sector("spam-search")), "sector");
            extra.IdValue = "translation";
            extra.Name = "Machine Translation";
            c.Market.Sectors.Add(extra);
            c.Version = "0.2.0-test";
        });

        Simulation loaded = Simulation.Load(newer, save);
        Assert.NotNull(loaded.State.Sector("translation"));
        Assert.Equal("0.2.0-test", loaded.State.ConfigVersion);
        loaded.StepMany(48);
    }

    [Fact]
    public void The_same_seed_gives_the_same_eight_years_to_the_last_bit()
    {
        Game a = PlayedFor(8 * 48, seed: 2026);
        Game b = PlayedFor(8 * 48, seed: 2026);
        Assert.Equal(a.Sim.Digest(), b.Sim.Digest());
        Assert.Equal(a.Sim.Save(), b.Sim.Save());
        _out.WriteLine("seed 2026 after 8 years: " + a.Sim.Digest());
    }

    [Fact]
    public void A_different_seed_gives_a_different_game()
    {
        var digests = new HashSet<string>();
        for (long seed = 1; seed <= 8; seed++) digests.Add(PlayedFor(2 * 48, seed).Sim.Digest());
        Assert.Equal(8, digests.Count);
    }

    [Fact]
    public void A_recorded_game_replays_to_the_same_state_from_its_orders_alone()
    {
        Game g = PlayedFor(4 * 48, seed: 21);
        Assert.True(g.Sim.Journal.Entries.Count > 20);

        // The journal itself is plain data: it can be written down and read back before it is replayed.
        ReplayLog log = ReplayLog.FromJson(g.Sim.Journal.ToJson());
        Simulation replayed = Simulation.Replay(TestData.Config(), log);
        Assert.Equal(g.Sim.Tick, replayed.Tick);
        Assert.Equal(g.Sim.Digest(), replayed.Digest());

        // And stopping the replay early gives the game as it stood that week.
        Game early = PlayedFor(48, seed: 21);
        Assert.Equal(early.Sim.Digest(), Simulation.Replay(TestData.Config(), log, 48).Digest());
    }

    [Fact]
    public void A_refused_order_is_not_journaled_and_changes_nothing()
    {
        Game g = Game.Solo();
        string before = g.Sim.Digest();
        Assert.False(g.Try(Command.Hire(g.Id, "researcher", 3)).Ok);
        Assert.Empty(g.Sim.Journal.Entries);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void The_dice_of_one_system_do_not_disturb_another()
    {
        // Two games differ only in how reckless the player is. Incidents use the safety dice, so the
        // training luck of every model, which uses the training dice, must come out the same.
        Game careful = Game.Solo(seed: 30).WithStarterLab(gpus: 8);
        Game reckless = Game.Solo(seed: 30).WithStarterLab(gpus: 8);
        for (int i = 0; i < 4; i++)
        {
            ModelState a = careful.Train("mini", 300);
            ModelState b = reckless.Train("mini", 0);
            careful.UntilTrained(a);
            reckless.UntilTrained(b);
            careful.Release(a, "game-ai");
            reckless.Release(b, "game-ai");
            careful.Weeks(40);
            reckless.Weeks(40);
            Assert.Equal(a.CapLuck, b.CapLuck);
            Assert.Equal(a.Capability, b.Capability);
        }
        Assert.True(reckless.Me.Models.Sum(m => m.Incidents) > 0);
        Assert.Equal(0, careful.Me.Models.Sum(m => m.Incidents));
    }

    [Fact]
    public void The_golden_run_has_not_moved()
    {
        // One pinned game: seed 1, the autopilot, one year. Any change to the rules or to the numbers in
        // default-config.json moves this digest. If you meant to change behaviour, run the test, copy the
        // new digest from the failure message into Golden below, and say why in the commit message.
        const string Golden = "0691ee3758a2482007d041e2e1c6bfb965e46371142702f9a24a4bd7c43bfd70";

        string digest = PlayedFor(48, seed: 1).Sim.Digest();
        Assert.True(digest == Golden, "The golden digest moved. New digest: " + digest);
    }

    [Fact]
    public void Eight_game_years_take_well_under_five_seconds()
    {
        PlayedFor(48);
        Stopwatch watch = Stopwatch.StartNew();
        Game g = PlayedFor(8 * 48);
        watch.Stop();
        _out.WriteLine("8 years, 4 labs: " + watch.ElapsedMilliseconds + " ms, save is " + g.Sim.Save().Length / 1024 + " KB");
        Assert.True(watch.ElapsedMilliseconds < 5000, "took " + watch.ElapsedMilliseconds + " ms");
    }

    [Fact]
    public void A_forty_year_game_keeps_its_logs_and_numbers_in_bounds()
    {
        // Endless play: nothing may grow without limit or wrap around.
        Game g = Game.New(seed: 3, tweak: c => c.Gauntlet.HistoryYears = 10);
        g.Autopilot(40 * 48);

        Assert.Equal(2035, g.Sim.Year);
        Assert.True(g.State.Events.Count <= g.C.EventLogMax);
        Assert.Equal(10, g.State.Gauntlets.Count);
        Assert.All(g.State.Labs, lab =>
        {
            Assert.True(lab.Ledger.History.Count <= g.C.Finance.HistoryMonths);
            Assert.InRange(lab.Cash, -10000000L, 1000000000000L);
            Assert.InRange(lab.Reputation, 0, 1000);
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        });
        Assert.All(g.State.Sectors, s => Assert.True(s.PoolPerWeek > 0));
        _out.WriteLine("after 40 years the save is " + g.Sim.Save().Length / 1024 + " KB");
    }
}
