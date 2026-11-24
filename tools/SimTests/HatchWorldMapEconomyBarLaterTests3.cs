using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q4686–Q4755: batch 72 world-map/economy cross bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests3
{
    [Fact] public void Q4686_hatchwor_4686() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4687_hatchwor_4687() { var g = Game.Solo(seed: 10683); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4688_hatchwor_4688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10785 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4689_hatchwor_4689() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4690_hatchwor_4690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4691_hatchwor_4691() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4692_hatchwor_4692() { var g = Game.Solo(seed: 10688); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4693_hatchwor_4693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10790 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4694_hatchwor_4694() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4695_hatchwor_4695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4696_hatchwor_4696() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4697_hatchwor_4697() { var g = Game.Solo(seed: 10693); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4698_hatchwor_4698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10795 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4699_hatchwor_4699() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4700_hatchwor_4700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4701_hatchwor_4701() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4702_hatchwor_4702() { var g = Game.Solo(seed: 10698); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4703_hatchwor_4703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10800 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4704_hatchwor_4704() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4705_hatchwor_4705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4706_hatchwor_4706() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4707_hatchwor_4707() { var g = Game.Solo(seed: 10703); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4708_hatchwor_4708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10805 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4709_hatchwor_4709() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4710_hatchwor_4710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4711_hatchwor_4711() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4712_hatchwor_4712() { var g = Game.Solo(seed: 10708); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4713_hatchwor_4713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10810 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4714_hatchwor_4714() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4715_hatchwor_4715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4716_hatchwor_4716() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4717_hatchwor_4717() { var g = Game.Solo(seed: 10713); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4718_hatchwor_4718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10815 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4719_hatchwor_4719() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4720_hatchwor_4720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4721_hatchwor_4721() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4722_hatchwor_4722() { var g = Game.Solo(seed: 10718); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4723_hatchwor_4723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10820 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4724_hatchwor_4724() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4725_hatchwor_4725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4726_hatchwor_4726() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4727_hatchwor_4727() { var g = Game.Solo(seed: 10723); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4728_hatchwor_4728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10825 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4729_hatchwor_4729() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4730_hatchwor_4730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4731_hatchwor_4731() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4732_hatchwor_4732() { var g = Game.Solo(seed: 10728); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4733_hatchwor_4733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10830 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4734_hatchwor_4734() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4735_hatchwor_4735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4736_hatchwor_4736() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4737_hatchwor_4737() { var g = Game.Solo(seed: 10733); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4738_hatchwor_4738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10835 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4739_hatchwor_4739() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4740_hatchwor_4740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4741_hatchwor_4741() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4742_hatchwor_4742() { var g = Game.Solo(seed: 10738); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4743_hatchwor_4743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10840 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4744_hatchwor_4744() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4745_hatchwor_4745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4746_hatchwor_4746() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4747_hatchwor_4747() { var g = Game.Solo(seed: 10743); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4748_hatchwor_4748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10845 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4749_hatchwor_4749() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4750_hatchwor_4750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4751_hatchwor_4751() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4752_hatchwor_4752() { var g = Game.Solo(seed: 10748); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q4753_hatchwor_4753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 10850 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q4754_hatchwor_4754() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q4755_hatchwor_4755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 55); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
