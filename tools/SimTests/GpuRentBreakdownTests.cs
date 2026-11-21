using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q56: LabQueries.GpuRentParts splits vendor, reserved, cloud, and peer rent.</summary>
public class GpuRentBreakdownTests
{
    [Fact]
    public void Solo_vendor_rent_only_when_renting_gpus()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(b.Total, LabQueries.WeeklyGpuRent(g.C, g.State, g.Me));
        Assert.True(b.VendorRented > 0);
        Assert.Equal(0, b.Reserved + b.Cloud + b.Peer);
    }

    [Fact]
    public void Reserved_contract_adds_to_breakdown()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Do(Command.SignReservedCompute(g.Id, 3));
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.True(b.Reserved > 0);
        Assert.Equal(b.VendorRented + b.Reserved, b.Total);
    }

    [Fact]
    public void Cloud_chips_add_cloud_line()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign d = CloudListingHelper.ReadyDesign(g);
        LabState buyer = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, buyer, HardwareBatch.Cloud, g.Id, d.Id, 10);
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, buyer);
        Assert.True(b.Cloud > 0);
        Assert.Equal(b.Cloud, ChipQueries.WeeklyCloudChipRent(g.C, g.State, buyer));
        Assert.Equal(b.Total, LabQueries.WeeklyGpuRent(g.C, g.State, buyer));
    }

    [Fact]
    public void Peer_lease_adds_peer_line()
    {
        var (g, landlord) = PeerGpuSetup(6);
        long beforeTotal = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.True(b.Peer > 0);
        Assert.Equal(b.Peer, PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me));
        Assert.Equal(beforeTotal, LabQueries.WeeklyGpuRent(g.C, g.State, g.Me));
    }

    [Fact]
    public void All_four_lines_stack()
    {
        Game g = Game.New();
        g.Me.Cash = 2_000_000;
        g.WithStarterLab(gpus: 2);
        g.Rent(2);
        g.Do(Command.SignReservedCompute(g.Id, 1));
        ChipDesign d = CloudListingHelper.ReadyDesign(g);
        ChipSystem.AddChips(g.State, g.Me, HardwareBatch.Cloud, g.Id, d.Id, 5);
        LabState landlord = g.Rival("openaye");
        for (int i = 0; i < 8; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 1));
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.True(b.VendorRented > 0 && b.Reserved > 0 && b.Cloud > 0 && b.Peer > 0);
        Assert.Equal(b.VendorRented + b.Reserved + b.Cloud, b.Total);
    }

    [Fact]
    public void Bankrupt_lab_breakdown_is_all_zero()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        Finance.GoBankrupt(g.C, g.State, g.Me);
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void Zero_cloud_rent_permille_leaves_cloud_line_at_zero()
    {
        Game g = CloudListingHelper.SellerBuyerPair(tweak: c => c.Chips.CloudRentPermille = 0);
        ChipDesign d = CloudListingHelper.ReadyDesign(g);
        LabState buyer = CloudListingHelper.RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, buyer, HardwareBatch.Cloud, g.Id, d.Id, 10);
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, buyer);
        Assert.Equal(0, b.Cloud);
    }

    [Fact]
    public void Chip_shortage_scales_vendor_and_cloud_in_breakdown()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 3);
        g.State.ChipShortageUntilTick = g.Tick + 20;
        g.State.ChipShortageStrengthPermille = 1200;
        long plain = ChipShortageSystem.ScalePrice(g.State,
            LabQueries.CurrentGeneration(g.C, g.Tick).RentPerGpuWeek * 3);
        LabQueries.GpuRentBreakdown b = LabQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(plain, b.VendorRented);
    }

    [Fact]
    public void Save_load_round_trip_breakdown_unchanged()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 2);
        g.Rent(1);
        long total = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(total, LabQueries.WeeklyGpuRent(loaded.C, loaded.State, loaded.Me));
        Assert.Equal(LabQueries.GpuRentParts(g.C, g.State, g.Me).Total, LabQueries.GpuRentParts(loaded.C, loaded.State, loaded.Me).Total);
    }

    [Fact]
    public void Cloud_landlord_royalty_crosses_finance_week_with_breakdown()
    {
        Game g = CloudListingHelper.SellerBuyerPair();
        ChipDesign d = CloudListingHelper.ReadyDesign(g);
        LabState renter = CloudListingHelper.RentOnlyBuyer(g);
        renter.RentedGpus = 0;
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, d.Id, 8);
        long cloud = LabQueries.GpuRentParts(g.C, g.State, renter).Cloud;
        long landlordBefore = g.Me.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(cloud, ChipQueries.WeeklyCloudChipRent(g.C, g.State, renter));
        Assert.True(g.Me.Ledger.Lifetime.Revenue - landlordBefore > 0);
        Assert.True(renter.Ledger.Lifetime.GpuRent < 0);
    }

    static (Game g, LabState landlord) PeerGpuSetup(int hq)
    {
        Game g = Game.New();
        g.Me.Cash = 500_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        landlord.Cash = 0;
        for (int i = 0; i < hq; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        return (g, landlord);
    }
}