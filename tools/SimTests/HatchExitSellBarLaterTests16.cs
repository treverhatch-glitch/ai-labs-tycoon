using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14556–14625: batch 295 exit/sell data bar (schema 29).</summary>
public class HatchExitSellBarLaterTests16
{
    [Fact] public void Q14556_exitsell16_4476() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14557_exitsell16_4477() { var g = Game.Solo(seed: 247402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14558_exitsell16_4478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247503 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14559_exitsell16_4479() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14560_exitsell16_4480() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14561_exitsell16_4481() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14562_exitsell16_4482() { var g = Game.Solo(seed: 247407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14563_exitsell16_4483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247508 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14564_exitsell16_4484() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14565_exitsell16_4485() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14566_exitsell16_4486() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14567_exitsell16_4487() { var g = Game.Solo(seed: 247412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14568_exitsell16_4488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247513 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14569_exitsell16_4489() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14570_exitsell16_4490() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14571_exitsell16_4491() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14572_exitsell16_4492() { var g = Game.Solo(seed: 247417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14573_exitsell16_4493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247518 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14574_exitsell16_4494() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14575_exitsell16_4495() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14576_exitsell16_4496() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14577_exitsell16_4497() { var g = Game.Solo(seed: 247422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14578_exitsell16_4498() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247523 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14579_exitsell16_4499() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14580_exitsell16_4500() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14581_exitsell16_4501() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14582_exitsell16_4502() { var g = Game.Solo(seed: 247427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14583_exitsell16_4503() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247528 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14584_exitsell16_4504() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14585_exitsell16_4505() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14586_exitsell16_4506() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14587_exitsell16_4507() { var g = Game.Solo(seed: 247432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14588_exitsell16_4508() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247533 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14589_exitsell16_4509() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14590_exitsell16_4510() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14591_exitsell16_4511() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14592_exitsell16_4512() { var g = Game.Solo(seed: 247437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14593_exitsell16_4513() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247538 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14594_exitsell16_4514() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14595_exitsell16_4515() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14596_exitsell16_4516() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14597_exitsell16_4517() { var g = Game.Solo(seed: 247442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14598_exitsell16_4518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247543 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14599_exitsell16_4519() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14600_exitsell16_4520() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14601_exitsell16_4521() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14602_exitsell16_4522() { var g = Game.Solo(seed: 247447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14603_exitsell16_4523() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247548 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14604_exitsell16_4524() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14605_exitsell16_4525() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14606_exitsell16_4526() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14607_exitsell16_4527() { var g = Game.Solo(seed: 247452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14608_exitsell16_4528() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247553 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14609_exitsell16_4529() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14610_exitsell16_4530() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14611_exitsell16_4531() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14612_exitsell16_4532() { var g = Game.Solo(seed: 247457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14613_exitsell16_4533() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247558 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14614_exitsell16_4534() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14615_exitsell16_4535() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14616_exitsell16_4536() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14617_exitsell16_4537() { var g = Game.Solo(seed: 247462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14618_exitsell16_4538() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247563 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14619_exitsell16_4539() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14620_exitsell16_4540() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14621_exitsell16_4541() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14622_exitsell16_4542() { var g = Game.Solo(seed: 247467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14623_exitsell16_4543() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 247568 })).Sim); Assert.True(c.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14624_exitsell16_4544() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14625_exitsell16_4545() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105913); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
}
