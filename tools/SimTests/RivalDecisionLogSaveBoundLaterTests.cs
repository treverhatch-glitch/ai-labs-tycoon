using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Rival decision log trim, save, and host slices (Q25287–Q25296).</summary>
public class RivalDecisionLogSaveBoundLaterTests
{
    static Game WithRival(int logMax = 12, long seed = 25287)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string> { "openaye", "anthoptics" } };
        return new Game(Simulation.NewGame(TestData.Config(c => c.Rivalry.LogMax = logMax), options));
    }

    [Fact]
    public void Q25287_trim_drops_oldest_over_log_max()
    {
        Game g = WithRival(6);
        LabState rival = g.Rival("openaye");
        for (int i = 0; i < 10; i++)
            DecisionLog.Add(g.C, g.State, rival, i, DecisionAction.Paused, i.ToString(), 0);
        Assert.Equal(6, rival.Log.Count);
        Assert.Equal("4", rival.Log[0].Ref);
    }

    [Fact]
    public void Q25288_save_restores_newest_log_tail()
    {
        Game g = WithRival(20);
        LabState rival = g.Rival("openaye");
        DecisionLog.Add(g.C, g.State, rival, 5, DecisionAction.Lobbied, "us", 1);
        string json = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, json));
        LabState back = loaded.Rival("openaye");
        Assert.Single(back.Log);
        Assert.Equal(DecisionAction.Lobbied, back.Log[0].Action);
    }

    [Fact]
    public void Q25289_each_rival_log_bounded_independently()
    {
        Game g = WithRival(5);
        LabState a = g.Rival("openaye");
        LabState b = g.Rival("anthoptics");
        for (int i = 0; i < 8; i++)
        {
            DecisionLog.Add(g.C, g.State, a, i, DecisionAction.Paused, "a", 0);
            DecisionLog.Add(g.C, g.State, b, i, DecisionAction.Paused, "b", 0);
        }
        Assert.Equal(5, a.Log.Count);
        Assert.Equal(5, b.Log.Count);
    }

    [Fact]
    public void Q25290_vendor_log_bounded_separate_from_rivals()
    {
        GameConfig c = TestData.Config(c => c.Vendor.LogMax = 4);
        var g = new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25290 }));
        for (int i = 0; i < 6; i++)
            DecisionLog.AddVendor(g.C, g.State, DecisionAction.ShiftedAllocation, "openaye", i);
        Assert.Equal(4, g.State.Vendor.Log.Count);
        Assert.Empty(g.Me.Log);
    }

    [Fact]
    public void Q25291_slices_match_log_after_trim()
    {
        Game g = WithRival(10);
        LabState rival = g.Rival("openaye");
        for (int i = 0; i < 15; i++)
            DecisionLog.Add(g.C, g.State, rival, i, DecisionAction.SkippedRun, i.ToString(), 0);
        var slices = DisplayQueries.RivalDecisionSlices(g.C, g.State, rival, 3);
        var lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, 3);
        Assert.Equal(3, slices.Count);
        Assert.Equal(3, lines.Count);
        Assert.Equal("12", slices[0].Ref);
    }

    [Fact]
    public void Q25292_logged_this_quarter_dedupes_same_action_ref()
    {
        Game g = WithRival(100);
        LabState rival = g.Rival("openaye");
        int tick = g.C.Calendar.WeeksPerMonth * 3;
        Assert.False(DecisionLog.LoggedThisQuarter(g.C, rival, DecisionAction.SkippedRun, "line-1", tick));
        DecisionLog.Add(g.C, g.State, rival, tick, DecisionAction.SkippedRun, "line-1", 0);
        Assert.True(DecisionLog.LoggedThisQuarter(g.C, rival, DecisionAction.SkippedRun, "line-1", tick + 1));
    }

    [Fact]
    public void Q25293_planner_weeks_stay_under_config_log_max()
    {
        GameConfig c = TestData.Config(c => c.Rivalry.LogMax = 80);
        Game g = new Game(Simulation.NewGame(c, 25293));
        g.Weeks(52);
        foreach (LabState lab in g.State.Labs)
            if (lab.Controller == LabController.Plan)
                Assert.True(lab.Log.Count <= c.Rivalry.LogMax);
    }

    [Fact]
    public void Q25294_decision_text_stable_after_save_load()
    {
        Game g = WithRival(30);
        LabState rival = g.Rival("openaye");
        DecisionLog.Add(g.C, g.State, rival, 42, DecisionAction.Lobbied, "us", 3);
        DecisionEntry e = rival.Log[^1];
        string before = DecisionText.Line(g.C, g.State, rival, e);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState back = loaded.Rival("openaye");
        Assert.Equal(before, DecisionText.Line(loaded.C, loaded.State, back, back.Log[^1]));
    }

    [Fact]
    public void Q25295_multi_rival_log_counts_persist_on_save()
    {
        Game g = WithRival(50);
        g.Weeks(30);
        var counts = g.State.Labs.Where(l => l.Controller == LabController.Plan).Select(l => l.Log.Count).ToList();
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var back = loaded.State.Labs.Where(l => l.Controller == LabController.Plan).Select(l => l.Log.Count).ToList();
        Assert.Equal(counts, back);
    }

    [Fact]
    public void Q25296_player_never_accumulates_planner_log_on_save()
    {
        var g = Game.Solo();
        DecisionLog.Add(g.C, g.State, g.Me, g.Tick, DecisionAction.Paused, "", 0);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Empty(loaded.Me.Log);
    }
}