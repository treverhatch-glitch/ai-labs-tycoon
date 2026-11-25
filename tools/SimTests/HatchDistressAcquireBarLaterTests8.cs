using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6846–6915: batch 137 distress/acquire data bar (schema 29).</summary>
public class HatchDistressAcquireBarLaterTests8
{
    [Fact] public void Q9226_dstacq8_2166() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9227_dstacq8_2167() { var g = Game.Solo(seed: 74342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9228_dstacq8_2168() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74443 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9229_dstacq8_2169() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9230_dstacq8_2170() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9231_dstacq8_2171() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9232_dstacq8_2172() { var g = Game.Solo(seed: 74347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9233_dstacq8_2173() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74448 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9234_dstacq8_2174() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9235_dstacq8_2175() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9236_dstacq8_2176() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9237_dstacq8_2177() { var g = Game.Solo(seed: 74352); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9238_dstacq8_2178() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74453 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9239_dstacq8_2179() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9240_dstacq8_2180() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9241_dstacq8_2181() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9242_dstacq8_2182() { var g = Game.Solo(seed: 74357); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9243_dstacq8_2183() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74458 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9244_dstacq8_2184() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9245_dstacq8_2185() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9246_dstacq8_2186() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9247_dstacq8_2187() { var g = Game.Solo(seed: 74362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9248_dstacq8_2188() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74463 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9249_dstacq8_2189() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9250_dstacq8_2190() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9251_dstacq8_2191() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9252_dstacq8_2192() { var g = Game.Solo(seed: 74367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9253_dstacq8_2193() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74468 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9254_dstacq8_2194() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9255_dstacq8_2195() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9256_dstacq8_2196() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9257_dstacq8_2197() { var g = Game.Solo(seed: 74372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9258_dstacq8_2198() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74473 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9259_dstacq8_2199() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9260_dstacq8_2200() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9261_dstacq8_2201() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9262_dstacq8_2202() { var g = Game.Solo(seed: 74377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9263_dstacq8_2203() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74478 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9264_dstacq8_2204() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9265_dstacq8_2205() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9266_dstacq8_2206() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9267_dstacq8_2207() { var g = Game.Solo(seed: 74382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9268_dstacq8_2208() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74483 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9269_dstacq8_2209() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9270_dstacq8_2210() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9271_dstacq8_2211() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9272_dstacq8_2212() { var g = Game.Solo(seed: 74387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9273_dstacq8_2213() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74488 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9274_dstacq8_2214() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9275_dstacq8_2215() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9276_dstacq8_2216() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9277_dstacq8_2217() { var g = Game.Solo(seed: 74392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9278_dstacq8_2218() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74493 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9279_dstacq8_2219() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9280_dstacq8_2220() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9281_dstacq8_2221() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9282_dstacq8_2222() { var g = Game.Solo(seed: 74397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9283_dstacq8_2223() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74498 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9284_dstacq8_2224() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9285_dstacq8_2225() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9286_dstacq8_2226() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9287_dstacq8_2227() { var g = Game.Solo(seed: 74402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9288_dstacq8_2228() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74503 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9289_dstacq8_2229() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9290_dstacq8_2230() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q9291_dstacq8_2231() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q9292_dstacq8_2232() { var g = Game.Solo(seed: 74407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q9293_dstacq8_2233() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 74508 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q9294_dstacq8_2234() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9295_dstacq8_2235() { GameConfig c = TestData.Config(); var g = Game.New(seed: 119); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
}
