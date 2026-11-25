using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Hack week. See <see cref="HackWeekConfig"/>. Only the player's lab holds one.</summary>
    public static class HackWeek
    {
        public static bool Enabled(GameConfig c) { return c.Hack.Weeks > 0; }

        /// <summary><c>HackWeeks</c> counts the weeks still to run, so it is above 0 only while one is on.</summary>
        public static bool Active(LabState lab) { return lab.HackWeeks > 0; }

        public static int Researchers(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Research); }

        public static long Cost(GameConfig c, LabState lab) { return (long)lab.Staff.Count * c.Hack.CostPerStaff; }

        public static int ResearchPermille(GameConfig c, LabState lab) { return Active(lab) ? c.Hack.ResearchPermille : 0; }

        public static int TrainingCutPermille(GameConfig c, LabState lab) { return Active(lab) ? c.Hack.TrainingCutPermille : 0; }

        public static void Hold(GameConfig c, GameState state, LabState lab, long cost)
        {
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Min(MathI.Permille, lab.Staff[i].Morale + c.Hack.MoraleLift);
            lab.HackWeeks = c.Hack.Weeks;
            lab.HackLockedUntil = state.Tick + c.Hack.CooldownWeeks;
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            GameEvent e = Events.Add(state, EventKind.HackWeek, lab.Id, c.Hack.Weeks, cost, "");
            e.Pick = 1;
        }

        /// <summary>One week of it: the count runs down and the last week is logged as the end.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return;
            lab.HackWeeks--;
            if (lab.HackWeeks > 0) return;
            GameEvent e = Events.Add(state, EventKind.HackWeek, lab.Id, c.Hack.Weeks, 0, "");
            e.Pick = 0;
        }
    }
}
