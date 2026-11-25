using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14616–14685: batch 331 Pop/second-place data bar (schema 29).</summary>
public class HatchPopSecondPlaceBarLaterTests19
{
    [Fact] public void Q14616_popsec19_2586() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14617_popsec19_2587() { var g = Game.Solo(seed: 331002); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14618_popsec19_2588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331103 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14619_popsec19_2589() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14620_popsec19_2590() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14621_popsec19_2591() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14622_popsec19_2592() { var g = Game.Solo(seed: 331007); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14623_popsec19_2593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331108 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14624_popsec19_2594() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14625_popsec19_2595() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14626_popsec19_2596() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14627_popsec19_2597() { var g = Game.Solo(seed: 331012); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14628_popsec19_2598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331113 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14629_popsec19_2599() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14630_popsec19_2600() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14631_popsec19_2601() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14632_popsec19_2602() { var g = Game.Solo(seed: 331017); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14633_popsec19_2603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331118 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14634_popsec19_2604() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14635_popsec19_2605() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14636_popsec19_2606() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14637_popsec19_2607() { var g = Game.Solo(seed: 331022); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14638_popsec19_2608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331123 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14639_popsec19_2609() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14640_popsec19_2610() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14641_popsec19_2611() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14642_popsec19_2612() { var g = Game.Solo(seed: 331027); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14643_popsec19_2613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331128 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14644_popsec19_2614() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14645_popsec19_2615() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14646_popsec19_2616() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14647_popsec19_2617() { var g = Game.Solo(seed: 331032); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14648_popsec19_2618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331133 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14649_popsec19_2619() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14650_popsec19_2620() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14651_popsec19_2621() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14652_popsec19_2622() { var g = Game.Solo(seed: 331037); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14653_popsec19_2623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331138 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14654_popsec19_2624() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14655_popsec19_2625() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14656_popsec19_2626() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14657_popsec19_2627() { var g = Game.Solo(seed: 331042); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14658_popsec19_2628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331143 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14659_popsec19_2629() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14660_popsec19_2630() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14661_popsec19_2631() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14662_popsec19_2632() { var g = Game.Solo(seed: 331047); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14663_popsec19_2633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331148 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14664_popsec19_2634() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14665_popsec19_2635() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14666_popsec19_2636() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14667_popsec19_2637() { var g = Game.Solo(seed: 331052); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14668_popsec19_2638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331153 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14669_popsec19_2639() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14670_popsec19_2640() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14671_popsec19_2641() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14672_popsec19_2642() { var g = Game.Solo(seed: 331057); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14673_popsec19_2643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331158 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14674_popsec19_2644() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14675_popsec19_2645() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14676_popsec19_2646() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14677_popsec19_2647() { var g = Game.Solo(seed: 331062); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14678_popsec19_2648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331163 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14679_popsec19_2649() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14680_popsec19_2650() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14681_popsec19_2651() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q14682_popsec19_2652() { var g = Game.Solo(seed: 331067); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q14683_popsec19_2653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 331168 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14684_popsec19_2654() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q14685_popsec19_2655() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
}
