using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Spot capacity. The player can run a share of the rented GPUs as cheap spot capacity that the cloud may take back (bible ch. 6, compute). The rent of that share falls. Each week the cloud may call a reclaim wave, less likely with engineers: every run that is not rushed in the run queue is interrupted and loses what a failure would by its checkpoint habit, and for the weeks the wave lasts the spot share of those runs does no work. A rushed run sits on reserved capacity and is neither interrupted nor slowed. A lab that never sets a share pays full rent and is never preempted. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SpotConfig : ISync
    {
        public int Enabled;
        /// <summary>Largest share of the rented GPUs that can run as spot capacity (permille).</summary>
        public int MaxPermille;
        /// <summary>Share taken off the rent of the spot share of the fleet (permille).</summary>
        public int DiscountPermille;
        /// <summary>Weekly chance, at a full spot share, that each training run is preempted (permille).</summary>
        public int PreemptPermille;
        /// <summary>Share of a run that a preemption throws away when the run has no checkpoint habit set (permille).</summary>
        public int DefaultLossPermille;
        /// <summary>Weeks before the spot share can be changed again.</summary>
        public int ChangeWeeks;

        /// <summary>Weeks a reclaim wave lasts: the spot share of every run that is not rushed does no work.</summary>
        public int ReclaimWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPermille", ref MaxPermille);
            s.Int("discountPermille", ref DiscountPermille);
            s.Int("preemptPermille", ref PreemptPermille);
            s.Int("defaultLossPermille", ref DefaultLossPermille);
            s.Int("changeWeeks", ref ChangeWeeks);
            s.Int("reclaimWeeks", ref ReclaimWeeks);
        }
    }
}
