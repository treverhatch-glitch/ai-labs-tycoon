using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Restructuring a loan. See <see cref="RestructureConfig"/>. Only the player's lab does it.</summary>
    public static class Restructure
    {
        public static bool Enabled(GameConfig c) { return c.Restructure.Enabled > 0; }

        public static int StretchedWeeks(GameConfig c) { return (int)MathI.Max(1, (long)c.Loans.TermWeeks * c.Restructure.StretchPermille / MathI.Permille); }

        public static long Fee(GameConfig c, Loan loan) { return MathI.MulPermille(loan.Balance, c.Restructure.FeePermille); }

        /// <summary>The weekly payment the loan would have afterwards: what would be owed, spread over the longer term.</summary>
        public static long NewPayment(GameConfig c, Loan loan)
        {
            long owed = loan.Balance + Fee(c, loan);
            long weeks = StretchedWeeks(c);
            return (owed + weeks - 1) / weeks;
        }

        public static bool Worthwhile(GameConfig c, Loan loan) { return NewPayment(c, loan) < loan.Payment; }

        public static void Run(GameConfig c, GameState state, LabState lab, Loan loan)
        {
            long fee = Fee(c, loan);
            int cleared = loan.Misses;
            loan.Payment = NewPayment(c, loan);
            loan.Balance += fee;
            loan.Misses = 0;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Restructure.ReputationLoss));
            GameEvent e = Events.Add(state, EventKind.LoanRestructured, lab.Id, loan.Id, fee, "");
            e.Pick = cleared > 0 ? 1 : 0;
        }
    }
}
