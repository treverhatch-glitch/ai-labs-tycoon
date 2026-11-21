using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Technical debt. See <see cref="TechDebtConfig"/>. Only the player's lab keeps it.</summary>
    public static class TechDebt
    {
        public const int Cap = 1000;

        public static bool Enabled(GameConfig c) { return c.TechDebt.Enabled > 0; }

        public static void Add(GameConfig c, GameState state, LabState lab, int points)
        {
            if (!Enabled(c) || points <= 0 || lab.Id != state.PlayerLabId) return;
            lab.TechDebt = (int)MathI.Min(Cap, lab.TechDebt + (long)points);
        }

        /// <summary>The extra weekly incident chance the lab's debt adds, in permille.</summary>
        public static int IncidentPermille(GameConfig c, LabState lab)
        {
            if (lab.TechDebt <= 0 || !Enabled(c)) return 0;
            return (int)((long)c.TechDebt.IncidentAtFull * lab.TechDebt / Cap);
        }

        public static long Cost(GameConfig c, int points) { return (long)c.TechDebt.CostPerPoint * points; }

        public static void PayDown(GameConfig c, GameState state, LabState lab, int points)
        {
            lab.TechDebt = (int)MathI.Max(0, lab.TechDebt - (long)points);
            Finance.Spend(lab, LedgerLine.Training, Cost(c, points));
            GameEvent e = Events.Add(state, EventKind.DebtPaid, lab.Id, lab.TechDebt, points, "");
            e.Pick = lab.TechDebt == 0 ? 1 : 0;
        }
    }
}
