using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PublicMetricsGainEraBoost
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetricsGainEraBoost.Enabled > 0 && PublicMetrics.Enabled(c); }

        public static int TrustGain(GameConfig c, GameState state, int gain)
        {
            if (!Enabled(c) || state == null || gain <= 0) return gain;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.PublicMetricsGainEraBoost.MinEra) return gain;
            int boost = c.PublicMetricsGainEraBoost.BoostPermille;
            if (boost <= 0) return gain;
            return (int)MathI.MulPermille(gain, MathI.Permille + boost);
        }

        public static void OnGain(GameConfig c, GameState state, LabState lab, ModelState model, int listGain, int applied)
        {
            if (!Enabled(c) || applied <= listGain) return;
            int extra = applied - listGain;
            if (extra <= 0) return;
            Events.Add(state, EventKind.PublicMetricsGainEraBoosted, lab.Id, model.Id, extra, model.Name);
        }
    }
}