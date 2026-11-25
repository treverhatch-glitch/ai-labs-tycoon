using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q4336–Q4405: batch 67 capital/bubble data bar coda (schema 29).</summary>
public class HatchCapitalBubbleBarLaterTests3
{
    [Fact] public void Q4336_capital3_4336() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4337_capital3_4337() { var g = Game.Solo(seed: 5781); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4338_capital3_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5882 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4339_capital3_4339() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4340_capital3_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4341_capital3_4341() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4342_capital3_4342() { var g = Game.Solo(seed: 5786); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4343_capital3_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5887 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4344_capital3_4344() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4345_capital3_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4346_capital3_4346() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4347_capital3_4347() { var g = Game.Solo(seed: 5791); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4348_capital3_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5892 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4349_capital3_4349() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4350_capital3_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4351_capital3_4351() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4352_capital3_4352() { var g = Game.Solo(seed: 5796); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4353_capital3_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5897 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4354_capital3_4354() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4355_capital3_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4356_capital3_4356() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4357_capital3_4357() { var g = Game.Solo(seed: 5801); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4358_capital3_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5902 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4359_capital3_4359() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4360_capital3_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4361_capital3_4361() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4362_capital3_4362() { var g = Game.Solo(seed: 5806); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4363_capital3_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5907 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4364_capital3_4364() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4365_capital3_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4366_capital3_4366() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4367_capital3_4367() { var g = Game.Solo(seed: 5811); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4368_capital3_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5912 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4369_capital3_4369() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4370_capital3_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4371_capital3_4371() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4372_capital3_4372() { var g = Game.Solo(seed: 5816); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4373_capital3_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5917 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4374_capital3_4374() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4375_capital3_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4376_capital3_4376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4377_capital3_4377() { var g = Game.Solo(seed: 5821); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4378_capital3_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5922 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4379_capital3_4379() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4380_capital3_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4381_capital3_4381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4382_capital3_4382() { var g = Game.Solo(seed: 5826); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4383_capital3_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5927 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4384_capital3_4384() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4385_capital3_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4386_capital3_4386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4387_capital3_4387() { var g = Game.Solo(seed: 5831); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4388_capital3_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5932 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4389_capital3_4389() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4390_capital3_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4391_capital3_4391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4392_capital3_4392() { var g = Game.Solo(seed: 5836); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4393_capital3_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5937 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4394_capital3_4394() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4395_capital3_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4396_capital3_4396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4397_capital3_4397() { var g = Game.Solo(seed: 5841); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4398_capital3_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5942 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4399_capital3_4399() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4400_capital3_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4401_capital3_4401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q4402_capital3_4402() { var g = Game.Solo(seed: 5846); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q4403_capital3_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5947 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q4404_capital3_4404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q4405_capital3_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 33); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
