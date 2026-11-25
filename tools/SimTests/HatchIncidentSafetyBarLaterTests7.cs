using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6356–6425: batch 140 incident/safety data bar coda (schema 29).</summary>
public class HatchIncidentSafetyBarLaterTests7
{
    [Fact] public void Q9436_incsafe7_3846() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9437_incsafe7_3847() { var g = Game.Solo(seed: 77282); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9438_incsafe7_3848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77383 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9439_incsafe7_3849() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9440_incsafe7_3850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9441_incsafe7_3851() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9442_incsafe7_3852() { var g = Game.Solo(seed: 77287); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9443_incsafe7_3853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77388 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9444_incsafe7_3854() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9445_incsafe7_3855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9446_incsafe7_3856() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9447_incsafe7_3857() { var g = Game.Solo(seed: 77292); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9448_incsafe7_3858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77393 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9449_incsafe7_3859() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9450_incsafe7_3860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9451_incsafe7_3861() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9452_incsafe7_3862() { var g = Game.Solo(seed: 77297); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9453_incsafe7_3863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77398 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9454_incsafe7_3864() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9455_incsafe7_3865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9456_incsafe7_3866() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9457_incsafe7_3867() { var g = Game.Solo(seed: 77302); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9458_incsafe7_3868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77403 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9459_incsafe7_3869() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9460_incsafe7_3870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9461_incsafe7_3871() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9462_incsafe7_3872() { var g = Game.Solo(seed: 77307); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9463_incsafe7_3873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77408 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9464_incsafe7_3874() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9465_incsafe7_3875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9466_incsafe7_3876() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9467_incsafe7_3877() { var g = Game.Solo(seed: 77312); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9468_incsafe7_3878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77413 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9469_incsafe7_3879() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9470_incsafe7_3880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9471_incsafe7_3881() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9472_incsafe7_3882() { var g = Game.Solo(seed: 77317); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9473_incsafe7_3883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77418 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9474_incsafe7_3884() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9475_incsafe7_3885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9476_incsafe7_3886() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9477_incsafe7_3887() { var g = Game.Solo(seed: 77322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9478_incsafe7_3888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77423 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9479_incsafe7_3889() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9480_incsafe7_3890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9481_incsafe7_3891() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9482_incsafe7_3892() { var g = Game.Solo(seed: 77327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9483_incsafe7_3893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77428 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9484_incsafe7_3894() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9485_incsafe7_3895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9486_incsafe7_3896() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9487_incsafe7_3897() { var g = Game.Solo(seed: 77332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9488_incsafe7_3898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77433 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9489_incsafe7_3899() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9490_incsafe7_3900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9491_incsafe7_3901() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9492_incsafe7_3902() { var g = Game.Solo(seed: 77337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9493_incsafe7_3903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77438 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9494_incsafe7_3904() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9495_incsafe7_3905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9496_incsafe7_3906() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9497_incsafe7_3907() { var g = Game.Solo(seed: 77342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9498_incsafe7_3908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77443 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9499_incsafe7_3909() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9500_incsafe7_3910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q9501_incsafe7_3911() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q9502_incsafe7_3912() { var g = Game.Solo(seed: 77347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q9503_incsafe7_3913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 77448 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q9504_incsafe7_3914() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q9505_incsafe7_3915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
}
