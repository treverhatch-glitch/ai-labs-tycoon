using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13996–14065: batch 264 era/incident load data bar coda (schema 29).</summary>
public class HatchEraIncidentBarLaterTests11
{
    [Fact] public void Q13996_erainc11_4196() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13997_erainc11_4197() { var g = Game.Solo(seed: 189922); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13998_erainc11_4198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190023 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13999_erainc11_4199() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14000_erainc11_4200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14001_erainc11_4201() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14002_erainc11_4202() { var g = Game.Solo(seed: 189927); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14003_erainc11_4203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190028 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14004_erainc11_4204() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14005_erainc11_4205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14006_erainc11_4206() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14007_erainc11_4207() { var g = Game.Solo(seed: 189932); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14008_erainc11_4208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190033 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14009_erainc11_4209() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14010_erainc11_4210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14011_erainc11_4211() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14012_erainc11_4212() { var g = Game.Solo(seed: 189937); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14013_erainc11_4213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190038 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14014_erainc11_4214() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14015_erainc11_4215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14016_erainc11_4216() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14017_erainc11_4217() { var g = Game.Solo(seed: 189942); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14018_erainc11_4218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190043 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14019_erainc11_4219() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14020_erainc11_4220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14021_erainc11_4221() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14022_erainc11_4222() { var g = Game.Solo(seed: 189947); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14023_erainc11_4223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190048 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14024_erainc11_4224() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14025_erainc11_4225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14026_erainc11_4226() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14027_erainc11_4227() { var g = Game.Solo(seed: 189952); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14028_erainc11_4228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190053 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14029_erainc11_4229() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14030_erainc11_4230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14031_erainc11_4231() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14032_erainc11_4232() { var g = Game.Solo(seed: 189957); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14033_erainc11_4233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190058 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14034_erainc11_4234() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14035_erainc11_4235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14036_erainc11_4236() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14037_erainc11_4237() { var g = Game.Solo(seed: 189962); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14038_erainc11_4238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190063 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14039_erainc11_4239() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14040_erainc11_4240() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14041_erainc11_4241() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14042_erainc11_4242() { var g = Game.Solo(seed: 189967); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14043_erainc11_4243() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190068 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14044_erainc11_4244() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14045_erainc11_4245() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14046_erainc11_4246() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14047_erainc11_4247() { var g = Game.Solo(seed: 189972); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14048_erainc11_4248() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190073 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14049_erainc11_4249() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14050_erainc11_4250() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14051_erainc11_4251() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14052_erainc11_4252() { var g = Game.Solo(seed: 189977); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14053_erainc11_4253() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190078 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14054_erainc11_4254() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14055_erainc11_4255() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14056_erainc11_4256() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14057_erainc11_4257() { var g = Game.Solo(seed: 189982); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14058_erainc11_4258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190083 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14059_erainc11_4259() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14060_erainc11_4260() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q14061_erainc11_4261() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q14062_erainc11_4262() { var g = Game.Solo(seed: 189987); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14063_erainc11_4263() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 190088 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q14064_erainc11_4264() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q14065_erainc11_4265() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
}
