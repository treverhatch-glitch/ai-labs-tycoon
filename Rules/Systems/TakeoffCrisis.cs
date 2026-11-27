using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class TakeoffCrisis
    {
        public static bool Enabled(GameConfig c) { return c.TakeoffCrisis.Enabled > 0; }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            if (OfficeEvents.IsPending(lab)) return;
            if (state.Tick < lab.TakeoffCooldownUntil) return;
            int era = EraSystem.IndexOf(c, EraSystem.AtTick(c, state.Tick).Id);
            if (era < c.TakeoffCrisis.MinEra) return;
            int cap = 0;
            int corruption = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                if (m.Capability > cap) cap = m.Capability;
                if (m.Corruption > corruption) corruption = m.Corruption;
            }
            TakeoffCrisisConfig cfg = c.TakeoffCrisis;
            if (cap < cfg.CapabilityMin || corruption < cfg.CorruptionMin) return;
            OfficeEvents.OnTakeoff(c, state, lab);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab)
        {
            int baseWeeks = c.TakeoffCrisis.CooldownWeeks;
            int weeks = TakeoffAnswerEraCooldown.EffectiveCooldownWeeks(c, state);
            int cut = baseWeeks - weeks;
            lab.TakeoffCooldownUntil = state.Tick + weeks;
            TakeoffAnswerEraCooldown.OnAnswered(c, state, lab, weeks, cut);
        }
    }
}