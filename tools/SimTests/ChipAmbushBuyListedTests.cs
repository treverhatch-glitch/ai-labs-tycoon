using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipAmbushBuyListedTests
{
    static Game TwoLabsWithListing()
    {
        Game g = ChipHardeningSupport.Lab(2017, rivals: true);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 20));
        LabState buyer = g.State.Labs[1];
        Finance.Earn(buyer, LedgerLine.CapitalIn, 50_000_000L);
        int hall = g.Do(Command.BuyLand(buyer.Id, "us-oregon")).CreatedId;
        buyer.Holding(hall).HallKw = 500_000;
        return g;
    }

    [Fact]
    public void Buyer_refused_during_keynote_ambush()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        ChipDesign d = g.Me.ChipDesigns[0];
        int hall = buyer.Holdings[0].Id;
        buyer.ChipAmbushUntilTick = g.State.Tick + g.C.Vendor.AmbushWeeks;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, hall, 5), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Buy_works_when_ambush_window_closed()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        ChipDesign d = g.Me.ChipDesigns[0];
        int hall = buyer.Holdings[0].Id;
        buyer.ChipAmbushUntilTick = g.State.Tick;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, hall, 3));
        Assert.Single(buyer.ChipOrders);
    }

    [Fact]
    public void Seller_ambush_does_not_block_buyer()
    {
        Game g = TwoLabsWithListing();
        g.Me.ChipAmbushUntilTick = g.State.Tick + 8;
        LabState buyer = g.State.Labs[1];
        ChipDesign d = g.Me.ChipDesigns[0];
        int hall = buyer.Holdings[0].Id;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, hall, 2));
        Assert.Single(buyer.ChipOrders);
    }

    [Fact]
    public void Cloud_buy_respects_buyer_ambush()
    {
        Game g = ChipHardeningSupport.Lab(2017);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, d.Id, 4000, 10));
        LabState buyer = g.State.Labs[1];
        Finance.Earn(buyer, LedgerLine.CapitalIn, 20_000_000L);
        buyer.ChipAmbushUntilTick = g.State.Tick + 6;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 2), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Keynote_ambush_on_buyer_blocks_listed_purchase()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipHardeningSupport.InjectReady(g, buyer, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        ChipDesign d = g.Me.ChipDesigns[0];
        int hall = buyer.Holdings[0].Id;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, hall, 1), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Bankrupt_buyer_hits_bankrupt_before_ambush()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        buyer.Bankrupt = true;
        buyer.ChipAmbushUntilTick = g.State.Tick + 5;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, g.Me.ChipDesigns[0].Id, buyer.Holdings[0].Id, 2), CommandError.LabBankrupt);
    }

    [Fact]
    public void Invalid_listing_never_hits_ambush_gate()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        buyer.ChipAmbushUntilTick = g.State.Tick + 8;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, 999, buyer.Holdings[0].Id, 1), CommandError.UnknownId);
    }

    [Fact]
    public void Rival_feeling_still_warms_after_ambush_ends()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        ChipDesign d = g.Me.ChipDesigns[0];
        int hall = buyer.Holdings[0].Id;
        buyer.ChipAmbushUntilTick = g.State.Tick + 1;
        g.State.Tick = buyer.ChipAmbushUntilTick;
        int before = buyer.FeelingToPlayer;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, hall, 2));
        Assert.True(buyer.FeelingToPlayer > before);
    }

    [Fact]
    public void Buy_listed_survives_save_load_while_ambush_active()
    {
        Game g = TwoLabsWithListing();
        LabState buyer = g.State.Labs[1];
        buyer.ChipAmbushUntilTick = g.State.Tick + 10;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState lb = loaded.State.Labs[1];
        loaded.Refused(Command.BuyListedChips(lb.Id, loaded.Id, loaded.Me.ChipDesigns[0].Id, lb.Holdings[0].Id, 1), CommandError.KeynoteAmbush);
    }
}