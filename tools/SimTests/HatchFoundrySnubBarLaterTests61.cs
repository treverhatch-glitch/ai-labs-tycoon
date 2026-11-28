using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18266+: snub398.</summary>
public class HatchFoundrySnubBarLaterTests61
{
    [Fact] public void Q18266_snub398_5000() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18267_snub398_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18268_snub398_5002() { var g = Game.Solo(seed: 397262); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18269_snub398_5003() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18270_snub398_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397264 })).Sim); }
    [Fact] public void Q18271_snub398_5005() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18272_snub398_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18273_snub398_5007() { var g = Game.Solo(seed: 397267); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18274_snub398_5008() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18275_snub398_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397269 })).Sim); }
    [Fact] public void Q18276_snub398_5010() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18277_snub398_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18278_snub398_5012() { var g = Game.Solo(seed: 397272); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18279_snub398_5013() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18280_snub398_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397274 })).Sim); }
    [Fact] public void Q18281_snub398_5015() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18282_snub398_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18283_snub398_5017() { var g = Game.Solo(seed: 397277); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18284_snub398_5018() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18285_snub398_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397279 })).Sim); }
    [Fact] public void Q18286_snub398_5020() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18287_snub398_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18288_snub398_5022() { var g = Game.Solo(seed: 397282); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18289_snub398_5023() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18290_snub398_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397284 })).Sim); }
    [Fact] public void Q18291_snub398_5025() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18292_snub398_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18293_snub398_5027() { var g = Game.Solo(seed: 397287); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18294_snub398_5028() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18295_snub398_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397289 })).Sim); }
    [Fact] public void Q18296_snub398_5030() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18297_snub398_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18298_snub398_5032() { var g = Game.Solo(seed: 397292); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18299_snub398_5033() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18300_snub398_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397294 })).Sim); }
    [Fact] public void Q18301_snub398_5035() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18302_snub398_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18303_snub398_5037() { var g = Game.Solo(seed: 397297); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18304_snub398_5038() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18305_snub398_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397299 })).Sim); }
    [Fact] public void Q18306_snub398_5040() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18307_snub398_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18308_snub398_5042() { var g = Game.Solo(seed: 397302); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18309_snub398_5043() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18310_snub398_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397304 })).Sim); }
    [Fact] public void Q18311_snub398_5045() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18312_snub398_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18313_snub398_5047() { var g = Game.Solo(seed: 397307); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18314_snub398_5048() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18315_snub398_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397309 })).Sim); }
    [Fact] public void Q18316_snub398_5050() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18317_snub398_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18318_snub398_5052() { var g = Game.Solo(seed: 397312); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18319_snub398_5053() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18320_snub398_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397314 })).Sim); }
    [Fact] public void Q18321_snub398_5055() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18322_snub398_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18323_snub398_5057() { var g = Game.Solo(seed: 397317); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18324_snub398_5058() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18325_snub398_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397319 })).Sim); }
    [Fact] public void Q18326_snub398_5060() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18327_snub398_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18328_snub398_5062() { var g = Game.Solo(seed: 397322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18329_snub398_5063() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18330_snub398_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397324 })).Sim); }
    [Fact] public void Q18331_snub398_5065() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubChancePermille > 0); }
    [Fact] public void Q18332_snub398_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18333_snub398_5067() { var g = Game.Solo(seed: 397327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18334_snub398_5068() { GameConfig c = TestData.Config(); Assert.True(c.Chips.SnubStandingBelow > 0); }
    [Fact] public void Q18335_snub398_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397329 })).Sim); }
}
