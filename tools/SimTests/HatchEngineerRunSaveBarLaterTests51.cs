using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17566+: engrun390 hatch bar.</summary>
public class HatchEngineerRunSaveBarLaterTests51
{
    [Fact] public void Q17566_engrun390_5000() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17567_engrun390_5001() { var g = Game.Solo(seed: 396551); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17568_engrun390_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396552 })).Sim); }
    [Fact] public void Q17569_engrun390_5003() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17570_engrun390_5004() { var g = Game.Solo(seed: 396554).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17571_engrun390_5005() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17572_engrun390_5006() { var g = Game.Solo(seed: 396556); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17573_engrun390_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396557 })).Sim); }
    [Fact] public void Q17574_engrun390_5008() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17575_engrun390_5009() { var g = Game.Solo(seed: 396559).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17576_engrun390_5010() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17577_engrun390_5011() { var g = Game.Solo(seed: 396561); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17578_engrun390_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396562 })).Sim); }
    [Fact] public void Q17579_engrun390_5013() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17580_engrun390_5014() { var g = Game.Solo(seed: 396564).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17581_engrun390_5015() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17582_engrun390_5016() { var g = Game.Solo(seed: 396566); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17583_engrun390_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396567 })).Sim); }
    [Fact] public void Q17584_engrun390_5018() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17585_engrun390_5019() { var g = Game.Solo(seed: 396569).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17586_engrun390_5020() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17587_engrun390_5021() { var g = Game.Solo(seed: 396571); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17588_engrun390_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396572 })).Sim); }
    [Fact] public void Q17589_engrun390_5023() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17590_engrun390_5024() { var g = Game.Solo(seed: 396574).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17591_engrun390_5025() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17592_engrun390_5026() { var g = Game.Solo(seed: 396576); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17593_engrun390_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396577 })).Sim); }
    [Fact] public void Q17594_engrun390_5028() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17595_engrun390_5029() { var g = Game.Solo(seed: 396579).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17596_engrun390_5030() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17597_engrun390_5031() { var g = Game.Solo(seed: 396581); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17598_engrun390_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396582 })).Sim); }
    [Fact] public void Q17599_engrun390_5033() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17600_engrun390_5034() { var g = Game.Solo(seed: 396584).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17601_engrun390_5035() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17602_engrun390_5036() { var g = Game.Solo(seed: 396586); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17603_engrun390_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396587 })).Sim); }
    [Fact] public void Q17604_engrun390_5038() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17605_engrun390_5039() { var g = Game.Solo(seed: 396589).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17606_engrun390_5040() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17607_engrun390_5041() { var g = Game.Solo(seed: 396591); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17608_engrun390_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396592 })).Sim); }
    [Fact] public void Q17609_engrun390_5043() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17610_engrun390_5044() { var g = Game.Solo(seed: 396594).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17611_engrun390_5045() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17612_engrun390_5046() { var g = Game.Solo(seed: 396596); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17613_engrun390_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396597 })).Sim); }
    [Fact] public void Q17614_engrun390_5048() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17615_engrun390_5049() { var g = Game.Solo(seed: 396599).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17616_engrun390_5050() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17617_engrun390_5051() { var g = Game.Solo(seed: 396601); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17618_engrun390_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396602 })).Sim); }
    [Fact] public void Q17619_engrun390_5053() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17620_engrun390_5054() { var g = Game.Solo(seed: 396604).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17621_engrun390_5055() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17622_engrun390_5056() { var g = Game.Solo(seed: 396606); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17623_engrun390_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396607 })).Sim); }
    [Fact] public void Q17624_engrun390_5058() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17625_engrun390_5059() { var g = Game.Solo(seed: 396609).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17626_engrun390_5060() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17627_engrun390_5061() { var g = Game.Solo(seed: 396611); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17628_engrun390_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396612 })).Sim); }
    [Fact] public void Q17629_engrun390_5063() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17630_engrun390_5064() { var g = Game.Solo(seed: 396614).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
    [Fact] public void Q17631_engrun390_5065() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17632_engrun390_5066() { var g = Game.Solo(seed: 396616); Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) >= 0); }
    [Fact] public void Q17633_engrun390_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396617 })).Sim); }
    [Fact] public void Q17634_engrun390_5068() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingPermillePerStaff >= 0); }
    [Fact] public void Q17635_engrun390_5069() { var g = Game.Solo(seed: 396619).WithStarterLab(engineers: 2); Assert.True(g.Me.Staff.Count >= 2); }
}
