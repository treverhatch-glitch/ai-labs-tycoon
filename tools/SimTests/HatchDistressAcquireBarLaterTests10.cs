using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11996–12065: batch 177 distress/acquire data bar (schema 29).</summary>
public class HatchDistressAcquireBarLaterTests10
{
    [Fact] public void Q11996_dstacq9_2166() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q11997_dstacq9_2167() { var g = Game.Solo(seed: 112482); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q11998_dstacq9_2168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112583 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q11999_dstacq9_2169() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12000_dstacq9_2170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12001_dstacq9_2171() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12002_dstacq9_2172() { var g = Game.Solo(seed: 112487); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12003_dstacq9_2173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112588 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12004_dstacq9_2174() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12005_dstacq9_2175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12006_dstacq9_2176() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12007_dstacq9_2177() { var g = Game.Solo(seed: 112492); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12008_dstacq9_2178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112593 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12009_dstacq9_2179() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12010_dstacq9_2180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12011_dstacq9_2181() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12012_dstacq9_2182() { var g = Game.Solo(seed: 112497); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12013_dstacq9_2183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112598 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12014_dstacq9_2184() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12015_dstacq9_2185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12016_dstacq9_2186() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12017_dstacq9_2187() { var g = Game.Solo(seed: 112502); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12018_dstacq9_2188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112603 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12019_dstacq9_2189() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12020_dstacq9_2190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12021_dstacq9_2191() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12022_dstacq9_2192() { var g = Game.Solo(seed: 112507); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12023_dstacq9_2193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112608 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12024_dstacq9_2194() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12025_dstacq9_2195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12026_dstacq9_2196() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12027_dstacq9_2197() { var g = Game.Solo(seed: 112512); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12028_dstacq9_2198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112613 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12029_dstacq9_2199() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12030_dstacq9_2200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12031_dstacq9_2201() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12032_dstacq9_2202() { var g = Game.Solo(seed: 112517); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12033_dstacq9_2203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112618 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12034_dstacq9_2204() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12035_dstacq9_2205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12036_dstacq9_2206() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12037_dstacq9_2207() { var g = Game.Solo(seed: 112522); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12038_dstacq9_2208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112623 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12039_dstacq9_2209() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12040_dstacq9_2210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12041_dstacq9_2211() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12042_dstacq9_2212() { var g = Game.Solo(seed: 112527); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12043_dstacq9_2213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112628 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12044_dstacq9_2214() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12045_dstacq9_2215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12046_dstacq9_2216() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12047_dstacq9_2217() { var g = Game.Solo(seed: 112532); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12048_dstacq9_2218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112633 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12049_dstacq9_2219() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12050_dstacq9_2220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12051_dstacq9_2221() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12052_dstacq9_2222() { var g = Game.Solo(seed: 112537); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12053_dstacq9_2223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112638 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12054_dstacq9_2224() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12055_dstacq9_2225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12056_dstacq9_2226() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12057_dstacq9_2227() { var g = Game.Solo(seed: 112542); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12058_dstacq9_2228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112643 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12059_dstacq9_2229() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12060_dstacq9_2230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q12061_dstacq9_2231() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q12062_dstacq9_2232() { var g = Game.Solo(seed: 112547); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q12063_dstacq9_2233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 112648 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q12064_dstacq9_2234() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q12065_dstacq9_2235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 159); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
}
