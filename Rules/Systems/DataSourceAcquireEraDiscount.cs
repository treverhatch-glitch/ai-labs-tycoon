using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSourceAcquireEraDiscount
    {
        public static bool Enabled(GameConfig c) { return c.DataSourceAcquireEraDiscount.Enabled > 0 && DataSources.Enabled(c); }

        public static long AcquireCost(GameConfig c, GameState state, long weeklyCost)
        {
            if (!Enabled(c) || state == null || weeklyCost <= 0) return weeklyCost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.DataSourceAcquireEraDiscount.MinEra) return weeklyCost;
            int cut = c.DataSourceAcquireEraDiscount.DiscountPermille;
            if (cut <= 0) return weeklyCost;
            return MathI.MulPermille(weeklyCost, MathI.Permille - cut);
        }

        public static void OnAcquire(GameConfig c, GameState state, LabState lab, int sourceIndex, string sourceId, long listCost, long paid)
        {
            if (!Enabled(c) || paid >= listCost) return;
            long saved = listCost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.DataSourceAcquireEraDiscounted, lab.Id, sourceIndex, saved, sourceId);
        }
    }
}