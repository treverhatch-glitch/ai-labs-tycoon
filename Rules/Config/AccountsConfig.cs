using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Sector account teams. The player assigns salespeople to a sector where a model of theirs is on sale. A team builds familiarity week by week (its appeal grows from a share of its full effect to all of it) and an assigned person no longer counts in the lab-wide sales force. Changing a team's size dilutes what it has built, and a person moved to another sector starts that team from nothing. The gain in a sector is capped together with any other account-level deal (Unity's enterprise seats, once merged) so the two never stack past <c>ComboCapPermille</c>. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AccountsConfig : ISync
    {
        public int Enabled;
        /// <summary>The most salespeople on one sector's team.</summary>
        public int MaxPeople;
        /// <summary>Salespeople who always stay on the lab-wide sales force: assigned people can be at most the lab's salespeople less this.</summary>
        public int KeepFree;
        /// <summary>The appeal each assigned person adds in their sector at full familiarity, in permille.</summary>
        public int PerPersonPermille;
        /// <summary>How much of that a new team has on its first day, in permille; it grows in a straight line to the full amount over <c>MaxWeeks</c>.</summary>
        public int StartPermille;
        /// <summary>Weeks of familiarity that count.</summary>
        public int MaxWeeks;
        /// <summary>The most a sector's appeal can gain from an account team and any other account-level deal together (Unity's enterprise seat deals, once merged), in permille.</summary>
        public int ComboCapPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPeople", ref MaxPeople);
            s.Int("keepFree", ref KeepFree);
            s.Int("perPersonPermille", ref PerPersonPermille);
            s.Int("startPermille", ref StartPermille);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("comboCapPermille", ref ComboCapPermille);
        }
    }
}
