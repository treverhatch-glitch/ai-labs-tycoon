using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17916+: loan393.</summary>
public class HatchChipLoansBarLaterTests56
{
    [Fact] public void Q17916_loan393_5000() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17917_loan393_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17918_loan393_5002() { var g = Game.Solo(seed: 396912); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17919_loan393_5003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396913 })).Sim); }
    [Fact] public void Q17920_loan393_5004() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17921_loan393_5005() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17922_loan393_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17923_loan393_5007() { var g = Game.Solo(seed: 396917); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17924_loan393_5008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396918 })).Sim); }
    [Fact] public void Q17925_loan393_5009() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17926_loan393_5010() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17927_loan393_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17928_loan393_5012() { var g = Game.Solo(seed: 396922); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17929_loan393_5013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396923 })).Sim); }
    [Fact] public void Q17930_loan393_5014() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17931_loan393_5015() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17932_loan393_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17933_loan393_5017() { var g = Game.Solo(seed: 396927); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17934_loan393_5018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396928 })).Sim); }
    [Fact] public void Q17935_loan393_5019() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17936_loan393_5020() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17937_loan393_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17938_loan393_5022() { var g = Game.Solo(seed: 396932); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17939_loan393_5023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396933 })).Sim); }
    [Fact] public void Q17940_loan393_5024() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17941_loan393_5025() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17942_loan393_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17943_loan393_5027() { var g = Game.Solo(seed: 396937); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17944_loan393_5028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396938 })).Sim); }
    [Fact] public void Q17945_loan393_5029() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17946_loan393_5030() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17947_loan393_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17948_loan393_5032() { var g = Game.Solo(seed: 396942); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17949_loan393_5033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396943 })).Sim); }
    [Fact] public void Q17950_loan393_5034() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17951_loan393_5035() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17952_loan393_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17953_loan393_5037() { var g = Game.Solo(seed: 396947); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17954_loan393_5038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396948 })).Sim); }
    [Fact] public void Q17955_loan393_5039() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17956_loan393_5040() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17957_loan393_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17958_loan393_5042() { var g = Game.Solo(seed: 396952); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17959_loan393_5043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396953 })).Sim); }
    [Fact] public void Q17960_loan393_5044() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17961_loan393_5045() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17962_loan393_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17963_loan393_5047() { var g = Game.Solo(seed: 396957); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17964_loan393_5048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396958 })).Sim); }
    [Fact] public void Q17965_loan393_5049() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17966_loan393_5050() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17967_loan393_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17968_loan393_5052() { var g = Game.Solo(seed: 396962); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17969_loan393_5053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396963 })).Sim); }
    [Fact] public void Q17970_loan393_5054() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17971_loan393_5055() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17972_loan393_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17973_loan393_5057() { var g = Game.Solo(seed: 396967); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17974_loan393_5058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396968 })).Sim); }
    [Fact] public void Q17975_loan393_5059() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17976_loan393_5060() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17977_loan393_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17978_loan393_5062() { var g = Game.Solo(seed: 396972); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17979_loan393_5063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396973 })).Sim); }
    [Fact] public void Q17980_loan393_5064() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
    [Fact] public void Q17981_loan393_5065() { GameConfig c = TestData.Config(); Assert.True(c.Loans.LoanToValuePermille > 0); }
    [Fact] public void Q17982_loan393_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17983_loan393_5067() { var g = Game.Solo(seed: 396977); Assert.Equal(0, AiLabs.Rules.Systems.Loans.Outstanding(g.Me)); }
    [Fact] public void Q17984_loan393_5068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396978 })).Sim); }
    [Fact] public void Q17985_loan393_5069() { GameConfig c = TestData.Config(); Assert.True(c.Loans.TermWeeks > 0); }
}
