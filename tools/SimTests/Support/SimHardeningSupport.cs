using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using Xunit;

namespace AiLabs.SimTests.Support;

/// <summary>Shared save/load/step helpers for sim hardening tests.</summary>
public static class SimHardeningSupport
{
    public static readonly string[] SchemaFixtures =
    {
        "schema1-save.json",
        "schema2-save.json",
        "schema3-save.json",
        "schema4-save.json",
        "schema5-bankrupt-save.json",
    };

    public static string FixtureJson(string file)
    {
        return File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", file));
    }

    public static Simulation LoadFixture(GameConfig c, string file) => Simulation.Load(c, FixtureJson(file));

    /// <summary>Save at a snapshot, resume, continue autopilot identically, then choppy reload once more.</summary>
    public static void AssertSaveLoadContinueIdentically(Game original, int continueWeeks = 16)
    {
        string save = original.Sim.Save();
        string digestAtSave = original.Sim.Digest();
        var resumed = new Game(Simulation.Load(original.C, save));
        Assert.Equal(save, resumed.Sim.Save());
        Assert.Equal(digestAtSave, resumed.Sim.Digest());
        Assert.Empty(SaveGame.Problems(original.C, resumed.State));

        original.Autopilot(continueWeeks);
        resumed.Autopilot(continueWeeks);
        Assert.Equal(original.Sim.Digest(), resumed.Sim.Digest());

        var choppy = new Game(Simulation.Load(resumed.C, resumed.Sim.Save()));
        choppy.Autopilot(continueWeeks);
        original.Autopilot(continueWeeks);
        Assert.Equal(original.Sim.Digest(), choppy.Sim.Digest());
    }

    /// <summary>Same as <see cref="AssertSaveLoadContinueIdentically"/> but after spectate (no player autopilot).</summary>
    public static void AssertSaveLoadContinueIdenticallySpectating(Game original, int continueWeeks = 16)
    {
        string save = original.Sim.Save();
        string digestAtSave = original.Sim.Digest();
        var resumed = new Game(Simulation.Load(original.C, save));
        Assert.Equal(save, resumed.Sim.Save());
        Assert.Equal(digestAtSave, resumed.Sim.Digest());
        Assert.Empty(SaveGame.Problems(original.C, resumed.State));

        original.Weeks(continueWeeks);
        resumed.Weeks(continueWeeks);
        Assert.Equal(original.Sim.Digest(), resumed.Sim.Digest());

        var choppy = new Game(Simulation.Load(resumed.C, resumed.Sim.Save()));
        choppy.Weeks(continueWeeks);
        original.Weeks(continueWeeks);
        Assert.Equal(original.Sim.Digest(), choppy.Sim.Digest());
    }

    public static void AssertMigratedFixtureSteps(GameConfig c, string file, int stepWeeks = 12)
    {
        Simulation sim = LoadFixture(c, file);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Empty(SaveGame.Problems(c, sim.State));
        string once = sim.Save();
        string twice = Simulation.Load(c, once).Save();
        Assert.Equal(once, twice);
        int tick0 = sim.Tick;
        var g = new Game(sim);
        g.Autopilot(stepWeeks);
        Assert.True(g.Tick > tick0);
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Bankrupt) continue;
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }
}