using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q6016–6085: batch 169 capital/bubble data bar coda (schema 29).</summary>
public class HatchCapitalBubbleBarLaterTests10
{
    [Fact] public void Q11456_capital10_4336() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11457_capital10_4337() { var g = Game.Solo(seed: 105442); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11458_capital10_4338() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105543 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11459_capital10_4339() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11460_capital10_4340() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11461_capital10_4341() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11462_capital10_4342() { var g = Game.Solo(seed: 105447); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11463_capital10_4343() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105548 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11464_capital10_4344() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11465_capital10_4345() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11466_capital10_4346() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11467_capital10_4347() { var g = Game.Solo(seed: 105452); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11468_capital10_4348() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105553 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11469_capital10_4349() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11470_capital10_4350() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11471_capital10_4351() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11472_capital10_4352() { var g = Game.Solo(seed: 105457); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11473_capital10_4353() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105558 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11474_capital10_4354() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11475_capital10_4355() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11476_capital10_4356() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11477_capital10_4357() { var g = Game.Solo(seed: 105462); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11478_capital10_4358() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105563 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11479_capital10_4359() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11480_capital10_4360() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11481_capital10_4361() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11482_capital10_4362() { var g = Game.Solo(seed: 105467); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11483_capital10_4363() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105568 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11484_capital10_4364() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11485_capital10_4365() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11486_capital10_4366() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11487_capital10_4367() { var g = Game.Solo(seed: 105472); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11488_capital10_4368() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105573 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11489_capital10_4369() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11490_capital10_4370() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11491_capital10_4371() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11492_capital10_4372() { var g = Game.Solo(seed: 105477); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11493_capital10_4373() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105578 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11494_capital10_4374() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11495_capital10_4375() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11496_capital10_4376() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11497_capital10_4377() { var g = Game.Solo(seed: 105482); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11498_capital10_4378() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105583 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11499_capital10_4379() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11500_capital10_4380() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11501_capital10_4381() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11502_capital10_4382() { var g = Game.Solo(seed: 105487); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11503_capital10_4383() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105588 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11504_capital10_4384() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11505_capital10_4385() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11506_capital10_4386() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11507_capital10_4387() { var g = Game.Solo(seed: 105492); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11508_capital10_4388() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105593 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11509_capital10_4389() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11510_capital10_4390() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11511_capital10_4391() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11512_capital10_4392() { var g = Game.Solo(seed: 105497); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11513_capital10_4393() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105598 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11514_capital10_4394() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11515_capital10_4395() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11516_capital10_4396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11517_capital10_4397() { var g = Game.Solo(seed: 105502); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11518_capital10_4398() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105603 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11519_capital10_4399() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11520_capital10_4400() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
    [Fact] public void Q11521_capital10_4401() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.CrashFundingCashPermille > 0); Assert.True(c.Bubble.ResetCapitalRaisedAfterCrash); }
    [Fact] public void Q11522_capital10_4402() { var g = Game.Solo(seed: 105507); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Funding.FounderStartPermille > 0); }
    [Fact] public void Q11523_capital10_4403() { GameConfig c = TestData.Config(); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 105608 })).Sim); Assert.True(c.Bubble.PostCrashResidualPermille > 0); }
    [Fact] public void Q11524_capital10_4404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.SurvivorRevenuePpm > 0); Assert.True(c.Bubble.WeakCashHaircutPermille > 0); }
    [Fact] public void Q11525_capital10_4405() { GameConfig c = TestData.Config(); var g = Game.New(seed: 151); Assert.Equal(29, g.State.Schema); Assert.True(g.C.Bubble.PopAtPermille > 0); }
}
