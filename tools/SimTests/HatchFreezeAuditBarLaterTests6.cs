using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7126–7195: batch 167 freeze/audit bar (schema 29).</summary>
public class HatchFreezeAuditBarLaterTests6
{
    [Fact] public void Q11316_freezaud6_1536() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11317_freezaud6_1537() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103783 })).State.Schema); }
    [Fact] public void Q11318_freezaud6_1538() { var g = Game.Solo(seed: 103682); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11319_freezaud6_1539() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11320_freezaud6_1540() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11321_freezaud6_1541() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103787 })).State.Schema); }
    [Fact] public void Q11322_freezaud6_1542() { var g = Game.Solo(seed: 103686); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11323_freezaud6_1543() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11324_freezaud6_1544() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11325_freezaud6_1545() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103791 })).State.Schema); }
    [Fact] public void Q11326_freezaud6_1546() { var g = Game.Solo(seed: 103690); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11327_freezaud6_1547() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11328_freezaud6_1548() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11329_freezaud6_1549() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103795 })).State.Schema); }
    [Fact] public void Q11330_freezaud6_1550() { var g = Game.Solo(seed: 103694); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11331_freezaud6_1551() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11332_freezaud6_1552() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11333_freezaud6_1553() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103799 })).State.Schema); }
    [Fact] public void Q11334_freezaud6_1554() { var g = Game.Solo(seed: 103698); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11335_freezaud6_1555() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11336_freezaud6_1556() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11337_freezaud6_1557() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103803 })).State.Schema); }
    [Fact] public void Q11338_freezaud6_1558() { var g = Game.Solo(seed: 103702); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11339_freezaud6_1559() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11340_freezaud6_1560() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11341_freezaud6_1561() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103807 })).State.Schema); }
    [Fact] public void Q11342_freezaud6_1562() { var g = Game.Solo(seed: 103706); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11343_freezaud6_1563() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11344_freezaud6_1564() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11345_freezaud6_1565() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103811 })).State.Schema); }
    [Fact] public void Q11346_freezaud6_1566() { var g = Game.Solo(seed: 103710); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11347_freezaud6_1567() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11348_freezaud6_1568() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11349_freezaud6_1569() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103815 })).State.Schema); }
    [Fact] public void Q11350_freezaud6_1570() { var g = Game.Solo(seed: 103714); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11351_freezaud6_1571() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11352_freezaud6_1572() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11353_freezaud6_1573() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103819 })).State.Schema); }
    [Fact] public void Q11354_freezaud6_1574() { var g = Game.Solo(seed: 103718); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11355_freezaud6_1575() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11356_freezaud6_1576() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11357_freezaud6_1577() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103823 })).State.Schema); }
    [Fact] public void Q11358_freezaud6_1578() { var g = Game.Solo(seed: 103722); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11359_freezaud6_1579() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11360_freezaud6_1580() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11361_freezaud6_1581() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103827 })).State.Schema); }
    [Fact] public void Q11362_freezaud6_1582() { var g = Game.Solo(seed: 103726); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11363_freezaud6_1583() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11364_freezaud6_1584() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11365_freezaud6_1585() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103831 })).State.Schema); }
    [Fact] public void Q11366_freezaud6_1586() { var g = Game.Solo(seed: 103730); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11367_freezaud6_1587() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11368_freezaud6_1588() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11369_freezaud6_1589() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103835 })).State.Schema); }
    [Fact] public void Q11370_freezaud6_1590() { var g = Game.Solo(seed: 103734); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11371_freezaud6_1591() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11372_freezaud6_1592() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11373_freezaud6_1593() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103839 })).State.Schema); }
    [Fact] public void Q11374_freezaud6_1594() { var g = Game.Solo(seed: 103738); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11375_freezaud6_1595() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11376_freezaud6_1596() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11377_freezaud6_1597() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103843 })).State.Schema); }
    [Fact] public void Q11378_freezaud6_1598() { var g = Game.Solo(seed: 103742); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11379_freezaud6_1599() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11380_freezaud6_1600() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q11381_freezaud6_1601() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103847 })).State.Schema); }
    [Fact] public void Q11382_freezaud6_1602() { var g = Game.Solo(seed: 103746); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q11383_freezaud6_1603() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q11384_freezaud6_1604() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q11385_freezaud6_1605() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(29, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 103851 })).State.Schema); }
}
