using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Per-sector price tiers (Q460, schema 138). Player only.</summary>
    public sealed class SectorPricingConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int MaxTier;
        /// <summary>Revenue permille per tier step above baseline.</summary>
        public int RevenuePerTier;
        /// <summary>Overload incident permille per tier step.</summary>
        public int OverloadPerTier;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("maxTier", ref MaxTier);
            s.Int("revenuePerTier", ref RevenuePerTier);
            s.Int("overloadPerTier", ref OverloadPerTier);
        }
    }
}