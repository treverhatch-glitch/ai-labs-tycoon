using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OpenWeightsEraThrottle
    {
        public static bool Enabled(GameConfig c) { return c.OpenWeightsEraThrottle.Enabled > 0; }

        public static int EffectiveCopyChancePermille(GameConfig c, GameState state)
        {
            if (!Enabled(c) || !OpenWeights.Enabled(c)) return c.OpenWeights.CopyChancePermille;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OpenWeightsEraThrottle.MinEra) return c.OpenWeights.CopyChancePermille;
            return (int)MathI.Max(0, c.OpenWeights.CopyChancePermille - c.OpenWeightsEraThrottle.CopyChanceCutPermille);
        }

        public static void OnCopied(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c)) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OpenWeightsEraThrottle.MinEra) return;
            Events.Add(state, EventKind.OpenWeightsEraThrottled, lab.Id, model.Id, c.OpenWeightsEraThrottle.CopyChanceCutPermille, model.Name);
        }
    }
}