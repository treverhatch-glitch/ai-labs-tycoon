using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11856–11925: batch 175 containment/ending data bar coda (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests9
{
    [Fact] public void Q11856_contend8_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11857_contend8_3917() { var g = Game.Solo(seed: 110722); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11858_contend8_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110823 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11859_contend8_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11860_contend8_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11861_contend8_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11862_contend8_3922() { var g = Game.Solo(seed: 110727); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11863_contend8_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110828 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11864_contend8_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11865_contend8_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11866_contend8_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11867_contend8_3927() { var g = Game.Solo(seed: 110732); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11868_contend8_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110833 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11869_contend8_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11870_contend8_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11871_contend8_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11872_contend8_3932() { var g = Game.Solo(seed: 110737); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11873_contend8_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110838 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11874_contend8_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11875_contend8_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11876_contend8_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11877_contend8_3937() { var g = Game.Solo(seed: 110742); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11878_contend8_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110843 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11879_contend8_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11880_contend8_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11881_contend8_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11882_contend8_3942() { var g = Game.Solo(seed: 110747); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11883_contend8_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110848 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11884_contend8_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11885_contend8_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11886_contend8_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11887_contend8_3947() { var g = Game.Solo(seed: 110752); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11888_contend8_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110853 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11889_contend8_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11890_contend8_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11891_contend8_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11892_contend8_3952() { var g = Game.Solo(seed: 110757); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11893_contend8_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110858 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11894_contend8_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11895_contend8_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11896_contend8_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11897_contend8_3957() { var g = Game.Solo(seed: 110762); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11898_contend8_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110863 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11899_contend8_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11900_contend8_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11901_contend8_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11902_contend8_3962() { var g = Game.Solo(seed: 110767); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11903_contend8_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110868 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11904_contend8_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11905_contend8_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11906_contend8_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11907_contend8_3967() { var g = Game.Solo(seed: 110772); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11908_contend8_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110873 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11909_contend8_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11910_contend8_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11911_contend8_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11912_contend8_3972() { var g = Game.Solo(seed: 110777); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11913_contend8_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110878 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11914_contend8_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11915_contend8_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11916_contend8_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11917_contend8_3977() { var g = Game.Solo(seed: 110782); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11918_contend8_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110883 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11919_contend8_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11920_contend8_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q11921_contend8_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q11922_contend8_3982() { var g = Game.Solo(seed: 110787); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q11923_contend8_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 110888 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q11924_contend8_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q11925_contend8_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
