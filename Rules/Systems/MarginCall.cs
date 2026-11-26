using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MarginCall
    {
        public static bool Enabled(GameConfig c) { return c.MarginCall.Enabled > 0; }

        static long RequiredCollateral(GameConfig c, long owed)
        {
            return MathI.MulPermille(owed, c.MarginCall.CovenantPermille);
        }

        public static bool InBreach(GameConfig c, GameState state, LabState lab)
        {
            long owed = Loans.Outstanding(lab);
            if (owed <= 0) return false;
            long need = RequiredCollateral(c, owed);
            return Loans.Collateral(c, state, lab) < need;
        }

        /// <summary>After loan payments: notice breach or advance the cure clock.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            long owed = Loans.Outstanding(lab);
            if (owed <= 0)
            {
                lab.MarginCallLeft = 0;
                return;
            }
            if (lab.MarginCallLeft > 0)
            {
                lab.MarginCallLeft--;
                if (lab.MarginCallLeft <= 0)
                {
                    if (lab.Loans.Count > 0) Loans.Default(c, state, lab, lab.Loans[0]);
                    Events.Add(state, EventKind.MarginCallDefaulted, lab.Id, -1, owed, "");
                    lab.MarginCallLeft = 0;
                }
                return;
            }
            if (!InBreach(c, state, lab)) return;
            int baseCure = c.MarginCall.CureWeeks;
            int cure = MarginCallEraGrace.CureWeeks(c, state);
            int grace = cure - baseCure;
            lab.MarginCallLeft = cure;
            Events.Add(state, EventKind.MarginCall, lab.Id, -1, owed, "");
            MarginCallEraGrace.OnCall(c, state, lab, cure, grace);
        }

        public static long RawCashDue(GameConfig c, GameState state, LabState lab)
        {
            long owed = Loans.Outstanding(lab);
            if (owed <= 0) return 0;
            long need = RequiredCollateral(c, owed);
            long gap = need - Loans.Collateral(c, state, lab);
            if (gap <= 0) return 0;
            return MathI.MulPermille(gap, c.MarginCall.PayPermille);
        }

        public static long CashDue(GameConfig c, GameState state, LabState lab)
        {
            return MarginSatisfyEraRelief.AdjustDue(c, state, RawCashDue(c, state, lab));
        }

        public static void SatisfyPay(GameConfig c, GameState state, LabState lab, long pay)
        {
            long listDue = RawCashDue(c, state, lab);
            Finance.Spend(lab, LedgerLine.CapitalIn, pay);
            long left = pay;
            for (int i = 0; i < lab.Loans.Count && left > 0; i++)
            {
                Loan loan = lab.Loans[i];
                long take = left < loan.Balance ? left : loan.Balance;
                loan.Balance -= take;
                left -= take;
                if (loan.Balance <= 0)
                {
                    Events.Add(state, EventKind.LoanRepaid, lab.Id, loan.Id, loan.Principal, "margin");
                    lab.Loans.RemoveAt(i);
                    i--;
                }
            }
            lab.MarginCallLeft = 0;
            Events.Add(state, EventKind.MarginCallSatisfied, lab.Id, -1, pay, "cash");
            MarginSatisfyEraRelief.OnPaid(c, state, lab, listDue, pay);
        }

        public static void SatisfySell(GameConfig c, GameState state, LabState lab, int count)
        {
            if (count < 1) return;
            HardwareBatch batch = null;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.ChipDesignId >= 0 || c.Compute.Generation(b.GpuGenId) == null) continue;
                if (batch == null || Fleet.TierOf(c, b) > Fleet.TierOf(c, batch)) batch = b;
            }
            if (batch == null) return;
            int take = (int)MathI.Min(count, batch.Count);
            long each = LabQueries.ResalePerUnit(c, c.Compute.Generation(batch.GpuGenId), state.Tick);
            Fleet.Take(lab, batch, take);
            long cash = take * each;
            Finance.Earn(lab, LedgerLine.CapitalIn, cash);
            if (!InBreach(c, state, lab)) lab.MarginCallLeft = 0;
            Events.Add(state, EventKind.MarginCallSatisfied, lab.Id, take, cash, "sell");
        }
    }
}