using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13926–13995: batch 244 era/incident load data bar coda (schema 29).</summary>
public class HatchEraIncidentBarLaterTests10
{
    [Fact] public void Q13926_erainc10_4126() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13927_erainc10_4127() { var g = Game.Solo(seed: 172322); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13928_erainc10_4128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172423 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13929_erainc10_4129() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13930_erainc10_4130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13931_erainc10_4131() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13932_erainc10_4132() { var g = Game.Solo(seed: 172327); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13933_erainc10_4133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172428 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13934_erainc10_4134() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13935_erainc10_4135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13936_erainc10_4136() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13937_erainc10_4137() { var g = Game.Solo(seed: 172332); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13938_erainc10_4138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172433 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13939_erainc10_4139() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13940_erainc10_4140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13941_erainc10_4141() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13942_erainc10_4142() { var g = Game.Solo(seed: 172337); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13943_erainc10_4143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172438 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13944_erainc10_4144() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13945_erainc10_4145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13946_erainc10_4146() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13947_erainc10_4147() { var g = Game.Solo(seed: 172342); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13948_erainc10_4148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172443 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13949_erainc10_4149() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13950_erainc10_4150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13951_erainc10_4151() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13952_erainc10_4152() { var g = Game.Solo(seed: 172347); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13953_erainc10_4153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172448 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13954_erainc10_4154() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13955_erainc10_4155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13956_erainc10_4156() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13957_erainc10_4157() { var g = Game.Solo(seed: 172352); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13958_erainc10_4158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172453 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13959_erainc10_4159() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13960_erainc10_4160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13961_erainc10_4161() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13962_erainc10_4162() { var g = Game.Solo(seed: 172357); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13963_erainc10_4163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172458 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13964_erainc10_4164() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13965_erainc10_4165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13966_erainc10_4166() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13967_erainc10_4167() { var g = Game.Solo(seed: 172362); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13968_erainc10_4168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172463 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13969_erainc10_4169() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13970_erainc10_4170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13971_erainc10_4171() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13972_erainc10_4172() { var g = Game.Solo(seed: 172367); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13973_erainc10_4173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172468 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13974_erainc10_4174() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13975_erainc10_4175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13976_erainc10_4176() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13977_erainc10_4177() { var g = Game.Solo(seed: 172372); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13978_erainc10_4178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172473 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13979_erainc10_4179() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13980_erainc10_4180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13981_erainc10_4181() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13982_erainc10_4182() { var g = Game.Solo(seed: 172377); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13983_erainc10_4183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172478 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13984_erainc10_4184() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13985_erainc10_4185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13986_erainc10_4186() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13987_erainc10_4187() { var g = Game.Solo(seed: 172382); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13988_erainc10_4188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172483 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13989_erainc10_4189() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13990_erainc10_4190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q13991_erainc10_4191() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q13992_erainc10_4192() { var g = Game.Solo(seed: 172387); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13993_erainc10_4193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 172488 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q13994_erainc10_4194() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q13995_erainc10_4195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
}
