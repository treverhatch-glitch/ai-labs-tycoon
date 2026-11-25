using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lender's watch on the player's loans. See <see cref="CovenantsConfig"/>.</summary>
    public static class Covenants
    {
        public static bool Enabled(GameConfig c) { return c.Covenants.Enabled > 0; }

        /// <summary>The chips' resale value over what is owed, in permille; 0 when nothing is owed (reported as no limit by <see cref="InBreach"/>).</summary>
        public static long Cover(GameConfig c, GameState state, LabState lab)
        {
            long loans = Loans.Outstanding(lab);
            return loans <= 0 ? 0 : Loans.Collateral(c, state, lab) * MathI.Permille / (loans + lab.RbfOwed);
        }

        public static bool Waived(GameState state, LabState lab) { return state.Tick < lab.CovWaiverUntil; }

        public static bool InBreach(GameConfig c, GameState state, LabState lab)
        {
            return Enabled(c) && lab.Loans.Count > 0 && Cover(c, state, lab) < c.Covenants.MinCoverPermille;
        }

        public static long WaiverFee(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(Loans.Outstanding(lab), c.Covenants.WaiverFeePermille);
        }

        /// <summary>What must be paid down for the chips to cover what is left as the covenant needs.</summary>
        public static long CureAmount(GameConfig c, GameState state, LabState lab)
        {
            long keep = MathI.Max(0, Loans.Collateral(c, state, lab) * MathI.Permille / c.Covenants.MinCoverPermille - lab.RbfOwed);
            long pay = Loans.Outstanding(lab) - keep;
            return pay > 0 ? pay : 0;
        }

        public static void Waive(GameConfig c, GameState state, LabState lab)
        {
            long fee = WaiverFee(c, lab);
            Finance.Spend(lab, LedgerLine.Insurance, fee);
            lab.CovWaiverUntil = state.Tick + c.Covenants.WaiverWeeks;
            lab.CovWaivers++;
            lab.CovBreach = 0;
            Events.Add(state, EventKind.CovenantWaived, lab.Id, -1, fee, "");
        }

        public static void Cure(GameConfig c, GameState state, LabState lab)
        {
            long pay = CureAmount(c, state, lab);
            Finance.Spend(lab, LedgerLine.CapitalIn, pay);
            for (int i = 0; i < lab.Loans.Count && pay > 0; i++)
            {
                Loan loan = lab.Loans[i];
                long take = pay < loan.Balance ? pay : loan.Balance;
                loan.Balance -= take;
                pay -= take;
                if (loan.Balance <= 0)
                {
                    Events.Add(state, EventKind.LoanRepaid, lab.Id, loan.Id, loan.Principal, "term");
                    lab.Loans.RemoveAt(i);
                    i--;
                }
            }
            lab.CovBreach = 0;
            Events.Add(state, EventKind.CovenantCured, lab.Id, -1, Cover(c, state, lab), "").Pick = 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.Loans.Count == 0)
            {
                lab.CovBreach = 0;
                return;
            }
            if (Waived(state, lab) || !InBreach(c, state, lab))
            {
                if (lab.CovBreach > 0)
                {
                    lab.CovBreach = 0;
                    Events.Add(state, EventKind.CovenantCured, lab.Id, -1, Cover(c, state, lab), "").Pick = 0;
                }
                return;
            }
            lab.CovBreach++;
            if (lab.CovBreach == 1) Events.Add(state, EventKind.CovenantBreached, lab.Id, -1, Cover(c, state, lab), "");
            long surcharge = MathI.MulPermille(Loans.Outstanding(lab), c.Covenants.SurchargePermille);
            if (surcharge > 0 && lab.Loans.Count > 0) lab.Loans[0].Balance += surcharge;
            if (lab.CovBreach < c.Covenants.CureWeeks) return;
            long owed = Loans.Outstanding(lab);
            lab.CovCalls++;
            lab.CovBreach = 0;
            if (lab.Cash >= owed)
            {
                Finance.Spend(lab, LedgerLine.CapitalIn, owed);
                lab.Loans.Clear();
                Events.Add(state, EventKind.LoansCalled, lab.Id, -1, owed, "").Pick = 1;
                return;
            }
            Events.Add(state, EventKind.LoansCalled, lab.Id, -1, owed, "").Pick = 0;
            while (lab.Loans.Count > 0)
            {
                Loans.Default(c, state, lab, lab.Loans[0]);
                lab.Loans.RemoveAt(0);
            }
        }
    }
}
