using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13586–13655: batch 240 incident/safety data bar coda (schema 29).</summary>
public class HatchIncidentSafetyBarLaterTests12
{
    [Fact] public void Q13586_incsafe12_3986() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13587_incsafe12_3987() { var g = Game.Solo(seed: 167922); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13588_incsafe12_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168023 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13589_incsafe12_3989() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13590_incsafe12_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13591_incsafe12_3991() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13592_incsafe12_3992() { var g = Game.Solo(seed: 167927); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13593_incsafe12_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168028 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13594_incsafe12_3994() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13595_incsafe12_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13596_incsafe12_3996() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13597_incsafe12_3997() { var g = Game.Solo(seed: 167932); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13598_incsafe12_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168033 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13599_incsafe12_3999() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13600_incsafe12_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13601_incsafe12_4001() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13602_incsafe12_4002() { var g = Game.Solo(seed: 167937); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13603_incsafe12_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168038 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13604_incsafe12_4004() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13605_incsafe12_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13606_incsafe12_4006() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13607_incsafe12_4007() { var g = Game.Solo(seed: 167942); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13608_incsafe12_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168043 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13609_incsafe12_4009() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13610_incsafe12_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13611_incsafe12_4011() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13612_incsafe12_4012() { var g = Game.Solo(seed: 167947); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13613_incsafe12_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168048 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13614_incsafe12_4014() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13615_incsafe12_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13616_incsafe12_4016() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13617_incsafe12_4017() { var g = Game.Solo(seed: 167952); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13618_incsafe12_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168053 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13619_incsafe12_4019() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13620_incsafe12_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13621_incsafe12_4021() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13622_incsafe12_4022() { var g = Game.Solo(seed: 167957); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13623_incsafe12_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168058 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13624_incsafe12_4024() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13625_incsafe12_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13626_incsafe12_4026() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13627_incsafe12_4027() { var g = Game.Solo(seed: 167962); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13628_incsafe12_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168063 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13629_incsafe12_4029() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13630_incsafe12_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13631_incsafe12_4031() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13632_incsafe12_4032() { var g = Game.Solo(seed: 167967); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13633_incsafe12_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168068 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13634_incsafe12_4034() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13635_incsafe12_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13636_incsafe12_4036() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13637_incsafe12_4037() { var g = Game.Solo(seed: 167972); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13638_incsafe12_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168073 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13639_incsafe12_4039() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13640_incsafe12_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13641_incsafe12_4041() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13642_incsafe12_4042() { var g = Game.Solo(seed: 167977); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13643_incsafe12_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168078 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13644_incsafe12_4044() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13645_incsafe12_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13646_incsafe12_4046() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13647_incsafe12_4047() { var g = Game.Solo(seed: 167982); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13648_incsafe12_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168083 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13649_incsafe12_4049() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13650_incsafe12_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13651_incsafe12_4051() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13652_incsafe12_4052() { var g = Game.Solo(seed: 167987); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13653_incsafe12_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 168088 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13654_incsafe12_4054() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13655_incsafe12_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35422); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
}
