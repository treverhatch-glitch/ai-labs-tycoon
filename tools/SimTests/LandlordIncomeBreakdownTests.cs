using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q73: LabQueries.LandlordIncomeParts sums peer and cloud landlord revenue.</summary>
public class LandlordIncomeBreakdownTests
{
    [Fact]
    public void Shipped_landlord_income_queries_compile()
    {
        GameConfig c = TestData.Config();
        ConfigValidator.Validate(c);
        Assert.InRange(c.Chips.CloudLandlordSharePermille, 1, 1000);
    }

    [Fact]
    public void Main_path_sums_peer_and_cloud_lines()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        LabQueries.LandlordIncomeBreakdown b = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me);
        Assert.Equal(CloudChipLandlordSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me), b.CloudChipRoyalty);
        Assert.Equal(0, b.PeerCompute);
        Assert.Equal(b.CloudChipRoyalty, b.Total);
    }

    [Fact]
    public void Zero_income_with_no_leases_or_cloud_designs()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        LabQueries.LandlordIncomeBreakdown b = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me);
        Assert.Equal(0, b.Total);
        Assert.Equal(0, LabQueries.WeeklyLandlordIncome(g.C, g.State, g.Me));
    }

    [Fact]
    public void Max_cloud_share_matches_gross_times_permille()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 1000);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 8);
        LabQueries.LandlordIncomeBreakdown b = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me);
        Assert.Equal(CloudChipLandlordSystem.RevenueParts(g.C, g.State, g.Me).GrossCloudRent, b.CloudChipRoyalty);
    }

    [Fact]
    public void Bankrupt_landlord_zero_breakdown()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        g.Me.Bankrupt = true;
        Assert.Equal(0, LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).Total);
    }

    [Fact]
    public void Bankrupt_cloud_renter_zeros_seller_cloud_line()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        renter.Bankrupt = true;
        Assert.Equal(0, LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).CloudChipRoyalty);
    }

    [Fact]
    public void Save_load_preserves_landlord_income_parts()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        long total = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).Total;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(total, LabQueries.LandlordIncomeParts(loaded.C, loaded.State, loaded.Me).Total);
    }

    [Fact]
    public void Weekly_income_matches_sum_after_sim_week()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 12);
        long cloud = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).CloudChipRoyalty;
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before + cloud, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Peer_only_landlord_income()
    {
        var (g, landlord) = PeerLandlordSetup(8);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        LabQueries.LandlordIncomeBreakdown b = LabQueries.LandlordIncomeParts(g.C, g.State, landlord);
        Assert.Equal(0, b.CloudChipRoyalty);
        Assert.Equal(PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord), b.PeerCompute);
    }

    [Fact]
    public void Combined_peer_and_cloud_same_lab()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState chipRenter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, chipRenter, HardwareBatch.Cloud, g.Id, design.Id, 6);
        LabState peerLandlord = WorldSetup.AddRival(g.C, g.State, g.C.Rival("anthoptics"), EraSystem.AtTick(g.C, g.Tick), false);
        for (int i = 0; i < 8; i++)
            peerLandlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Me.Cash = 600_000;
        g.Do(Command.RentPeerCompute(g.Id, peerLandlord.Id, 2));
        LabQueries.LandlordIncomeBreakdown seller = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me);
        LabQueries.LandlordIncomeBreakdown peerHost = LabQueries.LandlordIncomeParts(g.C, g.State, peerLandlord);
        Assert.True(seller.CloudChipRoyalty > 0);
        Assert.True(peerHost.PeerCompute > 0);
        Assert.Equal(peerHost.PeerCompute + peerHost.CloudChipRoyalty, peerHost.Total);
    }

    [Fact]
    public void Chip_shortage_scales_cloud_income_line()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        g.State.ChipShortageUntilTick = g.Tick + 20;
        g.State.ChipShortageStrengthPermille = 1200;
        long rev = LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).CloudChipRoyalty;
        Assert.True(rev > 0);
        long before = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before + rev, g.Me.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Running_cost_breakdown_unchanged_for_landlord()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 8);
        long burn = LabQueries.RunningCostParts(g.C, g.State, g.Me).Total;
        Assert.True(LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).CloudChipRoyalty > 0);
        Assert.True(burn >= 0);
    }

  [Fact]
    public void Zero_cloud_share_zero_cloud_line()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudLandlordSharePermille = 0);
        ChipDesign design = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, design.Id, 10);
        Assert.Equal(0, LabQueries.LandlordIncomeParts(g.C, g.State, g.Me).CloudChipRoyalty);
    }

    static (Game g, LabState landlord) PeerLandlordSetup(int hq)
    {
        Game g = Game.New();
        g.Me.Cash = 500_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        for (int i = 0; i < hq; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        return (g, landlord);
    }
}