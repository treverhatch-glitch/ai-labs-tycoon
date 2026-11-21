using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q118: ChipQueries.ComputeDeriveParts sums to Derive compute.</summary>
public class ChipComputeDeriveBreakdownTests
{
    const string Inf = "inference", Train = "training";

    [Fact]
    public void Shipped_compute_derive_matches_breakdown_total()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, Inf, "n28", "small", "fp16", "ddr", "air");
        var b = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        Assert.True(spec.Valid);
        Assert.Equal(spec.Compute, b.Total);
        Assert.Equal(b.AfterNode + b.FromTarget + b.FromDie + b.FromFormat + b.FromMemory + b.FromCooling, b.Total);
    }

    [Fact]
    public void Invalid_options_yield_zero_breakdown()
    {
        GameConfig c = TestData.Config();
        var b = ChipQueries.ComputeDeriveParts(c, "missing", "n28", "small", "fp16", "ddr", "air");
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void Inference_target_boosts_compute_over_training_at_same_die()
    {
        GameConfig c = TestData.Config();
        var inf = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        var trn = ChipQueries.ComputeDeriveParts(c, Train, "n28", "small", "fp16", "ddr", "air");
        Assert.True(inf.Total > trn.Total);
        Assert.True(inf.FromTarget > trn.FromTarget);
    }

    [Fact]
    public void Chiplet_die_part_positive_versus_medium()
    {
        GameConfig c = TestData.Config();
        var med = ChipQueries.ComputeDeriveParts(c, Train, "n28", "medium", "fp16", "ddr", "air");
        var chiplet = ChipQueries.ComputeDeriveParts(c, Train, "n28", "chiplet-mcm", "fp16", "ddr", "air");
        Assert.True(chiplet.FromDie > med.FromDie);
        Assert.True(chiplet.Total > med.Total);
    }

    [Fact]
    public void Optical_cooling_part_changes_cooling_delta()
    {
        GameConfig c = TestData.Config();
        var air = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "medium", "fp16", "ddr", "air");
        var optical = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "medium", "fp16", "ddr", "optical");
        Assert.NotEqual(air.FromCooling, optical.FromCooling);
        Assert.True(optical.Total > air.Total);
    }

    [Fact]
    public void Matches_derive_on_exotic_n3_node()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, Train, "n3", "medium", "fp16", "ddr", "air");
        var b = ChipQueries.ComputeDeriveParts(c, Train, "n3", "medium", "fp16", "ddr", "air");
        Assert.Equal(spec.Compute, b.Total);
    }

    [Fact]
    public void After_node_is_node_base_compute()
    {
        GameConfig c = TestData.Config();
        ChipNodeConfig node = c.Chips.Node("n28");
        var b = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        Assert.Equal(node.ComputePerChipWeek, b.AfterNode);
    }

    [Fact]
    public void Hbm_memory_delta_positive_on_training_stack()
    {
        GameConfig c = TestData.Config();
        var ddr = ChipQueries.ComputeDeriveParts(c, Train, "n5", "medium", "fp8", "ddr", "air");
        var hbm = ChipQueries.ComputeDeriveParts(c, Train, "n5", "medium", "fp8", "hbm", "air");
        Assert.True(hbm.FromMemory > ddr.FromMemory);
    }

    [Fact]
    public void Format_fp8_beats_fp16_on_same_stack()
    {
        GameConfig c = TestData.Config();
        var fp16 = ChipQueries.ComputeDeriveParts(c, Train, "n5", "medium", "fp16", "hbm-stacked", "liquid");
        var fp8 = ChipQueries.ComputeDeriveParts(c, Train, "n5", "medium", "fp8", "hbm-stacked", "liquid");
        Assert.True(fp8.Total > fp16.Total);
    }

    [Fact]
    public void Spec_on_design_matches_breakdown()
    {
        var g = ChipHardeningSupport.Lab(2013, rivals: false);
        ChipDesign injected = ChipHardeningSupport.InjectReady(g, g.Me, Inf, "n28", "small");
        g.Me.ChipDesigns.Clear();
        g.Me.ChipDesigns.Add(injected);
        ChipDesign d = injected;
        ChipSpec spec = ChipQueries.Spec(g.C, d);
        var b = ChipQueries.ComputeDeriveParts(g.C, d.Target, d.NodeId, d.DieId, d.FormatId, d.MemoryId, d.CoolingId);
        Assert.Equal(spec.Compute, b.Total);
    }

    [Fact]
    public void Small_die_lowers_die_delta_versus_medium()
    {
        GameConfig c = TestData.Config();
        var small = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "small", "fp16", "ddr", "air");
        var med = ChipQueries.ComputeDeriveParts(c, Inf, "n28", "medium", "fp16", "ddr", "air");
        Assert.True(med.FromDie > small.FromDie);
    }

    [Fact]
    public void Liquid_cooling_improves_total_compute()
    {
        GameConfig c = TestData.Config();
        long air = ChipQueries.ComputeDeriveParts(c, Train, "n3", "medium", "fp16", "ddr", "air").Total;
        long liquid = ChipQueries.ComputeDeriveParts(c, Train, "n3", "medium", "fp16", "ddr", "liquid").Total;
        Assert.True(liquid > air);
    }

    [Fact]
    public void Deterministic_across_calls()
    {
        GameConfig c = TestData.Config();
        var a = ChipQueries.ComputeDeriveParts(c, Inf, "n7", "small", "fp16", "ddr", "air");
        var b = ChipQueries.ComputeDeriveParts(c, Inf, "n7", "small", "fp16", "ddr", "air");
        Assert.Equal(a.Total, b.Total);
        Assert.Equal(a.FromDie, b.FromDie);
    }
}