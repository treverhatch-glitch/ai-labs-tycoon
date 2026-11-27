using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16866+: batch 376 rivals bible tail (trainobj).</summary>
public class HatchTrainingObjectiveBarLaterTests37
{
    [Fact] public void Q16866_trainobj376_4100() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16867_trainobj376_4101() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16868_trainobj376_4102() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16869_trainobj376_4103() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16870_trainobj376_4104() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16871_trainobj376_4105() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16872_trainobj376_4106() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16873_trainobj376_4107() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16874_trainobj376_4108() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16875_trainobj376_4109() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16876_trainobj376_4110() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16877_trainobj376_4111() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16878_trainobj376_4112() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16879_trainobj376_4113() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16880_trainobj376_4114() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16881_trainobj376_4115() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16882_trainobj376_4116() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16883_trainobj376_4117() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16884_trainobj376_4118() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16885_trainobj376_4119() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16886_trainobj376_4120() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16887_trainobj376_4121() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16888_trainobj376_4122() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16889_trainobj376_4123() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16890_trainobj376_4124() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16891_trainobj376_4125() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16892_trainobj376_4126() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16893_trainobj376_4127() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16894_trainobj376_4128() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16895_trainobj376_4129() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16896_trainobj376_4130() { GameConfig c = TestData.Config(); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16897_trainobj376_4131() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16898_trainobj376_4132() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396202 })).Sim); Assert.True(c.Objectives.Objectives.Count >= 1); }
    [Fact] public void Q16899_trainobj376_4133() { GameConfig c = TestData.Config(); Assert.True(TrainingObjectives.AppealPermille(c, new ModelState()) > 0); }
    [Fact] public void Q16900_trainobj376_4134() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Objectives.Objectives.Count >= 1); }
}
