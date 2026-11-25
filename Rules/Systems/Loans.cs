using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Loans against chips. See <see cref="LoansConfig"/>. Proceeds and payments are on the capital line of the books.</summary>
    public static class Loans
    {
        public static Loan Find(LabState lab, int id)
        {
            for (int i = 0; i < lab.Loans.Count; i++) if (lab.Loans[i].Id == id) return lab.Loans[i];
            return null;
        }

        /// <summary>What the lab's owned GPUs would fetch today. Chips of its own design have no market and do not count.</summary>
        public static long Collateral(GameConfig c, GameState state, LabState lab)
        {
            return MathI.Max(0, LabQueries.FleetResaleValue(c, state, lab) - lab.LeaseValue);
        }

        public static long Outstanding(LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Loans.Count; i++) total += lab.Loans[i].Balance;
            return total;
        }

        /// <summary>The most more the lab could borrow now: its share of the collateral less everything it still owes.</summary>
        public static long Capacity(GameConfig c, GameState state, LabState lab)
        {
            long room = MathI.MulPermille(Collateral(c, state, lab), c.Loans.LoanToValuePermille) - Outstanding(lab);
            return room > 0 ? room : 0;
        }

        /// <summary>What a loan of this principal adds up to with its interest.</summary>
        public static long Owed(GameConfig c, long principal)
        {
            return principal + MathI.MulPermille(principal, c.Loans.InterestPermille);
        }

        public static long PaymentFor(GameConfig c, long owed)
        {
            long weeks = c.Loans.TermWeeks;
            return (owed + weeks - 1) / weeks;
        }

        public static Loan Take(GameConfig c, GameState state, LabState lab, long principal)
        {
            var loan = new Loan();
            loan.Id = state.TakeId();
            loan.Principal = principal;
            loan.Balance = Owed(c, principal);
            loan.Payment = PaymentFor(c, loan.Balance);
            loan.StartTick = state.Tick;
            lab.Loans.Add(loan);
            Finance.Earn(lab, LedgerLine.CapitalIn, principal);
            Events.Add(state, EventKind.LoanTaken, lab.Id, loan.Id, principal, "");
            return loan;
        }

        /// <summary>Pays what is owed on one loan in one go, and closes it.</summary>
        public static void Repay(GameState state, LabState lab, Loan loan)
        {
            Finance.Spend(lab, LedgerLine.CapitalIn, loan.Balance);
            Events.Add(state, EventKind.LoanRepaid, lab.Id, loan.Id, loan.Balance, "early");
            lab.Loans.Remove(loan);
        }

        /// <summary>The weekly payments. Run after the week's bills, so a lab short of cash misses them.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.Loans.Count; i++)
            {
                Loan loan = lab.Loans[i];
                long pay = loan.Payment < loan.Balance ? loan.Payment : loan.Balance;
                if (lab.Cash >= pay)
                {
                    Finance.Spend(lab, LedgerLine.CapitalIn, pay);
                    loan.Balance -= pay;
                    loan.Misses = 0;
                    if (loan.Balance <= 0)
                    {
                        Events.Add(state, EventKind.LoanRepaid, lab.Id, loan.Id, loan.Principal, "term");
                        lab.Loans.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                loan.Misses++;
                loan.Balance += MathI.MulPermille(pay, c.Loans.MissPenaltyPermille);
                if (loan.Misses == 1) Events.Add(state, EventKind.LoanMissed, lab.Id, loan.Id, pay, "");
                if (loan.Misses >= c.Loans.DefaultAfterMisses)
                {
                    Default(c, state, lab, loan);
                    lab.Loans.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>The lender takes the most valuable chips first until what they would fetch covers what is owed; whatever is still owed is written off.</summary>
        public static void Default(GameConfig c, GameState state, LabState lab, Loan loan)
        {
            var batches = new List<HardwareBatch>();
            for (int i = 0; i < lab.Fleet.Count; i++)
                if (lab.Fleet[i].ChipDesignId < 0 && c.Compute.Generation(lab.Fleet[i].GpuGenId) != null) batches.Add(lab.Fleet[i]);
            batches.Sort((a, b) =>
            {
                int byTier = Fleet.TierOf(c, b).CompareTo(Fleet.TierOf(c, a));
                return byTier != 0 ? byTier : a.Id.CompareTo(b.Id);
            });
            long owed = loan.Balance;
            long units = 0;
            for (int i = 0; i < batches.Count && owed > 0; i++)
            {
                long each = LabQueries.ResalePerUnit(c, c.Compute.Generation(batches[i].GpuGenId), state.Tick);
                if (each <= 0) continue;
                long need = (owed + each - 1) / each;
                int take = (int)MathI.Min(need, batches[i].Count);
                owed -= take * each;
                units += take;
                Fleet.Take(lab, batches[i], take);
            }
            Events.Add(state, EventKind.LoanDefaulted, lab.Id, (int)units, owed > 0 ? owed : 0, loan.Id.ToString());
        }

        public static void OnBankrupt(LabState lab) { lab.Loans.Clear(); }
    }
}
