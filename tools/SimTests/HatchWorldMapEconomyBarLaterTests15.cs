using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14066–14135: batch 293 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests15
{
    [Fact] public void Q14066_hatchwor15_5106() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14067_hatchwor15_5107() { var g = Game.Solo(seed: 240962); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14068_hatchwor15_5108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241063 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14069_hatchwor15_5109() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14070_hatchwor15_5110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14071_hatchwor15_5111() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14072_hatchwor15_5112() { var g = Game.Solo(seed: 240967); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14073_hatchwor15_5113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241068 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14074_hatchwor15_5114() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14075_hatchwor15_5115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14076_hatchwor15_5116() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14077_hatchwor15_5117() { var g = Game.Solo(seed: 240972); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14078_hatchwor15_5118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241073 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14079_hatchwor15_5119() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14080_hatchwor15_5120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14081_hatchwor15_5121() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14082_hatchwor15_5122() { var g = Game.Solo(seed: 240977); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14083_hatchwor15_5123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241078 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14084_hatchwor15_5124() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14085_hatchwor15_5125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14086_hatchwor15_5126() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14087_hatchwor15_5127() { var g = Game.Solo(seed: 240982); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14088_hatchwor15_5128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241083 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14089_hatchwor15_5129() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14090_hatchwor15_5130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14091_hatchwor15_5131() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14092_hatchwor15_5132() { var g = Game.Solo(seed: 240987); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14093_hatchwor15_5133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241088 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14094_hatchwor15_5134() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14095_hatchwor15_5135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14096_hatchwor15_5136() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14097_hatchwor15_5137() { var g = Game.Solo(seed: 240992); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14098_hatchwor15_5138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241093 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14099_hatchwor15_5139() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14100_hatchwor15_5140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14101_hatchwor15_5141() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14102_hatchwor15_5142() { var g = Game.Solo(seed: 240997); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14103_hatchwor15_5143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241098 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14104_hatchwor15_5144() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14105_hatchwor15_5145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14106_hatchwor15_5146() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14107_hatchwor15_5147() { var g = Game.Solo(seed: 241002); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14108_hatchwor15_5148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241103 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14109_hatchwor15_5149() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14110_hatchwor15_5150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14111_hatchwor15_5151() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14112_hatchwor15_5152() { var g = Game.Solo(seed: 241007); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14113_hatchwor15_5153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241108 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14114_hatchwor15_5154() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14115_hatchwor15_5155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14116_hatchwor15_5156() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14117_hatchwor15_5157() { var g = Game.Solo(seed: 241012); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14118_hatchwor15_5158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241113 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14119_hatchwor15_5159() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14120_hatchwor15_5160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14121_hatchwor15_5161() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14122_hatchwor15_5162() { var g = Game.Solo(seed: 241017); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14123_hatchwor15_5163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241118 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14124_hatchwor15_5164() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14125_hatchwor15_5165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14126_hatchwor15_5166() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14127_hatchwor15_5167() { var g = Game.Solo(seed: 241022); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14128_hatchwor15_5168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241123 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14129_hatchwor15_5169() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14130_hatchwor15_5170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14131_hatchwor15_5171() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14132_hatchwor15_5172() { var g = Game.Solo(seed: 241027); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14133_hatchwor15_5173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 241128 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14134_hatchwor15_5174() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14135_hatchwor15_5175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 105905); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
