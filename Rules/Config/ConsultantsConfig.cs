using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Policy consultants: the player hires a firm for a few weeks to stand in as policy staff when a hearing or a push on the government is
    /// coming. Each level counts as <c>StaffPerLevel</c> extra policy people for hearings, the monthly attention gain and lobbying, for a fee
    /// and a weekly bill. They never count toward the compliance office's staff cover, so they cannot be stacked with a high compliance level
    /// to get a near-zero audit fine. After the weeks run out there is a cooldown. Only the player's lab hires them. Zero levels turns it off.
    /// </summary>
    public sealed class ConsultantsConfig : ISync
    {
        /// <summary>The highest level. Zero turns it off.</summary>
        public int MaxLevel;
        /// <summary>Cash for each level, paid when the firm is hired.</summary>
        public long FeePerLevel;
        /// <summary>Cash each week for each level while the firm is in.</summary>
        public long WeeklyPerLevel;
        /// <summary>The policy people each level counts as.</summary>
        public int StaffPerLevel;
        /// <summary>How many weeks the firm stays.</summary>
        public int Weeks;
        /// <summary>Weeks from the start of one engagement before the next can start.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("feePerLevel", ref FeePerLevel);
            s.Long("weeklyPerLevel", ref WeeklyPerLevel);
            s.Int("staffPerLevel", ref StaffPerLevel);
            s.Int("weeks", ref Weeks);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}
