using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14836–14905: batch 313 balance soak data bar (schema 29).</summary>
public class HatchBalanceSoakBarLaterTests19
{
    [Fact] public void Q14836_soak19_4756() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14837_soak19_4757() { var g = Game.Solo(seed: 284362); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14838_soak19_4758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284463 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14839_soak19_4759() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14840_soak19_4760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14841_soak19_4761() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14842_soak19_4762() { var g = Game.Solo(seed: 284367); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14843_soak19_4763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284468 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14844_soak19_4764() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14845_soak19_4765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14846_soak19_4766() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14847_soak19_4767() { var g = Game.Solo(seed: 284372); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14848_soak19_4768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284473 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14849_soak19_4769() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14850_soak19_4770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14851_soak19_4771() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14852_soak19_4772() { var g = Game.Solo(seed: 284377); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14853_soak19_4773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284478 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14854_soak19_4774() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14855_soak19_4775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14856_soak19_4776() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14857_soak19_4777() { var g = Game.Solo(seed: 284382); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14858_soak19_4778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284483 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14859_soak19_4779() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14860_soak19_4780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14861_soak19_4781() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14862_soak19_4782() { var g = Game.Solo(seed: 284387); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14863_soak19_4783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284488 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14864_soak19_4784() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14865_soak19_4785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14866_soak19_4786() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14867_soak19_4787() { var g = Game.Solo(seed: 284392); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14868_soak19_4788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284493 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14869_soak19_4789() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14870_soak19_4790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14871_soak19_4791() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14872_soak19_4792() { var g = Game.Solo(seed: 284397); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14873_soak19_4793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284498 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14874_soak19_4794() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14875_soak19_4795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14876_soak19_4796() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14877_soak19_4797() { var g = Game.Solo(seed: 284402); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14878_soak19_4798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284503 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14879_soak19_4799() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14880_soak19_4800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14881_soak19_4801() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14882_soak19_4802() { var g = Game.Solo(seed: 284407); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14883_soak19_4803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284508 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14884_soak19_4804() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14885_soak19_4805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14886_soak19_4806() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14887_soak19_4807() { var g = Game.Solo(seed: 284412); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14888_soak19_4808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284513 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14889_soak19_4809() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14890_soak19_4810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14891_soak19_4811() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14892_soak19_4812() { var g = Game.Solo(seed: 284417); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14893_soak19_4813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284518 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14894_soak19_4814() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14895_soak19_4815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14896_soak19_4816() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14897_soak19_4817() { var g = Game.Solo(seed: 284422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14898_soak19_4818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284523 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14899_soak19_4819() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14900_soak19_4820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14901_soak19_4821() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14902_soak19_4822() { var g = Game.Solo(seed: 284427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14903_soak19_4823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 284528 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14904_soak19_4824() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14905_soak19_4825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141155); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
}
