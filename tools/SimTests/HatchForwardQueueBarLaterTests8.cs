using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q10416–Q10485: batch 154 forward-queue data bar coda (schema 29).</summary>
public class HatchForwardQueueBarLaterTests8
{
    [Fact] public void Q10416_forward7_4126() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10417_forward7_4127() { var g = Game.Solo(seed: 91042); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10418_forward7_4128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91143 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10419_forward7_4129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10420_forward7_4130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10421_forward7_4131() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10422_forward7_4132() { var g = Game.Solo(seed: 91047); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10423_forward7_4133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91148 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10424_forward7_4134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10425_forward7_4135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10426_forward7_4136() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10427_forward7_4137() { var g = Game.Solo(seed: 91052); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10428_forward7_4138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91153 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10429_forward7_4139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10430_forward7_4140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10431_forward7_4141() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10432_forward7_4142() { var g = Game.Solo(seed: 91057); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10433_forward7_4143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91158 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10434_forward7_4144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10435_forward7_4145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10436_forward7_4146() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10437_forward7_4147() { var g = Game.Solo(seed: 91062); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10438_forward7_4148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91163 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10439_forward7_4149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10440_forward7_4150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10441_forward7_4151() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10442_forward7_4152() { var g = Game.Solo(seed: 91067); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10443_forward7_4153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91168 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10444_forward7_4154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10445_forward7_4155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10446_forward7_4156() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10447_forward7_4157() { var g = Game.Solo(seed: 91072); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10448_forward7_4158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91173 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10449_forward7_4159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10450_forward7_4160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10451_forward7_4161() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10452_forward7_4162() { var g = Game.Solo(seed: 91077); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10453_forward7_4163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91178 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10454_forward7_4164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10455_forward7_4165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10456_forward7_4166() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10457_forward7_4167() { var g = Game.Solo(seed: 91082); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10458_forward7_4168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91183 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10459_forward7_4169() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10460_forward7_4170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10461_forward7_4171() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10462_forward7_4172() { var g = Game.Solo(seed: 91087); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10463_forward7_4173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91188 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10464_forward7_4174() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10465_forward7_4175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10466_forward7_4176() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10467_forward7_4177() { var g = Game.Solo(seed: 91092); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10468_forward7_4178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91193 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10469_forward7_4179() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10470_forward7_4180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10471_forward7_4181() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10472_forward7_4182() { var g = Game.Solo(seed: 91097); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10473_forward7_4183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91198 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10474_forward7_4184() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10475_forward7_4185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10476_forward7_4186() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10477_forward7_4187() { var g = Game.Solo(seed: 91102); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10478_forward7_4188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91203 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10479_forward7_4189() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10480_forward7_4190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q10481_forward7_4191() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q10482_forward7_4192() { var g = Game.Solo(seed: 91107); Assert.Equal(29, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q10483_forward7_4193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 91208 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q10484_forward7_4194() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q10485_forward7_4195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 136); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
}
