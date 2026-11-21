using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Many short seeds must not throw and must keep ledger cash identity.</summary>
public class SeedLedgerPropertyHardeningTests
{
    const int Seeds = 24;
    const int Weeks = 96;

    [Fact]
    public void Many_seeds_autopilot_without_throw_and_ledger_stays_balanced()
    {
        for (long seed = 1; seed <= Seeds; seed++)
        {
            var g = new Game(Simulation.NewGame(TestData.Config(), seed));
            Exception caught = null;
            try
            {
                for (int w = 0; w < Weeks; w++)
                {
                    if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
                    g.Sim.Step();
                    if (GameDate.IsMonthStart(g.C.Calendar, g.Tick))
                    {
                        foreach (LabState lab in g.State.Labs)
                        {
                            if (lab.Bankrupt) continue;
                            Assert.False(double.IsNaN(lab.Cash));
                            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
                        }
                    }
                }
            }
            catch (Exception ex) { caught = ex; }
            Assert.Null(caught);
        }
    }
}