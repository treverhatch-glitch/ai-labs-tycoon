using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Loans against the lab's chips (bible ch. 9, the funding ladder: "Debt: loans against your chips. Repayment on schedule, whatever the chip is
    /// worth by then"). The player can borrow up to a share of what the GPUs they own would fetch today, and pays it back weekly. The lender does not
    /// care that chips lose value; a lab that falls behind loses the chips. Only the player takes loans.
    /// </summary>
    public sealed class LoansConfig : ISync
    {
        /// <summary>The most the lab can owe in all, as a share of what its owned GPUs would fetch today, in permille.</summary>
        public int LoanToValuePermille;
        /// <summary>Weeks a loan is paid back over.</summary>
        public int TermWeeks;
        /// <summary>The interest, added once at the start, in permille of the amount borrowed.</summary>
        public int InterestPermille;
        /// <summary>The smallest amount that can be borrowed, in dollars.</summary>
        public long MinPrincipal;
        /// <summary>How many loans can be open at once.</summary>
        public int MaxOpen;
        /// <summary>A missed payment adds this share of itself to what is owed, in permille.</summary>
        public int MissPenaltyPermille;
        /// <summary>This many missed payments in a row and the lender takes the chips.</summary>
        public int DefaultAfterMisses;

        public void Sync(Syncer s)
        {
            s.Int("loanToValuePermille", ref LoanToValuePermille);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("interestPermille", ref InterestPermille);
            s.Long("minPrincipal", ref MinPrincipal);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("missPenaltyPermille", ref MissPenaltyPermille);
            s.Int("defaultAfterMisses", ref DefaultAfterMisses);
        }
    }
}
