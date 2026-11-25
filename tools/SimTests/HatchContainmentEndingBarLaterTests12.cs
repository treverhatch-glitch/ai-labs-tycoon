using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14476–14545: batch 235 containment/superintel data bar (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests12
{
    [Fact] public void Q14476_contend11_3986() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14477_contend11_3987() { var g = Game.Solo(seed: 159882); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14478_contend11_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 159983 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14479_contend11_3989() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14480_contend11_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14481_contend11_3991() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14482_contend11_3992() { var g = Game.Solo(seed: 159887); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14483_contend11_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 159988 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14484_contend11_3994() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14485_contend11_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14486_contend11_3996() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14487_contend11_3997() { var g = Game.Solo(seed: 159892); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14488_contend11_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 159993 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14489_contend11_3999() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14490_contend11_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14491_contend11_4001() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14492_contend11_4002() { var g = Game.Solo(seed: 159897); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14493_contend11_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 159998 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14494_contend11_4004() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14495_contend11_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14496_contend11_4006() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14497_contend11_4007() { var g = Game.Solo(seed: 159902); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14498_contend11_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160003 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14499_contend11_4009() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14500_contend11_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14501_contend11_4011() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14502_contend11_4012() { var g = Game.Solo(seed: 159907); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14503_contend11_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160008 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14504_contend11_4014() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14505_contend11_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14506_contend11_4016() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14507_contend11_4017() { var g = Game.Solo(seed: 159912); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14508_contend11_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160013 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14509_contend11_4019() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14510_contend11_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14511_contend11_4021() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14512_contend11_4022() { var g = Game.Solo(seed: 159917); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14513_contend11_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160018 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14514_contend11_4024() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14515_contend11_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14516_contend11_4026() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14517_contend11_4027() { var g = Game.Solo(seed: 159922); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14518_contend11_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160023 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14519_contend11_4029() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14520_contend11_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14521_contend11_4031() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14522_contend11_4032() { var g = Game.Solo(seed: 159927); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14523_contend11_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160028 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14524_contend11_4034() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14525_contend11_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14526_contend11_4036() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14527_contend11_4037() { var g = Game.Solo(seed: 159932); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14528_contend11_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160033 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14529_contend11_4039() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14530_contend11_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14531_contend11_4041() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14532_contend11_4042() { var g = Game.Solo(seed: 159937); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14533_contend11_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160038 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14534_contend11_4044() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14535_contend11_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14536_contend11_4046() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14537_contend11_4047() { var g = Game.Solo(seed: 159942); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14538_contend11_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160043 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14539_contend11_4049() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14540_contend11_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q14541_contend11_4051() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q14542_contend11_4052() { var g = Game.Solo(seed: 159947); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q14543_contend11_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 160048 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q14544_contend11_4054() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q14545_contend11_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
