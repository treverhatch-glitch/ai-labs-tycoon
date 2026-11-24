using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6566–6635: batch 163 world-map/economy cross bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests7
{
    [Fact] public void Q11046_hatchwor7_4686() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11047_hatchwor7_4687() { var g = Game.Solo(seed: 99862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11048_hatchwor7_4688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99963 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11049_hatchwor7_4689() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11050_hatchwor7_4690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11051_hatchwor7_4691() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11052_hatchwor7_4692() { var g = Game.Solo(seed: 99867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11053_hatchwor7_4693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99968 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11054_hatchwor7_4694() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11055_hatchwor7_4695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11056_hatchwor7_4696() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11057_hatchwor7_4697() { var g = Game.Solo(seed: 99872); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11058_hatchwor7_4698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99973 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11059_hatchwor7_4699() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11060_hatchwor7_4700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11061_hatchwor7_4701() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11062_hatchwor7_4702() { var g = Game.Solo(seed: 99877); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11063_hatchwor7_4703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99978 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11064_hatchwor7_4704() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11065_hatchwor7_4705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11066_hatchwor7_4706() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11067_hatchwor7_4707() { var g = Game.Solo(seed: 99882); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11068_hatchwor7_4708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99983 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11069_hatchwor7_4709() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11070_hatchwor7_4710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11071_hatchwor7_4711() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11072_hatchwor7_4712() { var g = Game.Solo(seed: 99887); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11073_hatchwor7_4713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99988 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11074_hatchwor7_4714() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11075_hatchwor7_4715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11076_hatchwor7_4716() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11077_hatchwor7_4717() { var g = Game.Solo(seed: 99892); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11078_hatchwor7_4718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99993 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11079_hatchwor7_4719() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11080_hatchwor7_4720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11081_hatchwor7_4721() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11082_hatchwor7_4722() { var g = Game.Solo(seed: 99897); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11083_hatchwor7_4723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 99998 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11084_hatchwor7_4724() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11085_hatchwor7_4725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11086_hatchwor7_4726() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11087_hatchwor7_4727() { var g = Game.Solo(seed: 99902); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11088_hatchwor7_4728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100003 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11089_hatchwor7_4729() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11090_hatchwor7_4730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11091_hatchwor7_4731() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11092_hatchwor7_4732() { var g = Game.Solo(seed: 99907); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11093_hatchwor7_4733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100008 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11094_hatchwor7_4734() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11095_hatchwor7_4735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11096_hatchwor7_4736() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11097_hatchwor7_4737() { var g = Game.Solo(seed: 99912); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11098_hatchwor7_4738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100013 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11099_hatchwor7_4739() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11100_hatchwor7_4740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11101_hatchwor7_4741() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11102_hatchwor7_4742() { var g = Game.Solo(seed: 99917); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11103_hatchwor7_4743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100018 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11104_hatchwor7_4744() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11105_hatchwor7_4745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11106_hatchwor7_4746() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11107_hatchwor7_4747() { var g = Game.Solo(seed: 99922); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11108_hatchwor7_4748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100023 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11109_hatchwor7_4749() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11110_hatchwor7_4750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q11111_hatchwor7_4751() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11112_hatchwor7_4752() { var g = Game.Solo(seed: 99927); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q11113_hatchwor7_4753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100028 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q11114_hatchwor7_4754() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q11115_hatchwor7_4755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 145); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
