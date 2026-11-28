using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6426–6495: batch 121 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests6
{
    [Fact] public void Q8106_rivalecon6_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8107_rivalecon6_3707() { var g = Game.Solo(seed: 58762); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8108_rivalecon6_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58863 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8109_rivalecon6_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8110_rivalecon6_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8111_rivalecon6_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8112_rivalecon6_3712() { var g = Game.Solo(seed: 58767); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8113_rivalecon6_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58868 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8114_rivalecon6_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8115_rivalecon6_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8116_rivalecon6_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8117_rivalecon6_3717() { var g = Game.Solo(seed: 58772); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8118_rivalecon6_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58873 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8119_rivalecon6_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8120_rivalecon6_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8121_rivalecon6_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8122_rivalecon6_3722() { var g = Game.Solo(seed: 58777); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8123_rivalecon6_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58878 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8124_rivalecon6_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8125_rivalecon6_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8126_rivalecon6_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8127_rivalecon6_3727() { var g = Game.Solo(seed: 58782); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8128_rivalecon6_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58883 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8129_rivalecon6_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8130_rivalecon6_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8131_rivalecon6_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8132_rivalecon6_3732() { var g = Game.Solo(seed: 58787); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8133_rivalecon6_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58888 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8134_rivalecon6_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8135_rivalecon6_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8136_rivalecon6_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8137_rivalecon6_3737() { var g = Game.Solo(seed: 58792); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8138_rivalecon6_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58893 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8139_rivalecon6_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8140_rivalecon6_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8141_rivalecon6_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8142_rivalecon6_3742() { var g = Game.Solo(seed: 58797); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8143_rivalecon6_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58898 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8144_rivalecon6_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8145_rivalecon6_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8146_rivalecon6_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8147_rivalecon6_3747() { var g = Game.Solo(seed: 58802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8148_rivalecon6_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58903 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8149_rivalecon6_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8150_rivalecon6_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8151_rivalecon6_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8152_rivalecon6_3752() { var g = Game.Solo(seed: 58807); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8153_rivalecon6_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58908 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8154_rivalecon6_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8155_rivalecon6_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8156_rivalecon6_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8157_rivalecon6_3757() { var g = Game.Solo(seed: 58812); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8158_rivalecon6_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58913 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8159_rivalecon6_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8160_rivalecon6_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8161_rivalecon6_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8162_rivalecon6_3762() { var g = Game.Solo(seed: 58817); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8163_rivalecon6_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58918 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8164_rivalecon6_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8165_rivalecon6_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8166_rivalecon6_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8167_rivalecon6_3767() { var g = Game.Solo(seed: 58822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8168_rivalecon6_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58923 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8169_rivalecon6_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8170_rivalecon6_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q8171_rivalecon6_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q8172_rivalecon6_3772() { var g = Game.Solo(seed: 58827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q8173_rivalecon6_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 58928 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q8174_rivalecon6_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q8175_rivalecon6_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 103); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
