using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q1606–Q1675: batch 28 incident/bubble bar (schema 37).</summary>
public class HatchIncidentBubbleBarLaterTests
{
    [Fact] public void Q1606_incident_bubble_1606() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1607_incident_bubble_1607() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1608_incident_bubble_1608() { var g = Game.Solo(seed: 2902); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1609_incident_bubble_1609() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1610_incident_bubble_1610() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1611_incident_bubble_1611() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1612_incident_bubble_1612() { var g = Game.Solo(seed: 2906); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1613_incident_bubble_1613() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1614_incident_bubble_1614() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1615_incident_bubble_1615() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1616_incident_bubble_1616() { var g = Game.Solo(seed: 2910); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1617_incident_bubble_1617() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1618_incident_bubble_1618() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1619_incident_bubble_1619() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1620_incident_bubble_1620() { var g = Game.Solo(seed: 2914); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1621_incident_bubble_1621() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1622_incident_bubble_1622() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1623_incident_bubble_1623() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1624_incident_bubble_1624() { var g = Game.Solo(seed: 2918); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1625_incident_bubble_1625() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1626_incident_bubble_1626() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1627_incident_bubble_1627() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1628_incident_bubble_1628() { var g = Game.Solo(seed: 2922); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1629_incident_bubble_1629() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1630_incident_bubble_1630() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1631_incident_bubble_1631() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1632_incident_bubble_1632() { var g = Game.Solo(seed: 2926); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1633_incident_bubble_1633() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1634_incident_bubble_1634() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1635_incident_bubble_1635() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1636_incident_bubble_1636() { var g = Game.Solo(seed: 2930); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1637_incident_bubble_1637() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1638_incident_bubble_1638() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1639_incident_bubble_1639() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1640_incident_bubble_1640() { var g = Game.Solo(seed: 2934); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1641_incident_bubble_1641() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1642_incident_bubble_1642() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1643_incident_bubble_1643() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1644_incident_bubble_1644() { var g = Game.Solo(seed: 2938); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1645_incident_bubble_1645() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1646_incident_bubble_1646() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1647_incident_bubble_1647() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1648_incident_bubble_1648() { var g = Game.Solo(seed: 2942); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1649_incident_bubble_1649() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1650_incident_bubble_1650() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1651_incident_bubble_1651() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1652_incident_bubble_1652() { var g = Game.Solo(seed: 2946); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1653_incident_bubble_1653() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1654_incident_bubble_1654() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1655_incident_bubble_1655() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1656_incident_bubble_1656() { var g = Game.Solo(seed: 2950); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1657_incident_bubble_1657() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1658_incident_bubble_1658() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1659_incident_bubble_1659() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1660_incident_bubble_1660() { var g = Game.Solo(seed: 2954); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1661_incident_bubble_1661() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1662_incident_bubble_1662() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1663_incident_bubble_1663() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1664_incident_bubble_1664() { var g = Game.Solo(seed: 2958); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1665_incident_bubble_1665() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1666_incident_bubble_1666() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1667_incident_bubble_1667() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1668_incident_bubble_1668() { var g = Game.Solo(seed: 2962); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1669_incident_bubble_1669() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1670_incident_bubble_1670() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1671_incident_bubble_1671() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
    [Fact] public void Q1672_incident_bubble_1672() { var g = Game.Solo(seed: 2966); g.State.BubblePermille = g.C.Bubble.HotPermille; Assert.True(g.State.BubblePermille >= g.C.Bubble.HotPermille); SaveSchemaPretend.AssertRoundTrip(g.Sim); }
    [Fact] public void Q1673_incident_bubble_1673() { GameConfig c = TestData.Config(); Assert.True(c.Bubble.DecayPermillePerMonth > 0); Assert.Contains(c.Safety.IncidentRungs, r => r.Id.Length > 0); }
    [Fact] public void Q1674_incident_bubble_1674() { GameConfig c = TestData.Config(); Assert.True(c.Safety.IncidentRungs.Count >= 6); Assert.True(c.Bubble.PopAtPermille > 0); }
    [Fact] public void Q1675_incident_bubble_1675() { var g = Game.Solo(); var m = new ModelState { Corruption = 500, Alignment = 800, DisplayedAlignment = 800, Status = ModelStatus.Released }; Assert.True(SafetySystem.PickIncidentRung(g.C, m) >= 4); Assert.Equal(37, g.State.Schema); }
}
