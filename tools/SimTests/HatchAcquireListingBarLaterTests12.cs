using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13926–13995: batch 262 acquire/listing data bar coda (schema 29).</summary>
public class HatchAcquireListingBarLaterTests12
{
    [Fact] public void Q13926_hatchacq12_4826() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13927_hatchacq12_4827() { var g = Game.Solo(seed: 189042); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13928_hatchacq12_4828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189143 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13929_hatchacq12_4829() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13930_hatchacq12_4830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13931_hatchacq12_4831() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13932_hatchacq12_4832() { var g = Game.Solo(seed: 189047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13933_hatchacq12_4833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189148 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13934_hatchacq12_4834() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13935_hatchacq12_4835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13936_hatchacq12_4836() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13937_hatchacq12_4837() { var g = Game.Solo(seed: 189052); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13938_hatchacq12_4838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189153 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13939_hatchacq12_4839() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13940_hatchacq12_4840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13941_hatchacq12_4841() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13942_hatchacq12_4842() { var g = Game.Solo(seed: 189057); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13943_hatchacq12_4843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189158 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13944_hatchacq12_4844() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13945_hatchacq12_4845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13946_hatchacq12_4846() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13947_hatchacq12_4847() { var g = Game.Solo(seed: 189062); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13948_hatchacq12_4848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189163 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13949_hatchacq12_4849() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13950_hatchacq12_4850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13951_hatchacq12_4851() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13952_hatchacq12_4852() { var g = Game.Solo(seed: 189067); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13953_hatchacq12_4853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189168 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13954_hatchacq12_4854() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13955_hatchacq12_4855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13956_hatchacq12_4856() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13957_hatchacq12_4857() { var g = Game.Solo(seed: 189072); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13958_hatchacq12_4858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189173 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13959_hatchacq12_4859() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13960_hatchacq12_4860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13961_hatchacq12_4861() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13962_hatchacq12_4862() { var g = Game.Solo(seed: 189077); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13963_hatchacq12_4863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189178 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13964_hatchacq12_4864() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13965_hatchacq12_4865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13966_hatchacq12_4866() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13967_hatchacq12_4867() { var g = Game.Solo(seed: 189082); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13968_hatchacq12_4868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189183 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13969_hatchacq12_4869() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13970_hatchacq12_4870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13971_hatchacq12_4871() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13972_hatchacq12_4872() { var g = Game.Solo(seed: 189087); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13973_hatchacq12_4873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189188 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13974_hatchacq12_4874() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13975_hatchacq12_4875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13976_hatchacq12_4876() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13977_hatchacq12_4877() { var g = Game.Solo(seed: 189092); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13978_hatchacq12_4878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189193 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13979_hatchacq12_4879() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13980_hatchacq12_4880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13981_hatchacq12_4881() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13982_hatchacq12_4882() { var g = Game.Solo(seed: 189097); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13983_hatchacq12_4883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189198 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13984_hatchacq12_4884() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13985_hatchacq12_4885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13986_hatchacq12_4886() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13987_hatchacq12_4887() { var g = Game.Solo(seed: 189102); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13988_hatchacq12_4888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189203 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13989_hatchacq12_4889() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13990_hatchacq12_4890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q13991_hatchacq12_4891() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.DistressPricePermille > 0); Assert.True(c.Chips.DistressListingPricePermille > 0); }
    [Fact] public void Q13992_hatchacq12_4892() { var g = Game.Solo(seed: 189107); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.MinInstalledModels >= 0); }
    [Fact] public void Q13993_hatchacq12_4893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 189208 })).Sim); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); }
    [Fact] public void Q13994_hatchacq12_4894() { GameConfig c = TestData.Config(); Assert.True(c.Chips.PointsPerArchitectWeek > 0); Assert.True(c.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q13995_hatchacq12_4895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53046); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
