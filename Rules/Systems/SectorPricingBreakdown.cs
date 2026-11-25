using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SectorPricingBreakdown
    {
        public static bool Enabled(GameConfig c) { return c.SectorPricingBreakdown.Enabled > 0; }

        public struct Parts
        {
            public int Tier;
            public int BasePermille;
            public int TierRevenueAdd;
            public int TotalRevenuePermille;
            public int OverloadPermille;
        }

        public static Parts Query(GameConfig c, LabState lab, string sectorId)
        {
            var p = new Parts { BasePermille = MathI.Permille };
            if (!SectorPricing.Enabled(c)) return p;
            p.Tier = SectorPricing.Tier(c, lab, sectorId);
            p.TierRevenueAdd = p.Tier * c.SectorPricing.RevenuePerTier;
            p.TotalRevenuePermille = p.BasePermille + p.TierRevenueAdd;
            p.OverloadPermille = SectorPricing.OverloadPermille(c, lab, sectorId);
            return p;
        }

        public static void OnTierSet(GameConfig c, GameState state, LabState lab, string sectorId, int tier)
        {
            if (!Enabled(c) || c.SectorPricingBreakdown.PeakTierNotify <= 0) return;
            if (tier < c.SectorPricing.MaxTier) return;
            Events.Add(state, EventKind.SectorPriceLadderPeak, lab.Id, tier, 0, sectorId);
        }
    }
}