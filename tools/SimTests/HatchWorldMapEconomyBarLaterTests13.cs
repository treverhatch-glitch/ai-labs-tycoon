using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13926–13995: batch 277 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests13
{
    [Fact] public void Q13926_hatchwor13_4966() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13927_hatchwor13_4967() { var g = Game.Solo(seed: 205762); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13928_hatchwor13_4968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205863 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13929_hatchwor13_4969() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13930_hatchwor13_4970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13931_hatchwor13_4971() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13932_hatchwor13_4972() { var g = Game.Solo(seed: 205767); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13933_hatchwor13_4973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205868 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13934_hatchwor13_4974() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13935_hatchwor13_4975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13936_hatchwor13_4976() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13937_hatchwor13_4977() { var g = Game.Solo(seed: 205772); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13938_hatchwor13_4978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205873 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13939_hatchwor13_4979() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13940_hatchwor13_4980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13941_hatchwor13_4981() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13942_hatchwor13_4982() { var g = Game.Solo(seed: 205777); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13943_hatchwor13_4983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205878 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13944_hatchwor13_4984() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13945_hatchwor13_4985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13946_hatchwor13_4986() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13947_hatchwor13_4987() { var g = Game.Solo(seed: 205782); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13948_hatchwor13_4988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205883 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13949_hatchwor13_4989() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13950_hatchwor13_4990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13951_hatchwor13_4991() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13952_hatchwor13_4992() { var g = Game.Solo(seed: 205787); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13953_hatchwor13_4993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205888 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13954_hatchwor13_4994() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13955_hatchwor13_4995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13956_hatchwor13_4996() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13957_hatchwor13_4997() { var g = Game.Solo(seed: 205792); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13958_hatchwor13_4998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205893 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13959_hatchwor13_4999() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13960_hatchwor13_5000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13961_hatchwor13_5001() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13962_hatchwor13_5002() { var g = Game.Solo(seed: 205797); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13963_hatchwor13_5003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205898 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13964_hatchwor13_5004() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13965_hatchwor13_5005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13966_hatchwor13_5006() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13967_hatchwor13_5007() { var g = Game.Solo(seed: 205802); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13968_hatchwor13_5008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205903 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13969_hatchwor13_5009() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13970_hatchwor13_5010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13971_hatchwor13_5011() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13972_hatchwor13_5012() { var g = Game.Solo(seed: 205807); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13973_hatchwor13_5013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205908 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13974_hatchwor13_5014() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13975_hatchwor13_5015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13976_hatchwor13_5016() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13977_hatchwor13_5017() { var g = Game.Solo(seed: 205812); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13978_hatchwor13_5018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205913 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13979_hatchwor13_5019() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13980_hatchwor13_5020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13981_hatchwor13_5021() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13982_hatchwor13_5022() { var g = Game.Solo(seed: 205817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13983_hatchwor13_5023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205918 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13984_hatchwor13_5024() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13985_hatchwor13_5025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13986_hatchwor13_5026() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13987_hatchwor13_5027() { var g = Game.Solo(seed: 205822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13988_hatchwor13_5028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205923 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13989_hatchwor13_5029() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13990_hatchwor13_5030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13991_hatchwor13_5031() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13992_hatchwor13_5032() { var g = Game.Solo(seed: 205827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q13993_hatchwor13_5033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 205928 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q13994_hatchwor13_5034() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q13995_hatchwor13_5035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 70665); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
