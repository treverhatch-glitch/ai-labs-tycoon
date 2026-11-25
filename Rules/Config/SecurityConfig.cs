using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Information security. The lab can build its security up a tier at a time; each tier is paid for up front, takes weeks to build and costs upkeep every week. Every week the lab's models on sale add to its exposure and each tier takes some off. When the exposure is full a breach opens and the lab has a few weeks to answer: pay the ransom (it rises every time), disclose it (reputation and trust), or lock everything down at once at double the next tier's price. An answer clears the exposure. An unanswered breach leaks. Disclosing or leaking draws a privacy claim on the docket. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SecurityConfig : ISync
    {
        public int Enabled;
        /// <summary>The top tier.</summary>
        public int MaxTier;
        /// <summary>What tier 1 costs; each tier costs this many times its number.</summary>
        public int TierCost;
        /// <summary>Weeks a tier takes to build.</summary>
        public int BuildWeeks;
        /// <summary>Weekly upkeep for each tier held.</summary>
        public int UpkeepPerTier;
        /// <summary>Exposure each model on sale adds every week.</summary>
        public int ExposurePerModel;
        /// <summary>Exposure each tier takes off every week.</summary>
        public int ReliefPerTier;
        /// <summary>Exposure at which a breach opens.</summary>
        public int Threshold;
        /// <summary>Weeks the lab has to answer.</summary>
        public int AnswerWeeks;
        /// <summary>The first ransom; each one paid since the last disclosure adds this much again.</summary>
        public int Ransom;
        /// <summary>Reputation disclosing costs, before the press damps it.</summary>
        public int DiscloseReputation;
        /// <summary>Reputation an unanswered breach costs, before the press damps it.</summary>
        public int LeakReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxTier", ref MaxTier);
            s.Int("tierCost", ref TierCost);
            s.Int("buildWeeks", ref BuildWeeks);
            s.Int("upkeepPerTier", ref UpkeepPerTier);
            s.Int("exposurePerModel", ref ExposurePerModel);
            s.Int("reliefPerTier", ref ReliefPerTier);
            s.Int("threshold", ref Threshold);
            s.Int("answerWeeks", ref AnswerWeeks);
            s.Int("ransom", ref Ransom);
            s.Int("discloseReputation", ref DiscloseReputation);
            s.Int("leakReputation", ref LeakReputation);
        }
    }
}
