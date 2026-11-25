using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Weekly agent orchestration budget. See <see cref="OrchestrationBudgetConfig"/>. Only the player's lab.</summary>
    public static class OrchestrationBudget
    {
        public static bool Enabled(GameConfig c) { return c.OrchestrationBudget.Enabled > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.OrchLevel, 0, 3); }

        public static int Slots(GameConfig c, LabState lab)
        {
            int l = Level(c, lab);
            if (l == 1) return c.OrchestrationBudget.Slots1;
            if (l == 2) return c.OrchestrationBudget.Slots2;
            if (l == 3) return c.OrchestrationBudget.Slots3;
            return 0;
        }

        public static int KeepPermille(GameConfig c, LabState lab)
        {
            int l = Level(c, lab);
            if (l == 1) return c.OrchestrationBudget.Keep1;
            if (l == 2) return c.OrchestrationBudget.Keep2;
            if (l == 3) return c.OrchestrationBudget.Keep3;
            return MathI.Permille;
        }

        static int AgentModels(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status == ModelStatus.Released && m.AgentLevel > 0) n++;
            }
            return n;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int added = (int)MathI.Max(0, level - Level(c, lab));
            long cost = added * (long)c.OrchestrationBudget.SetupPerLevel;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            lab.OrchLevel = level;
            Events.Add(state, EventKind.OrchBudgetSet, lab.Id, level, cost, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int slots = Slots(c, lab);
            int agents = AgentModels(lab);
            int over = agents - slots;
            if (over > 0) lab.OrchOver += over;
            else if (lab.OrchOver > 0 && c.OrchestrationBudget.OverDecay > 0)
                lab.OrchOver = (int)MathI.Max(0, lab.OrchOver - c.OrchestrationBudget.OverDecay);
            if (lab.OrchOver > 0 && HumanQueue.Enabled(c) && c.OrchestrationBudget.QueuePerOver > 0)
            {
                int before = lab.HumanQueue;
                lab.HumanQueue += lab.OrchOver * c.OrchestrationBudget.QueuePerOver;
                if (before < c.HumanQueue.Capacity + 5 && lab.HumanQueue >= c.HumanQueue.Capacity + 5)
                    Events.Add(state, EventKind.HumanQueueDeep, lab.Id, -1, lab.HumanQueue, "");
            }
        }
    }
}