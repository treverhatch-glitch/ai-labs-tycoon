using System.IO;
using AiLabs.Rules.Sim;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The schema-5 bankrupt fixture still loads and plays without throwing even when cash and the ledger disagree at rest.</summary>
public class Schema5BankruptFixtureHardeningTests
{
    [Fact]
    public void The_bankrupt_schema_5_fixture_loads_and_steps_without_throwing()
    {
        var c = TestData.Config();
        string json = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema5-bankrupt-save.json"));
        Simulation sim = Simulation.Load(c, json);
        Assert.Empty(SaveGame.Problems(c, sim.State));
        for (int w = 0; w < 12; w++)
        {
            sim.Step();
            Assert.Empty(SaveGame.Problems(c, sim.State));
        }
    }
}