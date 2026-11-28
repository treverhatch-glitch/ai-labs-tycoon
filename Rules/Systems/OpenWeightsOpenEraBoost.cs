using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OpenWeightsOpenEraBoost
    {
        public static bool Enabled(GameConfig c) { return c.OpenWeightsOpenEraBoost.Enabled > 0 && OpenWeights.Enabled(c); }

        public static void OnOpened(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c) || state == null) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OpenWeightsOpenEraBoost.MinEra) return;
            int boost = c.OpenWeightsOpenEraBoost.TrustBoostPermille;
            if (boost <= 0) return;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], boost, false);
            Events.Add(state, EventKind.OpenWeightsOpenEraBoosted, lab.Id, model.Id, boost, model.Name);
        }
    }
}