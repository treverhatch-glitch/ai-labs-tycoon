using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6706–6775: batch 165 era/incident data bar coda (schema 29).</summary>
public class HatchEraIncidentBarLaterTests6
{
    [Fact] public void Q11186_erainc6_3986() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11187_erainc6_3987() { var g = Game.Solo(seed: 101822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11188_erainc6_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101923 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11189_erainc6_3989() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11190_erainc6_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11191_erainc6_3991() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11192_erainc6_3992() { var g = Game.Solo(seed: 101827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11193_erainc6_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101928 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11194_erainc6_3994() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11195_erainc6_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11196_erainc6_3996() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11197_erainc6_3997() { var g = Game.Solo(seed: 101832); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11198_erainc6_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101933 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11199_erainc6_3999() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11200_erainc6_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11201_erainc6_4001() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11202_erainc6_4002() { var g = Game.Solo(seed: 101837); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11203_erainc6_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101938 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11204_erainc6_4004() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11205_erainc6_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11206_erainc6_4006() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11207_erainc6_4007() { var g = Game.Solo(seed: 101842); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11208_erainc6_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101943 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11209_erainc6_4009() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11210_erainc6_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11211_erainc6_4011() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11212_erainc6_4012() { var g = Game.Solo(seed: 101847); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11213_erainc6_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101948 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11214_erainc6_4014() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11215_erainc6_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11216_erainc6_4016() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11217_erainc6_4017() { var g = Game.Solo(seed: 101852); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11218_erainc6_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101953 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11219_erainc6_4019() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11220_erainc6_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11221_erainc6_4021() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11222_erainc6_4022() { var g = Game.Solo(seed: 101857); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11223_erainc6_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101958 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11224_erainc6_4024() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11225_erainc6_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11226_erainc6_4026() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11227_erainc6_4027() { var g = Game.Solo(seed: 101862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11228_erainc6_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101963 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11229_erainc6_4029() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11230_erainc6_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11231_erainc6_4031() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11232_erainc6_4032() { var g = Game.Solo(seed: 101867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11233_erainc6_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101968 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11234_erainc6_4034() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11235_erainc6_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11236_erainc6_4036() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11237_erainc6_4037() { var g = Game.Solo(seed: 101872); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11238_erainc6_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101973 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11239_erainc6_4039() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11240_erainc6_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11241_erainc6_4041() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11242_erainc6_4042() { var g = Game.Solo(seed: 101877); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11243_erainc6_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101978 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11244_erainc6_4044() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11245_erainc6_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11246_erainc6_4046() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11247_erainc6_4047() { var g = Game.Solo(seed: 101882); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11248_erainc6_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101983 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11249_erainc6_4049() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11250_erainc6_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q11251_erainc6_4051() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q11252_erainc6_4052() { var g = Game.Solo(seed: 101887); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11253_erainc6_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101988 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q11254_erainc6_4054() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q11255_erainc6_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 147); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
}
