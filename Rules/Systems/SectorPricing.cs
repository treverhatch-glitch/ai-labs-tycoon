using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SectorPricing
    {
        public static bool Enabled(GameConfig c) { return c.SectorPricing.Enabled > 0; }

        public static int Tier(GameConfig c, LabState lab, string sectorId)
        {
            if (!Enabled(c)) return 0;
            for (int i = 0; i < lab.PriceSectors.Count; i++)
                if (lab.PriceSectors[i] == sectorId) return i < lab.PriceTiers.Count ? lab.PriceTiers[i] : 0;
            return 0;
        }

        public static void SetTier(GameConfig c, GameState state, LabState lab, string sectorId, int tier)
        {
            int t = (int)MathI.Clamp(tier, 0, c.SectorPricing.MaxTier);
            for (int i = 0; i < lab.PriceSectors.Count; i++)
            {
                if (lab.PriceSectors[i] != sectorId) continue;
                lab.PriceTiers[i] = t;
                Events.Add(state, EventKind.SectorPriceSet, lab.Id, t, 0, sectorId);
                return;
            }
            lab.PriceSectors.Add(sectorId);
            lab.PriceTiers.Add(t);
            Events.Add(state, EventKind.SectorPriceSet, lab.Id, t, 0, sectorId);
        }

        public static int RevenuePermille(GameConfig c, LabState lab, string sectorId)
        {
            int tier = Tier(c, lab, sectorId);
            return MathI.Permille + tier * c.SectorPricing.RevenuePerTier;
        }

        public static int OverloadPermille(GameConfig c, LabState lab, string sectorId)
        {
            int tier = Tier(c, lab, sectorId);
            return tier * c.SectorPricing.OverloadPerTier;
        }

        public static int LabOverloadPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return 0;
            int max = 0;
            for (int i = 0; i < lab.PriceTiers.Count; i++) if (lab.PriceTiers[i] > max) max = lab.PriceTiers[i];
            return max * c.SectorPricing.OverloadPerTier;
        }
    }
}