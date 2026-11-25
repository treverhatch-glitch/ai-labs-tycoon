using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14756–14825: batch 332 treaty ending data bar (schema 29).</summary>
public class HatchTreatyEndingBarLaterTests19
{
    [Fact] public void Q14756_treaty19_end_2516() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14757_treaty19_end_2517() { var g = Game.Solo(seed: 332762); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14758_treaty19_end_2518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332863 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14759_treaty19_end_2519() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14760_treaty19_end_2520() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14761_treaty19_end_2521() { var g = Game.Solo(seed: 332766); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14762_treaty19_end_2522() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332867 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14763_treaty19_end_2523() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14764_treaty19_end_2524() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14765_treaty19_end_2525() { var g = Game.Solo(seed: 332770); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14766_treaty19_end_2526() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332871 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14767_treaty19_end_2527() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14768_treaty19_end_2528() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14769_treaty19_end_2529() { var g = Game.Solo(seed: 332774); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14770_treaty19_end_2530() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332875 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14771_treaty19_end_2531() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14772_treaty19_end_2532() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14773_treaty19_end_2533() { var g = Game.Solo(seed: 332778); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14774_treaty19_end_2534() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332879 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14775_treaty19_end_2535() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14776_treaty19_end_2536() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14777_treaty19_end_2537() { var g = Game.Solo(seed: 332782); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14778_treaty19_end_2538() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332883 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14779_treaty19_end_2539() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14780_treaty19_end_2540() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14781_treaty19_end_2541() { var g = Game.Solo(seed: 332786); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14782_treaty19_end_2542() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332887 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14783_treaty19_end_2543() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14784_treaty19_end_2544() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14785_treaty19_end_2545() { var g = Game.Solo(seed: 332790); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14786_treaty19_end_2546() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332891 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14787_treaty19_end_2547() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14788_treaty19_end_2548() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14789_treaty19_end_2549() { var g = Game.Solo(seed: 332794); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14790_treaty19_end_2550() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332895 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14791_treaty19_end_2551() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14792_treaty19_end_2552() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14793_treaty19_end_2553() { var g = Game.Solo(seed: 332798); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14794_treaty19_end_2554() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332899 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14795_treaty19_end_2555() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14796_treaty19_end_2556() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14797_treaty19_end_2557() { var g = Game.Solo(seed: 332802); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14798_treaty19_end_2558() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332903 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14799_treaty19_end_2559() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14800_treaty19_end_2560() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14801_treaty19_end_2561() { var g = Game.Solo(seed: 332806); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14802_treaty19_end_2562() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332907 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14803_treaty19_end_2563() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14804_treaty19_end_2564() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14805_treaty19_end_2565() { var g = Game.Solo(seed: 332810); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14806_treaty19_end_2566() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332911 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14807_treaty19_end_2567() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14808_treaty19_end_2568() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14809_treaty19_end_2569() { var g = Game.Solo(seed: 332814); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14810_treaty19_end_2570() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332915 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14811_treaty19_end_2571() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14812_treaty19_end_2572() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14813_treaty19_end_2573() { var g = Game.Solo(seed: 332818); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14814_treaty19_end_2574() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332919 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14815_treaty19_end_2575() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14816_treaty19_end_2576() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14817_treaty19_end_2577() { var g = Game.Solo(seed: 332822); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14818_treaty19_end_2578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332923 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14819_treaty19_end_2579() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14820_treaty19_end_2580() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14821_treaty19_end_2581() { var g = Game.Solo(seed: 332826); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14822_treaty19_end_2582() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 332927 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14823_treaty19_end_2583() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14824_treaty19_end_2584() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14825_treaty19_end_2585() { var g = Game.Solo(seed: 332830); Assert.Equal(29, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
}
