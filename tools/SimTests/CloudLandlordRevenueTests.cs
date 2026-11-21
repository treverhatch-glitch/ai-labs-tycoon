using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q71: cloud chip landlord revenue query matches weekly royalty payout.</summary>
public class CloudLandlordRevenueTests
{
    [Fact]
    public void Shipped_cloud_landlord_share_validates()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Chips.CloudLandlordSharePermille, 1, 1000);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Main_path_royalty_matches_renter_cloud_rent()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 20);
        CloudChipLandlordSystem.RevenueBreakdown b = CloudChipLandlordSystem.RevenueParts(g.C, g.State, g.Me);
        Assert.True(b.GrossCloudRent > 0);
        Assert.Equal(MathI.MulPermille(b.GrossCloudRent, g.C.Chips.CloudLandlordSharePermille), b.Royalty);
        Assert.Equal(b.Royalty, CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void Zero_share_zero_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 0);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        Assert.Equal(0, CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void Max_share_royalty_equals_gross_cloud_rent()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 1000);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 6);
        CloudChipLandlordSystem.RevenueBreakdown b = CloudChipLandlordSystem.RevenueParts(g.C, g.State, g.Me);
        Assert.Equal(b.GrossCloudRent, b.Royalty);
    }

    [Fact]
    public void Bankrupt_landlord_zero_revenue()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        g.Me.Bankrupt = true;
        Assert.Equal(0, CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void Bankrupt_renter_zero_landlord_revenue()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        renter.Bankrupt = true;
        Assert.Equal(0, CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void Save_load_preserves_revenue_query()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 12);
        long rev = CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(rev, CloudChipLandlordSystem.WeeklyLandlordRevenue(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Landlord_ledger_matches_query_before_run_week()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 8);
        long rev = CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before + rev, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Gpu_rent_parts_cloud_line_matches_gross_rent()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        long renterCloud = LabQueries.GpuRentParts(g.C, g.State, renter).Cloud;
        CloudChipLandlordSystem.RevenueBreakdown b = CloudChipLandlordSystem.RevenueParts(g.C, g.State, g.Me);
        Assert.Equal(renterCloud, b.GrossCloudRent);
    }

    [Fact]
    public void Chip_shortage_scales_gross_and_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 12);
        g.State.ChipShortageUntilTick = g.Tick + 40;
        g.State.ChipShortageStrengthPermille = 1250;
        long rev = CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before + rev, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Two_renters_aggregate_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renterA = CloudListingHelper.RentOnlyBuyer(g);
        LabState renterB = WorldSetup.AddRival(g.C, g.State, g.C.Rival("deepsneak"), EraSystem.AtTick(g.C, g.Tick), true);
        renterB.PlanStartTick = 0;
        Finance.Earn(renterB, LedgerLine.CapitalIn, 3_000_000_000);
        ChipSystem.AddChips(g.State, renterA, HardwareBatch.Cloud, g.Id, design.Id, 6);
        ChipSystem.AddChips(g.State, renterB, HardwareBatch.Cloud, g.Id, design.Id, 6);
        CloudChipLandlordSystem.RevenueBreakdown b = CloudChipLandlordSystem.RevenueParts(g.C, g.State, g.Me);
        long singleRent = ChipQueries.CloudBatchWeeklyRent(g.C, g.State, renterA.Fleet[0]);
        Assert.True(b.GrossCloudRent >= singleRent * 2 - 1);
    }

    [Fact]
    public void Self_hosted_cloud_chips_pay_no_royalty()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Cloud, g.Id, design.Id, 10);
        Assert.Equal(0, CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void Peer_and_cloud_revenue_queries_both_positive_same_week()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState chipRenter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, chipRenter, HardwareBatch.Cloud, g.Id, design.Id, 5);
        LabState peerLandlord = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), false);
        for (int i = 0; i < 6; i++)
            peerLandlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Me.Cash = 600_000;
        g.Do(Command.RentPeerCompute(g.Id, peerLandlord.Id, 2));
        Assert.True(CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me) > 0);
        Assert.True(PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, peerLandlord) > 0);
    }
}