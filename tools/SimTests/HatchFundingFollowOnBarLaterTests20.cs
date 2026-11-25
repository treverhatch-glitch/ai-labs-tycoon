using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14346–14415: batch 330 funding follow-on data bar (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests20
{
    [Fact] public void Q14346_followon20_4126() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14347_followon20_4127() { var g = Game.Solo(seed: 328082); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14348_followon20_4128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 328183 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14349_followon20_4129() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14350_followon20_4130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14351_followon20_4131() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14352_followon20_4132() { var g = Game.Solo(seed: 328087); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14353_followon20_4133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 328188 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14354_followon20_4134() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14355_followon20_4135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14356_followon20_4136() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14357_followon20_4137() { var g = Game.Solo(seed: 328092); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14358_followon20_4138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 328193 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14359_followon20_4139() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14360_followon20_4140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14361_followon20_4141() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14362_followon20_4142() { var g = Game.Solo(seed: 328097); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14363_followon20_4143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 328198 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14364_followon20_4144() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14365_followon20_4145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14366_followon20_4146() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14367_followon20_4147() { var g = Game.Solo(seed: 322422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14368_followon20_4148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322524 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14369_followon20_4149() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14370_followon20_4150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14371_followon20_4151() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14372_followon20_4152() { var g = Game.Solo(seed: 322427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14373_followon20_4153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322529 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14374_followon20_4154() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14375_followon20_4155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14376_followon20_4156() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14377_followon20_4157() { var g = Game.Solo(seed: 322432); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14378_followon20_4158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322534 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14379_followon20_4159() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14380_followon20_4160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14381_followon20_4161() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14382_followon20_4162() { var g = Game.Solo(seed: 322437); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14383_followon20_4163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322539 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14384_followon20_4164() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14385_followon20_4165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14386_followon20_4166() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14387_followon20_4167() { var g = Game.Solo(seed: 322442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14388_followon20_4168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322544 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14389_followon20_4169() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14390_followon20_4170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14391_followon20_4171() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14392_followon20_4172() { var g = Game.Solo(seed: 322447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14393_followon20_4173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322549 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14394_followon20_4174() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14395_followon20_4175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14396_followon20_4176() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14397_followon20_4177() { var g = Game.Solo(seed: 322452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14398_followon20_4178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322554 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14399_followon20_4179() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14400_followon20_4180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14401_followon20_4181() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14402_followon20_4182() { var g = Game.Solo(seed: 322457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14403_followon20_4183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322559 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14404_followon20_4184() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14405_followon20_4185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14406_followon20_4186() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14407_followon20_4187() { var g = Game.Solo(seed: 322462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14408_followon20_4188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322564 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14409_followon20_4189() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14410_followon20_4190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14411_followon20_4191() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14412_followon20_4192() { var g = Game.Solo(seed: 322467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14413_followon20_4193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 322569 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14414_followon20_4194() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14415_followon20_4195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194004); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
