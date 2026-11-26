using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16516+: batch 371 rivals bible tail (rivchip).</summary>
public class HatchRivalChipsDepthBarLaterTests32
{
    [Fact] public void Q16516_rivchip371_5000() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16517_rivchip371_5001() { var g = Game.Solo(seed: 394101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16518_rivchip371_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394102 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16519_rivchip371_5003() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16520_rivchip371_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16521_rivchip371_5005() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16522_rivchip371_5006() { var g = Game.Solo(seed: 394106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16523_rivchip371_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394107 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16524_rivchip371_5008() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16525_rivchip371_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16526_rivchip371_5010() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16527_rivchip371_5011() { var g = Game.Solo(seed: 394111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16528_rivchip371_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394112 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16529_rivchip371_5013() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16530_rivchip371_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16531_rivchip371_5015() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16532_rivchip371_5016() { var g = Game.Solo(seed: 394116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16533_rivchip371_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394117 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16534_rivchip371_5018() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16535_rivchip371_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16536_rivchip371_5020() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16537_rivchip371_5021() { var g = Game.Solo(seed: 394121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16538_rivchip371_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394122 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16539_rivchip371_5023() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16540_rivchip371_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16541_rivchip371_5025() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16542_rivchip371_5026() { var g = Game.Solo(seed: 394126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16543_rivchip371_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394127 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16544_rivchip371_5028() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16545_rivchip371_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16546_rivchip371_5030() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16547_rivchip371_5031() { var g = Game.Solo(seed: 394131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16548_rivchip371_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394132 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16549_rivchip371_5033() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16550_rivchip371_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16551_rivchip371_5035() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16552_rivchip371_5036() { var g = Game.Solo(seed: 394136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16553_rivchip371_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394137 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16554_rivchip371_5038() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16555_rivchip371_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16556_rivchip371_5040() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16557_rivchip371_5041() { var g = Game.Solo(seed: 394141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16558_rivchip371_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394142 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16559_rivchip371_5043() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16560_rivchip371_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16561_rivchip371_5045() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16562_rivchip371_5046() { var g = Game.Solo(seed: 394146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16563_rivchip371_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394147 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16564_rivchip371_5048() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16565_rivchip371_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16566_rivchip371_5050() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16567_rivchip371_5051() { var g = Game.Solo(seed: 394151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16568_rivchip371_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394152 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16569_rivchip371_5053() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16570_rivchip371_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16571_rivchip371_5055() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16572_rivchip371_5056() { var g = Game.Solo(seed: 394156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16573_rivchip371_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394157 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16574_rivchip371_5058() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16575_rivchip371_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16576_rivchip371_5060() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16577_rivchip371_5061() { var g = Game.Solo(seed: 394161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16578_rivchip371_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394162 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16579_rivchip371_5063() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16580_rivchip371_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
    [Fact] public void Q16581_rivchip371_5065() { GameConfig c = TestData.Config(); Assert.True(c.Chips.FoundryVendorSharePermille >= 0); Assert.True(c.Rivalry.LogMax > 0); }
    [Fact] public void Q16582_rivchip371_5066() { var g = Game.Solo(seed: 394166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Listing.ListingFeePermille >= 0); }
    [Fact] public void Q16583_rivchip371_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394167 })).Sim); Assert.True(c.Chips.TapeoutWeeks > 0); }
    [Fact] public void Q16584_rivchip371_5068() { GameConfig c = TestData.Config(); Assert.True(c.Rivalry.RetrenchRunwayWeeks > 0); Assert.True(c.Rivalry.RetrenchCostPermille > 0); }
    [Fact] public void Q16585_rivchip371_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247400); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chips.Targets.Count >= 1); }
}
