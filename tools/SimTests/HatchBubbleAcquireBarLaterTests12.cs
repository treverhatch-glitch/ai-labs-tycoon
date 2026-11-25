using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q11666–11735: batch 212 bubble/acquire data bar coda (schema 29).</summary>
public class HatchBubbleAcquireBarLaterTests12
{
    [Fact] public void Q14206_bubacq12_4266() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14207_bubacq12_4267() { var g = Game.Solo(seed: 142682); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14208_bubacq12_4268() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142783 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14209_bubacq12_4269() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14210_bubacq12_4270() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14211_bubacq12_4271() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14212_bubacq12_4272() { var g = Game.Solo(seed: 142687); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14213_bubacq12_4273() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142788 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14214_bubacq12_4274() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14215_bubacq12_4275() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14216_bubacq12_4276() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14217_bubacq12_4277() { var g = Game.Solo(seed: 142692); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14218_bubacq12_4278() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142793 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14219_bubacq12_4279() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14220_bubacq12_4280() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14221_bubacq12_4281() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14222_bubacq12_4282() { var g = Game.Solo(seed: 142697); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14223_bubacq12_4283() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142798 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14224_bubacq12_4284() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14225_bubacq12_4285() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14226_bubacq12_4286() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14227_bubacq12_4287() { var g = Game.Solo(seed: 142702); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14228_bubacq12_4288() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142803 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14229_bubacq12_4289() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14230_bubacq12_4290() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14231_bubacq12_4291() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14232_bubacq12_4292() { var g = Game.Solo(seed: 142707); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14233_bubacq12_4293() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142808 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14234_bubacq12_4294() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14235_bubacq12_4295() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14236_bubacq12_4296() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14237_bubacq12_4297() { var g = Game.Solo(seed: 142712); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14238_bubacq12_4298() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142813 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14239_bubacq12_4299() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14240_bubacq12_4300() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14241_bubacq12_4301() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14242_bubacq12_4302() { var g = Game.Solo(seed: 142717); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14243_bubacq12_4303() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142818 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14244_bubacq12_4304() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14245_bubacq12_4305() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14246_bubacq12_4306() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14247_bubacq12_4307() { var g = Game.Solo(seed: 142722); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14248_bubacq12_4308() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142823 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14249_bubacq12_4309() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14250_bubacq12_4310() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14251_bubacq12_4311() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14252_bubacq12_4312() { var g = Game.Solo(seed: 142727); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14253_bubacq12_4313() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142828 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14254_bubacq12_4314() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14255_bubacq12_4315() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14256_bubacq12_4316() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14257_bubacq12_4317() { var g = Game.Solo(seed: 142732); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14258_bubacq12_4318() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142833 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14259_bubacq12_4319() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14260_bubacq12_4320() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14261_bubacq12_4321() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14262_bubacq12_4322() { var g = Game.Solo(seed: 142737); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14263_bubacq12_4323() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142838 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14264_bubacq12_4324() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14265_bubacq12_4325() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14266_bubacq12_4326() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14267_bubacq12_4327() { var g = Game.Solo(seed: 142742); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14268_bubacq12_4328() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142843 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14269_bubacq12_4329() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14270_bubacq12_4330() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14271_bubacq12_4331() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14272_bubacq12_4332() { var g = Game.Solo(seed: 142747); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14273_bubacq12_4333() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 142848 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14274_bubacq12_4334() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14275_bubacq12_4335() { GameConfig c = TestData.Config(); var g = Game.New(seed: 194); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
