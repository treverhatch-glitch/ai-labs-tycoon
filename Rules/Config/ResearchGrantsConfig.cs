using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Research grants (bible ch. 9, "grants and prizes"). The player applies for a grant of a tier, pays a small fee and waits out a review.
    /// The panel then scores the lab (its academic prestige plus a share of its reputation) against the tier's bar. A lab that clears it is paid
    /// at once but owes as many papers as the tier within a deadline; a lab that misses the bar loses only the fee. A lab that does not deliver
    /// pays part of the grant back and loses reputation; one that does gains reputation. One grant at a time, and a wait after each one.
    /// Only the player's lab applies. Zero tiers turns it off, and so does a game with no papers.
    /// </summary>
    public sealed class ResearchGrantsConfig : ISync
    {
        /// <summary>The highest tier. Zero turns grants off.</summary>
        public int MaxTier;
        /// <summary>The researchers the lab needs on the payroll to apply.</summary>
        public int MinResearchers;
        /// <summary>Cash for an application, for each tier.</summary>
        public long ApplyCostPerTier;
        /// <summary>Cash a grant pays, for each tier.</summary>
        public long AwardPerTier;
        /// <summary>The panel's score a tier needs, for each tier.</summary>
        public int BarPerTier;
        /// <summary>The share of the lab's reputation (0 to 1000) that counts towards the score, in permille.</summary>
        public int ReputationWeightPermille;
        /// <summary>Weeks from applying to the panel's verdict.</summary>
        public int ReviewWeeks;
        /// <summary>Weeks a lab has from the award to publish the papers it owes (one for each tier).</summary>
        public int DeliverWeeks;
        /// <summary>The share of the award paid back when the papers do not come, in permille.</summary>
        public int ClawbackPermille;
        /// <summary>Reputation points gained when the papers are delivered.</summary>
        public int CompletedReputation;
        /// <summary>Reputation points lost when they are not.</summary>
        public int FailedReputation;
        /// <summary>Weeks after a grant is settled (or declined) before the next application.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxTier", ref MaxTier);
            s.Int("minResearchers", ref MinResearchers);
            s.Long("applyCostPerTier", ref ApplyCostPerTier);
            s.Long("awardPerTier", ref AwardPerTier);
            s.Int("barPerTier", ref BarPerTier);
            s.Int("reputationWeightPermille", ref ReputationWeightPermille);
            s.Int("reviewWeeks", ref ReviewWeeks);
            s.Int("deliverWeeks", ref DeliverWeeks);
            s.Int("clawbackPermille", ref ClawbackPermille);
            s.Int("completedReputation", ref CompletedReputation);
            s.Int("failedReputation", ref FailedReputation);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}
