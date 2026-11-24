using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3636–Q3705: batch 57 acquisition/listing data bar coda (schema 29).</summary>
public class HatchAcquireListingBarLaterTests2
{
    [Fact] public void Q3636_acqlist2_3636() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3637_acqlist2_3637() { var g = Game.Solo(seed: 6382); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3638_acqlist2_3638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6484 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3639_acqlist2_3639() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3640_acqlist2_3640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3641_acqlist2_3641() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3642_acqlist2_3642() { var g = Game.Solo(seed: 6387); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3643_acqlist2_3643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6489 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3644_acqlist2_3644() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3645_acqlist2_3645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3646_acqlist2_3646() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3647_acqlist2_3647() { var g = Game.Solo(seed: 6392); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3648_acqlist2_3648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6494 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3649_acqlist2_3649() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3650_acqlist2_3650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3651_acqlist2_3651() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3652_acqlist2_3652() { var g = Game.Solo(seed: 6397); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3653_acqlist2_3653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6499 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3654_acqlist2_3654() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3655_acqlist2_3655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3656_acqlist2_3656() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3657_acqlist2_3657() { var g = Game.Solo(seed: 6402); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3658_acqlist2_3658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6504 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3659_acqlist2_3659() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3660_acqlist2_3660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3661_acqlist2_3661() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3662_acqlist2_3662() { var g = Game.Solo(seed: 6407); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3663_acqlist2_3663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6509 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3664_acqlist2_3664() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3665_acqlist2_3665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3666_acqlist2_3666() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3667_acqlist2_3667() { var g = Game.Solo(seed: 6412); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3668_acqlist2_3668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6514 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3669_acqlist2_3669() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3670_acqlist2_3670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3671_acqlist2_3671() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3672_acqlist2_3672() { var g = Game.Solo(seed: 6417); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3673_acqlist2_3673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6519 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3674_acqlist2_3674() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3675_acqlist2_3675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3676_acqlist2_3676() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3677_acqlist2_3677() { var g = Game.Solo(seed: 6422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3678_acqlist2_3678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6524 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3679_acqlist2_3679() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3680_acqlist2_3680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3681_acqlist2_3681() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3682_acqlist2_3682() { var g = Game.Solo(seed: 6427); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3683_acqlist2_3683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6529 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3684_acqlist2_3684() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3685_acqlist2_3685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3686_acqlist2_3686() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3687_acqlist2_3687() { var g = Game.Solo(seed: 6432); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3688_acqlist2_3688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6534 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3689_acqlist2_3689() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3690_acqlist2_3690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3691_acqlist2_3691() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3692_acqlist2_3692() { var g = Game.Solo(seed: 6437); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3693_acqlist2_3693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6539 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3694_acqlist2_3694() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3695_acqlist2_3695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3696_acqlist2_3696() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3697_acqlist2_3697() { var g = Game.Solo(seed: 6442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3698_acqlist2_3698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6544 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3699_acqlist2_3699() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3700_acqlist2_3700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q3701_acqlist2_3701() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); Assert.True(c.Acquisitions.ReadyChipBookPermille > 0); }
    [Fact] public void Q3702_acqlist2_3702() { var g = Game.Solo(seed: 6447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.InHouseTrainingBonusPermille > 0); }
    [Fact] public void Q3703_acqlist2_3703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6549 })).Sim); Assert.True(c.Acquisitions.FleetResaleLockWeeks > 0); }
    [Fact] public void Q3704_acqlist2_3704() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.MinOffer > 0); Assert.True(c.Chips.PointsPerArchitectWeek > 0); }
    [Fact] public void Q3705_acqlist2_3705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 41); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.CrashDistressPricePermille > 0); }
}
