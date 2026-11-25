using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q7126–7195: batch 131 freeze/audit bar (schema 29).</summary>
public class HatchFreezeAuditBarLaterTests4
{
    [Fact] public void Q8806_freezaud4_1536() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8807_freezaud4_1537() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68583 })).State.Schema); }
    [Fact] public void Q8808_freezaud4_1538() { var g = Game.Solo(seed: 68482); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8809_freezaud4_1539() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8810_freezaud4_1540() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8811_freezaud4_1541() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68587 })).State.Schema); }
    [Fact] public void Q8812_freezaud4_1542() { var g = Game.Solo(seed: 68486); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8813_freezaud4_1543() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8814_freezaud4_1544() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8815_freezaud4_1545() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68591 })).State.Schema); }
    [Fact] public void Q8816_freezaud4_1546() { var g = Game.Solo(seed: 68490); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8817_freezaud4_1547() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8818_freezaud4_1548() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8819_freezaud4_1549() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68595 })).State.Schema); }
    [Fact] public void Q8820_freezaud4_1550() { var g = Game.Solo(seed: 68494); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8821_freezaud4_1551() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8822_freezaud4_1552() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8823_freezaud4_1553() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68599 })).State.Schema); }
    [Fact] public void Q8824_freezaud4_1554() { var g = Game.Solo(seed: 68498); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8825_freezaud4_1555() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8826_freezaud4_1556() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8827_freezaud4_1557() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68603 })).State.Schema); }
    [Fact] public void Q8828_freezaud4_1558() { var g = Game.Solo(seed: 68502); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8829_freezaud4_1559() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8830_freezaud4_1560() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8831_freezaud4_1561() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68607 })).State.Schema); }
    [Fact] public void Q8832_freezaud4_1562() { var g = Game.Solo(seed: 68506); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8833_freezaud4_1563() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8834_freezaud4_1564() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8835_freezaud4_1565() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68611 })).State.Schema); }
    [Fact] public void Q8836_freezaud4_1566() { var g = Game.Solo(seed: 68510); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8837_freezaud4_1567() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8838_freezaud4_1568() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8839_freezaud4_1569() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68615 })).State.Schema); }
    [Fact] public void Q8840_freezaud4_1570() { var g = Game.Solo(seed: 68514); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8841_freezaud4_1571() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8842_freezaud4_1572() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8843_freezaud4_1573() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68619 })).State.Schema); }
    [Fact] public void Q8844_freezaud4_1574() { var g = Game.Solo(seed: 68518); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8845_freezaud4_1575() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8846_freezaud4_1576() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8847_freezaud4_1577() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68623 })).State.Schema); }
    [Fact] public void Q8848_freezaud4_1578() { var g = Game.Solo(seed: 68522); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8849_freezaud4_1579() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8850_freezaud4_1580() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8851_freezaud4_1581() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68627 })).State.Schema); }
    [Fact] public void Q8852_freezaud4_1582() { var g = Game.Solo(seed: 68526); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8853_freezaud4_1583() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8854_freezaud4_1584() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8855_freezaud4_1585() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68631 })).State.Schema); }
    [Fact] public void Q8856_freezaud4_1586() { var g = Game.Solo(seed: 68530); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8857_freezaud4_1587() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8858_freezaud4_1588() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8859_freezaud4_1589() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68635 })).State.Schema); }
    [Fact] public void Q8860_freezaud4_1590() { var g = Game.Solo(seed: 68534); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8861_freezaud4_1591() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8862_freezaud4_1592() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8863_freezaud4_1593() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68639 })).State.Schema); }
    [Fact] public void Q8864_freezaud4_1594() { var g = Game.Solo(seed: 68538); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8865_freezaud4_1595() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8866_freezaud4_1596() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8867_freezaud4_1597() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68643 })).State.Schema); }
    [Fact] public void Q8868_freezaud4_1598() { var g = Game.Solo(seed: 68542); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8869_freezaud4_1599() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8870_freezaud4_1600() { var g = Game.Solo(tweak: c => { foreach (var r in c.Funding.Rounds) r.OpensYear = 1995; c.Funding.Round("angel").MinReputation = 0; }); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen); }
    [Fact] public void Q8871_freezaud4_1601() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68647 })).State.Schema); }
    [Fact] public void Q8872_freezaud4_1602() { var g = Game.Solo(seed: 68546); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q8873_freezaud4_1603() { GameConfig c = TestData.Config(); Assert.True(c.World.Attention.AuditAt > 0); Assert.True(c.World.Attention.FreezeAt > 0); }
    [Fact] public void Q8874_freezaud4_1604() { var g = Game.Solo().WithStarterLab(); g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze; g.Me.Cash = 10_000_000; g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen); }
    [Fact] public void Q8875_freezaud4_1605() { GameConfig c = TestData.Config(); Assert.True(c.Shady.AuditInvestigationPermille >= 0); Assert.Equal(GameState.CurrentSchema, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 68651 })).State.Schema); }
}
