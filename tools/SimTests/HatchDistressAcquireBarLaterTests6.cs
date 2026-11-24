using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6846–6915: batch 103 distress/acquire data bar (schema 29).</summary>
public class HatchDistressAcquireBarLaterTests6
{
    [Fact] public void Q6846_dstacq6_2166() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6847_dstacq6_2167() { var g = Game.Solo(seed: 41182); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6848_dstacq6_2168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41283 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6849_dstacq6_2169() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6850_dstacq6_2170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6851_dstacq6_2171() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6852_dstacq6_2172() { var g = Game.Solo(seed: 41187); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6853_dstacq6_2173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41288 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6854_dstacq6_2174() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6855_dstacq6_2175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6856_dstacq6_2176() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6857_dstacq6_2177() { var g = Game.Solo(seed: 41192); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6858_dstacq6_2178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41293 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6859_dstacq6_2179() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6860_dstacq6_2180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6861_dstacq6_2181() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6862_dstacq6_2182() { var g = Game.Solo(seed: 41197); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6863_dstacq6_2183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41298 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6864_dstacq6_2184() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6865_dstacq6_2185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6866_dstacq6_2186() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6867_dstacq6_2187() { var g = Game.Solo(seed: 41202); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6868_dstacq6_2188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41303 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6869_dstacq6_2189() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6870_dstacq6_2190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6871_dstacq6_2191() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6872_dstacq6_2192() { var g = Game.Solo(seed: 41207); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6873_dstacq6_2193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41308 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6874_dstacq6_2194() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6875_dstacq6_2195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6876_dstacq6_2196() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6877_dstacq6_2197() { var g = Game.Solo(seed: 41212); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6878_dstacq6_2198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41313 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6879_dstacq6_2199() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6880_dstacq6_2200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6881_dstacq6_2201() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6882_dstacq6_2202() { var g = Game.Solo(seed: 41217); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6883_dstacq6_2203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41318 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6884_dstacq6_2204() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6885_dstacq6_2205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6886_dstacq6_2206() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6887_dstacq6_2207() { var g = Game.Solo(seed: 41222); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6888_dstacq6_2208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41323 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6889_dstacq6_2209() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6890_dstacq6_2210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6891_dstacq6_2211() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6892_dstacq6_2212() { var g = Game.Solo(seed: 41227); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6893_dstacq6_2213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41328 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6894_dstacq6_2214() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6895_dstacq6_2215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6896_dstacq6_2216() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6897_dstacq6_2217() { var g = Game.Solo(seed: 41232); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6898_dstacq6_2218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41333 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6899_dstacq6_2219() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6900_dstacq6_2220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6901_dstacq6_2221() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6902_dstacq6_2222() { var g = Game.Solo(seed: 41237); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6903_dstacq6_2223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41338 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6904_dstacq6_2224() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6905_dstacq6_2225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6906_dstacq6_2226() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6907_dstacq6_2227() { var g = Game.Solo(seed: 41242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6908_dstacq6_2228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41343 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6909_dstacq6_2229() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6910_dstacq6_2230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q6911_dstacq6_2231() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q6912_dstacq6_2232() { var g = Game.Solo(seed: 41247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q6913_dstacq6_2233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 41348 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q6914_dstacq6_2234() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q6915_dstacq6_2235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 85); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
}
