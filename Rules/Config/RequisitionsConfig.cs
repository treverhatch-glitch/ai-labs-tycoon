using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Open requisitions. The player can order people in a role for a future date at a locked, lower price, paying a deposit now. On the day the people arrive each needs a free seat and the rest of the fee; anyone who cannot be seated or paid for is lost and the deposit with them. Cancelling forfeits the deposit. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class RequisitionsConfig : ISync
    {
        public int Enabled;
        /// <summary>Most orders open at once.</summary>
        public int MaxOpen;
        /// <summary>Most people in one order.</summary>
        public int MaxCount;
        /// <summary>Weeks until the people arrive.</summary>
        public int LeadWeeks;
        /// <summary>The locked price, as a share of the hiring fee on the day of the order.</summary>
        public int FeePermille;
        /// <summary>The deposit paid now, as a share of the locked price; lost when the order is cancelled or cannot be filled.</summary>
        public int DepositPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("maxCount", ref MaxCount);
            s.Int("leadWeeks", ref LeadWeeks);
            s.Int("feePermille", ref FeePermille);
            s.Int("depositPermille", ref DepositPermille);
        }
    }
}
