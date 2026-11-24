using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1886–Q1955: batch 32 forward-queue data bar (schema 32).</summary>
public class HatchForwardQueueBarLaterTests
{
    [Fact] public void Q1886_forward_1886() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1887_forward_1887() { var g = Game.Solo(seed: 3601); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1888_forward_1888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3702 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1889_forward_1889() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1890_forward_1890() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1891_forward_1891() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1892_forward_1892() { var g = Game.Solo(seed: 3606); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1893_forward_1893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3707 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1894_forward_1894() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1895_forward_1895() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1896_forward_1896() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1897_forward_1897() { var g = Game.Solo(seed: 3611); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1898_forward_1898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3712 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1899_forward_1899() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1900_forward_1900() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1901_forward_1901() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1902_forward_1902() { var g = Game.Solo(seed: 3616); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1903_forward_1903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3717 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1904_forward_1904() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1905_forward_1905() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1906_forward_1906() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1907_forward_1907() { var g = Game.Solo(seed: 3621); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1908_forward_1908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3722 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1909_forward_1909() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1910_forward_1910() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1911_forward_1911() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1912_forward_1912() { var g = Game.Solo(seed: 3626); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1913_forward_1913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3727 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1914_forward_1914() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1915_forward_1915() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1916_forward_1916() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1917_forward_1917() { var g = Game.Solo(seed: 3631); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1918_forward_1918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3732 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1919_forward_1919() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1920_forward_1920() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1921_forward_1921() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1922_forward_1922() { var g = Game.Solo(seed: 3636); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1923_forward_1923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3737 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1924_forward_1924() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1925_forward_1925() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1926_forward_1926() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1927_forward_1927() { var g = Game.Solo(seed: 3641); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1928_forward_1928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3742 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1929_forward_1929() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1930_forward_1930() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1931_forward_1931() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1932_forward_1932() { var g = Game.Solo(seed: 3646); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1933_forward_1933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3747 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1934_forward_1934() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1935_forward_1935() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1936_forward_1936() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1937_forward_1937() { var g = Game.Solo(seed: 3651); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1938_forward_1938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3752 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1939_forward_1939() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1940_forward_1940() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1941_forward_1941() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1942_forward_1942() { var g = Game.Solo(seed: 3656); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1943_forward_1943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3757 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1944_forward_1944() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1945_forward_1945() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1946_forward_1946() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1947_forward_1947() { var g = Game.Solo(seed: 3661); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1948_forward_1948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3762 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1949_forward_1949() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1950_forward_1950() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
    [Fact] public void Q1951_forward_1951() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1952_forward_1952() { var g = Game.Solo(seed: 3666); Assert.Equal(32, g.State.Schema); Assert.True(DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total >= 0); }
    [Fact] public void Q1953_forward_1953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3767 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q1954_forward_1954() { GameConfig c = TestData.Config(); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q1955_forward_1955() { GameConfig c = TestData.Config(); Assert.True(c.Calendar.WeeksPerYear * 60 <= 60 * 52); var g = Game.New(seed: 77); Assert.Empty(SaveGame.Problems(g.C, g.State)); }
}
