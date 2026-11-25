using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetUserData(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab collects its users' data.");
            if (!UserData.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody collects users' data in this game.");
            if (cmd.Count < 0 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "The policy is none (0), opt-in (1) or on by default (2).");
            if (cmd.Count == lab.UserPolicy) return Fail(CommandError.InvalidArgument, "The lab already has that policy.");
            if (lab.UserSince > 0 && state.Tick + 1 < lab.UserSince + c.UserData.LockWeeks) return Fail(CommandError.InvalidArgument, "The policy was changed less than " + c.UserData.LockWeeks + " weeks ago.");
            UserData.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
