using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6776–6845: batch 136 acquisition gameplay data bar (schema 29).</summary>
public class HatchAcquireGameplayBarLaterTests5
{
    [Fact] public void Q9156_acquire_2096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9157_acquire_2097() { var g = Game.Solo(seed: 73362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9158_acquire_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73463 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9159_acquire_2099() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9160_acquire_2100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9161_acquire_2101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9162_acquire_2102() { var g = Game.Solo(seed: 73367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9163_acquire_2103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73468 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9164_acquire_2104() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9165_acquire_2105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9166_acquire_2106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9167_acquire_2107() { var g = Game.Solo(seed: 73372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9168_acquire_2108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73473 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9169_acquire_2109() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9170_acquire_2110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9171_acquire_2111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9172_acquire_2112() { var g = Game.Solo(seed: 73377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9173_acquire_2113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73478 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9174_acquire_2114() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9175_acquire_2115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9176_acquire_2116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9177_acquire_2117() { var g = Game.Solo(seed: 73382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9178_acquire_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73483 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9179_acquire_2119() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9180_acquire_2120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9181_acquire_2121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9182_acquire_2122() { var g = Game.Solo(seed: 73387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9183_acquire_2123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73488 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9184_acquire_2124() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9185_acquire_2125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9186_acquire_2126() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9187_acquire_2127() { var g = Game.Solo(seed: 73392); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9188_acquire_2128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73493 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9189_acquire_2129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9190_acquire_2130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9191_acquire_2131() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9192_acquire_2132() { var g = Game.Solo(seed: 73397); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9193_acquire_2133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73498 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9194_acquire_2134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9195_acquire_2135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9196_acquire_2136() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9197_acquire_2137() { var g = Game.Solo(seed: 73402); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9198_acquire_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73503 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9199_acquire_2139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9200_acquire_2140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9201_acquire_2141() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9202_acquire_2142() { var g = Game.Solo(seed: 73407); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9203_acquire_2143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73508 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9204_acquire_2144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9205_acquire_2145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9206_acquire_2146() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9207_acquire_2147() { var g = Game.Solo(seed: 73412); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9208_acquire_2148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73513 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9209_acquire_2149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9210_acquire_2150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9211_acquire_2151() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9212_acquire_2152() { var g = Game.Solo(seed: 73417); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9213_acquire_2153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73518 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9214_acquire_2154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9215_acquire_2155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9216_acquire_2156() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9217_acquire_2157() { var g = Game.Solo(seed: 73422); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9218_acquire_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73523 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9219_acquire_2159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9220_acquire_2160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q9221_acquire_2161() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q9222_acquire_2162() { var g = Game.Solo(seed: 73427); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q9223_acquire_2163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 73528 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q9224_acquire_2164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q9225_acquire_2165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 118); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
}
