using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6356–6425: batch 96 incident/safety data bar coda (schema 29).</summary>
public class HatchIncidentSafetyBarLaterTests5
{
    [Fact] public void Q5236_incsafe2_3846() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5237_incsafe2_3847() { var g = Game.Solo(seed: 34302); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5238_incsafe2_3848() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34403 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5239_incsafe2_3849() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5240_incsafe2_3850() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5241_incsafe2_3851() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5242_incsafe2_3852() { var g = Game.Solo(seed: 34307); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5243_incsafe2_3853() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34408 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5244_incsafe2_3854() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5245_incsafe2_3855() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5246_incsafe2_3856() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5247_incsafe2_3857() { var g = Game.Solo(seed: 34312); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5248_incsafe2_3858() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34413 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5249_incsafe2_3859() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5250_incsafe2_3860() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5251_incsafe2_3861() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5252_incsafe2_3862() { var g = Game.Solo(seed: 34317); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5253_incsafe2_3863() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34418 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5254_incsafe2_3864() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5255_incsafe2_3865() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5256_incsafe2_3866() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5257_incsafe2_3867() { var g = Game.Solo(seed: 34322); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5258_incsafe2_3868() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34423 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5259_incsafe2_3869() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5260_incsafe2_3870() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5261_incsafe2_3871() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5262_incsafe2_3872() { var g = Game.Solo(seed: 34327); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5263_incsafe2_3873() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34428 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5264_incsafe2_3874() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5265_incsafe2_3875() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5266_incsafe2_3876() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5267_incsafe2_3877() { var g = Game.Solo(seed: 34332); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5268_incsafe2_3878() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34433 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5269_incsafe2_3879() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5270_incsafe2_3880() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5271_incsafe2_3881() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5272_incsafe2_3882() { var g = Game.Solo(seed: 34337); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5273_incsafe2_3883() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34438 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5274_incsafe2_3884() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5275_incsafe2_3885() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5276_incsafe2_3886() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5277_incsafe2_3887() { var g = Game.Solo(seed: 34342); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5278_incsafe2_3888() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34443 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5279_incsafe2_3889() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5280_incsafe2_3890() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5281_incsafe2_3891() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5282_incsafe2_3892() { var g = Game.Solo(seed: 34347); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5283_incsafe2_3893() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34448 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5284_incsafe2_3894() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5285_incsafe2_3895() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5286_incsafe2_3896() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5287_incsafe2_3897() { var g = Game.Solo(seed: 34352); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5288_incsafe2_3898() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34453 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5289_incsafe2_3899() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5290_incsafe2_3900() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5291_incsafe2_3901() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5292_incsafe2_3902() { var g = Game.Solo(seed: 34357); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5293_incsafe2_3903() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34458 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5294_incsafe2_3904() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5295_incsafe2_3905() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5296_incsafe2_3906() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5297_incsafe2_3907() { var g = Game.Solo(seed: 34362); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5298_incsafe2_3908() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34463 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5299_incsafe2_3909() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5300_incsafe2_3910() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q5301_incsafe2_3911() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q5302_incsafe2_3912() { var g = Game.Solo(seed: 34367); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q5303_incsafe2_3913() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 34468 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q5304_incsafe2_3914() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q5305_incsafe2_3915() { GameConfig c = TestData.Config(); var g = Game.New(seed: 78); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
}
