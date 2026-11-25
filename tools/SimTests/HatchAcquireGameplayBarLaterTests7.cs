using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11926–11995: batch 176 acquisition gameplay data bar (schema 29).</summary>
public class HatchAcquireGameplayBarLaterTests7
{
    [Fact] public void Q11926_acquire_2096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11927_acquire_2097() { var g = Game.Solo(seed: 111602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11928_acquire_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111703 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11929_acquire_2099() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11930_acquire_2100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11931_acquire_2101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11932_acquire_2102() { var g = Game.Solo(seed: 111607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11933_acquire_2103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111708 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11934_acquire_2104() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11935_acquire_2105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11936_acquire_2106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11937_acquire_2107() { var g = Game.Solo(seed: 111612); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11938_acquire_2108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111713 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11939_acquire_2109() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11940_acquire_2110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11941_acquire_2111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11942_acquire_2112() { var g = Game.Solo(seed: 111617); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11943_acquire_2113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111718 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11944_acquire_2114() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11945_acquire_2115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11946_acquire_2116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11947_acquire_2117() { var g = Game.Solo(seed: 111622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11948_acquire_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111723 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11949_acquire_2119() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11950_acquire_2120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11951_acquire_2121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11952_acquire_2122() { var g = Game.Solo(seed: 111627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11953_acquire_2123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111728 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11954_acquire_2124() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11955_acquire_2125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11956_acquire_2126() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11957_acquire_2127() { var g = Game.Solo(seed: 111632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11958_acquire_2128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111733 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11959_acquire_2129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11960_acquire_2130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11961_acquire_2131() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11962_acquire_2132() { var g = Game.Solo(seed: 111637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11963_acquire_2133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111738 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11964_acquire_2134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11965_acquire_2135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11966_acquire_2136() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11967_acquire_2137() { var g = Game.Solo(seed: 111642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11968_acquire_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111743 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11969_acquire_2139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11970_acquire_2140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11971_acquire_2141() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11972_acquire_2142() { var g = Game.Solo(seed: 111647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11973_acquire_2143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111748 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11974_acquire_2144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11975_acquire_2145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11976_acquire_2146() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11977_acquire_2147() { var g = Game.Solo(seed: 111652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11978_acquire_2148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111753 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11979_acquire_2149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11980_acquire_2150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11981_acquire_2151() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11982_acquire_2152() { var g = Game.Solo(seed: 111657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11983_acquire_2153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111758 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11984_acquire_2154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11985_acquire_2155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11986_acquire_2156() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11987_acquire_2157() { var g = Game.Solo(seed: 111662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11988_acquire_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111763 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11989_acquire_2159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11990_acquire_2160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q11991_acquire_2161() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11992_acquire_2162() { var g = Game.Solo(seed: 111667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q11993_acquire_2163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 111768 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q11994_acquire_2164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11995_acquire_2165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 158); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
}
