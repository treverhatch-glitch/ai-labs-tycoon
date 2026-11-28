using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18196+: office397.</summary>
public class HatchOfficeEventsR3BarLaterTests60
{
    [Fact] public void Q18196_office397_5000() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18197_office397_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18198_office397_5002() { var g = Game.Solo(seed: 397192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18199_office397_5003() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18200_office397_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397194 })).Sim); }
    [Fact] public void Q18201_office397_5005() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18202_office397_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18203_office397_5007() { var g = Game.Solo(seed: 397197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18204_office397_5008() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18205_office397_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397199 })).Sim); }
    [Fact] public void Q18206_office397_5010() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18207_office397_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18208_office397_5012() { var g = Game.Solo(seed: 397202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18209_office397_5013() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18210_office397_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397204 })).Sim); }
    [Fact] public void Q18211_office397_5015() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18212_office397_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18213_office397_5017() { var g = Game.Solo(seed: 397207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18214_office397_5018() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18215_office397_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397209 })).Sim); }
    [Fact] public void Q18216_office397_5020() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18217_office397_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18218_office397_5022() { var g = Game.Solo(seed: 397212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18219_office397_5023() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18220_office397_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397214 })).Sim); }
    [Fact] public void Q18221_office397_5025() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18222_office397_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18223_office397_5027() { var g = Game.Solo(seed: 397217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18224_office397_5028() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18225_office397_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397219 })).Sim); }
    [Fact] public void Q18226_office397_5030() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18227_office397_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18228_office397_5032() { var g = Game.Solo(seed: 397222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18229_office397_5033() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18230_office397_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397224 })).Sim); }
    [Fact] public void Q18231_office397_5035() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18232_office397_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18233_office397_5037() { var g = Game.Solo(seed: 397227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18234_office397_5038() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18235_office397_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397229 })).Sim); }
    [Fact] public void Q18236_office397_5040() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18237_office397_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18238_office397_5042() { var g = Game.Solo(seed: 397232); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18239_office397_5043() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18240_office397_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397234 })).Sim); }
    [Fact] public void Q18241_office397_5045() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18242_office397_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18243_office397_5047() { var g = Game.Solo(seed: 397237); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18244_office397_5048() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18245_office397_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397239 })).Sim); }
    [Fact] public void Q18246_office397_5050() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18247_office397_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18248_office397_5052() { var g = Game.Solo(seed: 397242); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18249_office397_5053() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18250_office397_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397244 })).Sim); }
    [Fact] public void Q18251_office397_5055() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18252_office397_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18253_office397_5057() { var g = Game.Solo(seed: 397247); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18254_office397_5058() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18255_office397_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397249 })).Sim); }
    [Fact] public void Q18256_office397_5060() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18257_office397_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18258_office397_5062() { var g = Game.Solo(seed: 397252); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18259_office397_5063() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18260_office397_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397254 })).Sim); }
    [Fact] public void Q18261_office397_5065() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.IdValue == "researcher-leaves"); }
    [Fact] public void Q18262_office397_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18263_office397_5067() { var g = Game.Solo(seed: 397257); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18264_office397_5068() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 10); }
    [Fact] public void Q18265_office397_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397259 })).Sim); }
}
