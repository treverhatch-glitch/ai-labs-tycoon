using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6986–7055: batch 105 rival/plan data bar coda (schema 29).</summary>
public class HatchRivalPlanBarLaterTests6
{
    [Fact] public void Q6986_rivplan6_3286() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q6987_rivplan6_3287() { var g = Game.Solo(seed: 43142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q6988_rivplan6_3288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43243 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q6989_rivplan6_3289() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6990_rivplan6_3290() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q6991_rivplan6_3291() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q6992_rivplan6_3292() { var g = Game.Solo(seed: 43147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q6993_rivplan6_3293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43248 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q6994_rivplan6_3294() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6995_rivplan6_3295() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q6996_rivplan6_3296() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q6997_rivplan6_3297() { var g = Game.Solo(seed: 43152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q6998_rivplan6_3298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43253 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q6999_rivplan6_3299() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7000_rivplan6_3300() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7001_rivplan6_3301() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7002_rivplan6_3302() { var g = Game.Solo(seed: 43157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7003_rivplan6_3303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43258 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7004_rivplan6_3304() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7005_rivplan6_3305() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7006_rivplan6_3306() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7007_rivplan6_3307() { var g = Game.Solo(seed: 43162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7008_rivplan6_3308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43263 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7009_rivplan6_3309() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7010_rivplan6_3310() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7011_rivplan6_3311() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7012_rivplan6_3312() { var g = Game.Solo(seed: 43167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7013_rivplan6_3313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43268 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7014_rivplan6_3314() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7015_rivplan6_3315() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7016_rivplan6_3316() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7017_rivplan6_3317() { var g = Game.Solo(seed: 43172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7018_rivplan6_3318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43273 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7019_rivplan6_3319() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7020_rivplan6_3320() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7021_rivplan6_3321() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7022_rivplan6_3322() { var g = Game.Solo(seed: 43177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7023_rivplan6_3323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43278 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7024_rivplan6_3324() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7025_rivplan6_3325() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7026_rivplan6_3326() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7027_rivplan6_3327() { var g = Game.Solo(seed: 43182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7028_rivplan6_3328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43283 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7029_rivplan6_3329() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7030_rivplan6_3330() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7031_rivplan6_3331() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7032_rivplan6_3332() { var g = Game.Solo(seed: 43187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7033_rivplan6_3333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43288 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7034_rivplan6_3334() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7035_rivplan6_3335() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7036_rivplan6_3336() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7037_rivplan6_3337() { var g = Game.Solo(seed: 43192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7038_rivplan6_3338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43293 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7039_rivplan6_3339() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7040_rivplan6_3340() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7041_rivplan6_3341() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7042_rivplan6_3342() { var g = Game.Solo(seed: 43197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7043_rivplan6_3343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43298 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7044_rivplan6_3344() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7045_rivplan6_3345() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7046_rivplan6_3346() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7047_rivplan6_3347() { var g = Game.Solo(seed: 43202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7048_rivplan6_3348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43303 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7049_rivplan6_3349() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7050_rivplan6_3350() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q7051_rivplan6_3351() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q7052_rivplan6_3352() { var g = Game.Solo(seed: 43207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q7053_rivplan6_3353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 43308 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q7054_rivplan6_3354() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q7055_rivplan6_3355() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 87); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
}
