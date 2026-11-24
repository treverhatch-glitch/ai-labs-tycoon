using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q5876–Q5945: batch 89 rival/plan data bar coda (schema 29).</summary>
public class HatchRivalPlanBarLaterTests5
{
    [Fact] public void Q5876_rivplan5_3286() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5877_rivplan5_3287() { var g = Game.Solo(seed: 27442); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5878_rivplan5_3288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27543 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5879_rivplan5_3289() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5880_rivplan5_3290() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5881_rivplan5_3291() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5882_rivplan5_3292() { var g = Game.Solo(seed: 27447); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5883_rivplan5_3293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27548 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5884_rivplan5_3294() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5885_rivplan5_3295() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5886_rivplan5_3296() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5887_rivplan5_3297() { var g = Game.Solo(seed: 27452); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5888_rivplan5_3298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27553 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5889_rivplan5_3299() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5890_rivplan5_3300() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5891_rivplan5_3301() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5892_rivplan5_3302() { var g = Game.Solo(seed: 27457); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5893_rivplan5_3303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27558 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5894_rivplan5_3304() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5895_rivplan5_3305() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5896_rivplan5_3306() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5897_rivplan5_3307() { var g = Game.Solo(seed: 27462); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5898_rivplan5_3308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27563 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5899_rivplan5_3309() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5900_rivplan5_3310() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5901_rivplan5_3311() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5902_rivplan5_3312() { var g = Game.Solo(seed: 27467); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5903_rivplan5_3313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27568 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5904_rivplan5_3314() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5905_rivplan5_3315() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5906_rivplan5_3316() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5907_rivplan5_3317() { var g = Game.Solo(seed: 27472); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5908_rivplan5_3318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27573 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5909_rivplan5_3319() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5910_rivplan5_3320() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5911_rivplan5_3321() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5912_rivplan5_3322() { var g = Game.Solo(seed: 27477); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5913_rivplan5_3323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27578 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5914_rivplan5_3324() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5915_rivplan5_3325() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5916_rivplan5_3326() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5917_rivplan5_3327() { var g = Game.Solo(seed: 27482); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5918_rivplan5_3328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27583 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5919_rivplan5_3329() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5920_rivplan5_3330() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5921_rivplan5_3331() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5922_rivplan5_3332() { var g = Game.Solo(seed: 27487); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5923_rivplan5_3333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27588 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5924_rivplan5_3334() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5925_rivplan5_3335() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5926_rivplan5_3336() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5927_rivplan5_3337() { var g = Game.Solo(seed: 27492); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5928_rivplan5_3338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27593 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5929_rivplan5_3339() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5930_rivplan5_3340() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5931_rivplan5_3341() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5932_rivplan5_3342() { var g = Game.Solo(seed: 27497); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5933_rivplan5_3343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27598 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5934_rivplan5_3344() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5935_rivplan5_3345() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5936_rivplan5_3346() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5937_rivplan5_3347() { var g = Game.Solo(seed: 27502); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5938_rivplan5_3348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27603 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5939_rivplan5_3349() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5940_rivplan5_3350() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
    [Fact] public void Q5941_rivplan5_3351() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5942_rivplan5_3352() { var g = Game.Solo(seed: 27507); Assert.Equal(29, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q5943_rivplan5_3353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27608 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q5944_rivplan5_3354() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5945_rivplan5_3355() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 71); Assert.Equal(29, g.State.Schema); }
}
