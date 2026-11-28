using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Behavioral hardening for rival intent on the ticker (Q25257, batch seventeen tail).</summary>
public class RivalIntentNewsLaterTests
{
    static LabState PlanRival(Game g) => g.State.Labs.Find(l => l.Controller == LabController.Plan);

    [Fact]
    public void Q25257_disabled_config_logs_but_does_not_surface()
    {
        Game g = Game.New(252570, c => c.RivalIntentNews.Enabled = 0);
        LabState rival = PlanRival(g);
        DecisionLog.Add(g.C, g.State, rival, g.Tick, DecisionAction.Paused, "hold", 0);
        Assert.Single(rival.Log);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced);
    }

    [Fact]
    public void Q25258_lobbied_marks_pick_on_surfaced_event()
    {
        Game g = Game.New(252571, c => c.RivalIntentNews.MinWeeksBetween = 0);
        LabState rival = PlanRival(g);
        DecisionLog.Add(g.C, g.State, rival, g.Tick, DecisionAction.Lobbied, "us", 1);
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.RivalIntentSurfaced);
        Assert.Equal(rival.Id, ev.LabId);
        Assert.Equal(1, ev.Pick);
        Assert.Equal((int)DecisionAction.Lobbied, ev.SubjectId);
    }

    [Fact]
    public void Q25259_global_throttle_blocks_second_rival_within_gap()
    {
        var options = new NewGameOptions
        {
            Seed = 252572,
            RivalIds = new List<string> { "openaye", "anthoptics" },
        };
        Game g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.RivalIntentNews.MinWeeksBetween = 6;
        }), options));
        var rivals = g.State.Labs.Where(l => l.Controller == LabController.Plan).ToList();
        Assert.Equal(2, rivals.Count);
        DecisionLog.Add(g.C, g.State, rivals[0], 0, DecisionAction.Paused, "a", 0);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced);
        DecisionLog.Add(g.C, g.State, rivals[1], 2, DecisionAction.Paused, "b", 0);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced);
        DecisionLog.Add(g.C, g.State, rivals[1], 6, DecisionAction.Paused, "c", 0);
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced));
    }

    [Fact]
    public void Q25260_vendor_log_never_surfaces_rival_intent()
    {
        Game g = Game.Solo(252573, c => c.RivalIntentNews.MinWeeksBetween = 0);
        DecisionLog.AddVendor(g.C, g.State, DecisionAction.ShiftedAllocation, "openaye", 4);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced);
    }

    [Fact]
    public void Q25261_save_round_trip_preserves_throttle_tick()
    {
        Game g = Game.New(252574, c => c.RivalIntentNews.MinWeeksBetween = 8);
        LabState rival = PlanRival(g);
        g.State.Tick = 10;
        DecisionLog.Add(g.C, g.State, rival, 10, DecisionAction.Paused, "", 0);
        Assert.Equal(10, g.State.RivalIntentLastTick);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Game loaded = new Game(again);
        Assert.Equal(10, loaded.State.RivalIntentLastTick);
        int surfaced = loaded.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced);
        loaded.State.Tick = 12;
        DecisionLog.Add(loaded.C, loaded.State, PlanRival(loaded), 12, DecisionAction.Paused, "", 0);
        Assert.Equal(surfaced, loaded.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced));
        loaded.State.Tick = 18;
        DecisionLog.Add(loaded.C, loaded.State, PlanRival(loaded), 18, DecisionAction.Paused, "", 0);
        Assert.Equal(surfaced + 1, loaded.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced));
    }

    [Fact]
    public void Q25262_surfaced_ref_matches_decision_reference()
    {
        Game g = Game.New(252575, c => c.RivalIntentNews.MinWeeksBetween = 0);
        LabState rival = PlanRival(g);
        DecisionLog.Add(g.C, g.State, rival, g.Tick, DecisionAction.SkippedRun, "line-4", 0);
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.RivalIntentSurfaced);
        Assert.Equal("line-4", ev.Ref);
        Assert.Equal((int)DecisionAction.SkippedRun, ev.SubjectId);
        Assert.Contains(g.C.News.Headlines, h => h.Kind == EventKind.RivalIntentSurfaced.ToString());
    }
}