using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17426+: batch cmpque388 hatch bar.</summary>
public class HatchComputeQueueBarLaterTests49
{
    [Fact] public void Q17426_cmpque388_5000() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17427_cmpque388_5001() { var g = Game.Solo(seed: 396411); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17428_cmpque388_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396412 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17429_cmpque388_5003() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396413, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17430_cmpque388_5004() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17431_cmpque388_5005() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17432_cmpque388_5006() { var g = Game.Solo(seed: 396416); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17433_cmpque388_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396417 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17434_cmpque388_5008() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396418, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17435_cmpque388_5009() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17436_cmpque388_5010() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17437_cmpque388_5011() { var g = Game.Solo(seed: 396421); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17438_cmpque388_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396422 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17439_cmpque388_5013() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396423, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17440_cmpque388_5014() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17441_cmpque388_5015() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17442_cmpque388_5016() { var g = Game.Solo(seed: 396426); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17443_cmpque388_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396427 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17444_cmpque388_5018() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396428, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17445_cmpque388_5019() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17446_cmpque388_5020() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17447_cmpque388_5021() { var g = Game.Solo(seed: 396431); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17448_cmpque388_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396432 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17449_cmpque388_5023() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396433, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17450_cmpque388_5024() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17451_cmpque388_5025() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17452_cmpque388_5026() { var g = Game.Solo(seed: 396436); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17453_cmpque388_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396437 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17454_cmpque388_5028() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396438, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17455_cmpque388_5029() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17456_cmpque388_5030() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17457_cmpque388_5031() { var g = Game.Solo(seed: 396441); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17458_cmpque388_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396442 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17459_cmpque388_5033() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396443, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17460_cmpque388_5034() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17461_cmpque388_5035() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17462_cmpque388_5036() { var g = Game.Solo(seed: 396446); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17463_cmpque388_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396447 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17464_cmpque388_5038() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396448, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17465_cmpque388_5039() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17466_cmpque388_5040() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17467_cmpque388_5041() { var g = Game.Solo(seed: 396451); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17468_cmpque388_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396452 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17469_cmpque388_5043() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396453, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17470_cmpque388_5044() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17471_cmpque388_5045() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17472_cmpque388_5046() { var g = Game.Solo(seed: 396456); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17473_cmpque388_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396457 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17474_cmpque388_5048() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396458, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17475_cmpque388_5049() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17476_cmpque388_5050() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17477_cmpque388_5051() { var g = Game.Solo(seed: 396461); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17478_cmpque388_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396462 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17479_cmpque388_5053() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396463, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17480_cmpque388_5054() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17481_cmpque388_5055() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17482_cmpque388_5056() { var g = Game.Solo(seed: 396466); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17483_cmpque388_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396467 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17484_cmpque388_5058() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396468, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17485_cmpque388_5059() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17486_cmpque388_5060() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17487_cmpque388_5061() { var g = Game.Solo(seed: 396471); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17488_cmpque388_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396472 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17489_cmpque388_5063() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396473, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17490_cmpque388_5064() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
    [Fact] public void Q17491_cmpque388_5065() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedContractWeeks > 0); Assert.True(c.Vendor.LogMax > 0); }
    [Fact] public void Q17492_cmpque388_5066() { var g = Game.Solo(seed: 396476); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Compute.VendorName.Length > 0); }
    [Fact] public void Q17493_cmpque388_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396477 })).Sim); Assert.True(c.ReservedComputeGate.Enabled >= 0); }
    [Fact] public void Q17494_cmpque388_5068() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396478, c => c.GpuTierUnlock.Enabled = 0); Assert.True(VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing >= 0); }
    [Fact] public void Q17495_cmpque388_5069() { GameConfig c = TestData.Config(); Assert.True(c.Compute.ReservedDepositWeeks > 0); }
}
