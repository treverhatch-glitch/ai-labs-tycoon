using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Board targets. See <see cref="RevenueTargetsConfig"/>. Only the player's lab sets one, and nothing runs until it does.</summary>
    public static class RevenueTargets
    {
        public static bool Enabled(GameConfig c) { return c.RevenueTargets.MaxLevel > 0; }

        public static bool Active(LabState lab) { return lab.RevenueTargetLevel > 0; }

        /// <summary>What a target of this level would ask of the lab now: the last week's revenue over all the weeks, stretched.</summary>
        public static long Goal(GameConfig c, LabState lab, int level)
        {
            return MathI.MulPermille(lab.LastWeek.Revenue * c.RevenueTargets.Weeks, MathI.Permille + (long)c.RevenueTargets.StretchPerLevel * level);
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            lab.RevenueTargetLevel = level;
            lab.RevenueTargetEnd = state.Tick + c.RevenueTargets.Weeks;
            lab.RevenueTargetGoal = Goal(c, lab, level);
            lab.RevenueTargetSoFar = 0;
            Events.Add(state, EventKind.TargetSet, lab.Id, level, lab.RevenueTargetGoal, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return;
            lab.RevenueTargetSoFar += MathI.Max(0, lab.LastWeek.Revenue);
            if (state.Tick + 1 < lab.RevenueTargetEnd) return;
            bool met = lab.RevenueTargetSoFar >= lab.RevenueTargetGoal;
            int level = lab.RevenueTargetLevel;
            int change = met ? c.RevenueTargets.RewardPerLevel * level : -c.RevenueTargets.PenaltyPerLevel * level;
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + change, 0, MathI.Permille);
            GameEvent e = Events.Add(state, EventKind.TargetSettled, lab.Id, level, lab.RevenueTargetSoFar, "");
            e.Pick = met ? 1 : 0;
            lab.RevenueTargetLevel = 0;
            lab.RevenueTargetEnd = 0;
            lab.RevenueTargetGoal = 0;
            lab.RevenueTargetSoFar = 0;
        }
    }
}
