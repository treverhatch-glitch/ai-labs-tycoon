using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6706–6775: batch 101 era/incident data bar coda (schema 29).</summary>
public class HatchEraIncidentBarLaterTests3
{
    [Fact] public void Q6706_erainc3_3986() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6707_erainc3_3987() { var g = Game.Solo(seed: 39202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6708_erainc3_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39303 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6709_erainc3_3989() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6710_erainc3_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6711_erainc3_3991() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6712_erainc3_3992() { var g = Game.Solo(seed: 39207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6713_erainc3_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39308 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6714_erainc3_3994() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6715_erainc3_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6716_erainc3_3996() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6717_erainc3_3997() { var g = Game.Solo(seed: 39212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6718_erainc3_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39313 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6719_erainc3_3999() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6720_erainc3_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6721_erainc3_4001() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6722_erainc3_4002() { var g = Game.Solo(seed: 39217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6723_erainc3_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39318 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6724_erainc3_4004() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6725_erainc3_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6726_erainc3_4006() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6727_erainc3_4007() { var g = Game.Solo(seed: 39222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6728_erainc3_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39323 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6729_erainc3_4009() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6730_erainc3_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6731_erainc3_4011() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6732_erainc3_4012() { var g = Game.Solo(seed: 39227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6733_erainc3_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39328 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6734_erainc3_4014() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6735_erainc3_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6736_erainc3_4016() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6737_erainc3_4017() { var g = Game.Solo(seed: 39232); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6738_erainc3_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39333 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6739_erainc3_4019() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6740_erainc3_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6741_erainc3_4021() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6742_erainc3_4022() { var g = Game.Solo(seed: 39237); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6743_erainc3_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39338 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6744_erainc3_4024() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6745_erainc3_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6746_erainc3_4026() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6747_erainc3_4027() { var g = Game.Solo(seed: 39242); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6748_erainc3_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39343 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6749_erainc3_4029() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6750_erainc3_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6751_erainc3_4031() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6752_erainc3_4032() { var g = Game.Solo(seed: 39247); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6753_erainc3_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39348 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6754_erainc3_4034() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6755_erainc3_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6756_erainc3_4036() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6757_erainc3_4037() { var g = Game.Solo(seed: 39252); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6758_erainc3_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39353 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6759_erainc3_4039() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6760_erainc3_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6761_erainc3_4041() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6762_erainc3_4042() { var g = Game.Solo(seed: 39257); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6763_erainc3_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39358 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6764_erainc3_4044() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6765_erainc3_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6766_erainc3_4046() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6767_erainc3_4047() { var g = Game.Solo(seed: 39262); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6768_erainc3_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39363 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6769_erainc3_4049() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6770_erainc3_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q6771_erainc3_4051() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q6772_erainc3_4052() { var g = Game.Solo(seed: 39267); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6773_erainc3_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 39368 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q6774_erainc3_4054() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q6775_erainc3_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 83); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
}
