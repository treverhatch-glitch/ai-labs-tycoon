using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2446–Q2515: batch 40 model-sales/chip data bar (schema 42).</summary>
public class HatchModelSalesChipBarLaterTests
{
    [Fact] public void Q2446_mschip_2446() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2447_mschip_2447() { var g = Game.Solo(seed: 5202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2448_mschip_2448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5304 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2449_mschip_2449() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2450_mschip_2450() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2451_mschip_2451() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2452_mschip_2452() { var g = Game.Solo(seed: 5207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2453_mschip_2453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5309 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2454_mschip_2454() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2455_mschip_2455() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2456_mschip_2456() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2457_mschip_2457() { var g = Game.Solo(seed: 5212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2458_mschip_2458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5314 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2459_mschip_2459() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2460_mschip_2460() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2461_mschip_2461() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2462_mschip_2462() { var g = Game.Solo(seed: 5217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2463_mschip_2463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5319 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2464_mschip_2464() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2465_mschip_2465() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2466_mschip_2466() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2467_mschip_2467() { var g = Game.Solo(seed: 5222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2468_mschip_2468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5324 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2469_mschip_2469() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2470_mschip_2470() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2471_mschip_2471() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2472_mschip_2472() { var g = Game.Solo(seed: 5227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2473_mschip_2473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5329 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2474_mschip_2474() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2475_mschip_2475() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2476_mschip_2476() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2477_mschip_2477() { var g = Game.Solo(seed: 5232); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2478_mschip_2478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5334 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2479_mschip_2479() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2480_mschip_2480() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2481_mschip_2481() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2482_mschip_2482() { var g = Game.Solo(seed: 5237); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2483_mschip_2483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5339 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2484_mschip_2484() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2485_mschip_2485() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2486_mschip_2486() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2487_mschip_2487() { var g = Game.Solo(seed: 5242); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2488_mschip_2488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5344 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2489_mschip_2489() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2490_mschip_2490() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2491_mschip_2491() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2492_mschip_2492() { var g = Game.Solo(seed: 5247); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2493_mschip_2493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5349 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2494_mschip_2494() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2495_mschip_2495() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2496_mschip_2496() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2497_mschip_2497() { var g = Game.Solo(seed: 5252); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2498_mschip_2498() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5354 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2499_mschip_2499() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2500_mschip_2500() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2501_mschip_2501() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2502_mschip_2502() { var g = Game.Solo(seed: 5257); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2503_mschip_2503() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5359 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2504_mschip_2504() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2505_mschip_2505() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2506_mschip_2506() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2507_mschip_2507() { var g = Game.Solo(seed: 5262); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2508_mschip_2508() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5364 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2509_mschip_2509() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2510_mschip_2510() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q2511_mschip_2511() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q2512_mschip_2512() { var g = Game.Solo(seed: 5267); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q2513_mschip_2513() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5369 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q2514_mschip_2514() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q2515_mschip_2515() { GameConfig c = TestData.Config(); var g = Game.New(seed: 40); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
}
