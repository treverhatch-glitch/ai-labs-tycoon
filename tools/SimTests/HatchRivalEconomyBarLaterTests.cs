using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2586–Q2655: batch 42 rival/economy data bar (schema 34).</summary>
public class HatchRivalEconomyBarLaterTests
{
    [Fact] public void Q2586_rivalecon_2586() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2587_rivalecon_2587() { var g = Game.Solo(seed: 5602); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2588_rivalecon_2588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5704 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2589_rivalecon_2589() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2590_rivalecon_2590() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2591_rivalecon_2591() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2592_rivalecon_2592() { var g = Game.Solo(seed: 5607); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2593_rivalecon_2593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5709 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2594_rivalecon_2594() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2595_rivalecon_2595() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2596_rivalecon_2596() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2597_rivalecon_2597() { var g = Game.Solo(seed: 5612); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2598_rivalecon_2598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5714 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2599_rivalecon_2599() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2600_rivalecon_2600() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2601_rivalecon_2601() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2602_rivalecon_2602() { var g = Game.Solo(seed: 5617); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2603_rivalecon_2603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5719 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2604_rivalecon_2604() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2605_rivalecon_2605() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2606_rivalecon_2606() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2607_rivalecon_2607() { var g = Game.Solo(seed: 5622); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2608_rivalecon_2608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5724 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2609_rivalecon_2609() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2610_rivalecon_2610() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2611_rivalecon_2611() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2612_rivalecon_2612() { var g = Game.Solo(seed: 5627); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2613_rivalecon_2613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5729 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2614_rivalecon_2614() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2615_rivalecon_2615() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2616_rivalecon_2616() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2617_rivalecon_2617() { var g = Game.Solo(seed: 5632); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2618_rivalecon_2618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5734 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2619_rivalecon_2619() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2620_rivalecon_2620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2621_rivalecon_2621() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2622_rivalecon_2622() { var g = Game.Solo(seed: 5637); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2623_rivalecon_2623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5739 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2624_rivalecon_2624() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2625_rivalecon_2625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2626_rivalecon_2626() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2627_rivalecon_2627() { var g = Game.Solo(seed: 5642); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2628_rivalecon_2628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5744 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2629_rivalecon_2629() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2630_rivalecon_2630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2631_rivalecon_2631() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2632_rivalecon_2632() { var g = Game.Solo(seed: 5647); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2633_rivalecon_2633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5749 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2634_rivalecon_2634() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2635_rivalecon_2635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2636_rivalecon_2636() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2637_rivalecon_2637() { var g = Game.Solo(seed: 5652); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2638_rivalecon_2638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5754 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2639_rivalecon_2639() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2640_rivalecon_2640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2641_rivalecon_2641() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2642_rivalecon_2642() { var g = Game.Solo(seed: 5657); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2643_rivalecon_2643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5759 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2644_rivalecon_2644() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2645_rivalecon_2645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2646_rivalecon_2646() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2647_rivalecon_2647() { var g = Game.Solo(seed: 5662); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2648_rivalecon_2648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5764 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2649_rivalecon_2649() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2650_rivalecon_2650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q2651_rivalecon_2651() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q2652_rivalecon_2652() { var g = Game.Solo(seed: 5667); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q2653_rivalecon_2653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5769 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2654_rivalecon_2654() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q2655_rivalecon_2655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 42); Assert.Equal(34, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
