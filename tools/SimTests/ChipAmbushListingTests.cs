using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipAmbushListingTests
{
    [Fact]
    public void New_listing_refused_during_keynote_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + g.C.Vendor.AmbushWeeks;
        g.Refused(Command.SetChipListing(g.Id, d.Id, 5000, 10), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Price_change_refused_while_ambush_active()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 1000, 5));
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        g.Refused(Command.SetChipListing(g.Id, d.Id, 2000, 5), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Delisting_allowed_during_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 1000, 5));
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        g.Do(Command.SetChipListing(g.Id, d.Id, 0, 0));
        Assert.Null(g.Me.Listing(d.Id));
    }

    [Fact]
    public void After_ambush_window_listing_works()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick;
        g.Do(Command.SetChipListing(g.Id, d.Id, 3000, 8));
        Assert.Equal(3000, g.Me.Listing(d.Id).PricePerChip);
    }

    [Fact]
    public void Keynote_sets_ambush_then_blocks_fresh_listing()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        g.Refused(Command.SetChipListing(g.Id, d.Id, 1000, 5), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Bankrupt_lab_still_parses_listing_command_normally()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Me.Bankrupt = true;
        g.Refused(Command.SetChipListing(g.Id, d.Id, 1000, 5), CommandError.LabBankrupt);
    }

    [Fact]
    public void Not_ready_design_never_hits_ambush_gate()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 10;
        g.Refused(Command.SetChipListing(g.Id, 999, 1000, 5), CommandError.UnknownId);
    }

    [Fact]
    public void Ambush_tick_minus_one_allows_listing()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick;
        g.Do(Command.SetChipListing(g.Id, d.Id, 1500, 3));
        Assert.NotNull(g.Me.Listing(d.Id));
    }

    [Fact]
    public void Save_load_preserves_ambush_and_refusal()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 6;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Refused(Command.SetChipListing(loaded.Id, d.Id, 1000, 5), CommandError.KeynoteAmbush);
    }
}