using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MarginSatisfyEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.MarginSatisfyEraRelief.Enabled > 0 && MarginCall.Enabled(c); }

        public static long CashDue(GameConfig c, GameState state, long due)
        {
            if (!Enabled(c) || state == null || due <= 0) return due;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.MarginSatisfyEraRelief.MinEra) return due;
            int cut = c.MarginSatisfyEraRelief.ReliefPermille;
            if (cut <= 0) return due;
            return MathI.MulPermille(due, MathI.Permille - cut);
        }

        public static void OnSatisfy(GameConfig c, GameState state, LabState lab, long listDue, long paid)
        {
            if (!Enabled(c) || paid >= listDue) return;
            long saved = listDue - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.MarginSatisfyEraRelieved, lab.Id, -1, saved, "");
        }
    }
}