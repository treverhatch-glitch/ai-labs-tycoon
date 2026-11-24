using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q2656–Q2725: batch 43 world-map economy data bar (schema 31).</summary>
public class HatchWorldMapEconomyBarLaterTests
{
    [Fact] public void Q2656_world_2656() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2657_world_2657() { var g = Game.Solo(seed: 5802); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2658_world_2658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5904 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2659_world_2659() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2660_world_2660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2661_world_2661() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2662_world_2662() { var g = Game.Solo(seed: 5807); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2663_world_2663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5909 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2664_world_2664() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2665_world_2665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2666_world_2666() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2667_world_2667() { var g = Game.Solo(seed: 5812); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2668_world_2668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5914 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2669_world_2669() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2670_world_2670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2671_world_2671() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2672_world_2672() { var g = Game.Solo(seed: 5817); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2673_world_2673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5919 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2674_world_2674() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2675_world_2675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2676_world_2676() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2677_world_2677() { var g = Game.Solo(seed: 5822); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2678_world_2678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5924 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2679_world_2679() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2680_world_2680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2681_world_2681() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2682_world_2682() { var g = Game.Solo(seed: 5827); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2683_world_2683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5929 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2684_world_2684() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2685_world_2685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2686_world_2686() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2687_world_2687() { var g = Game.Solo(seed: 5832); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2688_world_2688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5934 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2689_world_2689() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2690_world_2690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2691_world_2691() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2692_world_2692() { var g = Game.Solo(seed: 5837); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2693_world_2693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5939 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2694_world_2694() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2695_world_2695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2696_world_2696() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2697_world_2697() { var g = Game.Solo(seed: 5842); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2698_world_2698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5944 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2699_world_2699() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2700_world_2700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2701_world_2701() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2702_world_2702() { var g = Game.Solo(seed: 5847); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2703_world_2703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5949 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2704_world_2704() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2705_world_2705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2706_world_2706() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2707_world_2707() { var g = Game.Solo(seed: 5852); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2708_world_2708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5954 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2709_world_2709() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2710_world_2710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2711_world_2711() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2712_world_2712() { var g = Game.Solo(seed: 5857); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2713_world_2713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5959 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2714_world_2714() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2715_world_2715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2716_world_2716() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2717_world_2717() { var g = Game.Solo(seed: 5862); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2718_world_2718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5964 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2719_world_2719() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2720_world_2720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
    [Fact] public void Q2721_world_2721() { GameConfig c = TestData.Config(); Assert.True(c.World.Blocs.Count >= 9); Assert.True(c.World.Sites.Count >= 10); }
    [Fact] public void Q2722_world_2722() { var g = Game.Solo(seed: 5867); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Bloc(g.C.World.Blocs[0].Id)); }
    [Fact] public void Q2723_world_2723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 5969 })).Sim); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q2724_world_2724() { GameConfig c = TestData.Config(); Assert.True(c.World.Heatwave.ThrottlePermille > 0); Assert.True(c.PlayerStart.HomeBlocId.Length > 0); }
    [Fact] public void Q2725_world_2725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 43); Assert.Equal(31, g.State.Schema); Assert.NotNull(g.C.World.Site(g.C.World.Sites[0].Id)); }
}
