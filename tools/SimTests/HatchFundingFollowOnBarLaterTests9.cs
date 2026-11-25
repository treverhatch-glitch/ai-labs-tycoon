using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6496–6565: batch 202 funding follow-on data bar coda (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests9
{
    [Fact] public void Q13576_followon9_3356() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13577_followon9_3357() { var g = Game.Solo(seed: 134482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13578_followon9_3358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 134583 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13579_followon9_3359() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13580_followon9_3360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13581_followon9_3361() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13582_followon9_3362() { var g = Game.Solo(seed: 134487); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13583_followon9_3363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 134588 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13584_followon9_3364() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13585_followon9_3365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13586_followon9_3366() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13587_followon9_3367() { var g = Game.Solo(seed: 134492); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13588_followon9_3368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 134593 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13589_followon9_3369() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13590_followon9_3370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13591_followon9_3371() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13592_followon9_3372() { var g = Game.Solo(seed: 134497); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13593_followon9_3373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 134598 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13594_followon9_3374() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13595_followon9_3375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13596_followon9_3376() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13597_followon9_3377() { var g = Game.Solo(seed: 128822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13598_followon9_3378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128924 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13599_followon9_3379() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13600_followon9_3380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13601_followon9_3381() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13602_followon9_3382() { var g = Game.Solo(seed: 128827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13603_followon9_3383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128929 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13604_followon9_3384() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13605_followon9_3385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13606_followon9_3386() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13607_followon9_3387() { var g = Game.Solo(seed: 128832); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13608_followon9_3388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128934 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13609_followon9_3389() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13610_followon9_3390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13611_followon9_3391() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13612_followon9_3392() { var g = Game.Solo(seed: 128837); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13613_followon9_3393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128939 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13614_followon9_3394() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13615_followon9_3395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13616_followon9_3396() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13617_followon9_3397() { var g = Game.Solo(seed: 128842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13618_followon9_3398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128944 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13619_followon9_3399() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13620_followon9_3400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13621_followon9_3401() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13622_followon9_3402() { var g = Game.Solo(seed: 128847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13623_followon9_3403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128949 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13624_followon9_3404() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13625_followon9_3405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13626_followon9_3406() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13627_followon9_3407() { var g = Game.Solo(seed: 128852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13628_followon9_3408() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128954 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13629_followon9_3409() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13630_followon9_3410() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13631_followon9_3411() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13632_followon9_3412() { var g = Game.Solo(seed: 128857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13633_followon9_3413() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128959 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13634_followon9_3414() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13635_followon9_3415() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13636_followon9_3416() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13637_followon9_3417() { var g = Game.Solo(seed: 128862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13638_followon9_3418() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128964 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13639_followon9_3419() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13640_followon9_3420() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13641_followon9_3421() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13642_followon9_3422() { var g = Game.Solo(seed: 128867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13643_followon9_3423() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128969 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13644_followon9_3424() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13645_followon9_3425() { GameConfig c = TestData.Config(); var g = Game.New(seed: 184); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
