using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PublicMetricsEraLoad
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetricsEraLoad.Enabled > 0; }

        static bool EraOpen(GameConfig c, GameState state)
        {
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            return era >= c.PublicMetricsEraLoad.MinEra;
        }

        public static void OnPublish(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c) || !PublicMetrics.Enabled(c) || !EraOpen(c, state)) return;
            int boost = c.PublicMetricsEraLoad.TrustBoostPermille;
            if (boost <= 0) return;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], boost, false);
            Events.Add(state, EventKind.PublicMetricsEraBoost, lab.Id, model.Id, boost, model.Name);
        }
    }
}