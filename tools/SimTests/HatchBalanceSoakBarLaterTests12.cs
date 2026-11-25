using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14346–14415: batch 233 balance soak data bar (schema 29).</summary>
public class HatchBalanceSoakBarLaterTests12
{
    [Fact] public void Q14346_soak12_4266() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14347_soak12_4267() { var g = Game.Solo(seed: 161162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14348_soak12_4268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161263 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14349_soak12_4269() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14350_soak12_4270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14351_soak12_4271() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14352_soak12_4272() { var g = Game.Solo(seed: 161167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14353_soak12_4273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161268 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14354_soak12_4274() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14355_soak12_4275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14356_soak12_4276() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14357_soak12_4277() { var g = Game.Solo(seed: 161172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14358_soak12_4278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161273 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14359_soak12_4279() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14360_soak12_4280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14361_soak12_4281() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14362_soak12_4282() { var g = Game.Solo(seed: 161177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14363_soak12_4283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161278 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14364_soak12_4284() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14365_soak12_4285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14366_soak12_4286() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14367_soak12_4287() { var g = Game.Solo(seed: 161182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14368_soak12_4288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161283 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14369_soak12_4289() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14370_soak12_4290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14371_soak12_4291() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14372_soak12_4292() { var g = Game.Solo(seed: 161187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14373_soak12_4293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161288 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14374_soak12_4294() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14375_soak12_4295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14376_soak12_4296() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14377_soak12_4297() { var g = Game.Solo(seed: 161192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14378_soak12_4298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161293 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14379_soak12_4299() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14380_soak12_4300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14381_soak12_4301() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14382_soak12_4302() { var g = Game.Solo(seed: 161197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14383_soak12_4303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161298 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14384_soak12_4304() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14385_soak12_4305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14386_soak12_4306() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14387_soak12_4307() { var g = Game.Solo(seed: 161202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14388_soak12_4308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161303 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14389_soak12_4309() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14390_soak12_4310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14391_soak12_4311() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14392_soak12_4312() { var g = Game.Solo(seed: 161207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14393_soak12_4313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161308 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14394_soak12_4314() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14395_soak12_4315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14396_soak12_4316() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14397_soak12_4317() { var g = Game.Solo(seed: 161212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14398_soak12_4318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161313 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14399_soak12_4319() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14400_soak12_4320() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14401_soak12_4321() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14402_soak12_4322() { var g = Game.Solo(seed: 161217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14403_soak12_4323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161318 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14404_soak12_4324() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14405_soak12_4325() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14406_soak12_4326() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14407_soak12_4327() { var g = Game.Solo(seed: 161222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14408_soak12_4328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161323 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14409_soak12_4329() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14410_soak12_4330() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14411_soak12_4331() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q14412_soak12_4332() { var g = Game.Solo(seed: 161227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q14413_soak12_4333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161328 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q14414_soak12_4334() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14415_soak12_4335() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17815); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerYear > 0); }
}
