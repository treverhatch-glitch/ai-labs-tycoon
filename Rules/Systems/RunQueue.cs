using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A priority queue across the training runs. See <see cref="RunQueueConfig"/>. Only the player's lab.</summary>
    public static class RunQueue
    {
        public static bool Enabled(GameConfig c) { return c.RunQueue.Enabled > 0; }

        public static int Level(ModelState m) { return m.QueueLevel == 0 ? 2 : m.QueueLevel; }

        public static int Weight(GameConfig c, ModelState m)
        {
            int level = Level(m);
            return level == 1 ? c.RunQueue.WeightBackground : level == 3 ? c.RunQueue.WeightRush : c.RunQueue.WeightNormal;
        }

        public static int Rushed(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status == ModelStatus.Training && lab.Models[i].QueueLevel == 3) n++;
            return n;
        }

        /// <summary>The compute each run in training gets this week, oldest run first. The oldest gets what does not divide evenly, so none is lost.</summary>
        public static long[] Shares(GameConfig c, LabState lab, long training)
        {
            int runs = LabQueries.ActiveRuns(lab);
            var shares = new long[runs];
            if (runs == 0) return shares;
            bool queued = false;
            if (Enabled(c))
                for (int i = 0; i < lab.Models.Count; i++)
                    if (lab.Models[i].Status == ModelStatus.Training && lab.Models[i].QueueLevel != 0) queued = true;
            long total = 0;
            int k = 0;
            if (!queued)
            {
                long each = training / runs;
                for (int i = 0; i < runs; i++) shares[i] = each;
                shares[0] += training - each * runs;
                return shares;
            }
            long weights = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status == ModelStatus.Training) weights += Weight(c, lab.Models[i]);
            for (int i = 0; i < lab.Models.Count; i++)
            {
                if (lab.Models[i].Status != ModelStatus.Training) continue;
                shares[k] = training * Weight(c, lab.Models[i]) / weights;
                total += shares[k];
                k++;
            }
            shares[0] += training - total;
            return shares;
        }

        public static long SwitchCost(GameConfig c, ModelState m) { return MathI.MulPermille(m.ProgressCompute, c.RunQueue.SwitchCostPermille); }

        public static void Set(GameConfig c, GameState state, LabState lab, ModelState m, int level)
        {
            long lost = SwitchCost(c, m);
            m.ProgressCompute -= lost;
            m.QueueLevel = level;
            Events.Add(state, EventKind.RunPriorityChanged, lab.Id, m.Id, lost, m.Name).Pick = level;
        }
    }
}
