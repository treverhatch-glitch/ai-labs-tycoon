using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1466–Q1535: batch 26 shady/china data bar (schema 42).</summary>
public class HatchShadyChinaBarLaterTests
{
    static readonly string[] Cards = { "intrusion-for-hire", "liberate-funds", "sabotage-run", "hack-government" };

    [Fact] public void Q1466_shady_china_1466() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[0]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1467_shady_china_1467() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[0], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1468_shady_china_1468() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2600 })).Sim); }
    [Fact] public void Q1469_shady_china_1469() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[0]); }
    [Fact] public void Q1470_shady_china_1470() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[1]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1471_shady_china_1471() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[1], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1472_shady_china_1472() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2604 })).Sim); }
    [Fact] public void Q1473_shady_china_1473() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[1]); }
    [Fact] public void Q1474_shady_china_1474() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[2]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1475_shady_china_1475() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[2], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1476_shady_china_1476() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2608 })).Sim); }
    [Fact] public void Q1477_shady_china_1477() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[2]); }
    [Fact] public void Q1478_shady_china_1478() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[3]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1479_shady_china_1479() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[3], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1480_shady_china_1480() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineAttention >= 0); Assert.Equal(43, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2612 })).State.Schema); }
    [Fact] public void Q1481_shady_china_1481() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[3]); }
    [Fact] public void Q1482_shady_china_1482() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[0]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1483_shady_china_1483() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[0], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1484_shady_china_1484() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2616 })).Sim); }
    [Fact] public void Q1485_shady_china_1485() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[0]); }
    [Fact] public void Q1486_shady_china_1486() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[1]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1487_shady_china_1487() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[1], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1488_shady_china_1488() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2620 })).Sim); }
    [Fact] public void Q1489_shady_china_1489() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[1]); }
    [Fact] public void Q1490_shady_china_1490() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[2]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1491_shady_china_1491() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[2], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1492_shady_china_1492() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2624 })).Sim); }
    [Fact] public void Q1493_shady_china_1493() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[2]); }
    [Fact] public void Q1494_shady_china_1494() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[3]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1495_shady_china_1495() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[3], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1496_shady_china_1496() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineAttention >= 0); Assert.Equal(43, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2628 })).State.Schema); }
    [Fact] public void Q1497_shady_china_1497() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[3]); }
    [Fact] public void Q1498_shady_china_1498() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[0]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1499_shady_china_1499() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[0], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1500_shady_china_1500() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2632 })).Sim); }
    [Fact] public void Q1501_shady_china_1501() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[0]); }
    [Fact] public void Q1502_shady_china_1502() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[1]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1503_shady_china_1503() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[1], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1504_shady_china_1504() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2636 })).Sim); }
    [Fact] public void Q1505_shady_china_1505() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[1]); }
    [Fact] public void Q1506_shady_china_1506() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[2]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1507_shady_china_1507() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[2], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1508_shady_china_1508() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2640 })).Sim); }
    [Fact] public void Q1509_shady_china_1509() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[2]); }
    [Fact] public void Q1510_shady_china_1510() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[3]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1511_shady_china_1511() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[3], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1512_shady_china_1512() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineAttention >= 0); Assert.Equal(43, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2644 })).State.Schema); }
    [Fact] public void Q1513_shady_china_1513() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[3]); }
    [Fact] public void Q1514_shady_china_1514() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[0]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1515_shady_china_1515() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[0], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1516_shady_china_1516() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2648 })).Sim); }
    [Fact] public void Q1517_shady_china_1517() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[0]); }
    [Fact] public void Q1518_shady_china_1518() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[1]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1519_shady_china_1519() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[1], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1520_shady_china_1520() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.MinInstalledKw > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2652 })).Sim); }
    [Fact] public void Q1521_shady_china_1521() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[1]); }
    [Fact] public void Q1522_shady_china_1522() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[2]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1523_shady_china_1523() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[2], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1524_shady_china_1524() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.ActivePowerPermille > 0); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2656 })).Sim); }
    [Fact] public void Q1525_shady_china_1525() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[2]); }
    [Fact] public void Q1526_shady_china_1526() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[3]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1527_shady_china_1527() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[3], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1528_shady_china_1528() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineAttention >= 0); Assert.Equal(43, new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 2660 })).State.Schema); }
    [Fact] public void Q1529_shady_china_1529() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[3]); }
    [Fact] public void Q1530_shady_china_1530() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[0]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1531_shady_china_1531() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[0], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
    [Fact] public void Q1532_shady_china_1532() { GameConfig c = TestData.Config(); Assert.True(c.World.ChinaDeals.DeclineCooldownMonths >= 1); SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2664 })).Sim); }
    [Fact] public void Q1533_shady_china_1533() { GameConfig c = TestData.Config(); Assert.Contains(c.Shady.Cards, x => x.Id == Cards[0]); }
    [Fact] public void Q1534_shady_china_1534() { GameConfig c = TestData.Config(); ShadyCardConfig card = c.Shady.Card(Cards[1]); Assert.True(card.Weeks > 0); Assert.True(card.SuccessPermille > 0); }
    [Fact] public void Q1535_shady_china_1535() { Game g = Game.New(); LabState r = g.Rival("openaye"); r.Models.Add(new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained }); g.Refused(Command.StartShadyOp(r.Id, Cards[1], r.Models[0].Id, -1, -1, ""), CommandError.InvalidArgument); }
}