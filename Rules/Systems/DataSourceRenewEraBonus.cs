using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSourceRenewEraBonus
    {
        public static bool Enabled(GameConfig c) { return c.DataSourceRenewEraBonus.Enabled > 0 && DataSources.Enabled(c); }

        public static int BonusWeeks(GameConfig c, GameState state)
        {
            if (!Enabled(c) || state == null) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.DataSourceRenewEraBonus.MinEra) return 0;
            return c.DataSourceRenewEraBonus.BonusWeeks;
        }

        public static void OnRenew(GameConfig c, GameState state, LabState lab, string sourceId, int bonus)
        {
            if (!Enabled(c) || bonus <= 0) return;
            Events.Add(state, EventKind.DataSourceRenewEraBonused, lab.Id, DataSources.Index(c, sourceId), bonus, sourceId);
        }
    }
}