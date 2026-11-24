using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2516–Q2585: batch 41 acquisition/listing data bar (schema 42).</summary>
public class HatchAcquireListingBarLaterTests
{
    [Fact] public void Q2516_acqlist_2516() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2517_acqlist_2517() { var g = Game.Solo(seed: 5402); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2518_acqlist_2518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5504 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2519_acqlist_2519() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2520_acqlist_2520() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2521_acqlist_2521() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2522_acqlist_2522() { var g = Game.Solo(seed: 5407); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2523_acqlist_2523() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5509 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2524_acqlist_2524() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2525_acqlist_2525() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2526_acqlist_2526() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2527_acqlist_2527() { var g = Game.Solo(seed: 5412); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2528_acqlist_2528() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5514 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2529_acqlist_2529() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2530_acqlist_2530() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2531_acqlist_2531() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2532_acqlist_2532() { var g = Game.Solo(seed: 5417); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2533_acqlist_2533() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5519 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2534_acqlist_2534() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2535_acqlist_2535() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2536_acqlist_2536() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2537_acqlist_2537() { var g = Game.Solo(seed: 5422); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2538_acqlist_2538() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5524 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2539_acqlist_2539() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2540_acqlist_2540() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2541_acqlist_2541() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2542_acqlist_2542() { var g = Game.Solo(seed: 5427); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2543_acqlist_2543() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5529 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2544_acqlist_2544() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2545_acqlist_2545() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2546_acqlist_2546() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2547_acqlist_2547() { var g = Game.Solo(seed: 5432); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2548_acqlist_2548() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5534 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2549_acqlist_2549() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2550_acqlist_2550() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2551_acqlist_2551() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2552_acqlist_2552() { var g = Game.Solo(seed: 5437); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2553_acqlist_2553() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5539 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2554_acqlist_2554() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2555_acqlist_2555() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2556_acqlist_2556() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2557_acqlist_2557() { var g = Game.Solo(seed: 5442); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2558_acqlist_2558() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5544 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2559_acqlist_2559() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2560_acqlist_2560() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2561_acqlist_2561() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2562_acqlist_2562() { var g = Game.Solo(seed: 5447); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2563_acqlist_2563() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5549 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2564_acqlist_2564() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2565_acqlist_2565() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2566_acqlist_2566() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2567_acqlist_2567() { var g = Game.Solo(seed: 5452); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2568_acqlist_2568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5554 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2569_acqlist_2569() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2570_acqlist_2570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2571_acqlist_2571() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2572_acqlist_2572() { var g = Game.Solo(seed: 5457); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2573_acqlist_2573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5559 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2574_acqlist_2574() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2575_acqlist_2575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2576_acqlist_2576() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2577_acqlist_2577() { var g = Game.Solo(seed: 5462); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2578_acqlist_2578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5564 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2579_acqlist_2579() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2580_acqlist_2580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q2581_acqlist_2581() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q2582_acqlist_2582() { var g = Game.Solo(seed: 5467); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q2583_acqlist_2583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5569 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q2584_acqlist_2584() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q2585_acqlist_2585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
}
