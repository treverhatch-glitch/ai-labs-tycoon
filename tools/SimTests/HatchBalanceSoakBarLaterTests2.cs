using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3216–Q3285: batch 51 balance/soak data bar coda (schema 41).</summary>
public class HatchBalanceSoakBarLaterTests2
{
    [Fact] public void Q3216_soak2_3216() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3217_soak2_3217() { var g = Game.Solo(seed: 7402); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3218_soak2_3218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7504 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3219_soak2_3219() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3220_soak2_3220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3221_soak2_3221() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3222_soak2_3222() { var g = Game.Solo(seed: 7407); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3223_soak2_3223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7509 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3224_soak2_3224() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3225_soak2_3225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3226_soak2_3226() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3227_soak2_3227() { var g = Game.Solo(seed: 7412); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3228_soak2_3228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7514 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3229_soak2_3229() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3230_soak2_3230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3231_soak2_3231() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3232_soak2_3232() { var g = Game.Solo(seed: 7417); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3233_soak2_3233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7519 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3234_soak2_3234() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3235_soak2_3235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3236_soak2_3236() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3237_soak2_3237() { var g = Game.Solo(seed: 7422); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3238_soak2_3238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7524 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3239_soak2_3239() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3240_soak2_3240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3241_soak2_3241() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3242_soak2_3242() { var g = Game.Solo(seed: 7427); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3243_soak2_3243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7529 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3244_soak2_3244() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3245_soak2_3245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3246_soak2_3246() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3247_soak2_3247() { var g = Game.Solo(seed: 7432); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3248_soak2_3248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7534 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3249_soak2_3249() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3250_soak2_3250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3251_soak2_3251() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3252_soak2_3252() { var g = Game.Solo(seed: 7437); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3253_soak2_3253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7539 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3254_soak2_3254() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3255_soak2_3255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3256_soak2_3256() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3257_soak2_3257() { var g = Game.Solo(seed: 7442); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3258_soak2_3258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7544 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3259_soak2_3259() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3260_soak2_3260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3261_soak2_3261() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3262_soak2_3262() { var g = Game.Solo(seed: 7447); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3263_soak2_3263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7549 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3264_soak2_3264() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3265_soak2_3265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3266_soak2_3266() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3267_soak2_3267() { var g = Game.Solo(seed: 7452); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3268_soak2_3268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7554 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3269_soak2_3269() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3270_soak2_3270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3271_soak2_3271() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3272_soak2_3272() { var g = Game.Solo(seed: 7457); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3273_soak2_3273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7559 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3274_soak2_3274() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3275_soak2_3275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3276_soak2_3276() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3277_soak2_3277() { var g = Game.Solo(seed: 7462); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3278_soak2_3278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7564 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3279_soak2_3279() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3280_soak2_3280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
    [Fact] public void Q3281_soak2_3281() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); }
    [Fact] public void Q3282_soak2_3282() { var g = Game.Solo(seed: 7467); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q3283_soak2_3283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7569 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q3284_soak2_3284() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q3285_soak2_3285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(41, g.State.Schema); Assert.True(c.Calendar.WeeksPerYear * 8 >= 48); }
}
