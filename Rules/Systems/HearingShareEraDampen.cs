using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class HearingShareEraDampen
    {
        public static bool Enabled(GameConfig c) { return c.HearingShareEraDampen.Enabled > 0 && HearingSharePrice.Enabled(c); }

        public static int DampenPermille(GameConfig c, GameState state, int reputation)
        {
            if (!Enabled(c) || state == null || reputation >= 0) return 0;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.HearingShareEraDampen.MinEra) return 0;
            return c.HearingShareEraDampen.DampenPermille;
        }

        public static int AppliedCut(int baseCut, int dampenPermille)
        {
            if (dampenPermille <= 0) return baseCut;
            return (int)MathI.MulPermille(baseCut, MathI.Permille - dampenPermille);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab, int rawCut, int appliedCut)
        {
            if (!Enabled(c) || appliedCut >= rawCut) return;
            Events.Add(state, EventKind.HearingShareEraDampened, lab.Id, rawCut - appliedCut, appliedCut, "");
        }
    }
}