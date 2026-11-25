using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A priority queue across the lab's training runs. Without it every run gets an equal share of the training compute. Each run can be set to background, normal or rush; the compute is shared by those weights. Changing a run's level costs it part of its progress (reloading from a checkpoint) and only so many runs may be rushed at once. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class RunQueueConfig : ISync
    {
        public int Enabled;
        /// <summary>Share weight of a background run.</summary>
        public int WeightBackground;
        /// <summary>Share weight of a normal run (also a run with no level set).</summary>
        public int WeightNormal;
        /// <summary>Share weight of a rush run.</summary>
        public int WeightRush;
        /// <summary>Share of a run progress lost when its level is changed (reloading from checkpoint, permille).</summary>
        public int SwitchCostPermille;
        /// <summary>Most runs that may be rushed at once.</summary>
        public int MaxRush;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("weightBackground", ref WeightBackground);
            s.Int("weightNormal", ref WeightNormal);
            s.Int("weightRush", ref WeightRush);
            s.Int("switchCostPermille", ref SwitchCostPermille);
            s.Int("maxRush", ref MaxRush);
        }
    }
}
