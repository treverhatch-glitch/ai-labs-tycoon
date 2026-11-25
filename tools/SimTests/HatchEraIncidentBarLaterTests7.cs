using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6706–6775: batch 185 era/incident data bar coda (schema 29).</summary>
public class HatchEraIncidentBarLaterTests7
{
    [Fact] public void Q12436_erainc7_3986() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12437_erainc7_3987() { var g = Game.Solo(seed: 119522); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12438_erainc7_3988() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119623 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12439_erainc7_3989() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12440_erainc7_3990() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12441_erainc7_3991() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12442_erainc7_3992() { var g = Game.Solo(seed: 119527); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12443_erainc7_3993() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119628 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12444_erainc7_3994() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12445_erainc7_3995() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12446_erainc7_3996() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12447_erainc7_3997() { var g = Game.Solo(seed: 119532); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12448_erainc7_3998() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119633 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12449_erainc7_3999() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12450_erainc7_4000() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12451_erainc7_4001() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12452_erainc7_4002() { var g = Game.Solo(seed: 119537); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12453_erainc7_4003() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119638 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12454_erainc7_4004() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12455_erainc7_4005() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12456_erainc7_4006() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12457_erainc7_4007() { var g = Game.Solo(seed: 119542); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12458_erainc7_4008() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119643 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12459_erainc7_4009() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12460_erainc7_4010() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12461_erainc7_4011() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12462_erainc7_4012() { var g = Game.Solo(seed: 119547); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12463_erainc7_4013() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119648 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12464_erainc7_4014() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12465_erainc7_4015() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12466_erainc7_4016() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12467_erainc7_4017() { var g = Game.Solo(seed: 119552); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12468_erainc7_4018() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119653 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12469_erainc7_4019() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12470_erainc7_4020() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12471_erainc7_4021() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12472_erainc7_4022() { var g = Game.Solo(seed: 119557); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12473_erainc7_4023() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119658 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12474_erainc7_4024() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12475_erainc7_4025() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12476_erainc7_4026() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12477_erainc7_4027() { var g = Game.Solo(seed: 119562); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12478_erainc7_4028() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119663 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12479_erainc7_4029() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12480_erainc7_4030() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12481_erainc7_4031() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12482_erainc7_4032() { var g = Game.Solo(seed: 119567); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12483_erainc7_4033() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119668 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12484_erainc7_4034() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12485_erainc7_4035() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12486_erainc7_4036() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12487_erainc7_4037() { var g = Game.Solo(seed: 119572); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12488_erainc7_4038() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119673 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12489_erainc7_4039() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12490_erainc7_4040() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12491_erainc7_4041() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12492_erainc7_4042() { var g = Game.Solo(seed: 119577); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12493_erainc7_4043() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119678 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12494_erainc7_4044() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12495_erainc7_4045() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12496_erainc7_4046() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12497_erainc7_4047() { var g = Game.Solo(seed: 119582); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12498_erainc7_4048() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119683 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12499_erainc7_4049() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12500_erainc7_4050() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
    [Fact] public void Q12501_erainc7_4051() { GameConfig c = TestData.Config(); var agents = c.Eras.First(e => e.Id == "agents"); Assert.True(agents.IncidentLoadPermille >= 1000); Assert.All(c.Eras, e => Assert.True(e.IncidentLoadPermille > 0)); }
    [Fact] public void Q12502_erainc7_4052() { var g = Game.Solo(seed: 119587); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12503_erainc7_4053() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 119688 })).Sim); Assert.Equal(9, c.Eras.Count); }
    [Fact] public void Q12504_erainc7_4054() { GameConfig c = TestData.Config(); var dial = c.Era("dial-up-days"); var agents = c.Era("agents"); Assert.True(agents.IncidentLoadPermille >= dial.IncidentLoadPermille); Assert.True(c.Safety.IncidentChancePermille > 0); }
    [Fact] public void Q12505_erainc7_4055() { GameConfig c = TestData.Config(); var g = Game.New(seed: 167); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Eras.First().IncidentLoadPermille > 0); }
}
