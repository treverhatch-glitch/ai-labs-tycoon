using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6566–6635: batch 99 world-map/economy cross bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests4
{
    [Fact] public void Q6566_hatchwor_4686() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6567_hatchwor_4687() { var g = Game.Solo(seed: 37242); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6568_hatchwor_4688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37343 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6569_hatchwor_4689() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6570_hatchwor_4690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6571_hatchwor_4691() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6572_hatchwor_4692() { var g = Game.Solo(seed: 37247); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6573_hatchwor_4693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37348 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6574_hatchwor_4694() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6575_hatchwor_4695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6576_hatchwor_4696() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6577_hatchwor_4697() { var g = Game.Solo(seed: 37252); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6578_hatchwor_4698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37353 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6579_hatchwor_4699() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6580_hatchwor_4700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6581_hatchwor_4701() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6582_hatchwor_4702() { var g = Game.Solo(seed: 37257); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6583_hatchwor_4703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37358 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6584_hatchwor_4704() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6585_hatchwor_4705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6586_hatchwor_4706() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6587_hatchwor_4707() { var g = Game.Solo(seed: 37262); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6588_hatchwor_4708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37363 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6589_hatchwor_4709() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6590_hatchwor_4710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6591_hatchwor_4711() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6592_hatchwor_4712() { var g = Game.Solo(seed: 37267); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6593_hatchwor_4713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37368 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6594_hatchwor_4714() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6595_hatchwor_4715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6596_hatchwor_4716() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6597_hatchwor_4717() { var g = Game.Solo(seed: 37272); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6598_hatchwor_4718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37373 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6599_hatchwor_4719() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6600_hatchwor_4720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6601_hatchwor_4721() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6602_hatchwor_4722() { var g = Game.Solo(seed: 37277); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6603_hatchwor_4723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37378 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6604_hatchwor_4724() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6605_hatchwor_4725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6606_hatchwor_4726() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6607_hatchwor_4727() { var g = Game.Solo(seed: 37282); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6608_hatchwor_4728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37383 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6609_hatchwor_4729() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6610_hatchwor_4730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6611_hatchwor_4731() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6612_hatchwor_4732() { var g = Game.Solo(seed: 37287); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6613_hatchwor_4733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37388 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6614_hatchwor_4734() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6615_hatchwor_4735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6616_hatchwor_4736() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6617_hatchwor_4737() { var g = Game.Solo(seed: 37292); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6618_hatchwor_4738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37393 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6619_hatchwor_4739() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6620_hatchwor_4740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6621_hatchwor_4741() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6622_hatchwor_4742() { var g = Game.Solo(seed: 37297); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6623_hatchwor_4743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37398 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6624_hatchwor_4744() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6625_hatchwor_4745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6626_hatchwor_4746() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6627_hatchwor_4747() { var g = Game.Solo(seed: 37302); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6628_hatchwor_4748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37403 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6629_hatchwor_4749() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6630_hatchwor_4750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6631_hatchwor_4751() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6632_hatchwor_4752() { var g = Game.Solo(seed: 37307); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q6633_hatchwor_4753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 37408 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q6634_hatchwor_4754() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q6635_hatchwor_4755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 81); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
