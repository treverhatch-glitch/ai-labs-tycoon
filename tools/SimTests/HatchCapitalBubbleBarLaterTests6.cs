using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6016–6085: batch 91 capital/bubble data bar coda (schema 29).</summary>
public class HatchCapitalBubbleBarLaterTests6
{
    [Fact] public void Q6016_capital6_4336() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6017_capital6_4337() { var g = Game.Solo(seed: 29402); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6018_capital6_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29503 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6019_capital6_4339() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6020_capital6_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6021_capital6_4341() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6022_capital6_4342() { var g = Game.Solo(seed: 29407); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6023_capital6_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29508 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6024_capital6_4344() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6025_capital6_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6026_capital6_4346() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6027_capital6_4347() { var g = Game.Solo(seed: 29412); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6028_capital6_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29513 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6029_capital6_4349() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6030_capital6_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6031_capital6_4351() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6032_capital6_4352() { var g = Game.Solo(seed: 29417); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6033_capital6_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29518 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6034_capital6_4354() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6035_capital6_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6036_capital6_4356() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6037_capital6_4357() { var g = Game.Solo(seed: 29422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6038_capital6_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29523 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6039_capital6_4359() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6040_capital6_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6041_capital6_4361() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6042_capital6_4362() { var g = Game.Solo(seed: 29427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6043_capital6_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29528 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6044_capital6_4364() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6045_capital6_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6046_capital6_4366() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6047_capital6_4367() { var g = Game.Solo(seed: 29432); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6048_capital6_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29533 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6049_capital6_4369() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6050_capital6_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6051_capital6_4371() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6052_capital6_4372() { var g = Game.Solo(seed: 29437); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6053_capital6_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29538 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6054_capital6_4374() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6055_capital6_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6056_capital6_4376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6057_capital6_4377() { var g = Game.Solo(seed: 29442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6058_capital6_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29543 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6059_capital6_4379() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6060_capital6_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6061_capital6_4381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6062_capital6_4382() { var g = Game.Solo(seed: 29447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6063_capital6_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29548 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6064_capital6_4384() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6065_capital6_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6066_capital6_4386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6067_capital6_4387() { var g = Game.Solo(seed: 29452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6068_capital6_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29553 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6069_capital6_4389() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6070_capital6_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6071_capital6_4391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6072_capital6_4392() { var g = Game.Solo(seed: 29457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6073_capital6_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29558 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6074_capital6_4394() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6075_capital6_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6076_capital6_4396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6077_capital6_4397() { var g = Game.Solo(seed: 29462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6078_capital6_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29563 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6079_capital6_4399() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6080_capital6_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6081_capital6_4401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q6082_capital6_4402() { var g = Game.Solo(seed: 29467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q6083_capital6_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 29568 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q6084_capital6_4404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q6085_capital6_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 73); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
