using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6636–6705: batch 204 acquire/listing × chips bar (schema 29).</summary>
public class HatchAcquireListingBarLaterTests9
{
    [Fact] public void Q13716_hatchacq9_4616() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13717_hatchacq9_4617() { var g = Game.Solo(seed: 136242); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13718_hatchacq9_4618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136343 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13719_hatchacq9_4619() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13720_hatchacq9_4620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13721_hatchacq9_4621() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13722_hatchacq9_4622() { var g = Game.Solo(seed: 136247); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13723_hatchacq9_4623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136348 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13724_hatchacq9_4624() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13725_hatchacq9_4625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13726_hatchacq9_4626() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13727_hatchacq9_4627() { var g = Game.Solo(seed: 136252); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13728_hatchacq9_4628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136353 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13729_hatchacq9_4629() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13730_hatchacq9_4630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13731_hatchacq9_4631() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13732_hatchacq9_4632() { var g = Game.Solo(seed: 136257); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13733_hatchacq9_4633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136358 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13734_hatchacq9_4634() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13735_hatchacq9_4635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13736_hatchacq9_4636() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13737_hatchacq9_4637() { var g = Game.Solo(seed: 136262); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13738_hatchacq9_4638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136363 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13739_hatchacq9_4639() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13740_hatchacq9_4640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13741_hatchacq9_4641() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13742_hatchacq9_4642() { var g = Game.Solo(seed: 136267); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13743_hatchacq9_4643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136368 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13744_hatchacq9_4644() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13745_hatchacq9_4645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13746_hatchacq9_4646() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13747_hatchacq9_4647() { var g = Game.Solo(seed: 136272); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13748_hatchacq9_4648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136373 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13749_hatchacq9_4649() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13750_hatchacq9_4650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13751_hatchacq9_4651() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13752_hatchacq9_4652() { var g = Game.Solo(seed: 136277); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13753_hatchacq9_4653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136378 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13754_hatchacq9_4654() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13755_hatchacq9_4655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13756_hatchacq9_4656() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13757_hatchacq9_4657() { var g = Game.Solo(seed: 136282); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13758_hatchacq9_4658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136383 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13759_hatchacq9_4659() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13760_hatchacq9_4660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13761_hatchacq9_4661() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13762_hatchacq9_4662() { var g = Game.Solo(seed: 136287); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13763_hatchacq9_4663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136388 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13764_hatchacq9_4664() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13765_hatchacq9_4665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13766_hatchacq9_4666() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13767_hatchacq9_4667() { var g = Game.Solo(seed: 136292); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13768_hatchacq9_4668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136393 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13769_hatchacq9_4669() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13770_hatchacq9_4670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13771_hatchacq9_4671() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13772_hatchacq9_4672() { var g = Game.Solo(seed: 136297); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13773_hatchacq9_4673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136398 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13774_hatchacq9_4674() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13775_hatchacq9_4675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13776_hatchacq9_4676() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13777_hatchacq9_4677() { var g = Game.Solo(seed: 136302); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13778_hatchacq9_4678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136403 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13779_hatchacq9_4679() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13780_hatchacq9_4680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13781_hatchacq9_4681() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13782_hatchacq9_4682() { var g = Game.Solo(seed: 136307); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13783_hatchacq9_4683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 136408 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13784_hatchacq9_4684() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13785_hatchacq9_4685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 186); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
