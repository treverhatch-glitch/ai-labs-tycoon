using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11856–11925: batch 195 containment/ending data bar coda (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests10
{
    [Fact] public void Q13106_contend9_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13107_contend9_3917() { var g = Game.Solo(seed: 128322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13108_contend9_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128423 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13109_contend9_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13110_contend9_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13111_contend9_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13112_contend9_3922() { var g = Game.Solo(seed: 128327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13113_contend9_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128428 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13114_contend9_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13115_contend9_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13116_contend9_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13117_contend9_3927() { var g = Game.Solo(seed: 128332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13118_contend9_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128433 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13119_contend9_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13120_contend9_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13121_contend9_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13122_contend9_3932() { var g = Game.Solo(seed: 128337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13123_contend9_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128438 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13124_contend9_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13125_contend9_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13126_contend9_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13127_contend9_3937() { var g = Game.Solo(seed: 128342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13128_contend9_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128443 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13129_contend9_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13130_contend9_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13131_contend9_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13132_contend9_3942() { var g = Game.Solo(seed: 128347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13133_contend9_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128448 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13134_contend9_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13135_contend9_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13136_contend9_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13137_contend9_3947() { var g = Game.Solo(seed: 128352); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13138_contend9_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128453 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13139_contend9_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13140_contend9_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13141_contend9_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13142_contend9_3952() { var g = Game.Solo(seed: 128357); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13143_contend9_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128458 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13144_contend9_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13145_contend9_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13146_contend9_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13147_contend9_3957() { var g = Game.Solo(seed: 128362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13148_contend9_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128463 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13149_contend9_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13150_contend9_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13151_contend9_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13152_contend9_3962() { var g = Game.Solo(seed: 128367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13153_contend9_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128468 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13154_contend9_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13155_contend9_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13156_contend9_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13157_contend9_3967() { var g = Game.Solo(seed: 128372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13158_contend9_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128473 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13159_contend9_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13160_contend9_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13161_contend9_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13162_contend9_3972() { var g = Game.Solo(seed: 128377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13163_contend9_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128478 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13164_contend9_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13165_contend9_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13166_contend9_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13167_contend9_3977() { var g = Game.Solo(seed: 128382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13168_contend9_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128483 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13169_contend9_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13170_contend9_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q13171_contend9_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q13172_contend9_3982() { var g = Game.Solo(seed: 128387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q13173_contend9_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 128488 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q13174_contend9_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q13175_contend9_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
