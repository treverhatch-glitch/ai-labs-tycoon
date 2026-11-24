using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6636–6705: batch 164 acquire/listing × chips bar (schema 29).</summary>
public class HatchAcquireListingBarLaterTests7
{
    [Fact] public void Q11116_hatchacq7_4616() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11117_hatchacq7_4617() { var g = Game.Solo(seed: 100842); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11118_hatchacq7_4618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100943 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11119_hatchacq7_4619() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11120_hatchacq7_4620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11121_hatchacq7_4621() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11122_hatchacq7_4622() { var g = Game.Solo(seed: 100847); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11123_hatchacq7_4623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100948 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11124_hatchacq7_4624() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11125_hatchacq7_4625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11126_hatchacq7_4626() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11127_hatchacq7_4627() { var g = Game.Solo(seed: 100852); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11128_hatchacq7_4628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100953 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11129_hatchacq7_4629() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11130_hatchacq7_4630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11131_hatchacq7_4631() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11132_hatchacq7_4632() { var g = Game.Solo(seed: 100857); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11133_hatchacq7_4633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100958 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11134_hatchacq7_4634() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11135_hatchacq7_4635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11136_hatchacq7_4636() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11137_hatchacq7_4637() { var g = Game.Solo(seed: 100862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11138_hatchacq7_4638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100963 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11139_hatchacq7_4639() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11140_hatchacq7_4640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11141_hatchacq7_4641() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11142_hatchacq7_4642() { var g = Game.Solo(seed: 100867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11143_hatchacq7_4643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100968 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11144_hatchacq7_4644() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11145_hatchacq7_4645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11146_hatchacq7_4646() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11147_hatchacq7_4647() { var g = Game.Solo(seed: 100872); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11148_hatchacq7_4648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100973 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11149_hatchacq7_4649() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11150_hatchacq7_4650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11151_hatchacq7_4651() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11152_hatchacq7_4652() { var g = Game.Solo(seed: 100877); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11153_hatchacq7_4653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100978 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11154_hatchacq7_4654() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11155_hatchacq7_4655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11156_hatchacq7_4656() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11157_hatchacq7_4657() { var g = Game.Solo(seed: 100882); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11158_hatchacq7_4658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100983 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11159_hatchacq7_4659() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11160_hatchacq7_4660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11161_hatchacq7_4661() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11162_hatchacq7_4662() { var g = Game.Solo(seed: 100887); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11163_hatchacq7_4663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100988 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11164_hatchacq7_4664() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11165_hatchacq7_4665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11166_hatchacq7_4666() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11167_hatchacq7_4667() { var g = Game.Solo(seed: 100892); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11168_hatchacq7_4668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100993 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11169_hatchacq7_4669() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11170_hatchacq7_4670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11171_hatchacq7_4671() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11172_hatchacq7_4672() { var g = Game.Solo(seed: 100897); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11173_hatchacq7_4673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 100998 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11174_hatchacq7_4674() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11175_hatchacq7_4675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11176_hatchacq7_4676() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11177_hatchacq7_4677() { var g = Game.Solo(seed: 100902); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11178_hatchacq7_4678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101003 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11179_hatchacq7_4679() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11180_hatchacq7_4680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q11181_hatchacq7_4681() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q11182_hatchacq7_4682() { var g = Game.Solo(seed: 100907); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q11183_hatchacq7_4683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 101008 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q11184_hatchacq7_4684() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q11185_hatchacq7_4685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 146); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
