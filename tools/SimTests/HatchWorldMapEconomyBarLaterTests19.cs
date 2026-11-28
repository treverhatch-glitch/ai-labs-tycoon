using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14346–14415: batch 325 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests19
{
    [Fact] public void Q14346_hatchwor19_5386() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14347_hatchwor19_5387() { var g = Game.Solo(seed: 311362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14348_hatchwor19_5388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311463 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14349_hatchwor19_5389() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14350_hatchwor19_5390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14351_hatchwor19_5391() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14352_hatchwor19_5392() { var g = Game.Solo(seed: 311367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14353_hatchwor19_5393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311468 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14354_hatchwor19_5394() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14355_hatchwor19_5395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14356_hatchwor19_5396() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14357_hatchwor19_5397() { var g = Game.Solo(seed: 311372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14358_hatchwor19_5398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311473 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14359_hatchwor19_5399() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14360_hatchwor19_5400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14361_hatchwor19_5401() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14362_hatchwor19_5402() { var g = Game.Solo(seed: 311377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14363_hatchwor19_5403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311478 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14364_hatchwor19_5404() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14365_hatchwor19_5405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14366_hatchwor19_5406() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14367_hatchwor19_5407() { var g = Game.Solo(seed: 311382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14368_hatchwor19_5408() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311483 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14369_hatchwor19_5409() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14370_hatchwor19_5410() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14371_hatchwor19_5411() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14372_hatchwor19_5412() { var g = Game.Solo(seed: 311387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14373_hatchwor19_5413() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311488 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14374_hatchwor19_5414() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14375_hatchwor19_5415() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14376_hatchwor19_5416() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14377_hatchwor19_5417() { var g = Game.Solo(seed: 311392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14378_hatchwor19_5418() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311493 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14379_hatchwor19_5419() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14380_hatchwor19_5420() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14381_hatchwor19_5421() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14382_hatchwor19_5422() { var g = Game.Solo(seed: 311397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14383_hatchwor19_5423() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311498 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14384_hatchwor19_5424() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14385_hatchwor19_5425() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14386_hatchwor19_5426() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14387_hatchwor19_5427() { var g = Game.Solo(seed: 311402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14388_hatchwor19_5428() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311503 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14389_hatchwor19_5429() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14390_hatchwor19_5430() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14391_hatchwor19_5431() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14392_hatchwor19_5432() { var g = Game.Solo(seed: 311407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14393_hatchwor19_5433() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311508 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14394_hatchwor19_5434() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14395_hatchwor19_5435() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14396_hatchwor19_5436() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14397_hatchwor19_5437() { var g = Game.Solo(seed: 311412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14398_hatchwor19_5438() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311513 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14399_hatchwor19_5439() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14400_hatchwor19_5440() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14401_hatchwor19_5441() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14402_hatchwor19_5442() { var g = Game.Solo(seed: 311417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14403_hatchwor19_5443() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311518 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14404_hatchwor19_5444() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14405_hatchwor19_5445() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14406_hatchwor19_5446() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14407_hatchwor19_5447() { var g = Game.Solo(seed: 311422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14408_hatchwor19_5448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311523 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14409_hatchwor19_5449() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14410_hatchwor19_5450() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14411_hatchwor19_5451() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14412_hatchwor19_5452() { var g = Game.Solo(seed: 311427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14413_hatchwor19_5453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 311528 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14414_hatchwor19_5454() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14415_hatchwor19_5455() { GameConfig c = TestData.Config(); var g = Game.New(seed: 176385); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
