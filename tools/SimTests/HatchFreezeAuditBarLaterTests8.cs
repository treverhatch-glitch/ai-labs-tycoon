using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7126–7195: batch 207 freeze/audit bar (schema 29).</summary>
public class HatchFreezeAuditBarLaterTests8
{
    [Fact] public void Q13916_freezaud8_1536() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13917_freezaud8_1537() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138383 })).State.Schema); }
    [Fact] public void Q13918_freezaud8_1538() { var g = Game.Solo(seed: 138282); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13919_freezaud8_1539() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13920_freezaud8_1540() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13921_freezaud8_1541() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138387 })).State.Schema); }
    [Fact] public void Q13922_freezaud8_1542() { var g = Game.Solo(seed: 138286); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13923_freezaud8_1543() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13924_freezaud8_1544() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13925_freezaud8_1545() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138391 })).State.Schema); }
    [Fact] public void Q13926_freezaud8_1546() { var g = Game.Solo(seed: 138290); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13927_freezaud8_1547() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13928_freezaud8_1548() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13929_freezaud8_1549() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138395 })).State.Schema); }
    [Fact] public void Q13930_freezaud8_1550() { var g = Game.Solo(seed: 138294); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13931_freezaud8_1551() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13932_freezaud8_1552() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13933_freezaud8_1553() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138399 })).State.Schema); }
    [Fact] public void Q13934_freezaud8_1554() { var g = Game.Solo(seed: 138298); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13935_freezaud8_1555() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13936_freezaud8_1556() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13937_freezaud8_1557() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138403 })).State.Schema); }
    [Fact] public void Q13938_freezaud8_1558() { var g = Game.Solo(seed: 138302); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13939_freezaud8_1559() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13940_freezaud8_1560() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13941_freezaud8_1561() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138407 })).State.Schema); }
    [Fact] public void Q13942_freezaud8_1562() { var g = Game.Solo(seed: 138306); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13943_freezaud8_1563() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13944_freezaud8_1564() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13945_freezaud8_1565() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138411 })).State.Schema); }
    [Fact] public void Q13946_freezaud8_1566() { var g = Game.Solo(seed: 138310); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13947_freezaud8_1567() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13948_freezaud8_1568() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13949_freezaud8_1569() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138415 })).State.Schema); }
    [Fact] public void Q13950_freezaud8_1570() { var g = Game.Solo(seed: 138314); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13951_freezaud8_1571() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13952_freezaud8_1572() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13953_freezaud8_1573() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138419 })).State.Schema); }
    [Fact] public void Q13954_freezaud8_1574() { var g = Game.Solo(seed: 138318); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13955_freezaud8_1575() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13956_freezaud8_1576() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13957_freezaud8_1577() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138423 })).State.Schema); }
    [Fact] public void Q13958_freezaud8_1578() { var g = Game.Solo(seed: 138322); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13959_freezaud8_1579() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13960_freezaud8_1580() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13961_freezaud8_1581() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138427 })).State.Schema); }
    [Fact] public void Q13962_freezaud8_1582() { var g = Game.Solo(seed: 138326); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13963_freezaud8_1583() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13964_freezaud8_1584() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13965_freezaud8_1585() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138431 })).State.Schema); }
    [Fact] public void Q13966_freezaud8_1586() { var g = Game.Solo(seed: 138330); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13967_freezaud8_1587() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13968_freezaud8_1588() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13969_freezaud8_1589() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138435 })).State.Schema); }
    [Fact] public void Q13970_freezaud8_1590() { var g = Game.Solo(seed: 138334); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13971_freezaud8_1591() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13972_freezaud8_1592() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13973_freezaud8_1593() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138439 })).State.Schema); }
    [Fact] public void Q13974_freezaud8_1594() { var g = Game.Solo(seed: 138338); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13975_freezaud8_1595() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13976_freezaud8_1596() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13977_freezaud8_1597() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138443 })).State.Schema); }
    [Fact] public void Q13978_freezaud8_1598() { var g = Game.Solo(seed: 138342); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13979_freezaud8_1599() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13980_freezaud8_1600() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q13981_freezaud8_1601() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138447 })).State.Schema); }
    [Fact] public void Q13982_freezaud8_1602() { var g = Game.Solo(seed: 138346); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q13983_freezaud8_1603() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q13984_freezaud8_1604() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q13985_freezaud8_1605() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 138451 })).State.Schema); }
}
