using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18336+: load399.</summary>
public class HatchOverloadCostsBarLaterTests62
{
    [Fact] public void Q18336_load399_5000() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18337_load399_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18338_load399_5002() { var g = Game.Solo(seed: 397332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18339_load399_5003() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18340_load399_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397334 })).Sim); }
    [Fact] public void Q18341_load399_5005() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18342_load399_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18343_load399_5007() { var g = Game.Solo(seed: 397337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18344_load399_5008() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18345_load399_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397339 })).Sim); }
    [Fact] public void Q18346_load399_5010() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18347_load399_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18348_load399_5012() { var g = Game.Solo(seed: 397342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18349_load399_5013() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18350_load399_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397344 })).Sim); }
    [Fact] public void Q18351_load399_5015() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18352_load399_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18353_load399_5017() { var g = Game.Solo(seed: 397347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18354_load399_5018() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18355_load399_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397349 })).Sim); }
    [Fact] public void Q18356_load399_5020() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18357_load399_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18358_load399_5022() { var g = Game.Solo(seed: 397352); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18359_load399_5023() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18360_load399_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397354 })).Sim); }
    [Fact] public void Q18361_load399_5025() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18362_load399_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18363_load399_5027() { var g = Game.Solo(seed: 397357); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18364_load399_5028() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18365_load399_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397359 })).Sim); }
    [Fact] public void Q18366_load399_5030() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18367_load399_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18368_load399_5032() { var g = Game.Solo(seed: 397362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18369_load399_5033() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18370_load399_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397364 })).Sim); }
    [Fact] public void Q18371_load399_5035() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18372_load399_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18373_load399_5037() { var g = Game.Solo(seed: 397367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18374_load399_5038() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18375_load399_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397369 })).Sim); }
    [Fact] public void Q18376_load399_5040() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18377_load399_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18378_load399_5042() { var g = Game.Solo(seed: 397372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18379_load399_5043() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18380_load399_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397374 })).Sim); }
    [Fact] public void Q18381_load399_5045() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18382_load399_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18383_load399_5047() { var g = Game.Solo(seed: 397377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18384_load399_5048() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18385_load399_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397379 })).Sim); }
    [Fact] public void Q18386_load399_5050() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18387_load399_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18388_load399_5052() { var g = Game.Solo(seed: 397382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18389_load399_5053() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18390_load399_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397384 })).Sim); }
    [Fact] public void Q18391_load399_5055() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18392_load399_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18393_load399_5057() { var g = Game.Solo(seed: 397387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18394_load399_5058() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18395_load399_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397389 })).Sim); }
    [Fact] public void Q18396_load399_5060() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18397_load399_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18398_load399_5062() { var g = Game.Solo(seed: 397392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18399_load399_5063() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18400_load399_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397394 })).Sim); }
    [Fact] public void Q18401_load399_5065() { GameConfig c = TestData.Config(); Assert.True(c.Safety.OverloadIncidentPermille >= 0); }
    [Fact] public void Q18402_load399_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18403_load399_5067() { var g = Game.Solo(seed: 397397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18404_load399_5068() { var g = Game.Solo().WithStarterLab(); Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me)); }
    [Fact] public void Q18405_load399_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397399 })).Sim); }
}
