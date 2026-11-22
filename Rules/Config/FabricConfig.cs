using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The interconnect and cooling that make a lot of GPUs work as one training cluster, built in tiers. Each tier opens with an era, takes weeks to build, costs a share of the GPU price for every GPU owned (a deposit now and the rest at delivery), and gives a training speed that replaces the tier before it. During the last weeks the cluster is down for the cutover. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class FabricConfig : ISync
    {
        public int Enabled;
        /// <summary>Owned GPUs a lab needs before it can build a fabric.</summary>
        public int MinGpus;
        /// <summary>Era (by place in the list) in which tier 1 can be built.</summary>
        public int EraA;
        /// <summary>Era in which tier 2 can be built.</summary>
        public int EraB;
        /// <summary>Era in which tier 3 can be built.</summary>
        public int EraC;
        /// <summary>Weeks to build tier 1.</summary>
        public int LeadA;
        /// <summary>Weeks to build tier 2.</summary>
        public int LeadB;
        /// <summary>Weeks to build tier 3.</summary>
        public int LeadC;
        /// <summary>Cost of tier 1 for each owned GPU, as a share of the current GPU price (permille).</summary>
        public int CostA;
        /// <summary>Same for tier 2.</summary>
        public int CostB;
        /// <summary>Same for tier 3.</summary>
        public int CostC;
        /// <summary>Training speed a finished tier 1 gives (permille).</summary>
        public int GainA;
        /// <summary>Same for tier 2 (replaces tier 1, not added).</summary>
        public int GainB;
        /// <summary>Same for tier 3.</summary>
        public int GainC;
        /// <summary>Share of the cost paid when the work is ordered; the rest is paid at delivery (permille).</summary>
        public int DepositPermille;
        /// <summary>Last weeks of a build in which the cluster is down for the cutover.</summary>
        public int CutoverWeeks;
        /// <summary>Training speed lost during the cutover (permille).</summary>
        public int CutoverLoss;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minGpus", ref MinGpus);
            s.Int("eraA", ref EraA);
            s.Int("eraB", ref EraB);
            s.Int("eraC", ref EraC);
            s.Int("leadA", ref LeadA);
            s.Int("leadB", ref LeadB);
            s.Int("leadC", ref LeadC);
            s.Int("costA", ref CostA);
            s.Int("costB", ref CostB);
            s.Int("costC", ref CostC);
            s.Int("gainA", ref GainA);
            s.Int("gainB", ref GainB);
            s.Int("gainC", ref GainC);
            s.Int("depositPermille", ref DepositPermille);
            s.Int("cutoverWeeks", ref CutoverWeeks);
            s.Int("cutoverLoss", ref CutoverLoss);
        }
    }
}
