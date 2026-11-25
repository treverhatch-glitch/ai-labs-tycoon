using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14406–14475: batch 307 Pop/second-place data bar (schema 29).</summary>
public class HatchPopSecondPlaceBarLaterTests16
{
    [Fact] public void Q14406_popsec16_2376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14407_popsec16_2377() { var g = Game.Solo(seed: 278202); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14408_popsec16_2378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278303 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14409_popsec16_2379() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14410_popsec16_2380() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14411_popsec16_2381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14412_popsec16_2382() { var g = Game.Solo(seed: 278207); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14413_popsec16_2383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278308 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14414_popsec16_2384() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14415_popsec16_2385() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14416_popsec16_2386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14417_popsec16_2387() { var g = Game.Solo(seed: 278212); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14418_popsec16_2388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278313 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14419_popsec16_2389() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14420_popsec16_2390() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14421_popsec16_2391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14422_popsec16_2392() { var g = Game.Solo(seed: 278217); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14423_popsec16_2393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278318 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14424_popsec16_2394() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14425_popsec16_2395() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14426_popsec16_2396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14427_popsec16_2397() { var g = Game.Solo(seed: 278222); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14428_popsec16_2398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278323 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14429_popsec16_2399() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14430_popsec16_2400() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14431_popsec16_2401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14432_popsec16_2402() { var g = Game.Solo(seed: 278227); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14433_popsec16_2403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278328 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14434_popsec16_2404() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14435_popsec16_2405() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14436_popsec16_2406() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14437_popsec16_2407() { var g = Game.Solo(seed: 278232); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14438_popsec16_2408() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278333 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14439_popsec16_2409() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14440_popsec16_2410() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14441_popsec16_2411() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14442_popsec16_2412() { var g = Game.Solo(seed: 278237); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14443_popsec16_2413() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278338 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14444_popsec16_2414() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14445_popsec16_2415() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14446_popsec16_2416() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14447_popsec16_2417() { var g = Game.Solo(seed: 278242); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14448_popsec16_2418() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278343 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14449_popsec16_2419() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14450_popsec16_2420() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14451_popsec16_2421() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14452_popsec16_2422() { var g = Game.Solo(seed: 278247); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14453_popsec16_2423() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278348 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14454_popsec16_2424() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14455_popsec16_2425() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14456_popsec16_2426() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14457_popsec16_2427() { var g = Game.Solo(seed: 278252); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14458_popsec16_2428() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278353 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14459_popsec16_2429() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14460_popsec16_2430() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14461_popsec16_2431() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14462_popsec16_2432() { var g = Game.Solo(seed: 278257); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14463_popsec16_2433() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278358 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14464_popsec16_2434() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14465_popsec16_2435() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14466_popsec16_2436() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14467_popsec16_2437() { var g = Game.Solo(seed: 278262); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14468_popsec16_2438() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278363 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14469_popsec16_2439() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14470_popsec16_2440() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14471_popsec16_2441() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14472_popsec16_2442() { var g = Game.Solo(seed: 278267); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14473_popsec16_2443() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 278368 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14474_popsec16_2444() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14475_popsec16_2445() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
}
