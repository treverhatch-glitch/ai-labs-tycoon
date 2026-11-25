using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13656–13725: batch 241 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests12
{
    [Fact] public void Q13656_rivalecon12_3846() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13657_rivalecon12_3847() { var g = Game.Solo(seed: 168802); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13658_rivalecon12_3848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168903 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13659_rivalecon12_3849() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13660_rivalecon12_3850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13661_rivalecon12_3851() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13662_rivalecon12_3852() { var g = Game.Solo(seed: 168807); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13663_rivalecon12_3853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168908 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13664_rivalecon12_3854() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13665_rivalecon12_3855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13666_rivalecon12_3856() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13667_rivalecon12_3857() { var g = Game.Solo(seed: 168812); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13668_rivalecon12_3858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168913 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13669_rivalecon12_3859() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13670_rivalecon12_3860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13671_rivalecon12_3861() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13672_rivalecon12_3862() { var g = Game.Solo(seed: 168817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13673_rivalecon12_3863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168918 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13674_rivalecon12_3864() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13675_rivalecon12_3865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13676_rivalecon12_3866() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13677_rivalecon12_3867() { var g = Game.Solo(seed: 168822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13678_rivalecon12_3868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168923 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13679_rivalecon12_3869() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13680_rivalecon12_3870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13681_rivalecon12_3871() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13682_rivalecon12_3872() { var g = Game.Solo(seed: 168827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13683_rivalecon12_3873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168928 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13684_rivalecon12_3874() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13685_rivalecon12_3875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13686_rivalecon12_3876() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13687_rivalecon12_3877() { var g = Game.Solo(seed: 168832); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13688_rivalecon12_3878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168933 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13689_rivalecon12_3879() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13690_rivalecon12_3880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13691_rivalecon12_3881() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13692_rivalecon12_3882() { var g = Game.Solo(seed: 168837); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13693_rivalecon12_3883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168938 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13694_rivalecon12_3884() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13695_rivalecon12_3885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13696_rivalecon12_3886() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13697_rivalecon12_3887() { var g = Game.Solo(seed: 168842); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13698_rivalecon12_3888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168943 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13699_rivalecon12_3889() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13700_rivalecon12_3890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13701_rivalecon12_3891() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13702_rivalecon12_3892() { var g = Game.Solo(seed: 168847); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13703_rivalecon12_3893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168948 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13704_rivalecon12_3894() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13705_rivalecon12_3895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13706_rivalecon12_3896() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13707_rivalecon12_3897() { var g = Game.Solo(seed: 168852); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13708_rivalecon12_3898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168953 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13709_rivalecon12_3899() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13710_rivalecon12_3900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13711_rivalecon12_3901() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13712_rivalecon12_3902() { var g = Game.Solo(seed: 168857); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13713_rivalecon12_3903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168958 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13714_rivalecon12_3904() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13715_rivalecon12_3905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13716_rivalecon12_3906() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13717_rivalecon12_3907() { var g = Game.Solo(seed: 168862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13718_rivalecon12_3908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168963 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13719_rivalecon12_3909() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13720_rivalecon12_3910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q13721_rivalecon12_3911() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q13722_rivalecon12_3912() { var g = Game.Solo(seed: 168867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q13723_rivalecon12_3913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168968 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q13724_rivalecon12_3914() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q13725_rivalecon12_3915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35423); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
