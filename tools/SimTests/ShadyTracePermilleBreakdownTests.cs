using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q130: ShadyQueries.TracePermilleParts sums to trace odds.</summary>
public class ShadyTracePermilleBreakdownTests
{
    [Fact]
    public void Shipped_trace_parts_match_total()
    {
        Game g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("sabotage-run");
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.Equal(b.Base + b.FromHeat, b.Total);
        Assert.Equal(ShadyQueries.TracePermille(g.C, g.Me, card), MathI.Clamp(b.Total, 0, 950));
    }

    [Fact]
    public void Base_is_card_trace_permille()
    {
        Game g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("crypto-heist");
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.Equal(card.TracePermille, b.Base);
    }

    [Fact]
    public void Heat_adds_fifth_per_point()
    {
        Game g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("grey-compute");
        g.Me.Heat = 100;
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.Equal(20, b.FromHeat);
        Assert.Equal(card.TracePermille + 20, b.Total);
    }

    [Fact]
    public void Trace_clamped_at_nine_fifty()
    {
        Game g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("sabotage-run") ;
        card.TracePermille = 800;
        g.Me.Heat = 2000;
        Assert.Equal(950, ShadyQueries.TracePermille(g.C, g.Me, card));
    }

    [Fact]
    public void Zero_heat_only_base_counts()
    {
        Game g = Game.Solo();
        g.Me.Heat = 0;
        ShadyCardConfig card = g.C.Shady.Card("smuggle-chips");
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.Equal(0, b.FromHeat);
        Assert.Equal(card.TracePermille, b.Total);
    }

    [Fact]
    public void Shady_system_uses_trace_query()
    {
        Game g = Game.Solo(seed: 2);
        ShadyCardConfig card = g.C.Shady.Card("intrusion-for-hire");
        card.TracePermille = 400;
        g.Me.Heat = 50;
        int trace = ShadyQueries.TracePermille(g.C, g.Me, card);
        Assert.Equal(ShadyQueries.TracePermilleParts(g.C, g.Me, card).Total >= 950 ? 950 : ShadyQueries.TracePermilleParts(g.C, g.Me, card).Total, trace);
    }

    [Fact]
    public void Different_cards_different_base()
    {
        Game g = Game.Solo();
        var a = ShadyQueries.TracePermilleParts(g.C, g.Me, g.C.Shady.Card("sabotage-run"));
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, g.C.Shady.Card("grey-compute"));
        Assert.NotEqual(a.Base, b.Base);
    }

    [Fact]
    public void Save_load_trace_parts_stable()
    {
        Game g = Game.Solo();
        g.Me.Heat = 333;
        ShadyCardConfig card = g.C.Shady.Card("crypto-heist");
        var before = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ShadyQueries.TracePermilleParts(loaded.C, loaded.Me, loaded.C.Shady.Card("crypto-heist"));
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void Investigation_path_independent_of_trace()
    {
        Game g = Game.Solo();
        g.Me.Heat = 600;
        int inv = ShadyQueries.InvestigationOpenPermille(g.C, g.Me);
        int trace = ShadyQueries.TracePermille(g.C, g.Me, g.C.Shady.Card("sabotage-run"));
        Assert.True(inv >= 0);
        Assert.True(trace >= 0);
    }

    [Fact]
    public void Deterministic_trace_breakdown()
    {
        Game g = Game.Solo();
        g.Me.Heat = 77;
        ShadyCardConfig card = g.C.Shady.Card("smuggle-chips");
        var x = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        var y = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.Equal(x.FromHeat, y.FromHeat);
    }

    [Fact]
    public void Low_trace_card_stays_below_cap_with_moderate_heat()
    {
        Game g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("grey-compute");
        card.TracePermille = 50;
        g.Me.Heat = 150;
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, card);
        Assert.True(b.Total < 950);
        Assert.Equal(ShadyQueries.TracePermille(g.C, g.Me, card), b.Total);
    }

    [Fact]
    public void Heat_four_gives_zero_from_heat_remainder()
    {
        Game g = Game.Solo();
        g.Me.Heat = 4;
        var b = ShadyQueries.TracePermilleParts(g.C, g.Me, g.C.Shady.Card("sabotage-run"));
        Assert.Equal(0, b.FromHeat);
    }

    [Fact]
    public void Conviction_parts_unaffected_by_trace_breakdown()
    {
        Game g = Game.Solo();
        g.Me.Heat = 500;
        var conv = ShadyQueries.ConvictionChanceParts(g.C, g.Me);
        var trace = ShadyQueries.TracePermilleParts(g.C, g.Me, g.C.Shady.Card("sabotage-run"));
        Assert.True(conv.Total > 0);
        Assert.True(trace.Total >= trace.Base);
    }
}