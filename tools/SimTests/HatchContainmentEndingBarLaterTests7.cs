using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7686–7755: batch 135 containment/ending data bar coda (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests7
{
    [Fact] public void Q9086_contend7_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9087_contend7_3917() { var g = Game.Solo(seed: 72382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9088_contend7_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72483 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9089_contend7_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9090_contend7_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9091_contend7_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9092_contend7_3922() { var g = Game.Solo(seed: 72387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9093_contend7_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72488 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9094_contend7_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9095_contend7_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9096_contend7_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9097_contend7_3927() { var g = Game.Solo(seed: 72392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9098_contend7_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72493 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9099_contend7_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9100_contend7_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9101_contend7_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9102_contend7_3932() { var g = Game.Solo(seed: 72397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9103_contend7_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72498 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9104_contend7_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9105_contend7_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9106_contend7_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9107_contend7_3937() { var g = Game.Solo(seed: 72402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9108_contend7_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72503 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9109_contend7_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9110_contend7_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9111_contend7_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9112_contend7_3942() { var g = Game.Solo(seed: 72407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9113_contend7_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72508 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9114_contend7_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9115_contend7_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9116_contend7_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9117_contend7_3947() { var g = Game.Solo(seed: 72412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9118_contend7_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72513 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9119_contend7_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9120_contend7_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9121_contend7_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9122_contend7_3952() { var g = Game.Solo(seed: 72417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9123_contend7_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72518 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9124_contend7_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9125_contend7_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9126_contend7_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9127_contend7_3957() { var g = Game.Solo(seed: 72422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9128_contend7_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72523 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9129_contend7_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9130_contend7_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9131_contend7_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9132_contend7_3962() { var g = Game.Solo(seed: 72427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9133_contend7_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72528 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9134_contend7_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9135_contend7_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9136_contend7_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9137_contend7_3967() { var g = Game.Solo(seed: 72432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9138_contend7_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72533 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9139_contend7_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9140_contend7_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9141_contend7_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9142_contend7_3972() { var g = Game.Solo(seed: 72437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9143_contend7_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72538 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9144_contend7_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9145_contend7_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9146_contend7_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9147_contend7_3977() { var g = Game.Solo(seed: 72442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9148_contend7_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72543 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9149_contend7_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9150_contend7_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q9151_contend7_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q9152_contend7_3982() { var g = Game.Solo(seed: 72447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q9153_contend7_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 72548 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q9154_contend7_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q9155_contend7_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
