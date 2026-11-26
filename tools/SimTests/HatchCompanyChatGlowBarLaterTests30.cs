using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q16376+: batch 369 rivals bible tail (chatglow).</summary>
public class HatchCompanyChatGlowBarLaterTests30
{
    [Fact] public void Q16376_chatglow369_5000() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16377_chatglow369_5001() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16378_chatglow369_5002() { var g = Game.Solo(seed: 394102); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16379_chatglow369_5003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394103 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16380_chatglow369_5004() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16381_chatglow369_5005() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16382_chatglow369_5006() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16383_chatglow369_5007() { var g = Game.Solo(seed: 394107); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16384_chatglow369_5008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394108 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16385_chatglow369_5009() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16386_chatglow369_5010() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16387_chatglow369_5011() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16388_chatglow369_5012() { var g = Game.Solo(seed: 394112); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16389_chatglow369_5013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394113 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16390_chatglow369_5014() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16391_chatglow369_5015() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16392_chatglow369_5016() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16393_chatglow369_5017() { var g = Game.Solo(seed: 394117); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16394_chatglow369_5018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394118 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16395_chatglow369_5019() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16396_chatglow369_5020() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16397_chatglow369_5021() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16398_chatglow369_5022() { var g = Game.Solo(seed: 394122); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16399_chatglow369_5023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394123 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16400_chatglow369_5024() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16401_chatglow369_5025() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16402_chatglow369_5026() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16403_chatglow369_5027() { var g = Game.Solo(seed: 394127); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16404_chatglow369_5028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394128 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16405_chatglow369_5029() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16406_chatglow369_5030() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16407_chatglow369_5031() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16408_chatglow369_5032() { var g = Game.Solo(seed: 394132); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16409_chatglow369_5033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394133 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16410_chatglow369_5034() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16411_chatglow369_5035() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16412_chatglow369_5036() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16413_chatglow369_5037() { var g = Game.Solo(seed: 394137); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16414_chatglow369_5038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394138 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16415_chatglow369_5039() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16416_chatglow369_5040() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16417_chatglow369_5041() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16418_chatglow369_5042() { var g = Game.Solo(seed: 394142); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16419_chatglow369_5043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394143 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16420_chatglow369_5044() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16421_chatglow369_5045() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16422_chatglow369_5046() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16423_chatglow369_5047() { var g = Game.Solo(seed: 394147); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16424_chatglow369_5048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394148 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16425_chatglow369_5049() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16426_chatglow369_5050() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16427_chatglow369_5051() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16428_chatglow369_5052() { var g = Game.Solo(seed: 394152); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16429_chatglow369_5053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394153 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16430_chatglow369_5054() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16431_chatglow369_5055() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16432_chatglow369_5056() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16433_chatglow369_5057() { var g = Game.Solo(seed: 394157); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16434_chatglow369_5058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394158 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16435_chatglow369_5059() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16436_chatglow369_5060() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16437_chatglow369_5061() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16438_chatglow369_5062() { var g = Game.Solo(seed: 394162); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16439_chatglow369_5063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394163 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16440_chatglow369_5064() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
    [Fact] public void Q16441_chatglow369_5065() { GameConfig c = TestData.Config(); Assert.True(c.Chat.Lines.Count >= 1); Assert.True(c.Chat.DefaultLines > 0); }
    [Fact] public void Q16442_chatglow369_5066() { GameConfig c = TestData.Config(); Assert.True(c.Glow.Stages.Count >= 1); Assert.True(c.Glow.FullAtCapability > 0); }
    [Fact] public void Q16443_chatglow369_5067() { var g = Game.Solo(seed: 394167); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Chat.Lines.Count >= 1); }
    [Fact] public void Q16444_chatglow369_5068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 394168 })).Sim); Assert.True(c.Glow.Stages.Count >= 1); }
    [Fact] public void Q16445_chatglow369_5069() { GameConfig c = TestData.Config(); var g = Game.New(seed: 247200); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Glow.Stages.Count >= 1); }
}
