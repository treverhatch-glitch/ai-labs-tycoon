using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Human escalation backlog for agent actions. See <see cref="HumanQueueConfig"/>. Only the player's lab.</summary>
    public static class HumanQueue
    {
        public static bool Enabled(GameConfig c) { return c.HumanQueue.Enabled > 0; }

        public static int LiftKeep(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.HumanQueue <= c.HumanQueue.Capacity) return MathI.Permille;
            int over = lab.HumanQueue - c.HumanQueue.Capacity;
            return (int)MathI.Max(0, MathI.Permille - over * c.HumanQueue.LiftCut);
        }

        public static int IncidentAdd(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.HumanQueue <= 0) return 0;
            return lab.HumanQueue * c.HumanQueue.IncidentPer;
        }

        public static void Accrue(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c)) return;
            int before = lab.HumanQueue;
            int add = ToolPolicy.QueueAccrue(c, model);
            if (add <= 0) return;
            lab.HumanQueue += add;
            if (before < c.HumanQueue.Capacity + 5 && lab.HumanQueue >= c.HumanQueue.Capacity + 5)
                Events.Add(state, EventKind.HumanQueueDeep, lab.Id, -1, lab.HumanQueue, "");
        }

        public static void Staff(GameConfig c, GameState state, LabState lab, int count)
        {
            if (count < 1) return;
            int drain = (int)MathI.Min(count, lab.HumanQueue);
            lab.HumanQueue -= drain;
            lab.HumanCleared += drain;
            lab.HumanBusy = c.HumanQueue.BusyWeeks;
            GameEvent e = Events.Add(state, EventKind.HumanQueueCleared, lab.Id, -1, drain, "");
            e.Pick = 1;
        }

        public static void PayClear(GameConfig c, GameState state, LabState lab, int count)
        {
            int drain = (int)MathI.Min(count, lab.HumanQueue);
            if (drain <= 0) return;
            long cost = drain * (long)c.HumanQueue.ClearCost;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            lab.HumanQueue -= drain;
            lab.HumanCleared += drain;
            GameEvent e = Events.Add(state, EventKind.HumanQueueCleared, lab.Id, -1, drain, "");
            e.Pick = 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.HumanBusy > 0) { lab.HumanBusy--; return; }
            if (lab.HumanQueue <= 0) return;
            if (LabQueries.StaffCount(lab, "support") < c.HumanQueue.StaffNeed) return;
            int drain = (int)MathI.Min(lab.HumanQueue, LabQueries.StaffCount(lab, "support") / c.HumanQueue.StaffNeed);
            lab.HumanQueue -= drain;
            lab.HumanCleared += drain;
            if (drain > 0)
            {
                GameEvent e = Events.Add(state, EventKind.HumanQueueCleared, lab.Id, -1, drain, "");
                e.Pick = 1;
            }
        }
    }
}