using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult TakeLoan(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab borrows against chips.");
            if (cmd.Permille < 1 || cmd.Permille > MathI.Permille) return Fail(CommandError.InvalidArgument, "Borrow from 1 to 1000 permille of what the chips allow.");
            if (lab.Loans.Count >= c.Loans.MaxOpen) return Fail(CommandError.LoanLimit, "The lab already has " + c.Loans.MaxOpen + " loans open.");
            long principal = MathI.MulPermille(Loans.Capacity(c, state, lab), cmd.Permille);
            if (principal < c.Loans.MinPrincipal) return Fail(CommandError.LoanLimit, "The chips back less than the smallest loan ($" + c.Loans.MinPrincipal + ").");
            Loan loan = Loans.Take(c, state, lab, principal);
            return CommandResult.Done(loan.Id);
        }

        static CommandResult RepayLoan(GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has loans.");
            Loan loan = Loans.Find(lab, cmd.TargetId);
            if (loan == null) return Fail(CommandError.UnknownId, "The lab has no loan " + cmd.TargetId + ".");
            if (lab.Cash < loan.Balance) return NeedCash(lab, loan.Balance);
            Loans.Repay(state, lab, loan);
            return CommandResult.Done(-1);
        }
    }
}
