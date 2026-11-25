using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11736–11805: batch 213 balance/soak data bar coda (schema 29).</summary>
public class HatchBalanceSoakBarLaterTests11
{
    [Fact] public void Q14276_soak11_4196() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14277_soak11_4197() { var g = Game.Solo(seed: 143562); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14278_soak11_4198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143663 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14279_soak11_4199() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14280_soak11_4200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14281_soak11_4201() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14282_soak11_4202() { var g = Game.Solo(seed: 143567); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14283_soak11_4203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143668 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14284_soak11_4204() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14285_soak11_4205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14286_soak11_4206() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14287_soak11_4207() { var g = Game.Solo(seed: 143572); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14288_soak11_4208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143673 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14289_soak11_4209() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14290_soak11_4210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14291_soak11_4211() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14292_soak11_4212() { var g = Game.Solo(seed: 143577); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14293_soak11_4213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143678 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14294_soak11_4214() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14295_soak11_4215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14296_soak11_4216() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14297_soak11_4217() { var g = Game.Solo(seed: 143582); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14298_soak11_4218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143683 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14299_soak11_4219() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14300_soak11_4220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14301_soak11_4221() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14302_soak11_4222() { var g = Game.Solo(seed: 143587); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14303_soak11_4223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143688 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14304_soak11_4224() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14305_soak11_4225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14306_soak11_4226() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14307_soak11_4227() { var g = Game.Solo(seed: 143592); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14308_soak11_4228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143693 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14309_soak11_4229() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14310_soak11_4230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14311_soak11_4231() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14312_soak11_4232() { var g = Game.Solo(seed: 143597); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14313_soak11_4233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143698 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14314_soak11_4234() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14315_soak11_4235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14316_soak11_4236() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14317_soak11_4237() { var g = Game.Solo(seed: 143602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14318_soak11_4238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143703 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14319_soak11_4239() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14320_soak11_4240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14321_soak11_4241() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14322_soak11_4242() { var g = Game.Solo(seed: 143607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14323_soak11_4243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143708 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14324_soak11_4244() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14325_soak11_4245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14326_soak11_4246() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14327_soak11_4247() { var g = Game.Solo(seed: 143612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14328_soak11_4248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143713 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14329_soak11_4249() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14330_soak11_4250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14331_soak11_4251() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14332_soak11_4252() { var g = Game.Solo(seed: 143617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14333_soak11_4253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143718 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14334_soak11_4254() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14335_soak11_4255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14336_soak11_4256() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14337_soak11_4257() { var g = Game.Solo(seed: 143622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14338_soak11_4258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143723 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14339_soak11_4259() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14340_soak11_4260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14341_soak11_4261() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14342_soak11_4262() { var g = Game.Solo(seed: 143627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14343_soak11_4263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143728 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14344_soak11_4264() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14345_soak11_4265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 195); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
}
