using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetCapitalPolicy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a capital allowance policy.");
            if (!CapitalAllowances.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no capital allowances in this game.");
            if (cmd.Count < 0 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "The policy is 0 (expense), 1 (capitalise) or 2 (enhanced).");
            if (cmd.Count == lab.CapPolicy) return Fail(CommandError.InvalidArgument, "That policy is already in force.");
            if (CapitalAllowances.Locked(c, state, lab)) return Fail(CommandError.InvalidArgument, "The policy was changed too recently to change again.");
            CapitalAllowances.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
