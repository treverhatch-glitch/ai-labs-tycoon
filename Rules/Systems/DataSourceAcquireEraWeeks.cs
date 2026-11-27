using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSourceAcquireEraWeeks
    {
        public static bool Enabled(GameConfig c) { return c.DataSourceAcquireEraWeeks.Enabled > 0 && DataSources.Enabled(c); }

        public static int BonusWeeks(GameConfig c, GameState state)
        {
            if (!Enabled(c) || state == null) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.DataSourceAcquireEraWeeks.MinEra) return 0;
            return c.DataSourceAcquireEraWeeks.BonusWeeks;
        }

        public static void OnAcquire(GameConfig c, GameState state, LabState lab, string sourceId, int bonus)
        {
            if (!Enabled(c) || bonus <= 0) return;
            Events.Add(state, EventKind.DataSourceAcquireEraWeeksBonused, lab.Id, DataSources.Index(c, sourceId), bonus, sourceId);
        }
    }
}