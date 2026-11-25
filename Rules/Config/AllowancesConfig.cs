using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Capital allowances. By default the player's hardware and building spend is deducted from the tax base the week it is spent. The lab can capitalise it instead: spend goes into a pool that is written off a share a week, which smooths the base, or claim an enhanced allowance that deducts more than was spent and so saves tax, at the cost of audit exposure. Changing the policy is locked for a time; going back to expensing writes the whole pool off at once. Needs the tax to be on. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AllowancesConfig : ISync
    {
        public int Enabled;
        /// <summary>Share of the pool written off each week, in permille.</summary>
        public int WriteOffPermille;
        /// <summary>What an enhanced allowance adds on top of the spend, in permille of it.</summary>
        public int EnhancedPermille;
        /// <summary>Audit exposure added in each week the enhanced allowance claims something extra.</summary>
        public int EnhancedExposure;
        /// <summary>Weeks after a change of policy before it can change again.</summary>
        public int LockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("writeOffPermille", ref WriteOffPermille);
            s.Int("enhancedPermille", ref EnhancedPermille);
            s.Int("enhancedExposure", ref EnhancedExposure);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }
}
