using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Public post-mortems: after a model has an incident the player can publish a write-up within a few weeks, once for each incident. A
    /// summary wins back part of the reputation the incident cost. A full write-up wins back more and also closes the gap between the model's
    /// displayed and true alignment (the public learns the real number), which costs the model the appeal the gap was borrowing. It needs a
    /// safety person to write it. Only the player's lab does it. Zero window turns it off.
    /// </summary>
    public sealed class PostMortemConfig : ISync
    {
        /// <summary>Weeks after an incident in which a post-mortem can be published. Zero turns it off.</summary>
        public int WindowWeeks;
        /// <summary>Cash for a summary.</summary>
        public long SummaryCost;
        /// <summary>Cash for a full write-up.</summary>
        public long FullCost;
        /// <summary>The share of the reputation the incident cost that a summary wins back, in permille.</summary>
        public int SummaryRecoverPermille;
        /// <summary>The share of the reputation the incident cost that a full write-up wins back, in permille.</summary>
        public int FullRecoverPermille;
        /// <summary>The safety people the lab needs to write one.</summary>
        public int MinSafetyStaff;

        public void Sync(Syncer s)
        {
            s.Int("windowWeeks", ref WindowWeeks);
            s.Long("summaryCost", ref SummaryCost);
            s.Long("fullCost", ref FullCost);
            s.Int("summaryRecoverPermille", ref SummaryRecoverPermille);
            s.Int("fullRecoverPermille", ref FullRecoverPermille);
            s.Int("minSafetyStaff", ref MinSafetyStaff);
        }
    }
}
