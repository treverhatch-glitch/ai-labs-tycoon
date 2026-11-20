using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Old saves keep loading: the JSON helpers, the plumbing of schema 2 and a real schema-1 save.</summary>
public class MigrationTests
{
    /// <summary>
    /// A save written by the schema-1 game (seed 7, tick 30): the player has 6 rented and 12 owned GPUs in a
    /// server closet, a mini in training, 305,340 in cash and 180 compute. After one more week the old game had 302,732.
    /// </summary>
    public static string Schema1Save()
    {
        return File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema1-save.json"));
    }

    /// <summary>
    /// A save written by the schema-2 game (seed 9, tick 62, all rivals): the player has 5 owned GPUs at the HQ in
    /// one batch, 672,710 in cash and 5 techs.
    /// </summary>
    public static string Schema2Save()
    {
        return File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema2-save.json"));
    }

    [Fact]
    public void JsonPatch_adds_a_key_only_when_it_is_missing()
    {
        JsonValue o = Json.Parse("{\"a\":1}");
        JsonPatch.AddIfMissing(o, "a", JsonValue.Of(9L));
        JsonPatch.AddIfMissing(o, "b", JsonValue.Of(2L));
        Assert.Equal("{\"a\":1,\"b\":2}", Json.Write(o, false));
    }

    [Fact]
    public void JsonPatch_removes_a_key_and_hands_back_what_it_held()
    {
        JsonValue o = Json.Parse("{\"a\":1,\"b\":2,\"c\":3}");
        Assert.Equal(2, JsonPatch.Remove(o, "b").Int);
        Assert.Null(JsonPatch.Remove(o, "b"));
        Assert.Equal("{\"a\":1,\"c\":3}", Json.Write(o, false));
    }

    [Fact]
    public void JsonPatch_visits_every_object_a_dotted_path_reaches_through_lists()
    {
        JsonValue root = Json.Parse("{\"labs\":[{\"id\":1,\"ledger\":{\"history\":[{\"totals\":{\"x\":1}},{\"totals\":{\"x\":2}}]}},{\"id\":2,\"ledger\":{\"history\":[]}},{\"id\":3}]}");
        var seen = new List<long>();
        JsonPatch.ForEach(root, "labs.ledger.history.totals", t => seen.Add(t.Get("x").Int));
        Assert.Equal(new long[] { 1, 2 }, seen);

        var ids = new List<long>();
        JsonPatch.ForEach(root, "labs", l => ids.Add(l.Get("id").Int));
        Assert.Equal(new long[] { 1, 2, 3 }, ids);

        int roots = 0;
        JsonPatch.ForEach(root, "", _ => roots++);
        Assert.Equal(1, roots);
    }

    [Fact]
    public void The_four_original_dice_streams_keep_their_values_and_the_new_ones_come_after()
    {
        var root = new Rng(1);
        RngSet set = RngSet.FromSeed(1);
        Assert.Equal((long)root.NextU64(), set.Training.State);
        Assert.Equal((long)root.NextU64(), set.Safety.State);
        Assert.Equal((long)root.NextU64(), set.Ranking.State);
        Assert.Equal((long)root.NextU64(), set.Rivals.State);
        Assert.Equal((long)root.NextU64(), set.World.State);
        Assert.Equal((long)root.NextU64(), set.Chips.State);
        Assert.Equal((long)root.NextU64(), set.Shady.State);
    }

    [Fact]
    public void A_save_from_before_the_new_streams_gets_them_from_its_seed_and_keeps_the_old_four()
    {
        Simulation sim = Simulation.Load(TestData.Config(), Schema1Save());
        long seed = sim.State.Seed;
        Assert.Equal(7, seed);
        Assert.Equal((long)new Rng(seed ^ 0x57_0000_0001L).NextU64(), sim.State.Rng.World.State);
        Assert.Equal((long)new Rng(seed ^ 0x57_0000_0002L).NextU64(), sim.State.Rng.Chips.State);
        Assert.Equal((long)new Rng(seed ^ 0x57_0000_0003L).NextU64(), sim.State.Rng.Shady.State);

        JsonValue old = Json.Parse(Schema1Save()).Get("rng");
        Assert.Equal(old.Get("training").Get("state").Int, sim.State.Rng.Training.State);
        Assert.Equal(old.Get("safety").Get("state").Int, sim.State.Rng.Safety.State);
        Assert.Equal(old.Get("ranking").Get("state").Int, sim.State.Rng.Ranking.State);
        Assert.Equal(old.Get("rivals").Get("state").Int, sim.State.Rng.Rivals.State);
    }

    [Fact]
    public void Every_ledger_line_is_in_the_totals_and_adds_up_in_the_net()
    {
        var totals = new LedgerTotals();
        long expected = 0;
        long amount = 1;
        foreach (LedgerLine line in Enum.GetValues(typeof(LedgerLine)))
        {
            totals.Add(line, amount);
            Assert.Equal(amount, totals.Get(line));
            expected += amount;
            amount *= 3;
        }
        Assert.Equal(expected, totals.Net);
        Assert.Equal(19, Enum.GetValues(typeof(LedgerLine)).Length);
    }

