using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14276–14345: batch 322 funding follow-on data bar (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests19
{
    [Fact] public void Q14276_followon19_4056() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14277_followon19_4057() { var g = Game.Solo(seed: 310482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14278_followon19_4058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 310583 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14279_followon19_4059() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14280_followon19_4060() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14281_followon19_4061() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14282_followon19_4062() { var g = Game.Solo(seed: 310487); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14283_followon19_4063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 310588 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14284_followon19_4064() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14285_followon19_4065() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14286_followon19_4066() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14287_followon19_4067() { var g = Game.Solo(seed: 310492); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14288_followon19_4068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 310593 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14289_followon19_4069() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14290_followon19_4070() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14291_followon19_4071() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14292_followon19_4072() { var g = Game.Solo(seed: 310497); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14293_followon19_4073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 310598 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14294_followon19_4074() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14295_followon19_4075() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14296_followon19_4076() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14297_followon19_4077() { var g = Game.Solo(seed: 304822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14298_followon19_4078() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304924 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14299_followon19_4079() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14300_followon19_4080() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14301_followon19_4081() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14302_followon19_4082() { var g = Game.Solo(seed: 304827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14303_followon19_4083() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304929 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14304_followon19_4084() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14305_followon19_4085() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14306_followon19_4086() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14307_followon19_4087() { var g = Game.Solo(seed: 304832); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14308_followon19_4088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304934 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14309_followon19_4089() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14310_followon19_4090() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14311_followon19_4091() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14312_followon19_4092() { var g = Game.Solo(seed: 304837); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14313_followon19_4093() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304939 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14314_followon19_4094() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14315_followon19_4095() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14316_followon19_4096() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14317_followon19_4097() { var g = Game.Solo(seed: 304842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14318_followon19_4098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304944 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14319_followon19_4099() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14320_followon19_4100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14321_followon19_4101() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14322_followon19_4102() { var g = Game.Solo(seed: 304847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14323_followon19_4103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304949 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14324_followon19_4104() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14325_followon19_4105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14326_followon19_4106() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14327_followon19_4107() { var g = Game.Solo(seed: 304852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14328_followon19_4108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304954 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14329_followon19_4109() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14330_followon19_4110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14331_followon19_4111() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14332_followon19_4112() { var g = Game.Solo(seed: 304857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14333_followon19_4113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304959 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14334_followon19_4114() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14335_followon19_4115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14336_followon19_4116() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14337_followon19_4117() { var g = Game.Solo(seed: 304862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14338_followon19_4118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304964 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14339_followon19_4119() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14340_followon19_4120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14341_followon19_4121() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14342_followon19_4122() { var g = Game.Solo(seed: 304867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14343_followon19_4123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 304969 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14344_followon19_4124() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14345_followon19_4125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176384); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
