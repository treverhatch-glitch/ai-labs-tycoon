using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public enum LeadEffectKind
    {
        /// <summary>Nothing came of it.</summary>
        Nothing,
        /// <summary>Research points: <see cref="LeadOutcomeConfig.Amount"/> weeks of what the lab's researchers make.</summary>
        ResearchWeeks,
        /// <summary>A lasting training speed bonus: <see cref="LeadOutcomeConfig.Amount"/> permille, added up across outcomes up to <see cref="LeadsConfig.MaxSpeedPermille"/>.</summary>
        TrainingSpeedPermille
    }

    /// <summary>
    /// Research leads (bible ch. 6: "sometimes a model is better than it has any right to be and nobody knows why. That mystery becomes a research
    /// lead"). A player run that lands well above its aim leaves a lead; following it costs research and time and ends in one of the outcomes.
    /// Only the player's labs find leads.
    /// </summary>
    public sealed class LeadsConfig : ISync
    {
        /// <summary>A run whose luck is at least this many permille above its aim leaves a lead.</summary>
        public int MinLuckPermille;
        /// <summary>The most leads waiting at once; a new one is not found while the desk is full.</summary>
        public int MaxOpen;
        /// <summary>Weeks a lead waits before it goes cold.</summary>
        public int ExpireWeeks;
        /// <summary>Weeks an investigation takes.</summary>
        public int FollowWeeks;
        /// <summary>The price of following a lead, in weeks of the lab's research output.</summary>
        public int FollowCostWeeks;
        /// <summary>The price is never below this many research points.</summary>
        public long MinCost;
        /// <summary>The most speed that all the training tricks together can add, in permille.</summary>
        public int MaxSpeedPermille;
        public List<LeadOutcomeConfig> Outcomes = new List<LeadOutcomeConfig>();

        public LeadOutcomeConfig Outcome(string id)
        {
            for (int i = 0; i < Outcomes.Count; i++) if (Outcomes[i].IdValue == id) return Outcomes[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("minLuckPermille", ref MinLuckPermille);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("expireWeeks", ref ExpireWeeks);
            s.Int("followWeeks", ref FollowWeeks);
            s.Int("followCostWeeks", ref FollowCostWeeks);
            s.Long("minCost", ref MinCost);
            s.Int("maxSpeedPermille", ref MaxSpeedPermille);
            s.ObjList("outcomes", ref Outcomes);
        }
    }

    /// <summary>What was behind a lucky run. One is picked when an investigation ends, by weight, from the lead itself (no dice).</summary>
    public sealed class LeadOutcomeConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int Weight;
        public LeadEffectKind Kind;
        public int Amount;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("weight", ref Weight);
            s.Enum("kind", ref Kind);
            s.Int("amount", ref Amount);
            s.Str("blurb", ref Blurb);
        }
    }
}