    [Fact]
    public void A_schema_1_save_loads_with_every_ledger_total_at_zero_for_the_new_lines()
    {
        Simulation sim = Simulation.Load(TestData.Config(), Schema1Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs)
        {
            var all = new List<LedgerTotals> { lab.Ledger.ThisWeek, lab.Ledger.LastWeek, lab.Ledger.ThisMonth, lab.Ledger.Lifetime };
            all.AddRange(lab.Ledger.History.Select(h => h.Totals));
            foreach (LedgerTotals t in all)
                foreach (LedgerLine line in new[] { LedgerLine.Land, LedgerLine.SiteBuild, LedgerLine.Power, LedgerLine.ChipDevelopment, LedgerLine.ChipSales, LedgerLine.GreyRevenue, LedgerLine.Laundered, LedgerLine.CapitalIn })
                    Assert.Equal(0, t.Get(line));
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }

    [Fact]
    public void A_migrated_save_plays_on_and_cash_is_still_the_founding_cash_plus_the_ledger()
    {
        Simulation sim = Simulation.Load(TestData.Config(), Schema1Save());
        Assert.Equal(305340, sim.Player.Cash);
        for (int week = 0; week < 2 * 48; week++)
        {
            sim.Step();
            foreach (LabState lab in sim.State.Labs) Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }

    [Fact]
    public void Migrating_the_same_save_twice_gives_the_same_game()
    {
        var c = TestData.Config();
        string a = Simulation.Load(c, Schema1Save()).Save();
        string b = Simulation.Load(c, Schema1Save()).Save();
        Assert.Equal(a, b);
        Assert.Equal(a, Simulation.Load(c, a).Save());
    }

    [Fact]
    public void A_save_from_a_newer_game_and_a_save_with_no_schema_are_refused_in_plain_words()
    {
        var c = TestData.Config();
        JsonValue newer = Json.Parse(Schema1Save());
        newer.Set("schema", JsonValue.Of(GameState.CurrentSchema + 1L));
        var e = Assert.Throws<SaveException>(() => Simulation.Load(c, Json.Write(newer, false)));
        Assert.Contains("newer version", e.Message);

        JsonValue none = Json.Parse(Schema1Save());
        JsonPatch.Remove(none, "schema");
        Assert.Throws<SaveException>(() => Simulation.Load(c, Json.Write(none, false)));
    }

    [Fact]
    public void A_schema_1_save_gets_the_world_map_too()
    {
        Simulation sim = Simulation.Load(TestData.Config(), Schema1Save());
        Assert.Equal(sim.Config.World.Sites.Count, sim.State.Sites.Count);
        foreach (LabState lab in sim.State.Labs)
        {
            Assert.Equal(sim.Config.World.Blocs.Count, lab.Blocs.Count);
            Assert.Empty(lab.Holdings);
            Assert.Equal("us", lab.HomeBlocId);
        }
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_schema_2_save_gets_sites_blocs_and_home_blocs_and_keeps_its_hardware_at_the_hq()
    {
        var c = TestData.Config();
        Simulation sim = Simulation.Load(c, Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(62, sim.Tick);
        Assert.Equal(672710, sim.Player.Cash);
        Assert.Equal(5, LabQueries.HqGpus(sim.Player));
        Assert.Equal(c.World.Sites.Count, sim.State.Sites.Count);
        Assert.All(sim.State.Sites, site => Assert.Equal(-1, site.HeatwaveUntilTick));
        foreach (LabState lab in sim.State.Labs)
        {
            Assert.Equal(c.World.Blocs.Count, lab.Blocs.Count);
            Assert.All(lab.Blocs, b => { Assert.Equal(0, b.Attention); Assert.Equal(0, b.Level); });
            Assert.Equal(string.IsNullOrEmpty(lab.RivalId) ? c.PlayerStart.HomeBlocId : c.Rival(lab.RivalId).HomeBlocId, lab.HomeBlocId);
            Assert.Equal(0, lab.IncidentsThisMonth);
        }
        Assert.Empty(SaveGame.Problems(c, sim.State));
    }

    [Fact]
    public void A_schema_2_save_plays_two_years_and_replays_from_its_journal_to_the_same_game()
    {
        var c = TestData.Config();
        Simulation sim = Simulation.Load(c, Schema2Save());
        sim.Apply(Command.BuyGpus(sim.Player.Id, 1));
        for (int week = 0; week < 2 * 48; week++)
        {
            sim.Step();
            foreach (LabState lab in sim.State.Labs) Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
        ReplayLog log = ReplayLog.FromJson(sim.Journal.ToJson());
        Assert.Equal(sim.Digest(), Simulation.Replay(c, log).Digest());
        Assert.Equal(sim.Save(), Simulation.Load(c, sim.Save()).Save());
    }
}
