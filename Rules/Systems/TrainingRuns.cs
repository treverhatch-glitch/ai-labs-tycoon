using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The approach and checkpoint habit a run was set up with: what they add, what a failure costs, and when one happens.</summary>
    public static class TrainingRuns
    {
        public static ApproachConfig ApproachOf(GameConfig c, ModelState model)
        {
            return c.Runs.Approach(string.IsNullOrEmpty(model.Approach) ? c.Runs.DefaultApproachId : model.Approach);
        }

        public static CheckpointConfig CheckpointOf(GameConfig c, ModelState model)
        {
            return c.Runs.Checkpoint(string.IsNullOrEmpty(model.Checkpoints) ? c.Runs.DefaultCheckpointId : model.Checkpoints);
        }

        /// <summary>The chance, in permille, that the run fails this week.</summary>
        public static int FailureChancePermille(GameConfig c, ModelState model)
        {
            ApproachConfig a = ApproachOf(c, model);
            return a == null ? 0 : a.FailurePermillePerWeek;
        }

        /// <summary>What a failure would throw away right now, in compute.</summary>
        public static long LossIfFailed(GameConfig c, ModelState model)
        {
            CheckpointConfig k = CheckpointOf(c, model);
            return k == null ? 0 : MathI.MulPermille(model.ProgressCompute, k.LossPermille);
        }

        /// <summary>The money a week of this habit costs the lab.</summary>
        public static long WeeklyCost(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            CheckpointConfig k = CheckpointOf(c, model);
            if (k == null || k.WeeklyCostPermille <= 0) return 0;
            return MathI.MulPermille(LabQueries.WeeklyRunningCost(c, state, lab), k.WeeklyCostPermille);
        }

        static long Mix(GameState state, LabState lab, ModelState model)
        {
            long mix = (state.Seed * 31 + state.Tick) * 31 + lab.Id * 17 + model.Id * 7 + 3;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        /// <summary>One training week for a model: the checkpoint habit is paid for, then the run may fail. A run that has not been set up does neither.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long cost = WeeklyCost(c, state, lab, model);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, MathI.Min(cost, MathI.Max(0, lab.Cash)));

            int chance = FailureChancePermille(c, model);
            if (chance <= 0 || (int)(Mix(state, lab, model) % MathI.Permille) >= chance) return;
            CheckpointConfig k = CheckpointOf(c, model);
            long lost = MathI.MulPermille(model.ProgressCompute, k.LossPermille);
            if (lost <= 0) return;
            model.ProgressCompute -= lost;
            model.AlignmentCompute -= MathI.MulPermille(model.AlignmentCompute, k.LossPermille);
            Events.Add(state, EventKind.TrainingFailed, lab.Id, model.Id, lost, model.Name);
        }

        /// <summary>Adds the approach's reach to a model whose run has just finished.</summary>
        public static void OnTrained(GameConfig c, ModelState model)
        {
            ApproachConfig a = ApproachOf(c, model);
            if (a == null || a.CapabilityPermille == 0) return;
            long delta = MathI.MulPermille(model.Capability, a.CapabilityPermille);
            if (model.Capability + delta < 1) delta = 1 - model.Capability;
            model.CapLuck += (int)delta;
            model.Capability += (int)delta;
        }
    }
}
