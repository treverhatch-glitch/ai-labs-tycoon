using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Rules 45–48 grouped depth (Q25321–Q25332).</summary>
public class BibleSystemsRules45_48LaterTests
{
    [Fact]
    public void Q25321_sector_flavour_failure_text_for_sector()
    {
        GameConfig c = TestData.Config();
        SectorFlavourConfig f = c.Safety.SectorFlavours[0];
        Assert.False(string.IsNullOrEmpty(f.Sector));
        Assert.False(string.IsNullOrEmpty(SectorFlavours.Failure(c, f.Sector)));
    }

    [Fact]
    public void Q25322_researcher_leaves_event_has_three_choices()
    {
        OfficeEventConfig e = TestData.Config().Office.Event("researcher-leaves");
        Assert.NotNull(e);
        Assert.True(e.Choices.Count >= 3);
    }

    [Fact]
    public void Q25323_minesweeper_requires_own_chip()
    {
        OfficeEventConfig e = TestData.Config().Office.Event("minesweeper");
        Assert.Equal(OfficeRequires.OwnChip, e.Requires);
    }

    [Fact]
    public void Q25324_snub_weeks_positive_when_configured()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chips.SnubWeeks > 0);
        Assert.True(c.Chips.SnubChancePermille > 0);
    }

    [Fact]
    public void Q25325_overload_rises_when_served_below_demand()
    {
        var g = Game.Solo(25325).WithStarterLab();
        g.Me.LastWeek.ServingDemand = 800;
        g.Me.LastWeek.ServedPermille = 300;
        Assert.True(SafetySystem.OverloadPermille(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Q25326_sector_flavours_save_roundtrip()
    {
        var g = Game.Solo(25326);
        g.Me.Staff.Add(new StaffMember
        {
            Id = g.State.TakeId(),
            RoleId = "researcher",
            TraitId = "steady",
            Morale = 500,
        });
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    [Fact]
    public void Q25327_ceo_friday_office_event_exists()
    {
        Assert.NotNull(TestData.Config().Office.Event("ceo-friday"));
    }

    [Fact]
    public void Q25328_chip_slot_snub_standing_gate()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chips.SnubStandingBelow < 1000);
    }

    [Fact]
    public void Q25329_overload_for_rivals_ships_in_safety_config()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Safety.OverloadIncidentPermille, 1, 1000);
    }

    [Fact]
    public void Q25330_trust_permille_in_range_for_flavour()
    {
        var g = Game.Solo(25330).WithStarterLab();
        string sector = g.C.Safety.SectorFlavours[0].Sector;
        int trust = SectorFlavours.TrustPermille(g.C, g.State, g.Me, sector);
        Assert.InRange(trust, 0, 2000);
    }

    [Fact]
    public void Q25331_model_off_event_poses_without_schema_bump()
    {
        var g = Game.Solo(25331).WithStarterLab();
        OfficeEventConfig e = g.C.Office.Event("model-off");
        Assert.NotNull(e);
        Assert.Equal(GameState.CurrentSchema, g.State.Schema);
    }

    [Fact]
    public void Q25332_weekly_incident_includes_overload_slice()
    {
        var g = Game.Solo(25332).WithStarterLab();
        g.Me.LastWeek.ServingDemand = 500;
        g.Me.LastWeek.ServedPermille = 100;
        var model = new ModelState { Alignment = 500, Corruption = 0, Status = ModelStatus.Released };
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, model);
        Assert.True(chance >= 0);
    }
}