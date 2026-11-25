using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14136–14205: batch 306 funding follow-on data bar (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests17
{
    [Fact] public void Q14136_followon17_3916() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14137_followon17_3917() { var g = Game.Solo(seed: 275282); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14138_followon17_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 275383 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14139_followon17_3919() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14140_followon17_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14141_followon17_3921() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14142_followon17_3922() { var g = Game.Solo(seed: 275287); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14143_followon17_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 275388 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14144_followon17_3924() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14145_followon17_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14146_followon17_3926() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14147_followon17_3927() { var g = Game.Solo(seed: 275292); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14148_followon17_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 275393 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14149_followon17_3929() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14150_followon17_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14151_followon17_3931() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14152_followon17_3932() { var g = Game.Solo(seed: 275297); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14153_followon17_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 275398 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14154_followon17_3934() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14155_followon17_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14156_followon17_3936() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14157_followon17_3937() { var g = Game.Solo(seed: 269622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14158_followon17_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269724 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14159_followon17_3939() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14160_followon17_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14161_followon17_3941() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14162_followon17_3942() { var g = Game.Solo(seed: 269627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14163_followon17_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269729 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14164_followon17_3944() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14165_followon17_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14166_followon17_3946() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14167_followon17_3947() { var g = Game.Solo(seed: 269632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14168_followon17_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269734 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14169_followon17_3949() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14170_followon17_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14171_followon17_3951() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14172_followon17_3952() { var g = Game.Solo(seed: 269637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14173_followon17_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269739 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14174_followon17_3954() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14175_followon17_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14176_followon17_3956() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14177_followon17_3957() { var g = Game.Solo(seed: 269642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14178_followon17_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269744 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14179_followon17_3959() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14180_followon17_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14181_followon17_3961() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14182_followon17_3962() { var g = Game.Solo(seed: 269647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14183_followon17_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269749 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14184_followon17_3964() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14185_followon17_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14186_followon17_3966() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14187_followon17_3967() { var g = Game.Solo(seed: 269652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14188_followon17_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269754 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14189_followon17_3969() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14190_followon17_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14191_followon17_3971() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14192_followon17_3972() { var g = Game.Solo(seed: 269657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14193_followon17_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269759 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14194_followon17_3974() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14195_followon17_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14196_followon17_3976() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14197_followon17_3977() { var g = Game.Solo(seed: 269662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14198_followon17_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269764 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14199_followon17_3979() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14200_followon17_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14201_followon17_3981() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14202_followon17_3982() { var g = Game.Solo(seed: 269667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14203_followon17_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 269769 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14204_followon17_3984() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14205_followon17_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
