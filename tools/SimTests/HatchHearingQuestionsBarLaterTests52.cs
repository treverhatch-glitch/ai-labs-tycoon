using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17636+: hear391 hatch bar.</summary>
public class HatchHearingQuestionsBarLaterTests52
{
    [Fact] public void Q17636_hear391_5000() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17637_hear391_5001() { var g = Game.Solo(seed: 396621); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17638_hear391_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396622 })).Sim); }
    [Fact] public void Q17639_hear391_5003() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17640_hear391_5004() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17641_hear391_5005() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17642_hear391_5006() { var g = Game.Solo(seed: 396626); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17643_hear391_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396627 })).Sim); }
    [Fact] public void Q17644_hear391_5008() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17645_hear391_5009() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17646_hear391_5010() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17647_hear391_5011() { var g = Game.Solo(seed: 396631); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17648_hear391_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396632 })).Sim); }
    [Fact] public void Q17649_hear391_5013() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17650_hear391_5014() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17651_hear391_5015() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17652_hear391_5016() { var g = Game.Solo(seed: 396636); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17653_hear391_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396637 })).Sim); }
    [Fact] public void Q17654_hear391_5018() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17655_hear391_5019() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17656_hear391_5020() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17657_hear391_5021() { var g = Game.Solo(seed: 396641); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17658_hear391_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396642 })).Sim); }
    [Fact] public void Q17659_hear391_5023() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17660_hear391_5024() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17661_hear391_5025() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17662_hear391_5026() { var g = Game.Solo(seed: 396646); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17663_hear391_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396647 })).Sim); }
    [Fact] public void Q17664_hear391_5028() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17665_hear391_5029() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17666_hear391_5030() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17667_hear391_5031() { var g = Game.Solo(seed: 396651); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17668_hear391_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396652 })).Sim); }
    [Fact] public void Q17669_hear391_5033() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17670_hear391_5034() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17671_hear391_5035() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17672_hear391_5036() { var g = Game.Solo(seed: 396656); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17673_hear391_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396657 })).Sim); }
    [Fact] public void Q17674_hear391_5038() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17675_hear391_5039() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17676_hear391_5040() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17677_hear391_5041() { var g = Game.Solo(seed: 396661); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17678_hear391_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396662 })).Sim); }
    [Fact] public void Q17679_hear391_5043() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17680_hear391_5044() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17681_hear391_5045() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17682_hear391_5046() { var g = Game.Solo(seed: 396666); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17683_hear391_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396667 })).Sim); }
    [Fact] public void Q17684_hear391_5048() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17685_hear391_5049() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17686_hear391_5050() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17687_hear391_5051() { var g = Game.Solo(seed: 396671); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17688_hear391_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396672 })).Sim); }
    [Fact] public void Q17689_hear391_5053() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17690_hear391_5054() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17691_hear391_5055() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17692_hear391_5056() { var g = Game.Solo(seed: 396676); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17693_hear391_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396677 })).Sim); }
    [Fact] public void Q17694_hear391_5058() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17695_hear391_5059() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17696_hear391_5060() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17697_hear391_5061() { var g = Game.Solo(seed: 396681); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17698_hear391_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396682 })).Sim); }
    [Fact] public void Q17699_hear391_5063() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17700_hear391_5064() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17701_hear391_5065() { GameConfig c = TestData.Config(); int h = 0; foreach (var e in c.Office.Events) if (e.Trigger == OfficeTrigger.Hearing) h++; Assert.True(h >= 1); }
    [Fact] public void Q17702_hear391_5066() { var g = Game.Solo(seed: 396686); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17703_hear391_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396687 })).Sim); }
    [Fact] public void Q17704_hear391_5068() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Office.Events); }
    [Fact] public void Q17705_hear391_5069() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
}
