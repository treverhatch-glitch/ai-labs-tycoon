using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The bug bounty. See <see cref="BugBountyConfig"/>. Only the player's lab runs one, and a lab at level 0 pays and gains nothing.</summary>
    public static class BugBounty
    {
        public static bool Enabled(GameConfig c) { return c.BugBounty.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.BountyLevel, 0, c.BugBounty.MaxLevel); }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.BountyLockedUntil; }

        public static int ReleasedModels(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Released) n++;
            return n;
        }

        /// <summary>What raising the programme to this level costs; nothing for staying or lowering it.</summary>
        public static long Fee(GameConfig c, LabState lab, int level)
        {
            return level > Level(c, lab) ? (level - Level(c, lab)) * c.BugBounty.SetupPerLevel : 0;
        }

        public static long WeeklyCost(GameConfig c, LabState lab)
        {
            return Level(c, lab) * c.BugBounty.WeeklyPerLevel * ReleasedModels(lab);
        }

        public static int CutPermille(GameConfig c, LabState lab) { return (int)MathI.Min(MathI.Permille, Level(c, lab) * (long)c.BugBounty.CutPerLevel); }

        /// <summary>What the programme takes off a weekly chance: zero or negative.</summary>
        public static int IncidentChange(GameConfig c, LabState lab, long chance)
        {
            return lab.BountyLevel <= 0 ? 0 : (int)-MathI.MulPermille(chance, CutPermille(c, lab));
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level, long fee)
        {
            if (fee > 0) Finance.Spend(lab, LedgerLine.Hiring, fee);
            int before = Level(c, lab);
            lab.BountyLevel = level;
            lab.BountyLockedUntil = state.Tick + c.BugBounty.CooldownWeeks;
            GameEvent e = Events.Add(state, EventKind.BountySet, lab.Id, level, fee, "");
            e.Pick = level > before ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.BountyLevel <= 0) return;
            long cost = WeeklyCost(c, lab);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Salaries, cost);
        }
    }
}
