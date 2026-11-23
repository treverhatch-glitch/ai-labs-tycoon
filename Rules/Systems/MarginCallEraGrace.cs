using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MarginCallEraGrace
    {
        public static bool Enabled(GameConfig c) { return c.MarginCallEraGrace.Enabled > 0 && MarginCall.Enabled(c); }

        public static int CureWeeks(GameConfig c, GameState state)
        {
            int weeks = c.MarginCall.CureWeeks;
            if (!Enabled(c) || state == null) return weeks;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.MarginCallEraGrace.MinEra) return weeks;
            return weeks + c.MarginCallEraGrace.GraceWeeks;
        }

        public static void OnCall(GameConfig c, GameState state, LabState lab, int cureWeeks, int graceAdded)
        {
            if (!Enabled(c) || graceAdded <= 0) return;
            Events.Add(state, EventKind.MarginCallEraGraced, lab.Id, cureWeeks, graceAdded, "");
        }
    }
}