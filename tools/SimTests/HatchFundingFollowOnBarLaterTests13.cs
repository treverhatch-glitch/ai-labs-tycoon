using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13856–13925: batch 274 funding follow-on data bar (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests13
{
    [Fact] public void Q13856_followon13_3636() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13857_followon13_3637() { var g = Game.Solo(seed: 204882); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13858_followon13_3638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 204983 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13859_followon13_3639() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13860_followon13_3640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13861_followon13_3641() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13862_followon13_3642() { var g = Game.Solo(seed: 204887); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13863_followon13_3643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 204988 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13864_followon13_3644() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13865_followon13_3645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13866_followon13_3646() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13867_followon13_3647() { var g = Game.Solo(seed: 204892); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13868_followon13_3648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 204993 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13869_followon13_3649() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13870_followon13_3650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13871_followon13_3651() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13872_followon13_3652() { var g = Game.Solo(seed: 204897); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13873_followon13_3653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 204998 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13874_followon13_3654() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13875_followon13_3655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13876_followon13_3656() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13877_followon13_3657() { var g = Game.Solo(seed: 199222); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13878_followon13_3658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199324 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13879_followon13_3659() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13880_followon13_3660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13881_followon13_3661() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13882_followon13_3662() { var g = Game.Solo(seed: 199227); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13883_followon13_3663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199329 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13884_followon13_3664() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13885_followon13_3665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13886_followon13_3666() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13887_followon13_3667() { var g = Game.Solo(seed: 199232); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13888_followon13_3668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199334 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13889_followon13_3669() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13890_followon13_3670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13891_followon13_3671() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13892_followon13_3672() { var g = Game.Solo(seed: 199237); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13893_followon13_3673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199339 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13894_followon13_3674() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13895_followon13_3675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13896_followon13_3676() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13897_followon13_3677() { var g = Game.Solo(seed: 199242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13898_followon13_3678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199344 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13899_followon13_3679() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13900_followon13_3680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13901_followon13_3681() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13902_followon13_3682() { var g = Game.Solo(seed: 199247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13903_followon13_3683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199349 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13904_followon13_3684() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13905_followon13_3685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13906_followon13_3686() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13907_followon13_3687() { var g = Game.Solo(seed: 199252); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13908_followon13_3688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199354 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13909_followon13_3689() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13910_followon13_3690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13911_followon13_3691() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13912_followon13_3692() { var g = Game.Solo(seed: 199257); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13913_followon13_3693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199359 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13914_followon13_3694() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13915_followon13_3695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13916_followon13_3696() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13917_followon13_3697() { var g = Game.Solo(seed: 199262); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13918_followon13_3698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199364 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13919_followon13_3699() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13920_followon13_3700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q13921_followon13_3701() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q13922_followon13_3702() { var g = Game.Solo(seed: 199267); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q13923_followon13_3703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 199369 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q13924_followon13_3704() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q13925_followon13_3705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70664); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
