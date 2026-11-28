using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>18126+: flav396.</summary>
public class HatchRivalSectorFlavourBarLaterTests59
{
    [Fact] public void Q18126_flav396_5000() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18127_flav396_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18128_flav396_5002() { var g = Game.Solo(seed: 397122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18129_flav396_5003() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18130_flav396_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397124 })).Sim); }
    [Fact] public void Q18131_flav396_5005() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18132_flav396_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18133_flav396_5007() { var g = Game.Solo(seed: 397127); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18134_flav396_5008() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18135_flav396_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397129 })).Sim); }
    [Fact] public void Q18136_flav396_5010() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18137_flav396_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18138_flav396_5012() { var g = Game.Solo(seed: 397132); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18139_flav396_5013() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18140_flav396_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397134 })).Sim); }
    [Fact] public void Q18141_flav396_5015() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18142_flav396_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18143_flav396_5017() { var g = Game.Solo(seed: 397137); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18144_flav396_5018() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18145_flav396_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397139 })).Sim); }
    [Fact] public void Q18146_flav396_5020() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18147_flav396_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18148_flav396_5022() { var g = Game.Solo(seed: 397142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18149_flav396_5023() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18150_flav396_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397144 })).Sim); }
    [Fact] public void Q18151_flav396_5025() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18152_flav396_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18153_flav396_5027() { var g = Game.Solo(seed: 397147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18154_flav396_5028() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18155_flav396_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397149 })).Sim); }
    [Fact] public void Q18156_flav396_5030() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18157_flav396_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18158_flav396_5032() { var g = Game.Solo(seed: 397152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18159_flav396_5033() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18160_flav396_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397154 })).Sim); }
    [Fact] public void Q18161_flav396_5035() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18162_flav396_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18163_flav396_5037() { var g = Game.Solo(seed: 397157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18164_flav396_5038() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18165_flav396_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397159 })).Sim); }
    [Fact] public void Q18166_flav396_5040() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18167_flav396_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18168_flav396_5042() { var g = Game.Solo(seed: 397162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18169_flav396_5043() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18170_flav396_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397164 })).Sim); }
    [Fact] public void Q18171_flav396_5045() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18172_flav396_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18173_flav396_5047() { var g = Game.Solo(seed: 397167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18174_flav396_5048() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18175_flav396_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397169 })).Sim); }
    [Fact] public void Q18176_flav396_5050() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18177_flav396_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18178_flav396_5052() { var g = Game.Solo(seed: 397172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18179_flav396_5053() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18180_flav396_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397174 })).Sim); }
    [Fact] public void Q18181_flav396_5055() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18182_flav396_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18183_flav396_5057() { var g = Game.Solo(seed: 397177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18184_flav396_5058() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18185_flav396_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397179 })).Sim); }
    [Fact] public void Q18186_flav396_5060() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18187_flav396_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18188_flav396_5062() { var g = Game.Solo(seed: 397182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18189_flav396_5063() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18190_flav396_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397184 })).Sim); }
    [Fact] public void Q18191_flav396_5065() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q18192_flav396_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q18193_flav396_5067() { var g = Game.Solo(seed: 397187); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q18194_flav396_5068() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q18195_flav396_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 397189 })).Sim); }
}
