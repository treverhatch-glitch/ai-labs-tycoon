using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ScenarioGoals
    {
        public static bool Enabled(GameConfig c) { return c.ScenarioGoals.Enabled > 0; }

        public static ScenarioGoalConfig GoalFor(LabState lab, GameConfig c)
        {
            if (string.IsNullOrEmpty(lab.ScenarioId)) return null;
            ScenarioConfig sc = c.Scenarios.Scenario(lab.ScenarioId);
            if (sc == null || string.IsNullOrEmpty(sc.GoalId)) return null;
            return c.ScenarioGoals.Goal(sc.GoalId);
        }

        public static bool Met(GameConfig c, GameState state, LabState lab, ScenarioGoalConfig goal)
        {
            return Campaign.Value(c, lab, goal.Metric, state) >= goal.Target;
        }

        public static void Check(GameConfig c, GameState state, LabState lab, bool deadline)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            if (lab.ScenarioGoalResult != 0) return;
            ScenarioGoalConfig goal = GoalFor(lab, c);
            if (goal == null) return;
            ScenarioConfig sc = c.Scenarios.Scenario(lab.ScenarioId);
            if (!deadline && sc.GoalDeadlinePermille > 0 && lab.CampaignDeadlineTick > 0)
            {
                long span = lab.CampaignDeadlineTick - lab.PlanStartTick;
                if (span > 0)
                {
                    long mark = lab.PlanStartTick + span * sc.GoalDeadlinePermille / MathI.Permille;
                    int slack = ScenarioGoalsEraSlack.SlackWeeks(c, state);
                    mark = ScenarioGoalsEraSlack.AdjustEarlyMark(c, state, mark);
                    if (state.Tick + 1 < mark) return;
                    if (slack > 0 && state.Tick + 1 == mark)
                        ScenarioGoalsEraSlack.OnEarlyCheck(c, state, lab, slack, mark);
                }
            }
            if (Met(c, state, lab, goal))
            {
                lab.ScenarioGoalResult = 1;
                Events.Add(state, EventKind.ScenarioGoalMet, lab.Id, (int)goal.Metric, goal.Target, goal.IdValue);
                ScenarioGoalEraTrust.OnMet(c, state, lab, goal.IdValue);
                if (!string.IsNullOrEmpty(goal.WinEndingId)) EndingSystem.RecordScenario(c, state, lab, goal.WinEndingId);
                return;
            }
            if (deadline)
            {
                lab.ScenarioGoalResult = -1;
                Events.Add(state, EventKind.ScenarioGoalMissed, lab.Id, (int)goal.Metric, goal.Target, goal.IdValue);
                if (!string.IsNullOrEmpty(goal.FailEndingId)) EndingSystem.RecordScenario(c, state, lab, goal.FailEndingId);
            }
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            Check(c, state, lab, deadline: false);
        }
    }
}