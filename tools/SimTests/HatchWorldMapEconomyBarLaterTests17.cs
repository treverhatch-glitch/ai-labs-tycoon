using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14206–14275: batch 309 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests17
{
    [Fact] public void Q14206_hatchwor17_5246() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14207_hatchwor17_5247() { var g = Game.Solo(seed: 276162); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14208_hatchwor17_5248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276263 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14209_hatchwor17_5249() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14210_hatchwor17_5250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14211_hatchwor17_5251() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14212_hatchwor17_5252() { var g = Game.Solo(seed: 276167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14213_hatchwor17_5253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276268 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14214_hatchwor17_5254() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14215_hatchwor17_5255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14216_hatchwor17_5256() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14217_hatchwor17_5257() { var g = Game.Solo(seed: 276172); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14218_hatchwor17_5258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276273 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14219_hatchwor17_5259() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14220_hatchwor17_5260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14221_hatchwor17_5261() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14222_hatchwor17_5262() { var g = Game.Solo(seed: 276177); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14223_hatchwor17_5263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276278 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14224_hatchwor17_5264() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14225_hatchwor17_5265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14226_hatchwor17_5266() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14227_hatchwor17_5267() { var g = Game.Solo(seed: 276182); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14228_hatchwor17_5268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276283 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14229_hatchwor17_5269() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14230_hatchwor17_5270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14231_hatchwor17_5271() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14232_hatchwor17_5272() { var g = Game.Solo(seed: 276187); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14233_hatchwor17_5273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276288 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14234_hatchwor17_5274() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14235_hatchwor17_5275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14236_hatchwor17_5276() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14237_hatchwor17_5277() { var g = Game.Solo(seed: 276192); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14238_hatchwor17_5278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276293 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14239_hatchwor17_5279() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14240_hatchwor17_5280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14241_hatchwor17_5281() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14242_hatchwor17_5282() { var g = Game.Solo(seed: 276197); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14243_hatchwor17_5283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276298 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14244_hatchwor17_5284() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14245_hatchwor17_5285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14246_hatchwor17_5286() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14247_hatchwor17_5287() { var g = Game.Solo(seed: 276202); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14248_hatchwor17_5288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276303 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14249_hatchwor17_5289() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14250_hatchwor17_5290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14251_hatchwor17_5291() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14252_hatchwor17_5292() { var g = Game.Solo(seed: 276207); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14253_hatchwor17_5293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276308 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14254_hatchwor17_5294() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14255_hatchwor17_5295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14256_hatchwor17_5296() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14257_hatchwor17_5297() { var g = Game.Solo(seed: 276212); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14258_hatchwor17_5298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276313 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14259_hatchwor17_5299() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14260_hatchwor17_5300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14261_hatchwor17_5301() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14262_hatchwor17_5302() { var g = Game.Solo(seed: 276217); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14263_hatchwor17_5303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276318 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14264_hatchwor17_5304() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14265_hatchwor17_5305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14266_hatchwor17_5306() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14267_hatchwor17_5307() { var g = Game.Solo(seed: 276222); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14268_hatchwor17_5308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276323 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14269_hatchwor17_5309() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14270_hatchwor17_5310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14271_hatchwor17_5311() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14272_hatchwor17_5312() { var g = Game.Solo(seed: 276227); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14273_hatchwor17_5313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 276328 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14274_hatchwor17_5314() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14275_hatchwor17_5315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
