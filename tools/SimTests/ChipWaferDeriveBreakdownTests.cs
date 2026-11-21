using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q126: ChipQueries.WaferDeriveParts sums to Derive wafer cost.</summary>
public class ChipWaferDeriveBreakdownTests
{
    const string Inf = "inference", Train = "training";

    [Fact]
    public void Shipped_wafer_derive_matches_breakdown_total()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, Inf, "n28", "small", "fp16", "ddr", "air");
        var b = ChipQueries.WaferDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        Assert.True(spec.Valid);
        Assert.Equal(spec.WaferCost, b.Total);
        Assert.Equal(b.AfterNode + b.FromDie + b.FromMemory + b.FromCooling, b.Total);
    }

    [Fact]
    public void Invalid_options_yield_zero_wafer_breakdown()
    {
        GameConfig c = TestData.Config();
        var b = ChipQueries.WaferDeriveParts(c, "missing", "n28", "small", "fp16", "ddr", "air");
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void After_node_is_node_wafer_base()
    {
        GameConfig c = TestData.Config();
        ChipNodeConfig node = c.Chips.Node("n28");
        var b = ChipQueries.WaferDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        Assert.Equal(node.WaferCostPerChip, b.AfterNode);
    }

    [Fact]
    public void Target_id_ignored_on_wafer_stack()
    {
        GameConfig c = TestData.Config();
        var inf = ChipQueries.WaferDeriveParts(c, Inf, "n5", "medium", "fp16", "ddr", "air");
        var trn = ChipQueries.WaferDeriveParts(c, Train, "n5", "medium", "fp16", "ddr", "air");
        Assert.Equal(inf.Total, trn.Total);
    }

    [Fact]
    public void Large_die_raises_die_wafer_delta()
    {
        GameConfig c = TestData.Config();
        var small = ChipQueries.WaferDeriveParts(c, Train, "n28", "small", "fp16", "ddr", "air");
        var med = ChipQueries.WaferDeriveParts(c, Train, "n28", "medium", "fp16", "ddr", "air");
        Assert.True(med.FromDie > small.FromDie);
        Assert.True(med.Total > small.Total);
    }

    [Fact]
    public void Hbm_memory_raises_wafer_cost()
    {
        GameConfig c = TestData.Config();
        var ddr = ChipQueries.WaferDeriveParts(c, Train, "n5", "medium", "fp8", "ddr", "air");
        var hbm = ChipQueries.WaferDeriveParts(c, Train, "n5", "medium", "fp8", "hbm", "air");
        Assert.True(hbm.FromMemory > ddr.FromMemory);
        Assert.True(hbm.Total > ddr.Total);
    }

    [Fact]
    public void Optical_cooling_wafer_delta_differs_from_air()
    {
        GameConfig c = TestData.Config();
        var air = ChipQueries.WaferDeriveParts(c, Inf, "n28", "medium", "fp16", "ddr", "air");
        var optical = ChipQueries.WaferDeriveParts(c, Inf, "n28", "medium", "fp16", "ddr", "optical");
        Assert.NotEqual(air.FromCooling, optical.FromCooling);
    }

    [Fact]
    public void Spec_wafer_on_design_matches_breakdown()
    {
        var g = ChipHardeningSupport.Lab(2013, rivals: false);
        ChipDesign injected = ChipHardeningSupport.InjectReady(g, g.Me, Train, "n7", "medium");
        g.Me.ChipDesigns.Clear();
        g.Me.ChipDesigns.Add(injected);
        ChipSpec spec = ChipQueries.Spec(g.C, injected);
        var b = ChipQueries.WaferDeriveParts(g.C, injected.Target, injected.NodeId, injected.DieId, injected.FormatId, injected.MemoryId, injected.CoolingId);
        Assert.Equal(spec.WaferCost, b.Total);
    }

    [Fact]
    public void N3_exotic_node_wafer_matches_derive()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, Train, "n3", "medium", "fp16", "ddr", "air");
        var b = ChipQueries.WaferDeriveParts(c, Train, "n3", "medium", "fp16", "ddr", "air");
        Assert.Equal(spec.WaferCost, b.Total);
    }

    [Fact]
    public void Format_choice_does_not_change_wafer_total()
    {
        GameConfig c = TestData.Config();
        var fp16 = ChipQueries.WaferDeriveParts(c, Train, "n28", "small", "fp16", "ddr", "air");
        var fp8 = ChipQueries.WaferDeriveParts(c, Train, "n28", "small", "fp8", "ddr", "air");
        Assert.Equal(fp16.Total, fp8.Total);
    }

    [Fact]
    public void Chiplet_die_wafer_premium_over_medium()
    {
        GameConfig c = TestData.Config();
        var med = ChipQueries.WaferDeriveParts(c, Train, "n28", "medium", "fp16", "ddr", "air");
        var chiplet = ChipQueries.WaferDeriveParts(c, Train, "n28", "chiplet-mcm", "fp16", "ddr", "air");
        Assert.True(chiplet.FromDie > med.FromDie);
    }

    [Fact]
    public void Wafer_parts_deterministic()
    {
        GameConfig c = TestData.Config();
        var a = ChipQueries.WaferDeriveParts(c, Inf, "n7", "small", "fp16", "ddr", "liquid");
        var b = ChipQueries.WaferDeriveParts(c, Inf, "n7", "small", "fp16", "ddr", "liquid");
        Assert.Equal(a.Total, b.Total);
        Assert.Equal(a.FromMemory, b.FromMemory);
    }

    [Fact]
    public void Shortage_order_cost_uses_same_wafer_derive()
    {
        var g = ChipHardeningSupport.Lab(2014, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, Train, "n28", "small");
        ChipSpec spec = ChipQueries.Spec(g.C, d);
        var w = ChipQueries.WaferDeriveParts(g.C, d.Target, d.NodeId, d.DieId, d.FormatId, d.MemoryId, d.CoolingId);
        Assert.Equal(spec.WaferCost, w.Total);
        long order = ChipQueries.OrderCost(spec, spec.InitialYieldPermille, 4);
        var o = ChipQueries.OrderCostParts(spec, spec.InitialYieldPermille, 4);
        Assert.Equal(o.WaferBase, w.Total);
        Assert.Equal(order, o.Total);
    }
}