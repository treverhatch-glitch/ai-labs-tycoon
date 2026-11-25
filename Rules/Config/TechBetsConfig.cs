using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A public promise to research a particular tech within a set number of weeks. Keeping it earns reputation and prestige; letting the weeks run out costs reputation; taking it back costs a little. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class TechBetsConfig : ISync
    {
        public int Enabled;
        /// <summary>Weeks the lab has to research the tech it has publicly bet on.</summary>
        public int Weeks;
        /// <summary>Reputation earned for keeping the promise.</summary>
        public int WinReputation;
        /// <summary>Prestige earned for keeping it.</summary>
        public int WinPrestige;
        /// <summary>Reputation lost when the weeks run out first (damped).</summary>
        public int LossReputation;
        /// <summary>Reputation lost for taking the bet back (damped).</summary>
        public int WithdrawReputation;
        /// <summary>Research points a tech must cost to be worth a public bet.</summary>
        public int MinCost;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("weeks", ref Weeks);
            s.Int("winReputation", ref WinReputation);
            s.Int("winPrestige", ref WinPrestige);
            s.Int("lossReputation", ref LossReputation);
            s.Int("withdrawReputation", ref WithdrawReputation);
            s.Int("minCost", ref MinCost);
        }
    }
}
