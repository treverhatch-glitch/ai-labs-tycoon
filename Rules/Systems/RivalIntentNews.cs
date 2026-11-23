using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class RivalIntentNews
    {
        public static bool Enabled(GameConfig c) { return c.RivalIntentNews.Enabled > 0; }

        public static void OnDecision(GameConfig c, GameState state, LabState lab, DecisionAction action, string reference)
        {
            if (!Enabled(c)) return;
            if (lab.Controller != LabController.Plan) return;
            int gap = c.RivalIntentNews.MinWeeksBetween;
            if (gap > 0 && state.RivalIntentLastTick >= 0 && state.Tick - state.RivalIntentLastTick < gap) return;
            state.RivalIntentLastTick = state.Tick;
            GameEvent ev = Events.Add(state, EventKind.RivalIntentSurfaced, lab.Id, (int)action, 0, reference ?? "");
            ev.Pick = action == DecisionAction.Lobbied ? 1 : 0;
        }
    }
}