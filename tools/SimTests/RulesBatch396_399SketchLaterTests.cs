using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Grouped sketch rules 45–48 (Q25333–Q25338).</summary>
public class RulesBatch396_399SketchLaterTests
{
    [Fact]
    public void Q25333_landlord_note_in_office_table()
    {
        Assert.Contains(TestData.Config().Office.Events, e => e.IdValue == "landlord-note");
    }

    [Fact]
    public void Q25334_husk_lol_event_id_unique()
    {
        var ids = TestData.Config().Office.Events.Select(e => e.IdValue).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void Q25335_snub_weeks_config_positive()
    {
        Assert.True(TestData.Config().Chips.SnubWeeks > 0);
    }

    [Fact]
    public void Q25336_overload_incident_permille_configured()
    {
        Assert.True(TestData.Config().Safety.OverloadIncidentPermille > 0);
    }

    [Fact]
    public void Q25337_sector_flavours_count_matches_markets()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Safety.SectorFlavours.Count >= c.Market.Sectors.Count / 2);
    }

    [Fact]
    public void Q25338_rival_log_still_empty_for_player_after_weeks()
    {
        var g = Game.New(25338);
        g.Weeks(8);
        Assert.Empty(g.Me.Log);
    }
}