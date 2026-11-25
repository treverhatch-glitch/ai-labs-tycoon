using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13516–13585: batch 220 incident/safety data bar coda (schema 29).</summary>
public class HatchIncidentSafetyBarLaterTests11
{
    [Fact] public void Q13516_incsafe11_3916() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13517_incsafe11_3917() { var g = Game.Solo(seed: 150322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13518_incsafe11_3918() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150423 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13519_incsafe11_3919() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13520_incsafe11_3920() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13521_incsafe11_3921() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13522_incsafe11_3922() { var g = Game.Solo(seed: 150327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13523_incsafe11_3923() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150428 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13524_incsafe11_3924() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13525_incsafe11_3925() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13526_incsafe11_3926() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13527_incsafe11_3927() { var g = Game.Solo(seed: 150332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13528_incsafe11_3928() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150433 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13529_incsafe11_3929() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13530_incsafe11_3930() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13531_incsafe11_3931() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13532_incsafe11_3932() { var g = Game.Solo(seed: 150337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13533_incsafe11_3933() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150438 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13534_incsafe11_3934() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13535_incsafe11_3935() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13536_incsafe11_3936() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13537_incsafe11_3937() { var g = Game.Solo(seed: 150342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13538_incsafe11_3938() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150443 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13539_incsafe11_3939() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13540_incsafe11_3940() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13541_incsafe11_3941() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13542_incsafe11_3942() { var g = Game.Solo(seed: 150347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13543_incsafe11_3943() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150448 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13544_incsafe11_3944() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13545_incsafe11_3945() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13546_incsafe11_3946() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13547_incsafe11_3947() { var g = Game.Solo(seed: 150352); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13548_incsafe11_3948() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150453 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13549_incsafe11_3949() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13550_incsafe11_3950() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13551_incsafe11_3951() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13552_incsafe11_3952() { var g = Game.Solo(seed: 150357); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13553_incsafe11_3953() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150458 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13554_incsafe11_3954() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13555_incsafe11_3955() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13556_incsafe11_3956() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13557_incsafe11_3957() { var g = Game.Solo(seed: 150362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13558_incsafe11_3958() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150463 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13559_incsafe11_3959() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13560_incsafe11_3960() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13561_incsafe11_3961() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13562_incsafe11_3962() { var g = Game.Solo(seed: 150367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13563_incsafe11_3963() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150468 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13564_incsafe11_3964() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13565_incsafe11_3965() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13566_incsafe11_3966() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13567_incsafe11_3967() { var g = Game.Solo(seed: 150372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13568_incsafe11_3968() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150473 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13569_incsafe11_3969() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13570_incsafe11_3970() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13571_incsafe11_3971() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13572_incsafe11_3972() { var g = Game.Solo(seed: 150377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13573_incsafe11_3973() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150478 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13574_incsafe11_3974() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13575_incsafe11_3975() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13576_incsafe11_3976() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13577_incsafe11_3977() { var g = Game.Solo(seed: 150382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13578_incsafe11_3978() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150483 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13579_incsafe11_3979() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13580_incsafe11_3980() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
    [Fact] public void Q13581_incsafe11_3981() { GameConfig c = TestData.Config(); Assert.True(c.Shady.InvestigationThreshold > 0); Assert.True(c.Shady.AuditInvestigationPermille >= 0); }
    [Fact] public void Q13582_incsafe11_3982() { var g = Game.Solo(seed: 150387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.HeatDecayPerWeek > 0); }
    [Fact] public void Q13583_incsafe11_3983() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 150488 })).Sim); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q13584_incsafe11_3984() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.IncidentGain > 0); Assert.True(c.Shady.InvestigationDivisor > 0); }
    [Fact] public void Q13585_incsafe11_3985() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.ConvictionDivisor > 0); }
}
