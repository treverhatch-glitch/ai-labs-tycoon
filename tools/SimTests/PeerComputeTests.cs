using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>GPUs rented between labs (Q52).</summary>
public class PeerComputeTests
{
    static void StockHqGpus(Game g, LabState lab, int count)
    {
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        lab.Fleet.Add(new HardwareBatch
        {
            Id = g.State.NextId++,
            HoldingId = HardwareBatch.Hq,
            GpuGenId = gen.Id,
            Count = count,
        });
    }

    static (Game g, LabState landlord) Landlord(int hq = 12)
    {
        Game g = Game.New();
        LabState landlord = g.Rival("openaye");
        landlord.Cash = 0;
        StockHqGpus(g, landlord, hq);
        return (g, landlord);
    }

    [Fact]
    public void Shipped_config_has_peer_lease_permille() =>
        Assert.InRange(TestData.Config().Compute.PeerLeaseRentPermille, 1, 3000);

    [Fact]
    public void Rent_peer_compute_moves_compute_and_cash()
    {
        var (g, landlord) = Landlord();
        long renterCash = g.Me.Cash;
        int count = 4;
        long weekRent = PeerComputeSystem.WeeklyRent(g.C, g.State, count);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, count));
        Assert.Equal(count, PeerComputeSystem.LeasedInGpus(g.Me));
        Assert.Equal(count, PeerComputeSystem.LeasedOutFrom(g.State, landlord.Id));
        Assert.Equal(renterCash - weekRent, g.Me.Cash);
        Assert.Equal(weekRent, landlord.Cash);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.True(LabQueries.Split(g.C, g.State, g.Me, false).General >= count * gen.ComputePerGpuWeek);
        Assert.True(LabQueries.Split(g.C, g.State, landlord, false).General < 12L * gen.ComputePerGpuWeek);
    }

    [Fact]
    public void Refusals_self_unknown_landlord_zero_and_over_cap()
    {
        var (g, landlord) = Landlord(2);
        g.Refused(Command.RentPeerCompute(g.Id, g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.RentPeerCompute(g.Id, -1, 1), CommandError.InvalidArgument);
        g.Refused(Command.RentPeerCompute(g.Id, 999, 1), CommandError.UnknownLab);
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 99), CommandError.NotEnoughGpus);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        g.Refused(Command.EndPeerCompute(g.Id, landlord.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.EndPeerCompute(g.Id, landlord.Id, 3), CommandError.InvalidArgument);
    }

    [Fact]
    public void End_lease_partial_then_full()
    {
        var (g, landlord) = Landlord(6);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 4));
        g.Do(Command.EndPeerCompute(g.Id, landlord.Id, 2));
        Assert.Equal(2, PeerComputeSystem.LeasedInGpus(g.Me));
        g.Do(Command.EndPeerCompute(g.Id, landlord.Id, 2));
        Assert.Empty(g.Me.PeerComputeLeases);
    }

    [Fact]
    public void Max_lease_takes_all_free_hq_gpus()
    {
        var (g, landlord) = Landlord(5);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 5));
        Assert.Equal(0, PeerComputeSystem.HqGpusFree(g.State, landlord));
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 1), CommandError.NotEnoughGpus);
    }

    [Fact]
    public void Stacked_signs_on_same_landlord_add_gpus()
    {
        var (g, landlord) = Landlord(10);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        Assert.Equal(5, PeerComputeSystem.LeasedInGpus(g.Me));
        Assert.Single(g.Me.PeerComputeLeases);
    }

    [Fact]
    public void Two_landlords_independent_leases()
    {
        var (g, a) = Landlord(6);
        LabState b = g.Rival("anthoptics");
        StockHqGpus(g, b, 4);
        g.Do(Command.RentPeerCompute(g.Id, a.Id, 2));
        g.Do(Command.RentPeerCompute(g.Id, b.Id, 1));
        Assert.Equal(2, g.Me.PeerComputeLeases.Count);
        Assert.Equal(2, PeerComputeSystem.LeasedOutFrom(g.State, a.Id));
        Assert.Equal(1, PeerComputeSystem.LeasedOutFrom(g.State, b.Id));
    }

    [Fact]
    public void Weekly_billing_on_sign_plus_run_weeks()
    {
        var (g, landlord) = Landlord(8);
        g.Me.GrantPerWeek = 0;
        long weekRent = PeerComputeSystem.WeeklyRent(g.C, g.State, 3);
        long rentBefore = g.Me.Ledger.Lifetime.GpuRent;
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        g.Weeks(2);
        Assert.Equal(-3 * weekRent, g.Me.Ledger.Lifetime.GpuRent - rentBefore);
    }

    [Fact]
    public void Bankrupt_landlord_refuses_new_lease()
    {
        var (g, landlord) = Landlord(4);
        Finance.GoBankrupt(g.C, g.State, landlord);
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void Bankrupt_renter_refuses_lease()
    {
        var (g, landlord) = Landlord(4);
        Finance.GoBankrupt(g.C, g.State, g.Me);
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void Home_freeze_refuses_rent()
    {
        var (g, landlord) = Landlord(4);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.RentPeerCompute(g.Id, landlord.Id, 1), CommandError.PermitFrozen);
    }

    [Fact]
    public void Chip_shortage_scales_peer_rent_like_vendor_rent()
    {
        var (g, landlord) = Landlord(4);
        g.State.ChipShortageUntilTick = g.Tick + 20;
        g.State.ChipShortageStrengthPermille = 1200;
        long scaled = PeerComputeSystem.WeeklyRent(g.C, g.State, 2);
        int strength = g.State.ChipShortageStrengthPermille;
        int until = g.State.ChipShortageUntilTick;
        g.State.ChipShortageStrengthPermille = 0;
        g.State.ChipShortageUntilTick = -1;
        long plain = PeerComputeSystem.WeeklyRent(g.C, g.State, 2);
        g.State.ChipShortageStrengthPermille = strength;
        g.State.ChipShortageUntilTick = until;
        Assert.True(scaled > plain);
        long rentBefore = g.Me.Ledger.Lifetime.GpuRent;
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        Assert.Equal(scaled, rentBefore - g.Me.Ledger.Lifetime.GpuRent);
    }

    [Fact]
    public void Plan_lab_cannot_rent_from_peer()
    {
        var (g, rival) = Landlord(6);
        g.Refused(Command.RentPeerCompute(rival.Id, g.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Peer_lease_adds_to_training_compute_budget()
    {
        var (g, landlord) = Landlord(8);
        g.WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 4));
        long gen = LabQueries.WeeklyCompute(g.C, g.State, g.Me);
        g.Do(Command.StartTraining(g.Id, "mini", 200));
        ModelState run = g.Me.Models.Single(m => m.Status == ModelStatus.Training);
        Assert.True(run.RequiredCompute > 0);
        Assert.True(gen >= LabQueries.Split(g.C, g.State, g.Me, false).General);
    }

    [Fact]
    public void Reserved_vendor_contract_and_peer_lease_both_add_general()
    {
        Game g = Game.New();
        g.Me.Cash = 500_000;
        g.WithStarterLab(gpus: 0);
        g.Rent(0);
        g.Do(Command.SignReservedCompute(g.Id, 2));
        LabState landlord = g.Rival("openaye");
        StockHqGpus(g, landlord, 6);
        long generalBeforePeer = LabQueries.Split(g.C, g.State, g.Me, false).General;
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.Equal(generalBeforePeer + 3 * gen.ComputePerGpuWeek, LabQueries.Split(g.C, g.State, g.Me, false).General);
        Assert.Equal(2, ComputeContractSystem.ReservedGpus(g.Me));
    }

    [Fact]
    public void Save_load_round_trip_peer_leases_on_solo()
    {
        Game g = Game.Solo();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 99, GpuCount = 2 });
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(2, loaded.Me.PeerComputeLeases[0].GpuCount);
        Assert.Equal(27, loaded.State.Schema);
    }

    [Fact]
    public void Migrate_19_to_20_adds_peer_leases_array()
    {
        Game g = Game.Solo();
        string json = g.Sim.Save().Replace("\"schema\":25", "\"schema\":19");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(27, loaded.State.Schema);
        Assert.NotNull(loaded.Me.PeerComputeLeases);
        Assert.Empty(loaded.Me.PeerComputeLeases);
    }

    [Fact]
    public void Save_json_includes_peer_lease_fields()
    {
        var (g, landlord) = Landlord(10);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 3));
        string json = g.Sim.Save();
        Assert.Contains("\"peerComputeLeases\"", json);
        Assert.Contains("\"landlordLabId\":" + landlord.Id, json);
    }

    [Fact]
    public void Schema_23_on_new_game() => Assert.Equal(27, Game.Solo().State.Schema);

    [Fact]
    public void Landlord_ledger_revenue_gets_peer_rent()
    {
        var (g, landlord) = Landlord(6);
        long before = landlord.Ledger.Lifetime.Revenue;
        long weekRent = PeerComputeSystem.WeeklyRent(g.C, g.State, 2);
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 2));
        Assert.Equal(before + weekRent, landlord.Ledger.Lifetime.Revenue);
    }
}