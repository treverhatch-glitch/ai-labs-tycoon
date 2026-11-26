using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class TakeoffCrisisEraGrace
    {
        public static bool Enabled(GameConfig c) { return c.TakeoffCrisisEraGrace.Enabled > 0 && TakeoffCrisis.Enabled(c); }

        public static void OnPosed(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !OfficeEvents.IsPending(lab)) return;
            OfficeEventConfig e = c.Office.Event(lab.OfficeEventId);
            if (e == null || e.Trigger != OfficeTrigger.Takeoff) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.TakeoffCrisisEraGrace.MinEra) return;
            int grace = c.TakeoffCrisisEraGrace.GraceWeeks;
            if (grace <= 0) return;
            lab.OfficeEventExpires += grace;
            Events.Add(state, EventKind.TakeoffCrisisEraGraced, lab.Id, lab.OfficeEventExpires, grace, e.IdValue);
        }
    }
}