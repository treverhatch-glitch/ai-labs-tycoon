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

/// <summary>Q200–Q201: rival plan economies stay solvent across seeds, shocks, and distress/retrenchment edge cases.</summary>
public class RivalEconomyRobustnessLaterTests
{
    const int Years = 40;
    const int SeedCount = 40;

    static void StepYears(Game g, int years)
    {
        GameConfig c = g.C;
        for (int w = 0; w < years * c.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, c.Autopilot);
            g.Sim.Step();
        }
    }

    static void StepMonthsWithPlans(Game g, int months)
    {
        GameConfig c = g.C;
        int stepped = 0;
        while (stepped < months)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick))
            {
                g.Sim.RunPlan(g.Id, c.Autopilot);
                stepped++;
            }
            g.Sim.Step();
        }
    }

    static List<string> PlanBankruptciesAfterYears(Action<GameConfig> tweak, long seed, int years)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(tweak), seed));
        StepYears(g, years);
        var bankrupts = new List<string>();
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Controller != LabController.Plan) continue;
            if (lab.Bankrupt) bankrupts.Add("seed " + seed + ": " + lab.Name);
        }
        return bankrupts;
    }

    static void AssertFortyFortySolvent(Action<GameConfig> tweak, string label)
    {
        var bankrupts = new List<string>();
        for (long seed = 1; seed <= SeedCount; seed++)
            bankrupts.AddRange(PlanBankruptciesAfterYears(tweak, seed, Years).Select(b => label + " " + b));
        Assert.Empty(bankrupts);
    }

    static IEnumerable<LabState> PlanLabs(Game g)
    {
        foreach (LabState lab in g.State.Labs)
            if (lab.Controller == LabController.Plan) yield return lab;
    }

    static bool SawRunwayTightening(LabState lab)
    {
        return lab.Log.Any(e => e.Action == DecisionAction.Tightened && e.Reasons.Any(r => r.Code == ReasonCode.RunwayWeeks));
    }

    [Fact]
    public void Q200_forty_seeds_forty_years_no_rival_bankruptcies()
    {
        AssertFortyFortySolvent(null, "");
    }

    [Fact]
    public void Q200_retrenchment_log_appears_before_debt_on_tight_seed()
    {
        var g = Game.New(seed: 19);
        bool saw = false;
        for (int i = 0; i < 18 * g.C.Calendar.WeeksPerYear && !saw; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            foreach (LabState lab in PlanLabs(g))
            {
                if (lab.WeeksInDebt > 0) continue;
                if (SawRunwayTightening(lab)) { saw = true; break; }
            }
        }
        Assert.True(saw, "expected runway-driven tightening on seed 19 before debt");
    }

    [Fact]
    public void Q201_shipped_rival_distress_harness_is_default_off()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(0, c.Acquisitions.PlannerAcquireChancePermille);
        Assert.True(c.Acquisitions.CrashDistressPricePermille < MathI.Permille);
        Assert.True(c.ModelSales.CrashDistressOfferPermille < MathI.Permille);
        Assert.True(c.Chips.DistressListingPricePermille < MathI.Permille);
    }

    [Fact]
    public void Q201_forty_seeds_forty_years_default_harness_no_plan_bankruptcies()
    {
        AssertFortyFortySolvent(null, "default");
    }

    /// <summary>Q201: mild year-1 research permille shock must not bankrupt plan labs (stack2 bar; no sector flavours).</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    public void Q201_forty_seeds_forty_years_no_plan_bankruptcies_with_research_perturbation(int bumpPermille)
    {
        AssertFortyFortySolvent(c =>
        {
            if (bumpPermille > 0)
                c.Eras[0].ResearchPermille = (int)MathI.MulPermille(c.Eras[0].ResearchPermille, MathI.Permille + bumpPermille);
        }, "bump " + bumpPermille);
    }

    [Fact]
    public void Q201_max_distress_cost_scale_forty_seeds_no_plan_bankruptcies()
    {
        AssertFortyFortySolvent(c =>
        {
            c.Acquisitions.CrashDistressPricePermille = MathI.Permille;
            c.Acquisitions.DistressPricePermille = MathI.Permille;
            c.ModelSales.CrashDistressOfferPermille = MathI.Permille;
            c.Chips.DistressListingPricePermille = MathI.Permille;
        }, "max-distress");
    }

    [Fact]
    public void Q201_zero_cash_plan_rival_retrenches_on_month_plan_before_debt()
    {
        var g = Game.New(seed: 31);
        StepMonthsWithPlans(g, 12);
        LabState rival = g.Rival("openaye");
        rival.Cash = 0;
        rival.WeeksInDebt = 0;
        int gpusBefore = rival.RentedGpus;
        g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);
        Assert.Equal(0, rival.WeeksInDebt);
        Assert.False(rival.Bankrupt);
        bool reacted = SawRunwayTightening(rival)
            || rival.RentedGpus < gpusBefore
            || rival.Log.Any(e => e.Action == DecisionAction.SoldModel);
        Assert.True(reacted, "zero-cash month plan should trigger Q200 retrench or liquidity sale");
    }

    [Fact]
    public void Q201_plan_rival_on_bankruptcy_line_retrenches_before_debt()
    {
        var g = Game.New(seed: 19);
        StepMonthsWithPlans(g, 24);
        LabState rival = g.Rival("openaye");
        long weekly = LabQueries.WeeklyRunningCost(g.C, g.State, rival);
        Assert.True(weekly > 0);
        int reserveWeeks = g.C.Rival("openaye").Plan.ReserveWeeks;
        rival.Cash = weekly * Math.Max(1, reserveWeeks / 2);
        rival.WeeksInDebt = 0;
        int gpusBefore = rival.RentedGpus;
        g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);
        Assert.Equal(0, rival.WeeksInDebt);
        Assert.False(rival.Bankrupt);
        Assert.True(SawRunwayTightening(rival) || rival.RentedGpus < gpusBefore,
            "expected runway retrench (log or GPU trim) before debt");
    }

    [Fact]
    public void Q201_save_load_mid_retrenchment_then_forty_years_still_solvent()
    {
        var g = Game.New(seed: 19);
        bool mid = false;
        for (int i = 0; i < 18 * g.C.Calendar.WeeksPerYear && !mid; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            mid = PlanLabs(g).Any(l => l.WeeksInDebt == 0 && SawRunwayTightening(l));
        }
        Assert.True(mid, "expected mid-retrenchment on seed 19 before save");
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        SaveSchemaPretend.AssertRoundTrip(loaded.Sim);
        StepYears(loaded, Years);
        foreach (LabState lab in PlanLabs(loaded))
            Assert.False(lab.Bankrupt, lab.Name + " bankrupt after save/load mid-retrench");
    }

    [Fact]
    public void Q201_retrenchment_and_no_bankruptcy_on_tight_seed_over_full_soak()
    {
        var g = Game.New(seed: 19);
        bool sawRetrench = false;
        GameConfig c = g.C;
        int weeks = Years * c.Calendar.WeeksPerYear;
        for (int w = 0; w < weeks; w++)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, c.Autopilot);
            g.Sim.Step();
            foreach (LabState lab in PlanLabs(g))
            {
                if (SawRunwayTightening(lab)) sawRetrench = true;
                Assert.False(lab.Bankrupt, "seed 19 week " + w + ": " + lab.Name);
            }
        }
        Assert.True(sawRetrench, "seed 19 full soak should hit Q200 retrenchment");
    }
}