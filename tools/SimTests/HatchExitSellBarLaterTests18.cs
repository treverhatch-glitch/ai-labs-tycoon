using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14696–14765: batch 311 exit/sell data bar (schema 29).</summary>
public class HatchExitSellBarLaterTests18
{
    [Fact] public void Q14696_exitsell18_4616() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14697_exitsell18_4617() { var g = Game.Solo(seed: 282602); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14698_exitsell18_4618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282703 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14699_exitsell18_4619() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14700_exitsell18_4620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14701_exitsell18_4621() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14702_exitsell18_4622() { var g = Game.Solo(seed: 282607); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14703_exitsell18_4623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282708 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14704_exitsell18_4624() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14705_exitsell18_4625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14706_exitsell18_4626() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14707_exitsell18_4627() { var g = Game.Solo(seed: 282612); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14708_exitsell18_4628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282713 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14709_exitsell18_4629() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14710_exitsell18_4630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14711_exitsell18_4631() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14712_exitsell18_4632() { var g = Game.Solo(seed: 282617); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14713_exitsell18_4633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282718 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14714_exitsell18_4634() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14715_exitsell18_4635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14716_exitsell18_4636() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14717_exitsell18_4637() { var g = Game.Solo(seed: 282622); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14718_exitsell18_4638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282723 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14719_exitsell18_4639() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14720_exitsell18_4640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14721_exitsell18_4641() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14722_exitsell18_4642() { var g = Game.Solo(seed: 282627); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14723_exitsell18_4643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282728 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14724_exitsell18_4644() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14725_exitsell18_4645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14726_exitsell18_4646() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14727_exitsell18_4647() { var g = Game.Solo(seed: 282632); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14728_exitsell18_4648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282733 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14729_exitsell18_4649() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14730_exitsell18_4650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14731_exitsell18_4651() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14732_exitsell18_4652() { var g = Game.Solo(seed: 282637); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14733_exitsell18_4653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282738 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14734_exitsell18_4654() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14735_exitsell18_4655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14736_exitsell18_4656() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14737_exitsell18_4657() { var g = Game.Solo(seed: 282642); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14738_exitsell18_4658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282743 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14739_exitsell18_4659() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14740_exitsell18_4660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14741_exitsell18_4661() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14742_exitsell18_4662() { var g = Game.Solo(seed: 282647); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14743_exitsell18_4663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282748 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14744_exitsell18_4664() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14745_exitsell18_4665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14746_exitsell18_4666() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14747_exitsell18_4667() { var g = Game.Solo(seed: 282652); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14748_exitsell18_4668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282753 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14749_exitsell18_4669() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14750_exitsell18_4670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14751_exitsell18_4671() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14752_exitsell18_4672() { var g = Game.Solo(seed: 282657); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14753_exitsell18_4673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282758 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14754_exitsell18_4674() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14755_exitsell18_4675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14756_exitsell18_4676() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14757_exitsell18_4677() { var g = Game.Solo(seed: 282662); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14758_exitsell18_4678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282763 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14759_exitsell18_4679() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14760_exitsell18_4680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14761_exitsell18_4681() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14762_exitsell18_4682() { var g = Game.Solo(seed: 282667); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14763_exitsell18_4683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 282768 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14764_exitsell18_4684() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14765_exitsell18_4685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141153); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
}
