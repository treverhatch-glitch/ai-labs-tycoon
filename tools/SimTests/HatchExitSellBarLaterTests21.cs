using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14906–14975: batch 335 exit/sell data bar (schema 29).</summary>
public class HatchExitSellBarLaterTests21
{
    [Fact] public void Q14906_exitsell21_4826() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14907_exitsell21_4827() { var g = Game.Solo(seed: 335402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14908_exitsell21_4828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335503 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14909_exitsell21_4829() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14910_exitsell21_4830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14911_exitsell21_4831() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14912_exitsell21_4832() { var g = Game.Solo(seed: 335407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14913_exitsell21_4833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335508 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14914_exitsell21_4834() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14915_exitsell21_4835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14916_exitsell21_4836() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14917_exitsell21_4837() { var g = Game.Solo(seed: 335412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14918_exitsell21_4838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335513 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14919_exitsell21_4839() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14920_exitsell21_4840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14921_exitsell21_4841() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14922_exitsell21_4842() { var g = Game.Solo(seed: 335417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14923_exitsell21_4843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335518 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14924_exitsell21_4844() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14925_exitsell21_4845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14926_exitsell21_4846() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14927_exitsell21_4847() { var g = Game.Solo(seed: 335422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14928_exitsell21_4848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335523 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14929_exitsell21_4849() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14930_exitsell21_4850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14931_exitsell21_4851() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14932_exitsell21_4852() { var g = Game.Solo(seed: 335427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14933_exitsell21_4853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335528 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14934_exitsell21_4854() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14935_exitsell21_4855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14936_exitsell21_4856() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14937_exitsell21_4857() { var g = Game.Solo(seed: 335432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14938_exitsell21_4858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335533 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14939_exitsell21_4859() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14940_exitsell21_4860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14941_exitsell21_4861() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14942_exitsell21_4862() { var g = Game.Solo(seed: 335437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14943_exitsell21_4863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335538 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14944_exitsell21_4864() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14945_exitsell21_4865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14946_exitsell21_4866() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14947_exitsell21_4867() { var g = Game.Solo(seed: 335442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14948_exitsell21_4868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335543 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14949_exitsell21_4869() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14950_exitsell21_4870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14951_exitsell21_4871() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14952_exitsell21_4872() { var g = Game.Solo(seed: 335447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14953_exitsell21_4873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335548 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14954_exitsell21_4874() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14955_exitsell21_4875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14956_exitsell21_4876() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14957_exitsell21_4877() { var g = Game.Solo(seed: 335452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14958_exitsell21_4878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335553 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14959_exitsell21_4879() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14960_exitsell21_4880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14961_exitsell21_4881() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14962_exitsell21_4882() { var g = Game.Solo(seed: 335457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14963_exitsell21_4883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335558 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14964_exitsell21_4884() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14965_exitsell21_4885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14966_exitsell21_4886() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14967_exitsell21_4887() { var g = Game.Solo(seed: 335462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14968_exitsell21_4888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335563 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14969_exitsell21_4889() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14970_exitsell21_4890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14971_exitsell21_4891() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14972_exitsell21_4892() { var g = Game.Solo(seed: 335467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14973_exitsell21_4893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 335568 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14974_exitsell21_4894() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14975_exitsell21_4895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194013); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
}
