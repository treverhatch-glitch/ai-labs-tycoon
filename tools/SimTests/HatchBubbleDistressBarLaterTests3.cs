using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q4406–Q4475: batch 68 bubble/distress data bar coda (schema 29).</summary>
public class HatchBubbleDistressBarLaterTests3
{
    [Fact] public void Q4406_bubdst3_4406() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4407_bubdst3_4407() { var g = Game.Solo(seed: 6762); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4408_bubdst3_4408() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6864 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4409_bubdst3_4409() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4410_bubdst3_4410() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4411_bubdst3_4411() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4412_bubdst3_4412() { var g = Game.Solo(seed: 6767); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4413_bubdst3_4413() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6869 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4414_bubdst3_4414() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4415_bubdst3_4415() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4416_bubdst3_4416() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4417_bubdst3_4417() { var g = Game.Solo(seed: 6772); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4418_bubdst3_4418() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6874 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4419_bubdst3_4419() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4420_bubdst3_4420() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4421_bubdst3_4421() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4422_bubdst3_4422() { var g = Game.Solo(seed: 6777); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4423_bubdst3_4423() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6879 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4424_bubdst3_4424() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4425_bubdst3_4425() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4426_bubdst3_4426() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4427_bubdst3_4427() { var g = Game.Solo(seed: 6782); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4428_bubdst3_4428() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6884 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4429_bubdst3_4429() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4430_bubdst3_4430() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4431_bubdst3_4431() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4432_bubdst3_4432() { var g = Game.Solo(seed: 6787); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4433_bubdst3_4433() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6889 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4434_bubdst3_4434() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4435_bubdst3_4435() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4436_bubdst3_4436() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4437_bubdst3_4437() { var g = Game.Solo(seed: 6792); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4438_bubdst3_4438() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6894 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4439_bubdst3_4439() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4440_bubdst3_4440() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4441_bubdst3_4441() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4442_bubdst3_4442() { var g = Game.Solo(seed: 6797); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4443_bubdst3_4443() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6899 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4444_bubdst3_4444() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4445_bubdst3_4445() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4446_bubdst3_4446() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4447_bubdst3_4447() { var g = Game.Solo(seed: 6802); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4448_bubdst3_4448() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6904 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4449_bubdst3_4449() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4450_bubdst3_4450() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4451_bubdst3_4451() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4452_bubdst3_4452() { var g = Game.Solo(seed: 6807); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4453_bubdst3_4453() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6909 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4454_bubdst3_4454() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4455_bubdst3_4455() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4456_bubdst3_4456() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4457_bubdst3_4457() { var g = Game.Solo(seed: 6812); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4458_bubdst3_4458() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6914 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4459_bubdst3_4459() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4460_bubdst3_4460() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4461_bubdst3_4461() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4462_bubdst3_4462() { var g = Game.Solo(seed: 6817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4463_bubdst3_4463() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6919 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4464_bubdst3_4464() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4465_bubdst3_4465() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4466_bubdst3_4466() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4467_bubdst3_4467() { var g = Game.Solo(seed: 6822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4468_bubdst3_4468() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6924 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4469_bubdst3_4469() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4470_bubdst3_4470() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4471_bubdst3_4471() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); }
    [Fact] public void Q4472_bubdst3_4472() { var g = Game.Solo(seed: 6827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q4473_bubdst3_4473() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6929 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q4474_bubdst3_4474() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4475_bubdst3_4475() { GameConfig c = TestData.Config(); var g = Game.New(seed: 51); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
