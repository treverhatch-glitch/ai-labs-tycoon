using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3496–Q3565: batch 55 world-map economy data bar coda (schema 37).</summary>
public class HatchWorldMapEconomyBarLaterTests2
{
    [Fact] public void Q3496_world2_3496() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3497_world2_3497() { var g = Game.Solo(seed: 6782); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3498_world2_3498() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6884 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3499_world2_3499() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3500_world2_3500() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3501_world2_3501() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3502_world2_3502() { var g = Game.Solo(seed: 6787); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3503_world2_3503() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6889 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3504_world2_3504() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3505_world2_3505() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3506_world2_3506() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3507_world2_3507() { var g = Game.Solo(seed: 6792); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3508_world2_3508() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6894 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3509_world2_3509() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3510_world2_3510() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3511_world2_3511() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3512_world2_3512() { var g = Game.Solo(seed: 6797); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3513_world2_3513() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6899 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3514_world2_3514() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3515_world2_3515() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3516_world2_3516() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3517_world2_3517() { var g = Game.Solo(seed: 6802); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3518_world2_3518() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6904 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3519_world2_3519() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3520_world2_3520() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3521_world2_3521() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3522_world2_3522() { var g = Game.Solo(seed: 6807); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3523_world2_3523() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6909 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3524_world2_3524() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3525_world2_3525() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3526_world2_3526() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3527_world2_3527() { var g = Game.Solo(seed: 6812); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3528_world2_3528() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6914 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3529_world2_3529() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3530_world2_3530() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3531_world2_3531() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3532_world2_3532() { var g = Game.Solo(seed: 6817); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3533_world2_3533() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6919 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3534_world2_3534() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3535_world2_3535() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3536_world2_3536() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3537_world2_3537() { var g = Game.Solo(seed: 6822); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3538_world2_3538() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6924 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3539_world2_3539() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3540_world2_3540() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3541_world2_3541() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3542_world2_3542() { var g = Game.Solo(seed: 6827); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3543_world2_3543() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6929 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3544_world2_3544() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3545_world2_3545() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3546_world2_3546() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3547_world2_3547() { var g = Game.Solo(seed: 6832); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3548_world2_3548() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6934 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3549_world2_3549() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3550_world2_3550() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3551_world2_3551() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3552_world2_3552() { var g = Game.Solo(seed: 6837); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3553_world2_3553() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6939 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3554_world2_3554() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3555_world2_3555() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3556_world2_3556() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3557_world2_3557() { var g = Game.Solo(seed: 6842); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3558_world2_3558() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6944 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3559_world2_3559() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3560_world2_3560() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q3561_world2_3561() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q3562_world2_3562() { var g = Game.Solo(seed: 6847); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q3563_world2_3563() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6949 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q3564_world2_3564() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q3565_world2_3565() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(37, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
}
