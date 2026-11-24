using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6216–6285: batch 94 regulator/bubble data bar coda (schema 29).</summary>
public class HatchRegulatorBubbleBarLaterTests5
{
    [Fact] public void Q6216_regbubble4_3566() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6217_regbubble4_3567() { var g = Game.Solo(seed: 32342); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6218_regbubble4_3568() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 32443 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6219_regbubble4_3569() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6220_regbubble4_3570() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6221_regbubble4_3571() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6222_regbubble4_3572() { var g = Game.Solo(seed: 32347); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6223_regbubble4_3573() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 32448 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6224_regbubble4_3574() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6225_regbubble4_3575() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6226_regbubble4_3576() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6227_regbubble4_3577() { var g = Game.Solo(seed: 32352); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6228_regbubble4_3578() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 32453 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6229_regbubble4_3579() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6230_regbubble4_3580() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6231_regbubble4_3581() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6232_regbubble4_3582() { var g = Game.Solo(seed: 32357); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6233_regbubble4_3583() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 32458 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6234_regbubble4_3584() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6235_regbubble4_3585() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6236_regbubble4_3586() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6237_regbubble4_3587() { var g = Game.Solo(seed: 27302); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6238_regbubble4_3588() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27404 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6239_regbubble4_3589() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6240_regbubble4_3590() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6241_regbubble4_3591() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6242_regbubble4_3592() { var g = Game.Solo(seed: 27307); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6243_regbubble4_3593() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27409 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6244_regbubble4_3594() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6245_regbubble4_3595() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6246_regbubble4_3596() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6247_regbubble4_3597() { var g = Game.Solo(seed: 27312); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6248_regbubble4_3598() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27414 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6249_regbubble4_3599() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6250_regbubble4_3600() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6251_regbubble4_3601() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6252_regbubble4_3602() { var g = Game.Solo(seed: 27317); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6253_regbubble4_3603() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27419 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6254_regbubble4_3604() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6255_regbubble4_3605() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6256_regbubble4_3606() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6257_regbubble4_3607() { var g = Game.Solo(seed: 27322); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6258_regbubble4_3608() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27424 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6259_regbubble4_3609() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6260_regbubble4_3610() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6261_regbubble4_3611() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6262_regbubble4_3612() { var g = Game.Solo(seed: 27327); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6263_regbubble4_3613() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27429 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6264_regbubble4_3614() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6265_regbubble4_3615() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6266_regbubble4_3616() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6267_regbubble4_3617() { var g = Game.Solo(seed: 27332); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6268_regbubble4_3618() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27434 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6269_regbubble4_3619() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6270_regbubble4_3620() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6271_regbubble4_3621() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6272_regbubble4_3622() { var g = Game.Solo(seed: 27337); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6273_regbubble4_3623() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27439 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6274_regbubble4_3624() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6275_regbubble4_3625() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6276_regbubble4_3626() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6277_regbubble4_3627() { var g = Game.Solo(seed: 27342); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6278_regbubble4_3628() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27444 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6279_regbubble4_3629() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6280_regbubble4_3630() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q6281_regbubble4_3631() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q6282_regbubble4_3632() { var g = Game.Solo(seed: 27347); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q6283_regbubble4_3633() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 27449 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q6284_regbubble4_3634() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q6285_regbubble4_3635() { GameConfig c = TestData.Config(); var g = Game.New(seed: 76); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
}
