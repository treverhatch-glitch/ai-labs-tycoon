using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16586+: batch 372 rivals bible tail (ctrlstale).</summary>
public class HatchControlStaleBarLaterTests33
{
    [Fact] public void Q16586_ctrlstale372_5000() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16587_ctrlstale372_5001() { var g = Game.Solo(seed: 395101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16588_ctrlstale372_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395102 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16589_ctrlstale372_5003() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16590_ctrlstale372_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16591_ctrlstale372_5005() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16592_ctrlstale372_5006() { var g = Game.Solo(seed: 395106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16593_ctrlstale372_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395107 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16594_ctrlstale372_5008() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16595_ctrlstale372_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16596_ctrlstale372_5010() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16597_ctrlstale372_5011() { var g = Game.Solo(seed: 395111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16598_ctrlstale372_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395112 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16599_ctrlstale372_5013() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16600_ctrlstale372_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16601_ctrlstale372_5015() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16602_ctrlstale372_5016() { var g = Game.Solo(seed: 395116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16603_ctrlstale372_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395117 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16604_ctrlstale372_5018() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16605_ctrlstale372_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16606_ctrlstale372_5020() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16607_ctrlstale372_5021() { var g = Game.Solo(seed: 395121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16608_ctrlstale372_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395122 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16609_ctrlstale372_5023() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16610_ctrlstale372_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16611_ctrlstale372_5025() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16612_ctrlstale372_5026() { var g = Game.Solo(seed: 395126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16613_ctrlstale372_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395127 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16614_ctrlstale372_5028() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16615_ctrlstale372_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16616_ctrlstale372_5030() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16617_ctrlstale372_5031() { var g = Game.Solo(seed: 395131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16618_ctrlstale372_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395132 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16619_ctrlstale372_5033() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16620_ctrlstale372_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16621_ctrlstale372_5035() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16622_ctrlstale372_5036() { var g = Game.Solo(seed: 395136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16623_ctrlstale372_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395137 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16624_ctrlstale372_5038() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16625_ctrlstale372_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16626_ctrlstale372_5040() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16627_ctrlstale372_5041() { var g = Game.Solo(seed: 395141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16628_ctrlstale372_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395142 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16629_ctrlstale372_5043() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16630_ctrlstale372_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16631_ctrlstale372_5045() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16632_ctrlstale372_5046() { var g = Game.Solo(seed: 395146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16633_ctrlstale372_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395147 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16634_ctrlstale372_5048() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16635_ctrlstale372_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16636_ctrlstale372_5050() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16637_ctrlstale372_5051() { var g = Game.Solo(seed: 395151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16638_ctrlstale372_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395152 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16639_ctrlstale372_5053() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16640_ctrlstale372_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16641_ctrlstale372_5055() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16642_ctrlstale372_5056() { var g = Game.Solo(seed: 395156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16643_ctrlstale372_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395157 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16644_ctrlstale372_5058() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16645_ctrlstale372_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16646_ctrlstale372_5060() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16647_ctrlstale372_5061() { var g = Game.Solo(seed: 395161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16648_ctrlstale372_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395162 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16649_ctrlstale372_5063() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16650_ctrlstale372_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
    [Fact] public void Q16651_ctrlstale372_5065() { GameConfig c = TestData.Config(); Assert.True(c.Control.GraceEras >= 0); Assert.True(c.Control.MaxLossPermille > 0); }
    [Fact] public void Q16652_ctrlstale372_5066() { var g = Game.Solo(seed: 395166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.Of(g.C, g.Me, g.State.Tick).BonusPermille, 0, 2000); }
    [Fact] public void Q16653_ctrlstale372_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395167 })).Sim); Assert.True(c.Control.LossPerEraPermille > 0); }
    [Fact] public void Q16654_ctrlstale372_5068() { GameConfig c = TestData.Config(); Assert.True(c.Culture.ListeningRoles.Count >= 1); Assert.True(c.Control.FiredListenerPoints != 0); }
    [Fact] public void Q16655_ctrlstale372_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247500); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.InRange(ControlStale.AlignmentBonusPermille(g.C, g.Me, g.State.Tick), 0, 2000); }
}
