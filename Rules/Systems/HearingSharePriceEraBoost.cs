using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class HearingSharePriceEraBoost
    {
        public static bool Enabled(GameConfig c) { return c.HearingSharePriceEraBoost.Enabled > 0 && HearingSharePrice.Enabled(c); }

        public static int BoostPermille(GameConfig c, GameState state, int reputation)
        {
            if (!Enabled(c) || state == null || reputation < 0) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.HearingSharePriceEraBoost.MinEra) return 0;
            return c.HearingSharePriceEraBoost.BoostPermille;
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab, int totalDelta, int boostApplied)
        {
            if (!Enabled(c) || boostApplied <= 0) return;
            Events.Add(state, EventKind.HearingShareEraBoosted, lab.Id, totalDelta, boostApplied, "");
        }
    }
}