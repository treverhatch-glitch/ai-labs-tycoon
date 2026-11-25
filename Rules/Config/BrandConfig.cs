using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The lab's name as an employer. A careers page starts a score that the lab's own people decisions move: promotions, equity, mentoring, sabbaticals and internships raise it, layoffs, resignations and strikes drop it. It cuts or raises every hire fee. A low score brings a bad review that waits for an answer. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class BrandConfig : ISync
    {
        public int Enabled;
        /// <summary>Cash to set up a careers page and start tracking the lab as an employer.</summary>
        public int OpenCost;
        /// <summary>Score the lab starts at (0 to 1000).</summary>
        public int Start;
        /// <summary>Score each promotion, equity grant, mentoring start, sabbatical or internship adds.</summary>
        public int KeptPoints;
        /// <summary>Score each layoff round, resignation or strike takes away.</summary>
        public int BrokenPoints;
        /// <summary>Hire fee cut at a score of 1000, and premium at a score of 0 (permille).</summary>
        public int MaxFeePermille;
        /// <summary>Score under which a bad review appears.</summary>
        public int LowBar;
        /// <summary>Weeks a bad review waits for an answer.</summary>
        public int ReviewWeeks;
        /// <summary>Score won back by owning up and fixing it.</summary>
        public int RepairPoints;
        /// <summary>Cash per person to fix what the review described.</summary>
        public int RepairCostPerStaff;
        /// <summary>Reputation lost for publicly rebutting it (damped).</summary>
        public int RebutReputation;
        /// <summary>Score lost when the review is left unanswered.</summary>
        public int IgnorePoints;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("openCost", ref OpenCost);
            s.Int("start", ref Start);
            s.Int("keptPoints", ref KeptPoints);
            s.Int("brokenPoints", ref BrokenPoints);
            s.Int("maxFeePermille", ref MaxFeePermille);
            s.Int("lowBar", ref LowBar);
            s.Int("reviewWeeks", ref ReviewWeeks);
            s.Int("repairPoints", ref RepairPoints);
            s.Int("repairCostPerStaff", ref RepairCostPerStaff);
            s.Int("rebutReputation", ref RebutReputation);
            s.Int("ignorePoints", ref IgnorePoints);
        }
    }
}
