using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6496–6565: batch 162 funding follow-on data bar coda (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests7
{
    [Fact] public void Q10976_followon7_3356() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q10977_followon7_3357() { var g = Game.Solo(seed: 98882); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q10978_followon7_3358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 98983 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q10979_followon7_3359() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q10980_followon7_3360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q10981_followon7_3361() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q10982_followon7_3362() { var g = Game.Solo(seed: 98887); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q10983_followon7_3363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 98988 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q10984_followon7_3364() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q10985_followon7_3365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q10986_followon7_3366() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q10987_followon7_3367() { var g = Game.Solo(seed: 98892); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q10988_followon7_3368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 98993 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q10989_followon7_3369() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q10990_followon7_3370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q10991_followon7_3371() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q10992_followon7_3372() { var g = Game.Solo(seed: 98897); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q10993_followon7_3373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 98998 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q10994_followon7_3374() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q10995_followon7_3375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q10996_followon7_3376() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q10997_followon7_3377() { var g = Game.Solo(seed: 93222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q10998_followon7_3378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93324 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q10999_followon7_3379() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11000_followon7_3380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11001_followon7_3381() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11002_followon7_3382() { var g = Game.Solo(seed: 93227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11003_followon7_3383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93329 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11004_followon7_3384() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11005_followon7_3385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11006_followon7_3386() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11007_followon7_3387() { var g = Game.Solo(seed: 93232); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11008_followon7_3388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93334 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11009_followon7_3389() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11010_followon7_3390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11011_followon7_3391() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11012_followon7_3392() { var g = Game.Solo(seed: 93237); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11013_followon7_3393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93339 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11014_followon7_3394() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11015_followon7_3395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11016_followon7_3396() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11017_followon7_3397() { var g = Game.Solo(seed: 93242); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11018_followon7_3398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93344 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11019_followon7_3399() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11020_followon7_3400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11021_followon7_3401() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11022_followon7_3402() { var g = Game.Solo(seed: 93247); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11023_followon7_3403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93349 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11024_followon7_3404() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11025_followon7_3405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11026_followon7_3406() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11027_followon7_3407() { var g = Game.Solo(seed: 93252); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11028_followon7_3408() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93354 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11029_followon7_3409() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11030_followon7_3410() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11031_followon7_3411() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11032_followon7_3412() { var g = Game.Solo(seed: 93257); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11033_followon7_3413() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93359 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11034_followon7_3414() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11035_followon7_3415() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11036_followon7_3416() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11037_followon7_3417() { var g = Game.Solo(seed: 93262); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11038_followon7_3418() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93364 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11039_followon7_3419() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11040_followon7_3420() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q11041_followon7_3421() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q11042_followon7_3422() { var g = Game.Solo(seed: 93267); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q11043_followon7_3423() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 93369 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q11044_followon7_3424() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q11045_followon7_3425() { GameConfig c = TestData.Config(); var g = Game.New(seed: 144); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
