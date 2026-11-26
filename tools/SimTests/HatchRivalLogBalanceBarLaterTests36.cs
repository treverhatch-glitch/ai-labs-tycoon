using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16796+: batch 375 rivals bible tail (rivbal).</summary>
public class HatchRivalLogBalanceBarLaterTests36
{
    [Fact] public void Q16796_rivbal375_5000() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16797_rivbal375_5001() { var g = Game.Solo(seed: 395101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16798_rivbal375_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395102 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16799_rivbal375_5003() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16800_rivbal375_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16801_rivbal375_5005() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16802_rivbal375_5006() { var g = Game.Solo(seed: 395106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16803_rivbal375_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395107 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16804_rivbal375_5008() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16805_rivbal375_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16806_rivbal375_5010() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16807_rivbal375_5011() { var g = Game.Solo(seed: 395111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16808_rivbal375_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395112 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16809_rivbal375_5013() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16810_rivbal375_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16811_rivbal375_5015() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16812_rivbal375_5016() { var g = Game.Solo(seed: 395116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16813_rivbal375_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395117 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16814_rivbal375_5018() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16815_rivbal375_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16816_rivbal375_5020() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16817_rivbal375_5021() { var g = Game.Solo(seed: 395121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16818_rivbal375_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395122 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16819_rivbal375_5023() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16820_rivbal375_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16821_rivbal375_5025() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16822_rivbal375_5026() { var g = Game.Solo(seed: 395126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16823_rivbal375_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395127 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16824_rivbal375_5028() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16825_rivbal375_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16826_rivbal375_5030() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16827_rivbal375_5031() { var g = Game.Solo(seed: 395131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16828_rivbal375_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395132 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16829_rivbal375_5033() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16830_rivbal375_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16831_rivbal375_5035() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16832_rivbal375_5036() { var g = Game.Solo(seed: 395136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16833_rivbal375_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395137 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16834_rivbal375_5038() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16835_rivbal375_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16836_rivbal375_5040() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16837_rivbal375_5041() { var g = Game.Solo(seed: 395141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16838_rivbal375_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395142 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16839_rivbal375_5043() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16840_rivbal375_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16841_rivbal375_5045() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16842_rivbal375_5046() { var g = Game.Solo(seed: 395146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16843_rivbal375_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395147 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16844_rivbal375_5048() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16845_rivbal375_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16846_rivbal375_5050() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16847_rivbal375_5051() { var g = Game.Solo(seed: 395151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16848_rivbal375_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395152 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16849_rivbal375_5053() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16850_rivbal375_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16851_rivbal375_5055() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16852_rivbal375_5056() { var g = Game.Solo(seed: 395156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16853_rivbal375_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395157 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16854_rivbal375_5058() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16855_rivbal375_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16856_rivbal375_5060() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16857_rivbal375_5061() { var g = Game.Solo(seed: 395161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16858_rivbal375_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395162 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16859_rivbal375_5063() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16860_rivbal375_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
    [Fact] public void Q16861_rivbal375_5065() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.LogMax > 0); Assert.True(BalanceSoak.DefaultPresets(c).Length >= 3); }
    [Fact] public void Q16862_rivbal375_5066() { var g = Game.Solo(seed: 395166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(BalanceSoak.DefaultPresets(g.C).Length >= 1); }
    [Fact] public void Q16863_rivbal375_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395167 })).Sim); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); }
    [Fact] public void Q16864_rivbal375_5068() { GameConfig c = TestData.Config(); Assert.All(BalanceSoak.DefaultPresets(c), p => Assert.False(string.IsNullOrEmpty(p.Id))); }
    [Fact] public void Q16865_rivbal375_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivalry.LogMax > 0); }
}
