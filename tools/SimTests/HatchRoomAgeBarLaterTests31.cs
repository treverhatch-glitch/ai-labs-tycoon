using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16446+: batch 370 rivals bible tail (roomage).</summary>
public class HatchRoomAgeBarLaterTests31
{
    [Fact] public void Q16446_roomage370_5000() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16447_roomage370_5001() { var g = Game.Solo(seed: 394101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16448_roomage370_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394102 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16449_roomage370_5003() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16450_roomage370_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16451_roomage370_5005() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16452_roomage370_5006() { var g = Game.Solo(seed: 394106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16453_roomage370_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394107 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16454_roomage370_5008() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16455_roomage370_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16456_roomage370_5010() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16457_roomage370_5011() { var g = Game.Solo(seed: 394111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16458_roomage370_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394112 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16459_roomage370_5013() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16460_roomage370_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16461_roomage370_5015() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16462_roomage370_5016() { var g = Game.Solo(seed: 394116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16463_roomage370_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394117 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16464_roomage370_5018() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16465_roomage370_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16466_roomage370_5020() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16467_roomage370_5021() { var g = Game.Solo(seed: 394121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16468_roomage370_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394122 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16469_roomage370_5023() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16470_roomage370_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16471_roomage370_5025() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16472_roomage370_5026() { var g = Game.Solo(seed: 394126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16473_roomage370_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394127 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16474_roomage370_5028() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16475_roomage370_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16476_roomage370_5030() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16477_roomage370_5031() { var g = Game.Solo(seed: 394131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16478_roomage370_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394132 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16479_roomage370_5033() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16480_roomage370_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16481_roomage370_5035() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16482_roomage370_5036() { var g = Game.Solo(seed: 394136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16483_roomage370_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394137 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16484_roomage370_5038() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16485_roomage370_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16486_roomage370_5040() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16487_roomage370_5041() { var g = Game.Solo(seed: 394141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16488_roomage370_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394142 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16489_roomage370_5043() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16490_roomage370_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16491_roomage370_5045() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16492_roomage370_5046() { var g = Game.Solo(seed: 394146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16493_roomage370_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394147 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16494_roomage370_5048() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16495_roomage370_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16496_roomage370_5050() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16497_roomage370_5051() { var g = Game.Solo(seed: 394151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16498_roomage370_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394152 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16499_roomage370_5053() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16500_roomage370_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16501_roomage370_5055() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16502_roomage370_5056() { var g = Game.Solo(seed: 394156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16503_roomage370_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394157 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16504_roomage370_5058() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16505_roomage370_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16506_roomage370_5060() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16507_roomage370_5061() { var g = Game.Solo(seed: 394161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16508_roomage370_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394162 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16509_roomage370_5063() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16510_roomage370_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16511_roomage370_5065() { GameConfig c = TestData.Config(); Assert.True(c.Ageing.Stages.Count >= 1); Assert.True(c.Ageing.RefurbishCostPermille > 0); }
    [Fact] public void Q16512_roomage370_5066() { var g = Game.Solo(seed: 394166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.MaxMoralePenalty >= 0); }
    [Fact] public void Q16513_roomage370_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394167 })).Sim); Assert.True(c.Ageing.Stages.Count >= 1); }
    [Fact] public void Q16514_roomage370_5068() { GameConfig c = TestData.Config(); Assert.All(c.Ageing.Stages, s => Assert.False(string.IsNullOrEmpty(s.IdValue))); }
    [Fact] public void Q16515_roomage370_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247300); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Ageing.RefurbishCostPermille > 0); }
}
