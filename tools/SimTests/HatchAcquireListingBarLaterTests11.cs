using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13856–13925: batch 242 acquire/listing data bar coda (schema 29).</summary>
public class HatchAcquireListingBarLaterTests11
{
    [Fact] public void Q13856_hatchacq11_4756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13857_hatchacq11_4757() { var g = Game.Solo(seed: 171442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13858_hatchacq11_4758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171543 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13859_hatchacq11_4759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13860_hatchacq11_4760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13861_hatchacq11_4761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13862_hatchacq11_4762() { var g = Game.Solo(seed: 171447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13863_hatchacq11_4763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171548 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13864_hatchacq11_4764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13865_hatchacq11_4765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13866_hatchacq11_4766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13867_hatchacq11_4767() { var g = Game.Solo(seed: 171452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13868_hatchacq11_4768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171553 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13869_hatchacq11_4769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13870_hatchacq11_4770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13871_hatchacq11_4771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13872_hatchacq11_4772() { var g = Game.Solo(seed: 171457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13873_hatchacq11_4773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171558 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13874_hatchacq11_4774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13875_hatchacq11_4775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13876_hatchacq11_4776() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13877_hatchacq11_4777() { var g = Game.Solo(seed: 171462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13878_hatchacq11_4778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171563 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13879_hatchacq11_4779() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13880_hatchacq11_4780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13881_hatchacq11_4781() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13882_hatchacq11_4782() { var g = Game.Solo(seed: 171467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13883_hatchacq11_4783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171568 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13884_hatchacq11_4784() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13885_hatchacq11_4785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13886_hatchacq11_4786() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13887_hatchacq11_4787() { var g = Game.Solo(seed: 171472); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13888_hatchacq11_4788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171573 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13889_hatchacq11_4789() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13890_hatchacq11_4790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13891_hatchacq11_4791() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13892_hatchacq11_4792() { var g = Game.Solo(seed: 171477); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13893_hatchacq11_4793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171578 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13894_hatchacq11_4794() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13895_hatchacq11_4795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13896_hatchacq11_4796() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13897_hatchacq11_4797() { var g = Game.Solo(seed: 171482); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13898_hatchacq11_4798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171583 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13899_hatchacq11_4799() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13900_hatchacq11_4800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13901_hatchacq11_4801() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13902_hatchacq11_4802() { var g = Game.Solo(seed: 171487); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13903_hatchacq11_4803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171588 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13904_hatchacq11_4804() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13905_hatchacq11_4805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13906_hatchacq11_4806() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13907_hatchacq11_4807() { var g = Game.Solo(seed: 171492); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13908_hatchacq11_4808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171593 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13909_hatchacq11_4809() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13910_hatchacq11_4810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13911_hatchacq11_4811() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13912_hatchacq11_4812() { var g = Game.Solo(seed: 171497); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13913_hatchacq11_4813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171598 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13914_hatchacq11_4814() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13915_hatchacq11_4815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13916_hatchacq11_4816() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13917_hatchacq11_4817() { var g = Game.Solo(seed: 171502); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13918_hatchacq11_4818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171603 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13919_hatchacq11_4819() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13920_hatchacq11_4820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13921_hatchacq11_4821() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13922_hatchacq11_4822() { var g = Game.Solo(seed: 171507); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13923_hatchacq11_4823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 171608 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13924_hatchacq11_4824() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13925_hatchacq11_4825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35426); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
