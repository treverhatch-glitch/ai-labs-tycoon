using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2726–Q2795: batch 44 shady/china data bar coda (schema 34).</summary>
public class HatchShadyChinaBarLaterTests2
{
    [Fact] public void Q2726_shchina2_2726() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2727_shchina2_2727() { var g = Game.Solo(seed: 6002); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2728_shchina2_2728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6104 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2729_shchina2_2729() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2730_shchina2_2730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2731_shchina2_2731() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2732_shchina2_2732() { var g = Game.Solo(seed: 6007); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2733_shchina2_2733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6109 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2734_shchina2_2734() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("intrusion-for-hire"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2735_shchina2_2735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2736_shchina2_2736() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2737_shchina2_2737() { var g = Game.Solo(seed: 6012); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2738_shchina2_2738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6114 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2739_shchina2_2739() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("liberate-funds"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2740_shchina2_2740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2741_shchina2_2741() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2742_shchina2_2742() { var g = Game.Solo(seed: 6017); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2743_shchina2_2743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6119 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2744_shchina2_2744() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("sabotage-run"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2745_shchina2_2745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2746_shchina2_2746() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2747_shchina2_2747() { var g = Game.Solo(seed: 6022); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2748_shchina2_2748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6124 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2749_shchina2_2749() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2750_shchina2_2750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2751_shchina2_2751() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2752_shchina2_2752() { var g = Game.Solo(seed: 6027); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2753_shchina2_2753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6129 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2754_shchina2_2754() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("intrusion-for-hire"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2755_shchina2_2755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2756_shchina2_2756() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2757_shchina2_2757() { var g = Game.Solo(seed: 6032); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2758_shchina2_2758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6134 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2759_shchina2_2759() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("liberate-funds"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2760_shchina2_2760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2761_shchina2_2761() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2762_shchina2_2762() { var g = Game.Solo(seed: 6037); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2763_shchina2_2763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6139 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2764_shchina2_2764() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("sabotage-run"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2765_shchina2_2765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2766_shchina2_2766() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2767_shchina2_2767() { var g = Game.Solo(seed: 6042); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2768_shchina2_2768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6144 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2769_shchina2_2769() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2770_shchina2_2770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2771_shchina2_2771() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2772_shchina2_2772() { var g = Game.Solo(seed: 6047); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2773_shchina2_2773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6149 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2774_shchina2_2774() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("intrusion-for-hire"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2775_shchina2_2775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2776_shchina2_2776() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2777_shchina2_2777() { var g = Game.Solo(seed: 6052); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2778_shchina2_2778() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6154 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2779_shchina2_2779() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("liberate-funds"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2780_shchina2_2780() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2781_shchina2_2781() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2782_shchina2_2782() { var g = Game.Solo(seed: 6057); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2783_shchina2_2783() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6159 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2784_shchina2_2784() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("sabotage-run"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2785_shchina2_2785() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2786_shchina2_2786() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2787_shchina2_2787() { var g = Game.Solo(seed: 6062); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2788_shchina2_2788() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6164 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2789_shchina2_2789() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("hack-government"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2790_shchina2_2790() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
    [Fact] public void Q2791_shchina2_2791() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); }
    [Fact] public void Q2792_shchina2_2792() { var g = Game.Solo(seed: 6067); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Shady.Cards.Count >= 4); }
    [Fact] public void Q2793_shchina2_2793() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6169 })).Sim); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); }
    [Fact] public void Q2794_shchina2_2794() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card("intrusion-for-hire"); Assert.True(card.Weeks > 0); Assert.True(c.World.ChinaDeals.AcceptAttention > 0); }
    [Fact] public void Q2795_shchina2_2795() { GameConfig c = TestData.Config(); var g = Game.New(seed: 44); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.NotNull(c.World.Bloc(c.World.ChinaDeals.BlocId)); }
}
