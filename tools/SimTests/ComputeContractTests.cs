using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ComputeContractTests
{
    [Fact]
    public void Reserved_compute_adds_general_compute_and_discounted_rent()
    {
        Game g = Game.Solo();
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long cash = g.Me.Cash;
        int count = 8;
        long deposit = ComputeContractSystem.Deposit(g.C, g.State, g.Me, count);
        g.Do(Command.SignReservedCompute(g.Id, count));
        Assert.Equal(cash - deposit, g.Me.Cash);
        Assert.Single(g.Me.ReservedContracts);
        Assert.Equal(count, ComputeContractSystem.ReservedGpus(g.Me));
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal((long)count * gen.ComputePerGpuWeek, split.General);
        Assert.True(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Reserved_rent_hits_the_ledger_each_week_the_contract_lasts()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Compute.ReservedContractWeeks = 3;
            c.PlayerStart.GrantPerWeek = 0;
        });
        g.Me.GrantPerWeek = 0;
        g.Rent(0);
        int count = 4;
        long gpuRentBeforeSign = g.Me.Ledger.Lifetime.GpuRent;
        g.Do(Command.SignReservedCompute(g.Id, count));
        long gpuRentAfterSign = g.Me.Ledger.Lifetime.GpuRent;
        Assert.True(gpuRentAfterSign < gpuRentBeforeSign);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long weekRent = MathI.MulPermille(count * gen.RentPerGpuWeek, g.C.Compute.ReservedRentPermille);
        for (int w = 0; w < 3; w++) g.Weeks(1);
        Assert.Empty(g.Me.ReservedContracts);
        long weeklyBilled = g.Me.Ledger.Lifetime.GpuRent - gpuRentAfterSign;
        Assert.Equal(-3 * weekRent, weeklyBilled);
    }

    [Fact]
    public void Signing_zero_gpus_is_refused()
    {
        var g = Game.Solo();
        g.Refused(Command.SignReservedCompute(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Signing_is_refused_when_cash_cannot_cover_the_deposit()
    {
        var g = Game.Solo();
        int count = 4;
        long deposit = ComputeContractSystem.Deposit(g.C, g.State, g.Me, count);
        g.Me.Cash = deposit - 1;
        g.Refused(Command.SignReservedCompute(g.Id, count), CommandError.NotEnoughCash);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_sign_or_queue_more_hardware()
    {
        var g = Game.Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.SignReservedCompute(g.Id, 2), CommandError.LabBankrupt);
        g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.LabBankrupt);
    }

    [Fact]
    public void Reserved_contracts_expire_and_stop_supplying_compute()
    {
        var g = Game.Solo(tweak: c => c.Compute.ReservedContractWeeks = 2);
        int count = 5;
        g.Do(Command.SignReservedCompute(g.Id, count));
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long withContract = LabQueries.Split(g.C, g.State, g.Me, false).General;
        Assert.Equal((long)count * gen.ComputePerGpuWeek, withContract);
        g.Weeks(2);
        Assert.Empty(g.Me.ReservedContracts);
        Assert.Equal(0, ComputeContractSystem.ReservedGpus(g.Me));
        Assert.Equal(0, LabQueries.Split(g.C, g.State, g.Me, false).General);
    }

    [Fact]
    public void Multiple_signings_stack_compute_and_rent()
    {
        var g = Game.Solo();
        g.Do(Command.SignReservedCompute(g.Id, 3));
        g.Do(Command.SignReservedCompute(g.Id, 2));
        Assert.Equal(5, ComputeContractSystem.ReservedGpus(g.Me));
        Assert.Equal(2, g.Me.ReservedContracts.Count);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.Equal(MathI.MulPermille(5L * gen.RentPerGpuWeek, g.C.Compute.ReservedRentPermille),
            LabQueries.WeeklyGpuRent(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_queue_buys_when_a_new_months_allocation_opens()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        g.Me.Cash += 50_000_000;
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.BoughtThisMonth = standing.AllocationThisMonth;
        Assert.Equal(0, VendorSystem.AllocationLeft(g.State, g.Me));
        int owned = LabQueries.OwnedUnits(g.Me);
        g.Do(Command.QueueGpuPurchase(g.Id, 2, HardwareBatch.Hq));
        Assert.Equal(2, standing.QueuedGpus);
        g.Weeks(4);
        Assert.Equal(0, standing.QueuedGpus);
        Assert.True(LabQueries.OwnedUnits(g.Me) >= owned + 2);
    }

    [Fact]
    public void Queued_units_do_not_buy_while_the_lab_is_bankrupt()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 3, 3);
        g.Me.Cash += 10_000_000;
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.BoughtThisMonth = standing.AllocationThisMonth;
        standing.QueuedGpus = 3;
        standing.QueueHoldingId = HardwareBatch.Hq;
        g.Me.Bankrupt = true;
        int owned = LabQueries.OwnedUnits(g.Me);
        VendorSystem.RunMonth(g.C, g.State);
        Assert.Equal(3, standing.QueuedGpus);
        Assert.Equal(owned, LabQueries.OwnedUnits(g.Me));
    }

    [Fact]
    public void Bankruptcy_clears_reserved_contracts()
    {
        var g = Game.Solo();
        g.Do(Command.SignReservedCompute(g.Id, 6));
        Assert.NotEmpty(g.Me.ReservedContracts);
        Finance.GoBankrupt(g.C, g.State, g.Me);
        Assert.Empty(g.Me.ReservedContracts);
        Assert.Equal(0, LabQueries.WeeklyGpuRent(g.C, g.State, g.Me));
    }

    [Fact]
    public void Reserved_rent_uses_the_gpu_generation_open_in_that_year()
    {
        var g = Game.Solo(tweak: c => c.Compute.ReservedContractWeeks = 600);
        g.Me.Cash += 100_000_000;
        g.Do(Command.SignReservedCompute(g.Id, 4));
        GpuGenerationConfig v1 = LabQueries.CurrentGeneration(g.C, g.Tick);
        long rent1995 = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        int target = GameDate.TickOfYear(g.C.Calendar, 2003);
        while (g.Tick < target) g.Sim.Step();
        Assert.NotEmpty(g.Me.ReservedContracts);
        GpuGenerationConfig v2 = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.NotEqual(v1.RentPerGpuWeek, v2.RentPerGpuWeek);
        long rent2003 = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        Assert.Equal(MathI.MulPermille(4L * v2.RentPerGpuWeek, g.C.Compute.ReservedRentPermille), rent2003);
        Assert.True(rent2003 > rent1995);
    }

    [Fact]
    public void Reserved_and_owned_gpus_both_count_in_the_compute_split()
    {
        var g = Game.Solo();
        g.Build("server-closet", 3, 3);
        g.Me.Cash += 5_000_000;
        g.Do(Command.SignReservedCompute(g.Id, 2));
        g.Do(Command.BuyGpus(g.Id, 3, HardwareBatch.Hq));
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long expected = (2 + 3L) * gen.ComputePerGpuWeek;
        Assert.Equal(expected, split.General);
    }

    [Fact]
    public void Contract_and_queue_state_survives_a_save_round_trip()
    {
        var g = Game.Solo();
        g.Do(Command.SignReservedCompute(g.Id, 4));
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.QueuedGpus = 2;
        standing.QueueHoldingId = HardwareBatch.Hq;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(4, ComputeContractSystem.ReservedGpus(loaded.Player));
        VendorStanding again = loaded.State.Vendor.Standing(loaded.Player.Id);
        Assert.Equal(2, again.QueuedGpus);
        Assert.Equal(HardwareBatch.Hq, again.QueueHoldingId);
    }

    [Fact]
    public void Schema_11_migration_adds_reserved_contracts_and_the_queue_fields()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Empty(sim.Player.ReservedContracts);
        VendorStanding standing = sim.State.Vendor.Standing(sim.Player.Id);
        Assert.NotNull(standing);
        Assert.Equal(0, standing.QueuedGpus);
        Assert.Equal(HardwareBatch.Hq, standing.QueueHoldingId);
    }

    [Fact]
    public void Queue_count_zero_clears_a_standing_queue()
    {
        var g = Game.Solo();
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.QueuedGpus = 5;
        standing.QueueHoldingId = HardwareBatch.Hq;
        g.Do(Command.QueueGpuPurchase(g.Id, 0, HardwareBatch.Hq));
        Assert.Equal(0, standing.QueuedGpus);
    }

    [Fact]
    public void A_second_holding_cannot_queue_while_another_is_pending()
    {
        var g = Game.Solo();
        g.Build("server-closet", 3, 3);
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        g.Do(Command.QueueGpuPurchase(g.Id, 2, HardwareBatch.Hq));
        g.Refused(Command.QueueGpuPurchase(g.Id, 1, 99), CommandError.InvalidArgument);
        Assert.Equal(2, standing.QueuedGpus);
    }

    [Fact]
    public void A_stalled_queue_clears_and_raises_an_event_when_cash_runs_out()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 3, 3);
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.BoughtThisMonth = standing.AllocationThisMonth;
        standing.QueuedGpus = 3;
        standing.QueueHoldingId = 99999;
        g.Me.Cash = 10_000_000;
        VendorSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, standing.QueuedGpus);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GpuQueueStalled);
    }

    [Fact]
    public void The_queue_buys_as_many_gpus_as_cash_allows_in_one_month()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.BoughtThisMonth = standing.AllocationThisMonth;
        int owned = LabQueries.OwnedUnits(g.Me);
        long price = VendorSystem.PricePerGpu(g.C, g.State, g.Me, LabQueries.CurrentGeneration(g.C, g.Tick));
        g.Me.Cash = price * 2;
        g.Do(Command.QueueGpuPurchase(g.Id, 5, HardwareBatch.Hq));
        VendorSystem.RunMonth(g.C, g.State);
        Assert.Equal(3, standing.QueuedGpus);
        Assert.Equal(owned + 2, LabQueries.OwnedUnits(g.Me));
    }

    [Fact]
    public void Loading_from_schema_10_adds_empty_reserved_and_queue_slots()
    {
        var g = Game.Solo();
        g.Do(Command.SignReservedCompute(g.Id, 2));
        string at10 = SaveSchemaPretend.At(g.C, 10, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, at10);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(loaded.Player.ReservedContracts);
        VendorStanding standing = loaded.State.Vendor.Standing(loaded.Player.Id);
        Assert.Equal(0, standing.QueuedGpus);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }
}