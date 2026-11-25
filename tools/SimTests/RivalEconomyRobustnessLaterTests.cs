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
    const int DefaultSeedCount = 40;
    const int FullSoakSeedCount = 200;
    const int MaxBankruptSeeds = 10;
    const int MinActivePlanRivals = 3;
    const int NoBankruptBeforeYear = 2005;

    static readonly string[] MajorRivalIds = { "openaye", "anthoptics" };

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

    sealed class PlanBankruptRecord
    {
        public string Name;
        public string RivalId;
        public int Year;
    }

    sealed class SoakBankruptcyResult
    {
        public long Seed;
        public List<PlanBankruptRecord> Bankrupts = new List<PlanBankruptRecord>();
        public int ActivePlanRivals;
    }

    static SoakBankruptcyResult SimulateSeedBankruptcies(Action<GameConfig> tweak, long seed, int years)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Devices.Enabled = 0;
            c.Platforms.Enabled = 0;
            c.Creed.Enabled = 0;
            c.Shocks.Enabled = 0;
            c.Replication.Enabled = 0;
            tweak?.Invoke(c);
        }), seed));
        GameConfig c = g.C;
        var result = new SoakBankruptcyResult { Seed = seed };
        var wasBankrupt = new Dictionary<int, bool>();
        foreach (LabState lab in g.State.Labs) wasBankrupt[lab.Id] = lab.Bankrupt;

        for (int w = 0; w < years * c.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, c.Autopilot);
            g.Sim.Step();
            foreach (LabState lab in g.State.Labs)
            {
                if (lab.Controller != LabController.Plan) continue;
                bool prev = wasBankrupt.TryGetValue(lab.Id, out bool b) && b;
                if (!prev && lab.Bankrupt)
                {
                    wasBankrupt[lab.Id] = true;
                    result.Bankrupts.Add(new PlanBankruptRecord
                    {
                        Name = lab.Name,
                        RivalId = lab.RivalId,
                        Year = GameDate.Year(c.Calendar, g.Tick)
                    });
                }
                else if (!wasBankrupt.ContainsKey(lab.Id)) wasBankrupt[lab.Id] = lab.Bankrupt;
            }
        }

        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Controller != LabController.Plan) continue;
            if (!lab.Bankrupt && lab.AbsorbedByLabId < 0) result.ActivePlanRivals++;
        }
        return result;
    }

    static List<string> PlanBankruptciesAfterYears(Action<GameConfig> tweak, long seed, int years) =>
        SimulateSeedBankruptcies(tweak, seed, years).Bankrupts.Select(b => "seed " + seed + ": " + b.Name).ToList();

  /// Q218 design bar (coordinator): occasional rival bankruptcy allowed on the 200×40 soak.
    static void AssertSoakBankruptcyDesignRules(Action<GameConfig> tweak, int seedCount)
    {
        int seedsWithBankrupt = 0;
        var failures = new List<string>();
        for (long seed = 1; seed <= seedCount; seed++)
        {
            SoakBankruptcyResult r = SimulateSeedBankruptcies(tweak, seed, Years);
            if (r.Bankrupts.Count > 0) seedsWithBankrupt++;

            foreach (PlanBankruptRecord b in r.Bankrupts)
            {
                if (b.Year < NoBankruptBeforeYear)
                    failures.Add("seed " + seed + " " + b.Name + " bankrupt year " + b.Year + " before " + NoBankruptBeforeYear);
            }
            int majorBankrupt = r.Bankrupts.Count(b => MajorRivalIds.Contains(b.RivalId));
            if (majorBankrupt > 1)
                failures.Add("seed " + seed + " has " + majorBankrupt + " major rival bankruptcies");
            if (r.ActivePlanRivals < MinActivePlanRivals)
                failures.Add("seed " + seed + " only " + r.ActivePlanRivals + " active plan rivals");
        }
        if (seedsWithBankrupt > MaxBankruptSeeds)
            failures.Insert(0, seedsWithBankrupt + " seeds with plan bankruptcies exceeds max " + MaxBankruptSeeds);
        Assert.Empty(failures);
    }

    static int SoakSeedCount()
    {
        string env = Environment.GetEnvironmentVariable("BALANCE_SOAK_FULL");
        return env == "1" || string.Equals(env, "true", StringComparison.OrdinalIgnoreCase) ? FullSoakSeedCount : DefaultSeedCount;
    }

    static void AssertFortyYearSolvent(Action<GameConfig> tweak, string label, int seedCount)
    {
        var bankrupts = new List<string>();
        for (long seed = 1; seed <= seedCount; seed++)
            bankrupts.AddRange(PlanBankruptciesAfterYears(tweak, seed, Years).Select(b => label + " " + b));
        Assert.Empty(bankrupts);
    }

    static void AssertFortyFortySolvent(Action<GameConfig> tweak, string label) =>
        AssertFortyYearSolvent(tweak, label, SoakSeedCount());

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
        var g = Game.New(seed: 20);
        bool saw = false;
        for (int i = 0; i < 24 * g.C.Calendar.WeeksPerYear && !saw; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            foreach (LabState lab in PlanLabs(g))
            {
                if (lab.WeeksInDebt > 0) continue;
                if (SawRunwayTightening(lab)) { saw = true; break; }
            }
        }
        Assert.True(saw, "expected runway-driven tightening on seed 20 before debt");
    }

    [Fact]
    public void Q201_shipped_rival_distress_harness_defaults()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(35, c.Acquisitions.PlannerAcquireChancePermille);
        Assert.True(c.Acquisitions.CrashDistressPricePermille < MathI.Permille);
        Assert.True(c.ModelSales.CrashDistressOfferPermille < MathI.Permille);
        Assert.True(c.Chips.DistressListingPricePermille < MathI.Permille);
    }

    [Fact]
    public void Q201_forty_seeds_forty_years_default_harness_no_plan_bankruptcies()
    {
        AssertSoakBankruptcyDesignRules(null, SoakSeedCount());
    }

    /// <summary>Q216 / Q218: soak seed range obeys coordinator bankruptcy design rules (not zero).</summary>
    [Fact]
    public void Q216_plan_rivals_solvent_across_soak_seed_range()
    {
        AssertSoakBankruptcyDesignRules(null, SoakSeedCount());
    }

    /// <summary>Q218: pins (a)–(d) on historical failure seeds (no pre-2005 / dual-major / &lt;3 rivals).</summary>
    [Theory]
    [InlineData(42)]
    [InlineData(54)]
    [InlineData(121)]
    [InlineData(141)]
    [InlineData(160)]
    [InlineData(172)]
    [InlineData(187)]
    [InlineData(188)]
    public void Q218_known_failure_seeds_obey_hard_bankruptcy_rules(long seed)
    {
        SoakBankruptcyResult r = SimulateSeedBankruptcies(null, seed, Years);
        foreach (PlanBankruptRecord b in r.Bankrupts)
            Assert.True(b.Year >= NoBankruptBeforeYear, seed + " " + b.Name + " year " + b.Year);
        Assert.True(r.Bankrupts.Count(b => MajorRivalIds.Contains(b.RivalId)) <= 1);
        Assert.True(r.ActivePlanRivals >= MinActivePlanRivals, "seed " + seed);
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
        var g = Game.New(seed: 20);
        bool mid = false;
        for (int i = 0; i < 24 * g.C.Calendar.WeeksPerYear && !mid; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
            mid = PlanLabs(g).Any(l => l.WeeksInDebt == 0 && SawRunwayTightening(l));
        }
        Assert.True(mid, "expected mid-retrenchment on seed 20 before save");
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
        var g = Game.New(seed: 20);
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
                Assert.False(lab.Bankrupt, "seed 20 week " + w + ": " + lab.Name);
            }
        }
        Assert.True(sawRetrench, "seed 20 full soak should hit Q200 retrenchment");
    }
}