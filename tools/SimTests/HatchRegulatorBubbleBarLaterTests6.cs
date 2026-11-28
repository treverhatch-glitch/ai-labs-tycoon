using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6216–6285: batch 118 regulator/bubble data bar coda (schema 29).</summary>
public class HatchRegulatorBubbleBarLaterTests6
{
    [Fact] public void Q7896_regbubble6_3566() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7897_regbubble6_3567() { var g = Game.Solo(seed: 55822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7898_regbubble6_3568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 55923 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7899_regbubble6_3569() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7900_regbubble6_3570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7901_regbubble6_3571() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7902_regbubble6_3572() { var g = Game.Solo(seed: 55827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7903_regbubble6_3573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 55928 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7904_regbubble6_3574() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7905_regbubble6_3575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7906_regbubble6_3576() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7907_regbubble6_3577() { var g = Game.Solo(seed: 55832); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7908_regbubble6_3578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 55933 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7909_regbubble6_3579() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7910_regbubble6_3580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7911_regbubble6_3581() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7912_regbubble6_3582() { var g = Game.Solo(seed: 55837); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7913_regbubble6_3583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 55938 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7914_regbubble6_3584() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7915_regbubble6_3585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7916_regbubble6_3586() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7917_regbubble6_3587() { var g = Game.Solo(seed: 50782); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7918_regbubble6_3588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50884 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7919_regbubble6_3589() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7920_regbubble6_3590() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7921_regbubble6_3591() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7922_regbubble6_3592() { var g = Game.Solo(seed: 50787); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7923_regbubble6_3593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50889 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7924_regbubble6_3594() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7925_regbubble6_3595() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7926_regbubble6_3596() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7927_regbubble6_3597() { var g = Game.Solo(seed: 50792); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7928_regbubble6_3598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50894 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7929_regbubble6_3599() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7930_regbubble6_3600() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7931_regbubble6_3601() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7932_regbubble6_3602() { var g = Game.Solo(seed: 50797); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7933_regbubble6_3603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50899 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7934_regbubble6_3604() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7935_regbubble6_3605() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7936_regbubble6_3606() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7937_regbubble6_3607() { var g = Game.Solo(seed: 50802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7938_regbubble6_3608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50904 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7939_regbubble6_3609() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7940_regbubble6_3610() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7941_regbubble6_3611() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7942_regbubble6_3612() { var g = Game.Solo(seed: 50807); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7943_regbubble6_3613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50909 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7944_regbubble6_3614() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7945_regbubble6_3615() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7946_regbubble6_3616() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7947_regbubble6_3617() { var g = Game.Solo(seed: 50812); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7948_regbubble6_3618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50914 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7949_regbubble6_3619() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7950_regbubble6_3620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7951_regbubble6_3621() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7952_regbubble6_3622() { var g = Game.Solo(seed: 50817); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7953_regbubble6_3623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50919 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7954_regbubble6_3624() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7955_regbubble6_3625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7956_regbubble6_3626() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7957_regbubble6_3627() { var g = Game.Solo(seed: 50822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7958_regbubble6_3628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50924 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7959_regbubble6_3629() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7960_regbubble6_3630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q7961_regbubble6_3631() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q7962_regbubble6_3632() { var g = Game.Solo(seed: 50827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q7963_regbubble6_3633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 50929 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q7964_regbubble6_3634() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q7965_regbubble6_3635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 100); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
}
