using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18056+: scen395.</summary>
public class HatchScenariosBarLaterTests58
{
    [Fact] public void Q18056_scen395_5000() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18057_scen395_5001() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18058_scen395_5002() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18059_scen395_5003() { var g = Game.Solo(seed: 397053); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18060_scen395_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397054 })).Sim); }
    [Fact] public void Q18061_scen395_5005() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18062_scen395_5006() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18063_scen395_5007() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18064_scen395_5008() { var g = Game.Solo(seed: 397058); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18065_scen395_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397059 })).Sim); }
    [Fact] public void Q18066_scen395_5010() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18067_scen395_5011() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18068_scen395_5012() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18069_scen395_5013() { var g = Game.Solo(seed: 397063); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18070_scen395_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397064 })).Sim); }
    [Fact] public void Q18071_scen395_5015() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18072_scen395_5016() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18073_scen395_5017() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18074_scen395_5018() { var g = Game.Solo(seed: 397068); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18075_scen395_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397069 })).Sim); }
    [Fact] public void Q18076_scen395_5020() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18077_scen395_5021() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18078_scen395_5022() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18079_scen395_5023() { var g = Game.Solo(seed: 397073); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18080_scen395_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397074 })).Sim); }
    [Fact] public void Q18081_scen395_5025() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18082_scen395_5026() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18083_scen395_5027() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18084_scen395_5028() { var g = Game.Solo(seed: 397078); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18085_scen395_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397079 })).Sim); }
    [Fact] public void Q18086_scen395_5030() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18087_scen395_5031() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18088_scen395_5032() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18089_scen395_5033() { var g = Game.Solo(seed: 397083); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18090_scen395_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397084 })).Sim); }
    [Fact] public void Q18091_scen395_5035() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18092_scen395_5036() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18093_scen395_5037() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18094_scen395_5038() { var g = Game.Solo(seed: 397088); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18095_scen395_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397089 })).Sim); }
    [Fact] public void Q18096_scen395_5040() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18097_scen395_5041() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18098_scen395_5042() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18099_scen395_5043() { var g = Game.Solo(seed: 397093); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18100_scen395_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397094 })).Sim); }
    [Fact] public void Q18101_scen395_5045() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18102_scen395_5046() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18103_scen395_5047() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18104_scen395_5048() { var g = Game.Solo(seed: 397098); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18105_scen395_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397099 })).Sim); }
    [Fact] public void Q18106_scen395_5050() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18107_scen395_5051() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18108_scen395_5052() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18109_scen395_5053() { var g = Game.Solo(seed: 397103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18110_scen395_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397104 })).Sim); }
    [Fact] public void Q18111_scen395_5055() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18112_scen395_5056() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18113_scen395_5057() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18114_scen395_5058() { var g = Game.Solo(seed: 397108); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18115_scen395_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397109 })).Sim); }
    [Fact] public void Q18116_scen395_5060() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18117_scen395_5061() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18118_scen395_5062() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18119_scen395_5063() { var g = Game.Solo(seed: 397113); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18120_scen395_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397114 })).Sim); }
    [Fact] public void Q18121_scen395_5065() { GameConfig c = TestData.Config(); Assert.True(c.Scenarios.Scenarios.Count >= 7); }
    [Fact] public void Q18122_scen395_5066() { GameConfig c = TestData.Config(); Assert.True(AiLabs.Rules.Systems.Scenarios.List(c).Count >= 7); }
    [Fact] public void Q18123_scen395_5067() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18124_scen395_5068() { var g = Game.Solo(seed: 397118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18125_scen395_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397119 })).Sim); }
}
