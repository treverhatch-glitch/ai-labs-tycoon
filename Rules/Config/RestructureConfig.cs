using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Restructuring a loan. The lender agrees to stretch what is left over a longer term, so the weekly payment falls, for a fee added to what is owed, a dent in the
    /// lab's name and a clean sheet of missed payments. It works only while it would lower the payment, so it cannot be repeated for nothing. Only the player's lab
    /// does it. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class RestructureConfig : ISync
    {
        /// <summary>1 when loans can be restructured, 0 when they cannot.</summary>
        public int Enabled;
        /// <summary>The new term as a share of the loan term in the data, in permille (2000 doubles it). What is left is spread over that many weeks.</summary>
        public int StretchPermille;
        /// <summary>Added to what is owed, in permille of the balance.</summary>
        public int FeePermille;
        /// <summary>Reputation lost, before press damping.</summary>
        public int ReputationLoss;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("stretchPermille", ref StretchPermille);
            s.Int("feePermille", ref FeePermille);
            s.Int("reputationLoss", ref ReputationLoss);
        }
    }
}
