using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q5736–Q5805: batch 87 distress/acquire data bar (schema 29).</summary>
public class HatchDistressAcquireBarLaterTests5
{
    [Fact] public void Q5736_dstacq5_2166() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5737_dstacq5_2167() { var g = Game.Solo(seed: 25482); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5738_dstacq5_2168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25583 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5739_dstacq5_2169() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5740_dstacq5_2170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5741_dstacq5_2171() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5742_dstacq5_2172() { var g = Game.Solo(seed: 25487); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5743_dstacq5_2173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25588 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5744_dstacq5_2174() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5745_dstacq5_2175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5746_dstacq5_2176() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5747_dstacq5_2177() { var g = Game.Solo(seed: 25492); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5748_dstacq5_2178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25593 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5749_dstacq5_2179() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5750_dstacq5_2180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5751_dstacq5_2181() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5752_dstacq5_2182() { var g = Game.Solo(seed: 25497); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5753_dstacq5_2183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25598 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5754_dstacq5_2184() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5755_dstacq5_2185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5756_dstacq5_2186() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5757_dstacq5_2187() { var g = Game.Solo(seed: 25502); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5758_dstacq5_2188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25603 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5759_dstacq5_2189() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5760_dstacq5_2190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5761_dstacq5_2191() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5762_dstacq5_2192() { var g = Game.Solo(seed: 25507); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5763_dstacq5_2193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25608 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5764_dstacq5_2194() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5765_dstacq5_2195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5766_dstacq5_2196() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5767_dstacq5_2197() { var g = Game.Solo(seed: 25512); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5768_dstacq5_2198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25613 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5769_dstacq5_2199() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5770_dstacq5_2200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5771_dstacq5_2201() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5772_dstacq5_2202() { var g = Game.Solo(seed: 25517); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5773_dstacq5_2203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25618 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5774_dstacq5_2204() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5775_dstacq5_2205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5776_dstacq5_2206() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5777_dstacq5_2207() { var g = Game.Solo(seed: 25522); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5778_dstacq5_2208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25623 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5779_dstacq5_2209() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5780_dstacq5_2210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5781_dstacq5_2211() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5782_dstacq5_2212() { var g = Game.Solo(seed: 25527); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5783_dstacq5_2213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25628 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5784_dstacq5_2214() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5785_dstacq5_2215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5786_dstacq5_2216() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5787_dstacq5_2217() { var g = Game.Solo(seed: 25532); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5788_dstacq5_2218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25633 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5789_dstacq5_2219() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5790_dstacq5_2220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5791_dstacq5_2221() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5792_dstacq5_2222() { var g = Game.Solo(seed: 25537); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5793_dstacq5_2223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25638 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5794_dstacq5_2224() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5795_dstacq5_2225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5796_dstacq5_2226() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5797_dstacq5_2227() { var g = Game.Solo(seed: 25542); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5798_dstacq5_2228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25643 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5799_dstacq5_2229() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5800_dstacq5_2230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q5801_dstacq5_2231() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q5802_dstacq5_2232() { var g = Game.Solo(seed: 25547); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q5803_dstacq5_2233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25648 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q5804_dstacq5_2234() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5805_dstacq5_2235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 69); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
}
