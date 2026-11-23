using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class ScenarioGoalsConfig : ISync
    {
        public int Enabled;
        public List<ScenarioGoalConfig> Goals = new List<ScenarioGoalConfig>();

        public ScenarioGoalConfig Goal(string id)
        {
            for (int i = 0; i < Goals.Count; i++) if (Goals[i].IdValue == id) return Goals[i];
            return null;
        }

        public void Sync(Syncer s) { s.ObjList("goals", ref Goals); }
    }

  /// <summary>Win or lose check for a named scenario (Q471, schema 147).</summary>
    public sealed class ScenarioGoalConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public GoalMetric Metric;
        public long Target;
        public string FailEndingId = "";
        public string WinEndingId = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Enum("metric", ref Metric);
            s.Long("target", ref Target);
            s.Str("failEndingId", ref FailEndingId);
            s.Str("winEndingId", ref WinEndingId);
        }
    }
}