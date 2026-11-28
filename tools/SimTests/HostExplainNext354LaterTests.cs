using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT item 6: hardware generations and fleet host slices (batch 354).</summary>
public class HostExplainNext354LaterTests
{
    [Fact]
    public void Hardware_fleet_parts_include_current_generation()
    {
        var g = Game.Solo();
        DisplayQueries.HardwareFleetBreakdown fleet = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        Assert.Equal(LabQueries.CurrentGeneration(g.C, g.Tick).Id, fleet.CurrentGenId);
        Assert.True(fleet.TotalUnits >= 0);
    }

    [Fact]
    public void Envydia_generations_are_seven_data_rows()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(7, c.Compute.GpuGenerations.Count);
        Assert.All(c.Compute.GpuGenerations, gen => Assert.False(string.IsNullOrEmpty(gen.Id)));
    }

    [Fact]
    public void Fleet_parts_split_own_rent_and_reserved()
    {
        var g = Game.Solo().WithStarterLab(gpus: 4);
        g.Do(Command.SignReservedCompute(g.Id, 2));
        DisplayQueries.HardwareFleetBreakdown fleet = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        Assert.Equal(2, fleet.ReservedGpus);
        Assert.Equal(fleet.OwnedHqGpus + fleet.OwnedOffsiteGpus + fleet.RentedGpus + fleet.ReservedGpus, fleet.TotalUnits);
    }
}