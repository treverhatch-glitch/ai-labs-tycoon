using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class GpuTierUnlock
    {
        public static bool Enabled(GameConfig c) { return c.GpuTierUnlock.Enabled > 0; }

        public static bool Unlocked(GameConfig c, GameState state, LabState lab, GpuGenerationConfig gen)
        {
            if (!Enabled(c)) return true;
            if (gen.Tier < c.GpuTierUnlock.TierMin) return true;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.GpuTierUnlock.MinEra) return false;
            if (c.GpuTierUnlock.UnlockPermille > 0 && lab.Reputation < c.GpuTierUnlock.UnlockPermille) return false;
            return true;
        }

        public static void Refuse(GameConfig c, GameState state, LabState lab, GpuGenerationConfig gen, int pick)
        {
            if (!Enabled(c)) return;
            GameEvent ev = Events.Add(state, EventKind.GpuTierLocked, lab.Id, gen.Tier, pick, gen.Id);
            ev.Pick = pick;
        }
    }
}