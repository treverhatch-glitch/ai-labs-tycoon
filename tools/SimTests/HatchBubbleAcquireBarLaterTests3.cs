using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q4266–Q4335: batch 66 bubble/acquire data bar coda (schema 29).</summary>
public class HatchBubbleAcquireBarLaterTests3
{
    [Fact] public void Q4266_bubacq3_4266() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4267_bubacq3_4267() { var g = Game.Solo(seed: 8182); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4268_bubacq3_4268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8284 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4269_bubacq3_4269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4270_bubacq3_4270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4271_bubacq3_4271() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4272_bubacq3_4272() { var g = Game.Solo(seed: 8187); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4273_bubacq3_4273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8289 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4274_bubacq3_4274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4275_bubacq3_4275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4276_bubacq3_4276() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4277_bubacq3_4277() { var g = Game.Solo(seed: 8192); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4278_bubacq3_4278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8294 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4279_bubacq3_4279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4280_bubacq3_4280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4281_bubacq3_4281() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4282_bubacq3_4282() { var g = Game.Solo(seed: 8197); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4283_bubacq3_4283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8299 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4284_bubacq3_4284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4285_bubacq3_4285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4286_bubacq3_4286() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4287_bubacq3_4287() { var g = Game.Solo(seed: 8202); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4288_bubacq3_4288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8304 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4289_bubacq3_4289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4290_bubacq3_4290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4291_bubacq3_4291() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4292_bubacq3_4292() { var g = Game.Solo(seed: 8207); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4293_bubacq3_4293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8309 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4294_bubacq3_4294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4295_bubacq3_4295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4296_bubacq3_4296() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4297_bubacq3_4297() { var g = Game.Solo(seed: 8212); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4298_bubacq3_4298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8314 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4299_bubacq3_4299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4300_bubacq3_4300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4301_bubacq3_4301() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4302_bubacq3_4302() { var g = Game.Solo(seed: 8217); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4303_bubacq3_4303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8319 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4304_bubacq3_4304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4305_bubacq3_4305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4306_bubacq3_4306() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4307_bubacq3_4307() { var g = Game.Solo(seed: 8222); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4308_bubacq3_4308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8324 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4309_bubacq3_4309() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4310_bubacq3_4310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4311_bubacq3_4311() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4312_bubacq3_4312() { var g = Game.Solo(seed: 8227); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4313_bubacq3_4313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8329 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4314_bubacq3_4314() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4315_bubacq3_4315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4316_bubacq3_4316() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4317_bubacq3_4317() { var g = Game.Solo(seed: 8232); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4318_bubacq3_4318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8334 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4319_bubacq3_4319() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4320_bubacq3_4320() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4321_bubacq3_4321() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4322_bubacq3_4322() { var g = Game.Solo(seed: 8237); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4323_bubacq3_4323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8339 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4324_bubacq3_4324() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4325_bubacq3_4325() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4326_bubacq3_4326() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4327_bubacq3_4327() { var g = Game.Solo(seed: 8242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4328_bubacq3_4328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8344 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4329_bubacq3_4329() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4330_bubacq3_4330() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q4331_bubacq3_4331() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q4332_bubacq3_4332() { var g = Game.Solo(seed: 8247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q4333_bubacq3_4333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 8349 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q4334_bubacq3_4334() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q4335_bubacq3_4335() { GameConfig c = TestData.Config(); var g = Game.New(seed: 50); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
