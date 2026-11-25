using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11856–11925: batch 215 containment/ending data bar coda (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests11
{
    [Fact] public void Q14406_contend10_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14407_contend10_3917() { var g = Game.Solo(seed: 142282); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14408_contend10_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142383 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14409_contend10_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14410_contend10_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14411_contend10_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14412_contend10_3922() { var g = Game.Solo(seed: 142287); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14413_contend10_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142388 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14414_contend10_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14415_contend10_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14416_contend10_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14417_contend10_3927() { var g = Game.Solo(seed: 142292); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14418_contend10_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142393 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14419_contend10_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14420_contend10_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14421_contend10_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14422_contend10_3932() { var g = Game.Solo(seed: 142297); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14423_contend10_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142398 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14424_contend10_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14425_contend10_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14426_contend10_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14427_contend10_3937() { var g = Game.Solo(seed: 142302); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14428_contend10_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142403 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14429_contend10_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14430_contend10_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14431_contend10_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14432_contend10_3942() { var g = Game.Solo(seed: 142307); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14433_contend10_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142408 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14434_contend10_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14435_contend10_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14436_contend10_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14437_contend10_3947() { var g = Game.Solo(seed: 142312); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14438_contend10_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142413 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14439_contend10_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14440_contend10_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14441_contend10_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14442_contend10_3952() { var g = Game.Solo(seed: 142317); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14443_contend10_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142418 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14444_contend10_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14445_contend10_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14446_contend10_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14447_contend10_3957() { var g = Game.Solo(seed: 142322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14448_contend10_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142423 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14449_contend10_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14450_contend10_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14451_contend10_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14452_contend10_3962() { var g = Game.Solo(seed: 142327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14453_contend10_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142428 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14454_contend10_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14455_contend10_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14456_contend10_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14457_contend10_3967() { var g = Game.Solo(seed: 142332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14458_contend10_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142433 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14459_contend10_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14460_contend10_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14461_contend10_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14462_contend10_3972() { var g = Game.Solo(seed: 142337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14463_contend10_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142438 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14464_contend10_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14465_contend10_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14466_contend10_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14467_contend10_3977() { var g = Game.Solo(seed: 142342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14468_contend10_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142443 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14469_contend10_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14470_contend10_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14471_contend10_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14472_contend10_3982() { var g = Game.Solo(seed: 142347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14473_contend10_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142448 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14474_contend10_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14475_contend10_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
