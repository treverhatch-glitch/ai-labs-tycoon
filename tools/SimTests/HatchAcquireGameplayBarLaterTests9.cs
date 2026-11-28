using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11926–11995: batch 216 acquisition gameplay data bar (schema 29).</summary>
public class HatchAcquireGameplayBarLaterTests9
{
    [Fact] public void Q14476_acquire_2096() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14477_acquire_2097() { var g = Game.Solo(seed: 143162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14478_acquire_2098() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143263 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14479_acquire_2099() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14480_acquire_2100() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14481_acquire_2101() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14482_acquire_2102() { var g = Game.Solo(seed: 143167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14483_acquire_2103() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143268 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14484_acquire_2104() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14485_acquire_2105() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14486_acquire_2106() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14487_acquire_2107() { var g = Game.Solo(seed: 143172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14488_acquire_2108() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143273 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14489_acquire_2109() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14490_acquire_2110() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14491_acquire_2111() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14492_acquire_2112() { var g = Game.Solo(seed: 143177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14493_acquire_2113() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143278 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14494_acquire_2114() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14495_acquire_2115() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14496_acquire_2116() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14497_acquire_2117() { var g = Game.Solo(seed: 143182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14498_acquire_2118() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143283 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14499_acquire_2119() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14500_acquire_2120() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14501_acquire_2121() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14502_acquire_2122() { var g = Game.Solo(seed: 143187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14503_acquire_2123() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143288 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14504_acquire_2124() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14505_acquire_2125() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14506_acquire_2126() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14507_acquire_2127() { var g = Game.Solo(seed: 143192); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14508_acquire_2128() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143293 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14509_acquire_2129() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14510_acquire_2130() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14511_acquire_2131() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14512_acquire_2132() { var g = Game.Solo(seed: 143197); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14513_acquire_2133() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143298 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14514_acquire_2134() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14515_acquire_2135() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14516_acquire_2136() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14517_acquire_2137() { var g = Game.Solo(seed: 143202); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14518_acquire_2138() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143303 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14519_acquire_2139() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14520_acquire_2140() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14521_acquire_2141() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14522_acquire_2142() { var g = Game.Solo(seed: 143207); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14523_acquire_2143() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143308 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14524_acquire_2144() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14525_acquire_2145() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14526_acquire_2146() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14527_acquire_2147() { var g = Game.Solo(seed: 143212); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14528_acquire_2148() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143313 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14529_acquire_2149() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14530_acquire_2150() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14531_acquire_2151() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14532_acquire_2152() { var g = Game.Solo(seed: 143217); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14533_acquire_2153() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143318 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14534_acquire_2154() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14535_acquire_2155() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14536_acquire_2156() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14537_acquire_2157() { var g = Game.Solo(seed: 143222); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14538_acquire_2158() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143323 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14539_acquire_2159() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14540_acquire_2160() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
    [Fact] public void Q14541_acquire_2161() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q14542_acquire_2162() { var g = Game.Solo(seed: 143227); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.PlannerAcquireChancePermille >= 0); }
    [Fact] public void Q14543_acquire_2163() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143328 })).Sim); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14544_acquire_2164() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14545_acquire_2165() { GameConfig c = TestData.Config(); var g = Game.New(seed: 198); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.CapabilityValuePerPoint > 0); }
}
