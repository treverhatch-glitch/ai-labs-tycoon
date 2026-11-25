using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SectorPricingEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.SectorPricingEraRelief.Enabled > 0 && SectorPricing.Enabled(c); }

        public static int ReliefPermille(GameConfig c, GameState state, int tier)
        {
            if (!Enabled(c) || state == null || tier <= 0) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.SectorPricingEraRelief.MinEra) return 0;
            return c.SectorPricingEraRelief.ReliefPermille;
        }

        public static int AdjustOverload(GameConfig c, GameState state, int tier, int rawPermille)
        {
            int relief = ReliefPermille(c, state, tier);
            if (relief <= 0) return rawPermille;
            return (int)MathI.Max(0, rawPermille - relief);
        }

        public static void OnTierSet(GameConfig c, GameState state, LabState lab, string sectorId, int tier)
        {
            int relief = ReliefPermille(c, state, tier);
            if (relief <= 0 || tier < 1) return;
            Events.Add(state, EventKind.SectorPricingEraRelieved, lab.Id, tier, relief, sectorId);
        }
    }
}