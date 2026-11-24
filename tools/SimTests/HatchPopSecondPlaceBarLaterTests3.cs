using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7056–7125: batch 106 pop/second-place data bar (schema 29).</summary>
public class HatchPopSecondPlaceBarLaterTests3
{
    [Fact] public void Q7056_popsec3_1816() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7057_popsec3_1817() { var g = Game.Solo(seed: 44122); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7058_popsec3_1818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44223 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7059_popsec3_1819() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7060_popsec3_1820() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7061_popsec3_1821() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7062_popsec3_1822() { var g = Game.Solo(seed: 44127); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7063_popsec3_1823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44228 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7064_popsec3_1824() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7065_popsec3_1825() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7066_popsec3_1826() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7067_popsec3_1827() { var g = Game.Solo(seed: 44132); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7068_popsec3_1828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44233 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7069_popsec3_1829() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7070_popsec3_1830() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7071_popsec3_1831() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7072_popsec3_1832() { var g = Game.Solo(seed: 44137); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7073_popsec3_1833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44238 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7074_popsec3_1834() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7075_popsec3_1835() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7076_popsec3_1836() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7077_popsec3_1837() { var g = Game.Solo(seed: 44142); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7078_popsec3_1838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44243 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7079_popsec3_1839() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7080_popsec3_1840() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7081_popsec3_1841() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7082_popsec3_1842() { var g = Game.Solo(seed: 44147); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7083_popsec3_1843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44248 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7084_popsec3_1844() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7085_popsec3_1845() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7086_popsec3_1846() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7087_popsec3_1847() { var g = Game.Solo(seed: 44152); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7088_popsec3_1848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44253 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7089_popsec3_1849() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7090_popsec3_1850() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7091_popsec3_1851() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7092_popsec3_1852() { var g = Game.Solo(seed: 44157); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7093_popsec3_1853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44258 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7094_popsec3_1854() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7095_popsec3_1855() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7096_popsec3_1856() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7097_popsec3_1857() { var g = Game.Solo(seed: 44162); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7098_popsec3_1858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44263 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7099_popsec3_1859() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7100_popsec3_1860() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7101_popsec3_1861() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7102_popsec3_1862() { var g = Game.Solo(seed: 44167); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7103_popsec3_1863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44268 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7104_popsec3_1864() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7105_popsec3_1865() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7106_popsec3_1866() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7107_popsec3_1867() { var g = Game.Solo(seed: 44172); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7108_popsec3_1868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44273 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7109_popsec3_1869() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7110_popsec3_1870() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7111_popsec3_1871() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7112_popsec3_1872() { var g = Game.Solo(seed: 44177); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7113_popsec3_1873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44278 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7114_popsec3_1874() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7115_popsec3_1875() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7116_popsec3_1876() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7117_popsec3_1877() { var g = Game.Solo(seed: 44182); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7118_popsec3_1878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44283 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7119_popsec3_1879() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7120_popsec3_1880() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7121_popsec3_1881() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ThePopEndingForPlayer); Assert.True(c.Endings.SecondPlaceRunnerUpYears > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q7122_popsec3_1882() { var g = Game.Solo(seed: 44187); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("the-pop")); Assert.NotNull(g.C.Endings.Def("second-place")); }
    [Fact] public void Q7123_popsec3_1883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 44288 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q7124_popsec3_1884() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "second-place"); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q7125_popsec3_1885() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.Feelings.SecondPlacePermille != 0); Assert.True(c.Bubble.PopAtPermille > 0); }
}
