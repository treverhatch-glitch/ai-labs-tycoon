using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ExitDebtEraHaircut
    {
        public static bool Enabled(GameConfig c) { return c.ExitDebtEraHaircut.Enabled > 0; }

        public static long AdjustDebt(GameConfig c, GameState state, long debt)
        {
            if (!Enabled(c) || state == null || debt <= 0) return debt;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.ExitDebtEraHaircut.MinEra) return debt;
            int cut = c.ExitDebtEraHaircut.HaircutPermille;
            if (cut <= 0) return debt;
            return MathI.MulPermille(debt, MathI.Permille - cut);
        }

        public static void OnExit(GameConfig c, GameState state, LabState lab, long rawDebt, long adjusted)
        {
            if (!Enabled(c) || adjusted >= rawDebt) return;
            Events.Add(state, EventKind.ExitDebtEraHaircutted, lab.Id, -1, rawDebt - adjusted, "");
        }
    }
}