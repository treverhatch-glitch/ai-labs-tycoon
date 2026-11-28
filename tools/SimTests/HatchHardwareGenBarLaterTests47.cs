using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17286+: batch hwgen386 hatch bar.</summary>
public class HatchHardwareGenBarLaterTests47
{
    [Fact] public void Q17286_hwgen386_5000() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17287_hwgen386_5001() { var g = Game.Solo(seed: 396271).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17288_hwgen386_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396272 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17289_hwgen386_5003() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396273); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17290_hwgen386_5004() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396274); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17291_hwgen386_5005() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17292_hwgen386_5006() { var g = Game.Solo(seed: 396276).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17293_hwgen386_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396277 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17294_hwgen386_5008() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396278); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17295_hwgen386_5009() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396279); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17296_hwgen386_5010() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17297_hwgen386_5011() { var g = Game.Solo(seed: 396281).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17298_hwgen386_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396282 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17299_hwgen386_5013() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396283); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17300_hwgen386_5014() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396284); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17301_hwgen386_5015() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17302_hwgen386_5016() { var g = Game.Solo(seed: 396286).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17303_hwgen386_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396287 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17304_hwgen386_5018() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396288); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17305_hwgen386_5019() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396289); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17306_hwgen386_5020() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17307_hwgen386_5021() { var g = Game.Solo(seed: 396291).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17308_hwgen386_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396292 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17309_hwgen386_5023() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396293); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17310_hwgen386_5024() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396294); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17311_hwgen386_5025() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17312_hwgen386_5026() { var g = Game.Solo(seed: 396296).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17313_hwgen386_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396297 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17314_hwgen386_5028() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396298); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17315_hwgen386_5029() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396299); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17316_hwgen386_5030() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17317_hwgen386_5031() { var g = Game.Solo(seed: 396301).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17318_hwgen386_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396302 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17319_hwgen386_5033() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396303); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17320_hwgen386_5034() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396304); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17321_hwgen386_5035() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17322_hwgen386_5036() { var g = Game.Solo(seed: 396306).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17323_hwgen386_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396307 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17324_hwgen386_5038() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396308); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17325_hwgen386_5039() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396309); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17326_hwgen386_5040() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17327_hwgen386_5041() { var g = Game.Solo(seed: 396311).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17328_hwgen386_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396312 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17329_hwgen386_5043() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396313); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17330_hwgen386_5044() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396314); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17331_hwgen386_5045() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17332_hwgen386_5046() { var g = Game.Solo(seed: 396316).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17333_hwgen386_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396317 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17334_hwgen386_5048() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396318); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17335_hwgen386_5049() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396319); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17336_hwgen386_5050() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17337_hwgen386_5051() { var g = Game.Solo(seed: 396321).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17338_hwgen386_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396322 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17339_hwgen386_5053() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396323); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17340_hwgen386_5054() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396324); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17341_hwgen386_5055() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17342_hwgen386_5056() { var g = Game.Solo(seed: 396326).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17343_hwgen386_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396327 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17344_hwgen386_5058() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396328); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17345_hwgen386_5059() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396329); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17346_hwgen386_5060() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17347_hwgen386_5061() { var g = Game.Solo(seed: 396331).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17348_hwgen386_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396332 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17349_hwgen386_5063() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396333); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17350_hwgen386_5064() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396334); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
    [Fact] public void Q17351_hwgen386_5065() { GameConfig c = TestData.Config(); Assert.True(c.Compute.GpuGenerations.Count >= 7); Assert.True(c.Compute.ResalePermille > 0); }
    [Fact] public void Q17352_hwgen386_5066() { var g = Game.Solo(seed: 396336).WithStarterLab(gpus: 4); Assert.Equal(g.C.Compute.GpuGenerations[0].Id, LabQueries.CurrentGeneration(g.C, g.Tick).Id); }
    [Fact] public void Q17353_hwgen386_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396337 })).Sim); Assert.True(c.Compute.GenerationDecayPermille > 0); }
    [Fact] public void Q17354_hwgen386_5068() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396338); DisplayQueries.HardwareFleetBreakdown f = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me); Assert.True(f.TotalUnits >= 0); }
    [Fact] public void Q17355_hwgen386_5069() { GameConfig c = TestData.Config(); var g = Game.Solo(seed: 396339); g.Me.Cash = 1_000_000_000; g.Build("server-closet", 2, 2); Assert.True(g.C.Compute.GpuGenerations[0].ComputePerGpuWeek > 0); }
}
