using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Long headless soaks: each official start era, bankruptcy, and a shady-heavy path.</summary>
public class HeadlessSoakVariantsHardeningTests
{
    const int Years = 10;
    const int WeeksPerYear = 48;

    public static IEnumerable<object[]> StartEras()
    {
        yield return new object[] { (string)null, 77L };
        yield return new object[] { "punchline", 9009L };
    }

    [Theory]
    [MemberData(nameof(StartEras))]
    public void Ten_years_from_each_start_era_is_deterministic_and_invariant_safe(string eraId, long seed)
    {
        HeadlessSimHardeningTests_Soak.AssertTwoRuns(seed, eraId, Years, null);
    }

    [Fact]
    public void Bankrupt_path_soak_stays_invariant_safe_and_deterministic()
    {
        HeadlessSimHardeningTests_Soak.Configure configure = g =>
        {
            g.Me.GrantPerWeek = 0;
            g.Me.GrantEndsTick = 0;
            g.WithStarterLab(researchers: 2, engineers: 2, gpus: 40);
        };
        HeadlessSimHardeningTests_Soak.AssertTwoRuns(31_031, null, 8, configure);
        Game once = HeadlessSimHardeningTests_Soak.RunGameOnce(31_031, null, 8, configure);
        Assert.Contains(once.State.Labs, l => l.Bankrupt);
    }

    [Fact]
    public void Heavy_shady_path_soak_is_deterministic_and_invariant_safe()
    {
        HeadlessSimHardeningTests_Soak.Configure configure = g =>
        {
            g.WithStarterLab(gpus: 10, safety: 1);
            g.Me.Heat = 420;
            g.Me.Stash = 180_000;
            ModelState model = g.Train("mini", 50);
            g.UntilTrained(model);
            model.Corruption = 820;
            g.Do(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""));
        };
        HeadlessSimHardeningTests_Soak.AssertTwoRuns(13_313, null, Years, configure);
    }

    static class HeadlessSimHardeningTests_Soak
    {
        public delegate void Configure(Game g);

        public static void AssertTwoRuns(long seed, string eraId, int years, Configure configure)
        {
            var a = RunOnceDigests(seed, eraId, years, configure);
            var b = RunOnceDigests(seed, eraId, years, configure);
            Assert.Equal(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++) Assert.Equal(a[i], b[i]);
        }

        public static Game RunGameOnce(long seed, string eraId, int years, Configure configure)
        {
            var options = new NewGameOptions { Seed = seed };
            if (!string.IsNullOrEmpty(eraId)) options.StartEraId = eraId;
            var g = new Game(Simulation.NewGame(TestData.Config(), options));
            configure?.Invoke(g);

            int totalWeeks = years * WeeksPerYear;
            for (int week = 0; week < totalWeeks; week++)
            {
                if (GameDate.IsMonthStart(g.C.Calendar, g.Tick) && g.State.PlayerLabId >= 0)
                    g.Sim.RunPlan(g.State.PlayerLabId, g.C.Autopilot);
                g.Sim.Step();
                if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) SimPlayInvariants.AssertAll(g.C, g.State);
            }

            SimPlayInvariants.AssertAll(g.C, g.State);
            return g;
        }

        public static List<string> RunOnceDigests(long seed, string eraId, int years, Configure configure)
        {
            var options = new NewGameOptions { Seed = seed };
            if (!string.IsNullOrEmpty(eraId)) options.StartEraId = eraId;
            var g = new Game(Simulation.NewGame(TestData.Config(), options));
            configure?.Invoke(g);
            int totalWeeks = years * WeeksPerYear;
            var monthDigests = new List<string>();
            for (int week = 0; week < totalWeeks; week++)
            {
                if (GameDate.IsMonthStart(g.C.Calendar, g.Tick) && g.State.PlayerLabId >= 0)
                    g.Sim.RunPlan(g.State.PlayerLabId, g.C.Autopilot);
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
}