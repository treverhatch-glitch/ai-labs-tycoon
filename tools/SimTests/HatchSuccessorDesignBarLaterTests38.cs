using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16901+: batch 377 rivals bible tail (succdes).</summary>
public class HatchSuccessorDesignBarLaterTests38
{
    [Fact] public void Q16901_succdes377_4100() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16902_succdes377_4101() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16903_succdes377_4102() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16904_succdes377_4103() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16905_succdes377_4104() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16906_succdes377_4105() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16907_succdes377_4106() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16908_succdes377_4107() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16909_succdes377_4108() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16910_succdes377_4109() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16911_succdes377_4110() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16912_succdes377_4111() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16913_succdes377_4112() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16914_succdes377_4113() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16915_succdes377_4114() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16916_succdes377_4115() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16917_succdes377_4116() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16918_succdes377_4117() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16919_succdes377_4118() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16920_succdes377_4119() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16921_succdes377_4120() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16922_succdes377_4121() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16923_succdes377_4122() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16924_succdes377_4123() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16925_succdes377_4124() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16926_succdes377_4125() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16927_succdes377_4126() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16928_succdes377_4127() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16929_succdes377_4128() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16930_succdes377_4129() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
    [Fact] public void Q16931_succdes377_4130() { GameConfig c = TestData.Config(); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16932_succdes377_4131() { var g = Game.Solo(seed: 396201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16933_succdes377_4132() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Successor.ComputeSavedPermille > 0); }
    [Fact] public void Q16934_succdes377_4133() { GameConfig c = TestData.Config(); Assert.True(c.Successor.AlignmentPoints > 0); }
    [Fact] public void Q16935_succdes377_4134() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Successor.ComputeSavedPermille >= 0); }
}
