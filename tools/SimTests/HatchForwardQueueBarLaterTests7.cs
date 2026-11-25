using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7616–7685: batch 134 forward-queue data bar coda (schema 29).</summary>
public class HatchForwardQueueBarLaterTests7
{
    [Fact] public void Q9016_forward7_4126() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9017_forward7_4127() { var g = Game.Solo(seed: 71402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9018_forward7_4128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71503 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9019_forward7_4129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9020_forward7_4130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9021_forward7_4131() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9022_forward7_4132() { var g = Game.Solo(seed: 71407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9023_forward7_4133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71508 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9024_forward7_4134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9025_forward7_4135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9026_forward7_4136() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9027_forward7_4137() { var g = Game.Solo(seed: 71412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9028_forward7_4138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71513 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9029_forward7_4139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9030_forward7_4140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9031_forward7_4141() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9032_forward7_4142() { var g = Game.Solo(seed: 71417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9033_forward7_4143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71518 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9034_forward7_4144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9035_forward7_4145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9036_forward7_4146() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9037_forward7_4147() { var g = Game.Solo(seed: 71422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9038_forward7_4148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71523 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9039_forward7_4149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9040_forward7_4150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9041_forward7_4151() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9042_forward7_4152() { var g = Game.Solo(seed: 71427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9043_forward7_4153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71528 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9044_forward7_4154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9045_forward7_4155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9046_forward7_4156() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9047_forward7_4157() { var g = Game.Solo(seed: 71432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9048_forward7_4158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71533 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9049_forward7_4159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9050_forward7_4160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9051_forward7_4161() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9052_forward7_4162() { var g = Game.Solo(seed: 71437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9053_forward7_4163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71538 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9054_forward7_4164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9055_forward7_4165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9056_forward7_4166() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9057_forward7_4167() { var g = Game.Solo(seed: 71442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9058_forward7_4168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71543 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9059_forward7_4169() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9060_forward7_4170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9061_forward7_4171() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9062_forward7_4172() { var g = Game.Solo(seed: 71447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9063_forward7_4173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71548 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9064_forward7_4174() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9065_forward7_4175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9066_forward7_4176() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9067_forward7_4177() { var g = Game.Solo(seed: 71452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9068_forward7_4178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71553 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9069_forward7_4179() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9070_forward7_4180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9071_forward7_4181() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9072_forward7_4182() { var g = Game.Solo(seed: 71457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9073_forward7_4183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71558 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9074_forward7_4184() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9075_forward7_4185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9076_forward7_4186() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9077_forward7_4187() { var g = Game.Solo(seed: 71462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9078_forward7_4188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71563 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9079_forward7_4189() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9080_forward7_4190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q9081_forward7_4191() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q9082_forward7_4192() { var g = Game.Solo(seed: 71467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q9083_forward7_4193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 71568 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q9084_forward7_4194() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q9085_forward7_4195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
}
