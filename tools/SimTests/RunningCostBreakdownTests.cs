using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q58: LabQueries.RunningCostParts mirrors weekly burn.</summary>
public class RunningCostBreakdownTests
{
    [Fact]
    public void Parts_sum_to_weekly_running_cost()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        LabQueries.RunningCostBreakdown b = LabQueries.RunningCostParts(g.C, g.State, g.Me);
        Assert.Equal(b.Total, LabQueries.WeeklyRunningCost(g.C, g.State, g.Me));
    }

    [Fact]
    public void Salaries_positive_with_staff()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        Assert.True(LabQueries.RunningCostParts(g.C, g.State, g.Me).Salaries > 0);
    }

    [Fact]
    public void Gpu_rent_line_matches_weekly_gpu_rent()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 6);
        var b = LabQueries.RunningCostParts(g.C, g.State, g.Me);
        Assert.Equal(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me), b.GpuRent);
    }

    [Fact]
    public void Owned_gpus_add_upkeep_not_vendor_rent()
    {
        Game g = Game.Solo(1, c => c.PlayerStart.Cash = 5_000_000);
        g.WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 4));
        var b = LabQueries.RunningCostParts(g.C, g.State, g.Me);
        Assert.Equal(0, b.GpuRent);
        Assert.True(b.GpuUpkeep > 0);
    }

    [Fact]
    public void Site_hardware_adds_power_line()
    {
        Game g = Game.Solo(1, c => c.PlayerStart.Cash = 5_000_000);
        g.WithStarterLab(gpus: 8);
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, land, 100));
        Holding holding = g.Me.Holding(land);
        g.Until(() => holding.HallKw >= 100, 120, "hall");
        g.Do(Command.BuyGpus(g.Id, 4, land));
        Assert.True(LabQueries.RunningCostParts(g.C, g.State, g.Me).Power > 0);
    }

    [Fact]
    public void Rooms_upkeep_grows_with_tiles()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        long baseRooms = LabQueries.RunningCostParts(g.C, g.State, g.Me).Rooms;
        g.Build("research-lab", 6, 6);
        Assert.True(LabQueries.RunningCostParts(g.C, g.State, g.Me).Rooms > baseRooms);
    }

    [Fact]
    public void Bankrupt_lab_parts_still_query()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        g.Me.Bankrupt = true;
        Assert.True(LabQueries.RunningCostParts(g.C, g.State, g.Me).Total >= 0);
    }

    [Fact]
    public void Reserved_contract_included_in_gpu_rent_line()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Do(Command.SignReservedCompute(g.Id, 2));
        Assert.Equal(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me),
            LabQueries.RunningCostParts(g.C, g.State, g.Me).GpuRent);
    }

    [Fact]
    public void Peer_bill_not_in_running_cost_total()
    {
        var (g, landlord) = PeerSetup(8);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        long total = LabQueries.WeeklyRunningCost(g.C, g.State, g.Me);
        long peer = PeerComputeSystem.WeeklyRentBill(g.C, g.State, g.Me);
        Assert.True(peer > 0);
        Assert.Equal(LabQueries.RunningCostParts(g.C, g.State, g.Me).Total, total);
    }

    [Fact]
    public void Finance_week_gpu_spend_uses_running_gpu_rent_only()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        long rent = LabQueries.RunningCostParts(g.C, g.State, g.Me).GpuRent;
        long before = g.Me.Ledger.Lifetime.GpuRent;
        g.Weeks(1);
        Assert.Equal(before - rent, g.Me.Ledger.Lifetime.GpuRent);
    }

    [Fact]
    public void Save_load_preserves_breakdown_total()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 6);
        long total = LabQueries.RunningCostParts(g.C, g.State, g.Me).Total;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(total, LabQueries.RunningCostParts(loaded.C, loaded.State, loaded.Me).Total);
    }

    [Fact]
    public void Zero_config_site_has_no_power_when_hq_only()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        Assert.Equal(0, LabQueries.RunningCostParts(g.C, g.State, g.Me).Power);
    }

    static (Game g, LabState landlord) PeerSetup(int hq)
    {
        Game g = Game.New();
        g.Me.Cash = 400_000;
        g.WithStarterLab(gpus: 0);
        LabState landlord = g.Rival("openaye");
        for (int i = 0; i < hq; i++)
            landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = "voodude", Count = 1 });
        return (g, landlord);
    }
}