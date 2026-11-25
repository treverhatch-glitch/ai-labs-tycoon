using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14476–14545: batch 254 forward queue data bar (schema 29).</summary>
public class HatchForwardQueueBarLaterTests13
{
    [Fact] public void Q14476_forward12_4266() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14477_forward12_4267() { var g = Game.Solo(seed: 176602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14478_forward12_4268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176703 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14479_forward12_4269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14480_forward12_4270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14481_forward12_4271() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14482_forward12_4272() { var g = Game.Solo(seed: 176607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14483_forward12_4273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176708 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14484_forward12_4274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14485_forward12_4275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14486_forward12_4276() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14487_forward12_4277() { var g = Game.Solo(seed: 176612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14488_forward12_4278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176713 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14489_forward12_4279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14490_forward12_4280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14491_forward12_4281() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14492_forward12_4282() { var g = Game.Solo(seed: 176617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14493_forward12_4283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176718 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14494_forward12_4284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14495_forward12_4285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14496_forward12_4286() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14497_forward12_4287() { var g = Game.Solo(seed: 176622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14498_forward12_4288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176723 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14499_forward12_4289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14500_forward12_4290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14501_forward12_4291() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14502_forward12_4292() { var g = Game.Solo(seed: 176627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14503_forward12_4293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176728 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14504_forward12_4294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14505_forward12_4295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14506_forward12_4296() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14507_forward12_4297() { var g = Game.Solo(seed: 176632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14508_forward12_4298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176733 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14509_forward12_4299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14510_forward12_4300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14511_forward12_4301() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14512_forward12_4302() { var g = Game.Solo(seed: 176637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14513_forward12_4303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176738 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14514_forward12_4304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14515_forward12_4305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14516_forward12_4306() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14517_forward12_4307() { var g = Game.Solo(seed: 176642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14518_forward12_4308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176743 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14519_forward12_4309() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14520_forward12_4310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14521_forward12_4311() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14522_forward12_4312() { var g = Game.Solo(seed: 176647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14523_forward12_4313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176748 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14524_forward12_4314() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14525_forward12_4315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14526_forward12_4316() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14527_forward12_4317() { var g = Game.Solo(seed: 176652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14528_forward12_4318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176753 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14529_forward12_4319() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14530_forward12_4320() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14531_forward12_4321() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14532_forward12_4322() { var g = Game.Solo(seed: 176657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14533_forward12_4323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176758 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14534_forward12_4324() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14535_forward12_4325() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14536_forward12_4326() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14537_forward12_4327() { var g = Game.Solo(seed: 176662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14538_forward12_4328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176763 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14539_forward12_4329() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14540_forward12_4330() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
    [Fact] public void Q14541_forward12_4331() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q14542_forward12_4332() { var g = Game.Solo(seed: 176667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q14543_forward12_4333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 176768 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14544_forward12_4334() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q14545_forward12_4335() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Calendar.WeeksPerMonth > 0); }
}
