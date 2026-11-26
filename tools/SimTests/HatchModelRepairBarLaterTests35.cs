using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16726+: batch 374 rivals bible tail (mdlrepair).</summary>
public class HatchModelRepairBarLaterTests35
{
    [Fact] public void Q16726_mdlrepair374_5000() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16727_mdlrepair374_5001() { var g = Game.Solo(seed: 395201); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16728_mdlrepair374_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395202 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16729_mdlrepair374_5003() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16730_mdlrepair374_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16731_mdlrepair374_5005() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16732_mdlrepair374_5006() { var g = Game.Solo(seed: 395206); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16733_mdlrepair374_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395207 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16734_mdlrepair374_5008() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16735_mdlrepair374_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16736_mdlrepair374_5010() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16737_mdlrepair374_5011() { var g = Game.Solo(seed: 395211); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16738_mdlrepair374_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395212 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16739_mdlrepair374_5013() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16740_mdlrepair374_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16741_mdlrepair374_5015() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16742_mdlrepair374_5016() { var g = Game.Solo(seed: 395216); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16743_mdlrepair374_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395217 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16744_mdlrepair374_5018() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16745_mdlrepair374_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16746_mdlrepair374_5020() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16747_mdlrepair374_5021() { var g = Game.Solo(seed: 395221); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16748_mdlrepair374_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395222 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16749_mdlrepair374_5023() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16750_mdlrepair374_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16751_mdlrepair374_5025() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16752_mdlrepair374_5026() { var g = Game.Solo(seed: 395226); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16753_mdlrepair374_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395227 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16754_mdlrepair374_5028() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16755_mdlrepair374_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16756_mdlrepair374_5030() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16757_mdlrepair374_5031() { var g = Game.Solo(seed: 395231); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16758_mdlrepair374_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395232 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16759_mdlrepair374_5033() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16760_mdlrepair374_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16761_mdlrepair374_5035() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16762_mdlrepair374_5036() { var g = Game.Solo(seed: 395236); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16763_mdlrepair374_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395237 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16764_mdlrepair374_5038() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16765_mdlrepair374_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16766_mdlrepair374_5040() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16767_mdlrepair374_5041() { var g = Game.Solo(seed: 395241); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16768_mdlrepair374_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395242 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16769_mdlrepair374_5043() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16770_mdlrepair374_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16771_mdlrepair374_5045() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16772_mdlrepair374_5046() { var g = Game.Solo(seed: 395246); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16773_mdlrepair374_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395247 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16774_mdlrepair374_5048() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16775_mdlrepair374_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16776_mdlrepair374_5050() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16777_mdlrepair374_5051() { var g = Game.Solo(seed: 395251); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16778_mdlrepair374_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395252 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16779_mdlrepair374_5053() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16780_mdlrepair374_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16781_mdlrepair374_5055() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16782_mdlrepair374_5056() { var g = Game.Solo(seed: 395256); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16783_mdlrepair374_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395257 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16784_mdlrepair374_5058() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16785_mdlrepair374_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16786_mdlrepair374_5060() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16787_mdlrepair374_5061() { var g = Game.Solo(seed: 395261); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16788_mdlrepair374_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395262 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16789_mdlrepair374_5063() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16790_mdlrepair374_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
    [Fact] public void Q16791_mdlrepair374_5065() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneMinCost > 0); Assert.True(c.Repair.FineTuneAlignment > 0); }
    [Fact] public void Q16792_mdlrepair374_5066() { var g = Game.Solo(seed: 395266); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16793_mdlrepair374_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 395267 })).Sim); Assert.True(c.Repair.FineTuneRevenueWeeks > 0); }
    [Fact] public void Q16794_mdlrepair374_5068() { GameConfig c = TestData.Config(); Assert.True(c.Repair.FineTuneAlignment > 0); Assert.True(c.Safety.GlitchWeeks > 0); }
    [Fact] public void Q16795_mdlrepair374_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247700); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Repair.FineTuneMinCost > 0); }
}
