using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17356+: batch ownchip387 hatch bar.</summary>
public class HatchOwnChipsBarLaterTests48
{
    [Fact] public void Q17356_ownchip387_5000() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17357_ownchip387_5001() { var g = Game.Solo(seed: 396341); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17358_ownchip387_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396342 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17359_ownchip387_5003() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17360_ownchip387_5004() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396344); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17361_ownchip387_5005() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17362_ownchip387_5006() { var g = Game.Solo(seed: 396346); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17363_ownchip387_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396347 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17364_ownchip387_5008() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17365_ownchip387_5009() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396349); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17366_ownchip387_5010() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17367_ownchip387_5011() { var g = Game.Solo(seed: 396351); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17368_ownchip387_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396352 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17369_ownchip387_5013() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17370_ownchip387_5014() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396354); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17371_ownchip387_5015() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17372_ownchip387_5016() { var g = Game.Solo(seed: 396356); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17373_ownchip387_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396357 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17374_ownchip387_5018() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17375_ownchip387_5019() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396359); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17376_ownchip387_5020() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17377_ownchip387_5021() { var g = Game.Solo(seed: 396361); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17378_ownchip387_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396362 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17379_ownchip387_5023() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17380_ownchip387_5024() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396364); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17381_ownchip387_5025() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17382_ownchip387_5026() { var g = Game.Solo(seed: 396366); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17383_ownchip387_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396367 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17384_ownchip387_5028() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17385_ownchip387_5029() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396369); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17386_ownchip387_5030() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17387_ownchip387_5031() { var g = Game.Solo(seed: 396371); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17388_ownchip387_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396372 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17389_ownchip387_5033() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17390_ownchip387_5034() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396374); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17391_ownchip387_5035() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17392_ownchip387_5036() { var g = Game.Solo(seed: 396376); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17393_ownchip387_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396377 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17394_ownchip387_5038() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17395_ownchip387_5039() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396379); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17396_ownchip387_5040() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17397_ownchip387_5041() { var g = Game.Solo(seed: 396381); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17398_ownchip387_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396382 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17399_ownchip387_5043() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17400_ownchip387_5044() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396384); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17401_ownchip387_5045() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17402_ownchip387_5046() { var g = Game.Solo(seed: 396386); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17403_ownchip387_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396387 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17404_ownchip387_5048() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17405_ownchip387_5049() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396389); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17406_ownchip387_5050() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17407_ownchip387_5051() { var g = Game.Solo(seed: 396391); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17408_ownchip387_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396392 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17409_ownchip387_5053() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17410_ownchip387_5054() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396394); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17411_ownchip387_5055() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17412_ownchip387_5056() { var g = Game.Solo(seed: 396396); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17413_ownchip387_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396397 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17414_ownchip387_5058() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17415_ownchip387_5059() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396399); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17416_ownchip387_5060() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17417_ownchip387_5061() { var g = Game.Solo(seed: 396401); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17418_ownchip387_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396402 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17419_ownchip387_5063() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17420_ownchip387_5064() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396404); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q17421_ownchip387_5065() { GameConfig c = TestData.Config(); Assert.True(c.Chips.TapeoutWeeks > 0); Assert.True(c.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q17422_ownchip387_5066() { var g = Game.Solo(seed: 396406); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q17423_ownchip387_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396407 })).Sim); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); }
    [Fact] public void Q17424_ownchip387_5068() { GameConfig c = TestData.Config(); Assert.True(c.Chips.Targets.Count > 0); }
    [Fact] public void Q17425_ownchip387_5069() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396409); Assert.True(g.C.Chips.TapeoutWeeks > 0); }
}
