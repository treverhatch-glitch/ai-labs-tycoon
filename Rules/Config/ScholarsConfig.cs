using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Visiting scholars. The player can host a group of scholars at researchers' desks for a set number of weeks. They are paid a stipend and add research points each week at a share of a researcher's rate. When they leave on schedule each adds prestige to the lab. Sending them home early earns no prestige. One group at a time; only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ScholarsConfig : ISync
    {
        public int Enabled;
        /// <summary>Most scholars in a group.</summary>
        public int MaxScholars;
        /// <summary>Weeks a group stays.</summary>
        public int Weeks;
        /// <summary>Stipend over the weeks, per scholar, as a share of a researcher's hiring fee.</summary>
        public int StipendPermille;
        /// <summary>A scholar's weekly research points, as a share of a researcher's.</summary>
        public int ResearchPermille;
        /// <summary>Prestige each scholar adds to the lab when they leave on schedule.</summary>
        public int PrestigeEach;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxScholars", ref MaxScholars);
            s.Int("weeks", ref Weeks);
            s.Int("stipendPermille", ref StipendPermille);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("prestigeEach", ref PrestigeEach);
        }
    }
}
