using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17986+: esc394.</summary>
public class HatchEscapeCrisisBarLaterTests57
{
    [Fact] public void Q17986_esc394_5000() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q17987_esc394_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17988_esc394_5002() { var g = Game.Solo(seed: 396982); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17989_esc394_5003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396983 })).Sim); }
    [Fact] public void Q17990_esc394_5004() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q17991_esc394_5005() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q17992_esc394_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17993_esc394_5007() { var g = Game.Solo(seed: 396987); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17994_esc394_5008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396988 })).Sim); }
    [Fact] public void Q17995_esc394_5009() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q17996_esc394_5010() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q17997_esc394_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17998_esc394_5012() { var g = Game.Solo(seed: 396992); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17999_esc394_5013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396993 })).Sim); }
    [Fact] public void Q18000_esc394_5014() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18001_esc394_5015() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18002_esc394_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18003_esc394_5017() { var g = Game.Solo(seed: 396997); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18004_esc394_5018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396998 })).Sim); }
    [Fact] public void Q18005_esc394_5019() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18006_esc394_5020() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18007_esc394_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18008_esc394_5022() { var g = Game.Solo(seed: 397002); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18009_esc394_5023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397003 })).Sim); }
    [Fact] public void Q18010_esc394_5024() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18011_esc394_5025() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18012_esc394_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18013_esc394_5027() { var g = Game.Solo(seed: 397007); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18014_esc394_5028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397008 })).Sim); }
    [Fact] public void Q18015_esc394_5029() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18016_esc394_5030() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18017_esc394_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18018_esc394_5032() { var g = Game.Solo(seed: 397012); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18019_esc394_5033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397013 })).Sim); }
    [Fact] public void Q18020_esc394_5034() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18021_esc394_5035() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18022_esc394_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18023_esc394_5037() { var g = Game.Solo(seed: 397017); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18024_esc394_5038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397018 })).Sim); }
    [Fact] public void Q18025_esc394_5039() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18026_esc394_5040() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18027_esc394_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18028_esc394_5042() { var g = Game.Solo(seed: 397022); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18029_esc394_5043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397023 })).Sim); }
    [Fact] public void Q18030_esc394_5044() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18031_esc394_5045() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18032_esc394_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18033_esc394_5047() { var g = Game.Solo(seed: 397027); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18034_esc394_5048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397028 })).Sim); }
    [Fact] public void Q18035_esc394_5049() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18036_esc394_5050() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18037_esc394_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18038_esc394_5052() { var g = Game.Solo(seed: 397032); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18039_esc394_5053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397033 })).Sim); }
    [Fact] public void Q18040_esc394_5054() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18041_esc394_5055() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18042_esc394_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18043_esc394_5057() { var g = Game.Solo(seed: 397037); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18044_esc394_5058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397038 })).Sim); }
    [Fact] public void Q18045_esc394_5059() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18046_esc394_5060() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18047_esc394_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18048_esc394_5062() { var g = Game.Solo(seed: 397042); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18049_esc394_5063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397043 })).Sim); }
    [Fact] public void Q18050_esc394_5064() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
    [Fact] public void Q18051_esc394_5065() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape); }
    [Fact] public void Q18052_esc394_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18053_esc394_5067() { var g = Game.Solo(seed: 397047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18054_esc394_5068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397048 })).Sim); }
    [Fact] public void Q18055_esc394_5069() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentChancePermille >= 0); }
}
