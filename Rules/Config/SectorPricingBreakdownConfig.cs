using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Player-facing sector price ladder parts and peak-tier notice (Q481, schema 155).</summary>
    public sealed class SectorPricingBreakdownConfig : ISync
    {
        public int Enabled;
        public int PeakTierNotify;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("peakTierNotify", ref PeakTierNotify);
        }
    }
}