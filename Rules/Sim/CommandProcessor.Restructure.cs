using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RestructureLoan(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has loans.");
            if (!Restructure.Enabled(c)) return Fail(CommandError.InvalidArgument, "Lenders do not restructure loans in this game.");
            Loan loan = Loans.Find(lab, cmd.TargetId);
            if (loan == null) return Fail(CommandError.UnknownId, "The lab has no loan " + cmd.TargetId + ".");
            if (!Restructure.Worthwhile(c, loan))
                return Fail(CommandError.InvalidArgument, "Stretching loan " + loan.Id + " would not lower its payment of $" + loan.Payment + " a week.");
            Restructure.Run(c, state, lab, loan);
            return CommandResult.Done(-1);
        }
    }
}
