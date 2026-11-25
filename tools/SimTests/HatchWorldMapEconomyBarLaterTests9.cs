using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6566–6635: batch 203 world-map/economy cross bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests9
{
    [Fact] public void Q13646_hatchwor9_4686() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13647_hatchwor9_4687() { var g = Game.Solo(seed: 135362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13648_hatchwor9_4688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135463 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13649_hatchwor9_4689() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13650_hatchwor9_4690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13651_hatchwor9_4691() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13652_hatchwor9_4692() { var g = Game.Solo(seed: 135367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13653_hatchwor9_4693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135468 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13654_hatchwor9_4694() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13655_hatchwor9_4695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13656_hatchwor9_4696() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13657_hatchwor9_4697() { var g = Game.Solo(seed: 135372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13658_hatchwor9_4698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135473 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13659_hatchwor9_4699() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13660_hatchwor9_4700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13661_hatchwor9_4701() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13662_hatchwor9_4702() { var g = Game.Solo(seed: 135377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13663_hatchwor9_4703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135478 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13664_hatchwor9_4704() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13665_hatchwor9_4705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13666_hatchwor9_4706() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13667_hatchwor9_4707() { var g = Game.Solo(seed: 135382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13668_hatchwor9_4708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135483 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13669_hatchwor9_4709() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13670_hatchwor9_4710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13671_hatchwor9_4711() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13672_hatchwor9_4712() { var g = Game.Solo(seed: 135387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13673_hatchwor9_4713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135488 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13674_hatchwor9_4714() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13675_hatchwor9_4715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13676_hatchwor9_4716() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13677_hatchwor9_4717() { var g = Game.Solo(seed: 135392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13678_hatchwor9_4718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135493 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13679_hatchwor9_4719() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13680_hatchwor9_4720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13681_hatchwor9_4721() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13682_hatchwor9_4722() { var g = Game.Solo(seed: 135397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13683_hatchwor9_4723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135498 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13684_hatchwor9_4724() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13685_hatchwor9_4725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13686_hatchwor9_4726() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13687_hatchwor9_4727() { var g = Game.Solo(seed: 135402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13688_hatchwor9_4728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135503 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13689_hatchwor9_4729() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13690_hatchwor9_4730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13691_hatchwor9_4731() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13692_hatchwor9_4732() { var g = Game.Solo(seed: 135407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13693_hatchwor9_4733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135508 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13694_hatchwor9_4734() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13695_hatchwor9_4735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13696_hatchwor9_4736() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13697_hatchwor9_4737() { var g = Game.Solo(seed: 135412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13698_hatchwor9_4738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135513 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13699_hatchwor9_4739() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13700_hatchwor9_4740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13701_hatchwor9_4741() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13702_hatchwor9_4742() { var g = Game.Solo(seed: 135417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13703_hatchwor9_4743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135518 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13704_hatchwor9_4744() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13705_hatchwor9_4745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13706_hatchwor9_4746() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13707_hatchwor9_4747() { var g = Game.Solo(seed: 135422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13708_hatchwor9_4748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135523 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13709_hatchwor9_4749() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13710_hatchwor9_4750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13711_hatchwor9_4751() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13712_hatchwor9_4752() { var g = Game.Solo(seed: 135427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13713_hatchwor9_4753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 135528 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13714_hatchwor9_4754() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13715_hatchwor9_4755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 185); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
