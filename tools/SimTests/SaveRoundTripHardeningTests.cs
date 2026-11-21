using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Save/load round-trips across a rich mid-game snapshot and every schema fixture.</summary>
public class SaveRoundTripHardeningTests
{
    static string Fixture(string name)
    {
        return File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", name));
    }

    static Game RichMidGame()
    {
        Game g = Game.New(17_033);
        g.WithStarterLab(researchers: 3, engineers: 2, safety: 1, sales: 1, gpus: 8);
        g.Autopilot(96);
        return g;
    }

    [Fact]
    public void Mid_game_save_load_matches_and_steps_the_same()
    {
        Game original = RichMidGame();
        string save = original.Sim.Save();
        string digestAtSave = original.Sim.Digest();

        Game resumed = new Game(Simulation.Load(original.C, save));
        Assert.Equal(save, resumed.Sim.Save());
        Assert.Equal(digestAtSave, resumed.Sim.Digest());
        Assert.Empty(SaveGame.Problems(original.C, resumed.State));

        const int moreWeeks = 20;
        original.Autopilot(moreWeeks);
        resumed.Autopilot(moreWeeks);
        AssertLedgerCash(resumed);

        Assert.Equal(original.Sim.Digest(), resumed.Sim.Digest());

        Game choppy = new Game(Simulation.Load(resumed.C, resumed.Sim.Save()));
        choppy.Autopilot(moreWeeks);
        original.Autopilot(moreWeeks);
        Assert.Equal(original.Sim.Digest(), choppy.Sim.Digest());
    }

    static void AssertLedgerCash(Game g)
    {
        if (!GameDate.IsMonthStart(g.C.Calendar, g.Tick)) return;
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Bankrupt) continue;
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }

    [Theory]
    [InlineData("schema1-save.json")]
    [InlineData("schema2-save.json")]
    [InlineData("schema3-save.json")]
    [InlineData("schema4-save.json")]
    [InlineData("schema5-bankrupt-save.json")]
    public void Every_schema_fixture_migrates_loads_and_round_trips(string file)
    {
        GameConfig c = TestData.Config();
        string json = Fixture(file);
        Simulation sim = Simulation.Load(c, json);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Empty(SaveGame.Problems(c, sim.State));

        string once = sim.Save();
        string twice = Simulation.Load(c, once).Save();
        Assert.Equal(once, twice);

        int tick0 = sim.Tick;
        sim.StepMany(12);
        Assert.True(sim.Tick > tick0);
        foreach (LabState lab in sim.State.Labs)
        {
            if (lab.Bankrupt) continue;
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }
}