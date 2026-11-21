using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q200: rival plan economies stay solvent across seeds and mild early shocks.</summary>
public class RivalEconomyRobustnessLaterTests
{
    // Q200 retrenchment tuning in progress — keep file local until forty-seed soak is green.
    const string WipSkip = "Q200 WIP: retrenchment tuning";
    const int Years = 40;

    static void StepYears(Game g, int years)
    {
        GameConfig c = g.C;
        for (int w = 0; w < years * c.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, c.Autopilot);
            g.Sim.Step();
        }
    }

    static long OpenAyeCash(Game g)
    {
        foreach (LabState lab in g.State.Labs)
            if (lab.RivalId == "openaye") return lab.Cash;
        return 0;
    }

    [Fact(Skip = WipSkip)]
    public void Q200_forty_seeds_forty_years_no_rival_bankruptcies()
    {
        var bankrupts = new List<string>();
        for (long seed = 1; seed <= 40; seed++)
        {
            var g = new Game(Simulation.NewGame(TestData.Config(), seed));
            StepYears(g, Years);
            foreach (LabState lab in g.State.Labs)
            {
                if (lab.Controller != LabController.Plan) continue;
                if (lab.Bankrupt) bankrupts.Add("seed " + seed + ": " + lab.Name);
            }
        }
        Assert.Empty(bankrupts);
    }

    [Fact(Skip = WipSkip)]
    public void Q200_openaye_cash_spread_not_bimodal_across_seeds()
    {
        var cashes = new List<long>();
        for (long seed = 1; seed <= 40; seed++)
        {
            var g = new Game(Simulation.NewGame(TestData.Config(), seed));
            StepYears(g, Years);
            long cash = OpenAyeCash(g);
            Assert.True(cash > 0, "seed " + seed + ": OpenAye bankrupt or missing");
            cashes.Add(cash);
        }
        cashes.Sort();
        long p10 = cashes[3];
        long p90 = cashes[35];
        Assert.True(p90 < p10 * 500, "OpenAye cash spread too wide (p10=" + p10 + " p90=" + p90 + ")");
        Assert.True(p90 > 50_000, "OpenAye should not stall at pocket change on every seed");
    }

    [Fact(Skip = WipSkip)]
    public void Q200_retrenchment_log_appears_before_debt_on_tight_seed()
    {
        var g = Game.New(seed: 19);
        bool saw = false;
        for (int i = 0; i < 18 * g.C.Calendar.WeeksPerYear && !saw; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            foreach (LabState lab in g.State.Labs)
            {
                if (lab.Controller != LabController.Plan || lab.WeeksInDebt > 0) continue;
                if (lab.Log.Any(e => e.Action == DecisionAction.Tightened && e.Reasons.Any(r => r.Code == ReasonCode.RunwayWeeks)))
                {
                    saw = true;
                    break;
                }
            }
        }
        Assert.True(saw, "expected runway-driven tightening on seed 19 before debt");
    }
}