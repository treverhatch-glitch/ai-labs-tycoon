using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The whistleblower. When a released model of the player's lab shows more alignment than it has, and the people who would know (the safety staff)
    /// are unhappy, one of them eventually raises it. The player gets a few weeks to answer: come clean, or pay for silence. An unanswered concern
    /// leaks to the press, and so does every silence once it is spent. Nothing here draws a random number. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class WhistleConfig : ISync
    {
        /// <summary>1 when whistleblowers exist, 0 when they do not.</summary>
        public int Enabled;
        /// <summary>The function whose people notice, and who one of them is when somebody resigns.</summary>
        public string RoleId = "";
        /// <summary>How far the shown alignment must stand above the real one, in permille, for a model to worry anybody.</summary>
        public int GapPermille;
        /// <summary>A person of that function at or below this morale is unhappy enough to speak up.</summary>
        public int MoraleBelow;
        /// <summary>Weeks the gap and an unhappy person must last (a good week takes one back off) before the concern is raised.</summary>
        public int WeeksToConcern;
        /// <summary>Weeks the player has to answer before it leaks.</summary>
        public int AnswerWeeks;
        /// <summary>Reputation lost by coming clean, before press damping.</summary>
        public int CleanReputation;
        /// <summary>Morale every person of the function gains for being listened to.</summary>
        public int CleanMorale;
        /// <summary>What the first silence costs. The next costs twice that, then three times, until the story leaks.</summary>
        public long SilenceFee;
        /// <summary>Morale every person of the function loses when the concern is bought off.</summary>
        public int SilenceMorale;
        /// <summary>Reputation lost when it leaks, before press damping.</summary>
        public int LeakReputation;
        /// <summary>Extra reputation lost for each silence that was bought before it leaked.</summary>
        public int LeakPerSilence;
        /// <summary>Morale every person left of the function loses when it leaks.</summary>
        public int LeakMorale;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Str("roleId", ref RoleId);
            s.Int("gapPermille", ref GapPermille);
            s.Int("moraleBelow", ref MoraleBelow);
            s.Int("weeksToConcern", ref WeeksToConcern);
            s.Int("answerWeeks", ref AnswerWeeks);
            s.Int("cleanReputation", ref CleanReputation);
            s.Int("cleanMorale", ref CleanMorale);
            s.Long("silenceFee", ref SilenceFee);
            s.Int("silenceMorale", ref SilenceMorale);
            s.Int("leakReputation", ref LeakReputation);
            s.Int("leakPerSilence", ref LeakPerSilence);
            s.Int("leakMorale", ref LeakMorale);
        }
    }
}
