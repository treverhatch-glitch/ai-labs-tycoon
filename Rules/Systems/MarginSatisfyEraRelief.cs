using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MarginSatisfyEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.MarginSatisfyEraRelief.Enabled > 0 && MarginCall.Enabled(c); }

        public static int ReliefPermille(GameConfig c, GameState state)
        {
            if (!Enabled(c) || state == null) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.MarginSatisfyEraRelief.MinEra) return 0;
            return c.MarginSatisfyEraRelief.ReliefPermille;
        }

        public static long AdjustDue(GameConfig c, GameState state, long rawDue)
        {
            int relief = ReliefPermille(c, state);
            if (relief <= 0 || rawDue <= 0) return rawDue;
            return MathI.MulPermille(rawDue, MathI.Permille - relief);
        }

        public static void OnPaid(GameConfig c, GameState state, LabState lab, long listDue, long paid)
        {
            if (!Enabled(c) || listDue <= 0 || paid >= listDue) return;
            long saved = listDue - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.MarginSatisfyEraRelieved, lab.Id, ReliefPermille(c, state), saved, "");
        }
    }
}