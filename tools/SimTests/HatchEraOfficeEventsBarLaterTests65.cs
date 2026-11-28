using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18546+: eraoff402.</summary>
public class HatchEraOfficeEventsBarLaterTests65
{
    [Fact] public void Q18546_eraoff402_5000() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18547_eraoff402_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18548_eraoff402_5002() { var g = Game.Solo(seed: 397542); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18549_eraoff402_5003() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18550_eraoff402_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397544 })).Sim); }
    [Fact] public void Q18551_eraoff402_5005() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18552_eraoff402_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18553_eraoff402_5007() { var g = Game.Solo(seed: 397547); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18554_eraoff402_5008() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18555_eraoff402_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397549 })).Sim); }
    [Fact] public void Q18556_eraoff402_5010() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18557_eraoff402_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18558_eraoff402_5012() { var g = Game.Solo(seed: 397552); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18559_eraoff402_5013() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18560_eraoff402_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397554 })).Sim); }
    [Fact] public void Q18561_eraoff402_5015() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18562_eraoff402_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18563_eraoff402_5017() { var g = Game.Solo(seed: 397557); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18564_eraoff402_5018() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18565_eraoff402_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397559 })).Sim); }
    [Fact] public void Q18566_eraoff402_5020() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18567_eraoff402_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18568_eraoff402_5022() { var g = Game.Solo(seed: 397562); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18569_eraoff402_5023() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18570_eraoff402_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397564 })).Sim); }
    [Fact] public void Q18571_eraoff402_5025() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18572_eraoff402_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18573_eraoff402_5027() { var g = Game.Solo(seed: 397567); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18574_eraoff402_5028() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18575_eraoff402_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397569 })).Sim); }
    [Fact] public void Q18576_eraoff402_5030() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18577_eraoff402_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18578_eraoff402_5032() { var g = Game.Solo(seed: 397572); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18579_eraoff402_5033() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18580_eraoff402_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397574 })).Sim); }
    [Fact] public void Q18581_eraoff402_5035() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18582_eraoff402_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18583_eraoff402_5037() { var g = Game.Solo(seed: 397577); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18584_eraoff402_5038() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18585_eraoff402_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397579 })).Sim); }
    [Fact] public void Q18586_eraoff402_5040() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18587_eraoff402_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18588_eraoff402_5042() { var g = Game.Solo(seed: 397582); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18589_eraoff402_5043() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18590_eraoff402_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397584 })).Sim); }
    [Fact] public void Q18591_eraoff402_5045() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18592_eraoff402_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18593_eraoff402_5047() { var g = Game.Solo(seed: 397587); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18594_eraoff402_5048() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18595_eraoff402_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397589 })).Sim); }
    [Fact] public void Q18596_eraoff402_5050() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18597_eraoff402_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18598_eraoff402_5052() { var g = Game.Solo(seed: 397592); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18599_eraoff402_5053() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18600_eraoff402_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397594 })).Sim); }
    [Fact] public void Q18601_eraoff402_5055() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18602_eraoff402_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18603_eraoff402_5057() { var g = Game.Solo(seed: 397597); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18604_eraoff402_5058() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18605_eraoff402_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397599 })).Sim); }
    [Fact] public void Q18606_eraoff402_5060() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18607_eraoff402_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18608_eraoff402_5062() { var g = Game.Solo(seed: 397602); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18609_eraoff402_5063() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18610_eraoff402_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397604 })).Sim); }
    [Fact] public void Q18611_eraoff402_5065() { GameConfig c = TestData.Config(); Assert.Contains(c.Office.Events, e => e.FromYear > 0 || e.ToYear > 0); }
    [Fact] public void Q18612_eraoff402_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18613_eraoff402_5067() { var g = Game.Solo(seed: 397607); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18614_eraoff402_5068() { GameConfig c = TestData.Config(); Assert.True(OfficeEvents.InYears(c, c.Office.Events[0], 52)); }
    [Fact] public void Q18615_eraoff402_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397609 })).Sim); }
}
