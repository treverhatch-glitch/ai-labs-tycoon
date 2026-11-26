using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16166+: batch 366 NEXT bible tail (secflav).</summary>
public class HatchSectorFlavourBarLaterTests27
{
    [Fact] public void Q16166_secflav366_4000() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16167_secflav366_4001() { var g = Game.Solo(seed: 391101); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16168_secflav366_4002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391202 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16169_secflav366_4003() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16170_secflav366_4004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16171_secflav366_4005() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16172_secflav366_4006() { var g = Game.Solo(seed: 391106); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16173_secflav366_4007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391207 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16174_secflav366_4008() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16175_secflav366_4009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16176_secflav366_4010() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16177_secflav366_4011() { var g = Game.Solo(seed: 391111); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16178_secflav366_4012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391212 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16179_secflav366_4013() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16180_secflav366_4014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16181_secflav366_4015() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16182_secflav366_4016() { var g = Game.Solo(seed: 391116); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16183_secflav366_4017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391217 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16184_secflav366_4018() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16185_secflav366_4019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16186_secflav366_4020() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16187_secflav366_4021() { var g = Game.Solo(seed: 391121); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16188_secflav366_4022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391222 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16189_secflav366_4023() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16190_secflav366_4024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16191_secflav366_4025() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16192_secflav366_4026() { var g = Game.Solo(seed: 391126); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16193_secflav366_4027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391227 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16194_secflav366_4028() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16195_secflav366_4029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16196_secflav366_4030() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16197_secflav366_4031() { var g = Game.Solo(seed: 391131); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16198_secflav366_4032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391232 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16199_secflav366_4033() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16200_secflav366_4034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16201_secflav366_4035() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16202_secflav366_4036() { var g = Game.Solo(seed: 391136); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16203_secflav366_4037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391237 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16204_secflav366_4038() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16205_secflav366_4039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16206_secflav366_4040() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16207_secflav366_4041() { var g = Game.Solo(seed: 391141); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16208_secflav366_4042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391242 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16209_secflav366_4043() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16210_secflav366_4044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16211_secflav366_4045() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16212_secflav366_4046() { var g = Game.Solo(seed: 391146); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16213_secflav366_4047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391247 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16214_secflav366_4048() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16215_secflav366_4049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16216_secflav366_4050() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16217_secflav366_4051() { var g = Game.Solo(seed: 391151); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16218_secflav366_4052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391252 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16219_secflav366_4053() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16220_secflav366_4054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16221_secflav366_4055() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16222_secflav366_4056() { var g = Game.Solo(seed: 391156); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16223_secflav366_4057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391257 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16224_secflav366_4058() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16225_secflav366_4059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16226_secflav366_4060() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16227_secflav366_4061() { var g = Game.Solo(seed: 391161); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16228_secflav366_4062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391262 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16229_secflav366_4063() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16230_secflav366_4064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16231_secflav366_4065() { GameConfig c = TestData.Config(); Assert.True(c.Safety.SectorFlavours.Count >= 1); Assert.True(c.Safety.SectorFlavoursForRivals); }
    [Fact] public void Q16232_secflav366_4066() { var g = Game.Solo(seed: 391166); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16233_secflav366_4067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 391267 })).Sim); Assert.True(c.Safety.SectorFlavours.Count >= 1); }
    [Fact] public void Q16234_secflav366_4068() { GameConfig c = TestData.Config(); Assert.All(c.Safety.SectorFlavours, f => Assert.False(string.IsNullOrEmpty(f.Sector))); }
    [Fact] public void Q16235_secflav366_4069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 246900); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.SectorFlavours.Count >= 1); }
}
