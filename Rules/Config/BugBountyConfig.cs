using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The bug bounty. A standing programme that pays outside researchers to find faults in the lab's released models before they find the customers. It takes a
    /// fee to raise, a bill every week for every model on sale, and takes a share off the weekly chance of an incident for as long as it runs. Changing the level
    /// locks it for some weeks, so it cannot be switched on only when the risk is high. Only the player's lab runs one. Zero in <c>MaxLevel</c> turns it off.
    /// </summary>
    public sealed class BugBountyConfig : ISync
    {
        /// <summary>The top level; 0 turns the bounty off.</summary>
        public int MaxLevel;
        /// <summary>The fee for each level a change raises the programme by. Lowering it is free.</summary>
        public long SetupPerLevel;
        /// <summary>What each level costs every week for every released model.</summary>
        public long WeeklyPerLevel;
        /// <summary>The share of the weekly incident chance each level takes off, in permille.</summary>
        public int CutPerLevel;
        /// <summary>Weeks after a change before the level can change again.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("setupPerLevel", ref SetupPerLevel);
            s.Long("weeklyPerLevel", ref WeeklyPerLevel);
            s.Int("cutPerLevel", ref CutPerLevel);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}
