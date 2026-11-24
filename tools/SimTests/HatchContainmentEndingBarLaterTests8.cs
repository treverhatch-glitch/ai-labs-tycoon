using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q10486–Q10555: batch 155 containment/ending data bar coda (schema 29).</summary>
public class HatchContainmentEndingBarLaterTests8
{
    [Fact] public void Q10486_contend7_3916() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10487_contend7_3917() { var g = Game.Solo(seed: 92022); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10488_contend7_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92123 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10489_contend7_3919() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10490_contend7_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10491_contend7_3921() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10492_contend7_3922() { var g = Game.Solo(seed: 92027); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10493_contend7_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92128 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10494_contend7_3924() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10495_contend7_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10496_contend7_3926() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10497_contend7_3927() { var g = Game.Solo(seed: 92032); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10498_contend7_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92133 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10499_contend7_3929() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10500_contend7_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10501_contend7_3931() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10502_contend7_3932() { var g = Game.Solo(seed: 92037); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10503_contend7_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92138 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10504_contend7_3934() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10505_contend7_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10506_contend7_3936() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10507_contend7_3937() { var g = Game.Solo(seed: 92042); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10508_contend7_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92143 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10509_contend7_3939() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10510_contend7_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10511_contend7_3941() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10512_contend7_3942() { var g = Game.Solo(seed: 92047); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10513_contend7_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92148 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10514_contend7_3944() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10515_contend7_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10516_contend7_3946() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10517_contend7_3947() { var g = Game.Solo(seed: 92052); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10518_contend7_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92153 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10519_contend7_3949() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10520_contend7_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10521_contend7_3951() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10522_contend7_3952() { var g = Game.Solo(seed: 92057); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10523_contend7_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92158 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10524_contend7_3954() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10525_contend7_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10526_contend7_3956() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10527_contend7_3957() { var g = Game.Solo(seed: 92062); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10528_contend7_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92163 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10529_contend7_3959() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10530_contend7_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10531_contend7_3961() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10532_contend7_3962() { var g = Game.Solo(seed: 92067); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10533_contend7_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92168 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10534_contend7_3964() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10535_contend7_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10536_contend7_3966() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10537_contend7_3967() { var g = Game.Solo(seed: 92072); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10538_contend7_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92173 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10539_contend7_3969() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10540_contend7_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10541_contend7_3971() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10542_contend7_3972() { var g = Game.Solo(seed: 92077); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10543_contend7_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92178 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10544_contend7_3974() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10545_contend7_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10546_contend7_3976() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10547_contend7_3977() { var g = Game.Solo(seed: 92082); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10548_contend7_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92183 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10549_contend7_3979() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10550_contend7_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
    [Fact] public void Q10551_contend7_3981() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, m => !string.IsNullOrEmpty(m.Id)); Assert.NotNull(c.Endings.Superintelligence); }
    [Fact] public void Q10552_contend7_3982() { var g = Game.Solo(seed: 92087); Assert.Equal(29, g.State.Schema); Assert.Contains(g.C.RoomTypes, r => r.Id == "containment-wing"); }
    [Fact] public void Q10553_contend7_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 92188 })).Sim); Assert.True(c.Training.CarryoverPermille > 0); }
    [Fact] public void Q10554_contend7_3984() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Bubble.ThePopEndingForPlayer); }
    [Fact] public void Q10555_contend7_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 137); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Endings.MonopolyCrownYears > 0); }
}
