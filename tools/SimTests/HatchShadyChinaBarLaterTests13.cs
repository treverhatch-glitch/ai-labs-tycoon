using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13586–13655: batch 259 shady/china data bar coda (schema 29).</summary>
public class HatchShadyChinaBarLaterTests13
{
    [Fact] public void Q13586_shchina13_3986() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13587_shchina13_3987() { var g = Game.Solo(seed: 184642); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13588_shchina13_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184743 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13589_shchina13_3989() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13590_shchina13_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13591_shchina13_3991() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13592_shchina13_3992() { var g = Game.Solo(seed: 184647); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13593_shchina13_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184748 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13594_shchina13_3994() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13595_shchina13_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13596_shchina13_3996() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13597_shchina13_3997() { var g = Game.Solo(seed: 184652); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13598_shchina13_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184753 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13599_shchina13_3999() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13600_shchina13_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13601_shchina13_4001() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13602_shchina13_4002() { var g = Game.Solo(seed: 184657); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13603_shchina13_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184758 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13604_shchina13_4004() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13605_shchina13_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13606_shchina13_4006() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13607_shchina13_4007() { var g = Game.Solo(seed: 184662); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13608_shchina13_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184763 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13609_shchina13_4009() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13610_shchina13_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13611_shchina13_4011() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13612_shchina13_4012() { var g = Game.Solo(seed: 184667); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13613_shchina13_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184768 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13614_shchina13_4014() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13615_shchina13_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13616_shchina13_4016() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13617_shchina13_4017() { var g = Game.Solo(seed: 184672); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13618_shchina13_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184773 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13619_shchina13_4019() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13620_shchina13_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13621_shchina13_4021() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13622_shchina13_4022() { var g = Game.Solo(seed: 184677); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13623_shchina13_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184778 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13624_shchina13_4024() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13625_shchina13_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13626_shchina13_4026() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13627_shchina13_4027() { var g = Game.Solo(seed: 184682); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13628_shchina13_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184783 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13629_shchina13_4029() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13630_shchina13_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13631_shchina13_4031() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13632_shchina13_4032() { var g = Game.Solo(seed: 184687); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13633_shchina13_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184788 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13634_shchina13_4034() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13635_shchina13_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13636_shchina13_4036() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13637_shchina13_4037() { var g = Game.Solo(seed: 184692); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13638_shchina13_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184793 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13639_shchina13_4039() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13640_shchina13_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13641_shchina13_4041() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13642_shchina13_4042() { var g = Game.Solo(seed: 184697); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13643_shchina13_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184798 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13644_shchina13_4044() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13645_shchina13_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13646_shchina13_4046() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13647_shchina13_4047() { var g = Game.Solo(seed: 184702); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13648_shchina13_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184803 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13649_shchina13_4049() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13650_shchina13_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q13651_shchina13_4051() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q13652_shchina13_4052() { var g = Game.Solo(seed: 184707); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q13653_shchina13_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 184808 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q13654_shchina13_4054() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q13655_shchina13_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 53041); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
}
