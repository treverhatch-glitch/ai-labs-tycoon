using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17706+: rivsave393.</summary>
public class HatchRivalLogSaveBoundBarLaterTests53
{
    [Fact] public void Q17706_rivsave393_5000() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17707_rivsave393_5001() { var g = Game.New(seed: 396701, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17708_rivsave393_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396702, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17709_rivsave393_5003() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17710_rivsave393_5004() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396704); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17711_rivsave393_5005() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17712_rivsave393_5006() { var g = Game.New(seed: 396706, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17713_rivsave393_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396707, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17714_rivsave393_5008() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17715_rivsave393_5009() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396709); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17716_rivsave393_5010() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17717_rivsave393_5011() { var g = Game.New(seed: 396711, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17718_rivsave393_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396712, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17719_rivsave393_5013() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17720_rivsave393_5014() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396714); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17721_rivsave393_5015() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17722_rivsave393_5016() { var g = Game.New(seed: 396716, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17723_rivsave393_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396717, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17724_rivsave393_5018() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17725_rivsave393_5019() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396719); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17726_rivsave393_5020() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17727_rivsave393_5021() { var g = Game.New(seed: 396721, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17728_rivsave393_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396722, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17729_rivsave393_5023() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17730_rivsave393_5024() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396724); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17731_rivsave393_5025() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17732_rivsave393_5026() { var g = Game.New(seed: 396726, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17733_rivsave393_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396727, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17734_rivsave393_5028() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17735_rivsave393_5029() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396729); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17736_rivsave393_5030() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17737_rivsave393_5031() { var g = Game.New(seed: 396731, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17738_rivsave393_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396732, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17739_rivsave393_5033() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17740_rivsave393_5034() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396734); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17741_rivsave393_5035() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17742_rivsave393_5036() { var g = Game.New(seed: 396736, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17743_rivsave393_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396737, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17744_rivsave393_5038() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17745_rivsave393_5039() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396739); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17746_rivsave393_5040() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17747_rivsave393_5041() { var g = Game.New(seed: 396741, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17748_rivsave393_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396742, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17749_rivsave393_5043() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17750_rivsave393_5044() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396744); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17751_rivsave393_5045() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17752_rivsave393_5046() { var g = Game.New(seed: 396746, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17753_rivsave393_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396747, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17754_rivsave393_5048() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17755_rivsave393_5049() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396749); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17756_rivsave393_5050() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17757_rivsave393_5051() { var g = Game.New(seed: 396751, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17758_rivsave393_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396752, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17759_rivsave393_5053() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17760_rivsave393_5054() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396754); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17761_rivsave393_5055() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17762_rivsave393_5056() { var g = Game.New(seed: 396756, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17763_rivsave393_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396757, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17764_rivsave393_5058() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17765_rivsave393_5059() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396759); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17766_rivsave393_5060() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17767_rivsave393_5061() { var g = Game.New(seed: 396761, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17768_rivsave393_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396762, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17769_rivsave393_5063() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17770_rivsave393_5064() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396764); Assert.Empty(g.Me.Log); }
    [Fact] public void Q17771_rivsave393_5065() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax >= 60); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17772_rivsave393_5066() { var g = Game.New(seed: 396766, c => c.Rivalry.LogMax = 64); Assert.True(g.Rival("openaye").Controller == LabController.Plan); }
    [Fact] public void Q17773_rivsave393_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396767, RivalIds = new System.Collections.Generic.List<string>() })).Sim); }
    [Fact] public void Q17774_rivsave393_5068() { GameConfig c = TestData.Config(); Assert.False(string.IsNullOrEmpty(c.Rivalry.ActionTemplate("Paused"))); }
    [Fact] public void Q17775_rivsave393_5069() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396769); Assert.Empty(g.Me.Log); }
}
