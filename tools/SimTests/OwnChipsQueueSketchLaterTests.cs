using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT 4b chips + queue sketch (Q25275–Q25280).</summary>
public class OwnChipsQueueSketchLaterTests
{
    [Fact]
    public void Q25275_start_chip_design_creates_design_row()
    {
        Game g = ChipTests.Lab(2023);
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        Assert.Single(g.Me.ChipDesigns);
        Assert.Equal(ChipDesignStatus.Designing, g.Me.ChipDesigns[0].Status);
    }

    [Fact]
    public void Q25276_compute_split_counts_general_and_inference()
    {
        Game g = ChipTests.Lab(2023, engineers: 4);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.True(split.General >= 0);
        Assert.True(split.Inference >= 0);
    }

    [Fact]
    public void Q25277_queue_then_month_allocates_vendor_standing()
    {
        var g = Game.Solo(tweak: c => { c.GpuTierUnlock.Enabled = 0; c.ReservedComputeGate.Enabled = 0; });
        g.Build("server-closet", 2, 2);
        g.Me.Cash = 80_000_000;
        VendorStanding st = g.State.Vendor.Standing(g.Me.Id);
        st.BoughtThisMonth = st.AllocationThisMonth;
        g.Do(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq));
        Assert.Equal(1, st.QueuedGpus);
    }

    [Fact]
    public void Q25278_listing_fee_applies_when_setting_chip_listing()
    {
        Game g = ChipTests.Lab(2023);
        ChipDesign d = ChipTests.Ready(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 4));
        Assert.Equal(5000, g.Me.Listing(d.Id).PricePerChip);
        Assert.Equal(4, g.Me.Listing(d.Id).MaxPerMonth);
    }

    [Fact]
    public void Q25279_save_round_trip_keeps_chip_design_row()
    {
        Game g = ChipTests.Lab(2023);
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        int id = g.Me.ChipDesigns[0].Id;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Contains(loaded.Me.ChipDesigns, d => d.Id == id);
    }

    [Fact]
    public void Q25280_vendor_price_permille_matches_display_query()
    {
        var g = Game.Solo();
        int ppm = VendorSystem.PricePermille(g.C, g.State, g.Me);
        Assert.Equal(ppm, DisplayQueries.VendorQueueParts(g.C, g.State, g.Me).PricePermille);
    }
}