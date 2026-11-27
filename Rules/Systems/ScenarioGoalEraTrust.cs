using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ScenarioGoalEraTrust
    {
        public static bool Enabled(GameConfig c) { return c.ScenarioGoalEraTrust.Enabled > 0 && ScenarioGoals.Enabled(c); }

        public static void OnMet(GameConfig c, GameState state, LabState lab, string goalId)
        {
            if (!Enabled(c) || state == null) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.ScenarioGoalEraTrust.MinEra) return;
            int pts = c.ScenarioGoalEraTrust.TrustPoints;
            if (pts <= 0) return;
            for (int s = 0; s < state.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, state.Sectors[s].Id, pts, false);
            Events.Add(state, EventKind.ScenarioGoalEraTrusted, lab.Id, pts, 0, goalId);
        }
    }
}