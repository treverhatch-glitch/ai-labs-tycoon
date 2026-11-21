using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Mentoring. The player pairs a senior with a junior in the same function. For the weeks it lasts the mentor stops counting for more than a plain person and the junior counts for a bit
    /// less (they are learning), while the junior's morale climbs. If the pair holds for <c>Weeks</c> and the function has a senior seat free, the junior is promoted for nothing, which
    /// a promotion with the fee does not offer, but the lab carries the lost output the whole time. A pair breaks up when either person leaves, the mentor stops being senior, the junior
    /// is promoted some other way or the two stop sharing a function. Only the player's lab pairs people. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class MentoringConfig : ISync
    {
        public int Enabled;
        /// <summary>The most pairs the lab can run at once.</summary>
        public int MaxPairs;
        /// <summary>Weeks a pair has to hold before the junior graduates.</summary>
        public int Weeks;
        /// <summary>What a junior in a pair counts for while it lasts, in permille of a whole person.</summary>
        public int MenteeWeightPermille;
        /// <summary>Morale the junior gains each week, until it reaches the ceiling.</summary>
        public int MoralePerWeek;
        public int MoraleCeiling;
        /// <summary>Morale the junior gains on graduating.</summary>
        public int GraduateMorale;
        /// <summary>Morale the mentor gains when the junior graduates.</summary>
        public int MentorMorale;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPairs", ref MaxPairs);
            s.Int("weeks", ref Weeks);
            s.Int("menteeWeightPermille", ref MenteeWeightPermille);
            s.Int("moralePerWeek", ref MoralePerWeek);
            s.Int("moraleCeiling", ref MoraleCeiling);
            s.Int("graduateMorale", ref GraduateMorale);
            s.Int("mentorMorale", ref MentorMorale);
        }
    }
}
