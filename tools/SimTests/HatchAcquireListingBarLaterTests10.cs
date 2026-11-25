using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13786–13855: batch 222 acquire/listing data bar coda (schema 29).</summary>
public class HatchAcquireListingBarLaterTests10
{
    [Fact] public void Q13786_hatchacq10_4686() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13787_hatchacq10_4687() { var g = Game.Solo(seed: 153842); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13788_hatchacq10_4688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153943 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13789_hatchacq10_4689() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13790_hatchacq10_4690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13791_hatchacq10_4691() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13792_hatchacq10_4692() { var g = Game.Solo(seed: 153847); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13793_hatchacq10_4693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153948 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13794_hatchacq10_4694() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13795_hatchacq10_4695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13796_hatchacq10_4696() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13797_hatchacq10_4697() { var g = Game.Solo(seed: 153852); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13798_hatchacq10_4698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153953 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13799_hatchacq10_4699() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13800_hatchacq10_4700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13801_hatchacq10_4701() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13802_hatchacq10_4702() { var g = Game.Solo(seed: 153857); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13803_hatchacq10_4703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153958 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13804_hatchacq10_4704() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13805_hatchacq10_4705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13806_hatchacq10_4706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13807_hatchacq10_4707() { var g = Game.Solo(seed: 153862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13808_hatchacq10_4708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153963 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13809_hatchacq10_4709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13810_hatchacq10_4710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13811_hatchacq10_4711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13812_hatchacq10_4712() { var g = Game.Solo(seed: 153867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13813_hatchacq10_4713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153968 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13814_hatchacq10_4714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13815_hatchacq10_4715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13816_hatchacq10_4716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13817_hatchacq10_4717() { var g = Game.Solo(seed: 153872); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13818_hatchacq10_4718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153973 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13819_hatchacq10_4719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13820_hatchacq10_4720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13821_hatchacq10_4721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13822_hatchacq10_4722() { var g = Game.Solo(seed: 153877); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13823_hatchacq10_4723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153978 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13824_hatchacq10_4724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13825_hatchacq10_4725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13826_hatchacq10_4726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13827_hatchacq10_4727() { var g = Game.Solo(seed: 153882); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13828_hatchacq10_4728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153983 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13829_hatchacq10_4729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13830_hatchacq10_4730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13831_hatchacq10_4731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13832_hatchacq10_4732() { var g = Game.Solo(seed: 153887); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13833_hatchacq10_4733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153988 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13834_hatchacq10_4734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13835_hatchacq10_4735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13836_hatchacq10_4736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13837_hatchacq10_4737() { var g = Game.Solo(seed: 153892); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13838_hatchacq10_4738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153993 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13839_hatchacq10_4739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13840_hatchacq10_4740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13841_hatchacq10_4741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13842_hatchacq10_4742() { var g = Game.Solo(seed: 153897); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13843_hatchacq10_4743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 153998 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13844_hatchacq10_4744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13845_hatchacq10_4745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13846_hatchacq10_4746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13847_hatchacq10_4747() { var g = Game.Solo(seed: 153902); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13848_hatchacq10_4748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 154003 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13849_hatchacq10_4749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13850_hatchacq10_4750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13851_hatchacq10_4751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13852_hatchacq10_4752() { var g = Game.Solo(seed: 153907); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13853_hatchacq10_4753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 154008 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13854_hatchacq10_4754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13855_hatchacq10_4755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17806); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
