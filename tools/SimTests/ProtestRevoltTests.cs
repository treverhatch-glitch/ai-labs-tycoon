using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q61: high home attention amplifies protest headlines (permit revolt lite).</summary>
public class ProtestRevoltTests
{
    static Game NoDecay(int seed = 2) =>
        Game.Solo(seed, c => c.World.Attention.DecayPermillePerMonth = 0);

    static void Fire(Game g, string eventId)
    {
        g.State.Tick = EraSystem.EventTick(g.C, g.C.WorldEvent(eventId)) - 1;
        g.Weeks(2);
    }

    [Fact]
    public void Shipped_protest_revolt_config()
    {
        AttentionConfig a = TestData.Config().World.Attention;
        Assert.Equal(600, a.ProtestRevoltAttentionAt);
        Assert.Equal(500, a.ProtestRevoltBonusPermille);
        ConfigValidator.Validate(TestData.Config());
    }

    [Fact]
    public void Below_threshold_base_bump_only()
    {
        Game g = NoDecay(9);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 500;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Fire(g, "teen-chatbot");
        Assert.Equal(before + 35, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void At_threshold_adds_bonus_permille()
    {
        Game g = NoDecay(9);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 600;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Fire(g, "teen-chatbot");
        int expected = 35 + 35 * 500 / 1000;
        Assert.Equal(before + expected, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Attention_rose_records_total_delta()
    {
        Game g = NoDecay(9);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 700;
        Fire(g, "pause-letter");
        int bump = 45 + 45 * 500 / 1000;
        var ev = g.State.Events.Last(e => e.Kind == EventKind.AttentionRose && e.LabId == g.Id);
        Assert.Equal(bump, ev.Amount);
    }

    [Fact]
    public void Zero_bonus_config_disables_revolt()
    {
        Game g = NoDecay();
        g.C.World.Attention.ProtestRevoltBonusPermille = 0;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 800;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Fire(g, "teen-chatbot");
        Assert.Equal(before + 35, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Zero_threshold_never_bonus()
    {
        Game g = NoDecay();
        g.C.World.Attention.ProtestRevoltAttentionAt = 0;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 900;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Fire(g, "teen-chatbot");
        Assert.Equal(before + 35, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Named_bloc_protests_use_that_standing()
    {
        Game g = NoDecay();
        g.Me.Bloc("eu").Attention = 650;
        WorldEventConfig e = g.C.WorldEvent("pause-letter");
        e.ProtestBlocId = "eu";
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.True(g.Me.Bloc("eu").Attention >= 650 + 45 + 45 * 500 / 1000);
    }

    [Fact]
    public void Bankrupt_labs_skipped()
    {
        Game g = NoDecay();
        g.Me.Bankrupt = true;
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Fire(g, "pause-letter");
        Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Events_with_zero_protest_ignored()
    {
        Game g = NoDecay();
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 700;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.State.Tick = EraSystem.EventTick(g.C, g.C.WorldEvent("bubble-peak")) - 1;
        g.Weeks(2);
        Assert.Equal(before, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Max_attention_clamped_at_permille()
    {
        Game g = NoDecay(9);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 980;
        Fire(g, "pause-letter");
        Assert.Equal(1000, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Government_decay_still_runs_after_protest()
    {
        Game g = Game.Solo(1, c => c.World.Attention.DecayPermillePerMonth = 200);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 620;
        Fire(g, "teen-chatbot");
        int afterEvent = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Weeks(4);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention < afterEvent);
    }

    [Fact]
    public void Save_load_then_protest_matches()
    {
        Game g = NoDecay(9);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 650;
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        loaded.Me.Bloc(loaded.Me.HomeBlocId).Attention = 650;
        Fire(loaded, "teen-chatbot");
        int bump = 35 + 35 * 500 / 1000;
        Assert.Equal(650 + bump, loaded.Me.Bloc(loaded.Me.HomeBlocId).Attention);
    }
}