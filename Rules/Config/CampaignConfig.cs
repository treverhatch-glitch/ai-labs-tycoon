using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>What an era goal measures. Every one is read off the player's lab as it stands when the era ends.</summary>
    public enum GoalMetric
    {
        /// <summary>Models that have been put on sale (on sale now or retired).</summary>
        ModelsReleased,
        /// <summary>Sectors the lab has a model on sale in.</summary>
        SectorsServed,
        /// <summary>Sectors whose Gauntlet crown the lab holds.</summary>
        SectorsLed,
        /// <summary>Reputation, 0 to 1000.</summary>
        Reputation,
        /// <summary>People on the payroll.</summary>
        Staff,
        /// <summary>Techs researched.</summary>
        Techs,
        /// <summary>Everything the lab has ever earned from customers.</summary>
        LifetimeRevenue,
        /// <summary>The best true alignment of any model on sale, 0 to 1000.</summary>
        BestAlignment
    }

    /// <summary>
    /// Campaign scoring (bible ch. 13: "a campaign mode sets goals for each era and scores the run at the end"). The goals live in the data. When an era
    /// ends the game marks the player's lab against that era's goals and keeps the score. A sandbox game simply never looks at the score; nothing
    /// in the sim reads it, so scoring changes no one's play.
    /// </summary>
    public sealed class CampaignConfig : ISync
    {
        /// <summary>Extra points for meeting every goal of an era.</summary>
        public int AllMetBonus;
        public List<EraGoalConfig> Goals = new List<EraGoalConfig>();

        public void Sync(Syncer s)
        {
            s.Int("allMetBonus", ref AllMetBonus);
            s.ObjList("goals", ref Goals);
        }

        public List<EraGoalConfig> GoalsOf(string eraId)
        {
            var found = new List<EraGoalConfig>();
            for (int i = 0; i < Goals.Count; i++) if (Goals[i].EraId == eraId) found.Add(Goals[i]);
            return found;
        }
    }

    public sealed class EraGoalConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string EraId = "";
        public string Name = "";
        public GoalMetric Metric;
        public long Target;
        public int Points;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("era", ref EraId);
            s.Str("name", ref Name);
            s.Enum("metric", ref Metric);
            s.Long("target", ref Target);
            s.Int("points", ref Points);
        }
    }
}
