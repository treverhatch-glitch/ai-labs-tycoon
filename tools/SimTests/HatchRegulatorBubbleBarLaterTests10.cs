using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6216–6285: batch 198 regulator/bubble data bar coda (schema 29).</summary>
public class HatchRegulatorBubbleBarLaterTests10
{
    [Fact] public void Q13306_regbubble10_3566() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13307_regbubble10_3567() { var g = Game.Solo(seed: 130962); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13308_regbubble10_3568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131063 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13309_regbubble10_3569() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13310_regbubble10_3570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13311_regbubble10_3571() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13312_regbubble10_3572() { var g = Game.Solo(seed: 130967); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13313_regbubble10_3573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131068 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13314_regbubble10_3574() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13315_regbubble10_3575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13316_regbubble10_3576() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13317_regbubble10_3577() { var g = Game.Solo(seed: 130972); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13318_regbubble10_3578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131073 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13319_regbubble10_3579() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13320_regbubble10_3580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13321_regbubble10_3581() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13322_regbubble10_3582() { var g = Game.Solo(seed: 130977); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13323_regbubble10_3583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 131078 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13324_regbubble10_3584() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13325_regbubble10_3585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13326_regbubble10_3586() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13327_regbubble10_3587() { var g = Game.Solo(seed: 125922); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13328_regbubble10_3588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126024 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13329_regbubble10_3589() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13330_regbubble10_3590() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13331_regbubble10_3591() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13332_regbubble10_3592() { var g = Game.Solo(seed: 125927); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13333_regbubble10_3593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126029 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13334_regbubble10_3594() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13335_regbubble10_3595() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13336_regbubble10_3596() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13337_regbubble10_3597() { var g = Game.Solo(seed: 125932); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13338_regbubble10_3598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126034 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13339_regbubble10_3599() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13340_regbubble10_3600() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13341_regbubble10_3601() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13342_regbubble10_3602() { var g = Game.Solo(seed: 125937); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13343_regbubble10_3603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126039 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13344_regbubble10_3604() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13345_regbubble10_3605() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13346_regbubble10_3606() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13347_regbubble10_3607() { var g = Game.Solo(seed: 125942); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13348_regbubble10_3608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126044 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13349_regbubble10_3609() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13350_regbubble10_3610() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13351_regbubble10_3611() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13352_regbubble10_3612() { var g = Game.Solo(seed: 125947); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13353_regbubble10_3613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126049 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13354_regbubble10_3614() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13355_regbubble10_3615() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13356_regbubble10_3616() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13357_regbubble10_3617() { var g = Game.Solo(seed: 125952); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13358_regbubble10_3618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126054 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13359_regbubble10_3619() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13360_regbubble10_3620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13361_regbubble10_3621() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13362_regbubble10_3622() { var g = Game.Solo(seed: 125957); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13363_regbubble10_3623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126059 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13364_regbubble10_3624() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13365_regbubble10_3625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13366_regbubble10_3626() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13367_regbubble10_3627() { var g = Game.Solo(seed: 125962); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13368_regbubble10_3628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126064 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13369_regbubble10_3629() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13370_regbubble10_3630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13371_regbubble10_3631() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13372_regbubble10_3632() { var g = Game.Solo(seed: 125967); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13373_regbubble10_3633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 126069 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13374_regbubble10_3634() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13375_regbubble10_3635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 180); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
}
