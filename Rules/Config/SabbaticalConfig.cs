using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Sabbaticals. The player can send a person away for a set number of weeks on full pay: they do no work and cannot quit while they are gone. How they come back depends on how they went: someone who was worn down returns refreshed, someone who was already happy comes back restless, everyone else returns a little better than settled. At most a few people are away at once. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SabbaticalConfig : ISync
    {
        public int Enabled;
        /// <summary>Weeks a sabbatical lasts.</summary>
        public int Weeks;
        /// <summary>Most people away at once.</summary>
        public int MaxAway;
        /// <summary>Morale below which a person is worn down when they go.</summary>
        public int BurnoutBelow;
        /// <summary>Morale a worn-down person returns with.</summary>
        public int RefreshedMorale;
        /// <summary>Morale above which a person is already happy when they go.</summary>
        public int RestlessAbove;
        /// <summary>Morale such a person returns with.</summary>
        public int RestlessMorale;
        /// <summary>What everyone else returns with, added to where people settle.</summary>
        public int RestedBonus;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("weeks", ref Weeks);
            s.Int("maxAway", ref MaxAway);
            s.Int("burnoutBelow", ref BurnoutBelow);
            s.Int("refreshedMorale", ref RefreshedMorale);
            s.Int("restlessAbove", ref RestlessAbove);
            s.Int("restlessMorale", ref RestlessMorale);
            s.Int("restedBonus", ref RestedBonus);
        }
    }
}
