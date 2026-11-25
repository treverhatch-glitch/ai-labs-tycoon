using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14416–14485: batch 333 world-map economy data bar (schema 29).</summary>
public class HatchWorldMapEconomyBarLaterTests20
{
    [Fact] public void Q14416_hatchwor20_5456() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14417_hatchwor20_5457() { var g = Game.Solo(seed: 328962); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14418_hatchwor20_5458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329063 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14419_hatchwor20_5459() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14420_hatchwor20_5460() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14421_hatchwor20_5461() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14422_hatchwor20_5462() { var g = Game.Solo(seed: 328967); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14423_hatchwor20_5463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329068 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14424_hatchwor20_5464() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14425_hatchwor20_5465() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14426_hatchwor20_5466() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14427_hatchwor20_5467() { var g = Game.Solo(seed: 328972); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14428_hatchwor20_5468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329073 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14429_hatchwor20_5469() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14430_hatchwor20_5470() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14431_hatchwor20_5471() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14432_hatchwor20_5472() { var g = Game.Solo(seed: 328977); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14433_hatchwor20_5473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329078 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14434_hatchwor20_5474() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14435_hatchwor20_5475() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14436_hatchwor20_5476() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14437_hatchwor20_5477() { var g = Game.Solo(seed: 328982); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14438_hatchwor20_5478() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329083 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14439_hatchwor20_5479() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14440_hatchwor20_5480() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14441_hatchwor20_5481() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14442_hatchwor20_5482() { var g = Game.Solo(seed: 328987); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14443_hatchwor20_5483() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329088 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14444_hatchwor20_5484() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14445_hatchwor20_5485() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14446_hatchwor20_5486() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14447_hatchwor20_5487() { var g = Game.Solo(seed: 328992); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14448_hatchwor20_5488() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329093 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14449_hatchwor20_5489() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14450_hatchwor20_5490() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14451_hatchwor20_5491() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14452_hatchwor20_5492() { var g = Game.Solo(seed: 328997); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14453_hatchwor20_5493() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329098 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14454_hatchwor20_5494() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14455_hatchwor20_5495() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14456_hatchwor20_5496() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14457_hatchwor20_5497() { var g = Game.Solo(seed: 329002); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14458_hatchwor20_5498() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329103 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14459_hatchwor20_5499() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14460_hatchwor20_5500() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14461_hatchwor20_5501() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14462_hatchwor20_5502() { var g = Game.Solo(seed: 329007); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14463_hatchwor20_5503() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329108 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14464_hatchwor20_5504() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14465_hatchwor20_5505() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14466_hatchwor20_5506() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14467_hatchwor20_5507() { var g = Game.Solo(seed: 329012); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14468_hatchwor20_5508() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329113 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14469_hatchwor20_5509() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14470_hatchwor20_5510() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14471_hatchwor20_5511() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14472_hatchwor20_5512() { var g = Game.Solo(seed: 329017); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14473_hatchwor20_5513() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329118 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14474_hatchwor20_5514() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14475_hatchwor20_5515() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14476_hatchwor20_5516() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14477_hatchwor20_5517() { var g = Game.Solo(seed: 329022); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14478_hatchwor20_5518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329123 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14479_hatchwor20_5519() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14480_hatchwor20_5520() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14481_hatchwor20_5521() { GameConfig c = TestData.Config(); Assert.True(c.Eras.Count >= 4); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q14482_hatchwor20_5522() { var g = Game.Solo(seed: 329027); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.Count >= 4); }
    [Fact] public void Q14483_hatchwor20_5523() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 329128 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q14484_hatchwor20_5524() { GameConfig c = TestData.Config(); Assert.True(c.Funding.FounderStartPermille > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q14485_hatchwor20_5525() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194005); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
}
