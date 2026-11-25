using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14556–14625: batch 288 bubble/acquire data bar (schema 29).</summary>
public class HatchBubbleAcquireBarLaterTests17
{
    [Fact] public void Q14556_bubacq17_4616() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14557_bubacq17_4617() { var g = Game.Solo(seed: 230682); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14558_bubacq17_4618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230783 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14559_bubacq17_4619() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14560_bubacq17_4620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14561_bubacq17_4621() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14562_bubacq17_4622() { var g = Game.Solo(seed: 230687); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14563_bubacq17_4623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230788 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14564_bubacq17_4624() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14565_bubacq17_4625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14566_bubacq17_4626() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14567_bubacq17_4627() { var g = Game.Solo(seed: 230692); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14568_bubacq17_4628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230793 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14569_bubacq17_4629() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14570_bubacq17_4630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14571_bubacq17_4631() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14572_bubacq17_4632() { var g = Game.Solo(seed: 230697); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14573_bubacq17_4633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230798 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14574_bubacq17_4634() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14575_bubacq17_4635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14576_bubacq17_4636() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14577_bubacq17_4637() { var g = Game.Solo(seed: 230702); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14578_bubacq17_4638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230803 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14579_bubacq17_4639() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14580_bubacq17_4640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14581_bubacq17_4641() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14582_bubacq17_4642() { var g = Game.Solo(seed: 230707); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14583_bubacq17_4643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230808 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14584_bubacq17_4644() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14585_bubacq17_4645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14586_bubacq17_4646() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14587_bubacq17_4647() { var g = Game.Solo(seed: 230712); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14588_bubacq17_4648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230813 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14589_bubacq17_4649() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14590_bubacq17_4650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14591_bubacq17_4651() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14592_bubacq17_4652() { var g = Game.Solo(seed: 230717); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14593_bubacq17_4653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230818 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14594_bubacq17_4654() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14595_bubacq17_4655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14596_bubacq17_4656() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14597_bubacq17_4657() { var g = Game.Solo(seed: 230722); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14598_bubacq17_4658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230823 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14599_bubacq17_4659() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14600_bubacq17_4660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14601_bubacq17_4661() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14602_bubacq17_4662() { var g = Game.Solo(seed: 230727); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14603_bubacq17_4663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230828 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14604_bubacq17_4664() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14605_bubacq17_4665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14606_bubacq17_4666() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14607_bubacq17_4667() { var g = Game.Solo(seed: 230732); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14608_bubacq17_4668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230833 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14609_bubacq17_4669() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14610_bubacq17_4670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14611_bubacq17_4671() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14612_bubacq17_4672() { var g = Game.Solo(seed: 230737); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14613_bubacq17_4673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230838 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14614_bubacq17_4674() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14615_bubacq17_4675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14616_bubacq17_4676() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14617_bubacq17_4677() { var g = Game.Solo(seed: 230742); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14618_bubacq17_4678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230843 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14619_bubacq17_4679() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14620_bubacq17_4680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14621_bubacq17_4681() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14622_bubacq17_4682() { var g = Game.Solo(seed: 230747); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14623_bubacq17_4683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 230848 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14624_bubacq17_4684() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14625_bubacq17_4685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 88294); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
