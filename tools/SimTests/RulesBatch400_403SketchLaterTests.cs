using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Grouped sketch rules 49–52 (Q25351–Q25356).</summary>
public class RulesBatch400_403SketchLaterTests
{
    [Fact]
    public void Q25351_release_office_event_has_choices()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig rel = c.Office.Events.First(e => e.Trigger == OfficeTrigger.Release);
        Assert.True(rel.Choices.Count >= 1);
    }

    [Fact]
    public void Q25352_max_compute_share_permille_in_range()
    {
        Assert.InRange(TestData.Config().Research.MaxComputeSharePermille, 1, 1000);
    }

    [Fact]
    public void Q25353_office_from_year_non_negative()
    {
        Assert.All(TestData.Config().Office.Events, e => Assert.True(e.FromYear >= 0));
    }

    [Fact]
    public void Q25354_news_filler_distinct_ids()
    {
        var ids = TestData.Config().News.Filler.Select(f => f.IdValue).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void Q25355_set_research_share_command_kind()
    {
        var g = Game.Solo();
        Assert.Equal(CommandKind.SetResearchShare, Command.SetResearchShare(g.Id, 100).Kind);
    }

    [Fact]
    public void Q25356_display_incident_overload_slice_available()
    {
        var g = Game.Solo().WithStarterLab();
        var model = new ModelState { Alignment = 500, Corruption = 0, Status = ModelStatus.Released };
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.True(b.FromOverload >= 0);
    }
}