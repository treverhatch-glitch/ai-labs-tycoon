using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q4896–Q4965: batch 75 shady/china data bar coda (schema 29).</summary>
public class HatchShadyChinaBarLaterTests4
{
    [Fact] public void Q4896_shchina4_3776() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4897_shchina4_3777() { var g = Game.Solo(seed: 13622); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4898_shchina4_3778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13723 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4899_shchina4_3779() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4900_shchina4_3780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4901_shchina4_3781() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4902_shchina4_3782() { var g = Game.Solo(seed: 13627); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4903_shchina4_3783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13728 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4904_shchina4_3784() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4905_shchina4_3785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4906_shchina4_3786() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4907_shchina4_3787() { var g = Game.Solo(seed: 13632); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4908_shchina4_3788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13733 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4909_shchina4_3789() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4910_shchina4_3790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4911_shchina4_3791() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4912_shchina4_3792() { var g = Game.Solo(seed: 13637); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4913_shchina4_3793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13738 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4914_shchina4_3794() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4915_shchina4_3795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4916_shchina4_3796() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4917_shchina4_3797() { var g = Game.Solo(seed: 13642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4918_shchina4_3798() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13743 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4919_shchina4_3799() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4920_shchina4_3800() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4921_shchina4_3801() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4922_shchina4_3802() { var g = Game.Solo(seed: 13647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4923_shchina4_3803() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13748 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4924_shchina4_3804() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4925_shchina4_3805() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4926_shchina4_3806() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4927_shchina4_3807() { var g = Game.Solo(seed: 13652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4928_shchina4_3808() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13753 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4929_shchina4_3809() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4930_shchina4_3810() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4931_shchina4_3811() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4932_shchina4_3812() { var g = Game.Solo(seed: 13657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4933_shchina4_3813() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13758 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4934_shchina4_3814() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4935_shchina4_3815() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4936_shchina4_3816() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4937_shchina4_3817() { var g = Game.Solo(seed: 13662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4938_shchina4_3818() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13763 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4939_shchina4_3819() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4940_shchina4_3820() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4941_shchina4_3821() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4942_shchina4_3822() { var g = Game.Solo(seed: 13667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4943_shchina4_3823() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13768 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4944_shchina4_3824() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4945_shchina4_3825() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4946_shchina4_3826() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4947_shchina4_3827() { var g = Game.Solo(seed: 13672); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4948_shchina4_3828() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13773 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4949_shchina4_3829() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4950_shchina4_3830() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4951_shchina4_3831() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4952_shchina4_3832() { var g = Game.Solo(seed: 13677); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4953_shchina4_3833() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13778 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4954_shchina4_3834() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4955_shchina4_3835() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4956_shchina4_3836() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4957_shchina4_3837() { var g = Game.Solo(seed: 13682); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4958_shchina4_3838() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13783 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4959_shchina4_3839() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4960_shchina4_3840() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q4961_shchina4_3841() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q4962_shchina4_3842() { var g = Game.Solo(seed: 13687); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q4963_shchina4_3843() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 13788 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q4964_shchina4_3844() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q4965_shchina4_3845() { GameConfig c = TestData.Config(); var g = Game.New(seed: 58); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
}
