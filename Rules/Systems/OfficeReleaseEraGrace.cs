using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OfficeReleaseEraGrace
    {
        public static bool Enabled(GameConfig c) { return c.OfficeReleaseEraGrace.Enabled > 0; }

        public static int AnswerWeeks(GameConfig c, GameState state)
        {
            int weeks = c.Office.ExpireWeeks;
            if (!Enabled(c) || state == null) return weeks;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OfficeReleaseEraGrace.MinEra) return weeks;
            return weeks + c.OfficeReleaseEraGrace.GraceWeeks;
        }

        public static void OnPosed(GameConfig c, GameState state, LabState lab, int graceAdded)
        {
            if (!Enabled(c) || graceAdded <= 0) return;
            Events.Add(state, EventKind.OfficeReleaseEraGraced, lab.Id, graceAdded, 0, "");
        }
    }
}