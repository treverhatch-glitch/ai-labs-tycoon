using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6426–6495: batch 201 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests10
{
    [Fact] public void Q13516_rivalecon10_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13517_rivalecon10_3707() { var g = Game.Solo(seed: 133602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13518_rivalecon10_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133703 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13519_rivalecon10_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13520_rivalecon10_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13521_rivalecon10_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13522_rivalecon10_3712() { var g = Game.Solo(seed: 133607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13523_rivalecon10_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133708 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13524_rivalecon10_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13525_rivalecon10_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13526_rivalecon10_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13527_rivalecon10_3717() { var g = Game.Solo(seed: 133612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13528_rivalecon10_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133713 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13529_rivalecon10_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13530_rivalecon10_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13531_rivalecon10_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13532_rivalecon10_3722() { var g = Game.Solo(seed: 133617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13533_rivalecon10_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133718 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13534_rivalecon10_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13535_rivalecon10_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13536_rivalecon10_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13537_rivalecon10_3727() { var g = Game.Solo(seed: 133622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13538_rivalecon10_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133723 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13539_rivalecon10_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13540_rivalecon10_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13541_rivalecon10_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13542_rivalecon10_3732() { var g = Game.Solo(seed: 133627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13543_rivalecon10_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133728 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13544_rivalecon10_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13545_rivalecon10_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13546_rivalecon10_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13547_rivalecon10_3737() { var g = Game.Solo(seed: 133632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13548_rivalecon10_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133733 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13549_rivalecon10_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13550_rivalecon10_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13551_rivalecon10_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13552_rivalecon10_3742() { var g = Game.Solo(seed: 133637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13553_rivalecon10_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133738 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13554_rivalecon10_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13555_rivalecon10_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13556_rivalecon10_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13557_rivalecon10_3747() { var g = Game.Solo(seed: 133642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13558_rivalecon10_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133743 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13559_rivalecon10_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13560_rivalecon10_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13561_rivalecon10_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13562_rivalecon10_3752() { var g = Game.Solo(seed: 133647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13563_rivalecon10_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133748 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13564_rivalecon10_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13565_rivalecon10_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13566_rivalecon10_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13567_rivalecon10_3757() { var g = Game.Solo(seed: 133652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13568_rivalecon10_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133753 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13569_rivalecon10_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13570_rivalecon10_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13571_rivalecon10_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13572_rivalecon10_3762() { var g = Game.Solo(seed: 133657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13573_rivalecon10_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133758 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13574_rivalecon10_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13575_rivalecon10_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13576_rivalecon10_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13577_rivalecon10_3767() { var g = Game.Solo(seed: 133662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13578_rivalecon10_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133763 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13579_rivalecon10_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13580_rivalecon10_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13581_rivalecon10_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13582_rivalecon10_3772() { var g = Game.Solo(seed: 133667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13583_rivalecon10_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 133768 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13584_rivalecon10_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13585_rivalecon10_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 183); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
