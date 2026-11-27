using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class TakeoffAnswerEraCooldown
    {
        public static bool Enabled(GameConfig c) { return c.TakeoffAnswerEraCooldown.Enabled > 0 && TakeoffCrisis.Enabled(c); }

        public static int EffectiveCooldownWeeks(GameConfig c, GameState state)
        {
            int weeks = c.TakeoffCrisis.CooldownWeeks;
            if (!Enabled(c) || state == null) return weeks;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.TakeoffAnswerEraCooldown.MinEra) return weeks;
            int cut = c.TakeoffAnswerEraCooldown.CooldownCutWeeks;
            if (cut <= 0) return weeks;
            return (int)MathI.Max(0, weeks - cut);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab, int appliedWeeks, int cut)
        {
            if (!Enabled(c) || cut <= 0) return;
            Events.Add(state, EventKind.TakeoffAnswerEraCooldownShortened, lab.Id, cut, appliedWeeks, "");
        }
    }
}