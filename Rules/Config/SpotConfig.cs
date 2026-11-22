using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Spot capacity. The player can run a share of the rented GPUs as cheap spot capacity that the cloud may take back (bible ch. 6, compute). The rent of that share falls, and each week every training run may be preempted: it loses what a failure would, by the run's checkpoint habit, and engineers cut the chance as they do for failed runs. A lab that never sets a share pays full rent and is never preempted. Only the player's lab. Zero in <c>Enabled</c> turns it off.
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

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPermille", ref MaxPermille);
            s.Int("discountPermille", ref DiscountPermille);
            s.Int("preemptPermille", ref PreemptPermille);
            s.Int("defaultLossPermille", ref DefaultLossPermille);
            s.Int("changeWeeks", ref ChangeWeeks);
        }
    }
}
