using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16236+: batch 367 NEXT bible tail (safcul).</summary>
public class HatchSafetyCultureBarLaterTests28
{
    [Fact] public void Q16236_safcul367_4000() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16237_safcul367_4001() { var g = Game.Solo(seed: 392101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16238_safcul367_4002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392202 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16239_safcul367_4003() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16240_safcul367_4004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16241_safcul367_4005() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16242_safcul367_4006() { var g = Game.Solo(seed: 392106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16243_safcul367_4007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392207 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16244_safcul367_4008() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16245_safcul367_4009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16246_safcul367_4010() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16247_safcul367_4011() { var g = Game.Solo(seed: 392111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16248_safcul367_4012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392212 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16249_safcul367_4013() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16250_safcul367_4014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16251_safcul367_4015() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16252_safcul367_4016() { var g = Game.Solo(seed: 392116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16253_safcul367_4017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392217 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16254_safcul367_4018() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16255_safcul367_4019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16256_safcul367_4020() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16257_safcul367_4021() { var g = Game.Solo(seed: 392121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16258_safcul367_4022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392222 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16259_safcul367_4023() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16260_safcul367_4024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16261_safcul367_4025() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16262_safcul367_4026() { var g = Game.Solo(seed: 392126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16263_safcul367_4027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392227 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16264_safcul367_4028() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16265_safcul367_4029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16266_safcul367_4030() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16267_safcul367_4031() { var g = Game.Solo(seed: 392131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16268_safcul367_4032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392232 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16269_safcul367_4033() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16270_safcul367_4034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16271_safcul367_4035() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16272_safcul367_4036() { var g = Game.Solo(seed: 392136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16273_safcul367_4037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392237 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16274_safcul367_4038() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16275_safcul367_4039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16276_safcul367_4040() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16277_safcul367_4041() { var g = Game.Solo(seed: 392141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16278_safcul367_4042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392242 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16279_safcul367_4043() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16280_safcul367_4044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16281_safcul367_4045() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16282_safcul367_4046() { var g = Game.Solo(seed: 392146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16283_safcul367_4047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392247 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16284_safcul367_4048() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16285_safcul367_4049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16286_safcul367_4050() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16287_safcul367_4051() { var g = Game.Solo(seed: 392151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16288_safcul367_4052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392252 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16289_safcul367_4053() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16290_safcul367_4054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16291_safcul367_4055() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16292_safcul367_4056() { var g = Game.Solo(seed: 392156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16293_safcul367_4057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392257 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16294_safcul367_4058() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16295_safcul367_4059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16296_safcul367_4060() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16297_safcul367_4061() { var g = Game.Solo(seed: 392161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16298_safcul367_4062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392262 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16299_safcul367_4063() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16300_safcul367_4064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
    [Fact] public void Q16301_safcul367_4065() { GameConfig c = TestData.Config(); Assert.True(c.Culture.Start > 0); Assert.True(c.Culture.Bands.Count >= 3); }
    [Fact] public void Q16302_safcul367_4066() { var g = Game.Solo(seed: 392166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.NeutralLevel > 0); }
    [Fact] public void Q16303_safcul367_4067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 392267 })).Sim); Assert.True(c.Culture.CatchSlopePermille >= 0); }
    [Fact] public void Q16304_safcul367_4068() { GameConfig c = TestData.Config(); Assert.True(c.Culture.MaxGate > 0); Assert.True(c.Culture.GateLockWeeks > 0); }
    [Fact] public void Q16305_safcul367_4069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247000); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Culture.ListeningRoles.Count >= 1); }
}
