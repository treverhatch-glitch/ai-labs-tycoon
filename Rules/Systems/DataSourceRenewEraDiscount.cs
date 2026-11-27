using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSourceRenewEraDiscount
    {
        public static bool Enabled(GameConfig c) { return c.DataSourceRenewEraDiscount.Enabled > 0 && DataSources.Enabled(c); }

        public static long RenewCost(GameConfig c, GameState state, long weeklyCost)
        {
            if (!Enabled(c) || state == null || weeklyCost <= 0) return weeklyCost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.DataSourceRenewEraDiscount.MinEra) return weeklyCost;
            int cut = c.DataSourceRenewEraDiscount.DiscountPermille;
            if (cut <= 0) return weeklyCost;
            return MathI.MulPermille(weeklyCost, MathI.Permille - cut);
        }

        public static void OnRenew(GameConfig c, GameState state, LabState lab, int sourceIndex, string sourceId, long listCost, long paid)
        {
            if (!Enabled(c) || paid >= listCost) return;
            long saved = listCost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.DataSourceRenewEraDiscounted, lab.Id, sourceIndex, saved, sourceId);
        }
    }
}