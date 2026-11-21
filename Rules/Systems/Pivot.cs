using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>A change of style: who leaves, what it costs, and how long customers are confused. All of it is worked out from the books and the staff list.</summary>
    public static class Pivot
    {
        public static string EraId(GameConfig c, GameState state) { return EraSystem.AtTick(c, state.Tick).Id; }

        public static bool Spent(GameConfig c, GameState state, LabState lab) { return lab.PivotEraId == EraId(c, state); }

        public static long Cost(GameConfig c, GameState state, LabState lab)
        {
            return c.Pivot.CostWeeks * MathI.Max(c.Pivot.MinWeekCost, LabQueries.WeeklyRunningCost(c, state, lab));
        }

        /// <summary>How many people leave: the share of the staff, rounded down, at least one when there is anyone to leave.</summary>
        public static int Leavers(GameConfig c, LabState lab)
        {
            if (lab.Staff.Count == 0) return 0;
            return (int)MathI.Max(1, MathI.MulPermille(lab.Staff.Count, c.Pivot.StaffLossPermille));
        }

        /// <summary>The multiplier on every model's appeal, in permille: 1000 normally, less while customers are confused.</summary>
        public static int AppealPermille(GameConfig c, GameState state, LabState lab)
        {
            return state.Tick < lab.PivotUntilTick ? MathI.Permille - c.Pivot.ConfusionAppealPermille : MathI.Permille;
        }

        public static int ConfusionWeeksLeft(GameState state, LabState lab)
        {
            int left = lab.PivotUntilTick - state.Tick;
            return left > 0 ? left : 0;
        }

        /// <summary>The people who leave are the unhappiest first; a tie goes to the one hired earlier.</summary>
        public static int Leave(GameConfig c, LabState lab)
        {
            int n = Leavers(c, lab);
            for (int removed = 0; removed < n; removed++)
            {
                int pick = 0;
                for (int i = 1; i < lab.Staff.Count; i++)
                {
                    StaffMember p = lab.Staff[i];
                    StaffMember best = lab.Staff[pick];
                    if (p.Morale < best.Morale || (p.Morale == best.Morale && p.Id < best.Id)) pick = i;
                }
                lab.Staff.RemoveAt(pick);
            }
            return n;
        }
    }
}
