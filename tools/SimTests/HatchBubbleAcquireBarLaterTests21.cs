using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14836–14905: batch 320 bubble/acquire data bar (schema 29).</summary>
public class HatchBubbleAcquireBarLaterTests21
{
    [Fact] public void Q14836_bubacq21_4896() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14837_bubacq21_4897() { var g = Game.Solo(seed: 301082); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14838_bubacq21_4898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301183 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14839_bubacq21_4899() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14840_bubacq21_4900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14841_bubacq21_4901() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14842_bubacq21_4902() { var g = Game.Solo(seed: 301087); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14843_bubacq21_4903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301188 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14844_bubacq21_4904() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14845_bubacq21_4905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14846_bubacq21_4906() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14847_bubacq21_4907() { var g = Game.Solo(seed: 301092); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14848_bubacq21_4908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301193 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14849_bubacq21_4909() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14850_bubacq21_4910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14851_bubacq21_4911() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14852_bubacq21_4912() { var g = Game.Solo(seed: 301097); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14853_bubacq21_4913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301198 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14854_bubacq21_4914() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14855_bubacq21_4915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14856_bubacq21_4916() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14857_bubacq21_4917() { var g = Game.Solo(seed: 301102); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14858_bubacq21_4918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301203 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14859_bubacq21_4919() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14860_bubacq21_4920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14861_bubacq21_4921() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14862_bubacq21_4922() { var g = Game.Solo(seed: 301107); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14863_bubacq21_4923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301208 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14864_bubacq21_4924() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14865_bubacq21_4925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14866_bubacq21_4926() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14867_bubacq21_4927() { var g = Game.Solo(seed: 301112); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14868_bubacq21_4928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301213 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14869_bubacq21_4929() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14870_bubacq21_4930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14871_bubacq21_4931() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14872_bubacq21_4932() { var g = Game.Solo(seed: 301117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14873_bubacq21_4933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301218 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14874_bubacq21_4934() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14875_bubacq21_4935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14876_bubacq21_4936() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14877_bubacq21_4937() { var g = Game.Solo(seed: 301122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14878_bubacq21_4938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301223 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14879_bubacq21_4939() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14880_bubacq21_4940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14881_bubacq21_4941() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14882_bubacq21_4942() { var g = Game.Solo(seed: 301127); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14883_bubacq21_4943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301228 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14884_bubacq21_4944() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14885_bubacq21_4945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14886_bubacq21_4946() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14887_bubacq21_4947() { var g = Game.Solo(seed: 301132); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14888_bubacq21_4948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301233 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14889_bubacq21_4949() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14890_bubacq21_4950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14891_bubacq21_4951() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14892_bubacq21_4952() { var g = Game.Solo(seed: 301137); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14893_bubacq21_4953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301238 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14894_bubacq21_4954() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14895_bubacq21_4955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14896_bubacq21_4956() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14897_bubacq21_4957() { var g = Game.Solo(seed: 301142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14898_bubacq21_4958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301243 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14899_bubacq21_4959() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14900_bubacq21_4960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14901_bubacq21_4961() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14902_bubacq21_4962() { var g = Game.Solo(seed: 301147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14903_bubacq21_4963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 301248 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14904_bubacq21_4964() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14905_bubacq21_4965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158774); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
