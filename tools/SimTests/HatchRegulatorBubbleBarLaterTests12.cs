using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q13446–13515: batch 238 regulator/bubble data bar coda (schema 29).</summary>
public class HatchRegulatorBubbleBarLaterTests12
{
    [Fact] public void Q13446_regbubble12_3706() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13447_regbubble12_3707() { var g = Game.Solo(seed: 166162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13448_regbubble12_3708() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 166263 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13449_regbubble12_3709() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13450_regbubble12_3710() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13451_regbubble12_3711() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13452_regbubble12_3712() { var g = Game.Solo(seed: 166167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13453_regbubble12_3713() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 166268 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13454_regbubble12_3714() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13455_regbubble12_3715() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13456_regbubble12_3716() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13457_regbubble12_3717() { var g = Game.Solo(seed: 166172); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13458_regbubble12_3718() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 166273 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13459_regbubble12_3719() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13460_regbubble12_3720() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13461_regbubble12_3721() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13462_regbubble12_3722() { var g = Game.Solo(seed: 166177); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13463_regbubble12_3723() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 166278 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13464_regbubble12_3724() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13465_regbubble12_3725() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13466_regbubble12_3726() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13467_regbubble12_3727() { var g = Game.Solo(seed: 161122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13468_regbubble12_3728() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161224 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13469_regbubble12_3729() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13470_regbubble12_3730() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13471_regbubble12_3731() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13472_regbubble12_3732() { var g = Game.Solo(seed: 161127); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13473_regbubble12_3733() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161229 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13474_regbubble12_3734() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13475_regbubble12_3735() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13476_regbubble12_3736() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13477_regbubble12_3737() { var g = Game.Solo(seed: 161132); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13478_regbubble12_3738() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161234 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13479_regbubble12_3739() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13480_regbubble12_3740() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13481_regbubble12_3741() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13482_regbubble12_3742() { var g = Game.Solo(seed: 161137); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13483_regbubble12_3743() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161239 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13484_regbubble12_3744() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13485_regbubble12_3745() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13486_regbubble12_3746() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13487_regbubble12_3747() { var g = Game.Solo(seed: 161142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13488_regbubble12_3748() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161244 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13489_regbubble12_3749() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13490_regbubble12_3750() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13491_regbubble12_3751() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13492_regbubble12_3752() { var g = Game.Solo(seed: 161147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13493_regbubble12_3753() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161249 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13494_regbubble12_3754() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13495_regbubble12_3755() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13496_regbubble12_3756() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13497_regbubble12_3757() { var g = Game.Solo(seed: 161152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13498_regbubble12_3758() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161254 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13499_regbubble12_3759() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13500_regbubble12_3760() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13501_regbubble12_3761() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13502_regbubble12_3762() { var g = Game.Solo(seed: 161157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13503_regbubble12_3763() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161259 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13504_regbubble12_3764() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13505_regbubble12_3765() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13506_regbubble12_3766() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13507_regbubble12_3767() { var g = Game.Solo(seed: 161162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13508_regbubble12_3768() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161264 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13509_regbubble12_3769() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13510_regbubble12_3770() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
    [Fact] public void Q13511_regbubble12_3771() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.BubbleScrutinyAtPermille > 0); Assert.True(c.Regulators.MonopolyRegulatorAttention >= 0); }
    [Fact] public void Q13512_regbubble12_3772() { var g = Game.Solo(seed: 161167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q13513_regbubble12_3773() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 161269 })).Sim); Assert.True(c.Bubble.DecayPermillePerMonth > 0); }
    [Fact] public void Q13514_regbubble12_3774() { GameConfig c = TestData.Config(); Assert.True(c.Regulators.ScrutinyAttentionPerMonth > 0); Assert.True(c.Bubble.CrashDurationMonths > 0); }
    [Fact] public void Q13515_regbubble12_3775() { GameConfig c = TestData.Config(); var g = Game.New(seed: 35420); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Bubble.ListedChipBoostPermille > 0); }
}
