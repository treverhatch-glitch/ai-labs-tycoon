using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7196–7265: batch 132 treaty/ending data bar (schema 29).</summary>
public class HatchTreatyEndingBarLaterTests4
{
    [Fact] public void Q8876_treaty4_end_1746() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8877_treaty4_end_1747() { var g = Game.Solo(seed: 69462); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8878_treaty4_end_1748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69563 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8879_treaty4_end_1749() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8880_treaty4_end_1750() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8881_treaty4_end_1751() { var g = Game.Solo(seed: 69466); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8882_treaty4_end_1752() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69567 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8883_treaty4_end_1753() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8884_treaty4_end_1754() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8885_treaty4_end_1755() { var g = Game.Solo(seed: 69470); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8886_treaty4_end_1756() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69571 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8887_treaty4_end_1757() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8888_treaty4_end_1758() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8889_treaty4_end_1759() { var g = Game.Solo(seed: 69474); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8890_treaty4_end_1760() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69575 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8891_treaty4_end_1761() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8892_treaty4_end_1762() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8893_treaty4_end_1763() { var g = Game.Solo(seed: 69478); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8894_treaty4_end_1764() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69579 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8895_treaty4_end_1765() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8896_treaty4_end_1766() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8897_treaty4_end_1767() { var g = Game.Solo(seed: 69482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8898_treaty4_end_1768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69583 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8899_treaty4_end_1769() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8900_treaty4_end_1770() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8901_treaty4_end_1771() { var g = Game.Solo(seed: 69486); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8902_treaty4_end_1772() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69587 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8903_treaty4_end_1773() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8904_treaty4_end_1774() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8905_treaty4_end_1775() { var g = Game.Solo(seed: 69490); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8906_treaty4_end_1776() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69591 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8907_treaty4_end_1777() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8908_treaty4_end_1778() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8909_treaty4_end_1779() { var g = Game.Solo(seed: 69494); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8910_treaty4_end_1780() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69595 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8911_treaty4_end_1781() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8912_treaty4_end_1782() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8913_treaty4_end_1783() { var g = Game.Solo(seed: 69498); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8914_treaty4_end_1784() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69599 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8915_treaty4_end_1785() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8916_treaty4_end_1786() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8917_treaty4_end_1787() { var g = Game.Solo(seed: 69502); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8918_treaty4_end_1788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69603 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8919_treaty4_end_1789() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8920_treaty4_end_1790() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8921_treaty4_end_1791() { var g = Game.Solo(seed: 69506); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8922_treaty4_end_1792() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69607 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8923_treaty4_end_1793() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8924_treaty4_end_1794() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8925_treaty4_end_1795() { var g = Game.Solo(seed: 69510); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8926_treaty4_end_1796() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69611 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8927_treaty4_end_1797() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8928_treaty4_end_1798() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8929_treaty4_end_1799() { var g = Game.Solo(seed: 69514); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8930_treaty4_end_1800() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69615 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8931_treaty4_end_1801() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8932_treaty4_end_1802() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8933_treaty4_end_1803() { var g = Game.Solo(seed: 69518); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8934_treaty4_end_1804() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69619 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8935_treaty4_end_1805() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8936_treaty4_end_1806() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8937_treaty4_end_1807() { var g = Game.Solo(seed: 69522); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8938_treaty4_end_1808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69623 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8939_treaty4_end_1809() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8940_treaty4_end_1810() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8941_treaty4_end_1811() { var g = Game.Solo(seed: 69526); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q8942_treaty4_end_1812() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 69627 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q8943_treaty4_end_1813() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q8944_treaty4_end_1814() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q8945_treaty4_end_1815() { var g = Game.Solo(seed: 69530); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
}
