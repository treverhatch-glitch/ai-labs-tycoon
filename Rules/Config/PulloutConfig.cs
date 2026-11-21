using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Pulling a model out of one sector. The lab walks away from that sector's customers: it pays them a break fee of some weeks of what the model earned there,
    /// loses their trust and a little reputation, and gives up the share it had built. In return the model's serving load, and the revenue its incidents
    /// are fined against, leave with it. A model with no sector left is retired. Only the player's lab does it. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PulloutConfig : ISync
    {
        /// <summary>1 when a lab can pull a model out of a sector, 0 when it cannot.</summary>
        public int Enabled;
        /// <summary>Weeks of the model's last weekly revenue in that sector paid as a break fee.</summary>
        public int FeeWeeks;
        /// <summary>Trust lost in that sector (not carried to its family).</summary>
        public int TrustDrop;
        /// <summary>Reputation lost, before press damping.</summary>
        public int ReputationLoss;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("feeWeeks", ref FeeWeeks);
            s.Int("trustDrop", ref TrustDrop);
            s.Int("reputationLoss", ref ReputationLoss);
        }
    }
}
