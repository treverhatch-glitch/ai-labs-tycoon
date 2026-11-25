using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6286–6355: batch 199 shady/china data bar coda (schema 29).</summary>
public class HatchShadyChinaBarLaterTests10
{
    [Fact] public void Q13376_shchina10_3776() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13377_shchina10_3777() { var g = Game.Solo(seed: 131842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13378_shchina10_3778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131943 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13379_shchina10_3779() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13380_shchina10_3780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13381_shchina10_3781() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13382_shchina10_3782() { var g = Game.Solo(seed: 131847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13383_shchina10_3783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131948 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13384_shchina10_3784() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13385_shchina10_3785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13386_shchina10_3786() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13387_shchina10_3787() { var g = Game.Solo(seed: 131852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13388_shchina10_3788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131953 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13389_shchina10_3789() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13390_shchina10_3790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13391_shchina10_3791() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13392_shchina10_3792() { var g = Game.Solo(seed: 131857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13393_shchina10_3793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131958 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13394_shchina10_3794() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13395_shchina10_3795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13396_shchina10_3796() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13397_shchina10_3797() { var g = Game.Solo(seed: 131862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13398_shchina10_3798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131963 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13399_shchina10_3799() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13400_shchina10_3800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13401_shchina10_3801() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13402_shchina10_3802() { var g = Game.Solo(seed: 131867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13403_shchina10_3803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131968 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13404_shchina10_3804() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13405_shchina10_3805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13406_shchina10_3806() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13407_shchina10_3807() { var g = Game.Solo(seed: 131872); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13408_shchina10_3808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131973 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13409_shchina10_3809() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13410_shchina10_3810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13411_shchina10_3811() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13412_shchina10_3812() { var g = Game.Solo(seed: 131877); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13413_shchina10_3813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131978 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13414_shchina10_3814() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13415_shchina10_3815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13416_shchina10_3816() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13417_shchina10_3817() { var g = Game.Solo(seed: 131882); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13418_shchina10_3818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131983 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13419_shchina10_3819() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13420_shchina10_3820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13421_shchina10_3821() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13422_shchina10_3822() { var g = Game.Solo(seed: 131887); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13423_shchina10_3823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131988 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13424_shchina10_3824() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13425_shchina10_3825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13426_shchina10_3826() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13427_shchina10_3827() { var g = Game.Solo(seed: 131892); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13428_shchina10_3828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131993 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13429_shchina10_3829() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13430_shchina10_3830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13431_shchina10_3831() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13432_shchina10_3832() { var g = Game.Solo(seed: 131897); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13433_shchina10_3833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131998 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13434_shchina10_3834() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13435_shchina10_3835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13436_shchina10_3836() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13437_shchina10_3837() { var g = Game.Solo(seed: 131902); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13438_shchina10_3838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132003 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13439_shchina10_3839() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13440_shchina10_3840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13441_shchina10_3841() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13442_shchina10_3842() { var g = Game.Solo(seed: 131907); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13443_shchina10_3843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 132008 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13444_shchina10_3844() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13445_shchina10_3845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 181); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
}
