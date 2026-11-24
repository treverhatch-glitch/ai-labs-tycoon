using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1536–Q1605: batch 27 freeze/audit bar (schema 42).</summary>
public class HatchFreezeAuditBarLaterTests
{
    [Fact] public void Q1536_freeze_audit_1536() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1537_freeze_audit_1537() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2701 })).State.Schema); }
    [Fact] public void Q1538_freeze_audit_1538() { var g = Game.Solo(seed: 2802); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1539_freeze_audit_1539() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1540_freeze_audit_1540() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1541_freeze_audit_1541() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2705 })).State.Schema); }
    [Fact] public void Q1542_freeze_audit_1542() { var g = Game.Solo(seed: 2806); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1543_freeze_audit_1543() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1544_freeze_audit_1544() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1545_freeze_audit_1545() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2709 })).State.Schema); }
    [Fact] public void Q1546_freeze_audit_1546() { var g = Game.Solo(seed: 2810); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1547_freeze_audit_1547() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1548_freeze_audit_1548() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1549_freeze_audit_1549() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2713 })).State.Schema); }
    [Fact] public void Q1550_freeze_audit_1550() { var g = Game.Solo(seed: 2814); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1551_freeze_audit_1551() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1552_freeze_audit_1552() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1553_freeze_audit_1553() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2717 })).State.Schema); }
    [Fact] public void Q1554_freeze_audit_1554() { var g = Game.Solo(seed: 2818); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1555_freeze_audit_1555() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1556_freeze_audit_1556() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1557_freeze_audit_1557() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2721 })).State.Schema); }
    [Fact] public void Q1558_freeze_audit_1558() { var g = Game.Solo(seed: 2822); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1559_freeze_audit_1559() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1560_freeze_audit_1560() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1561_freeze_audit_1561() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2725 })).State.Schema); }
    [Fact] public void Q1562_freeze_audit_1562() { var g = Game.Solo(seed: 2826); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1563_freeze_audit_1563() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1564_freeze_audit_1564() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1565_freeze_audit_1565() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2729 })).State.Schema); }
    [Fact] public void Q1566_freeze_audit_1566() { var g = Game.Solo(seed: 2830); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1567_freeze_audit_1567() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1568_freeze_audit_1568() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1569_freeze_audit_1569() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2733 })).State.Schema); }
    [Fact] public void Q1570_freeze_audit_1570() { var g = Game.Solo(seed: 2834); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1571_freeze_audit_1571() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1572_freeze_audit_1572() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1573_freeze_audit_1573() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2737 })).State.Schema); }
    [Fact] public void Q1574_freeze_audit_1574() { var g = Game.Solo(seed: 2838); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1575_freeze_audit_1575() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1576_freeze_audit_1576() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1577_freeze_audit_1577() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2741 })).State.Schema); }
    [Fact] public void Q1578_freeze_audit_1578() { var g = Game.Solo(seed: 2842); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1579_freeze_audit_1579() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1580_freeze_audit_1580() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1581_freeze_audit_1581() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2745 })).State.Schema); }
    [Fact] public void Q1582_freeze_audit_1582() { var g = Game.Solo(seed: 2846); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1583_freeze_audit_1583() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1584_freeze_audit_1584() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1585_freeze_audit_1585() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2749 })).State.Schema); }
    [Fact] public void Q1586_freeze_audit_1586() { var g = Game.Solo(seed: 2850); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1587_freeze_audit_1587() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1588_freeze_audit_1588() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1589_freeze_audit_1589() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2753 })).State.Schema); }
    [Fact] public void Q1590_freeze_audit_1590() { var g = Game.Solo(seed: 2854); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1591_freeze_audit_1591() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1592_freeze_audit_1592() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1593_freeze_audit_1593() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2757 })).State.Schema); }
    [Fact] public void Q1594_freeze_audit_1594() { var g = Game.Solo(seed: 2858); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1595_freeze_audit_1595() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1596_freeze_audit_1596() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1597_freeze_audit_1597() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2761 })).State.Schema); }
    [Fact] public void Q1598_freeze_audit_1598() { var g = Game.Solo(seed: 2862); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1599_freeze_audit_1599() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1600_freeze_audit_1600() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q1601_freeze_audit_1601() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2765 })).State.Schema); }
    [Fact] public void Q1602_freeze_audit_1602() { var g = Game.Solo(seed: 2866); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1603_freeze_audit_1603() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q1604_freeze_audit_1604() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q1605_freeze_audit_1605() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(45, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2769 })).State.Schema); }
}
