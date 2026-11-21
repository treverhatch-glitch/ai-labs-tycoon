using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Every schema fixture must migrate to current and keep each system's persisted slice playable.</summary>
public class SystemMigrationHardeningTests
{
    static GameConfig C => TestData.Config();

    public static IEnumerable<object[]> FixtureRows()
    {
        foreach (string file in SimHardeningSupport.SchemaFixtures) yield return new object[] { file };
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Rivals_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.StartsWith("schema2") || file.StartsWith("schema3") || file.StartsWith("schema4"))
            Assert.Contains(sim.State.Labs, l => l.Controller == LabController.Plan);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Chips_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.StartsWith("schema4"))
            Assert.NotEqual(0, sim.State.Rng.Chips.State);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void World_map_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        Assert.Equal(sim.Config.World.Sites.Count, sim.State.Sites.Count);
        if (file.StartsWith("schema3") || file.StartsWith("schema4"))
            Assert.All(sim.State.Labs, lab => Assert.Equal(sim.Config.World.Blocs.Count, lab.Blocs.Count));
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Hardware_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.StartsWith("schema1"))
            Assert.True(LabQueries.HqGpus(sim.Player) + sim.Player.RentedGpus > 0);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Alignment_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.StartsWith("schema3") || file.StartsWith("schema4"))
            Assert.Contains(sim.Player.Models, m => m.Status != ModelStatus.Training || m.ProgressCompute > 0);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Staff_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.StartsWith("schema3") || file.StartsWith("schema4"))
            Assert.True(sim.Player.Staff.Count > 0 || LabQueries.StaffCount(sim.Player, "researcher") > 0);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Reserved_compute_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        _ = sim.Player.ReservedContracts;
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Funding_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        foreach (LabState lab in sim.State.Labs.Where(l => !l.Bankrupt))
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Cloud_deals_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        _ = sim.State.Vendor;
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Endings_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        if (file.Contains("bankrupt"))
            Assert.Contains(sim.State.Endings, e => e.EndingId == "bankrupt");
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }

    [Theory]
    [MemberData(nameof(FixtureRows))]
    public void Shady_and_legal_fixture_migrates_and_round_trips(string file)
    {
        Simulation sim = SimHardeningSupport.LoadFixture(C, file);
        _ = sim.State.Rng.Shady;
        SimHardeningSupport.AssertMigratedFixtureSteps(C, file);
    }
}