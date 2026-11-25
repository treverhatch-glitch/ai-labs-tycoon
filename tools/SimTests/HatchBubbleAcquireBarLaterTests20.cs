using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q14766–14835: batch 312 bubble/acquire data bar (schema 29).</summary>
public class HatchBubbleAcquireBarLaterTests20
{
    [Fact] public void Q14766_bubacq20_4826() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14767_bubacq20_4827() { var g = Game.Solo(seed: 283482); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14768_bubacq20_4828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283583 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14769_bubacq20_4829() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14770_bubacq20_4830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14771_bubacq20_4831() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14772_bubacq20_4832() { var g = Game.Solo(seed: 283487); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14773_bubacq20_4833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283588 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14774_bubacq20_4834() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14775_bubacq20_4835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14776_bubacq20_4836() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14777_bubacq20_4837() { var g = Game.Solo(seed: 283492); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14778_bubacq20_4838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283593 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14779_bubacq20_4839() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14780_bubacq20_4840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14781_bubacq20_4841() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14782_bubacq20_4842() { var g = Game.Solo(seed: 283497); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14783_bubacq20_4843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283598 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14784_bubacq20_4844() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14785_bubacq20_4845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14786_bubacq20_4846() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14787_bubacq20_4847() { var g = Game.Solo(seed: 283502); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14788_bubacq20_4848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283603 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14789_bubacq20_4849() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14790_bubacq20_4850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14791_bubacq20_4851() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14792_bubacq20_4852() { var g = Game.Solo(seed: 283507); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14793_bubacq20_4853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283608 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14794_bubacq20_4854() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14795_bubacq20_4855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14796_bubacq20_4856() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14797_bubacq20_4857() { var g = Game.Solo(seed: 283512); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14798_bubacq20_4858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283613 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14799_bubacq20_4859() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14800_bubacq20_4860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14801_bubacq20_4861() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14802_bubacq20_4862() { var g = Game.Solo(seed: 283517); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14803_bubacq20_4863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283618 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14804_bubacq20_4864() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14805_bubacq20_4865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14806_bubacq20_4866() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14807_bubacq20_4867() { var g = Game.Solo(seed: 283522); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14808_bubacq20_4868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283623 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14809_bubacq20_4869() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14810_bubacq20_4870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14811_bubacq20_4871() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14812_bubacq20_4872() { var g = Game.Solo(seed: 283527); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14813_bubacq20_4873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283628 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14814_bubacq20_4874() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14815_bubacq20_4875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14816_bubacq20_4876() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14817_bubacq20_4877() { var g = Game.Solo(seed: 283532); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14818_bubacq20_4878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283633 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14819_bubacq20_4879() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14820_bubacq20_4880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14821_bubacq20_4881() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14822_bubacq20_4882() { var g = Game.Solo(seed: 283537); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14823_bubacq20_4883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283638 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14824_bubacq20_4884() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14825_bubacq20_4885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14826_bubacq20_4886() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14827_bubacq20_4887() { var g = Game.Solo(seed: 283542); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14828_bubacq20_4888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283643 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14829_bubacq20_4889() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14830_bubacq20_4890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
    [Fact] public void Q14831_bubacq20_4891() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Acquisitions.DistressPricePermille > 0); }
    [Fact] public void Q14832_bubacq20_4892() { var g = Game.Solo(seed: 283547); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q14833_bubacq20_4893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 283648 })).Sim); Assert.True(c.Acquisitions.CrashDistressPricePermille > 0); }
    [Fact] public void Q14834_bubacq20_4894() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); Assert.True(c.Acquisitions.MinInstalledModels > 0); }
    [Fact] public void Q14835_bubacq20_4895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 141154); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.FleetResaleLockWeeks >= 0); }
}
