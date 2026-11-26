using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q15326–15395: batch 355 (CurrentSchema).</summary>
public class HatchRivalEconomyBarLaterTests14
{
    [Fact] public void Q15326_rivalecon14_5516() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15327_rivalecon14_5517() { var g = Game.Solo(seed: 186402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15328_rivalecon14_5518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186503 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15329_rivalecon14_5519() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15330_rivalecon14_5520() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15331_rivalecon14_5521() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15332_rivalecon14_5522() { var g = Game.Solo(seed: 186407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15333_rivalecon14_5523() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186508 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15334_rivalecon14_5524() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15335_rivalecon14_5525() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15336_rivalecon14_5526() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15337_rivalecon14_5527() { var g = Game.Solo(seed: 186412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15338_rivalecon14_5528() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186513 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15339_rivalecon14_5529() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15340_rivalecon14_5530() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15341_rivalecon14_5531() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15342_rivalecon14_5532() { var g = Game.Solo(seed: 186417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15343_rivalecon14_5533() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186518 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15344_rivalecon14_5534() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15345_rivalecon14_5535() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15346_rivalecon14_5536() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15347_rivalecon14_5537() { var g = Game.Solo(seed: 186422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15348_rivalecon14_5538() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186523 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15349_rivalecon14_5539() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15350_rivalecon14_5540() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15351_rivalecon14_5541() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15352_rivalecon14_5542() { var g = Game.Solo(seed: 186427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15353_rivalecon14_5543() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186528 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15354_rivalecon14_5544() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15355_rivalecon14_5545() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15356_rivalecon14_5546() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15357_rivalecon14_5547() { var g = Game.Solo(seed: 186432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15358_rivalecon14_5548() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186533 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15359_rivalecon14_5549() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15360_rivalecon14_5550() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15361_rivalecon14_5551() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15362_rivalecon14_5552() { var g = Game.Solo(seed: 186437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15363_rivalecon14_5553() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186538 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15364_rivalecon14_5554() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15365_rivalecon14_5555() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15366_rivalecon14_5556() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15367_rivalecon14_5557() { var g = Game.Solo(seed: 186442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15368_rivalecon14_5558() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186543 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15369_rivalecon14_5559() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15370_rivalecon14_5560() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15371_rivalecon14_5561() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15372_rivalecon14_5562() { var g = Game.Solo(seed: 186447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15373_rivalecon14_5563() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186548 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15374_rivalecon14_5564() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15375_rivalecon14_5565() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15376_rivalecon14_5566() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15377_rivalecon14_5567() { var g = Game.Solo(seed: 186452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15378_rivalecon14_5568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186553 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15379_rivalecon14_5569() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15380_rivalecon14_5570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15381_rivalecon14_5571() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15382_rivalecon14_5572() { var g = Game.Solo(seed: 186457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15383_rivalecon14_5573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186558 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15384_rivalecon14_5574() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15385_rivalecon14_5575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15386_rivalecon14_5576() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15387_rivalecon14_5577() { var g = Game.Solo(seed: 186462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15388_rivalecon14_5578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186563 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15389_rivalecon14_5579() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15390_rivalecon14_5580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q15391_rivalecon14_5581() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q15392_rivalecon14_5582() { var g = Game.Solo(seed: 186467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q15393_rivalecon14_5583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 186568 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15394_rivalecon14_5584() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q15395_rivalecon14_5585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53043); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
