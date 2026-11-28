using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17846+: trait392.</summary>
public class HatchStaffTraitsBarLaterTests55
{
    [Fact] public void Q17846_trait392_5000() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17847_trait392_5001() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17848_trait392_5002() { var g = Game.Solo(seed: 396842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17849_trait392_5003() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17850_trait392_5004() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396844 })).Sim); }
    [Fact] public void Q17851_trait392_5005() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17852_trait392_5006() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17853_trait392_5007() { var g = Game.Solo(seed: 396847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17854_trait392_5008() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17855_trait392_5009() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396849 })).Sim); }
    [Fact] public void Q17856_trait392_5010() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17857_trait392_5011() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17858_trait392_5012() { var g = Game.Solo(seed: 396852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17859_trait392_5013() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17860_trait392_5014() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396854 })).Sim); }
    [Fact] public void Q17861_trait392_5015() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17862_trait392_5016() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17863_trait392_5017() { var g = Game.Solo(seed: 396857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17864_trait392_5018() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17865_trait392_5019() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396859 })).Sim); }
    [Fact] public void Q17866_trait392_5020() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17867_trait392_5021() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17868_trait392_5022() { var g = Game.Solo(seed: 396862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17869_trait392_5023() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17870_trait392_5024() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396864 })).Sim); }
    [Fact] public void Q17871_trait392_5025() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17872_trait392_5026() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17873_trait392_5027() { var g = Game.Solo(seed: 396867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17874_trait392_5028() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17875_trait392_5029() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396869 })).Sim); }
    [Fact] public void Q17876_trait392_5030() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17877_trait392_5031() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17878_trait392_5032() { var g = Game.Solo(seed: 396872); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17879_trait392_5033() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17880_trait392_5034() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396874 })).Sim); }
    [Fact] public void Q17881_trait392_5035() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17882_trait392_5036() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17883_trait392_5037() { var g = Game.Solo(seed: 396877); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17884_trait392_5038() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17885_trait392_5039() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396879 })).Sim); }
    [Fact] public void Q17886_trait392_5040() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17887_trait392_5041() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17888_trait392_5042() { var g = Game.Solo(seed: 396882); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17889_trait392_5043() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17890_trait392_5044() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396884 })).Sim); }
    [Fact] public void Q17891_trait392_5045() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17892_trait392_5046() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17893_trait392_5047() { var g = Game.Solo(seed: 396887); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17894_trait392_5048() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17895_trait392_5049() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396889 })).Sim); }
    [Fact] public void Q17896_trait392_5050() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17897_trait392_5051() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17898_trait392_5052() { var g = Game.Solo(seed: 396892); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17899_trait392_5053() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17900_trait392_5054() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396894 })).Sim); }
    [Fact] public void Q17901_trait392_5055() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17902_trait392_5056() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17903_trait392_5057() { var g = Game.Solo(seed: 396897); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17904_trait392_5058() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17905_trait392_5059() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396899 })).Sim); }
    [Fact] public void Q17906_trait392_5060() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17907_trait392_5061() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17908_trait392_5062() { var g = Game.Solo(seed: 396902); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17909_trait392_5063() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17910_trait392_5064() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396904 })).Sim); }
    [Fact] public void Q17911_trait392_5065() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale > 0); }
    [Fact] public void Q17912_trait392_5066() { GameConfig c = TestData.Config(); Assert.Empty(ConfigValidator.Problems(c)); }
    [Fact] public void Q17913_trait392_5067() { var g = Game.Solo(seed: 396907); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17914_trait392_5068() { GameConfig c = TestData.Config(); Assert.True(c.StaffPeople.Traits.Count >= 3); }
    [Fact] public void Q17915_trait392_5069() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396909 })).Sim); }
}
