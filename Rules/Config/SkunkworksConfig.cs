using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Skunkworks. The player pulls a team out of one function (research, safety or engineering) to work on a single project for a set number of weeks. The people are away the whole
    /// time: they stop counting in their function's output, and they cost an up-front budget a head and week. What comes back depends on the function: research points, real alignment
    /// on the lab's finished models, or less technical debt. It is paid out for the person-weeks actually worked, so losing people mid-project shrinks it. A long project pays extra; wrapping
    /// up early pays a smaller share of what was done so far. Only the player's lab runs a project, one at a time. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SkunkworksConfig : ISync
    {
        public int Enabled;
        /// <summary>The fewest people on a team.</summary>
        public int MinPeople;
        /// <summary>The most of a function the team can take, in permille of its people, rounded down. At least one person always stays behind.</summary>
        public int MaxSharePermille;
        /// <summary>The shortest and longest project, in weeks.</summary>
        public int MinWeeks;
        public int MaxWeeks;
        /// <summary>A project of at least this many weeks pays <c>LongBonusPermille</c> more.</summary>
        public int LongWeeks;
        public int LongBonusPermille;
        /// <summary>The share of the work done so far that wrapping up early pays, in permille, with no long bonus.</summary>
        public int EarlyPermille;
        /// <summary>The budget a person-week of the project costs, paid when it starts.</summary>
        public int CostPerPersonWeek;
        /// <summary>Weeks after a project ends before the next can start.</summary>
        public int LockWeeks;
        /// <summary>Research: points paid for a person-week, in permille of what a researcher normally makes in a week.</summary>
        public int ResearchPayoffPermille;
        /// <summary>Safety: alignment gained for every 100 person-weeks, in permille of alignment, and the most a project can add.</summary>
        public int AlignmentPerHundred;
        public int MaxAlignment;
        /// <summary>Engineering: technical debt points cleared for every person-week.</summary>
        public int DebtPerPersonWeek;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minPeople", ref MinPeople);
            s.Int("maxSharePermille", ref MaxSharePermille);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("longWeeks", ref LongWeeks);
            s.Int("longBonusPermille", ref LongBonusPermille);
            s.Int("earlyPermille", ref EarlyPermille);
            s.Int("costPerPersonWeek", ref CostPerPersonWeek);
            s.Int("lockWeeks", ref LockWeeks);
            s.Int("researchPayoffPermille", ref ResearchPayoffPermille);
            s.Int("alignmentPerHundred", ref AlignmentPerHundred);
            s.Int("maxAlignment", ref MaxAlignment);
            s.Int("debtPerPersonWeek", ref DebtPerPersonWeek);
        }
    }
}
