using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>How the player's hardware and building spend is deducted for tax. See <see cref="AllowancesConfig"/>.</summary>
    public static class CapitalAllowances
    {
        public const int Expense = 0;
        public const int Capitalise = 1;
        public const int Enhanced = 2;

        public static bool Enabled(GameConfig c) { return c.Allowances.Enabled > 0 && Tax.Enabled(c); }

        public static bool Locked(GameConfig c, GameState state, LabState lab)
        {
            return lab.CapSwitchTick > 0 && state.Tick < lab.CapSwitchTick + c.Allowances.LockWeeks;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int policy)
        {
            long flushed = policy == Expense ? lab.CapPool : 0;
            lab.CapPolicy = policy;
            lab.CapSwitchTick = state.Tick + 1;
            Events.Add(state, EventKind.CapitalPolicySet, lab.Id, (int)MathI.Min(flushed, int.MaxValue), policy, "").Pick = policy;
        }

        /// <summary>What to add to the week's taxable profit: the spend added back and the write-off taken off, or the whole pool taken off when expensing.</summary>
        public static long Adjust(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c)) return 0;
            LedgerTotals w = lab.Ledger.LastWeek;
            long spend = MathI.Max(0, -(w.Hardware + w.Construction));
            if (lab.CapPolicy == Expense)
            {
                long all = lab.CapPool;
                lab.CapPool = 0;
                lab.CapWritten += all;
                return -all;
            }
            long extra = lab.CapPolicy == Enhanced ? MathI.MulPermille(spend, c.Allowances.EnhancedPermille) : 0;
            lab.CapPool += spend + extra;
            long off = MathI.MulPermille(lab.CapPool, c.Allowances.WriteOffPermille);
            lab.CapPool -= off;
            lab.CapWritten += off;
            if (extra > 0)
            {
                lab.TaxSaved += MathI.MulPermille(extra, c.Tax.RatePermille);
                lab.TaxExposure += c.Allowances.EnhancedExposure;
            }
            return spend - off;
        }
    }
}
