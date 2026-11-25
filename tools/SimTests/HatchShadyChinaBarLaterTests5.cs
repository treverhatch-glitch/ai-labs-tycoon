using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6286–6355: batch 95 shady/china data bar coda (schema 29).</summary>
public class HatchShadyChinaBarLaterTests5
{
    [Fact] public void Q6286_shchina4_3776() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6287_shchina4_3777() { var g = Game.Solo(seed: 33322); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6288_shchina4_3778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33423 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6289_shchina4_3779() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6290_shchina4_3780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6291_shchina4_3781() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6292_shchina4_3782() { var g = Game.Solo(seed: 33327); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6293_shchina4_3783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33428 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6294_shchina4_3784() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6295_shchina4_3785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6296_shchina4_3786() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6297_shchina4_3787() { var g = Game.Solo(seed: 33332); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6298_shchina4_3788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33433 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6299_shchina4_3789() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6300_shchina4_3790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6301_shchina4_3791() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6302_shchina4_3792() { var g = Game.Solo(seed: 33337); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6303_shchina4_3793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33438 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6304_shchina4_3794() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6305_shchina4_3795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6306_shchina4_3796() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6307_shchina4_3797() { var g = Game.Solo(seed: 33342); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6308_shchina4_3798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33443 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6309_shchina4_3799() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6310_shchina4_3800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6311_shchina4_3801() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6312_shchina4_3802() { var g = Game.Solo(seed: 33347); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6313_shchina4_3803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33448 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6314_shchina4_3804() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6315_shchina4_3805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6316_shchina4_3806() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6317_shchina4_3807() { var g = Game.Solo(seed: 33352); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6318_shchina4_3808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33453 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6319_shchina4_3809() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6320_shchina4_3810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6321_shchina4_3811() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6322_shchina4_3812() { var g = Game.Solo(seed: 33357); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6323_shchina4_3813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33458 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6324_shchina4_3814() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6325_shchina4_3815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6326_shchina4_3816() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6327_shchina4_3817() { var g = Game.Solo(seed: 33362); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6328_shchina4_3818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33463 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6329_shchina4_3819() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6330_shchina4_3820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6331_shchina4_3821() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6332_shchina4_3822() { var g = Game.Solo(seed: 33367); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6333_shchina4_3823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33468 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6334_shchina4_3824() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6335_shchina4_3825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6336_shchina4_3826() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6337_shchina4_3827() { var g = Game.Solo(seed: 33372); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6338_shchina4_3828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33473 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6339_shchina4_3829() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6340_shchina4_3830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6341_shchina4_3831() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6342_shchina4_3832() { var g = Game.Solo(seed: 33377); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6343_shchina4_3833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33478 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6344_shchina4_3834() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6345_shchina4_3835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6346_shchina4_3836() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6347_shchina4_3837() { var g = Game.Solo(seed: 33382); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6348_shchina4_3838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33483 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6349_shchina4_3839() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6350_shchina4_3840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q6351_shchina4_3841() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q6352_shchina4_3842() { var g = Game.Solo(seed: 33387); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q6353_shchina4_3843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 33488 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q6354_shchina4_3844() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q6355_shchina4_3845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 77); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
}
