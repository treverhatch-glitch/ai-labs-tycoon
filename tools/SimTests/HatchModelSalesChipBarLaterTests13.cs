using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14206–14275: batch 250 model-sales/chip data bar (schema 29).</summary>
public class HatchModelSalesChipBarLaterTests13
{
    [Fact] public void Q14206_mschip13_3566() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14207_mschip13_3567() { var g = Game.Solo(seed: 176122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14208_mschip13_3568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176223 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14209_mschip13_3569() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14210_mschip13_3570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14211_mschip13_3571() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14212_mschip13_3572() { var g = Game.Solo(seed: 176127); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14213_mschip13_3573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176228 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14214_mschip13_3574() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14215_mschip13_3575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14216_mschip13_3576() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14217_mschip13_3577() { var g = Game.Solo(seed: 176132); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14218_mschip13_3578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176233 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14219_mschip13_3579() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14220_mschip13_3580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14221_mschip13_3581() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14222_mschip13_3582() { var g = Game.Solo(seed: 176137); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14223_mschip13_3583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176238 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14224_mschip13_3584() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14225_mschip13_3585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14226_mschip13_3586() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14227_mschip13_3587() { var g = Game.Solo(seed: 176142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14228_mschip13_3588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176243 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14229_mschip13_3589() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14230_mschip13_3590() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14231_mschip13_3591() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14232_mschip13_3592() { var g = Game.Solo(seed: 176147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14233_mschip13_3593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176248 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14234_mschip13_3594() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14235_mschip13_3595() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14236_mschip13_3596() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14237_mschip13_3597() { var g = Game.Solo(seed: 176152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14238_mschip13_3598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176253 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14239_mschip13_3599() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14240_mschip13_3600() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14241_mschip13_3601() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14242_mschip13_3602() { var g = Game.Solo(seed: 176157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14243_mschip13_3603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176258 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14244_mschip13_3604() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14245_mschip13_3605() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14246_mschip13_3606() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14247_mschip13_3607() { var g = Game.Solo(seed: 176162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14248_mschip13_3608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176263 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14249_mschip13_3609() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14250_mschip13_3610() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14251_mschip13_3611() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14252_mschip13_3612() { var g = Game.Solo(seed: 176167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14253_mschip13_3613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176268 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14254_mschip13_3614() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14255_mschip13_3615() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14256_mschip13_3616() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14257_mschip13_3617() { var g = Game.Solo(seed: 176172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14258_mschip13_3618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176273 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14259_mschip13_3619() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14260_mschip13_3620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14261_mschip13_3621() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14262_mschip13_3622() { var g = Game.Solo(seed: 176177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14263_mschip13_3623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176278 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14264_mschip13_3624() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14265_mschip13_3625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14266_mschip13_3626() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14267_mschip13_3627() { var g = Game.Solo(seed: 176182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14268_mschip13_3628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176283 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14269_mschip13_3629() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14270_mschip13_3630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14271_mschip13_3631() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14272_mschip13_3632() { var g = Game.Solo(seed: 176187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14273_mschip13_3633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176288 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14274_mschip13_3634() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14275_mschip13_3635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
}
