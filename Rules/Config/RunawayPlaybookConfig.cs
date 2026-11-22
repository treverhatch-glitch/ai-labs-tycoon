using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Runaway playbook (bible ch. 10, runaway-agents risk): lab-wide response posture when agent blow-ups stack. A level sets how aggressively the lab caps the next blow-up; arming the playbook for a few weeks needs safety staff and a cooldown. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class RunawayPlaybookConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for the runaway playbook.</summary>
        public int MinEra;
        /// <summary>Highest posture level.</summary>
        public int MaxLevel;
        /// <summary>Cash to raise the posture one level.</summary>
        public int SetupPerLevel;
        /// <summary>Safety people needed to arm the playbook.</summary>
        public int Staff;
        /// <summary>Weeks before the playbook can be armed again.</summary>
        public int CooldownWeeks;
        /// <summary>Weeks the playbook stays armed.</summary>
        public int ArmWeeks;
        /// <summary>Share taken off weekly agent blow-up chance while armed (permille of the posture level).</summary>
        public int BlowupCut;
        /// <summary>Share of blow-up trust loss kept while armed (permille).</summary>
        public int TrustKeep;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("maxLevel", ref MaxLevel);
            s.Int("setupPerLevel", ref SetupPerLevel);
            s.Int("staff", ref Staff);
            s.Int("cooldownWeeks", ref CooldownWeeks);
            s.Int("armWeeks", ref ArmWeeks);
            s.Int("blowupCut", ref BlowupCut);
            s.Int("trustKeep", ref TrustKeep);
        }
    }
}