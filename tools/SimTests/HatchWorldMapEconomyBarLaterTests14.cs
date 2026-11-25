using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13996–14065: batch 285 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests14
{
    [Fact] public void Q13996_hatchwor14_5036() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13997_hatchwor14_5037() { var g = Game.Solo(seed: 223362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13998_hatchwor14_5038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223463 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13999_hatchwor14_5039() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14000_hatchwor14_5040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14001_hatchwor14_5041() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14002_hatchwor14_5042() { var g = Game.Solo(seed: 223367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14003_hatchwor14_5043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223468 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14004_hatchwor14_5044() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14005_hatchwor14_5045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14006_hatchwor14_5046() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14007_hatchwor14_5047() { var g = Game.Solo(seed: 223372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14008_hatchwor14_5048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223473 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14009_hatchwor14_5049() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14010_hatchwor14_5050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14011_hatchwor14_5051() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14012_hatchwor14_5052() { var g = Game.Solo(seed: 223377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14013_hatchwor14_5053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223478 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14014_hatchwor14_5054() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14015_hatchwor14_5055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14016_hatchwor14_5056() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14017_hatchwor14_5057() { var g = Game.Solo(seed: 223382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14018_hatchwor14_5058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223483 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14019_hatchwor14_5059() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14020_hatchwor14_5060() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14021_hatchwor14_5061() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14022_hatchwor14_5062() { var g = Game.Solo(seed: 223387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14023_hatchwor14_5063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223488 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14024_hatchwor14_5064() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14025_hatchwor14_5065() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14026_hatchwor14_5066() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14027_hatchwor14_5067() { var g = Game.Solo(seed: 223392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14028_hatchwor14_5068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223493 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14029_hatchwor14_5069() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14030_hatchwor14_5070() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14031_hatchwor14_5071() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14032_hatchwor14_5072() { var g = Game.Solo(seed: 223397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14033_hatchwor14_5073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223498 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14034_hatchwor14_5074() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14035_hatchwor14_5075() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14036_hatchwor14_5076() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14037_hatchwor14_5077() { var g = Game.Solo(seed: 223402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14038_hatchwor14_5078() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223503 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14039_hatchwor14_5079() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14040_hatchwor14_5080() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14041_hatchwor14_5081() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14042_hatchwor14_5082() { var g = Game.Solo(seed: 223407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14043_hatchwor14_5083() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223508 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14044_hatchwor14_5084() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14045_hatchwor14_5085() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14046_hatchwor14_5086() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14047_hatchwor14_5087() { var g = Game.Solo(seed: 223412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14048_hatchwor14_5088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223513 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14049_hatchwor14_5089() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14050_hatchwor14_5090() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14051_hatchwor14_5091() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14052_hatchwor14_5092() { var g = Game.Solo(seed: 223417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14053_hatchwor14_5093() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223518 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14054_hatchwor14_5094() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14055_hatchwor14_5095() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14056_hatchwor14_5096() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14057_hatchwor14_5097() { var g = Game.Solo(seed: 223422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14058_hatchwor14_5098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223523 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14059_hatchwor14_5099() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14060_hatchwor14_5100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14061_hatchwor14_5101() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14062_hatchwor14_5102() { var g = Game.Solo(seed: 223427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14063_hatchwor14_5103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 223528 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14064_hatchwor14_5104() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14065_hatchwor14_5105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88285); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
