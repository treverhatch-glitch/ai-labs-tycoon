using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11786–11855: batch 214 forward-queue data bar coda (schema 29).</summary>
public class HatchForwardQueueBarLaterTests11
{
    [Fact] public void Q14336_forward10_4126() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14337_forward10_4127() { var g = Game.Solo(seed: 141402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14338_forward10_4128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141503 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14339_forward10_4129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14340_forward10_4130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14341_forward10_4131() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14342_forward10_4132() { var g = Game.Solo(seed: 141407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14343_forward10_4133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141508 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14344_forward10_4134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14345_forward10_4135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14346_forward10_4136() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14347_forward10_4137() { var g = Game.Solo(seed: 141412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14348_forward10_4138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141513 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14349_forward10_4139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14350_forward10_4140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14351_forward10_4141() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14352_forward10_4142() { var g = Game.Solo(seed: 141417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14353_forward10_4143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141518 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14354_forward10_4144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14355_forward10_4145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14356_forward10_4146() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14357_forward10_4147() { var g = Game.Solo(seed: 141422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14358_forward10_4148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141523 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14359_forward10_4149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14360_forward10_4150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14361_forward10_4151() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14362_forward10_4152() { var g = Game.Solo(seed: 141427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14363_forward10_4153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141528 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14364_forward10_4154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14365_forward10_4155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14366_forward10_4156() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14367_forward10_4157() { var g = Game.Solo(seed: 141432); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14368_forward10_4158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141533 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14369_forward10_4159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14370_forward10_4160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14371_forward10_4161() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14372_forward10_4162() { var g = Game.Solo(seed: 141437); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14373_forward10_4163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141538 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14374_forward10_4164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14375_forward10_4165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14376_forward10_4166() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14377_forward10_4167() { var g = Game.Solo(seed: 141442); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14378_forward10_4168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141543 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14379_forward10_4169() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14380_forward10_4170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14381_forward10_4171() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14382_forward10_4172() { var g = Game.Solo(seed: 141447); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14383_forward10_4173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141548 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14384_forward10_4174() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14385_forward10_4175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14386_forward10_4176() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14387_forward10_4177() { var g = Game.Solo(seed: 141452); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14388_forward10_4178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141553 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14389_forward10_4179() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14390_forward10_4180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14391_forward10_4181() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14392_forward10_4182() { var g = Game.Solo(seed: 141457); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14393_forward10_4183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141558 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14394_forward10_4184() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14395_forward10_4185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14396_forward10_4186() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14397_forward10_4187() { var g = Game.Solo(seed: 141462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14398_forward10_4188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141563 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14399_forward10_4189() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14400_forward10_4190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14401_forward10_4191() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14402_forward10_4192() { var g = Game.Solo(seed: 141467); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14403_forward10_4193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 141568 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14404_forward10_4194() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14405_forward10_4195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 196); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
}
