using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7126–7195: batch 107 freeze/audit bar (schema 29).</summary>
public class HatchFreezeAuditBarLaterTests3
{
    [Fact] public void Q7126_freezaud3_1536() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7127_freezaud3_1537() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45203 })).State.Schema); }
    [Fact] public void Q7128_freezaud3_1538() { var g = Game.Solo(seed: 45102); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7129_freezaud3_1539() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7130_freezaud3_1540() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7131_freezaud3_1541() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45207 })).State.Schema); }
    [Fact] public void Q7132_freezaud3_1542() { var g = Game.Solo(seed: 45106); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7133_freezaud3_1543() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7134_freezaud3_1544() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7135_freezaud3_1545() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45211 })).State.Schema); }
    [Fact] public void Q7136_freezaud3_1546() { var g = Game.Solo(seed: 45110); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7137_freezaud3_1547() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7138_freezaud3_1548() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7139_freezaud3_1549() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45215 })).State.Schema); }
    [Fact] public void Q7140_freezaud3_1550() { var g = Game.Solo(seed: 45114); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7141_freezaud3_1551() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7142_freezaud3_1552() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7143_freezaud3_1553() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45219 })).State.Schema); }
    [Fact] public void Q7144_freezaud3_1554() { var g = Game.Solo(seed: 45118); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7145_freezaud3_1555() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7146_freezaud3_1556() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7147_freezaud3_1557() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45223 })).State.Schema); }
    [Fact] public void Q7148_freezaud3_1558() { var g = Game.Solo(seed: 45122); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7149_freezaud3_1559() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7150_freezaud3_1560() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7151_freezaud3_1561() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45227 })).State.Schema); }
    [Fact] public void Q7152_freezaud3_1562() { var g = Game.Solo(seed: 45126); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7153_freezaud3_1563() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7154_freezaud3_1564() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7155_freezaud3_1565() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45231 })).State.Schema); }
    [Fact] public void Q7156_freezaud3_1566() { var g = Game.Solo(seed: 45130); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7157_freezaud3_1567() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7158_freezaud3_1568() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7159_freezaud3_1569() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45235 })).State.Schema); }
    [Fact] public void Q7160_freezaud3_1570() { var g = Game.Solo(seed: 45134); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7161_freezaud3_1571() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7162_freezaud3_1572() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7163_freezaud3_1573() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45239 })).State.Schema); }
    [Fact] public void Q7164_freezaud3_1574() { var g = Game.Solo(seed: 45138); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7165_freezaud3_1575() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7166_freezaud3_1576() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7167_freezaud3_1577() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45243 })).State.Schema); }
    [Fact] public void Q7168_freezaud3_1578() { var g = Game.Solo(seed: 45142); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7169_freezaud3_1579() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7170_freezaud3_1580() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7171_freezaud3_1581() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45247 })).State.Schema); }
    [Fact] public void Q7172_freezaud3_1582() { var g = Game.Solo(seed: 45146); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7173_freezaud3_1583() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7174_freezaud3_1584() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7175_freezaud3_1585() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45251 })).State.Schema); }
    [Fact] public void Q7176_freezaud3_1586() { var g = Game.Solo(seed: 45150); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7177_freezaud3_1587() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7178_freezaud3_1588() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7179_freezaud3_1589() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45255 })).State.Schema); }
    [Fact] public void Q7180_freezaud3_1590() { var g = Game.Solo(seed: 45154); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7181_freezaud3_1591() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7182_freezaud3_1592() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7183_freezaud3_1593() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45259 })).State.Schema); }
    [Fact] public void Q7184_freezaud3_1594() { var g = Game.Solo(seed: 45158); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7185_freezaud3_1595() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7186_freezaud3_1596() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7187_freezaud3_1597() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45263 })).State.Schema); }
    [Fact] public void Q7188_freezaud3_1598() { var g = Game.Solo(seed: 45162); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7189_freezaud3_1599() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7190_freezaud3_1600() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q7191_freezaud3_1601() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45267 })).State.Schema); }
    [Fact] public void Q7192_freezaud3_1602() { var g = Game.Solo(seed: 45166); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q7193_freezaud3_1603() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q7194_freezaud3_1604() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q7195_freezaud3_1605() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 45271 })).State.Schema); }
}
