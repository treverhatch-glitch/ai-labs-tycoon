using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q3006–Q3075: batch 48 era/incident data bar (schema 42).</summary>
public class HatchEraIncidentBarLaterTests
{
    [Fact] public void Q3006_erainc_3006() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3007_erainc_3007() { var g = Game.Solo(seed: 6802); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3008_erainc_3008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6904 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3009_erainc_3009() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3010_erainc_3010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3011_erainc_3011() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3012_erainc_3012() { var g = Game.Solo(seed: 6807); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3013_erainc_3013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6909 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3014_erainc_3014() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3015_erainc_3015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3016_erainc_3016() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3017_erainc_3017() { var g = Game.Solo(seed: 6812); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3018_erainc_3018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6914 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3019_erainc_3019() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3020_erainc_3020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3021_erainc_3021() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3022_erainc_3022() { var g = Game.Solo(seed: 6817); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3023_erainc_3023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6919 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3024_erainc_3024() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3025_erainc_3025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3026_erainc_3026() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3027_erainc_3027() { var g = Game.Solo(seed: 6822); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3028_erainc_3028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6924 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3029_erainc_3029() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3030_erainc_3030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3031_erainc_3031() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3032_erainc_3032() { var g = Game.Solo(seed: 6827); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3033_erainc_3033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6929 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3034_erainc_3034() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3035_erainc_3035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3036_erainc_3036() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3037_erainc_3037() { var g = Game.Solo(seed: 6832); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3038_erainc_3038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6934 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3039_erainc_3039() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3040_erainc_3040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3041_erainc_3041() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3042_erainc_3042() { var g = Game.Solo(seed: 6837); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3043_erainc_3043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6939 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3044_erainc_3044() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3045_erainc_3045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3046_erainc_3046() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3047_erainc_3047() { var g = Game.Solo(seed: 6842); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3048_erainc_3048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6944 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3049_erainc_3049() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3050_erainc_3050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3051_erainc_3051() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3052_erainc_3052() { var g = Game.Solo(seed: 6847); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3053_erainc_3053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6949 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3054_erainc_3054() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3055_erainc_3055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3056_erainc_3056() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3057_erainc_3057() { var g = Game.Solo(seed: 6852); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3058_erainc_3058() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6954 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3059_erainc_3059() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3060_erainc_3060() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3061_erainc_3061() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3062_erainc_3062() { var g = Game.Solo(seed: 6857); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3063_erainc_3063() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6959 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3064_erainc_3064() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3065_erainc_3065() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3066_erainc_3066() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3067_erainc_3067() { var g = Game.Solo(seed: 6862); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3068_erainc_3068() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6964 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3069_erainc_3069() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3070_erainc_3070() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
    [Fact] public void Q3071_erainc_3071() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q3072_erainc_3072() { var g = Game.Solo(seed: 6867); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3073_erainc_3073() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 6969 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q3074_erainc_3074() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q3075_erainc_3075() { GameConfig c = TestData.Config(); var g = Game.New(seed: 48); Assert.Equal(GameState.CurrentSchema, g.State.Schema); Assert.True(c.Safety.IncidentRungs.Count >= 6); }
}
