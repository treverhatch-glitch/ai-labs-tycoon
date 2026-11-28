using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ScenarioGoalsEraSlack
    {
        public static bool Enabled(GameConfig c) { return c.ScenarioGoalsEraSlack.Enabled > 0 && ScenarioGoals.Enabled(c); }

        public static int SlackWeeks(GameConfig c, GameState state)
        {
            if (!Enabled(c) || state == null) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.ScenarioGoalsEraSlack.MinEra) return 0;
            return c.ScenarioGoalsEraSlack.SlackWeeks;
        }

        public static long AdjustEarlyMark(GameConfig c, GameState state, long mark)
        {
            int slack = SlackWeeks(c, state);
            if (slack <= 0) return mark;
            return mark + slack;
        }

        public static void OnEarlyCheck(GameConfig c, GameState state, LabState lab, int slack, long mark)
        {
            if (slack <= 0) return;
            Events.Add(state, EventKind.ScenarioGoalsEraSlacked, lab.Id, slack, mark, lab.ScenarioId);
        }
    }
}