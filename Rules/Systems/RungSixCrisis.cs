using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class RungSixCrisis
    {
        public static bool Enabled(GameConfig c) { return c.RungSixCrisis.Enabled > 0; }

        public static void OnArcAnswered(GameConfig c, GameState state, LabState lab, OfficeTrigger from)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (from != OfficeTrigger.Escape && from != OfficeTrigger.Takeoff) return;
            lab.RungSixArcUntil = state.Tick + c.RungSixCrisis.ArcWeeks;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            if (OfficeEvents.IsPending(lab)) return;
            if (state.Tick < lab.RungSixCooldownUntil) return;
            if (state.Tick > lab.RungSixArcUntil) return;
            int era = EraSystem.IndexOf(c, EraSystem.AtTick(c, state.Tick).Id);
            if (era < c.RungSixCrisis.MinEra) return;
            int corruption = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                if (m.Corruption > corruption) corruption = m.Corruption;
            }
            if (corruption < c.RungSixCrisis.CorruptionMin) return;
            OfficeEvents.OnRungSix(c, state, lab);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab)
        {
            lab.RungSixCooldownUntil = state.Tick + c.RungSixCrisis.CooldownWeeks;
            lab.RungSixArcUntil = 0;
        }
    }
}