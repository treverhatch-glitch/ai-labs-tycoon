using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3916–Q3985: batch 61 containment/ending data bar coda (schema 42).</summary>
public class HatchContainmentEndingBarLaterTests3
{
    [Fact] public void Q3916_contend3_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3917_contend3_3917() { var g = Game.Solo(seed: 7382); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3918_contend3_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7484 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3919_contend3_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3920_contend3_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3921_contend3_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3922_contend3_3922() { var g = Game.Solo(seed: 7387); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3923_contend3_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7489 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3924_contend3_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3925_contend3_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3926_contend3_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3927_contend3_3927() { var g = Game.Solo(seed: 7392); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3928_contend3_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7494 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3929_contend3_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3930_contend3_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3931_contend3_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3932_contend3_3932() { var g = Game.Solo(seed: 7397); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3933_contend3_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7499 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3934_contend3_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3935_contend3_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3936_contend3_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3937_contend3_3937() { var g = Game.Solo(seed: 7402); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3938_contend3_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7504 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3939_contend3_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3940_contend3_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3941_contend3_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3942_contend3_3942() { var g = Game.Solo(seed: 7407); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3943_contend3_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7509 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3944_contend3_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3945_contend3_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3946_contend3_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3947_contend3_3947() { var g = Game.Solo(seed: 7412); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3948_contend3_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7514 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3949_contend3_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3950_contend3_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3951_contend3_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3952_contend3_3952() { var g = Game.Solo(seed: 7417); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3953_contend3_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7519 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3954_contend3_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3955_contend3_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3956_contend3_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3957_contend3_3957() { var g = Game.Solo(seed: 7422); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3958_contend3_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7524 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3959_contend3_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3960_contend3_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3961_contend3_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3962_contend3_3962() { var g = Game.Solo(seed: 7427); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3963_contend3_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7529 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3964_contend3_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3965_contend3_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3966_contend3_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3967_contend3_3967() { var g = Game.Solo(seed: 7432); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3968_contend3_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7534 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3969_contend3_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3970_contend3_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3971_contend3_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3972_contend3_3972() { var g = Game.Solo(seed: 7437); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3973_contend3_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7539 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3974_contend3_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3975_contend3_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3976_contend3_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3977_contend3_3977() { var g = Game.Solo(seed: 7442); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3978_contend3_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7544 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3979_contend3_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3980_contend3_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q3981_contend3_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q3982_contend3_3982() { var g = Game.Solo(seed: 7447); Assert.Equal(42, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q3983_contend3_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7549 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q3984_contend3_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q3985_contend3_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 46); Assert.Equal(42, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
