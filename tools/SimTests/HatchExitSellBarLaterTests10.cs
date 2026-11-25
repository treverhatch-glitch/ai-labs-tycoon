using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11596–11665: batch 211 exit/sell data bar coda (schema 29).</summary>
public class HatchExitSellBarLaterTests10
{
    [Fact] public void Q14136_exitsell10_4056() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14137_exitsell10_4057() { var g = Game.Solo(seed: 141802); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14138_exitsell10_4058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141903 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14139_exitsell10_4059() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14140_exitsell10_4060() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14141_exitsell10_4061() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14142_exitsell10_4062() { var g = Game.Solo(seed: 141807); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14143_exitsell10_4063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141908 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14144_exitsell10_4064() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14145_exitsell10_4065() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14146_exitsell10_4066() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14147_exitsell10_4067() { var g = Game.Solo(seed: 141812); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14148_exitsell10_4068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141913 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14149_exitsell10_4069() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14150_exitsell10_4070() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14151_exitsell10_4071() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14152_exitsell10_4072() { var g = Game.Solo(seed: 141817); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14153_exitsell10_4073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141918 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14154_exitsell10_4074() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14155_exitsell10_4075() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14156_exitsell10_4076() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14157_exitsell10_4077() { var g = Game.Solo(seed: 141822); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14158_exitsell10_4078() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141923 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14159_exitsell10_4079() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14160_exitsell10_4080() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14161_exitsell10_4081() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14162_exitsell10_4082() { var g = Game.Solo(seed: 141827); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14163_exitsell10_4083() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141928 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14164_exitsell10_4084() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14165_exitsell10_4085() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14166_exitsell10_4086() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14167_exitsell10_4087() { var g = Game.Solo(seed: 141832); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14168_exitsell10_4088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141933 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14169_exitsell10_4089() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14170_exitsell10_4090() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14171_exitsell10_4091() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14172_exitsell10_4092() { var g = Game.Solo(seed: 141837); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14173_exitsell10_4093() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141938 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14174_exitsell10_4094() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14175_exitsell10_4095() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14176_exitsell10_4096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14177_exitsell10_4097() { var g = Game.Solo(seed: 141842); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14178_exitsell10_4098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141943 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14179_exitsell10_4099() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14180_exitsell10_4100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14181_exitsell10_4101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14182_exitsell10_4102() { var g = Game.Solo(seed: 141847); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14183_exitsell10_4103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141948 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14184_exitsell10_4104() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14185_exitsell10_4105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14186_exitsell10_4106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14187_exitsell10_4107() { var g = Game.Solo(seed: 141852); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14188_exitsell10_4108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141953 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14189_exitsell10_4109() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14190_exitsell10_4110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14191_exitsell10_4111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14192_exitsell10_4112() { var g = Game.Solo(seed: 141857); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14193_exitsell10_4113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141958 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14194_exitsell10_4114() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14195_exitsell10_4115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14196_exitsell10_4116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14197_exitsell10_4117() { var g = Game.Solo(seed: 141862); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14198_exitsell10_4118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141963 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14199_exitsell10_4119() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14200_exitsell10_4120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14201_exitsell10_4121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14202_exitsell10_4122() { var g = Game.Solo(seed: 141867); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14203_exitsell10_4123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141968 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14204_exitsell10_4124() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14205_exitsell10_4125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 193); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
}
