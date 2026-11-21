using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The scaling policy. The lab can commit to a policy of up to a few levels. Level k has a tripwire at a capability that rises with k; a trained model that reaches a tripwire of the policy cannot go on sale, and no new run can start, until the lab holds the safeguards for that level (a security tier and safety people for each level) or breaks its policy. Holding a policy softens scandals and weakens negligence claims. Raising it costs a fee; lowering or breaking it costs reputation, and the lowered level is locked for a while. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ScalingConfig : ISync
    {
        public int Enabled;
        /// <summary>The highest level.</summary>
        public int MaxLevel;
        /// <summary>The capability of level 1's tripwire.</summary>
        public int FirstTripwire;
        /// <summary>What each level above the first adds to the tripwire's capability.</summary>
        public int TripwireStep;
        /// <summary>Safety people the safeguards of each level need (a security tier of the same number is needed too).</summary>
        public int SafetyPerTier;
        /// <summary>What raising the policy costs for each level it goes up.</summary>
        public int AdoptFee;
        /// <summary>Each level takes this much off the reputation a scandal costs.</summary>
        public int DampPermille;
        /// <summary>Each level takes this much off the strength of a negligence claim.</summary>
        public int ClaimStrength;
        /// <summary>Reputation lowering the policy costs, before the press damps it.</summary>
        public int WalkBackReputation;
        /// <summary>Reputation breaking the policy costs, before the press damps it.</summary>
        public int OverrideReputation;
        /// <summary>Morale the safety staff lose when the policy is broken.</summary>
        public int OverrideMorale;
        /// <summary>Weeks after a change or a break before the policy can be lowered.</summary>
        public int LockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxLevel", ref MaxLevel);
            s.Int("firstTripwire", ref FirstTripwire);
            s.Int("tripwireStep", ref TripwireStep);
            s.Int("safetyPerTier", ref SafetyPerTier);
            s.Int("adoptFee", ref AdoptFee);
            s.Int("dampPermille", ref DampPermille);
            s.Int("claimStrength", ref ClaimStrength);
            s.Int("walkBackReputation", ref WalkBackReputation);
            s.Int("overrideReputation", ref OverrideReputation);
            s.Int("overrideMorale", ref OverrideMorale);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }
}
