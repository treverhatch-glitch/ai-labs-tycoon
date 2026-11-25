using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Who knows the model (bible ch. 6, key-person risk and documentation). Every model of the player's lab can have one owner, a person of the lab who knows how it works, and a person can own only a few. The lab can write documentation for a model, a level at a time; it costs money and goes stale after a year. When the owner leaves (laid off, quits, is poached or leaves in a pivot) the model keeps a knowledge gap that the documentation made smaller, and until a new owner is named and has closed it week by week, the model fails in front of customers more often and nobody dares change it. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class KnowledgeConfig : ISync
    {
        public int Enabled;
        /// <summary>Most models one person can own.</summary>
        public int MaxOwned;
        /// <summary>Most levels of documentation a model can have.</summary>
        public int MaxDocs;
        /// <summary>What writing the next level of documentation costs.</summary>
        public int DocCost;
        /// <summary>Share of the gap a departure leaves that each level of documentation takes away (permille).</summary>
        public int DocCut;
        /// <summary>Weeks after which the documentation of a model has gone stale and loses a level.</summary>
        public int DocStale;
        /// <summary>Knowledge gap a departing owner leaves with no documentation (out of 1000).</summary>
        public int GapBase;
        /// <summary>Gap a new owner closes each week (out of 1000).</summary>
        public int Ramp;
        /// <summary>Incident chance a full gap adds to the model, in permille.</summary>
        public int GapIncident;
        /// <summary>Gap from which nobody dares change the model (out of 1000).</summary>
        public int GapRiskAt;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxOwned", ref MaxOwned);
            s.Int("maxDocs", ref MaxDocs);
            s.Int("docCost", ref DocCost);
            s.Int("docCut", ref DocCut);
            s.Int("docStale", ref DocStale);
            s.Int("gapBase", ref GapBase);
            s.Int("ramp", ref Ramp);
            s.Int("gapIncident", ref GapIncident);
            s.Int("gapRiskAt", ref GapRiskAt);
        }
    }
}
