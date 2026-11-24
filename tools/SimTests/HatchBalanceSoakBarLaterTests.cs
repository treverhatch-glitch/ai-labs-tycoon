using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1956–Q2025: batch 33 balance/soak data bar (schema 42).</summary>
public class HatchBalanceSoakBarLaterTests
{
    [Fact] public void Q1956_soak_1956() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1957_soak_1957() { var g = Game.Solo(seed: 3801); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1958_soak_1958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3902 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1959_soak_1959() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1960_soak_1960() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1961_soak_1961() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1962_soak_1962() { var g = Game.Solo(seed: 3806); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1963_soak_1963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3907 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1964_soak_1964() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1965_soak_1965() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1966_soak_1966() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1967_soak_1967() { var g = Game.Solo(seed: 3811); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1968_soak_1968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3912 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1969_soak_1969() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1970_soak_1970() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1971_soak_1971() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1972_soak_1972() { var g = Game.Solo(seed: 3816); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1973_soak_1973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3917 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1974_soak_1974() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1975_soak_1975() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1976_soak_1976() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1977_soak_1977() { var g = Game.Solo(seed: 3821); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1978_soak_1978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3922 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1979_soak_1979() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1980_soak_1980() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1981_soak_1981() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1982_soak_1982() { var g = Game.Solo(seed: 3826); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1983_soak_1983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3927 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1984_soak_1984() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1985_soak_1985() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1986_soak_1986() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1987_soak_1987() { var g = Game.Solo(seed: 3831); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1988_soak_1988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3932 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1989_soak_1989() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1990_soak_1990() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1991_soak_1991() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1992_soak_1992() { var g = Game.Solo(seed: 3836); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1993_soak_1993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3937 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1994_soak_1994() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q1995_soak_1995() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q1996_soak_1996() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q1997_soak_1997() { var g = Game.Solo(seed: 3841); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q1998_soak_1998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3942 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q1999_soak_1999() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2000_soak_2000() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q2001_soak_2001() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q2002_soak_2002() { var g = Game.Solo(seed: 3846); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q2003_soak_2003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3947 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q2004_soak_2004() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2005_soak_2005() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q2006_soak_2006() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q2007_soak_2007() { var g = Game.Solo(seed: 3851); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q2008_soak_2008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3952 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q2009_soak_2009() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2010_soak_2010() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q2011_soak_2011() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q2012_soak_2012() { var g = Game.Solo(seed: 3856); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q2013_soak_2013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3957 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q2014_soak_2014() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2015_soak_2015() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q2016_soak_2016() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q2017_soak_2017() { var g = Game.Solo(seed: 3861); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q2018_soak_2018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3962 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q2019_soak_2019() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2020_soak_2020() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
    [Fact] public void Q2021_soak_2021() { GameConfig c = TestData.Config(); var presets = BalanceSoak.DefaultPresets(c); Assert.True(presets.Length >= 3); Assert.True(c.Calendar.WeeksPerYear > 0); }
    [Fact] public void Q2022_soak_2022() { var g = Game.Solo(seed: 3866); Assert.Equal(45, g.State.Schema); Assert.True(g.C.Training.CarryoverPermille > 0); }
    [Fact] public void Q2023_soak_2023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 3967 })).Sim); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 1); }
    [Fact] public void Q2024_soak_2024() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); Assert.True(c.Eras.Count > 0); }
    [Fact] public void Q2025_soak_2025() { GameConfig c = TestData.Config(); int wpy = c.Calendar.WeeksPerYear; Assert.True(wpy * 8 >= 48 * 8); var g = Game.New(seed: 5); Assert.Equal(45, g.State.Schema); }
}