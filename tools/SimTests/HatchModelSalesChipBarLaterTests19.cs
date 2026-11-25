using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14626–14695: batch 310 model-sales/chip data bar (schema 29).</summary>
public class HatchModelSalesChipBarLaterTests19
{
    [Fact] public void Q14626_mschip19_3986() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14627_mschip19_3987() { var g = Game.Solo(seed: 281722); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14628_mschip19_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281823 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14629_mschip19_3989() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14630_mschip19_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14631_mschip19_3991() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14632_mschip19_3992() { var g = Game.Solo(seed: 281727); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14633_mschip19_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281828 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14634_mschip19_3994() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14635_mschip19_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14636_mschip19_3996() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14637_mschip19_3997() { var g = Game.Solo(seed: 281732); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14638_mschip19_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281833 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14639_mschip19_3999() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14640_mschip19_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14641_mschip19_4001() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14642_mschip19_4002() { var g = Game.Solo(seed: 281737); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14643_mschip19_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281838 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14644_mschip19_4004() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14645_mschip19_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14646_mschip19_4006() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14647_mschip19_4007() { var g = Game.Solo(seed: 281742); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14648_mschip19_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281843 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14649_mschip19_4009() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14650_mschip19_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14651_mschip19_4011() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14652_mschip19_4012() { var g = Game.Solo(seed: 281747); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14653_mschip19_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281848 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14654_mschip19_4014() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14655_mschip19_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14656_mschip19_4016() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14657_mschip19_4017() { var g = Game.Solo(seed: 281752); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14658_mschip19_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281853 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14659_mschip19_4019() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14660_mschip19_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14661_mschip19_4021() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14662_mschip19_4022() { var g = Game.Solo(seed: 281757); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14663_mschip19_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281858 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14664_mschip19_4024() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14665_mschip19_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14666_mschip19_4026() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14667_mschip19_4027() { var g = Game.Solo(seed: 281762); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14668_mschip19_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281863 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14669_mschip19_4029() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14670_mschip19_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14671_mschip19_4031() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14672_mschip19_4032() { var g = Game.Solo(seed: 281767); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14673_mschip19_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281868 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14674_mschip19_4034() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14675_mschip19_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14676_mschip19_4036() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14677_mschip19_4037() { var g = Game.Solo(seed: 281772); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14678_mschip19_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281873 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14679_mschip19_4039() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14680_mschip19_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14681_mschip19_4041() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14682_mschip19_4042() { var g = Game.Solo(seed: 281777); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14683_mschip19_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281878 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14684_mschip19_4044() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14685_mschip19_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14686_mschip19_4046() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14687_mschip19_4047() { var g = Game.Solo(seed: 281782); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14688_mschip19_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281883 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14689_mschip19_4049() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14690_mschip19_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
    [Fact] public void Q14691_mschip19_4051() { GameConfig c = TestData.Config(); Assert.True(c.ModelSales.CrashDistressOfferPermille > 0); Assert.True(c.ModelSales.EarningsMultipleWeeks > 0); }
    [Fact] public void Q14692_mschip19_4052() { var g = Game.Solo(seed: 281787); Assert.Equal(29, g.State.Schema); Assert.True(g.C.ModelSales.MinOffer > 0); }
    [Fact] public void Q14693_mschip19_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 281888 })).Sim); Assert.True(c.ModelSales.IncomeSmoothingPermille > 0); }
    [Fact] public void Q14694_mschip19_4054() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.ModelSales.UnprofitableSalePermille > 0); }
    [Fact] public void Q14695_mschip19_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141152); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Chips.SoftwareCap > 0); }
}
