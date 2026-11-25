using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Orchestration budget (bible ch. 6): how many agent-level models the lab can run at full orchestration lift. Overspending accrues overrun that delays revenue and feeds the human queue. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class OrchestrationBudgetConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for orchestration budgets.</summary>
        public int MinEra;
        /// <summary>Cash to raise the budget one level.</summary>
        public int SetupPerLevel;
        /// <summary>Agent-level models the budget allows at levels 1, 2 and 3.</summary>
        public int Slots1;
        public int Slots2;
        public int Slots3;
        /// <summary>Share of agent revenue kept at levels 1, 2 and 3 (permille).</summary>
        public int Keep1;
        public int Keep2;
        public int Keep3;
        /// <summary>Overrun points lost each week under budget.</summary>
        public int OverDecay;
        /// <summary>Human queue items added each week per overrun point.</summary>
        public int QueuePerOver;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("setupPerLevel", ref SetupPerLevel);
            s.Int("slots1", ref Slots1);
            s.Int("slots2", ref Slots2);
            s.Int("slots3", ref Slots3);
            s.Int("keep1", ref Keep1);
            s.Int("keep2", ref Keep2);
            s.Int("keep3", ref Keep3);
            s.Int("overDecay", ref OverDecay);
            s.Int("queuePerOver", ref QueuePerOver);
        }
    }
}