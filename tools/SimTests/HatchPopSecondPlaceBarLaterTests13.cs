using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14196–14265: batch 283 Pop/second-place data bar (schema 29).</summary>
public class HatchPopSecondPlaceBarLaterTests13
{
    [Fact] public void Q14196_popsec13_2166() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14197_popsec13_2167() { var g = Game.Solo(seed: 225402); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14198_popsec13_2168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225503 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14199_popsec13_2169() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14200_popsec13_2170() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14201_popsec13_2171() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14202_popsec13_2172() { var g = Game.Solo(seed: 225407); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14203_popsec13_2173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225508 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14204_popsec13_2174() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14205_popsec13_2175() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14206_popsec13_2176() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14207_popsec13_2177() { var g = Game.Solo(seed: 225412); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14208_popsec13_2178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225513 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14209_popsec13_2179() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14210_popsec13_2180() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14211_popsec13_2181() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14212_popsec13_2182() { var g = Game.Solo(seed: 225417); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14213_popsec13_2183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225518 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14214_popsec13_2184() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14215_popsec13_2185() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14216_popsec13_2186() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14217_popsec13_2187() { var g = Game.Solo(seed: 225422); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14218_popsec13_2188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225523 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14219_popsec13_2189() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14220_popsec13_2190() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14221_popsec13_2191() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14222_popsec13_2192() { var g = Game.Solo(seed: 225427); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14223_popsec13_2193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225528 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14224_popsec13_2194() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14225_popsec13_2195() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14226_popsec13_2196() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14227_popsec13_2197() { var g = Game.Solo(seed: 225432); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14228_popsec13_2198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225533 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14229_popsec13_2199() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14230_popsec13_2200() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14231_popsec13_2201() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14232_popsec13_2202() { var g = Game.Solo(seed: 225437); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14233_popsec13_2203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225538 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14234_popsec13_2204() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14235_popsec13_2205() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14236_popsec13_2206() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14237_popsec13_2207() { var g = Game.Solo(seed: 225442); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14238_popsec13_2208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225543 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14239_popsec13_2209() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14240_popsec13_2210() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14241_popsec13_2211() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14242_popsec13_2212() { var g = Game.Solo(seed: 225447); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14243_popsec13_2213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225548 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14244_popsec13_2214() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14245_popsec13_2215() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14246_popsec13_2216() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14247_popsec13_2217() { var g = Game.Solo(seed: 225452); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14248_popsec13_2218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225553 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14249_popsec13_2219() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14250_popsec13_2220() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14251_popsec13_2221() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14252_popsec13_2222() { var g = Game.Solo(seed: 225457); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14253_popsec13_2223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225558 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14254_popsec13_2224() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14255_popsec13_2225() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14256_popsec13_2226() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14257_popsec13_2227() { var g = Game.Solo(seed: 225462); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14258_popsec13_2228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225563 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14259_popsec13_2229() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14260_popsec13_2230() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14261_popsec13_2231() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14262_popsec13_2232() { var g = Game.Solo(seed: 225467); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14263_popsec13_2233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 225568 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14264_popsec13_2234() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14265_popsec13_2235() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
}
