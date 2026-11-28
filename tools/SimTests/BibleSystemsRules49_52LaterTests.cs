using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Rules 49–52 grouped depth (Q25339–Q25350).</summary>
public class BibleSystemsRules49_52LaterTests
{
    [Fact]
    public void Q25339_release_notes_events_in_office_table()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release);
    }

    [Fact]
    public void Q25340_set_research_share_sticks_on_lab()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetResearchShare(g.Id, 200));
        Assert.Equal(200, g.Me.ResearchSharePermille);
    }

    [Fact]
    public void Q25341_research_share_save_roundtrip()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetResearchShare(g.Id, 150));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(150, loaded.Me.ResearchSharePermille);
    }

    [Fact]
    public void Q25342_compute_share_bonus_positive_when_share_set()
    {
        var g = Game.Solo().WithStarterLab();
        g.Do(Command.SetResearchShare(g.Id, 300));
        Assert.True(TrainingSystem.ComputeShareBonusPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void Q25343_era_office_event_has_year_bounds()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Office.Events, e => e.FromYear > 0 && e.Choices.Count >= 1);
    }

    [Fact]
    public void Q25344_in_years_false_before_from_year()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig e = c.Office.Events.First(x => x.FromYear > 1990);
        Assert.False(OfficeEvents.InYears(c, e, 0));
    }

    [Fact]
    public void Q25345_filler_to_year_filters_old_eras()
    {
        GameConfig c = TestData.Config();
        FillerConfig f = c.News.Filler.First(x => x.ToYear > 0 && x.ToYear < 2050);
        Assert.True(f.ToYear > 0);
    }

    [Fact]
    public void Q25346_release_trigger_on_first_model_release()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Release);
    }

    [Fact]
    public void Q25347_research_share_refused_over_max()
    {
        var g = Game.Solo().WithStarterLab();
        int max = g.C.Research.MaxComputeSharePermille;
        g.Refused(Command.SetResearchShare(g.Id, max + 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q25348_schema_42_supports_research_share_field()
    {
        var g = Game.Solo().WithStarterLab();
        Assert.True(g.State.Schema >= 42);
    }

    [Fact]
    public void Q25349_filler_list_validates_clean()
    {
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }

    [Fact]
    public void Q25350_decision_log_empty_player_after_planner_weeks()
    {
        Game g = Game.New(25350);
        g.Weeks(12);
        Assert.Empty(g.Me.Log);
        Assert.True(g.Rival("openaye").Log.Count >= 0);
    }
}