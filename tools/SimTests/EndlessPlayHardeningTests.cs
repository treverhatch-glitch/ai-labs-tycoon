using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Fifty-year soak through endings continuation: bounds, invariants, and a final save/load.</summary>
public class EndlessPlayHardeningTests
{
    const long MaxReasonableCash = 500_000_000_000_000L;
    const long MaxCapability = 5_000_000L;

    [Fact]
    public void Short_endless_play_stays_bounded_with_final_save_round_trip()
    {
        RunSoak(years: 6, seeds: new[] { 7L, 19L });
    }

    [Fact]
    [Trait("Category", "BalanceScale")]
    public void Fifty_year_endless_play_under_full_balance_flag()
    {
        string env = Environment.GetEnvironmentVariable("BALANCE_SOAK_FULL");
        if (env != "1" && env != "true") return;
        RunSoak(years: 50, seeds: new[] { 1L, 43L, 97L });
    }

    static void RunSoak(int years, long[] seeds)
    {
        GameConfig c = TestData.Config();
        foreach (long seed in seeds)
        {
            var g = new Game(Simulation.NewGame(c, seed));
            int weeks = years * c.Calendar.WeeksPerYear;
            long minCash = long.MaxValue;
            long maxCash = long.MinValue;
            long maxCap = 0;
            int maxRep = 0;

            for (int w = 0; w < weeks; w++)
            {
                TryContinueEndings(g);
                if (g.State.PlayerLabId >= 0 && GameDate.IsMonthStart(c.Calendar, g.Tick))
                    g.Sim.RunPlan(g.State.PlayerLabId, c.Autopilot);
                g.Sim.Step();

                if (GameDate.IsMonthStart(c.Calendar, g.Tick))
                {
                    SimPlayInvariants.AssertAll(c, g.State);
                    Survey(g, ref minCash, ref maxCash, ref maxCap, ref maxRep);
                }
            }

            Assert.True(maxCash <= MaxReasonableCash, "seed " + seed + " cash max " + maxCash);
            Assert.True(maxCap <= MaxCapability, "seed " + seed + " capability max " + maxCap);
            Assert.InRange(maxRep, 0, 1000);

            string save = g.Sim.Save();
            var loaded = new Game(Simulation.Load(c, save));
            Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());
            Assert.Empty(SaveGame.Problems(c, loaded.State));
            loaded.Weeks(8);
            AssertLedger(c, loaded.State);
        }
    }

    static void TryContinueEndings(Game g)
    {
        if (g.State.PlayerLabId < 0) return;
        for (int i = 0; i < g.State.Endings.Count; i++)
        {
            EndingRecord e = g.State.Endings[i];
            if (e.Choice != "") continue;
            if (e.LabId != g.State.PlayerLabId) continue;
            string mode = e.EndingId == "bankrupt" ? "bailout" : "found-again";
            CommandResult r = g.Try(Command.ContinueAfterEnding(g.State.PlayerLabId, i, mode));
            if (r.Ok) break;
            r = g.Try(Command.ContinueAfterEnding(g.State.PlayerLabId, i, "spectate"));
            if (r.Ok) break;
        }
    }

    static void Survey(Game g, ref long minCash, ref long maxCash, ref long maxCap, ref int maxRep)
    {
        foreach (LabState lab in g.State.Labs)
        {
            minCash = Math.Min(minCash, lab.Cash);
            maxCash = Math.Max(maxCash, lab.Cash);
            maxRep = Math.Max(maxRep, lab.Reputation);
            foreach (ModelState model in lab.Models)
                if (model.Status != ModelStatus.Training) maxCap = Math.Max(maxCap, model.Capability);
        }
    }

    static void AssertLedger(GameConfig c, GameState state)
    {
        foreach (LabState lab in state.Labs)
        {
            if (lab.Bankrupt) continue;
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }
}