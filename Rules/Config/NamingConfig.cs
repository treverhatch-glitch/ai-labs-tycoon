using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Version naming (bible ch. 6, "joke that is also a system"). Every new version of a model makes the lab's names a little more confusing:
    /// suffixes stack up, and customers cannot tell which model is which. Clean naming costs a little marketing effort. Only the player's labs
    /// let it rot; rivals keep plain names.
    /// </summary>
    public sealed class NamingConfig : ISync
    {
        /// <summary>The mess a new training run adds to the lab's names.</summary>
        public int MessPerVersion;
        /// <summary>The most mess a lab can have, from 1 to 1000.</summary>
        public int MaxMess;
        /// <summary>Mess for each stacked suffix on a new model's name.</summary>
        public int SuffixEveryPoints;
        /// <summary>The words that stack up, in order. A lab with more mess than words keeps the last one it has.</summary>
        public List<string> Suffixes = new List<string>();
        /// <summary>The share of appeal customers lose at full mess, in permille. It grows with the mess.</summary>
        public int ConfusionPermille;
        /// <summary>What one clean-up takes off the mess.</summary>
        public int CleanPoints;
        /// <summary>What a clean-up costs, in hiring fees of a sales rep of the era (the marketing team's time).</summary>
        public int CleanFeeHireFees;

        public void Sync(Syncer s)
        {
            s.Int("messPerVersion", ref MessPerVersion);
            s.Int("maxMess", ref MaxMess);
            s.Int("suffixEveryPoints", ref SuffixEveryPoints);
            s.StrList("suffixes", ref Suffixes);
            s.Int("confusionPermille", ref ConfusionPermille);
            s.Int("cleanPoints", ref CleanPoints);
            s.Int("cleanFeeHireFees", ref CleanFeeHireFees);
        }
    }
}
