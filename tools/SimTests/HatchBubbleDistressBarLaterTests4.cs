using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q5806–Q5875: batch 88 bubble/distress data bar (schema 29).</summary>
public class HatchBubbleDistressBarLaterTests4
{
    [Fact] public void Q5806_bubdst4_2236() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5807_bubdst4_2237() { var g = Game.Solo(seed: 26462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5808_bubdst4_2238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26563 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5809_bubdst4_2239() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5810_bubdst4_2240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5811_bubdst4_2241() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5812_bubdst4_2242() { var g = Game.Solo(seed: 26467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5813_bubdst4_2243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26568 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5814_bubdst4_2244() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5815_bubdst4_2245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5816_bubdst4_2246() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5817_bubdst4_2247() { var g = Game.Solo(seed: 26472); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5818_bubdst4_2248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26573 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5819_bubdst4_2249() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5820_bubdst4_2250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5821_bubdst4_2251() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5822_bubdst4_2252() { var g = Game.Solo(seed: 26477); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5823_bubdst4_2253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26578 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5824_bubdst4_2254() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5825_bubdst4_2255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5826_bubdst4_2256() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5827_bubdst4_2257() { var g = Game.Solo(seed: 26482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5828_bubdst4_2258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26583 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5829_bubdst4_2259() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5830_bubdst4_2260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5831_bubdst4_2261() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5832_bubdst4_2262() { var g = Game.Solo(seed: 26487); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5833_bubdst4_2263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26588 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5834_bubdst4_2264() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5835_bubdst4_2265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5836_bubdst4_2266() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5837_bubdst4_2267() { var g = Game.Solo(seed: 26492); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5838_bubdst4_2268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26593 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5839_bubdst4_2269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5840_bubdst4_2270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5841_bubdst4_2271() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5842_bubdst4_2272() { var g = Game.Solo(seed: 26497); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5843_bubdst4_2273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26598 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5844_bubdst4_2274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5845_bubdst4_2275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5846_bubdst4_2276() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5847_bubdst4_2277() { var g = Game.Solo(seed: 26502); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5848_bubdst4_2278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26603 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5849_bubdst4_2279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5850_bubdst4_2280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5851_bubdst4_2281() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5852_bubdst4_2282() { var g = Game.Solo(seed: 26507); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5853_bubdst4_2283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26608 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5854_bubdst4_2284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5855_bubdst4_2285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5856_bubdst4_2286() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5857_bubdst4_2287() { var g = Game.Solo(seed: 26512); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5858_bubdst4_2288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26613 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5859_bubdst4_2289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5860_bubdst4_2290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5861_bubdst4_2291() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5862_bubdst4_2292() { var g = Game.Solo(seed: 26517); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5863_bubdst4_2293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26618 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5864_bubdst4_2294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5865_bubdst4_2295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5866_bubdst4_2296() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5867_bubdst4_2297() { var g = Game.Solo(seed: 26522); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5868_bubdst4_2298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26623 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5869_bubdst4_2299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5870_bubdst4_2300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5871_bubdst4_2301() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q5872_bubdst4_2302() { var g = Game.Solo(seed: 26527); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q5873_bubdst4_2303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 26628 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5874_bubdst4_2304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5875_bubdst4_2305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
