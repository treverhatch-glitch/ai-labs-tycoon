using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Incident response and the on-call rota. See <see cref="OnCallConfig"/>. Only the player's lab runs one.</summary>
    public static class OnCall
    {
        public static bool Enabled(GameConfig c) { return c.OnCall.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.OnCallLevel, 0, c.OnCall.MaxLevel); }

        /// <summary>The safety people and engineers on the payroll.</summary>
        public static int Staff(GameConfig c, LabState lab)
        {
            return LabQueries.StaffByFunction(c, lab, RoleFunction.Safety) + LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering);
        }

        /// <summary>The highest level the people on the payroll can staff right now.</summary>
        public static int StaffCover(GameConfig c, LabState lab)
        {
            int per = c.OnCall.StaffPerLevel;
            int covered = per <= 0 ? c.OnCall.MaxLevel : Staff(c, lab) / per;
            return (int)MathI.Min(c.OnCall.MaxLevel, covered);
        }

        /// <summary>The level that counts: the set level, held down by the people behind it.</summary>
        public static int Effective(GameConfig c, LabState lab)
        {
            int level = Level(c, lab);
            return level == 0 ? 0 : (int)MathI.Min(level, StaffCover(c, lab));
        }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.OnCallLockedUntil; }

        public static long WeeklyCost(GameConfig c, LabState lab) { return Effective(c, lab) * c.OnCall.WeeklyCostPerLevel; }

        public static int FineCutPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Effective(c, lab) * (long)c.OnCall.FineCutPermillePerLevel);
        }

        public static int ReputationCutPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Effective(c, lab) * (long)c.OnCall.ReputationCutPermillePerLevel);
        }

        public static int MoraleDropCutPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(MathI.Permille, Effective(c, lab) * (long)c.OnCall.MoraleDropCutPermillePerLevel);
        }

        /// <summary>The points off where the team's morale settles.</summary>
        public static int MoralePenalty(GameConfig c, LabState lab) { return (int)(Effective(c, lab) * (long)c.OnCall.MoralePenaltyPerLevel); }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int was = Level(c, lab);
            lab.OnCallLevel = level;
            lab.OnCallLockedUntil = state.Tick + c.OnCall.LockWeeks;
            GameEvent e = Events.Add(state, EventKind.OnCallSet, lab.Id, level, 0, "");
            e.Pick = level > was ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            long cost = WeeklyCost(c, lab);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Salaries, cost);
        }
    }
}
