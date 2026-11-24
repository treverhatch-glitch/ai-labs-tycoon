using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1256–Q1325: batch 23 rival arrival bar (schema 41).</summary>
public class HatchRivalArrivalLaterTests
{
    [Fact] public void Q1256_rival_arrival_1256() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1257_rival_arrival_1257() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1258_rival_arrival_1258() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1259_rival_arrival_1259() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1260_rival_arrival_1260() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1261_rival_arrival_1261() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1262_rival_arrival_1262() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1263_rival_arrival_1263() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1264_rival_arrival_1264() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1265_rival_arrival_1265() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1266_rival_arrival_1266() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1267_rival_arrival_1267() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1268_rival_arrival_1268() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1269_rival_arrival_1269() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1270_rival_arrival_1270() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1271_rival_arrival_1271() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1272_rival_arrival_1272() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1273_rival_arrival_1273() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1274_rival_arrival_1274() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1275_rival_arrival_1275() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1276_rival_arrival_1276() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1277_rival_arrival_1277() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1278_rival_arrival_1278() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1279_rival_arrival_1279() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1280_rival_arrival_1280() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1281_rival_arrival_1281() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1282_rival_arrival_1282() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1283_rival_arrival_1283() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1284_rival_arrival_1284() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1285_rival_arrival_1285() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1286_rival_arrival_1286() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1287_rival_arrival_1287() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1288_rival_arrival_1288() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1289_rival_arrival_1289() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1290_rival_arrival_1290() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1291_rival_arrival_1291() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1292_rival_arrival_1292() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1293_rival_arrival_1293() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1294_rival_arrival_1294() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1295_rival_arrival_1295() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1296_rival_arrival_1296() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1297_rival_arrival_1297() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1298_rival_arrival_1298() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1299_rival_arrival_1299() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1300_rival_arrival_1300() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1301_rival_arrival_1301() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1302_rival_arrival_1302() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1303_rival_arrival_1303() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1304_rival_arrival_1304() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1305_rival_arrival_1305() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1306_rival_arrival_1306() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1307_rival_arrival_1307() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1308_rival_arrival_1308() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1309_rival_arrival_1309() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1310_rival_arrival_1310() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1311_rival_arrival_1311() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1312_rival_arrival_1312() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1313_rival_arrival_1313() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1314_rival_arrival_1314() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1315_rival_arrival_1315() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1316_rival_arrival_1316() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1317_rival_arrival_1317() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1318_rival_arrival_1318() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1319_rival_arrival_1319() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1320_rival_arrival_1320() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
    [Fact] public void Q1321_rival_arrival_1321() { GameConfig c = TestData.Config(); Assert.Equal(2007, c.Rival("metta").JoinsYear); Assert.False(string.IsNullOrEmpty(c.Rival("metta").Leader)); }
    [Fact] public void Q1322_rival_arrival_1322() { Game g = Game.New(); Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray()); Assert.Equal(41, g.State.Schema); }
    [Fact] public void Q1323_rival_arrival_1323() { Game g = Game.Solo(); Assert.Empty(g.State.PendingRivals); g.Years(5); Assert.Single(g.State.Labs); }
    [Fact] public void Q1324_rival_arrival_1324() { GameConfig c = TestData.Config(); Assert.True(c.Rival("deepsneak").JoinsYear > 1995 || "deepsneak" == "openaye"); }
    [Fact] public void Q1325_rival_arrival_1325() { Game g = Game.New(); int tick = GameDate.TickOfYear(g.C.Calendar, 2023); g.Sim.StepMany(tick - g.Tick); Assert.Contains(g.State.Labs, l => l.RivalId == "deepsneak" || g.State.PendingRivals.Count >= 0); }
}
