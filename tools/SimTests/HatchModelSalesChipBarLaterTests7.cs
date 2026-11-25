using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q5946–6015: batch 126 model-sales/chip data bar coda (schema 29).</summary>
public class HatchModelSalesChipBarLaterTests7
{
    [Fact] public void Q8456_mschip7_3426() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8457_mschip7_3427() { var g = Game.Solo(seed: 63602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8458_mschip7_3428() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63703 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8459_mschip7_3429() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8460_mschip7_3430() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8461_mschip7_3431() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8462_mschip7_3432() { var g = Game.Solo(seed: 63607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8463_mschip7_3433() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63708 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8464_mschip7_3434() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8465_mschip7_3435() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8466_mschip7_3436() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8467_mschip7_3437() { var g = Game.Solo(seed: 63612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8468_mschip7_3438() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63713 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8469_mschip7_3439() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8470_mschip7_3440() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8471_mschip7_3441() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8472_mschip7_3442() { var g = Game.Solo(seed: 63617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8473_mschip7_3443() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63718 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8474_mschip7_3444() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8475_mschip7_3445() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8476_mschip7_3446() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8477_mschip7_3447() { var g = Game.Solo(seed: 63622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8478_mschip7_3448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63723 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8479_mschip7_3449() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8480_mschip7_3450() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8481_mschip7_3451() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8482_mschip7_3452() { var g = Game.Solo(seed: 63627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8483_mschip7_3453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63728 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8484_mschip7_3454() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8485_mschip7_3455() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8486_mschip7_3456() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8487_mschip7_3457() { var g = Game.Solo(seed: 63632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8488_mschip7_3458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63733 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8489_mschip7_3459() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8490_mschip7_3460() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8491_mschip7_3461() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8492_mschip7_3462() { var g = Game.Solo(seed: 63637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8493_mschip7_3463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63738 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8494_mschip7_3464() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8495_mschip7_3465() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8496_mschip7_3466() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8497_mschip7_3467() { var g = Game.Solo(seed: 63642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8498_mschip7_3468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63743 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8499_mschip7_3469() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8500_mschip7_3470() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8501_mschip7_3471() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8502_mschip7_3472() { var g = Game.Solo(seed: 63647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8503_mschip7_3473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63748 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8504_mschip7_3474() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8505_mschip7_3475() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8506_mschip7_3476() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8507_mschip7_3477() { var g = Game.Solo(seed: 63652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8508_mschip7_3478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63753 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8509_mschip7_3479() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8510_mschip7_3480() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8511_mschip7_3481() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8512_mschip7_3482() { var g = Game.Solo(seed: 63657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8513_mschip7_3483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63758 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8514_mschip7_3484() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8515_mschip7_3485() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8516_mschip7_3486() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8517_mschip7_3487() { var g = Game.Solo(seed: 63662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8518_mschip7_3488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63763 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8519_mschip7_3489() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8520_mschip7_3490() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q8521_mschip7_3491() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q8522_mschip7_3492() { var g = Game.Solo(seed: 63667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q8523_mschip7_3493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 63768 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q8524_mschip7_3494() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q8525_mschip7_3495() { GameConfig c = TestData.Config(); var g = Game.New(seed: 108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
}
