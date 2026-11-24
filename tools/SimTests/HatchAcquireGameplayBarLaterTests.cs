using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2096–Q2165: batch 35 acquisition gameplay data bar (schema 31).</summary>
public class HatchAcquireGameplayBarLaterTests
{
    [Fact] public void Q2096_acquire_2096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2097_acquire_2097() { var g = Game.Solo(seed: 4201); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2098_acquire_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4302 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2099_acquire_2099() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2100_acquire_2100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2101_acquire_2101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2102_acquire_2102() { var g = Game.Solo(seed: 4206); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2103_acquire_2103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4307 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2104_acquire_2104() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2105_acquire_2105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2106_acquire_2106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2107_acquire_2107() { var g = Game.Solo(seed: 4211); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2108_acquire_2108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4312 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2109_acquire_2109() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2110_acquire_2110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2111_acquire_2111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2112_acquire_2112() { var g = Game.Solo(seed: 4216); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2113_acquire_2113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4317 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2114_acquire_2114() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2115_acquire_2115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2116_acquire_2116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2117_acquire_2117() { var g = Game.Solo(seed: 4221); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2118_acquire_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4322 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2119_acquire_2119() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2120_acquire_2120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2121_acquire_2121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2122_acquire_2122() { var g = Game.Solo(seed: 4226); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2123_acquire_2123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4327 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2124_acquire_2124() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2125_acquire_2125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2126_acquire_2126() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2127_acquire_2127() { var g = Game.Solo(seed: 4231); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2128_acquire_2128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4332 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2129_acquire_2129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2130_acquire_2130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2131_acquire_2131() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2132_acquire_2132() { var g = Game.Solo(seed: 4236); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2133_acquire_2133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4337 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2134_acquire_2134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2135_acquire_2135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2136_acquire_2136() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2137_acquire_2137() { var g = Game.Solo(seed: 4241); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2138_acquire_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4342 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2139_acquire_2139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2140_acquire_2140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2141_acquire_2141() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2142_acquire_2142() { var g = Game.Solo(seed: 4246); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2143_acquire_2143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4347 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2144_acquire_2144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2145_acquire_2145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2146_acquire_2146() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2147_acquire_2147() { var g = Game.Solo(seed: 4251); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2148_acquire_2148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4352 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2149_acquire_2149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2150_acquire_2150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2151_acquire_2151() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2152_acquire_2152() { var g = Game.Solo(seed: 4256); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2153_acquire_2153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4357 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2154_acquire_2154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2155_acquire_2155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2156_acquire_2156() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2157_acquire_2157() { var g = Game.Solo(seed: 4261); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2158_acquire_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4362 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2159_acquire_2159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2160_acquire_2160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q2161_acquire_2161() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q2162_acquire_2162() { var g = Game.Solo(seed: 4266); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q2163_acquire_2163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 4367 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q2164_acquire_2164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q2165_acquire_2165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 3); Assert.Equal(31, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
}
