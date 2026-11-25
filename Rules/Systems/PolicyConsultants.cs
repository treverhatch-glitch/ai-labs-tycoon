using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Policy consultants. See <see cref="ConsultantsConfig"/>. Only the player's lab hires them.</summary>
    public static class PolicyConsultants
    {
        public static bool Enabled(GameConfig c) { return c.Consultants.MaxLevel > 0 && c.Consultants.Weeks > 0; }

        public static bool Running(LabState lab) { return lab.ConsultantWeeks > 0 && lab.ConsultantLevel > 0; }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.ConsultantLockedUntil; }

        public static long Fee(GameConfig c, int level) { return level * c.Consultants.FeePerLevel; }

        public static long WeeklyCost(GameConfig c, LabState lab)
        {
            return Running(lab) ? MathI.Clamp(lab.ConsultantLevel, 0, c.Consultants.MaxLevel) * c.Consultants.WeeklyPerLevel : 0;
        }

        /// <summary>The policy people the firm counts as right now. Compliance never counts them.</summary>
        public static int Staff(GameConfig c, LabState lab)
        {
            return Running(lab) ? (int)(MathI.Clamp(lab.ConsultantLevel, 0, c.Consultants.MaxLevel) * (long)c.Consultants.StaffPerLevel) : 0;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, int level, long fee)
        {
            if (fee > 0) Finance.Spend(lab, LedgerLine.Hiring, fee);
            lab.ConsultantLevel = level;
            lab.ConsultantWeeks = c.Consultants.Weeks;
            lab.ConsultantLockedUntil = state.Tick + c.Consultants.CooldownWeeks;
            GameEvent e = Events.Add(state, EventKind.ConsultantsHired, lab.Id, level, fee, "");
            e.Pick = 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Running(lab)) return;
            long cost = WeeklyCost(c, lab);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Salaries, cost);
            lab.ConsultantWeeks--;
            if (lab.ConsultantWeeks > 0) return;
            int level = lab.ConsultantLevel;
            lab.ConsultantWeeks = 0;
            lab.ConsultantLevel = 0;
            GameEvent e = Events.Add(state, EventKind.ConsultantsHired, lab.Id, level, 0, "");
            e.Pick = 0;
        }
    }
}
