using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT rules 45–48: sector flavours, office r3, foundry snub, overload (batch 396–399).</summary>
public class HostExplainNext391LaterTests
{
    [Fact]
    public void Rivals_sector_flavours_switch_ships_on()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Safety.SectorFlavoursForRivals);
        Assert.True(c.Safety.SectorFlavours.Count >= 1);
    }

    [Fact]
    public void Office_round_three_includes_landlord_note()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig note = c.Office.Events.First(e => e.IdValue == "landlord-note");
        Assert.False(string.IsNullOrEmpty(note.Text));
        Assert.True(note.Choices.Count >= 1);
    }

    [Fact]
    public void Foundry_snub_thresholds_exposed_in_chip_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chips.SnubStandingBelow > 0);
        Assert.InRange(c.Chips.SnubChancePermille, 1, 1000);
    }

    [Fact]
    public void Overload_incident_permille_contributes_to_breakdown()
    {
        var g = Game.Solo().WithStarterLab();
        g.Me.LastWeek.ServingDemand = 1000;
        g.Me.LastWeek.ServedPermille = 400;
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.True(b.FromOverload >= 0);
        Assert.Equal(SafetySystem.OverloadPermille(g.C, g.State, g.Me), b.FromOverload);
    }
}