using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16306+: batch 368 NEXT bible tail (vernname).</summary>
public class HatchVersionNamingBarLaterTests29
{
    [Fact] public void Q16306_vernname368_4000() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16307_vernname368_4001() { var g = Game.Solo(seed: 393101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16308_vernname368_4002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393202 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16309_vernname368_4003() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16310_vernname368_4004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16311_vernname368_4005() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16312_vernname368_4006() { var g = Game.Solo(seed: 393106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16313_vernname368_4007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393207 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16314_vernname368_4008() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16315_vernname368_4009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16316_vernname368_4010() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16317_vernname368_4011() { var g = Game.Solo(seed: 393111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16318_vernname368_4012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393212 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16319_vernname368_4013() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16320_vernname368_4014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16321_vernname368_4015() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16322_vernname368_4016() { var g = Game.Solo(seed: 393116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16323_vernname368_4017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393217 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16324_vernname368_4018() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16325_vernname368_4019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16326_vernname368_4020() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16327_vernname368_4021() { var g = Game.Solo(seed: 393121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16328_vernname368_4022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393222 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16329_vernname368_4023() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16330_vernname368_4024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16331_vernname368_4025() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16332_vernname368_4026() { var g = Game.Solo(seed: 393126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16333_vernname368_4027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393227 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16334_vernname368_4028() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16335_vernname368_4029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16336_vernname368_4030() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16337_vernname368_4031() { var g = Game.Solo(seed: 393131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16338_vernname368_4032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393232 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16339_vernname368_4033() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16340_vernname368_4034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16341_vernname368_4035() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16342_vernname368_4036() { var g = Game.Solo(seed: 393136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16343_vernname368_4037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393237 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16344_vernname368_4038() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16345_vernname368_4039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16346_vernname368_4040() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16347_vernname368_4041() { var g = Game.Solo(seed: 393141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16348_vernname368_4042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393242 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16349_vernname368_4043() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16350_vernname368_4044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16351_vernname368_4045() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16352_vernname368_4046() { var g = Game.Solo(seed: 393146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16353_vernname368_4047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393247 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16354_vernname368_4048() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16355_vernname368_4049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16356_vernname368_4050() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16357_vernname368_4051() { var g = Game.Solo(seed: 393151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16358_vernname368_4052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393252 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16359_vernname368_4053() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16360_vernname368_4054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16361_vernname368_4055() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16362_vernname368_4056() { var g = Game.Solo(seed: 393156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16363_vernname368_4057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393257 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16364_vernname368_4058() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16365_vernname368_4059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16366_vernname368_4060() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16367_vernname368_4061() { var g = Game.Solo(seed: 393161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16368_vernname368_4062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393262 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16369_vernname368_4063() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16370_vernname368_4064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
    [Fact] public void Q16371_vernname368_4065() { GameConfig c = TestData.Config(); Assert.True(c.Naming.Suffixes.Count >= 1); Assert.True(c.Naming.MaxMess > 0); }
    [Fact] public void Q16372_vernname368_4066() { var g = Game.Solo(seed: 393166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.ConfusionPermille > 0); }
    [Fact] public void Q16373_vernname368_4067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 393267 })).Sim); Assert.True(c.Naming.MessPerVersion > 0); }
    [Fact] public void Q16374_vernname368_4068() { GameConfig c = TestData.Config(); Assert.All(c.Naming.Suffixes, s => Assert.False(string.IsNullOrEmpty(s))); }
    [Fact] public void Q16375_vernname368_4069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Naming.CleanPoints > 0); }
}
