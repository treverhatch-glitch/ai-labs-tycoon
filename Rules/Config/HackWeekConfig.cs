using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Hack week: the researchers drop the roadmap and build what they like. The team is happier and finds more, but the lab's
    /// training work slows while it lasts, and it cannot run again for a while. It ends by itself, and cannot overlap crunch time.
    /// Only the player's lab holds one. Zero weeks turns it off.
    /// </summary>
    public sealed class HackWeekConfig : ISync
    {
        /// <summary>How many weeks it lasts. Zero turns hack week off.</summary>
        public int Weeks;
        /// <summary>Researchers needed to hold one.</summary>
        public int MinResearchers;
        /// <summary>The lowest average morale that can hold one (they have to be able to enjoy it).</summary>
        public int MinMorale;
        /// <summary>Cash for each person on the payroll (snacks, prizes, a rented arcade).</summary>
        public long CostPerStaff;
        /// <summary>Morale every person gains when it starts, up to the scale's top.</summary>
        public int MoraleLift;
        /// <summary>What it adds to the lab's research per week while it lasts, in permille.</summary>
        public int ResearchPermille;
        /// <summary>What it takes off the lab's training speed while it lasts, in permille.</summary>
        public int TrainingCutPermille;
        /// <summary>Weeks from the start before the next one can be held.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("weeks", ref Weeks);
            s.Int("minResearchers", ref MinResearchers);
            s.Int("minMorale", ref MinMorale);
            s.Long("costPerStaff", ref CostPerStaff);
            s.Int("moraleLift", ref MoraleLift);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("trainingCutPermille", ref TrainingCutPermille);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}
