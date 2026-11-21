using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q72: Fleet.StaleGpuBatch query matches planner sell-oldest refresh.</summary>
public class PlannerStaleGpuTests
{
    static Game At(int year, long cash = 2_000_000_000, System.Action<GameConfig> tweak = null) =>
        ExpansionTestSupport.SoloAtYear(year, cash, tweak).WithStarterLab(gpus: 0);

    static void AddGpuBatch(Game g, string genId, int count = 4)
    {
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, genId, count);
    }

    [Fact]
    public void Shipped_replace_tier_gap_validates()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Rivalry.ReplaceTierGap >= 1);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Main_path_old_gen_returns_stale_batch()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 6);
        HardwareBatch stale = Fleet.StaleGpuBatch(g.C, g.State, g.Me);
        Assert.NotNull(stale);
        Assert.Equal("voodude", stale.GpuGenId);
    }

    [Fact]
    public void Zero_gap_too_small_returns_null()
    {
        Game g = At(2028);
        AddGpuBatch(g, "hoppa", 4);
        Assert.Null(Fleet.StaleGpuBatch(g.C, g.State, g.Me));
    }

    [Fact]
    public void Max_gap_picks_lowest_tier_among_stale()
    {
        Game g = At(2028);
        AddGpuBatch(g, "geforks", 3);
        AddGpuBatch(g, "voodude", 3);
        HardwareBatch stale = Fleet.StaleGpuBatch(g.C, g.State, g.Me);
        Assert.Equal("voodude", stale.GpuGenId);
    }

    [Fact]
    public void Bankrupt_lab_query_still_finds_stale_batch()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 4);
        g.Me.Bankrupt = true;
        Assert.NotNull(Fleet.StaleGpuBatch(g.C, g.State, g.Me));
    }

    [Fact]
    public void Save_load_preserves_stale_batch_pick()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 5);
        HardwareBatch stale = Fleet.StaleGpuBatch(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        HardwareBatch again = Fleet.StaleGpuBatch(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(stale.GpuGenId, again.GpuGenId);
        Assert.Equal(stale.Id, again.Id);
    }

    [Fact]
    public void Cloud_chip_batches_skipped()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 2);
        g.Me.Fleet.Add(new HardwareBatch
        {
            Id = g.State.TakeId(), HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", ChipDesignId = 99,
            DesignLabId = g.Id, Count = 4,
        });
        HardwareBatch stale = Fleet.StaleGpuBatch(g.C, g.State, g.Me);
        Assert.True(stale.ChipDesignId < 0);
    }

    [Fact]
    public void Sell_command_reduces_stale_batch_count()
    {
        Game g = At(2028, cash: 5_000_000);
        AddGpuBatch(g, "voodude", 8);
        HardwareBatch stale = Fleet.StaleGpuBatch(g.C, g.State, g.Me);
        int before = stale.Count;
        g.Do(Command.SellGpus(g.Id, 3, HardwareBatch.Hq));
        Assert.Equal(before - 3, Fleet.StaleGpuBatch(g.C, g.State, g.Me).Count);
    }

    [Fact]
    public void Resale_value_positive_for_stale_gen()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 4);
        GpuGenerationConfig old = g.C.Compute.Generation("voodude");
        Assert.True(LabQueries.ResalePerUnit(g.C, old, g.State.Tick) > 0);
    }

    [Fact]
    public void Tier_gap_config_blocks_borderline_stale()
    {
        Game g = At(2028, tweak: c => c.Rivalry.ReplaceTierGap = 3);
        AddGpuBatch(g, "hoppa", 4);
        Assert.Null(Fleet.StaleGpuBatch(g.C, g.State, g.Me));
    }

    [Fact]
    public void Current_generation_owned_no_stale()
    {
        Game g = At(2028);
        string current = LabQueries.CurrentGeneration(g.C, g.State.Tick).Id;
        AddGpuBatch(g, current, 6);
        Assert.Null(Fleet.StaleGpuBatch(g.C, g.State, g.Me));
    }

    [Fact]
    public void Oldest_first_lists_stale_before_newer_gen()
    {
        Game g = At(2028);
        AddGpuBatch(g, "voodude", 3);
        AddGpuBatch(g, "geforks", 3);
        var order = Fleet.OldestFirst(g.C, g.Me, HardwareBatch.Hq);
        Assert.Equal("voodude", order[0].GpuGenId);
        Assert.Equal(Fleet.StaleGpuBatch(g.C, g.State, g.Me).Id, order[0].Id);
    }

    [Fact]
    public void Running_cost_query_unchanged_when_stale_present()
    {
        Game g = At(2028);
        g.Rent(4);
        long before = LabQueries.WeeklyRunningCost(g.C, g.State, g.Me);
        AddGpuBatch(g, "voodude", 4);
        Assert.NotNull(Fleet.StaleGpuBatch(g.C, g.State, g.Me));
        Assert.True(LabQueries.WeeklyRunningCost(g.C, g.State, g.Me) > before);
    }
}