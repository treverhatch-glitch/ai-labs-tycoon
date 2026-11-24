using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6016–6085: batch 109 capital/bubble data bar coda (schema 29).</summary>
public class HatchCapitalBubbleBarLaterTests7
{
    [Fact] public void Q7266_capital7_4336() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7267_capital7_4337() { var g = Game.Solo(seed: 47062); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7268_capital7_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47163 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7269_capital7_4339() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7270_capital7_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7271_capital7_4341() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7272_capital7_4342() { var g = Game.Solo(seed: 47067); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7273_capital7_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47168 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7274_capital7_4344() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7275_capital7_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7276_capital7_4346() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7277_capital7_4347() { var g = Game.Solo(seed: 47072); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7278_capital7_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47173 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7279_capital7_4349() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7280_capital7_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7281_capital7_4351() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7282_capital7_4352() { var g = Game.Solo(seed: 47077); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7283_capital7_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47178 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7284_capital7_4354() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7285_capital7_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7286_capital7_4356() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7287_capital7_4357() { var g = Game.Solo(seed: 47082); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7288_capital7_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47183 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7289_capital7_4359() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7290_capital7_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7291_capital7_4361() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7292_capital7_4362() { var g = Game.Solo(seed: 47087); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7293_capital7_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47188 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7294_capital7_4364() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7295_capital7_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7296_capital7_4366() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7297_capital7_4367() { var g = Game.Solo(seed: 47092); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7298_capital7_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47193 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7299_capital7_4369() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7300_capital7_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7301_capital7_4371() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7302_capital7_4372() { var g = Game.Solo(seed: 47097); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7303_capital7_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47198 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7304_capital7_4374() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7305_capital7_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7306_capital7_4376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7307_capital7_4377() { var g = Game.Solo(seed: 47102); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7308_capital7_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47203 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7309_capital7_4379() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7310_capital7_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7311_capital7_4381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7312_capital7_4382() { var g = Game.Solo(seed: 47107); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7313_capital7_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47208 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7314_capital7_4384() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7315_capital7_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7316_capital7_4386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7317_capital7_4387() { var g = Game.Solo(seed: 47112); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7318_capital7_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47213 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7319_capital7_4389() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7320_capital7_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7321_capital7_4391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7322_capital7_4392() { var g = Game.Solo(seed: 47117); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7323_capital7_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47218 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7324_capital7_4394() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7325_capital7_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7326_capital7_4396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7327_capital7_4397() { var g = Game.Solo(seed: 47122); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7328_capital7_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47223 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7329_capital7_4399() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7330_capital7_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7331_capital7_4401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q7332_capital7_4402() { var g = Game.Solo(seed: 47127); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q7333_capital7_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 47228 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q7334_capital7_4404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7335_capital7_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 91); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
