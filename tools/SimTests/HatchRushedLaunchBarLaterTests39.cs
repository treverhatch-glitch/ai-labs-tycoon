using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16936+: batch 378 rivals bible tail (rushln).</summary>
public class HatchRushedLaunchBarLaterTests39
{
    [Fact] public void Q16936_rushln378_4100() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16937_rushln378_4101() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16938_rushln378_4102() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16939_rushln378_4103() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16940_rushln378_4104() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16941_rushln378_4105() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16942_rushln378_4106() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16943_rushln378_4107() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16944_rushln378_4108() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16945_rushln378_4109() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16946_rushln378_4110() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16947_rushln378_4111() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16948_rushln378_4112() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16949_rushln378_4113() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16950_rushln378_4114() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16951_rushln378_4115() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16952_rushln378_4116() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16953_rushln378_4117() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16954_rushln378_4118() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16955_rushln378_4119() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16956_rushln378_4120() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16957_rushln378_4121() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16958_rushln378_4122() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16959_rushln378_4123() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16960_rushln378_4124() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16961_rushln378_4125() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16962_rushln378_4126() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16963_rushln378_4127() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16964_rushln378_4128() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16965_rushln378_4129() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
    [Fact] public void Q16966_rushln378_4130() { GameConfig c = TestData.Config(); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16967_rushln378_4131() { var g = Game.Solo(seed: 396301); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.IncidentWeeks >= 0); }
    [Fact] public void Q16968_rushln378_4132() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Rush.MaxWeeks > 0); }
    [Fact] public void Q16969_rushln378_4133() { GameConfig c = TestData.Config(); Assert.True(c.Rush.IncidentPermillePerWeek > 0); }
    [Fact] public void Q16970_rushln378_4134() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rush.MaxWeeks > 0); }
}
