using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipAmbushOrderTests
{
    [Fact]
    public void Order_refused_during_keynote_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + g.C.Vendor.AmbushWeeks;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 10), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Order_allowed_when_ambush_window_closed()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Ambush_tick_minus_one_allows_orders()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 1;
        g.State.Tick = g.Me.ChipAmbushUntilTick;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 3));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Not_ready_design_never_hits_ambush_order_gate()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 10;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Refused(Command.OrderChips(g.Id, 999, hall, 5), CommandError.UnknownId);
    }

    [Fact]
    public void Keynote_sets_ambush_then_blocks_chip_order()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        ChipSystem.OnKeynote(g.C, g.State);
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 10), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Listing_and_order_both_blocked_during_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 2), CommandError.KeynoteAmbush);
        g.Refused(Command.SetChipListing(g.Id, d.Id, 1000, 5), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Bankrupt_lab_hits_bankrupt_before_ambush_on_order()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.Bankrupt = true;
        g.Me.ChipAmbushUntilTick = g.State.Tick + 8;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 5), CommandError.LabBankrupt);
    }

    [Fact]
    public void Delist_still_allowed_while_orders_blocked()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 1000, 5));
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 5), CommandError.KeynoteAmbush);
        g.Do(Command.SetChipListing(g.Id, d.Id, 0, 0));
        Assert.Null(g.Me.Listing(d.Id));
    }

    [Fact]
    public void Tape_out_still_allowed_while_orders_blocked_during_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectTapingOut(g, g.Me);
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 5), CommandError.DesignNotReady);
        g.Weeks(1);
        Assert.Equal(ChipDesignStatus.Ready, g.Me.Design(d.Id).Status);
    }
}