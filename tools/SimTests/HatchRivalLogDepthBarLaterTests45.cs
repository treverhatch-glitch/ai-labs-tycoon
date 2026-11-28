using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q17146+: batch 384 rival decision log depth (rivlog384).</summary>
public class HatchRivalLogDepthBarLaterTests45
{
    [Fact] public void Q17146_rivlog384_5000() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17147_rivlog384_5001() { var g = Game.Solo(seed: 396101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17148_rivlog384_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396102 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17149_rivlog384_5003() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17150_rivlog384_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17151_rivlog384_5005() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17152_rivlog384_5006() { var g = Game.Solo(seed: 396106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17153_rivlog384_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396107 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17154_rivlog384_5008() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17155_rivlog384_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17156_rivlog384_5010() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17157_rivlog384_5011() { var g = Game.Solo(seed: 396111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17158_rivlog384_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396112 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17159_rivlog384_5013() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17160_rivlog384_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17161_rivlog384_5015() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17162_rivlog384_5016() { var g = Game.Solo(seed: 396116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17163_rivlog384_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396117 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17164_rivlog384_5018() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17165_rivlog384_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17166_rivlog384_5020() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17167_rivlog384_5021() { var g = Game.Solo(seed: 396121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17168_rivlog384_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396122 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17169_rivlog384_5023() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17170_rivlog384_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17171_rivlog384_5025() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17172_rivlog384_5026() { var g = Game.Solo(seed: 396126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17173_rivlog384_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396127 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17174_rivlog384_5028() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17175_rivlog384_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17176_rivlog384_5030() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17177_rivlog384_5031() { var g = Game.Solo(seed: 396131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17178_rivlog384_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396132 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17179_rivlog384_5033() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17180_rivlog384_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17181_rivlog384_5035() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17182_rivlog384_5036() { var g = Game.Solo(seed: 396136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17183_rivlog384_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396137 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17184_rivlog384_5038() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17185_rivlog384_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17186_rivlog384_5040() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17187_rivlog384_5041() { var g = Game.Solo(seed: 396141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17188_rivlog384_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396142 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17189_rivlog384_5043() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17190_rivlog384_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17191_rivlog384_5045() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17192_rivlog384_5046() { var g = Game.Solo(seed: 396146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17193_rivlog384_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396147 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17194_rivlog384_5048() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17195_rivlog384_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17196_rivlog384_5050() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17197_rivlog384_5051() { var g = Game.Solo(seed: 396151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17198_rivlog384_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396152 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17199_rivlog384_5053() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17200_rivlog384_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17201_rivlog384_5055() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17202_rivlog384_5056() { var g = Game.Solo(seed: 396156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17203_rivlog384_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396157 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17204_rivlog384_5058() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17205_rivlog384_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17206_rivlog384_5060() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17207_rivlog384_5061() { var g = Game.Solo(seed: 396161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17208_rivlog384_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396162 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17209_rivlog384_5063() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17210_rivlog384_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
    [Fact] public void Q17211_rivlog384_5065() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q17212_rivlog384_5066() { var g = Game.Solo(seed: 396166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q17213_rivlog384_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396167 })).Sim); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q17214_rivlog384_5068() { GameConfig c = TestData.Config(); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q17215_rivlog384_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 248400); Assert.Equal(LabController.Plan, g.State.Labs.Find(l => l.RivalId == "openaye").Controller); }
}
