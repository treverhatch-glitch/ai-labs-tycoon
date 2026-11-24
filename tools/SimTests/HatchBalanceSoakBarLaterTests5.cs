using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6156–6225: batch 93 balance/soak data bar coda (schema 29).</summary>
public class HatchBalanceSoakBarLaterTests5
{
    [Fact] public void Q6156_soak5_4196() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6157_soak5_4197() { var g = Game.Solo(seed: 31362); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6158_soak5_4198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31463 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6159_soak5_4199() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6160_soak5_4200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6161_soak5_4201() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6162_soak5_4202() { var g = Game.Solo(seed: 31367); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6163_soak5_4203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31468 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6164_soak5_4204() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6165_soak5_4205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6166_soak5_4206() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6167_soak5_4207() { var g = Game.Solo(seed: 31372); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6168_soak5_4208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31473 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6169_soak5_4209() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6170_soak5_4210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6171_soak5_4211() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6172_soak5_4212() { var g = Game.Solo(seed: 31377); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6173_soak5_4213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31478 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6174_soak5_4214() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6175_soak5_4215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6176_soak5_4216() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6177_soak5_4217() { var g = Game.Solo(seed: 31382); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6178_soak5_4218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31483 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6179_soak5_4219() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6180_soak5_4220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6181_soak5_4221() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6182_soak5_4222() { var g = Game.Solo(seed: 31387); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6183_soak5_4223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31488 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6184_soak5_4224() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6185_soak5_4225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6186_soak5_4226() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6187_soak5_4227() { var g = Game.Solo(seed: 31392); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6188_soak5_4228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31493 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6189_soak5_4229() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6190_soak5_4230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6191_soak5_4231() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6192_soak5_4232() { var g = Game.Solo(seed: 31397); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6193_soak5_4233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31498 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6194_soak5_4234() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6195_soak5_4235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6196_soak5_4236() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6197_soak5_4237() { var g = Game.Solo(seed: 31402); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6198_soak5_4238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31503 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6199_soak5_4239() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6200_soak5_4240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6201_soak5_4241() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6202_soak5_4242() { var g = Game.Solo(seed: 31407); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6203_soak5_4243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31508 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6204_soak5_4244() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6205_soak5_4245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6206_soak5_4246() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6207_soak5_4247() { var g = Game.Solo(seed: 31412); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6208_soak5_4248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31513 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6209_soak5_4249() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6210_soak5_4250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6211_soak5_4251() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6212_soak5_4252() { var g = Game.Solo(seed: 31417); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6213_soak5_4253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31518 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6214_soak5_4254() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6215_soak5_4255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6216_soak5_4256() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6217_soak5_4257() { var g = Game.Solo(seed: 31422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6218_soak5_4258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31523 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6219_soak5_4259() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6220_soak5_4260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6221_soak5_4261() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q6222_soak5_4262() { var g = Game.Solo(seed: 31427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q6223_soak5_4263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 31528 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q6224_soak5_4264() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q6225_soak5_4265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 75); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
}
