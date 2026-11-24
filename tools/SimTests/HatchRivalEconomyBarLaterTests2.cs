using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3706–Q3775: batch 58 rival economy data bar coda (schema 31).</summary>
public class HatchRivalEconomyBarLaterTests2
{
    [Fact] public void Q3706_rivalecon2_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3707_rivalecon2_3707() { var g = Game.Solo(seed: 6582); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3708_rivalecon2_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6684 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3709_rivalecon2_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3710_rivalecon2_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3711_rivalecon2_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3712_rivalecon2_3712() { var g = Game.Solo(seed: 6587); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3713_rivalecon2_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6689 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3714_rivalecon2_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3715_rivalecon2_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3716_rivalecon2_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3717_rivalecon2_3717() { var g = Game.Solo(seed: 6592); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3718_rivalecon2_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6694 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3719_rivalecon2_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3720_rivalecon2_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3721_rivalecon2_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3722_rivalecon2_3722() { var g = Game.Solo(seed: 6597); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3723_rivalecon2_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6699 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3724_rivalecon2_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3725_rivalecon2_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3726_rivalecon2_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3727_rivalecon2_3727() { var g = Game.Solo(seed: 6602); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3728_rivalecon2_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6704 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3729_rivalecon2_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3730_rivalecon2_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3731_rivalecon2_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3732_rivalecon2_3732() { var g = Game.Solo(seed: 6607); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3733_rivalecon2_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6709 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3734_rivalecon2_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3735_rivalecon2_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3736_rivalecon2_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3737_rivalecon2_3737() { var g = Game.Solo(seed: 6612); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3738_rivalecon2_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6714 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3739_rivalecon2_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3740_rivalecon2_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3741_rivalecon2_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3742_rivalecon2_3742() { var g = Game.Solo(seed: 6617); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3743_rivalecon2_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6719 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3744_rivalecon2_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3745_rivalecon2_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3746_rivalecon2_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3747_rivalecon2_3747() { var g = Game.Solo(seed: 6622); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3748_rivalecon2_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6724 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3749_rivalecon2_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3750_rivalecon2_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3751_rivalecon2_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3752_rivalecon2_3752() { var g = Game.Solo(seed: 6627); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3753_rivalecon2_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6729 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3754_rivalecon2_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3755_rivalecon2_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3756_rivalecon2_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3757_rivalecon2_3757() { var g = Game.Solo(seed: 6632); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3758_rivalecon2_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6734 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3759_rivalecon2_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3760_rivalecon2_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3761_rivalecon2_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3762_rivalecon2_3762() { var g = Game.Solo(seed: 6637); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3763_rivalecon2_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6739 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3764_rivalecon2_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3765_rivalecon2_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3766_rivalecon2_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3767_rivalecon2_3767() { var g = Game.Solo(seed: 6642); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3768_rivalecon2_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6744 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3769_rivalecon2_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3770_rivalecon2_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q3771_rivalecon2_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q3772_rivalecon2_3772() { var g = Game.Solo(seed: 6647); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q3773_rivalecon2_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6749 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3774_rivalecon2_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q3775_rivalecon2_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
