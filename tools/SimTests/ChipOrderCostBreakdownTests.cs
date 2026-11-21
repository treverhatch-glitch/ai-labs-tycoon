using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q127: ChipQueries.OrderCostParts explains fab order spend.</summary>
public class ChipOrderCostBreakdownTests
{
    [Fact]
    public void Single_chip_order_parts_match_order_cost()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "training", "n28", "small", "fp16", "ddr", "air");
        int y = spec.InitialYieldPermille;
        var b = ChipQueries.OrderCostParts(spec, y, 1);
        Assert.Equal(spec.WaferCost, b.WaferBase);
        Assert.Equal(ChipQueries.OrderCost(spec, y, 1), b.Total);
        Assert.Equal(b.WaferBase + b.FromYield + b.FromCount, b.Total);
    }

    [Fact]
    public void Multi_count_scales_via_from_count()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "inference", "n5", "medium", "fp16", "ddr", "air");
        int y = 700;
        long one = ChipQueries.OrderCost(spec, y, 1);
        var b = ChipQueries.OrderCostParts(spec, y, 5);
        Assert.Equal(one, b.WaferBase + b.FromYield);
        Assert.Equal(ChipQueries.OrderCost(spec, y, 5) - one, b.FromCount);
        Assert.Equal(ChipQueries.OrderCost(spec, y, 5), b.Total);
    }

    [Fact]
    public void Low_yield_raises_from_yield()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "training", "n3", "medium", "fp16", "hbm", "liquid");
        var high = ChipQueries.OrderCostParts(spec, 800, 2);
        var low = ChipQueries.OrderCostParts(spec, 400, 2);
        Assert.True(low.FromYield > high.FromYield);
        Assert.True(low.Total > high.Total);
    }

    [Fact]
    public void Yield_one_permille_falls_back_to_wafer_times_count()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "training", "n28", "small", "fp16", "ddr", "air");
        Assert.Equal(spec.WaferCost * 3, ChipQueries.OrderCost(spec, 0, 3));
        var b = ChipQueries.OrderCostParts(spec, 0, 3);
        Assert.Equal(spec.WaferCost * 3, b.Total);
    }

    [Fact]
    public void From_yield_equals_per_good_minus_base()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "inference", "n7", "medium", "fp8", "ddr", "air");
        int y = 650;
        long perGood = ChipQueries.CostPerGoodChip(spec, y);
        var b = ChipQueries.OrderCostParts(spec, y, 1);
        Assert.Equal(perGood - spec.WaferCost, b.FromYield);
    }

    [Fact]
    public void Chiplet_die_same_yield_math_as_medium()
    {
        GameConfig c = TestData.Config();
        ChipSpec chiplet = ChipQueries.Derive(c, "training", "n5", "chiplet-mcm", "fp16", "ddr", "air");
        ChipSpec med = ChipQueries.Derive(c, "training", "n5", "medium", "fp16", "ddr", "air");
        int y = 600;
        Assert.True(ChipQueries.OrderCostParts(chiplet, y, 2).Total > ChipQueries.OrderCostParts(med, y, 2).Total);
    }

    [Fact]
    public void Order_cost_parts_deterministic()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "training", "n28", "medium", "fp16", "ddr", "air");
        var a = ChipQueries.OrderCostParts(spec, 720, 8);
        var b = ChipQueries.OrderCostParts(spec, 720, 8);
        Assert.Equal(a.Total, b.Total);
        Assert.Equal(a.FromCount, b.FromCount);
    }

    [Fact]
    public void Max_yield_lowers_order_total_at_same_count()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "inference", "n28", "small", "fp16", "ddr", "air");
        long atInitial = ChipQueries.OrderCostParts(spec, spec.InitialYieldPermille, 1).Total;
        long atMax = ChipQueries.OrderCostParts(spec, spec.MaxYieldPermille, 1).Total;
        Assert.True(atMax < atInitial);
    }

    [Fact]
    public void Invalid_spec_zero_wafer_still_composes()
    {
        var spec = new ChipSpec { WaferCost = 0 };
        var b = ChipQueries.OrderCostParts(spec, 500, 2);
        Assert.Equal(0, b.WaferBase);
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void High_wafer_orders_match_order_cost_total()
    {
        var spec = new ChipSpec { Valid = true, WaferCost = 50_000 };
        var b = ChipQueries.OrderCostParts(spec, 900, 12);
        Assert.Equal(ChipQueries.OrderCost(spec, 900, 12), b.Total);
        Assert.Equal(b.WaferBase + b.FromYield + b.FromCount, b.Total);
    }

    [Fact]
    public void Order_parts_track_shortage_scaled_price_path()
    {
        var g = ChipHardeningSupport.Lab(2015, rivals: false);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, "training", "n28", "small");
        ChipSpec spec = ChipQueries.Spec(g.C, d);
        ChipShortageSystem.Start(g.C, g.State, 1300);
        long raw = ChipQueries.OrderCost(spec, spec.InitialYieldPermille, 2);
        long scaled = ChipShortageSystem.ScalePrice(g.State, raw);
        Assert.True(scaled > raw);
        var b = ChipQueries.OrderCostParts(spec, spec.InitialYieldPermille, 2);
        Assert.Equal(raw, b.Total);
    }

    [Fact]
    public void Hbm_stack_order_more_expensive_than_ddr()
    {
        GameConfig c = TestData.Config();
        ChipSpec ddr = ChipQueries.Derive(c, "training", "n5", "medium", "fp8", "ddr", "air");
        ChipSpec hbm = ChipQueries.Derive(c, "training", "n5", "medium", "fp8", "hbm-stacked", "air");
        Assert.True(ChipQueries.OrderCostParts(hbm, 700, 1).Total > ChipQueries.OrderCostParts(ddr, 700, 1).Total);
    }

    [Fact]
    public void Count_one_from_count_zero()
    {
        GameConfig c = TestData.Config();
        ChipSpec spec = ChipQueries.Derive(c, "training", "n28", "small", "fp16", "ddr", "air");
        var b = ChipQueries.OrderCostParts(spec, 750, 1);
        Assert.Equal(0, b.FromCount);
    }
}