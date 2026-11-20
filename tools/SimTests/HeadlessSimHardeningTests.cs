using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Ten-year headless soak with monthly invariants and cross-run determinism.</summary>
public class HeadlessSimHardeningTests
{
    const int WeeksPerYear = 48;

    [Fact]
    public void Ten_years_from_1995_satisfy_monthly_invariants_and_determinism()
    {
        SoakTwoRuns(42, null, 10);
    }

    [Fact]
    public void Ten_years_from_2009_quick_start_satisfy_monthly_invariants_and_determinism()
    {
        SoakTwoRuns(9009, "punchline", 10);
    }

    static void SoakTwoRuns(long seed, string startEraId, int years)
    {
        var digestsA = RunSoak(seed, startEraId, years);
        var digestsB = RunSoak(seed, startEraId, years);
        Assert.Equal(digestsA.Count, digestsB.Count);
        for (int i = 0; i < digestsA.Count; i++) Assert.Equal(digestsA[i], digestsB[i]);
    }

    static List<string> RunSoak(long seed, string startEraId, int years)
    {
        var options = new NewGameOptions { Seed = seed };
        if (!string.IsNullOrEmpty(startEraId)) options.StartEraId = startEraId;

        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        int totalWeeks = years * WeeksPerYear;
        var monthDigests = new List<string>();

        for (int week = 0; week < totalWeeks; week++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick))
            {
                SimPlayInvariants.AssertAll(g.C, g.State);
                monthDigests.Add(g.Sim.Digest());
            }
        }

        SimPlayInvariants.AssertAll(g.C, g.State);
        return monthDigests;
    }
}

/// <summary>Presentation-free checks the headless soak asserts each month.</summary>
static class SimPlayInvariants
{
    public static void AssertAll(GameConfig c, GameState state)
    {
        Assert.True(state.Tick >= 0);
        Assert.NotNull(state.Labs);
        Assert.NotEmpty(state.Labs);
        Assert.Equal(c.World.Sites.Count, state.Sites.Count);
        Assert.True(state.Events.Count <= c.EventLogMax);

        foreach (LabState lab in state.Labs)
        {
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
            Assert.True(lab.WeeksInDebt >= 0);
            Assert.InRange(lab.Reputation, 0, 1000);
            Assert.True(lab.Ledger.History.Count <= c.Finance.HistoryMonths);

            foreach (ModelState model in lab.Models)
            {
                Assert.True(model.RequiredCompute >= 0);
                Assert.True(model.ProgressCompute >= 0);
                Assert.True(model.Alignment >= 0);
                Assert.InRange(model.Alignment, 0, 1000);
                if (model.Status != ModelStatus.Training)
                {
                    Assert.True(model.Capability >= 0);
                    Assert.Equal(model.Capability, model.CapBase + model.CapTech + model.CapCarryover + model.CapLuck);
                }
                else
                {
                    Assert.True(model.ProgressCompute <= model.RequiredCompute || model.RequiredCompute == 0);
                }
            }

            if (lab.Rooms.Count > 0)
                Assert.Empty(SaveGame.Problems(c, state));
        }

        foreach (SectorState sector in state.Sectors)
            Assert.True(sector.PoolPerWeek >= 0);
    }
}