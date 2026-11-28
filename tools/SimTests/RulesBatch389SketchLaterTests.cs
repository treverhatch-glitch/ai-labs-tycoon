using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>BIG LOCAL batch: rules 38–40 sketch (Q25281–Q25286).</summary>
public class RulesBatch389SketchLaterTests
{
    [Fact]
    public void Q25281_experimental_approach_on_training_model()
    {
        var g = Game.Solo().WithStarterLab();
        var m = g.Train("mini", 0);
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        Assert.Equal("experimental", m.Approach);
    }

    [Fact]
    public void Q25282_checkpoints_stick_on_model()
    {
        var g = Game.Solo().WithStarterLab();
        var m = g.Train("mini", 0);
        const string cp = "often";
        g.Do(Command.SetCheckpoints(g.Id, m.Id, cp));
        Assert.Equal(cp, m.Checkpoints);
    }

    [Fact]
    public void Q25283_engineer_saving_never_exceeds_cap()
    {
        var g = Game.Solo().WithStarterLab(engineers: 2);
        Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) <= g.C.Runs.EngineerSavingCapPermille);
    }

    [Fact]
    public void Q25284_planner_rival_log_survives_save()
    {
        Game g = Game.New(25284, c => c.Rivalry.LogMax = 64);
        LabState rival = g.Rival("openaye");
        int before = rival.Log.Count;
        g.Weeks(24);
        Assert.True(rival.Log.Count > before);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(loaded.Rival("openaye").Log.Count, rival.Log.Count);
    }

    [Fact]
    public void Q25285_player_lab_never_gets_decision_log()
    {
        var g = Game.Solo();
        DecisionLog.Add(g.C, g.State, g.Me, g.Tick, DecisionAction.Paused, "", 0);
        Assert.Empty(g.Me.Log);
    }

    [Fact]
    public void Q25286_hearing_events_present_in_office_config()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Hearing);
    }
}