using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6356–6425: batch 200 incident/safety data bar coda (schema 29).</summary>
public class HatchIncidentSafetyBarLaterTests10
{
    [Fact] public void Q13446_incsafe10_3846() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13447_incsafe10_3847() { var g = Game.Solo(seed: 132722); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13448_incsafe10_3848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132823 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13449_incsafe10_3849() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13450_incsafe10_3850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13451_incsafe10_3851() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13452_incsafe10_3852() { var g = Game.Solo(seed: 132727); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13453_incsafe10_3853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132828 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13454_incsafe10_3854() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13455_incsafe10_3855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13456_incsafe10_3856() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13457_incsafe10_3857() { var g = Game.Solo(seed: 132732); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13458_incsafe10_3858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132833 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13459_incsafe10_3859() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13460_incsafe10_3860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13461_incsafe10_3861() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13462_incsafe10_3862() { var g = Game.Solo(seed: 132737); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13463_incsafe10_3863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132838 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13464_incsafe10_3864() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13465_incsafe10_3865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13466_incsafe10_3866() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13467_incsafe10_3867() { var g = Game.Solo(seed: 132742); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13468_incsafe10_3868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132843 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13469_incsafe10_3869() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13470_incsafe10_3870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13471_incsafe10_3871() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13472_incsafe10_3872() { var g = Game.Solo(seed: 132747); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13473_incsafe10_3873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132848 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13474_incsafe10_3874() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13475_incsafe10_3875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13476_incsafe10_3876() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13477_incsafe10_3877() { var g = Game.Solo(seed: 132752); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13478_incsafe10_3878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132853 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13479_incsafe10_3879() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13480_incsafe10_3880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13481_incsafe10_3881() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13482_incsafe10_3882() { var g = Game.Solo(seed: 132757); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13483_incsafe10_3883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132858 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13484_incsafe10_3884() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13485_incsafe10_3885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13486_incsafe10_3886() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13487_incsafe10_3887() { var g = Game.Solo(seed: 132762); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13488_incsafe10_3888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132863 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13489_incsafe10_3889() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13490_incsafe10_3890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13491_incsafe10_3891() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13492_incsafe10_3892() { var g = Game.Solo(seed: 132767); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13493_incsafe10_3893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132868 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13494_incsafe10_3894() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13495_incsafe10_3895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13496_incsafe10_3896() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13497_incsafe10_3897() { var g = Game.Solo(seed: 132772); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13498_incsafe10_3898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132873 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13499_incsafe10_3899() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13500_incsafe10_3900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13501_incsafe10_3901() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13502_incsafe10_3902() { var g = Game.Solo(seed: 132777); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13503_incsafe10_3903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132878 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13504_incsafe10_3904() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13505_incsafe10_3905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13506_incsafe10_3906() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13507_incsafe10_3907() { var g = Game.Solo(seed: 132782); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13508_incsafe10_3908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132883 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13509_incsafe10_3909() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13510_incsafe10_3910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13511_incsafe10_3911() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13512_incsafe10_3912() { var g = Game.Solo(seed: 132787); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13513_incsafe10_3913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132888 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13514_incsafe10_3914() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13515_incsafe10_3915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 182); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
}
