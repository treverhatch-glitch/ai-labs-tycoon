using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT rules 49–52: release notes, research share, era office, era filler (batch 400–403).</summary>
public class HostExplainNext392LaterTests
{
    [Fact]
    public void Release_trigger_events_ship_in_office_config()
    {
        GameConfig c = TestData.Config();
        int rel = c.Office.Events.Count(e => e.Trigger == OfficeTrigger.Release);
        Assert.True(rel >= 3);
    }

    [Fact]
    public void Research_share_defaults_zero_until_set()
    {
        var g = Game.Solo().WithStarterLab();
        Assert.Equal(0, g.Me.ResearchSharePermille);
        Assert.True(g.C.Research.MaxComputeSharePermille > 0);
    }

    [Fact]
    public void Era_gated_office_events_use_year_window()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig gated = c.Office.Events.First(e => e.FromYear > 0 || e.ToYear > 0);
        var g = Game.Solo();
        Assert.True(OfficeEvents.InYears(g.C, gated, g.Tick));
    }

    [Fact]
    public void Filler_headlines_respect_to_year()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.News.Filler, f => f.ToYear > 0);
        Assert.True(c.News.Filler.Count >= 10);
    }
}