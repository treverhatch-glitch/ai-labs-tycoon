using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT rules 38–39: experimental runs and engineer save (batch 389–390).</summary>
public class HostExplainNext388LaterTests
{
    [Fact]
    public void Run_approaches_and_checkpoints_ship_in_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Runs.Approaches.Count >= 2);
        Assert.True(c.Runs.Checkpoints.Count >= 1);
        Assert.False(string.IsNullOrEmpty(c.Runs.DefaultApproachId));
    }

    [Fact]
    public void Engineer_saving_permille_rises_with_engineers()
    {
        var g = Game.Solo().WithStarterLab(engineers: 1);
        int save0 = TrainingRuns.EngineerSavingPermille(g.C, g.Me);
        g.Build("engineering-bay", 4, 4);
        g.Hire("engineer", 2);
        int save1 = TrainingRuns.EngineerSavingPermille(g.C, g.Me);
        Assert.True(save1 >= save0);
    }

    [Fact]
    public void Hearing_pool_includes_triggered_office_events()
    {
        GameConfig c = TestData.Config();
        int hear = 0;
        foreach (OfficeEventConfig e in c.Office.Events)
            if (e.Trigger == OfficeTrigger.Hearing) hear++;
        Assert.True(hear >= 1);
    }

    [Fact]
    public void Rival_log_max_bounds_planner_entries_on_save()
    {
        var g = Game.New(388, c => c.Rivalry.LogMax = 8);
        LabState rival = g.Rival("openaye");
        for (int i = 0; i < 12; i++)
            DecisionLog.Add(g.C, g.State, rival, i, DecisionAction.Paused, i.ToString(), 0);
        Assert.Equal(8, rival.Log.Count);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        LabState back = loaded.Rival("openaye");
        Assert.Equal(8, back.Log.Count);
    }
}