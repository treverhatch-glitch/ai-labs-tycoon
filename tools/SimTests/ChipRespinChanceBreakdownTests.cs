using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q128: ChipQueries.RespinChanceParts matches tape-out respin odds.</summary>
public class ChipRespinChanceBreakdownTests
{
    [Fact]
    public void Shipped_respin_parts_total_matches_query()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 0);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        Assert.Equal(b.FromNode + b.FromEngineers, b.Total);
        Assert.Equal(ChipQueries.RespinChancePermille(g.C, g.Me, node), b.Total);
    }

    [Fact]
    public void From_node_is_node_base_respin()
    {
        Game g = Game.Solo();
        ChipNodeConfig node = g.C.Chips.Node("n5");
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        Assert.Equal(node.RespinPermille, b.FromNode);
    }

    [Fact]
    public void Engineers_cut_respin_via_from_engineers()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 0);
        ChipNodeConfig node = g.C.Chips.Node("n7");
        var bare = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        g.Hire("engineer", 4);
        var staffed = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        Assert.True(staffed.FromEngineers < 0);
        Assert.True(staffed.Total < bare.Total);
    }

    [Fact]
    public void Engineer_cap_limits_respin_cut()
    {
        Game g = Game.Solo(tweak: c => c.Chips.RespinEngineerCap = 3);
        g.WithStarterLab(engineers: 3);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        int maxCut = 3 * g.C.Chips.RespinPerEngineerPermille;
        Assert.Equal(-maxCut, b.FromEngineers);
    }

    [Fact]
    public void Null_node_zero_breakdown()
    {
        Game g = Game.Solo();
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, null);
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void Exotic_n3_node_base_respin_matches_config()
    {
        Game g = Game.Solo();
        ChipNodeConfig n3 = g.C.Chips.Node("n3");
        var parts = ChipQueries.RespinChanceParts(g.C, g.Me, n3);
        Assert.Equal(n3.RespinPermille, parts.FromNode);
        Assert.Equal(ChipQueries.RespinChancePermille(g.C, g.Me, n3), parts.Total);
    }

    [Fact]
    public void Respin_never_below_zero()
    {
        Game g = Game.Solo(tweak: c =>
        {
            c.Chips.RespinEngineerCap = 4;
            c.Chips.RespinPerEngineerPermille = 50;
            c.Chips.Node("n28").RespinPermille = 100;
        });
        g.WithStarterLab(engineers: 4);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        Assert.Equal(0, ChipQueries.RespinChanceParts(g.C, g.Me, node).Total);
    }

    [Fact]
    public void Save_load_respin_parts_stable()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 2);
        ChipNodeConfig node = g.C.Chips.Node("n5");
        var before = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ChipQueries.RespinChanceParts(loaded.C, loaded.Me, loaded.C.Chips.Node("n5"));
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void Chip_system_uses_same_respin_permille()
    {
        Game g = Game.Solo(seed: 9).WithStarterLab(engineers: 1);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        int chance = ChipQueries.RespinChanceParts(g.C, g.Me, node).Total;
        Assert.Equal(chance, ChipQueries.RespinChancePermille(g.C, g.Me, node));
    }

    [Fact]
    public void Deterministic_across_calls()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 3);
        ChipNodeConfig node = g.C.Chips.Node("n7");
        var a = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        Assert.Equal(a.FromEngineers, b.FromEngineers);
        Assert.Equal(a.Total, b.Total);
    }

    [Fact]
    public void Zero_engineers_from_engineers_zero()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 0);
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, g.C.Chips.Node("n28"));
        Assert.Equal(0, b.FromEngineers);
        Assert.Equal(b.FromNode, b.Total);
    }

    [Fact]
    public void N5_tapeout_respin_matches_existing_chip_test_math()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 0);
        ChipNodeConfig n5 = g.C.Chips.Node("n5");
        int chance = ChipQueries.RespinChancePermille(g.C, g.Me, n5);
        var b = ChipQueries.RespinChanceParts(g.C, g.Me, n5);
        Assert.Equal(chance, b.Total);
    }

    [Fact]
    public void Rivals_lab_engineers_affect_only_that_lab()
    {
        Game g = Game.New();
        g.WithStarterLab(engineers: 1);
        ChipNodeConfig node = g.C.Chips.Node("n28");
        var player = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        LabState rival = g.Rival("openaye");
        g.Hire("engineer", 3);
        var staffedPlayer = ChipQueries.RespinChanceParts(g.C, g.Me, node);
        var rivalBare = ChipQueries.RespinChanceParts(g.C, rival, node);
        Assert.NotEqual(player.Total, staffedPlayer.Total);
        Assert.Equal(rivalBare.Total, ChipQueries.RespinChancePermille(g.C, rival, node));
    }
}