using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14336–14405: batch 284 treaty ending data bar (schema 29).</summary>
public class HatchTreatyEndingBarLaterTests13
{
    [Fact] public void Q14336_treaty13_end_2096() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14337_treaty13_end_2097() { var g = Game.Solo(seed: 227162); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14338_treaty13_end_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227263 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14339_treaty13_end_2099() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14340_treaty13_end_2100() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14341_treaty13_end_2101() { var g = Game.Solo(seed: 227166); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14342_treaty13_end_2102() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227267 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14343_treaty13_end_2103() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14344_treaty13_end_2104() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14345_treaty13_end_2105() { var g = Game.Solo(seed: 227170); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14346_treaty13_end_2106() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227271 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14347_treaty13_end_2107() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14348_treaty13_end_2108() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14349_treaty13_end_2109() { var g = Game.Solo(seed: 227174); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14350_treaty13_end_2110() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227275 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14351_treaty13_end_2111() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14352_treaty13_end_2112() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14353_treaty13_end_2113() { var g = Game.Solo(seed: 227178); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14354_treaty13_end_2114() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227279 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14355_treaty13_end_2115() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14356_treaty13_end_2116() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14357_treaty13_end_2117() { var g = Game.Solo(seed: 227182); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14358_treaty13_end_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227283 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14359_treaty13_end_2119() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14360_treaty13_end_2120() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14361_treaty13_end_2121() { var g = Game.Solo(seed: 227186); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14362_treaty13_end_2122() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227287 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14363_treaty13_end_2123() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14364_treaty13_end_2124() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14365_treaty13_end_2125() { var g = Game.Solo(seed: 227190); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14366_treaty13_end_2126() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227291 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14367_treaty13_end_2127() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14368_treaty13_end_2128() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14369_treaty13_end_2129() { var g = Game.Solo(seed: 227194); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14370_treaty13_end_2130() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227295 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14371_treaty13_end_2131() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14372_treaty13_end_2132() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14373_treaty13_end_2133() { var g = Game.Solo(seed: 227198); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14374_treaty13_end_2134() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227299 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14375_treaty13_end_2135() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14376_treaty13_end_2136() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14377_treaty13_end_2137() { var g = Game.Solo(seed: 227202); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14378_treaty13_end_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227303 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14379_treaty13_end_2139() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14380_treaty13_end_2140() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14381_treaty13_end_2141() { var g = Game.Solo(seed: 227206); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14382_treaty13_end_2142() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227307 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14383_treaty13_end_2143() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14384_treaty13_end_2144() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14385_treaty13_end_2145() { var g = Game.Solo(seed: 227210); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14386_treaty13_end_2146() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227311 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14387_treaty13_end_2147() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14388_treaty13_end_2148() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14389_treaty13_end_2149() { var g = Game.Solo(seed: 227214); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14390_treaty13_end_2150() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227315 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14391_treaty13_end_2151() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14392_treaty13_end_2152() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14393_treaty13_end_2153() { var g = Game.Solo(seed: 227218); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14394_treaty13_end_2154() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227319 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14395_treaty13_end_2155() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14396_treaty13_end_2156() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14397_treaty13_end_2157() { var g = Game.Solo(seed: 227222); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14398_treaty13_end_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227323 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14399_treaty13_end_2159() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14400_treaty13_end_2160() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14401_treaty13_end_2161() { var g = Game.Solo(seed: 227226); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14402_treaty13_end_2162() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 227327 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14403_treaty13_end_2163() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14404_treaty13_end_2164() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14405_treaty13_end_2165() { var g = Game.Solo(seed: 227230); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
}
