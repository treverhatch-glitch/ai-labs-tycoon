using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18616+: fill403.</summary>
public class HatchEraFillerBarLaterTests66
{
    [Fact] public void Q18616_fill403_5000() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18617_fill403_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18618_fill403_5002() { var g = Game.Solo(seed: 397612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18619_fill403_5003() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18620_fill403_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397614 })).Sim); }
    [Fact] public void Q18621_fill403_5005() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18622_fill403_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18623_fill403_5007() { var g = Game.Solo(seed: 397617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18624_fill403_5008() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18625_fill403_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397619 })).Sim); }
    [Fact] public void Q18626_fill403_5010() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18627_fill403_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18628_fill403_5012() { var g = Game.Solo(seed: 397622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18629_fill403_5013() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18630_fill403_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397624 })).Sim); }
    [Fact] public void Q18631_fill403_5015() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18632_fill403_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18633_fill403_5017() { var g = Game.Solo(seed: 397627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18634_fill403_5018() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18635_fill403_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397629 })).Sim); }
    [Fact] public void Q18636_fill403_5020() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18637_fill403_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18638_fill403_5022() { var g = Game.Solo(seed: 397632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18639_fill403_5023() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18640_fill403_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397634 })).Sim); }
    [Fact] public void Q18641_fill403_5025() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18642_fill403_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18643_fill403_5027() { var g = Game.Solo(seed: 397637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18644_fill403_5028() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18645_fill403_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397639 })).Sim); }
    [Fact] public void Q18646_fill403_5030() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18647_fill403_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18648_fill403_5032() { var g = Game.Solo(seed: 397642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18649_fill403_5033() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18650_fill403_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397644 })).Sim); }
    [Fact] public void Q18651_fill403_5035() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18652_fill403_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18653_fill403_5037() { var g = Game.Solo(seed: 397647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18654_fill403_5038() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18655_fill403_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397649 })).Sim); }
    [Fact] public void Q18656_fill403_5040() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18657_fill403_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18658_fill403_5042() { var g = Game.Solo(seed: 397652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18659_fill403_5043() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18660_fill403_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397654 })).Sim); }
    [Fact] public void Q18661_fill403_5045() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18662_fill403_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18663_fill403_5047() { var g = Game.Solo(seed: 397657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18664_fill403_5048() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18665_fill403_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397659 })).Sim); }
    [Fact] public void Q18666_fill403_5050() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18667_fill403_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18668_fill403_5052() { var g = Game.Solo(seed: 397662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18669_fill403_5053() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18670_fill403_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397664 })).Sim); }
    [Fact] public void Q18671_fill403_5055() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18672_fill403_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18673_fill403_5057() { var g = Game.Solo(seed: 397667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18674_fill403_5058() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18675_fill403_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397669 })).Sim); }
    [Fact] public void Q18676_fill403_5060() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18677_fill403_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18678_fill403_5062() { var g = Game.Solo(seed: 397672); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18679_fill403_5063() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18680_fill403_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397674 })).Sim); }
    [Fact] public void Q18681_fill403_5065() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q18682_fill403_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18683_fill403_5067() { var g = Game.Solo(seed: 397677); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18684_fill403_5068() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.News.Filler.FindAll(f => f.ToYear > 0)); }
    [Fact] public void Q18685_fill403_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397679 })).Sim); }
}
