using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2866–Q2935: batch 46 containment/ending data bar coda (schema 42).</summary>
public class HatchContainmentEndingBarLaterTests2
{
    [Fact] public void Q2866_contend2_2866() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2867_contend2_2867() { var g = Game.Solo(seed: 6402); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2868_contend2_2868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6504 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2869_contend2_2869() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2870_contend2_2870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2871_contend2_2871() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2872_contend2_2872() { var g = Game.Solo(seed: 6407); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2873_contend2_2873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6509 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2874_contend2_2874() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2875_contend2_2875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2876_contend2_2876() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2877_contend2_2877() { var g = Game.Solo(seed: 6412); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2878_contend2_2878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6514 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2879_contend2_2879() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2880_contend2_2880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2881_contend2_2881() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2882_contend2_2882() { var g = Game.Solo(seed: 6417); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2883_contend2_2883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6519 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2884_contend2_2884() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2885_contend2_2885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2886_contend2_2886() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2887_contend2_2887() { var g = Game.Solo(seed: 6422); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2888_contend2_2888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6524 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2889_contend2_2889() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2890_contend2_2890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2891_contend2_2891() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2892_contend2_2892() { var g = Game.Solo(seed: 6427); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2893_contend2_2893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6529 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2894_contend2_2894() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2895_contend2_2895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2896_contend2_2896() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2897_contend2_2897() { var g = Game.Solo(seed: 6432); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2898_contend2_2898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6534 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2899_contend2_2899() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2900_contend2_2900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2901_contend2_2901() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2902_contend2_2902() { var g = Game.Solo(seed: 6437); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2903_contend2_2903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6539 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2904_contend2_2904() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2905_contend2_2905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2906_contend2_2906() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2907_contend2_2907() { var g = Game.Solo(seed: 6442); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2908_contend2_2908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6544 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2909_contend2_2909() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2910_contend2_2910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2911_contend2_2911() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2912_contend2_2912() { var g = Game.Solo(seed: 6447); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2913_contend2_2913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6549 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2914_contend2_2914() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2915_contend2_2915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2916_contend2_2916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2917_contend2_2917() { var g = Game.Solo(seed: 6452); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2918_contend2_2918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6554 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2919_contend2_2919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2920_contend2_2920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2921_contend2_2921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2922_contend2_2922() { var g = Game.Solo(seed: 6457); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2923_contend2_2923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6559 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2924_contend2_2924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2925_contend2_2925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2926_contend2_2926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2927_contend2_2927() { var g = Game.Solo(seed: 6462); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2928_contend2_2928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6564 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2929_contend2_2929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2930_contend2_2930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q2931_contend2_2931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q2932_contend2_2932() { var g = Game.Solo(seed: 6467); Assert.Equal(45, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q2933_contend2_2933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6569 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q2934_contend2_2934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q2935_contend2_2935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
