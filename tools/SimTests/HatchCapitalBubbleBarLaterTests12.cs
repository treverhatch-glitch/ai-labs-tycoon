using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6016–6085: batch 209 capital/bubble data bar coda (schema 29).</summary>
public class HatchCapitalBubbleBarLaterTests12
{
    [Fact] public void Q14056_capital12_4336() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14057_capital12_4337() { var g = Game.Solo(seed: 140042); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14058_capital12_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140143 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14059_capital12_4339() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14060_capital12_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14061_capital12_4341() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14062_capital12_4342() { var g = Game.Solo(seed: 140047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14063_capital12_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140148 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14064_capital12_4344() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14065_capital12_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14066_capital12_4346() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14067_capital12_4347() { var g = Game.Solo(seed: 140052); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14068_capital12_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140153 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14069_capital12_4349() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14070_capital12_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14071_capital12_4351() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14072_capital12_4352() { var g = Game.Solo(seed: 140057); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14073_capital12_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140158 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14074_capital12_4354() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14075_capital12_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14076_capital12_4356() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14077_capital12_4357() { var g = Game.Solo(seed: 140062); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14078_capital12_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140163 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14079_capital12_4359() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14080_capital12_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14081_capital12_4361() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14082_capital12_4362() { var g = Game.Solo(seed: 140067); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14083_capital12_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140168 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14084_capital12_4364() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14085_capital12_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14086_capital12_4366() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14087_capital12_4367() { var g = Game.Solo(seed: 140072); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14088_capital12_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140173 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14089_capital12_4369() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14090_capital12_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14091_capital12_4371() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14092_capital12_4372() { var g = Game.Solo(seed: 140077); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14093_capital12_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140178 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14094_capital12_4374() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14095_capital12_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14096_capital12_4376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14097_capital12_4377() { var g = Game.Solo(seed: 140082); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14098_capital12_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140183 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14099_capital12_4379() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14100_capital12_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14101_capital12_4381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14102_capital12_4382() { var g = Game.Solo(seed: 140087); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14103_capital12_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140188 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14104_capital12_4384() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14105_capital12_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14106_capital12_4386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14107_capital12_4387() { var g = Game.Solo(seed: 140092); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14108_capital12_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140193 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14109_capital12_4389() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14110_capital12_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14111_capital12_4391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14112_capital12_4392() { var g = Game.Solo(seed: 140097); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14113_capital12_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140198 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14114_capital12_4394() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14115_capital12_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14116_capital12_4396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14117_capital12_4397() { var g = Game.Solo(seed: 140102); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14118_capital12_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140203 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14119_capital12_4399() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14120_capital12_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14121_capital12_4401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q14122_capital12_4402() { var g = Game.Solo(seed: 140107); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q14123_capital12_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 140208 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q14124_capital12_4404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14125_capital12_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 191); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
