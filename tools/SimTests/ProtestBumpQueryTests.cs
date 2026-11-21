using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q74: ProtestSystem.ProtestAttentionBump matches OnWorldEvent attention deltas.</summary>
public class ProtestBumpQueryTests
{
    static Game NoDecay(int seed = 3, System.Action<GameConfig> tweak = null) =>
        Game.Solo(seed, c =>
        {
            c.World.Attention.DecayPermillePerMonth = 0;
            tweak?.Invoke(c);
        });

    [Fact]
    public void Shipped_revolt_threshold_config()
    {
        AttentionConfig a = TestData.Config().World.Attention;
        ConfigValidator.Validate(TestData.Config());
        Assert.True(a.ProtestRevoltAttentionAt > 0);
    }

    [Fact]
    public void Main_path_base_only_below_threshold()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(35, ProtestSystem.ProtestAttentionBump(c, 400, 35));
    }

    [Fact]
    public void Main_path_bonus_at_threshold()
    {
        GameConfig c = TestData.Config();
        int bump = ProtestSystem.ProtestAttentionBump(c, 600, 35);
        Assert.Equal(35 + 35 * c.World.Attention.ProtestRevoltBonusPermille / 1000, bump);
    }

    [Fact]
    public void Zero_base_returns_zero()
    {
        Assert.Equal(0, ProtestSystem.ProtestAttentionBump(TestData.Config(), 800, 0));
    }

    [Fact]
    public void Max_attention_before_clamp_by_event()
    {
        Game g = NoDecay();
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 990;
        int bump = ProtestSystem.ProtestAttentionBump(g.C, 990, 45);
        ProtestSystem.OnWorldEvent(g.C, g.State, g.C.WorldEvent("pause-letter"));
        Assert.Equal(MathI.Min(1000, 990 + bump), g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Zero_bonus_config_no_extra()
    {
        Game g = NoDecay(3, c => c.World.Attention.ProtestRevoltBonusPermille = 0);
        Assert.Equal(35, ProtestSystem.ProtestAttentionBump(g.C, 700, 35));
    }

    [Fact]
    public void Zero_threshold_config_no_extra()
    {
        Game g = NoDecay(3, c => c.World.Attention.ProtestRevoltAttentionAt = 0);
        Assert.Equal(45, ProtestSystem.ProtestAttentionBump(g.C, 900, 45));
    }

    [Fact]
    public void Bankrupt_lab_skipped_by_on_world_event()
    {
        Game g = NoDecay();
        g.Me.Bankrupt = true;
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ProtestSystem.OnWorldEvent(g.C, g.State, g.C.WorldEvent("teen-chatbot"));
        Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Save_load_config_preserves_bump_math()
    {
        Game g = NoDecay();
        int expected = ProtestSystem.ProtestAttentionBump(g.C, 650, 35);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(expected, ProtestSystem.ProtestAttentionBump(loaded.C, 650, 35));
    }

    [Fact]
    public void On_world_event_matches_query_delta()
    {
        Game g = NoDecay();
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 620;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        WorldEventConfig e = g.C.WorldEvent("teen-chatbot");
        int bump = ProtestSystem.ProtestAttentionBump(g.C, before, e.ProtestAttentionPermille);
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.Equal(before + bump, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Attention_rose_amount_equals_query_bump()
    {
        Game g = NoDecay();
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 700;
        WorldEventConfig e = g.C.WorldEvent("pause-letter");
        int bump = ProtestSystem.ProtestAttentionBump(g.C, 700, e.ProtestAttentionPermille);
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        GameEvent ev = g.State.Events.Find(x => x.Kind == EventKind.AttentionRose && x.LabId == g.Id);
        Assert.NotNull(ev);
        Assert.Equal(bump, ev.Amount);
    }

    [Fact]
    public void Named_bloc_standing_uses_query()
    {
        Game g = NoDecay();
        g.Me.Bloc("eu").Attention = 650;
        WorldEventConfig e = g.C.WorldEvent("pause-letter");
        e.ProtestBlocId = "eu";
        int bump = ProtestSystem.ProtestAttentionBump(g.C, 650, e.ProtestAttentionPermille);
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.Equal(650 + bump, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Trust_decay_cross_system_after_protest_bump()
    {
        Game g = Game.Solo(1, c => c.World.Attention.DecayPermillePerMonth = 150);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = 610;
        ProtestSystem.OnWorldEvent(g.C, g.State, g.C.WorldEvent("teen-chatbot"));
        int after = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Weeks(8);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention < after);
    }
}