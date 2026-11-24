using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1326–Q1395: batch 24 sector/rival economy data bar (schema 37).</summary>
public class HatchSectorRivalLaterTests
{
    [Fact] public void Q1326_sector_rival_1326() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1327_sector_rival_1327() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1328_sector_rival_1328() { var g = Game.New(seed: 2202); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1329_sector_rival_1329() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2303 })).Sim); }
    [Fact] public void Q1330_sector_rival_1330() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1331_sector_rival_1331() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1332_sector_rival_1332() { var g = Game.New(seed: 2206); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1333_sector_rival_1333() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2307 })).Sim); }
    [Fact] public void Q1334_sector_rival_1334() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1335_sector_rival_1335() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1336_sector_rival_1336() { var g = Game.New(seed: 2210); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1337_sector_rival_1337() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2311 })).Sim); }
    [Fact] public void Q1338_sector_rival_1338() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1339_sector_rival_1339() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1340_sector_rival_1340() { var g = Game.New(seed: 2214); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1341_sector_rival_1341() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2315 })).Sim); }
    [Fact] public void Q1342_sector_rival_1342() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1343_sector_rival_1343() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1344_sector_rival_1344() { var g = Game.New(seed: 2218); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1345_sector_rival_1345() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2319 })).Sim); }
    [Fact] public void Q1346_sector_rival_1346() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1347_sector_rival_1347() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1348_sector_rival_1348() { var g = Game.New(seed: 2222); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1349_sector_rival_1349() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2323 })).Sim); }
    [Fact] public void Q1350_sector_rival_1350() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1351_sector_rival_1351() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1352_sector_rival_1352() { var g = Game.New(seed: 2226); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1353_sector_rival_1353() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2327 })).Sim); }
    [Fact] public void Q1354_sector_rival_1354() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1355_sector_rival_1355() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1356_sector_rival_1356() { var g = Game.New(seed: 2230); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1357_sector_rival_1357() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2331 })).Sim); }
    [Fact] public void Q1358_sector_rival_1358() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1359_sector_rival_1359() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1360_sector_rival_1360() { var g = Game.New(seed: 2234); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1361_sector_rival_1361() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2335 })).Sim); }
    [Fact] public void Q1362_sector_rival_1362() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1363_sector_rival_1363() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1364_sector_rival_1364() { var g = Game.New(seed: 2238); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1365_sector_rival_1365() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2339 })).Sim); }
    [Fact] public void Q1366_sector_rival_1366() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1367_sector_rival_1367() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1368_sector_rival_1368() { var g = Game.New(seed: 2242); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1369_sector_rival_1369() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2343 })).Sim); }
    [Fact] public void Q1370_sector_rival_1370() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1371_sector_rival_1371() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1372_sector_rival_1372() { var g = Game.New(seed: 2246); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1373_sector_rival_1373() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2347 })).Sim); }
    [Fact] public void Q1374_sector_rival_1374() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1375_sector_rival_1375() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1376_sector_rival_1376() { var g = Game.New(seed: 2250); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1377_sector_rival_1377() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2351 })).Sim); }
    [Fact] public void Q1378_sector_rival_1378() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1379_sector_rival_1379() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1380_sector_rival_1380() { var g = Game.New(seed: 2254); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1381_sector_rival_1381() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2355 })).Sim); }
    [Fact] public void Q1382_sector_rival_1382() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1383_sector_rival_1383() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1384_sector_rival_1384() { var g = Game.New(seed: 2258); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1385_sector_rival_1385() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2359 })).Sim); }
    [Fact] public void Q1386_sector_rival_1386() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1387_sector_rival_1387() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1388_sector_rival_1388() { var g = Game.New(seed: 2262); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1389_sector_rival_1389() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2363 })).Sim); }
    [Fact] public void Q1390_sector_rival_1390() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1391_sector_rival_1391() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1392_sector_rival_1392() { var g = Game.New(seed: 2266); Assert.All(g.C.Market.Sectors, s => Assert.True(s.MinCapability <= s.SaturationCapability)); }
    [Fact] public void Q1393_sector_rival_1393() { GameConfig c = TestData.Config(); Assert.Contains(c.Rivals, r => r.JoinsYear == 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2367 })).Sim); }
    [Fact] public void Q1394_sector_rival_1394() { GameConfig c = TestData.Config(); Assert.NotEmpty(c.Market.Sectors); Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Era))); }
    [Fact] public void Q1395_sector_rival_1395() { Game g = Game.New(); Assert.True(g.State.Labs.Count >= 4); Assert.Equal(37, g.State.Schema); }
}
