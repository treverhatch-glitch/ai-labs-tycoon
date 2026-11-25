using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6916–6985: batch 104 bubble/distress data bar (schema 29).</summary>
public class HatchBubbleDistressBarLaterTests5
{
    [Fact] public void Q6916_bubdst5_2236() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6917_bubdst5_2237() { var g = Game.Solo(seed: 42162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6918_bubdst5_2238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42263 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6919_bubdst5_2239() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6920_bubdst5_2240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6921_bubdst5_2241() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6922_bubdst5_2242() { var g = Game.Solo(seed: 42167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6923_bubdst5_2243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42268 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6924_bubdst5_2244() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6925_bubdst5_2245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6926_bubdst5_2246() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6927_bubdst5_2247() { var g = Game.Solo(seed: 42172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6928_bubdst5_2248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42273 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6929_bubdst5_2249() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6930_bubdst5_2250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6931_bubdst5_2251() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6932_bubdst5_2252() { var g = Game.Solo(seed: 42177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6933_bubdst5_2253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42278 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6934_bubdst5_2254() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6935_bubdst5_2255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6936_bubdst5_2256() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6937_bubdst5_2257() { var g = Game.Solo(seed: 42182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6938_bubdst5_2258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42283 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6939_bubdst5_2259() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6940_bubdst5_2260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6941_bubdst5_2261() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6942_bubdst5_2262() { var g = Game.Solo(seed: 42187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6943_bubdst5_2263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42288 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6944_bubdst5_2264() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6945_bubdst5_2265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6946_bubdst5_2266() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6947_bubdst5_2267() { var g = Game.Solo(seed: 42192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6948_bubdst5_2268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42293 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6949_bubdst5_2269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6950_bubdst5_2270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6951_bubdst5_2271() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6952_bubdst5_2272() { var g = Game.Solo(seed: 42197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6953_bubdst5_2273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42298 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6954_bubdst5_2274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6955_bubdst5_2275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6956_bubdst5_2276() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6957_bubdst5_2277() { var g = Game.Solo(seed: 42202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6958_bubdst5_2278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42303 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6959_bubdst5_2279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6960_bubdst5_2280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6961_bubdst5_2281() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6962_bubdst5_2282() { var g = Game.Solo(seed: 42207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6963_bubdst5_2283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42308 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6964_bubdst5_2284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6965_bubdst5_2285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6966_bubdst5_2286() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6967_bubdst5_2287() { var g = Game.Solo(seed: 42212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6968_bubdst5_2288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42313 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6969_bubdst5_2289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6970_bubdst5_2290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6971_bubdst5_2291() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6972_bubdst5_2292() { var g = Game.Solo(seed: 42217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6973_bubdst5_2293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42318 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6974_bubdst5_2294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6975_bubdst5_2295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6976_bubdst5_2296() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6977_bubdst5_2297() { var g = Game.Solo(seed: 42222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6978_bubdst5_2298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42323 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6979_bubdst5_2299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6980_bubdst5_2300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6981_bubdst5_2301() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q6982_bubdst5_2302() { var g = Game.Solo(seed: 42227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6983_bubdst5_2303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 42328 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6984_bubdst5_2304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6985_bubdst5_2305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 86); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
