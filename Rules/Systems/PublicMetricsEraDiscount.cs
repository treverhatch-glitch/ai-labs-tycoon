using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PublicMetricsEraDiscount
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetricsEraDiscount.Enabled > 0 && PublicMetrics.Enabled(c); }

        public static long PublishCost(GameConfig c, GameState state, long cost)
        {
            if (!Enabled(c) || state == null || cost <= 0) return cost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.PublicMetricsEraDiscount.MinEra) return cost;
            int cut = c.PublicMetricsEraDiscount.DiscountPermille;
            if (cut <= 0) return cost;
            return MathI.MulPermille(cost, MathI.Permille - cut);
        }

        public static void OnPublish(GameConfig c, GameState state, LabState lab, ModelState model, long listCost, long paid)
        {
            if (!Enabled(c) || paid >= listCost) return;
            long saved = listCost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.PublicMetricsEraDiscounted, lab.Id, model.Id, saved, model.Name);
        }
    }
}