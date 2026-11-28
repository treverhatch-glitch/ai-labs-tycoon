using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17776+: explain394.</summary>
public class HatchExplainNumbersBarLaterTests54
{
    [Fact] public void Q17776_explain394_5000() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17777_explain394_5001() { var g = Game.Solo(seed: 396771); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17778_explain394_5002() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17779_explain394_5003() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17780_explain394_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396774 })).Sim); }
    [Fact] public void Q17781_explain394_5005() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17782_explain394_5006() { var g = Game.Solo(seed: 396776); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17783_explain394_5007() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17784_explain394_5008() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17785_explain394_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396779 })).Sim); }
    [Fact] public void Q17786_explain394_5010() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17787_explain394_5011() { var g = Game.Solo(seed: 396781); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17788_explain394_5012() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17789_explain394_5013() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17790_explain394_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396784 })).Sim); }
    [Fact] public void Q17791_explain394_5015() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17792_explain394_5016() { var g = Game.Solo(seed: 396786); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17793_explain394_5017() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17794_explain394_5018() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17795_explain394_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396789 })).Sim); }
    [Fact] public void Q17796_explain394_5020() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17797_explain394_5021() { var g = Game.Solo(seed: 396791); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17798_explain394_5022() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17799_explain394_5023() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17800_explain394_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396794 })).Sim); }
    [Fact] public void Q17801_explain394_5025() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17802_explain394_5026() { var g = Game.Solo(seed: 396796); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17803_explain394_5027() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17804_explain394_5028() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17805_explain394_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396799 })).Sim); }
    [Fact] public void Q17806_explain394_5030() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17807_explain394_5031() { var g = Game.Solo(seed: 396801); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17808_explain394_5032() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17809_explain394_5033() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17810_explain394_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396804 })).Sim); }
    [Fact] public void Q17811_explain394_5035() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17812_explain394_5036() { var g = Game.Solo(seed: 396806); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17813_explain394_5037() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17814_explain394_5038() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17815_explain394_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396809 })).Sim); }
    [Fact] public void Q17816_explain394_5040() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17817_explain394_5041() { var g = Game.Solo(seed: 396811); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17818_explain394_5042() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17819_explain394_5043() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17820_explain394_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396814 })).Sim); }
    [Fact] public void Q17821_explain394_5045() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17822_explain394_5046() { var g = Game.Solo(seed: 396816); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17823_explain394_5047() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17824_explain394_5048() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17825_explain394_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396819 })).Sim); }
    [Fact] public void Q17826_explain394_5050() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17827_explain394_5051() { var g = Game.Solo(seed: 396821); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17828_explain394_5052() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17829_explain394_5053() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17830_explain394_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396824 })).Sim); }
    [Fact] public void Q17831_explain394_5055() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17832_explain394_5056() { var g = Game.Solo(seed: 396826); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17833_explain394_5057() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17834_explain394_5058() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17835_explain394_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396829 })).Sim); }
    [Fact] public void Q17836_explain394_5060() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17837_explain394_5061() { var g = Game.Solo(seed: 396831); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17838_explain394_5062() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17839_explain394_5063() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17840_explain394_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396834 })).Sim); }
    [Fact] public void Q17841_explain394_5065() { GameConfig c = TestData.Config(); Assert.True(c.Market.Sectors.Count >= 1); }
    [Fact] public void Q17842_explain394_5066() { var g = Game.Solo(seed: 396836); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17843_explain394_5067() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17844_explain394_5068() { GameConfig c = TestData.Config(); var g = Game.Solo().WithStarterLab(); Assert.True(g.C.Sector("game-ai") != null); }
    [Fact] public void Q17845_explain394_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396839 })).Sim); }
}
