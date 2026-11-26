using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16971+: batch 379 rivals bible tail (newstic).</summary>
public class HatchNewsTickerBarLaterTests40
{
    [Fact] public void Q16971_newstic379_4100() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16972_newstic379_4101() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16973_newstic379_4102() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16974_newstic379_4103() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q16975_newstic379_4104() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q16976_newstic379_4105() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16977_newstic379_4106() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16978_newstic379_4107() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16979_newstic379_4108() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q16980_newstic379_4109() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q16981_newstic379_4110() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16982_newstic379_4111() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16983_newstic379_4112() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16984_newstic379_4113() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q16985_newstic379_4114() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q16986_newstic379_4115() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16987_newstic379_4116() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16988_newstic379_4117() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16989_newstic379_4118() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q16990_newstic379_4119() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q16991_newstic379_4120() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16992_newstic379_4121() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16993_newstic379_4122() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16994_newstic379_4123() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q16995_newstic379_4124() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q16996_newstic379_4125() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16997_newstic379_4126() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q16998_newstic379_4127() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q16999_newstic379_4128() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q17000_newstic379_4129() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
    [Fact] public void Q17001_newstic379_4130() { GameConfig c = TestData.Config(); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q17002_newstic379_4131() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.News.Headlines.Count >= 424); }
    [Fact] public void Q17003_newstic379_4132() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); Assert.True(c.News.Headlines.Count >= 424); }
    [Fact] public void Q17004_newstic379_4133() { GameConfig c = TestData.Config(); Assert.True(c.News.Filler.Count >= 1); }
    [Fact] public void Q17005_newstic379_4134() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 377); }
}
