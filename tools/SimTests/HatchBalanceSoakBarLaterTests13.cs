using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14416–14485: batch 253 balance soak data bar (schema 29).</summary>
public class HatchBalanceSoakBarLaterTests13
{
    [Fact] public void Q14416_soak13_4336() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14417_soak13_4337() { var g = Game.Solo(seed: 178762); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14418_soak13_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178863 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14419_soak13_4339() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14420_soak13_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14421_soak13_4341() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14422_soak13_4342() { var g = Game.Solo(seed: 178767); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14423_soak13_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178868 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14424_soak13_4344() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14425_soak13_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14426_soak13_4346() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14427_soak13_4347() { var g = Game.Solo(seed: 178772); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14428_soak13_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178873 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14429_soak13_4349() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14430_soak13_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14431_soak13_4351() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14432_soak13_4352() { var g = Game.Solo(seed: 178777); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14433_soak13_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178878 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14434_soak13_4354() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14435_soak13_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14436_soak13_4356() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14437_soak13_4357() { var g = Game.Solo(seed: 178782); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14438_soak13_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178883 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14439_soak13_4359() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14440_soak13_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14441_soak13_4361() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14442_soak13_4362() { var g = Game.Solo(seed: 178787); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14443_soak13_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178888 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14444_soak13_4364() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14445_soak13_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14446_soak13_4366() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14447_soak13_4367() { var g = Game.Solo(seed: 178792); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14448_soak13_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178893 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14449_soak13_4369() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14450_soak13_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14451_soak13_4371() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14452_soak13_4372() { var g = Game.Solo(seed: 178797); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14453_soak13_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178898 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14454_soak13_4374() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14455_soak13_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14456_soak13_4376() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14457_soak13_4377() { var g = Game.Solo(seed: 178802); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14458_soak13_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178903 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14459_soak13_4379() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14460_soak13_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14461_soak13_4381() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14462_soak13_4382() { var g = Game.Solo(seed: 178807); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14463_soak13_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178908 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14464_soak13_4384() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14465_soak13_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14466_soak13_4386() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14467_soak13_4387() { var g = Game.Solo(seed: 178812); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14468_soak13_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178913 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14469_soak13_4389() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14470_soak13_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14471_soak13_4391() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14472_soak13_4392() { var g = Game.Solo(seed: 178817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14473_soak13_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178918 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14474_soak13_4394() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14475_soak13_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14476_soak13_4396() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14477_soak13_4397() { var g = Game.Solo(seed: 178822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14478_soak13_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178923 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14479_soak13_4399() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14480_soak13_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14481_soak13_4401() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14482_soak13_4402() { var g = Game.Solo(seed: 178827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14483_soak13_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 178928 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14484_soak13_4404() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14485_soak13_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35435); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
}
