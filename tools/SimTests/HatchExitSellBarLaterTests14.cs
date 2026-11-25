using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14416–14485: batch 279 exit/sell data bar (schema 29).</summary>
public class HatchExitSellBarLaterTests14
{
    [Fact] public void Q14416_exitsell14_4336() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14417_exitsell14_4337() { var g = Game.Solo(seed: 212202); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14418_exitsell14_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212303 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14419_exitsell14_4339() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14420_exitsell14_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14421_exitsell14_4341() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14422_exitsell14_4342() { var g = Game.Solo(seed: 212207); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14423_exitsell14_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212308 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14424_exitsell14_4344() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14425_exitsell14_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14426_exitsell14_4346() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14427_exitsell14_4347() { var g = Game.Solo(seed: 212212); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14428_exitsell14_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212313 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14429_exitsell14_4349() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14430_exitsell14_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14431_exitsell14_4351() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14432_exitsell14_4352() { var g = Game.Solo(seed: 212217); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14433_exitsell14_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212318 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14434_exitsell14_4354() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14435_exitsell14_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14436_exitsell14_4356() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14437_exitsell14_4357() { var g = Game.Solo(seed: 212222); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14438_exitsell14_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212323 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14439_exitsell14_4359() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14440_exitsell14_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14441_exitsell14_4361() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14442_exitsell14_4362() { var g = Game.Solo(seed: 212227); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14443_exitsell14_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212328 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14444_exitsell14_4364() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14445_exitsell14_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14446_exitsell14_4366() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14447_exitsell14_4367() { var g = Game.Solo(seed: 212232); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14448_exitsell14_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212333 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14449_exitsell14_4369() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14450_exitsell14_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14451_exitsell14_4371() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14452_exitsell14_4372() { var g = Game.Solo(seed: 212237); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14453_exitsell14_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212338 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14454_exitsell14_4374() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14455_exitsell14_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14456_exitsell14_4376() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14457_exitsell14_4377() { var g = Game.Solo(seed: 212242); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14458_exitsell14_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212343 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14459_exitsell14_4379() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14460_exitsell14_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14461_exitsell14_4381() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14462_exitsell14_4382() { var g = Game.Solo(seed: 212247); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14463_exitsell14_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212348 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14464_exitsell14_4384() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14465_exitsell14_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14466_exitsell14_4386() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14467_exitsell14_4387() { var g = Game.Solo(seed: 212252); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14468_exitsell14_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212353 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14469_exitsell14_4389() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14470_exitsell14_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14471_exitsell14_4391() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14472_exitsell14_4392() { var g = Game.Solo(seed: 212257); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14473_exitsell14_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212358 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14474_exitsell14_4394() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14475_exitsell14_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14476_exitsell14_4396() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14477_exitsell14_4397() { var g = Game.Solo(seed: 212262); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14478_exitsell14_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212363 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14479_exitsell14_4399() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14480_exitsell14_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14481_exitsell14_4401() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14482_exitsell14_4402() { var g = Game.Solo(seed: 212267); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14483_exitsell14_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 212368 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14484_exitsell14_4404() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14485_exitsell14_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70673); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
}
