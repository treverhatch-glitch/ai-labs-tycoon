using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18406+: rel400.</summary>
public class HatchReleaseNotesBarLaterTests63
{
    [Fact] public void Q18406_rel400_5000() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18407_rel400_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18408_rel400_5002() { var g = Game.Solo(seed: 397402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18409_rel400_5003() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18410_rel400_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397404 })).Sim); }
    [Fact] public void Q18411_rel400_5005() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18412_rel400_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18413_rel400_5007() { var g = Game.Solo(seed: 397407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18414_rel400_5008() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18415_rel400_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397409 })).Sim); }
    [Fact] public void Q18416_rel400_5010() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18417_rel400_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18418_rel400_5012() { var g = Game.Solo(seed: 397412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18419_rel400_5013() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18420_rel400_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397414 })).Sim); }
    [Fact] public void Q18421_rel400_5015() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18422_rel400_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18423_rel400_5017() { var g = Game.Solo(seed: 397417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18424_rel400_5018() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18425_rel400_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397419 })).Sim); }
    [Fact] public void Q18426_rel400_5020() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18427_rel400_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18428_rel400_5022() { var g = Game.Solo(seed: 397422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18429_rel400_5023() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18430_rel400_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397424 })).Sim); }
    [Fact] public void Q18431_rel400_5025() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18432_rel400_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18433_rel400_5027() { var g = Game.Solo(seed: 397427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18434_rel400_5028() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18435_rel400_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397429 })).Sim); }
    [Fact] public void Q18436_rel400_5030() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18437_rel400_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18438_rel400_5032() { var g = Game.Solo(seed: 397432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18439_rel400_5033() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18440_rel400_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397434 })).Sim); }
    [Fact] public void Q18441_rel400_5035() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18442_rel400_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18443_rel400_5037() { var g = Game.Solo(seed: 397437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18444_rel400_5038() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18445_rel400_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397439 })).Sim); }
    [Fact] public void Q18446_rel400_5040() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18447_rel400_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18448_rel400_5042() { var g = Game.Solo(seed: 397442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18449_rel400_5043() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18450_rel400_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397444 })).Sim); }
    [Fact] public void Q18451_rel400_5045() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18452_rel400_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18453_rel400_5047() { var g = Game.Solo(seed: 397447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18454_rel400_5048() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18455_rel400_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397449 })).Sim); }
    [Fact] public void Q18456_rel400_5050() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18457_rel400_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18458_rel400_5052() { var g = Game.Solo(seed: 397452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18459_rel400_5053() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18460_rel400_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397454 })).Sim); }
    [Fact] public void Q18461_rel400_5055() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18462_rel400_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18463_rel400_5057() { var g = Game.Solo(seed: 397457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18464_rel400_5058() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18465_rel400_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397459 })).Sim); }
    [Fact] public void Q18466_rel400_5060() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18467_rel400_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18468_rel400_5062() { var g = Game.Solo(seed: 397462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18469_rel400_5063() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18470_rel400_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397464 })).Sim); }
    [Fact] public void Q18471_rel400_5065() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release); }
    [Fact] public void Q18472_rel400_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18473_rel400_5067() { var g = Game.Solo(seed: 397467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18474_rel400_5068() { GameConfig c = TestData.Config(); Assert.True(c.Office.Events.Count >= 5); }
    [Fact] public void Q18475_rel400_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397469 })).Sim); }
}
