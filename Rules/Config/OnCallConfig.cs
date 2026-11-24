using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Incident response: the player puts safety people and engineers on an on-call rota. Each level softens what an incident costs (the fine,
    /// the reputation and the blow to the team's morale when it lands) for a weekly fee, and wears the team down a little because nobody can
    /// switch off. The rota needs the people behind it: the level that counts is held down by the safety staff and engineers on the payroll.
    /// A change locks the rota for some weeks. Only the player's lab does it. Zero levels turns it off.
    /// </summary>
    public sealed class OnCallConfig : ISync
    {
        /// <summary>The highest level. Zero turns it off.</summary>
        public int MaxLevel;
        /// <summary>Cash each week for each level that counts.</summary>
        public long WeeklyCostPerLevel;
        /// <summary>The safety people and engineers (together) each level needs on the payroll.</summary>
        public int StaffPerLevel;
        /// <summary>The share each level takes off an incident's fine, in permille.</summary>
        public int FineCutPermillePerLevel;
        /// <summary>The share each level takes off an incident's reputation loss, in permille.</summary>
        public int ReputationCutPermillePerLevel;
        /// <summary>The share each level takes off the morale the team loses when an incident lands, in permille.</summary>
        public int MoraleDropCutPermillePerLevel;
        /// <summary>The points each level takes off where the team's morale settles.</summary>
        public int MoralePenaltyPerLevel;
        /// <summary>Weeks the rota cannot be changed after it was.</summary>
        public int LockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("weeklyCostPerLevel", ref WeeklyCostPerLevel);
            s.Int("staffPerLevel", ref StaffPerLevel);
            s.Int("fineCutPermillePerLevel", ref FineCutPermillePerLevel);
            s.Int("reputationCutPermillePerLevel", ref ReputationCutPermillePerLevel);
            s.Int("moraleDropCutPermillePerLevel", ref MoraleDropCutPermillePerLevel);
            s.Int("moralePenaltyPerLevel", ref MoralePenaltyPerLevel);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }
}
