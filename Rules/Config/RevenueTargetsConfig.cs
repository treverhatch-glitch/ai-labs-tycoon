using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Board targets. The player can promise the board a revenue number for the next quarter. The goal is the last week's revenue times the weeks,
    /// stretched by the level the player picks. At the end of the quarter the lab is judged on what it actually made: a met target earns reputation and a
    /// missed one costs more. The player decides how hard to push, and nothing is judged until a target is set. Zero in <c>MaxLevel</c> turns it off.
    /// </summary>
    public sealed class RevenueTargetsConfig : ISync
    {
        /// <summary>The hardest level that can be set; 0 turns targets off.</summary>
        public int MaxLevel;
        /// <summary>The weeks of revenue the target covers.</summary>
        public int Weeks;
        /// <summary>For each level, how far above the last week's pace the goal sits, in permille.</summary>
        public int StretchPerLevel;
        /// <summary>The weekly revenue the lab needs before it can set a target, so there is something to measure against.</summary>
        public long MinWeeklyRevenue;
        /// <summary>Reputation gained for each level when the target is met.</summary>
        public int RewardPerLevel;
        /// <summary>Reputation lost for each level when it is missed.</summary>
        public int PenaltyPerLevel;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("weeks", ref Weeks);
            s.Int("stretchPerLevel", ref StretchPerLevel);
            s.Long("minWeeklyRevenue", ref MinWeeklyRevenue);
            s.Int("rewardPerLevel", ref RewardPerLevel);
            s.Int("penaltyPerLevel", ref PenaltyPerLevel);
        }
    }
}
