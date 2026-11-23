using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class CampaignDeadlineEraGrace
    {
        public static bool Enabled(GameConfig c) { return c.CampaignDeadlineEraGrace.Enabled > 0; }

        public static int AdjustDeadline(GameConfig c, GameState state, int deadlineTick)
        {
            if (!Enabled(c) || !CampaignDeadline.Enabled(c) || deadlineTick <= 0) return deadlineTick;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.CampaignDeadlineEraGrace.MinEra) return deadlineTick;
            int w = c.CampaignDeadlineEraGrace.GraceWeeks;
            if (w <= 0) return deadlineTick;
            return deadlineTick + w;
        }

        public static void OnSet(GameConfig c, GameState state, LabState lab, int adjustedTick, int graceWeeks)
        {
            if (!Enabled(c) || graceWeeks <= 0) return;
            Events.Add(state, EventKind.CampaignDeadlineElongated, lab.Id, adjustedTick, graceWeeks, lab.ScenarioId);
        }
    }
}