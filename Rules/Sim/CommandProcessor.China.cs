using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AcceptChinaDeal(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player may answer a state request.");
            if (!lab.ChinaDealPending) return Fail(CommandError.InvalidArgument, "There is no open request from China.");
            ChinaSystem.Accept(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult DeclineChinaDeal(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player may answer a state request.");
            if (!lab.ChinaDealPending) return Fail(CommandError.InvalidArgument, "There is no open request from China.");
            ChinaSystem.Decline(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}