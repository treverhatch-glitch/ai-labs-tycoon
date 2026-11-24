using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q5946–6015: batch 90 model-sales/chip data bar coda (schema 29).</summary>
public class HatchModelSalesChipBarLaterTests5
{
    [Fact] public void Q5946_mschip2_3426() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5947_mschip2_3427() { var g = Game.Solo(seed: 28422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5948_mschip2_3428() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28523 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5949_mschip2_3429() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5950_mschip2_3430() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5951_mschip2_3431() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5952_mschip2_3432() { var g = Game.Solo(seed: 28427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5953_mschip2_3433() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28528 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5954_mschip2_3434() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5955_mschip2_3435() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5956_mschip2_3436() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5957_mschip2_3437() { var g = Game.Solo(seed: 28432); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5958_mschip2_3438() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28533 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5959_mschip2_3439() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5960_mschip2_3440() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5961_mschip2_3441() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5962_mschip2_3442() { var g = Game.Solo(seed: 28437); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5963_mschip2_3443() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28538 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5964_mschip2_3444() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5965_mschip2_3445() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5966_mschip2_3446() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5967_mschip2_3447() { var g = Game.Solo(seed: 28442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5968_mschip2_3448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28543 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5969_mschip2_3449() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5970_mschip2_3450() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5971_mschip2_3451() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5972_mschip2_3452() { var g = Game.Solo(seed: 28447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5973_mschip2_3453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28548 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5974_mschip2_3454() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5975_mschip2_3455() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5976_mschip2_3456() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5977_mschip2_3457() { var g = Game.Solo(seed: 28452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5978_mschip2_3458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28553 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5979_mschip2_3459() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5980_mschip2_3460() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5981_mschip2_3461() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5982_mschip2_3462() { var g = Game.Solo(seed: 28457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5983_mschip2_3463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28558 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5984_mschip2_3464() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5985_mschip2_3465() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5986_mschip2_3466() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5987_mschip2_3467() { var g = Game.Solo(seed: 28462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5988_mschip2_3468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28563 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5989_mschip2_3469() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5990_mschip2_3470() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5991_mschip2_3471() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5992_mschip2_3472() { var g = Game.Solo(seed: 28467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5993_mschip2_3473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28568 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5994_mschip2_3474() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q5995_mschip2_3475() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q5996_mschip2_3476() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q5997_mschip2_3477() { var g = Game.Solo(seed: 28472); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q5998_mschip2_3478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28573 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q5999_mschip2_3479() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q6000_mschip2_3480() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q6001_mschip2_3481() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q6002_mschip2_3482() { var g = Game.Solo(seed: 28477); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q6003_mschip2_3483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28578 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q6004_mschip2_3484() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q6005_mschip2_3485() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q6006_mschip2_3486() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q6007_mschip2_3487() { var g = Game.Solo(seed: 28482); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q6008_mschip2_3488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28583 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q6009_mschip2_3489() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q6010_mschip2_3490() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q6011_mschip2_3491() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q6012_mschip2_3492() { var g = Game.Solo(seed: 28487); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q6013_mschip2_3493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 28588 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q6014_mschip2_3494() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q6015_mschip2_3495() { GameConfig c = TestData.Config(); var g = Game.New(seed: 72); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
}
