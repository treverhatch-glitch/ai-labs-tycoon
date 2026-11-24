using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6426–6495: batch 97 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests5
{
    [Fact] public void Q5096_rivalecon2_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5097_rivalecon2_3707() { var g = Game.Solo(seed: 35282); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5098_rivalecon2_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35383 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5099_rivalecon2_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5100_rivalecon2_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5101_rivalecon2_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5102_rivalecon2_3712() { var g = Game.Solo(seed: 35287); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5103_rivalecon2_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35388 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5104_rivalecon2_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5105_rivalecon2_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5106_rivalecon2_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5107_rivalecon2_3717() { var g = Game.Solo(seed: 35292); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5108_rivalecon2_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35393 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5109_rivalecon2_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5110_rivalecon2_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5111_rivalecon2_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5112_rivalecon2_3722() { var g = Game.Solo(seed: 35297); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5113_rivalecon2_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35398 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5114_rivalecon2_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5115_rivalecon2_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5116_rivalecon2_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5117_rivalecon2_3727() { var g = Game.Solo(seed: 35302); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5118_rivalecon2_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35403 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5119_rivalecon2_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5120_rivalecon2_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5121_rivalecon2_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5122_rivalecon2_3732() { var g = Game.Solo(seed: 35307); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5123_rivalecon2_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35408 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5124_rivalecon2_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5125_rivalecon2_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5126_rivalecon2_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5127_rivalecon2_3737() { var g = Game.Solo(seed: 35312); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5128_rivalecon2_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35413 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5129_rivalecon2_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5130_rivalecon2_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5131_rivalecon2_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5132_rivalecon2_3742() { var g = Game.Solo(seed: 35317); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5133_rivalecon2_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35418 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5134_rivalecon2_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5135_rivalecon2_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5136_rivalecon2_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5137_rivalecon2_3747() { var g = Game.Solo(seed: 35322); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5138_rivalecon2_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35423 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5139_rivalecon2_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5140_rivalecon2_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5141_rivalecon2_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5142_rivalecon2_3752() { var g = Game.Solo(seed: 35327); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5143_rivalecon2_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35428 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5144_rivalecon2_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5145_rivalecon2_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5146_rivalecon2_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5147_rivalecon2_3757() { var g = Game.Solo(seed: 35332); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5148_rivalecon2_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35433 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5149_rivalecon2_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5150_rivalecon2_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5151_rivalecon2_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5152_rivalecon2_3762() { var g = Game.Solo(seed: 35337); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5153_rivalecon2_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35438 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5154_rivalecon2_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5155_rivalecon2_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5156_rivalecon2_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5157_rivalecon2_3767() { var g = Game.Solo(seed: 35342); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5158_rivalecon2_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35443 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5159_rivalecon2_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5160_rivalecon2_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q5161_rivalecon2_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q5162_rivalecon2_3772() { var g = Game.Solo(seed: 35347); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q5163_rivalecon2_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 35448 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q5164_rivalecon2_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q5165_rivalecon2_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 79); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
