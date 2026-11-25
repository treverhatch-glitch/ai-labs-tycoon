using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14686–14755: batch 257 distress/acquire data bar (schema 29).</summary>
public class HatchDistressAcquireBarLaterTests14
{
    [Fact] public void Q14686_dstacq13_2306() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14687_dstacq13_2307() { var g = Game.Solo(seed: 179242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14688_dstacq13_2308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179343 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14689_dstacq13_2309() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14690_dstacq13_2310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14691_dstacq13_2311() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14692_dstacq13_2312() { var g = Game.Solo(seed: 179247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14693_dstacq13_2313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179348 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14694_dstacq13_2314() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14695_dstacq13_2315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14696_dstacq13_2316() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14697_dstacq13_2317() { var g = Game.Solo(seed: 179252); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14698_dstacq13_2318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179353 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14699_dstacq13_2319() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14700_dstacq13_2320() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14701_dstacq13_2321() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14702_dstacq13_2322() { var g = Game.Solo(seed: 179257); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14703_dstacq13_2323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179358 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14704_dstacq13_2324() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14705_dstacq13_2325() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14706_dstacq13_2326() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14707_dstacq13_2327() { var g = Game.Solo(seed: 179262); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14708_dstacq13_2328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179363 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14709_dstacq13_2329() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14710_dstacq13_2330() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14711_dstacq13_2331() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14712_dstacq13_2332() { var g = Game.Solo(seed: 179267); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14713_dstacq13_2333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179368 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14714_dstacq13_2334() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14715_dstacq13_2335() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14716_dstacq13_2336() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14717_dstacq13_2337() { var g = Game.Solo(seed: 179272); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14718_dstacq13_2338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179373 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14719_dstacq13_2339() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14720_dstacq13_2340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14721_dstacq13_2341() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14722_dstacq13_2342() { var g = Game.Solo(seed: 179277); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14723_dstacq13_2343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179378 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14724_dstacq13_2344() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14725_dstacq13_2345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14726_dstacq13_2346() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14727_dstacq13_2347() { var g = Game.Solo(seed: 179282); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14728_dstacq13_2348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179383 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14729_dstacq13_2349() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14730_dstacq13_2350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14731_dstacq13_2351() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14732_dstacq13_2352() { var g = Game.Solo(seed: 179287); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14733_dstacq13_2353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179388 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14734_dstacq13_2354() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14735_dstacq13_2355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14736_dstacq13_2356() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14737_dstacq13_2357() { var g = Game.Solo(seed: 179292); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14738_dstacq13_2358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179393 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14739_dstacq13_2359() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14740_dstacq13_2360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14741_dstacq13_2361() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14742_dstacq13_2362() { var g = Game.Solo(seed: 179297); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14743_dstacq13_2363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179398 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14744_dstacq13_2364() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14745_dstacq13_2365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14746_dstacq13_2366() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14747_dstacq13_2367() { var g = Game.Solo(seed: 179302); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14748_dstacq13_2368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179403 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14749_dstacq13_2369() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14750_dstacq13_2370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
    [Fact] public void Q14751_dstacq13_2371() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.MinOffer > 0); }
    [Fact] public void Q14752_dstacq13_2372() { var g = Game.Solo(seed: 179307); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14753_dstacq13_2373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 179408 })).Sim); Assert.True(c.Acquisitions.RivalRollupChancePermille >= 0); }
    [Fact] public void Q14754_dstacq13_2374() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14755_dstacq13_2375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35439); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer >= 0); }
}
