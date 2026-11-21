using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q59: landlord-side peer compute revenue query.</summary>
public class PeerLandlordRevenueTests
{
    static (Game g, LabState landlord) Pair(int hq = 10)
    {
        Game g = Game.New();
        g.Me.Cash = 600_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        landlord.Cash = 0;
        for (int i = 0; i < hq; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        return (g, landlord);
    }

    [Fact]
    public void No_leases_zero_landlord_revenue()
    {
        var (g, landlord) = Pair();
        Assert.Equal(0, PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void One_lease_matches_weekly_rent()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        Assert.Equal(PeerComputeSystem.WeeklyRent(g.C, g.State, 3),
            PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void Two_landlords_split_renter_bills()
    {
        var (g, landlord) = Pair(8);
        LabState other = g.Rival("xay");
        other.Cash = 0;
        for (int i = 0; i < 8; i++)
            other.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        g.Do(Command.RentPeerCompute(g.Id, other.Id, 3));
        Assert.Equal(PeerComputeSystem.WeeklyRent(g.C, g.State, 2),
            PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
        Assert.Equal(PeerComputeSystem.WeeklyRent(g.C, g.State, 3),
            PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, other));
        Assert.Equal(PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me),
            PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord)
            + PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, other));
    }

    [Fact]
    public void Landlord_ledger_matches_run_week()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        long rev = PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord);
        long before = landlord.Ledger.Lifetime.Revenue;
        g.Weeks(1);
        Assert.Equal(before + rev, landlord.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Bankrupt_renter_pays_nothing_to_landlord()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        g.Me.Bankrupt = true;
        Assert.Equal(0, PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void Bankrupt_landlord_reports_zero()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        landlord.Bankrupt = true;
        Assert.Equal(0, PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void Matches_renter_bill_sum()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 5));
        Assert.Equal(PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me),
            PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void Self_lease_refused_at_command()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 6);
        g.Refused(Command.RentPeerCompute(g.Id, g.Id, 2), CommandError.InvalidArgument);
        Assert.Equal(0, PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, g.Me));
    }

    [Fact]
    public void End_lease_drops_revenue_to_zero()
    {
        var (g, landlord) = Pair();
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        g.Do(Command.EndPeerCompute(g.Id, landlord.Id, 2));
        Assert.Equal(0, PeerComputeSystem.WeeklyLandlordRevenue(g.C, g.State, landlord));
    }

    [Fact]
    public void Save_load_preserves_renter_bill_used_for_landlord_revenue()
    {
        Game g = Game.Solo();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 99, GpuCount = 3 });
        long bill = PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(bill, PeerComputeSystem.WeeklyRentBill(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Zero_gpus_lease_refused()
    {
        var (g, landlord) = Pair();
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 0), CommandError.InvalidArgument);
    }
}