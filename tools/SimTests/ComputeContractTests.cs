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
        long rent = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        Assert.Equal(MathI.MulPermille(gen.RentPerGpuWeek * count, g.C.Compute.ReservedRentPermille), rent);
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
    public void Schema_11_migration_adds_reserved_contracts_and_the_queue_fields()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Empty(sim.Player.ReservedContracts);
        VendorStanding standing = sim.State.Vendor.Standing(sim.Player.Id);
        Assert.NotNull(standing);
        Assert.Equal(0, standing.QueuedGpus);
        Assert.Equal(0, standing.QueueHoldingId);
    }
}