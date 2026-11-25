using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14546–14615: batch 255 containment/superintel data bar (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests13
{
    [Fact] public void Q14546_contend12_4056() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14547_contend12_4057() { var g = Game.Solo(seed: 177482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14548_contend12_4058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177583 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14549_contend12_4059() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14550_contend12_4060() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14551_contend12_4061() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14552_contend12_4062() { var g = Game.Solo(seed: 177487); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14553_contend12_4063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177588 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14554_contend12_4064() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14555_contend12_4065() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14556_contend12_4066() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14557_contend12_4067() { var g = Game.Solo(seed: 177492); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14558_contend12_4068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177593 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14559_contend12_4069() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14560_contend12_4070() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14561_contend12_4071() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14562_contend12_4072() { var g = Game.Solo(seed: 177497); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14563_contend12_4073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177598 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14564_contend12_4074() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14565_contend12_4075() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14566_contend12_4076() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14567_contend12_4077() { var g = Game.Solo(seed: 177502); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14568_contend12_4078() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177603 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14569_contend12_4079() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14570_contend12_4080() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14571_contend12_4081() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14572_contend12_4082() { var g = Game.Solo(seed: 177507); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14573_contend12_4083() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177608 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14574_contend12_4084() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14575_contend12_4085() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14576_contend12_4086() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14577_contend12_4087() { var g = Game.Solo(seed: 177512); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14578_contend12_4088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177613 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14579_contend12_4089() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14580_contend12_4090() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14581_contend12_4091() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14582_contend12_4092() { var g = Game.Solo(seed: 177517); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14583_contend12_4093() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177618 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14584_contend12_4094() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14585_contend12_4095() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14586_contend12_4096() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14587_contend12_4097() { var g = Game.Solo(seed: 177522); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14588_contend12_4098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177623 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14589_contend12_4099() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14590_contend12_4100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14591_contend12_4101() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14592_contend12_4102() { var g = Game.Solo(seed: 177527); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14593_contend12_4103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177628 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14594_contend12_4104() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14595_contend12_4105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14596_contend12_4106() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14597_contend12_4107() { var g = Game.Solo(seed: 177532); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14598_contend12_4108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177633 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14599_contend12_4109() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14600_contend12_4110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14601_contend12_4111() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14602_contend12_4112() { var g = Game.Solo(seed: 177537); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14603_contend12_4113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177638 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14604_contend12_4114() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14605_contend12_4115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14606_contend12_4116() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14607_contend12_4117() { var g = Game.Solo(seed: 177542); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14608_contend12_4118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177643 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14609_contend12_4119() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14610_contend12_4120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14611_contend12_4121() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14612_contend12_4122() { var g = Game.Solo(seed: 177547); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14613_contend12_4123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 177648 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14614_contend12_4124() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14615_contend12_4125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
