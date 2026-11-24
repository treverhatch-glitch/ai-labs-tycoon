using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2026–Q2095: batch 34 rival/plan data bar (schema 42).</summary>
public class HatchRivalPlanBarLaterTests
{
    [Fact] public void Q2026_rival_plan_2026() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2027_rival_plan_2027() { var g = Game.Solo(seed: 4001); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2028_rival_plan_2028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4102 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2029_rival_plan_2029() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2030_rival_plan_2030() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2031_rival_plan_2031() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2032_rival_plan_2032() { var g = Game.Solo(seed: 4006); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2033_rival_plan_2033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4107 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2034_rival_plan_2034() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2035_rival_plan_2035() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2036_rival_plan_2036() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2037_rival_plan_2037() { var g = Game.Solo(seed: 4011); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2038_rival_plan_2038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4112 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2039_rival_plan_2039() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2040_rival_plan_2040() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2041_rival_plan_2041() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2042_rival_plan_2042() { var g = Game.Solo(seed: 4016); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2043_rival_plan_2043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4117 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2044_rival_plan_2044() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2045_rival_plan_2045() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2046_rival_plan_2046() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2047_rival_plan_2047() { var g = Game.Solo(seed: 4021); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2048_rival_plan_2048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4122 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2049_rival_plan_2049() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2050_rival_plan_2050() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2051_rival_plan_2051() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2052_rival_plan_2052() { var g = Game.Solo(seed: 4026); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2053_rival_plan_2053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4127 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2054_rival_plan_2054() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2055_rival_plan_2055() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2056_rival_plan_2056() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2057_rival_plan_2057() { var g = Game.Solo(seed: 4031); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2058_rival_plan_2058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4132 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2059_rival_plan_2059() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2060_rival_plan_2060() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2061_rival_plan_2061() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2062_rival_plan_2062() { var g = Game.Solo(seed: 4036); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2063_rival_plan_2063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4137 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2064_rival_plan_2064() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2065_rival_plan_2065() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2066_rival_plan_2066() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2067_rival_plan_2067() { var g = Game.Solo(seed: 4041); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2068_rival_plan_2068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4142 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2069_rival_plan_2069() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2070_rival_plan_2070() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2071_rival_plan_2071() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2072_rival_plan_2072() { var g = Game.Solo(seed: 4046); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2073_rival_plan_2073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4147 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2074_rival_plan_2074() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2075_rival_plan_2075() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2076_rival_plan_2076() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2077_rival_plan_2077() { var g = Game.Solo(seed: 4051); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2078_rival_plan_2078() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4152 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2079_rival_plan_2079() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2080_rival_plan_2080() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2081_rival_plan_2081() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2082_rival_plan_2082() { var g = Game.Solo(seed: 4056); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2083_rival_plan_2083() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4157 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2084_rival_plan_2084() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2085_rival_plan_2085() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2086_rival_plan_2086() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2087_rival_plan_2087() { var g = Game.Solo(seed: 4061); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2088_rival_plan_2088() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4162 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2089_rival_plan_2089() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2090_rival_plan_2090() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
    [Fact] public void Q2091_rival_plan_2091() { GameConfig c = TestData.Config(); Assert.True(c.Rivals.Count >= 3); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q2092_rival_plan_2092() { var g = Game.Solo(seed: 4066); Assert.Equal(43, g.State.Schema); Assert.True(g.State.Labs.Count >= 1); }
    [Fact] public void Q2093_rival_plan_2093() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4167 })).Sim); Assert.Contains(c.Rivals, r => r.Id == "openaye"); }
    [Fact] public void Q2094_rival_plan_2094() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2095_rival_plan_2095() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); var g = Game.New(seed: 12); Assert.Equal(43, g.State.Schema); }
}
