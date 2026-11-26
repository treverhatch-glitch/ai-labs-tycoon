using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16656+: batch 373 rivals bible tail (reslead).</summary>
public class HatchResearchLeadsBarLaterTests34
{
    [Fact] public void Q16656_reslead373_5000() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16657_reslead373_5001() { var g = Game.Solo(seed: 395101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16658_reslead373_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395102 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16659_reslead373_5003() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16660_reslead373_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16661_reslead373_5005() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16662_reslead373_5006() { var g = Game.Solo(seed: 395106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16663_reslead373_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395107 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16664_reslead373_5008() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16665_reslead373_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16666_reslead373_5010() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16667_reslead373_5011() { var g = Game.Solo(seed: 395111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16668_reslead373_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395112 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16669_reslead373_5013() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16670_reslead373_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16671_reslead373_5015() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16672_reslead373_5016() { var g = Game.Solo(seed: 395116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16673_reslead373_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395117 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16674_reslead373_5018() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16675_reslead373_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16676_reslead373_5020() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16677_reslead373_5021() { var g = Game.Solo(seed: 395121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16678_reslead373_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395122 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16679_reslead373_5023() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16680_reslead373_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16681_reslead373_5025() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16682_reslead373_5026() { var g = Game.Solo(seed: 395126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16683_reslead373_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395127 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16684_reslead373_5028() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16685_reslead373_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16686_reslead373_5030() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16687_reslead373_5031() { var g = Game.Solo(seed: 395131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16688_reslead373_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395132 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16689_reslead373_5033() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16690_reslead373_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16691_reslead373_5035() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16692_reslead373_5036() { var g = Game.Solo(seed: 395136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16693_reslead373_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395137 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16694_reslead373_5038() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16695_reslead373_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16696_reslead373_5040() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16697_reslead373_5041() { var g = Game.Solo(seed: 395141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16698_reslead373_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395142 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16699_reslead373_5043() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16700_reslead373_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16701_reslead373_5045() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16702_reslead373_5046() { var g = Game.Solo(seed: 395146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16703_reslead373_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395147 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16704_reslead373_5048() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16705_reslead373_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16706_reslead373_5050() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16707_reslead373_5051() { var g = Game.Solo(seed: 395151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16708_reslead373_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395152 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16709_reslead373_5053() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16710_reslead373_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16711_reslead373_5055() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16712_reslead373_5056() { var g = Game.Solo(seed: 395156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16713_reslead373_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395157 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16714_reslead373_5058() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16715_reslead373_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16716_reslead373_5060() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16717_reslead373_5061() { var g = Game.Solo(seed: 395161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16718_reslead373_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395162 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16719_reslead373_5063() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16720_reslead373_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
    [Fact] public void Q16721_reslead373_5065() { GameConfig c = TestData.Config(); Assert.True(c.Leads.MaxOpen >= 1); Assert.True(c.Leads.MinLuckPermille > 0); }
    [Fact] public void Q16722_reslead373_5066() { var g = Game.Solo(seed: 395166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.Empty(ResearchLeads.Rows(g.C, g.Me, g.State.Tick)); }
    [Fact] public void Q16723_reslead373_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395167 })).Sim); Assert.True(c.Leads.ExpireWeeks > 0); }
    [Fact] public void Q16724_reslead373_5068() { GameConfig c = TestData.Config(); Assert.True(c.Leads.FollowCostWeeks > 0); Assert.True(c.Leads.MinCost >= 0); }
    [Fact] public void Q16725_reslead373_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247600); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.False(ResearchLeads.AnyFollowed(g.Me)); }
}
