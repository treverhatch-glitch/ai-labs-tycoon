using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Debt taken off exit offers (Q464, schema 141).</summary>
    public static class ExitDebt
    {
        public static long LoanBalance(LabState lab) { return Loans.Outstanding(lab); }

        public static long NoteBalance(LabState lab) { return lab.NoteOwed; }

        public static long RbfBalance(LabState lab) { return lab.RbfOwed; }

        public static long CovenantCalls(GameConfig c, LabState lab)
        {
            if (lab.CovBreach < 1 || lab.Loans.Count < 1) return 0;
            return MathI.MulPermille(Loans.Outstanding(lab), c.Endings.ExitCovenantCallPermille);
        }

        public static long Total(GameConfig c, LabState lab)
        {
            return LoanBalance(lab) + NoteBalance(lab) + RbfBalance(lab) + CovenantCalls(c, lab);
        }
    }
}