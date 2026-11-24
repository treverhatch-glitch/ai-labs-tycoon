using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Era goals and the run's score. The goals are read from the data; the only thing kept is each finished era's mark.</summary>
    public static class Campaign
    {
        public struct GoalProgress
        {
            public EraGoalConfig Goal;
            public long Value;
            public bool Met;
        }

        public static long Value(GameConfig c, LabState lab, GoalMetric metric, GameState state)
        {
            switch (metric)
            {
                case GoalMetric.ModelsReleased:
                    {
                        int n = 0;
                        for (int i = 0; i < lab.Models.Count; i++)
                            if (lab.Models[i].Status == ModelStatus.Released || lab.Models[i].Status == ModelStatus.Retired) n++;
                        return n;
                    }
                case GoalMetric.SectorsServed:
                    {
                        var seen = new HashSet<string>();
                        for (int i = 0; i < lab.Models.Count; i++)
                        {
                            if (lab.Models[i].Status != ModelStatus.Released) continue;
                            for (int k = 0; k < lab.Models[i].Sectors.Count; k++) seen.Add(lab.Models[i].Sectors[k]);
                        }
                        return seen.Count;
                    }
                case GoalMetric.SectorsLed:
                    {
                        int n = 0;
                        for (int i = 0; i < state.Sectors.Count; i++) if (state.Sectors[i].ChampionLabId == lab.Id) n++;
                        return n;
                    }
                case GoalMetric.Reputation: return lab.Reputation;
                case GoalMetric.Staff: return lab.Staff.Count;
                case GoalMetric.Techs: return lab.Techs.Count;
                case GoalMetric.LifetimeRevenue: return lab.Ledger.Lifetime.Revenue;
                case GoalMetric.BestAlignment:
                    {
                        int best = 0;
                        for (int i = 0; i < lab.Models.Count; i++)
                            if (lab.Models[i].Status == ModelStatus.Released && lab.Models[i].Alignment > best) best = lab.Models[i].Alignment;
                        return best;
                    }
                default: return 0;
            }
        }

        /// <summary>Where the lab stands against each goal of an era right now.</summary>
        public static List<GoalProgress> Progress(GameConfig c, GameState state, LabState lab, string eraId)
        {
            var list = new List<GoalProgress>();
            List<EraGoalConfig> goals = c.Campaign.GoalsOf(eraId);
            for (int i = 0; i < goals.Count; i++)
            {
                var p = new GoalProgress();
                p.Goal = goals[i];
                p.Value = Value(c, lab, goals[i].Metric, state);
                p.Met = p.Value >= goals[i].Target;
                list.Add(p);
            }
            return list;
        }

        /// <summary>The mark an era would get as things stand: goals met, goals there are, and points with the all-met bonus.</summary>
        public static EraScore Mark(GameConfig c, GameState state, LabState lab, string eraId)
        {
            var score = new EraScore();
            score.EraId = eraId;
            List<GoalProgress> progress = Progress(c, state, lab, eraId);
            score.Of = progress.Count;
            for (int i = 0; i < progress.Count; i++)
                if (progress[i].Met) { score.Met++; score.Points += progress[i].Goal.Points; }
            if (score.Of > 0 && score.Met == score.Of) score.Points += c.Campaign.AllMetBonus;
            return score;
        }

        public static EraScore Recorded(LabState lab, string eraId)
        {
            for (int i = 0; i < lab.EraScores.Count; i++) if (lab.EraScores[i].EraId == eraId) return lab.EraScores[i];
            return null;
        }

        /// <summary>The run's score: every finished era's mark plus what the current era would get if it ended now.</summary>
        public static int RunScore(GameConfig c, GameState state, LabState lab)
        {
            int total = 0;
            for (int i = 0; i < lab.EraScores.Count; i++) total += lab.EraScores[i].Points;
            string now = EraSystem.AtTick(c, state.Tick).Id;
            if (Recorded(lab, now) == null) total += Mark(c, state, lab, now).Points;
            return total;
        }

        /// <summary>
        /// The era that has just ended is marked for the player's lab. Only the player is marked, and only once per era. A game that starts in a later
        /// era has no marks for the eras before it (the player's PlanStartTick is the tick the lab was founded).
        /// </summary>
        public static void OnEraBegan(GameConfig c, GameState state, int newEraIndex)
        {
            if (newEraIndex < 1) return;
            LabState lab = state.Player;
            if (lab == null || lab.Bankrupt) return;
            EraConfig endedEra = c.Eras[newEraIndex - 1];
            if (EraSystem.StartTick(c, endedEra) < lab.PlanStartTick) return;
            string ended = endedEra.Id;
            if (Recorded(lab, ended) != null) return;
            EraScore score = Mark(c, state, lab, ended);
            if (score.Of == 0) return;
            lab.EraScores.Add(score);
            Events.Add(state, EventKind.EraScored, lab.Id, score.Met, score.Points, ended);
        }
    }
}
