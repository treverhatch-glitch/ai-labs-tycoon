using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q55: design lab earns a share of another lab's weekly cloud-chip rent.</summary>
public class CloudChipLandlordTests
{
    [Fact]
    public void Shipped_cloud_landlord_share_permille()
    {
        Assert.InRange(TestData.Config().Chips.CloudLandlordSharePermille, 1, 1000);
        ConfigValidator.Validate(TestData.Config());
    }

    [Fact]
    public void Landlord_earns_share_after_renter_pays_weekly_cloud_rent()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 20);
        long landlordBefore = g.Me.Ledger.Lifetime.Revenue;
        long renterGpuBefore = renter.Ledger.Lifetime.GpuRent;
        g.Weeks(1);
        long rent = ChipQueries.WeeklyCloudChipRent(g.C, g.State, renter);
        long expected = MathI.MulPermille(rent, g.C.Chips.CloudLandlordSharePermille);
        Assert.Equal(expected, g.Me.Ledger.Lifetime.Revenue - landlordBefore);
        Assert.True(renter.Ledger.Lifetime.GpuRent < renterGpuBefore);
    }

    [Fact]
    public void Zero_share_skips_landlord_payout()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 0);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Max_share_pays_whole_rent_slice_to_landlord()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 1000);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 8);
        long rent = ChipQueries.WeeklyCloudChipRent(g.C, g.State, renter);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(rent, g.Me.Ledger.Lifetime.Revenue - before);
    }

    [Fact]
    public void Bankrupt_landlord_gets_no_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        Finance.GoBankrupt(g.C, g.State, g.Me);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Bankrupt_renter_stops_paying_and_landlord_gets_nothing()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        Finance.GoBankrupt(g.C, g.State, renter);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Own_cloud_chips_do_not_pay_self_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Cloud, g.Id, design.Id, 12);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Chip_shortage_scales_landlord_and_renter_rent()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 15);
        g.State.ChipShortageUntilTick = g.Tick + 30;
        g.State.ChipShortageStrengthPermille = 1300;
        long rent = ChipQueries.WeeklyCloudChipRent(g.C, g.State, renter);
        long landlordBefore = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(MathI.MulPermille(rent, g.C.Chips.CloudLandlordSharePermille), g.Me.Ledger.Lifetime.Revenue - landlordBefore);
    }

    [Fact]
    public void Save_load_round_trip_cloud_landlord_royalty_still_runs()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState renter2 = loaded.State.Lab(renter.Id);
        long before = loaded.Me.Ledger.Lifetime.Revenue;
        loaded.Weeks(1);
        Assert.True(loaded.Me.Ledger.Lifetime.Revenue > before);
        Assert.True(ChipQueries.WeeklyCloudChipRent(loaded.C, loaded.State, renter2) > 0);
    }

    [Fact]
    public void Peer_compute_week_and_cloud_royalty_both_run_same_week()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState chipRenter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, chipRenter, HardwareBatch.Cloud, g.Id, design.Id, 6);
        LabState peerLandlord = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), false);
        for (int i = 0; i < 8; i++)
            peerLandlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Me.Cash = 800_000;
        g.Do(Command.RentPeerCompute(g.Id, peerLandlord.Id, 2));
        long sellerBefore = g.Me.Ledger.Lifetime.Revenue;
        long playerGpuBefore = g.Me.Ledger.Lifetime.GpuRent;
        g.Weeks(1);
        Assert.True(g.Me.Ledger.Lifetime.Revenue > sellerBefore);
        Assert.True(g.Me.Ledger.Lifetime.GpuRent < playerGpuBefore);
        Assert.True(ChipQueries.WeeklyCloudChipRent(g.C, g.State, chipRenter) > 0);
    }
}