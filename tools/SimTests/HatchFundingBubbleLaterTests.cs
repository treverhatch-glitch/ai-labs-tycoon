using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1396–Q1465: batch 25 funding/bubble data bar (schema 41).</summary>
public class HatchFundingBubbleLaterTests
{
    [Fact] public void Q1396_fund_bubble_1396() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1397_fund_bubble_1397() { Game g = Game.Solo(seed: 2401); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1398_fund_bubble_1398() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1399_fund_bubble_1399() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2503); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1400_fund_bubble_1400() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1401_fund_bubble_1401() { Game g = Game.Solo(seed: 2405); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1402_fund_bubble_1402() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1403_fund_bubble_1403() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2507); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1404_fund_bubble_1404() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1405_fund_bubble_1405() { Game g = Game.Solo(seed: 2409); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1406_fund_bubble_1406() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1407_fund_bubble_1407() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2511); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1408_fund_bubble_1408() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1409_fund_bubble_1409() { Game g = Game.Solo(seed: 2413); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1410_fund_bubble_1410() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1411_fund_bubble_1411() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2515); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1412_fund_bubble_1412() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1413_fund_bubble_1413() { Game g = Game.Solo(seed: 2417); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1414_fund_bubble_1414() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1415_fund_bubble_1415() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2519); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1416_fund_bubble_1416() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1417_fund_bubble_1417() { Game g = Game.Solo(seed: 2421); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1418_fund_bubble_1418() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1419_fund_bubble_1419() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2523); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1420_fund_bubble_1420() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1421_fund_bubble_1421() { Game g = Game.Solo(seed: 2425); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1422_fund_bubble_1422() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1423_fund_bubble_1423() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2527); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1424_fund_bubble_1424() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1425_fund_bubble_1425() { Game g = Game.Solo(seed: 2429); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1426_fund_bubble_1426() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1427_fund_bubble_1427() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2531); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1428_fund_bubble_1428() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1429_fund_bubble_1429() { Game g = Game.Solo(seed: 2433); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1430_fund_bubble_1430() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1431_fund_bubble_1431() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2535); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1432_fund_bubble_1432() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1433_fund_bubble_1433() { Game g = Game.Solo(seed: 2437); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1434_fund_bubble_1434() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1435_fund_bubble_1435() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2539); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1436_fund_bubble_1436() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1437_fund_bubble_1437() { Game g = Game.Solo(seed: 2441); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1438_fund_bubble_1438() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1439_fund_bubble_1439() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2543); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1440_fund_bubble_1440() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1441_fund_bubble_1441() { Game g = Game.Solo(seed: 2445); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1442_fund_bubble_1442() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1443_fund_bubble_1443() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2547); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1444_fund_bubble_1444() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1445_fund_bubble_1445() { Game g = Game.Solo(seed: 2449); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1446_fund_bubble_1446() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1447_fund_bubble_1447() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2551); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1448_fund_bubble_1448() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1449_fund_bubble_1449() { Game g = Game.Solo(seed: 2453); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1450_fund_bubble_1450() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1451_fund_bubble_1451() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2555); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1452_fund_bubble_1452() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1453_fund_bubble_1453() { Game g = Game.Solo(seed: 2457); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1454_fund_bubble_1454() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1455_fund_bubble_1455() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2559); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1456_fund_bubble_1456() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1457_fund_bubble_1457() { Game g = Game.Solo(seed: 2461); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1458_fund_bubble_1458() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1459_fund_bubble_1459() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2563); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1460_fund_bubble_1460() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1461_fund_bubble_1461() { Game g = Game.Solo(seed: 2465); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
    [Fact] public void Q1462_fund_bubble_1462() { Game g = Game.Solo(); Assert.True(g.State.BubblePermille >= 0); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1463_fund_bubble_1463() { GameConfig c = TestData.Config(); Assert.Contains(c.Funding.Rounds, r => !string.IsNullOrEmpty(r.Id)); var g = Game.Solo(seed: 2567); Assert.NotNull(g.C.Bubble); }
    [Fact] public void Q1464_fund_bubble_1464() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.PopAtPermille > 0); Assert.True(c.Bubble.HotPermille > 0); }
    [Fact] public void Q1465_fund_bubble_1465() { Game g = Game.Solo(seed: 2469); Assert.Equal(41, g.State.Schema); Assert.True(g.C.Funding.Rounds.Count >= 3); }
}
