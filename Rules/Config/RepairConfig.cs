using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Mending a glitching model (bible ch. 6: a glitching model is "recent incident or bad data; fix with fine-tune or rollback"). A fine-tune pays
    /// to end the glitch now and buys a little real alignment; a rollback withdraws the model and puts the previous version of its line back on
    /// sale, at a cost to reputation.
    /// </summary>
    public sealed class RepairConfig : ISync
    {
        /// <summary>A fine-tune costs this many weeks of what the model sells for.</summary>
        public int FineTuneRevenueWeeks;
        /// <summary>A fine-tune never costs less than this.</summary>
        public long FineTuneMinCost;
        /// <summary>The real alignment a fine-tune adds, as points on the 0 to 1000 meter (the shown meter moves with it).</summary>
        public int FineTuneAlignment;
        /// <summary>Reputation lost when a model is withdrawn and the older version goes back on sale.</summary>
        public int RollbackReputationLoss;

        public void Sync(Syncer s)
        {
            s.Int("fineTuneRevenueWeeks", ref FineTuneRevenueWeeks);
            s.Long("fineTuneMinCost", ref FineTuneMinCost);
            s.Int("fineTuneAlignment", ref FineTuneAlignment);
            s.Int("rollbackReputationLoss", ref RollbackReputationLoss);
        }
    }
}
