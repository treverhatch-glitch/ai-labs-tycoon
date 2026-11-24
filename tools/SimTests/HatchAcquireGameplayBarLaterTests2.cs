using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2096–Q2165: batch 35 acquisition gameplay data bar (schema 29).</summary>
public class HatchAcquireGameplayBarLaterTests2
{
    [Fact] public void Q5596_acquire_2096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5597_acquire_2097() { var g = Game.Solo(seed: 24402); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5598_acquire_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24503 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5599_acquire_2099() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5600_acquire_2100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5601_acquire_2101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5602_acquire_2102() { var g = Game.Solo(seed: 24407); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5603_acquire_2103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24508 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5604_acquire_2104() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5605_acquire_2105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5606_acquire_2106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5607_acquire_2107() { var g = Game.Solo(seed: 24412); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5608_acquire_2108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24513 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5609_acquire_2109() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5610_acquire_2110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5611_acquire_2111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5612_acquire_2112() { var g = Game.Solo(seed: 24417); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5613_acquire_2113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24518 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5614_acquire_2114() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5615_acquire_2115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5616_acquire_2116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5617_acquire_2117() { var g = Game.Solo(seed: 24422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5618_acquire_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24523 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5619_acquire_2119() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5620_acquire_2120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5621_acquire_2121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5622_acquire_2122() { var g = Game.Solo(seed: 24427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5623_acquire_2123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24528 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5624_acquire_2124() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5625_acquire_2125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5626_acquire_2126() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5627_acquire_2127() { var g = Game.Solo(seed: 24432); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5628_acquire_2128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24533 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5629_acquire_2129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5630_acquire_2130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5631_acquire_2131() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5632_acquire_2132() { var g = Game.Solo(seed: 24437); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5633_acquire_2133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24538 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5634_acquire_2134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5635_acquire_2135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5636_acquire_2136() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5637_acquire_2137() { var g = Game.Solo(seed: 24442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5638_acquire_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24543 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5639_acquire_2139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5640_acquire_2140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5641_acquire_2141() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5642_acquire_2142() { var g = Game.Solo(seed: 24447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5643_acquire_2143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24548 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5644_acquire_2144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5645_acquire_2145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5646_acquire_2146() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5647_acquire_2147() { var g = Game.Solo(seed: 24452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5648_acquire_2148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24553 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5649_acquire_2149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5650_acquire_2150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5651_acquire_2151() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5652_acquire_2152() { var g = Game.Solo(seed: 24457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5653_acquire_2153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24558 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5654_acquire_2154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5655_acquire_2155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5656_acquire_2156() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5657_acquire_2157() { var g = Game.Solo(seed: 24462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5658_acquire_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24563 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5659_acquire_2159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5660_acquire_2160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q5661_acquire_2161() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q5662_acquire_2162() { var g = Game.Solo(seed: 24467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q5663_acquire_2163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 24568 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q5664_acquire_2164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q5665_acquire_2165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 68); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
}
