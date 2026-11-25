using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The public roadmap. The lab announces that it will release a model at least so much more capable than its best one so far, inside a stated number of weeks. The announcement
    /// earns reputation at once, and the level is how bold the promise is. Releasing a model that clears the bar in time pays a bigger reward; the deadline passing first costs far more
    /// than the announcement earned, so a roadmap is not free hype. A lab needs a model of its own to announce a successor to, and holds one roadmap at a time. Only the player's lab
    /// announces. Zero in <c>MaxLevel</c> turns it off.
    /// </summary>
    public sealed class RoadmapConfig : ISync
    {
        /// <summary>The boldest level; 0 turns roadmaps off.</summary>
        public int MaxLevel;
        /// <summary>The shortest and longest deadline, in weeks.</summary>
        public int MinWeeks;
        public int MaxWeeks;
        /// <summary>How much more capable than the lab's best model the promised one must be, in permille, for each level.</summary>
        public int StretchPerLevel;
        /// <summary>Reputation gained on the day of the announcement, for each level.</summary>
        public int HypePerLevel;
        /// <summary>Reputation gained when a model clears the bar in time, for each level.</summary>
        public int DeliverPerLevel;
        /// <summary>Reputation lost when the deadline passes first, for each level.</summary>
        public int MissPerLevel;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("stretchPerLevel", ref StretchPerLevel);
            s.Int("hypePerLevel", ref HypePerLevel);
            s.Int("deliverPerLevel", ref DeliverPerLevel);
            s.Int("missPerLevel", ref MissPerLevel);
        }
    }
}
