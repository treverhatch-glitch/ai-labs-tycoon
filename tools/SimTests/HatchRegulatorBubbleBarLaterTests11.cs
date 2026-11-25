using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13376–13445: batch 218 regulator/bubble data bar coda (schema 29).</summary>
public class HatchRegulatorBubbleBarLaterTests11
{
    [Fact] public void Q13376_regbubble11_3636() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13377_regbubble11_3637() { var g = Game.Solo(seed: 148562); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13378_regbubble11_3638() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 148663 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13379_regbubble11_3639() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13380_regbubble11_3640() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13381_regbubble11_3641() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13382_regbubble11_3642() { var g = Game.Solo(seed: 148567); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13383_regbubble11_3643() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 148668 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13384_regbubble11_3644() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13385_regbubble11_3645() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13386_regbubble11_3646() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13387_regbubble11_3647() { var g = Game.Solo(seed: 148572); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13388_regbubble11_3648() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 148673 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13389_regbubble11_3649() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13390_regbubble11_3650() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13391_regbubble11_3651() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13392_regbubble11_3652() { var g = Game.Solo(seed: 148577); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13393_regbubble11_3653() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 148678 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13394_regbubble11_3654() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13395_regbubble11_3655() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13396_regbubble11_3656() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13397_regbubble11_3657() { var g = Game.Solo(seed: 143522); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13398_regbubble11_3658() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143624 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13399_regbubble11_3659() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13400_regbubble11_3660() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13401_regbubble11_3661() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13402_regbubble11_3662() { var g = Game.Solo(seed: 143527); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13403_regbubble11_3663() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143629 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13404_regbubble11_3664() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13405_regbubble11_3665() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13406_regbubble11_3666() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13407_regbubble11_3667() { var g = Game.Solo(seed: 143532); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13408_regbubble11_3668() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143634 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13409_regbubble11_3669() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13410_regbubble11_3670() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13411_regbubble11_3671() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13412_regbubble11_3672() { var g = Game.Solo(seed: 143537); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13413_regbubble11_3673() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143639 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13414_regbubble11_3674() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13415_regbubble11_3675() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13416_regbubble11_3676() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13417_regbubble11_3677() { var g = Game.Solo(seed: 143542); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13418_regbubble11_3678() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143644 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13419_regbubble11_3679() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13420_regbubble11_3680() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13421_regbubble11_3681() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13422_regbubble11_3682() { var g = Game.Solo(seed: 143547); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13423_regbubble11_3683() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143649 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13424_regbubble11_3684() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13425_regbubble11_3685() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13426_regbubble11_3686() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13427_regbubble11_3687() { var g = Game.Solo(seed: 143552); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13428_regbubble11_3688() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143654 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13429_regbubble11_3689() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13430_regbubble11_3690() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13431_regbubble11_3691() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13432_regbubble11_3692() { var g = Game.Solo(seed: 143557); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13433_regbubble11_3693() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143659 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13434_regbubble11_3694() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13435_regbubble11_3695() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13436_regbubble11_3696() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13437_regbubble11_3697() { var g = Game.Solo(seed: 143562); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13438_regbubble11_3698() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143664 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13439_regbubble11_3699() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13440_regbubble11_3700() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13441_regbubble11_3701() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13442_regbubble11_3702() { var g = Game.Solo(seed: 143567); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13443_regbubble11_3703() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 143669 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13444_regbubble11_3704() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13445_regbubble11_3705() { GameConfig c = TestData.Config(); var g = Game.New(seed: 17800); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
}
