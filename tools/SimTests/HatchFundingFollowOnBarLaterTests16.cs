using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14066–14135: batch 298 funding follow-on data bar (schema 29).</summary>
public class HatchFundingFollowOnBarLaterTests16
{
    [Fact] public void Q14066_followon16_3846() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14067_followon16_3847() { var g = Game.Solo(seed: 257682); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14068_followon16_3848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 257783 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14069_followon16_3849() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14070_followon16_3850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14071_followon16_3851() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14072_followon16_3852() { var g = Game.Solo(seed: 257687); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14073_followon16_3853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 257788 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14074_followon16_3854() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14075_followon16_3855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14076_followon16_3856() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14077_followon16_3857() { var g = Game.Solo(seed: 257692); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14078_followon16_3858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 257793 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14079_followon16_3859() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14080_followon16_3860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14081_followon16_3861() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14082_followon16_3862() { var g = Game.Solo(seed: 257697); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14083_followon16_3863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 257798 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14084_followon16_3864() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14085_followon16_3865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14086_followon16_3866() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14087_followon16_3867() { var g = Game.Solo(seed: 252022); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14088_followon16_3868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252124 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14089_followon16_3869() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14090_followon16_3870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14091_followon16_3871() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14092_followon16_3872() { var g = Game.Solo(seed: 252027); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14093_followon16_3873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252129 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14094_followon16_3874() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14095_followon16_3875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14096_followon16_3876() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14097_followon16_3877() { var g = Game.Solo(seed: 252032); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14098_followon16_3878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252134 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14099_followon16_3879() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14100_followon16_3880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14101_followon16_3881() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14102_followon16_3882() { var g = Game.Solo(seed: 252037); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14103_followon16_3883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252139 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14104_followon16_3884() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14105_followon16_3885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14106_followon16_3886() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14107_followon16_3887() { var g = Game.Solo(seed: 252042); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14108_followon16_3888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252144 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14109_followon16_3889() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14110_followon16_3890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14111_followon16_3891() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14112_followon16_3892() { var g = Game.Solo(seed: 252047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14113_followon16_3893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252149 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14114_followon16_3894() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14115_followon16_3895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14116_followon16_3896() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14117_followon16_3897() { var g = Game.Solo(seed: 252052); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14118_followon16_3898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252154 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14119_followon16_3899() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14120_followon16_3900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14121_followon16_3901() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14122_followon16_3902() { var g = Game.Solo(seed: 252057); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14123_followon16_3903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252159 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14124_followon16_3904() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14125_followon16_3905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14126_followon16_3906() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14127_followon16_3907() { var g = Game.Solo(seed: 252062); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14128_followon16_3908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252164 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14129_followon16_3909() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14130_followon16_3910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
    [Fact] public void Q14131_followon16_3911() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.DownRoundBubblePermille > 0); Assert.True(c.Funding.FollowOn.DownRoundCashPermille > 0); }
    [Fact] public void Q14132_followon16_3912() { var g = Game.Solo(seed: 252067); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.BoardDramaPermillePerYear > 0); }
    [Fact] public void Q14133_followon16_3913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 252169 })).Sim); Assert.True(c.Funding.Rounds.Count >= 3); }
    [Fact] public void Q14134_followon16_3914() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FollowOn.InvestorEventPermillePerMonth > 0); Assert.True(c.Funding.FollowOn.InvestorEventBubblePermille > 0); }
    [Fact] public void Q14135_followon16_3915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 123524); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FollowOn.DownRoundExtraDilutionPermille > 0); }
}
