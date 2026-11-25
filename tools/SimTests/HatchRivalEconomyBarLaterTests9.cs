using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6426–6495: batch 181 rival economy data bar coda (schema 29).</summary>
public class HatchRivalEconomyBarLaterTests9
{
    [Fact] public void Q12166_rivalecon9_3706() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12167_rivalecon9_3707() { var g = Game.Solo(seed: 116002); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12168_rivalecon9_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116103 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12169_rivalecon9_3709() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12170_rivalecon9_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12171_rivalecon9_3711() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12172_rivalecon9_3712() { var g = Game.Solo(seed: 116007); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12173_rivalecon9_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116108 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12174_rivalecon9_3714() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12175_rivalecon9_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12176_rivalecon9_3716() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12177_rivalecon9_3717() { var g = Game.Solo(seed: 116012); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12178_rivalecon9_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116113 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12179_rivalecon9_3719() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12180_rivalecon9_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12181_rivalecon9_3721() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12182_rivalecon9_3722() { var g = Game.Solo(seed: 116017); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12183_rivalecon9_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116118 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12184_rivalecon9_3724() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12185_rivalecon9_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12186_rivalecon9_3726() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12187_rivalecon9_3727() { var g = Game.Solo(seed: 116022); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12188_rivalecon9_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116123 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12189_rivalecon9_3729() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12190_rivalecon9_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12191_rivalecon9_3731() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12192_rivalecon9_3732() { var g = Game.Solo(seed: 116027); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12193_rivalecon9_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116128 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12194_rivalecon9_3734() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12195_rivalecon9_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12196_rivalecon9_3736() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12197_rivalecon9_3737() { var g = Game.Solo(seed: 116032); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12198_rivalecon9_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116133 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12199_rivalecon9_3739() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12200_rivalecon9_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12201_rivalecon9_3741() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12202_rivalecon9_3742() { var g = Game.Solo(seed: 116037); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12203_rivalecon9_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116138 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12204_rivalecon9_3744() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12205_rivalecon9_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12206_rivalecon9_3746() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12207_rivalecon9_3747() { var g = Game.Solo(seed: 116042); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12208_rivalecon9_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116143 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12209_rivalecon9_3749() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12210_rivalecon9_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12211_rivalecon9_3751() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12212_rivalecon9_3752() { var g = Game.Solo(seed: 116047); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12213_rivalecon9_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116148 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12214_rivalecon9_3754() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12215_rivalecon9_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12216_rivalecon9_3756() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12217_rivalecon9_3757() { var g = Game.Solo(seed: 116052); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12218_rivalecon9_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116153 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12219_rivalecon9_3759() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12220_rivalecon9_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12221_rivalecon9_3761() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12222_rivalecon9_3762() { var g = Game.Solo(seed: 116057); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12223_rivalecon9_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116158 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12224_rivalecon9_3764() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12225_rivalecon9_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12226_rivalecon9_3766() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12227_rivalecon9_3767() { var g = Game.Solo(seed: 116062); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12228_rivalecon9_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116163 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12229_rivalecon9_3769() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12230_rivalecon9_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
    [Fact] public void Q12231_rivalecon9_3771() { GameConfig c = TestData.Config(); Assert.True(c.Acquisitions.RivalRollupChancePermille > 0); Assert.True(c.Acquisitions.PlannerAcquireChancePermille > 0); }
    [Fact] public void Q12232_rivalecon9_3772() { var g = Game.Solo(seed: 116067); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Rivals.Count >= 3); }
    [Fact] public void Q12233_rivalecon9_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 116168 })).Sim); Assert.True(c.Rivals[0].Plan.WealthGrowthPermillePerMillion >= 0); }
    [Fact] public void Q12234_rivalecon9_3774() { GameConfig c = TestData.Config(); Assert.True(c.Chips.DistressListingPricePermille > 0); Assert.All(c.Rivals, r => Assert.NotNull(r.Plan)); }
    [Fact] public void Q12235_rivalecon9_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 163); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Acquisitions.RivalRollupChancePermille > 0); }
}
