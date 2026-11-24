using System.Collections.Generic;
using AiLabs.Rules.Core;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The small parts everything else stands on: the seeded dice, whole-number maths and strict JSON.</summary>
public class CoreTests
{
    [Fact]
    public void The_same_seed_rolls_the_same_dice()
    {
        var a = new Rng(42);
        var b = new Rng(42);
        for (int i = 0; i < 1000; i++) Assert.Equal(a.NextU64(), b.NextU64());
    }

    [Fact]
    public void The_dice_are_the_same_on_every_machine()
    {
        // SplitMix64 from seed 0: the published first outputs. If these move, every saved game and golden moves.
        var rng = new Rng(0);
        Assert.Equal(0xE220A8397B1DCDAFUL, rng.NextU64());
        Assert.Equal(0x6E789E6AA1B965F4UL, rng.NextU64());
        Assert.Equal(0x06C45D188009454FUL, rng.NextU64());
    }

    [Fact]
    public void A_range_stays_inside_its_ends_and_reaches_both()
    {
        var rng = new Rng(7);
        var seen = new HashSet<int>();
        for (int i = 0; i < 2000; i++)
        {
            int v = rng.Range(-3, 3);
            Assert.InRange(v, -3, 3);
            seen.Add(v);
        }
        Assert.Equal(7, seen.Count);
    }

    [Fact]
    public void A_chance_of_300_permille_comes_up_about_three_times_in_ten()
    {
        var rng = new Rng(11);
        int hits = 0;
        for (int i = 0; i < 20000; i++) if (rng.Chance(300)) hits++;
        Assert.InRange(hits, 5700, 6300);
        Assert.False(new Rng(1).Chance(0));
        Assert.True(new Rng(1).Chance(1000));
    }

    [Fact]
    public void Each_system_has_its_own_dice_so_one_never_shifts_another()
    {
        RngSet a = RngSet.FromSeed(5);
        RngSet b = RngSet.FromSeed(5);
        for (int i = 0; i < 50; i++) a.Safety.NextU64();
        Assert.Equal(a.Training.NextU64(), b.Training.NextU64());
        Assert.NotEqual(a.Training.State, a.Rivals.State);
    }

    [Fact]
    public void The_dice_state_survives_a_save()
    {
        var rng = new Rng(99);
        rng.NextU64();
        Rng copy = Syncer.Read<Rng>(Json.Parse(Json.Write(Syncer.Write(rng), false)), "rng");
        Assert.Equal(rng.NextU64(), copy.NextU64());
    }

    [Fact]
    public void Json_round_trips_text_numbers_lists_and_objects()
    {
        string text = "{\"name\":\"Fable \\\"Mini\\\" \\u00e9\\n\",\"big\":-9007199254740993,\"on\":true,\"list\":[1,2,3],\"nested\":{\"empty\":[]}}";
        JsonValue value = Json.Parse(text);
        Assert.Equal(-9007199254740993L, value.Get("big").Int);
        Assert.Equal("Fable \"Mini\" \u00e9\n", value.Get("name").Str);
        Assert.Equal(Json.Write(value, false), Json.Write(Json.Parse(Json.Write(value, true)), false));
    }

    [Theory]
    [InlineData("{\"a\":1.5}", "whole numbers")]
    [InlineData("{\"a\":1e3}", "whole numbers")]
    [InlineData("{\"a\":1,\"a\":2}", "twice")]
    [InlineData("{\"a\":1,}", "")]
    [InlineData("{\"a\":1} extra", "")]
    [InlineData("", "")]
    public void Json_refuses_what_the_rules_cannot_use(string text, string because)
    {
        JsonException e = Assert.Throws<JsonException>(() => Json.Parse(text));
        Assert.Contains(because, e.Message);
    }

    [Fact]
    public void Whole_number_maths_rounds_toward_zero()
    {
        Assert.Equal(333, MathI.MulPermille(1000, 333));
        Assert.Equal(0, MathI.MulPermille(1, 999));
        Assert.Equal(-1, MathI.MulPermille(-1999, 1000) / 1000);
        Assert.Equal(0, MathI.MulDiv(5, 5, 0));
        Assert.Equal(-2, MathI.FloorDiv(-3, 2));
        Assert.Equal(1, MathI.FloorDiv(3, 2));
        Assert.Equal(-2, MathI.FloorDiv(-4, 2));
        Assert.Equal(5, MathI.Clamp(9, 0, 5));
    }

    [Fact]
    public void A_digest_is_the_sha256_of_the_text()
    {
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", Digest.Sha256Hex("abc"));
    }
}
