using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q15256–15325: batch 354 (CurrentSchema).</summary>
public class HatchRivalPlanBarLaterTests7
{
    [Fact] public void Q15256_rivplan7_11556() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15257_rivplan7_11557() { var g = Game.Solo(seed: 43142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15258_rivplan7_11558() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43243 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15259_rivplan7_11559() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15260_rivplan7_11560() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15261_rivplan7_11561() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15262_rivplan7_11562() { var g = Game.Solo(seed: 43147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15263_rivplan7_11563() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43248 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15264_rivplan7_11564() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15265_rivplan7_11565() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15266_rivplan7_11566() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15267_rivplan7_11567() { var g = Game.Solo(seed: 43152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15268_rivplan7_11568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43253 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15269_rivplan7_11569() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15270_rivplan7_11570() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15271_rivplan7_11571() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15272_rivplan7_11572() { var g = Game.Solo(seed: 43157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15273_rivplan7_11573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43258 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15274_rivplan7_11574() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15275_rivplan7_11575() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15276_rivplan7_11576() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15277_rivplan7_11577() { var g = Game.Solo(seed: 43162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15278_rivplan7_11578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43263 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15279_rivplan7_11579() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15280_rivplan7_11580() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15281_rivplan7_11581() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15282_rivplan7_11582() { var g = Game.Solo(seed: 43167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15283_rivplan7_11583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43268 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15284_rivplan7_11584() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15285_rivplan7_11585() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15286_rivplan7_11586() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15287_rivplan7_11587() { var g = Game.Solo(seed: 43172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15288_rivplan7_11588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43273 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15289_rivplan7_11589() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15290_rivplan7_11590() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15291_rivplan7_11591() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15292_rivplan7_11592() { var g = Game.Solo(seed: 43177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15293_rivplan7_11593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43278 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15294_rivplan7_11594() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15295_rivplan7_11595() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15296_rivplan7_11596() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15297_rivplan7_11597() { var g = Game.Solo(seed: 43182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15298_rivplan7_11598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43283 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15299_rivplan7_11599() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15300_rivplan7_11600() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15301_rivplan7_11601() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15302_rivplan7_11602() { var g = Game.Solo(seed: 43187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15303_rivplan7_11603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43288 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15304_rivplan7_11604() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15305_rivplan7_11605() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15306_rivplan7_11606() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15307_rivplan7_11607() { var g = Game.Solo(seed: 43192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15308_rivplan7_11608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43293 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15309_rivplan7_11609() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15310_rivplan7_11610() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15311_rivplan7_11611() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15312_rivplan7_11612() { var g = Game.Solo(seed: 43197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15313_rivplan7_11613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43298 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15314_rivplan7_11614() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15315_rivplan7_11615() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15316_rivplan7_11616() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15317_rivplan7_11617() { var g = Game.Solo(seed: 43202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15318_rivplan7_11618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43303 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15319_rivplan7_11619() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15320_rivplan7_11620() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q15321_rivplan7_11621() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q15322_rivplan7_11622() { var g = Game.Solo(seed: 43207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q15323_rivplan7_11623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43308 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q15324_rivplan7_11624() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q15325_rivplan7_11625() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
}
