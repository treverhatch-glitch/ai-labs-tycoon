using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class RungSixEraCooldown
    {
        public static bool Enabled(GameConfig c) { return c.RungSixEraCooldown.Enabled > 0 && RungSixCrisis.Enabled(c); }

        public static int EffectiveCooldownWeeks(GameConfig c, GameState state)
        {
            int weeks = c.RungSixCrisis.CooldownWeeks;
            if (!Enabled(c) || state == null) return weeks;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.RungSixEraCooldown.MinEra) return weeks;
            int cut = c.RungSixEraCooldown.CooldownCutWeeks;
            if (cut <= 0) return weeks;
            return (int)MathI.Max(0, weeks - cut);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab, int appliedWeeks, int cut)
        {
            if (!Enabled(c) || cut <= 0) return;
            Events.Add(state, EventKind.RungSixEraCooldownShortened, lab.Id, cut, appliedWeeks, "");
        }
    }
}