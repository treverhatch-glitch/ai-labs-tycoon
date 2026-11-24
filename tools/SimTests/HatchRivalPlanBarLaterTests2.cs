using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3286–Q3355: batch 52 rival/plan data bar coda (schema 34).</summary>
public class HatchRivalPlanBarLaterTests2
{
    [Fact] public void Q3286_rivplan2_3286() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3287_rivplan2_3287() { var g = Game.Solo(seed: 7602); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3288_rivplan2_3288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7704 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3289_rivplan2_3289() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3290_rivplan2_3290() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3291_rivplan2_3291() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3292_rivplan2_3292() { var g = Game.Solo(seed: 7607); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3293_rivplan2_3293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7709 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3294_rivplan2_3294() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3295_rivplan2_3295() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3296_rivplan2_3296() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3297_rivplan2_3297() { var g = Game.Solo(seed: 7612); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3298_rivplan2_3298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7714 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3299_rivplan2_3299() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3300_rivplan2_3300() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3301_rivplan2_3301() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3302_rivplan2_3302() { var g = Game.Solo(seed: 7617); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3303_rivplan2_3303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7719 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3304_rivplan2_3304() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3305_rivplan2_3305() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3306_rivplan2_3306() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3307_rivplan2_3307() { var g = Game.Solo(seed: 7622); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3308_rivplan2_3308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7724 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3309_rivplan2_3309() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3310_rivplan2_3310() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3311_rivplan2_3311() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3312_rivplan2_3312() { var g = Game.Solo(seed: 7627); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3313_rivplan2_3313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7729 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3314_rivplan2_3314() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3315_rivplan2_3315() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3316_rivplan2_3316() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3317_rivplan2_3317() { var g = Game.Solo(seed: 7632); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3318_rivplan2_3318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7734 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3319_rivplan2_3319() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3320_rivplan2_3320() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3321_rivplan2_3321() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3322_rivplan2_3322() { var g = Game.Solo(seed: 7637); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3323_rivplan2_3323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7739 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3324_rivplan2_3324() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3325_rivplan2_3325() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3326_rivplan2_3326() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3327_rivplan2_3327() { var g = Game.Solo(seed: 7642); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3328_rivplan2_3328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7744 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3329_rivplan2_3329() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3330_rivplan2_3330() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3331_rivplan2_3331() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3332_rivplan2_3332() { var g = Game.Solo(seed: 7647); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3333_rivplan2_3333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7749 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3334_rivplan2_3334() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3335_rivplan2_3335() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3336_rivplan2_3336() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3337_rivplan2_3337() { var g = Game.Solo(seed: 7652); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3338_rivplan2_3338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7754 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3339_rivplan2_3339() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3340_rivplan2_3340() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3341_rivplan2_3341() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3342_rivplan2_3342() { var g = Game.Solo(seed: 7657); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3343_rivplan2_3343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7759 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3344_rivplan2_3344() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3345_rivplan2_3345() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3346_rivplan2_3346() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3347_rivplan2_3347() { var g = Game.Solo(seed: 7662); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3348_rivplan2_3348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7764 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3349_rivplan2_3349() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3350_rivplan2_3350() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
    [Fact] public void Q3351_rivplan2_3351() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q3352_rivplan2_3352() { var g = Game.Solo(seed: 7667); Assert.Equal(34, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q3353_rivplan2_3353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 7769 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q3354_rivplan2_3354() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q3355_rivplan2_3355() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 52); Assert.Equal(34, g.State.Schema); }
}
