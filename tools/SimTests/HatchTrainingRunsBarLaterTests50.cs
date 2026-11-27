using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>17496+: run389 hatch bar.</summary>
public class HatchTrainingRunsBarLaterTests50
{
    [Fact] public void Q17496_run389_5000() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17497_run389_5001() { var g = Game.Solo(seed: 396481); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17498_run389_5002() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396482 })).Sim); }
    [Fact] public void Q17499_run389_5003() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17500_run389_5004() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17501_run389_5005() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17502_run389_5006() { var g = Game.Solo(seed: 396486); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17503_run389_5007() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396487 })).Sim); }
    [Fact] public void Q17504_run389_5008() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17505_run389_5009() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17506_run389_5010() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17507_run389_5011() { var g = Game.Solo(seed: 396491); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17508_run389_5012() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396492 })).Sim); }
    [Fact] public void Q17509_run389_5013() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17510_run389_5014() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17511_run389_5015() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17512_run389_5016() { var g = Game.Solo(seed: 396496); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17513_run389_5017() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396497 })).Sim); }
    [Fact] public void Q17514_run389_5018() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17515_run389_5019() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17516_run389_5020() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17517_run389_5021() { var g = Game.Solo(seed: 396501); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17518_run389_5022() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396502 })).Sim); }
    [Fact] public void Q17519_run389_5023() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17520_run389_5024() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17521_run389_5025() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17522_run389_5026() { var g = Game.Solo(seed: 396506); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17523_run389_5027() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396507 })).Sim); }
    [Fact] public void Q17524_run389_5028() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17525_run389_5029() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17526_run389_5030() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17527_run389_5031() { var g = Game.Solo(seed: 396511); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17528_run389_5032() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396512 })).Sim); }
    [Fact] public void Q17529_run389_5033() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17530_run389_5034() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17531_run389_5035() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17532_run389_5036() { var g = Game.Solo(seed: 396516); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17533_run389_5037() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396517 })).Sim); }
    [Fact] public void Q17534_run389_5038() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17535_run389_5039() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17536_run389_5040() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17537_run389_5041() { var g = Game.Solo(seed: 396521); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17538_run389_5042() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396522 })).Sim); }
    [Fact] public void Q17539_run389_5043() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17540_run389_5044() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17541_run389_5045() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17542_run389_5046() { var g = Game.Solo(seed: 396526); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17543_run389_5047() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396527 })).Sim); }
    [Fact] public void Q17544_run389_5048() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17545_run389_5049() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17546_run389_5050() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17547_run389_5051() { var g = Game.Solo(seed: 396531); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17548_run389_5052() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396532 })).Sim); }
    [Fact] public void Q17549_run389_5053() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17550_run389_5054() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17551_run389_5055() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17552_run389_5056() { var g = Game.Solo(seed: 396536); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17553_run389_5057() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396537 })).Sim); }
    [Fact] public void Q17554_run389_5058() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17555_run389_5059() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17556_run389_5060() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17557_run389_5061() { var g = Game.Solo(seed: 396541); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17558_run389_5062() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396542 })).Sim); }
    [Fact] public void Q17559_run389_5063() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17560_run389_5064() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
    [Fact] public void Q17561_run389_5065() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Approaches.Count >= 1); }
    [Fact] public void Q17562_run389_5066() { var g = Game.Solo(seed: 396546); Assert.Equal(GameState.CurrentSchema, g.State.Schema); }
    [Fact] public void Q17563_run389_5067() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 396547 })).Sim); }
    [Fact] public void Q17564_run389_5068() { GameConfig c = TestData.Config(); Assert.True(c.Runs.Checkpoints.Count >= 1); }
    [Fact] public void Q17565_run389_5069() { GameConfig c = TestData.Config(); Assert.True(c.Runs.EngineerSavingCapPermille > 0); }
}
