using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2236–Q2305: batch 37 bubble/distress data bar (schema 42).</summary>
public class HatchBubbleDistressBarLaterTests
{
    [Fact] public void Q2236_bubble_2236() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2237_bubble_2237() { var g = Game.Solo(seed: 4601); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2238_bubble_2238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4702 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2239_bubble_2239() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2240_bubble_2240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2241_bubble_2241() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2242_bubble_2242() { var g = Game.Solo(seed: 4606); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2243_bubble_2243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4707 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2244_bubble_2244() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2245_bubble_2245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2246_bubble_2246() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2247_bubble_2247() { var g = Game.Solo(seed: 4611); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2248_bubble_2248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4712 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2249_bubble_2249() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2250_bubble_2250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2251_bubble_2251() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2252_bubble_2252() { var g = Game.Solo(seed: 4616); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2253_bubble_2253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4717 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2254_bubble_2254() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2255_bubble_2255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2256_bubble_2256() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2257_bubble_2257() { var g = Game.Solo(seed: 4621); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2258_bubble_2258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4722 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2259_bubble_2259() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2260_bubble_2260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2261_bubble_2261() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2262_bubble_2262() { var g = Game.Solo(seed: 4626); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2263_bubble_2263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4727 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2264_bubble_2264() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2265_bubble_2265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2266_bubble_2266() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2267_bubble_2267() { var g = Game.Solo(seed: 4631); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2268_bubble_2268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4732 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2269_bubble_2269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2270_bubble_2270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2271_bubble_2271() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2272_bubble_2272() { var g = Game.Solo(seed: 4636); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2273_bubble_2273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4737 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2274_bubble_2274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2275_bubble_2275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2276_bubble_2276() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2277_bubble_2277() { var g = Game.Solo(seed: 4641); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2278_bubble_2278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4742 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2279_bubble_2279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2280_bubble_2280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2281_bubble_2281() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2282_bubble_2282() { var g = Game.Solo(seed: 4646); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2283_bubble_2283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4747 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2284_bubble_2284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2285_bubble_2285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2286_bubble_2286() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2287_bubble_2287() { var g = Game.Solo(seed: 4651); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2288_bubble_2288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4752 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2289_bubble_2289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2290_bubble_2290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2291_bubble_2291() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2292_bubble_2292() { var g = Game.Solo(seed: 4656); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2293_bubble_2293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4757 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2294_bubble_2294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2295_bubble_2295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2296_bubble_2296() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2297_bubble_2297() { var g = Game.Solo(seed: 4661); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2298_bubble_2298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4762 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2299_bubble_2299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2300_bubble_2300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2301_bubble_2301() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q2302_bubble_2302() { var g = Game.Solo(seed: 4666); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q2303_bubble_2303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4767 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2304_bubble_2304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2305_bubble_2305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 21); Assert.Equal(43, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
