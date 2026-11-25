using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14266–14335: batch 276 treaty ending data bar (schema 29).</summary>
public class HatchTreatyEndingBarLaterTests12
{
    [Fact] public void Q14266_treaty12_end_2026() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14267_treaty12_end_2027() { var g = Game.Solo(seed: 209562); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14268_treaty12_end_2028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209663 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14269_treaty12_end_2029() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14270_treaty12_end_2030() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14271_treaty12_end_2031() { var g = Game.Solo(seed: 209566); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14272_treaty12_end_2032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209667 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14273_treaty12_end_2033() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14274_treaty12_end_2034() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14275_treaty12_end_2035() { var g = Game.Solo(seed: 209570); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14276_treaty12_end_2036() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209671 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14277_treaty12_end_2037() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14278_treaty12_end_2038() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14279_treaty12_end_2039() { var g = Game.Solo(seed: 209574); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14280_treaty12_end_2040() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209675 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14281_treaty12_end_2041() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14282_treaty12_end_2042() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14283_treaty12_end_2043() { var g = Game.Solo(seed: 209578); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14284_treaty12_end_2044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209679 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14285_treaty12_end_2045() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14286_treaty12_end_2046() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14287_treaty12_end_2047() { var g = Game.Solo(seed: 209582); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14288_treaty12_end_2048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209683 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14289_treaty12_end_2049() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14290_treaty12_end_2050() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14291_treaty12_end_2051() { var g = Game.Solo(seed: 209586); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14292_treaty12_end_2052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209687 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14293_treaty12_end_2053() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14294_treaty12_end_2054() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14295_treaty12_end_2055() { var g = Game.Solo(seed: 209590); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14296_treaty12_end_2056() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209691 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14297_treaty12_end_2057() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14298_treaty12_end_2058() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14299_treaty12_end_2059() { var g = Game.Solo(seed: 209594); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14300_treaty12_end_2060() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209695 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14301_treaty12_end_2061() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14302_treaty12_end_2062() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14303_treaty12_end_2063() { var g = Game.Solo(seed: 209598); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14304_treaty12_end_2064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209699 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14305_treaty12_end_2065() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14306_treaty12_end_2066() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14307_treaty12_end_2067() { var g = Game.Solo(seed: 209602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14308_treaty12_end_2068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209703 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14309_treaty12_end_2069() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14310_treaty12_end_2070() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14311_treaty12_end_2071() { var g = Game.Solo(seed: 209606); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14312_treaty12_end_2072() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209707 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14313_treaty12_end_2073() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14314_treaty12_end_2074() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14315_treaty12_end_2075() { var g = Game.Solo(seed: 209610); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14316_treaty12_end_2076() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209711 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14317_treaty12_end_2077() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14318_treaty12_end_2078() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14319_treaty12_end_2079() { var g = Game.Solo(seed: 209614); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14320_treaty12_end_2080() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209715 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14321_treaty12_end_2081() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14322_treaty12_end_2082() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14323_treaty12_end_2083() { var g = Game.Solo(seed: 209618); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14324_treaty12_end_2084() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209719 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14325_treaty12_end_2085() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14326_treaty12_end_2086() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14327_treaty12_end_2087() { var g = Game.Solo(seed: 209622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14328_treaty12_end_2088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209723 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14329_treaty12_end_2089() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14330_treaty12_end_2090() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14331_treaty12_end_2091() { var g = Game.Solo(seed: 209626); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14332_treaty12_end_2092() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 209727 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14333_treaty12_end_2093() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14334_treaty12_end_2094() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14335_treaty12_end_2095() { var g = Game.Solo(seed: 209630); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
}
