using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14476–14545: batch 300 treaty ending data bar (schema 29).</summary>
public class HatchTreatyEndingBarLaterTests15
{
    [Fact] public void Q14476_treaty15_end_2236() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14477_treaty15_end_2237() { var g = Game.Solo(seed: 262362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14478_treaty15_end_2238() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262463 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14479_treaty15_end_2239() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14480_treaty15_end_2240() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14481_treaty15_end_2241() { var g = Game.Solo(seed: 262366); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14482_treaty15_end_2242() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262467 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14483_treaty15_end_2243() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14484_treaty15_end_2244() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14485_treaty15_end_2245() { var g = Game.Solo(seed: 262370); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14486_treaty15_end_2246() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262471 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14487_treaty15_end_2247() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14488_treaty15_end_2248() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14489_treaty15_end_2249() { var g = Game.Solo(seed: 262374); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14490_treaty15_end_2250() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262475 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14491_treaty15_end_2251() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14492_treaty15_end_2252() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14493_treaty15_end_2253() { var g = Game.Solo(seed: 262378); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14494_treaty15_end_2254() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262479 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14495_treaty15_end_2255() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14496_treaty15_end_2256() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14497_treaty15_end_2257() { var g = Game.Solo(seed: 262382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14498_treaty15_end_2258() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262483 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14499_treaty15_end_2259() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14500_treaty15_end_2260() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14501_treaty15_end_2261() { var g = Game.Solo(seed: 262386); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14502_treaty15_end_2262() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262487 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14503_treaty15_end_2263() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14504_treaty15_end_2264() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14505_treaty15_end_2265() { var g = Game.Solo(seed: 262390); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14506_treaty15_end_2266() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262491 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14507_treaty15_end_2267() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14508_treaty15_end_2268() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14509_treaty15_end_2269() { var g = Game.Solo(seed: 262394); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14510_treaty15_end_2270() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262495 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14511_treaty15_end_2271() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14512_treaty15_end_2272() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14513_treaty15_end_2273() { var g = Game.Solo(seed: 262398); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14514_treaty15_end_2274() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262499 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14515_treaty15_end_2275() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14516_treaty15_end_2276() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14517_treaty15_end_2277() { var g = Game.Solo(seed: 262402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14518_treaty15_end_2278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262503 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14519_treaty15_end_2279() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14520_treaty15_end_2280() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14521_treaty15_end_2281() { var g = Game.Solo(seed: 262406); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14522_treaty15_end_2282() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262507 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14523_treaty15_end_2283() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14524_treaty15_end_2284() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14525_treaty15_end_2285() { var g = Game.Solo(seed: 262410); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14526_treaty15_end_2286() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262511 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14527_treaty15_end_2287() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14528_treaty15_end_2288() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14529_treaty15_end_2289() { var g = Game.Solo(seed: 262414); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14530_treaty15_end_2290() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262515 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14531_treaty15_end_2291() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14532_treaty15_end_2292() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14533_treaty15_end_2293() { var g = Game.Solo(seed: 262418); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14534_treaty15_end_2294() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262519 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14535_treaty15_end_2295() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14536_treaty15_end_2296() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14537_treaty15_end_2297() { var g = Game.Solo(seed: 262422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14538_treaty15_end_2298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262523 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14539_treaty15_end_2299() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14540_treaty15_end_2300() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14541_treaty15_end_2301() { var g = Game.Solo(seed: 262426); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
    [Fact] public void Q14542_treaty15_end_2302() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 262527 })).Sim); Assert.True(c.Endings.SecondPlaceRunnerUpYears >= 0); }
    [Fact] public void Q14543_treaty15_end_2303() { GameConfig c = TestData.Config(); Assert.Contains(c.Endings.Endings, e => e.Id == "monopoly"); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14544_treaty15_end_2304() { GameConfig c = TestData.Config(); Assert.True(c.Endings.TreatyForeignBlocCount > 0); Assert.True(c.Endings.NationalisedHomeLevelMin >= BlocStanding.Freeze); }
    [Fact] public void Q14545_treaty15_end_2305() { var g = Game.Solo(seed: 262430); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(g.C.Endings.Def("monopoly")); }
}
