using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14476–14545: batch 315 Pop/second-place data bar (schema 29).</summary>
public class HatchPopSecondPlaceBarLaterTests17
{
    [Fact] public void Q14476_popsec17_2446() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14477_popsec17_2447() { var g = Game.Solo(seed: 295802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14478_popsec17_2448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295903 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14479_popsec17_2449() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14480_popsec17_2450() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14481_popsec17_2451() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14482_popsec17_2452() { var g = Game.Solo(seed: 295807); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14483_popsec17_2453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295908 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14484_popsec17_2454() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14485_popsec17_2455() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14486_popsec17_2456() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14487_popsec17_2457() { var g = Game.Solo(seed: 295812); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14488_popsec17_2458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295913 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14489_popsec17_2459() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14490_popsec17_2460() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14491_popsec17_2461() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14492_popsec17_2462() { var g = Game.Solo(seed: 295817); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14493_popsec17_2463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295918 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14494_popsec17_2464() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14495_popsec17_2465() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14496_popsec17_2466() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14497_popsec17_2467() { var g = Game.Solo(seed: 295822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14498_popsec17_2468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295923 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14499_popsec17_2469() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14500_popsec17_2470() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14501_popsec17_2471() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14502_popsec17_2472() { var g = Game.Solo(seed: 295827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14503_popsec17_2473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295928 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14504_popsec17_2474() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14505_popsec17_2475() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14506_popsec17_2476() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14507_popsec17_2477() { var g = Game.Solo(seed: 295832); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14508_popsec17_2478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295933 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14509_popsec17_2479() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14510_popsec17_2480() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14511_popsec17_2481() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14512_popsec17_2482() { var g = Game.Solo(seed: 295837); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14513_popsec17_2483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295938 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14514_popsec17_2484() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14515_popsec17_2485() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14516_popsec17_2486() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14517_popsec17_2487() { var g = Game.Solo(seed: 295842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14518_popsec17_2488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295943 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14519_popsec17_2489() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14520_popsec17_2490() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14521_popsec17_2491() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14522_popsec17_2492() { var g = Game.Solo(seed: 295847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14523_popsec17_2493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295948 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14524_popsec17_2494() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14525_popsec17_2495() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14526_popsec17_2496() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14527_popsec17_2497() { var g = Game.Solo(seed: 295852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14528_popsec17_2498() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295953 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14529_popsec17_2499() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14530_popsec17_2500() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14531_popsec17_2501() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14532_popsec17_2502() { var g = Game.Solo(seed: 295857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14533_popsec17_2503() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295958 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14534_popsec17_2504() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14535_popsec17_2505() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14536_popsec17_2506() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14537_popsec17_2507() { var g = Game.Solo(seed: 295862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14538_popsec17_2508() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295963 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14539_popsec17_2509() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14540_popsec17_2510() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14541_popsec17_2511() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14542_popsec17_2512() { var g = Game.Solo(seed: 295867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14543_popsec17_2513() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 295968 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14544_popsec17_2514() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14545_popsec17_2515() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
}
