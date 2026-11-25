using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The service-level pledge: the player promises customers uptime. Each level earns a premium on the week's revenue. In a week the promise
    /// is broken (the lab could serve less than a set share of its demand, or one of its models had an incident) the lab refunds a share of that
    /// week's revenue, earns no premium, and loses reputation. A run of broken weeks drops the pledge a level. The pledge needs a reputation to
    /// sell and a change locks it for some weeks. Only the player's lab does it. Zero levels turns it off.
    /// </summary>
    public sealed class SlaConfig : ISync
    {
        /// <summary>The highest level. Zero turns it off.</summary>
        public int MaxLevel;
        /// <summary>The premium each level adds to the week's revenue, in permille.</summary>
        public int PremiumPermillePerLevel;
        /// <summary>A week counts as broken when the share of demand the lab could serve falls below this, in permille.</summary>
        public int BreachBelowPermille;
        /// <summary>The share of the week's revenue each level refunds in a broken week, in permille.</summary>
        public int RefundPermillePerLevel;
        /// <summary>The reputation points each level costs in a broken week.</summary>
        public int BreachReputationPerLevel;
        /// <summary>The reputation (0 to 1000) needed to raise the pledge.</summary>
        public int MinReputation;
        /// <summary>Weeks a change locks the pledge.</summary>
        public int LockWeeks;
        /// <summary>Broken weeks in a row that drop the pledge a level. Zero never drops it.</summary>
        public int StrikeWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("premiumPermillePerLevel", ref PremiumPermillePerLevel);
            s.Int("breachBelowPermille", ref BreachBelowPermille);
            s.Int("refundPermillePerLevel", ref RefundPermillePerLevel);
            s.Int("breachReputationPerLevel", ref BreachReputationPerLevel);
            s.Int("minReputation", ref MinReputation);
            s.Int("lockWeeks", ref LockWeeks);
            s.Int("strikeWeeks", ref StrikeWeeks);
        }
    }
}
