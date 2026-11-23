using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class RivalIntentNewsTests
{
    [Fact]
    public void Rival_decision_emits_ticker_event()
    {
        Game g = Game.New(12, c => { c.RivalIntentNews.Enabled = 1; c.RivalIntentNews.MinWeeksBetween = 0; });
        LabState rival = g.State.Labs.Find(l => l.Controller == LabController.Plan);
        Assert.NotNull(rival);
        DecisionLog.Add(g.C, g.State, rival, g.Tick, DecisionAction.Paused, "", 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced && e.LabId == rival.Id);
    }

    [Fact]
    public void Player_decisions_do_not_emit()
    {
        Game g = Game.Solo(13, c => c.RivalIntentNews.Enabled = 1);
        DecisionLog.Add(g.C, g.State, g.Me, g.Tick, DecisionAction.Paused, "", 0);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RivalIntentSurfaced);
    }

    [Fact]
    public void Throttle_respects_min_weeks()
    {
        Game g = Game.New(14, c => { c.RivalIntentNews.Enabled = 1; c.RivalIntentNews.MinWeeksBetween = 8; });
        LabState rival = g.State.Labs.Find(l => l.Controller == LabController.Plan);
        DecisionLog.Add(g.C, g.State, rival, g.Tick, DecisionAction.Paused, "x", 0);
        int n = g.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced);
        DecisionLog.Add(g.C, g.State, rival, g.Tick + 2, DecisionAction.Paused, "y", 0);
        Assert.Equal(n, g.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced));
        DecisionLog.Add(g.C, g.State, rival, g.Tick + 8, DecisionAction.Paused, "z", 0);
        Assert.True(g.State.Events.Count(e => e.Kind == EventKind.RivalIntentSurfaced) > n);
    }
}