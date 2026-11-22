using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Spot capacity. See <see cref="SpotConfig"/>. Only the player's lab.</summary>
    public static class Spot
    {
        public static bool Enabled(GameConfig c) { return c.Spot.Enabled > 0; }

        /// <summary>What the spot share takes off a week of the base rent.</summary>
        public static long Saving(GameConfig c, long baseRent, LabState lab)
        {
            if (!Enabled(c) || lab.SpotPermille <= 0) return 0;
            return MathI.MulPermille(MathI.MulPermille(baseRent, lab.SpotPermille), c.Spot.DiscountPermille);
        }

        /// <summary>The chance, in permille, that a run is preempted this week.</summary>
        public static int PreemptChance(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.SpotPermille <= 0) return 0;
            long chance = MathI.MulPermille(c.Spot.PreemptPermille, lab.SpotPermille);
            return (int)MathI.MulPermille(chance, MathI.Permille - TrainingRuns.EngineerSavingPermille(c, lab));
        }

        public static long LossShare(GameConfig c, ModelState model)
        {
            CheckpointConfig k = TrainingRuns.CheckpointOf(c, model);
            return k == null ? c.Spot.DefaultLossPermille : k.LossPermille;
        }

        static long Mix(GameState state, LabState lab, ModelState model)
        {
            long mix = (state.Seed * 37 + state.Tick) * 37 + lab.Id * 19 + model.Id * 11 + 5;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.SpotPermille <= 0) return;
            long base_ = LabQueries.CurrentGeneration(c, state.Tick).RentPerGpuWeek * lab.RentedGpus;
            lab.SpotSaved += Saving(c, base_, lab);
            int chance = PreemptChance(c, lab);
            if (chance <= 0) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Training) continue;
                if ((int)(Mix(state, lab, model) % MathI.Permille) >= chance) continue;
                long share = LossShare(c, model);
                long lost = MathI.MulPermille(model.ProgressCompute, share);
                if (lost <= 0) continue;
                model.ProgressCompute -= lost;
                model.AlignmentCompute -= MathI.MulPermille(model.AlignmentCompute, share);
                lab.SpotPreempts++;
                lab.SpotLost += lost;
                Events.Add(state, EventKind.SpotPreempted, lab.Id, model.Id, lost, model.Name);
            }
        }
    }
}
