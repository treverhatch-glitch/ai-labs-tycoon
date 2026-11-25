using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13586–13655: batch 221 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests11
{
    [Fact] public void Q13586_rivalecon11_3776() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13587_rivalecon11_3777() { var g = Game.Solo(seed: 151202); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13588_rivalecon11_3778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151303 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13589_rivalecon11_3779() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13590_rivalecon11_3780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13591_rivalecon11_3781() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13592_rivalecon11_3782() { var g = Game.Solo(seed: 151207); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13593_rivalecon11_3783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151308 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13594_rivalecon11_3784() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13595_rivalecon11_3785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13596_rivalecon11_3786() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13597_rivalecon11_3787() { var g = Game.Solo(seed: 151212); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13598_rivalecon11_3788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151313 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13599_rivalecon11_3789() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13600_rivalecon11_3790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13601_rivalecon11_3791() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13602_rivalecon11_3792() { var g = Game.Solo(seed: 151217); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13603_rivalecon11_3793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151318 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13604_rivalecon11_3794() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13605_rivalecon11_3795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13606_rivalecon11_3796() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13607_rivalecon11_3797() { var g = Game.Solo(seed: 151222); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13608_rivalecon11_3798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151323 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13609_rivalecon11_3799() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13610_rivalecon11_3800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13611_rivalecon11_3801() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13612_rivalecon11_3802() { var g = Game.Solo(seed: 151227); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13613_rivalecon11_3803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151328 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13614_rivalecon11_3804() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13615_rivalecon11_3805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13616_rivalecon11_3806() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13617_rivalecon11_3807() { var g = Game.Solo(seed: 151232); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13618_rivalecon11_3808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151333 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13619_rivalecon11_3809() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13620_rivalecon11_3810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13621_rivalecon11_3811() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13622_rivalecon11_3812() { var g = Game.Solo(seed: 151237); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13623_rivalecon11_3813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151338 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13624_rivalecon11_3814() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13625_rivalecon11_3815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13626_rivalecon11_3816() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13627_rivalecon11_3817() { var g = Game.Solo(seed: 151242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13628_rivalecon11_3818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151343 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13629_rivalecon11_3819() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13630_rivalecon11_3820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13631_rivalecon11_3821() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13632_rivalecon11_3822() { var g = Game.Solo(seed: 151247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13633_rivalecon11_3823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151348 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13634_rivalecon11_3824() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13635_rivalecon11_3825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13636_rivalecon11_3826() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13637_rivalecon11_3827() { var g = Game.Solo(seed: 151252); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13638_rivalecon11_3828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151353 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13639_rivalecon11_3829() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13640_rivalecon11_3830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13641_rivalecon11_3831() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13642_rivalecon11_3832() { var g = Game.Solo(seed: 151257); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13643_rivalecon11_3833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151358 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13644_rivalecon11_3834() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13645_rivalecon11_3835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13646_rivalecon11_3836() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13647_rivalecon11_3837() { var g = Game.Solo(seed: 151262); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13648_rivalecon11_3838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151363 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13649_rivalecon11_3839() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13650_rivalecon11_3840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13651_rivalecon11_3841() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13652_rivalecon11_3842() { var g = Game.Solo(seed: 151267); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13653_rivalecon11_3843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 151368 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13654_rivalecon11_3844() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13655_rivalecon11_3845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17803); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
