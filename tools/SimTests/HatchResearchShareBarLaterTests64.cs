using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18476+: rshare401.</summary>
public class HatchResearchShareBarLaterTests64
{
    [Fact] public void Q18476_rshare401_5000() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18477_rshare401_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18478_rshare401_5002() { var g = Game.Solo(seed: 397472).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18479_rshare401_5003() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18480_rshare401_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397474 })).Sim); }
    [Fact] public void Q18481_rshare401_5005() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18482_rshare401_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18483_rshare401_5007() { var g = Game.Solo(seed: 397477).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18484_rshare401_5008() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18485_rshare401_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397479 })).Sim); }
    [Fact] public void Q18486_rshare401_5010() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18487_rshare401_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18488_rshare401_5012() { var g = Game.Solo(seed: 397482).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18489_rshare401_5013() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18490_rshare401_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397484 })).Sim); }
    [Fact] public void Q18491_rshare401_5015() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18492_rshare401_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18493_rshare401_5017() { var g = Game.Solo(seed: 397487).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18494_rshare401_5018() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18495_rshare401_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397489 })).Sim); }
    [Fact] public void Q18496_rshare401_5020() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18497_rshare401_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18498_rshare401_5022() { var g = Game.Solo(seed: 397492).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18499_rshare401_5023() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18500_rshare401_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397494 })).Sim); }
    [Fact] public void Q18501_rshare401_5025() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18502_rshare401_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18503_rshare401_5027() { var g = Game.Solo(seed: 397497).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18504_rshare401_5028() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18505_rshare401_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397499 })).Sim); }
    [Fact] public void Q18506_rshare401_5030() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18507_rshare401_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18508_rshare401_5032() { var g = Game.Solo(seed: 397502).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18509_rshare401_5033() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18510_rshare401_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397504 })).Sim); }
    [Fact] public void Q18511_rshare401_5035() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18512_rshare401_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18513_rshare401_5037() { var g = Game.Solo(seed: 397507).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18514_rshare401_5038() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18515_rshare401_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397509 })).Sim); }
    [Fact] public void Q18516_rshare401_5040() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18517_rshare401_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18518_rshare401_5042() { var g = Game.Solo(seed: 397512).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18519_rshare401_5043() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18520_rshare401_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397514 })).Sim); }
    [Fact] public void Q18521_rshare401_5045() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18522_rshare401_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18523_rshare401_5047() { var g = Game.Solo(seed: 397517).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18524_rshare401_5048() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18525_rshare401_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397519 })).Sim); }
    [Fact] public void Q18526_rshare401_5050() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18527_rshare401_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18528_rshare401_5052() { var g = Game.Solo(seed: 397522).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18529_rshare401_5053() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18530_rshare401_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397524 })).Sim); }
    [Fact] public void Q18531_rshare401_5055() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18532_rshare401_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18533_rshare401_5057() { var g = Game.Solo(seed: 397527).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18534_rshare401_5058() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18535_rshare401_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397529 })).Sim); }
    [Fact] public void Q18536_rshare401_5060() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18537_rshare401_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18538_rshare401_5062() { var g = Game.Solo(seed: 397532).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18539_rshare401_5063() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18540_rshare401_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397534 })).Sim); }
    [Fact] public void Q18541_rshare401_5065() { GameConfig c = TestData.Config(); Assert.True(c.Research.MaxComputeSharePermille > 0); }
    [Fact] public void Q18542_rshare401_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18543_rshare401_5067() { var g = Game.Solo(seed: 397537).WithStarterLab(); Assert.Equal(0, g.Me.ResearchSharePermille); }
    [Fact] public void Q18544_rshare401_5068() { GameConfig c = TestData.Config(); Assert.True(c.Research.ComputeShareBonusPermille >= 0); }
    [Fact] public void Q18545_rshare401_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397539 })).Sim); }
}
