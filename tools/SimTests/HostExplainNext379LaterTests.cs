using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT #19–#22 explain: training objectives, successor design, rushed launch, news ticker (batch 379).</summary>
public class HostExplainNext379LaterTests
{
    [Fact]
    public void Training_objective_appeal_permille_tracks_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Objectives.Objectives.Count >= 1);
        var g = Game.Solo().WithStarterLab();
        ModelState m = g.Train("mini", 0);
        Assert.Equal(MathI.Permille, TrainingObjectives.AppealPermille(c, m));
    }

    [Fact]
    public void Successor_design_compute_saved_permille_exposed()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Successor.ComputeSavedPermille > 0);
        Assert.True(c.Successor.AlignmentPoints > 0);
    }

    [Fact]
    public void Rushed_launch_week_bounds_and_incident_permille()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Rush.MaxWeeks > 0);
        Assert.True(RushedLaunch.IncidentPermille(c, 0, new ModelState { RushWeeks = c.Rush.MaxWeeks }) > 0);
    }

    [Fact]
    public void News_ticker_headline_count_and_pick_filler()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.News.Headlines.Count >= 424);
        var g = Game.Solo().WithStarterLab();
        List<Headline> ticker = NewsTicker.Headlines(c, g.State, 3);
        Assert.True(ticker.Count >= 1);
        Assert.False(string.IsNullOrEmpty(ticker[0].Text));
    }
}
