using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult HireConsultants(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab hires policy consultants.");
            if (!PolicyConsultants.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no policy consultants in this game.");
            if (cmd.Count < 1 || cmd.Count > c.Consultants.MaxLevel) return Fail(CommandError.InvalidArgument, "Consultants are from level 1 to " + c.Consultants.MaxLevel + ".");
            if (PolicyConsultants.Running(lab)) return Fail(CommandError.InvalidArgument, "A firm of consultants is already in.");
            if (PolicyConsultants.Locked(state, lab)) return Fail(CommandError.ConsultantsLocked, "The last firm only just left; the next can start at week " + lab.ConsultantLockedUntil + ".");
            long fee = PolicyConsultants.Fee(c, cmd.Count);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            PolicyConsultants.Start(c, state, lab, cmd.Count, fee);
            return CommandResult.Done(-1);
        }
    }
}
