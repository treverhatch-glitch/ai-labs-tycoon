using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6426–6495: batch 161 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests8
{
    [Fact] public void Q10906_rivalecon8_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10907_rivalecon8_3707() { var g = Game.Solo(seed: 97802); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10908_rivalecon8_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97903 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10909_rivalecon8_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10910_rivalecon8_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10911_rivalecon8_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10912_rivalecon8_3712() { var g = Game.Solo(seed: 97807); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10913_rivalecon8_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97908 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10914_rivalecon8_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10915_rivalecon8_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10916_rivalecon8_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10917_rivalecon8_3717() { var g = Game.Solo(seed: 97812); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10918_rivalecon8_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97913 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10919_rivalecon8_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10920_rivalecon8_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10921_rivalecon8_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10922_rivalecon8_3722() { var g = Game.Solo(seed: 97817); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10923_rivalecon8_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97918 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10924_rivalecon8_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10925_rivalecon8_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10926_rivalecon8_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10927_rivalecon8_3727() { var g = Game.Solo(seed: 97822); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10928_rivalecon8_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97923 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10929_rivalecon8_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10930_rivalecon8_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10931_rivalecon8_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10932_rivalecon8_3732() { var g = Game.Solo(seed: 97827); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10933_rivalecon8_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97928 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10934_rivalecon8_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10935_rivalecon8_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10936_rivalecon8_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10937_rivalecon8_3737() { var g = Game.Solo(seed: 97832); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10938_rivalecon8_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97933 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10939_rivalecon8_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10940_rivalecon8_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10941_rivalecon8_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10942_rivalecon8_3742() { var g = Game.Solo(seed: 97837); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10943_rivalecon8_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97938 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10944_rivalecon8_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10945_rivalecon8_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10946_rivalecon8_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10947_rivalecon8_3747() { var g = Game.Solo(seed: 97842); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10948_rivalecon8_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97943 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10949_rivalecon8_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10950_rivalecon8_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10951_rivalecon8_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10952_rivalecon8_3752() { var g = Game.Solo(seed: 97847); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10953_rivalecon8_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97948 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10954_rivalecon8_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10955_rivalecon8_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10956_rivalecon8_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10957_rivalecon8_3757() { var g = Game.Solo(seed: 97852); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10958_rivalecon8_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97953 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10959_rivalecon8_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10960_rivalecon8_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10961_rivalecon8_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10962_rivalecon8_3762() { var g = Game.Solo(seed: 97857); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10963_rivalecon8_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97958 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10964_rivalecon8_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10965_rivalecon8_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10966_rivalecon8_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10967_rivalecon8_3767() { var g = Game.Solo(seed: 97862); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10968_rivalecon8_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97963 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10969_rivalecon8_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10970_rivalecon8_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q10971_rivalecon8_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q10972_rivalecon8_3772() { var g = Game.Solo(seed: 97867); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q10973_rivalecon8_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 97968 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q10974_rivalecon8_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q10975_rivalecon8_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 143); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
